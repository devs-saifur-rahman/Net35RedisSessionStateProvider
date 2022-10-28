namespace RedisSessionProvider.redisWrapper.commands
{
    public class DumpCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "DUMP";
            }
        }

        public string Key { get; set; }

        public string GetCommand()
        {
            return $"DUMP {Key}";
        }
    }

}
