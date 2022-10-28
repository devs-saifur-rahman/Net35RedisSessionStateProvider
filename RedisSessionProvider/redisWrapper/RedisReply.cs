namespace RedisSessionProvider.redisWrapper
{

    internal enum RESPType
    {
        Array,
        BulkString,
        Error,
        Integer,
        SimpleString
    }

    internal struct RedisReply
    {
        internal RESPType Type { get; set; }

        internal object Value { get; set; }

        internal RedisReply(RESPType type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}
