using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedisSessionProvider
{
    public interface IRedis
    {
        TimeSpan Ping(CommandFlags flags = CommandFlags.None);
    }
    public interface IDatabase : IRedis
    {
        /// <summary>
        /// The numeric identifier of this database
        /// </summary>
        int Database { get; }

        RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None);

        RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry, When when);


        bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None);
        bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None);

        bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags);
        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags);

        bool KeyExpire(RedisKey key, TimeSpan? expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None);



        RedisResult ScriptEvaluate(string script, RedisKey[]? keys = null, RedisValue[]? values = null, CommandFlags flags = CommandFlags.None);


    }
}
