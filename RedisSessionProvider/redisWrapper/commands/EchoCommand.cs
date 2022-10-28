namespace RedisSessionProvider.redisWrapper.commands
{

    public class EchoCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "ECHO";
            }
        }

        public string Message { get; set; }

        public string GetCommand()
        {
            return $"ECHO {Message}";
        }
    }

}
