using System;

namespace RedisSessionProvider.redisWrapper
{
    public class RedisException : ApplicationException
    {
        public RedisException(string message) : base(message)
        {
        }
    }

}
