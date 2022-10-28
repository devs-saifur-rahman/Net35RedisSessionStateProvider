using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;

namespace RedisSessionProvider.redisWrapper
{
    internal class RedisClientConnection : IRedisClientConnection
    {
        private RedisClient redisClient;
        private ProviderConfiguration providerConfiguration;
        public RedisClientConnection()
        {

        }
        public RedisClientConnection(ProviderConfiguration _providerConfiguration)
        {
            providerConfiguration = _providerConfiguration;
            redisClient = new RedisClient(providerConfiguration.Host, providerConfiguration.Port);
        }


        public void Close()
        {
            
          //  throw new NotImplementedException();
        }

        public object Eval(string script, string[] keyArgs, object[] valueArgs)
        {
            throw new NotImplementedException();
        }

        public bool Expiry(string key, int timeInSeconds)
        {
            throw new NotImplementedException();
        }

        public byte[] Get(string key)
        {
            return Encoding.ASCII.GetBytes(redisClient.Get(key));
        }

        public string GetLockId(object rowDataFromRedis)
        {
            throw new NotImplementedException();
        }

        public ISessionStateItemCollection GetSessionData(object rowDataFromRedis)
        {
            throw new NotImplementedException();
        }

        public int GetSessionTimeout(object rowDataFromRedis)
        {
            throw new NotImplementedException();
        }

        public bool IsLocked(object rowDataFromRedis)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] data, DateTime utcExpiry)
        {
            
            TimeSpan timeSpanForExpiry = utcExpiry - DateTime.UtcNow;
            redisClient.Set(key, data, timeSpanForExpiry);

        }
    }
}
