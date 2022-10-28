namespace RedisSessionProvider.redisWrapper.commands
{
    public class PingCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "PING";
            }
        }

        public string GetCommand()
        {
            return "PING";
        }
    }
}
