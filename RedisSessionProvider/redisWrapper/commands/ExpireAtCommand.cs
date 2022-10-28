namespace RedisSessionProvider.redisWrapper.commands
{
   public class ExpireAtCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "EXPIREAT";
            }
        }

        public string Key { get; set; }

        public long TTL { get; set; }

        public string GetCommand()
        {
            return $"EXPIREAT {Key} {TTL}";
        }
    }

}
