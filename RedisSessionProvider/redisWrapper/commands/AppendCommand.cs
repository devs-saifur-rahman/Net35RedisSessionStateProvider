namespace RedisSessionProvider.redisWrapper.commands
{
    namespace Commands
    {
        public class AppendCommand : IRedisCommand
        {
            public string Name
            {
                get
                {
                    return "APPEND";
                }
            }

            public string Key { get; set; }

            public string Value { get; set; }

            public string GetCommand()
            {
                return $"APPEND {Key} {Value}";
            }
        }
    }

}
