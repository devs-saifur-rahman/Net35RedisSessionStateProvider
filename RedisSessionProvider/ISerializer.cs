namespace RedisSessionProvider
{
    public interface ISerializer
    {
        byte[] Serialize(object data);
        object Deserialize(byte[] data);
    }
}
