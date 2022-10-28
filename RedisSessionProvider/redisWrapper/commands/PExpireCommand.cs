namespace RedisSessionProvider.redisWrapper.commands
{
    public class PExpireCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "PEXPIRE";
            }
        }

        public string Key { get; set; }

        public int Timeout { get; set; }

        public string GetCommand()
        {
            return $"PEXPIRE {Key} {Timeout}";
        }
    }

}
