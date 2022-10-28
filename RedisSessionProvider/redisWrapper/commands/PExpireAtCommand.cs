namespace RedisSessionProvider.redisWrapper.commands
{
    public class PExpireAtCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "PEXPIREAT";
            }
        }

        public string Key { get; set; }

        public long TTL { get; set; }

        public string GetCommand()
        {
            return $"PEXPIREAT {Key} {TTL}";
        }
    }

}
