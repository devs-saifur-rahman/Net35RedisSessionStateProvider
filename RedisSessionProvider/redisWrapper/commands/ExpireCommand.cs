namespace RedisSessionProvider.redisWrapper.commands
{
    public class ExpireCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "EXPIRE";
            }
        }

        public string Key { get; set; }

        public int Timeout { get; set; }

        public string GetCommand()
        {
            return $"EXPIRE {Key} {Timeout}";
        }
    }
}
