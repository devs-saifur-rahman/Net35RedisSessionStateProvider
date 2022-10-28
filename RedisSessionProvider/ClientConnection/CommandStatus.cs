namespace RedisSessionProvider
{
    public enum CommandStatus
    {
        /// <summary>
        /// Command status unknown.
        /// </summary>
        Unknown,
        /// <summary>
        /// ConnectionMultiplexer has not yet started writing this command to redis.
        /// </summary>
        WaitingToBeSent,
        /// <summary>
        /// Command has been sent to Redis.
        /// </summary>
        Sent,
    }
}