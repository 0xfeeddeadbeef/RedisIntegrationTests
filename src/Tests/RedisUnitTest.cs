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

namespace RedisIntegrationTests;

[Collection("Redis")]
public class RedisUnitTest
{
    private readonly RedisFixture _redis;

    public RedisUnitTest(RedisFixture redis)
    {
        ArgumentNullException.ThrowIfNull(redis);
        _redis = redis;
    }

    [Fact]
    public void Test1()
    {
        var db = _redis.Connection!.GetDatabase();
        var wasSet = db.StringSet("HELLO", "WORLD", expiry: TimeSpan.FromMinutes(5d));
        Assert.True(wasSet);
    }

    [Fact]
    public async Task Test2()
    {
        var db = _redis.Connection!.GetDatabase();
        var wasSet = await db.StringSetAsync("BYE", "WORLD", expiry: TimeSpan.FromMinutes(5d));
        Assert.True(wasSet);
    }

    [Fact]
    public async Task Test3()
    {
        var db = _redis.Connection!.GetDatabase();
        var wasSet = await db.StringSetAsync("KEY1", "VALUE1", expiry: TimeSpan.FromMinutes(5d));
        Assert.True(wasSet);
        Assert.Equal("VALUE1", (string?)await db.StringGetAsync("KEY1"));
    }
}
