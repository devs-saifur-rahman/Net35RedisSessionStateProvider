namespace RedisSessionProvider.redisWrapper
{
    public interface IRedisCommand
    {
        string Name { get; }

        string GetCommand();
    }

}
