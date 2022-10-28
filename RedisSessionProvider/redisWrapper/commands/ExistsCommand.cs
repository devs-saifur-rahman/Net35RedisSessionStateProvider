namespace RedisSessionProvider.redisWrapper.commands
{
    public class ExistsCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "EXISTS";
            }
        }

        public string Key { get; set; }

        public string GetCommand()
        {
            return $"EXISTS {Key}";
        }
    }
}