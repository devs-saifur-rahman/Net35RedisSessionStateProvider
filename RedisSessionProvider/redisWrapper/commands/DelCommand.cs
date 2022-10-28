namespace RedisSessionProvider.redisWrapper.commands
{

    public class DelCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "DEL";
            }
        }

        public string[] Keys { get; set; }

        public string GetCommand()
        {
            return $"DEL {string.Join(" ", Keys)}";
        }
    }

}
