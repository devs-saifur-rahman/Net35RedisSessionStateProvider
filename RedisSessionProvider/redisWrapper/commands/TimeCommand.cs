
namespace RedisSessionProvider.redisWrapper.commands
{
    public class TimeCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "TIME";
            }
        }

        public string GetCommand()
        {
            return "TIME";
        }
    }
}
