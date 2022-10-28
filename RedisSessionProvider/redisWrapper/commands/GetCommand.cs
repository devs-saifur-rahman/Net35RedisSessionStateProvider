namespace RedisSessionProvider.redisWrapper.commands
{
    public class GetCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "GET";
            }
        }

        public string Key { get; set; }

        string IRedisCommand.GetCommand()
        {
            return $"GET {Key}";
        }
    }

}
