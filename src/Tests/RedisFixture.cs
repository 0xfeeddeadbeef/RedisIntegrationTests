/*
 * Copyright 2023 Giorgi Chakhidze
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the “Software”), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 *
 */

using StackExchange.Redis;
using Testcontainers.Redis;

namespace RedisIntegrationTests;

public class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _container;

    public RedisFixture()
    {
        _container = new RedisBuilder()
            .WithImage("redis:6.2")
            .WithCleanUp(true)
            .WithExposedPort(RedisBuilder.RedisPort)
            .WithPortBinding(RedisBuilder.RedisPort, assignRandomHostPort: true)
            .Build();
    }

    public IConnectionMultiplexer? Connection
    {
        get; private set;
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        Connection = await ConnectionMultiplexer.ConnectAsync(_container.GetConnectionString()).ConfigureAwait(false);
        _ = await Connection.GetDatabase().ExecuteAsync("FLUSHALL", "SYNC").ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        if (Connection is not null)
        {
            try
            {
                _ = await Connection.GetDatabase().ExecuteAsync("SHUTDOWN", "NOSAVE", "NOW", "FORCE").ConfigureAwait(false);
            }
            catch
            {
                // Redis is shutdown immediately and connection cuts short; catch and ignore
            }

            await Connection.DisposeAsync().ConfigureAwait(false);
        }

        await _container.DisposeAsync().ConfigureAwait(false);
    }
}
