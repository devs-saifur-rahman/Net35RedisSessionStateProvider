using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Management;
using System.Web.SessionState;

namespace RedisSessionProvider.redisWrapper
{
    internal class RedisClientConnection : IRedisClientConnection
    {
        private RedisClient redisClient;
        private ProviderConfiguration providerConfiguration;
        private RedisUtility redisUtility;
        public RedisClientConnection()
        {

        }
        public RedisClientConnection(ProviderConfiguration _providerConfiguration)
        {
            providerConfiguration = _providerConfiguration;
            redisUtility = new RedisUtility(providerConfiguration);
            redisClient = new RedisClient(providerConfiguration.Host, providerConfiguration.Port);
        }
        public void Open()
        {

            redisClient.Connect();
            //throw new NotImplementedException();
        }

        public void Close()
        {
            redisClient.Close();
            //  throw new NotImplementedException();
        }

        public void Set(string key, byte[] data, DateTime utcExpiry)
        {

            TimeSpan timeSpanForExpiry = utcExpiry - DateTime.UtcNow;
            redisClient.Set(key, data, timeSpanForExpiry);

        }

        public byte[] Get(string key)
        {
            return Encoding.ASCII.GetBytes(redisClient.Get(key));
        }

        public void Remove(string key)
        {
            redisClient.Del(key);
            //redisClient.Dump(key);
            //throw new NotImplementedException();
        }
        public bool Expiry(string key, int timeInSeconds)
        {
            // TimeSpan timeSpan = new TimeSpan(0, 0, timeInSeconds);

            redisClient.Expire(key, timeInSeconds);
            throw new NotImplementedException();
        }

        /// TO BE DONE
        public object Eval(string script, string[] keyArgs, object[] valueArgs)
        {
            //throw new NotImplementedException();

            string[] redisKeyArgs = new string[keyArgs.Length];
            object[] redisValueArgs = new object[valueArgs.Length];

            int i = 0;
            foreach (string key in keyArgs)
            {
                redisKeyArgs[i] = key;
                i++;
            }

            i = 0;
            foreach (object val in valueArgs)
            {
                if (val.GetType() == typeof(byte[]))
                {
                    // User data is always in bytes
                    redisValueArgs[i] = (byte[])val;
                }
                else
                {
                    // Internal data like session timeout and indexes are stored as strings
                    redisValueArgs[i] = val.ToString();
                }
                i++;
            }

            return RetryLogic(redisOperation: () => redisClient.ScriptEvaluate(script, redisKeyArgs, redisValueArgs));
        }





        public string GetLockId(object rowDataFromRedis)
        {
            //   throw new NotImplementedException();

            return rowDataFromRedis.ToString();
        }

        public ISessionStateItemCollection GetSessionData(object rowDataFromRedis)
        {
            //  throw new NotImplementedException();

            object rowDataAsRedisResult = rowDataFromRedis;
            object[] lockScriptReturnValueArray = (object[])rowDataAsRedisResult;
            ChangeTrackingSessionStateItemCollection sessionData = null;
            if (lockScriptReturnValueArray.Length > 1 && lockScriptReturnValueArray[1] != null)
            {
                object[] data = (object[])lockScriptReturnValueArray[1];

                // LUA script returns data as object array so keys and values are store one after another
                // This list has to be even because it contains pair of <key, value> as {key, value, key, value}
                if (data != null && data.Length != 0 && data.Length % 2 == 0)
                {
                    sessionData = new ChangeTrackingSessionStateItemCollection(redisUtility);
                    // In every cycle of loop we are getting one pair of key value and putting it into session items
                    // thats why increment is by 2 because we want to move to next pair
                    for (int i = 0; (i + 1) < data.Length; i += 2)
                    {
                        string key = (string)data[i];
                        if (key != null)
                        {
                            sessionData.SetData(key, (byte[])data[i + 1]);
                        }
                    }
                }
            }
            return sessionData;
        }

        public int GetSessionTimeout(object rowDataFromRedis)
        {
            //throw new NotImplementedException();


            return 5000;
        }

        public bool IsLocked(object rowDataFromRedis)
        {
            // throw new NotImplementedException();

            return true;
        }

        #region private functions

        private object RetryForScriptNotFound(Func<object> redisOperation)
        {
            try
            {
                return redisOperation.Invoke();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("NOSCRIPT"))
                {
                    // Second call should pass if it was script not found issue
                    return redisOperation.Invoke();
                }
                throw;
            }
        }

        /// <summary>
        /// If retry timout is provide than we will retry first time after 20 ms and after that every 1 sec till retry timout is expired or we get value.
        /// </summary>
        private object RetryLogic(Func<object> redisOperation)
        {
            int timeToSleepBeforeRetryInMiliseconds = 20;
            DateTime startTime = DateTime.Now;
            while (true)
            {
                try
                {
                    return RetryForScriptNotFound(redisOperation);
                }
                catch (Exception)
                {
                    TimeSpan passedTime = DateTime.Now - startTime;
                    if (providerConfiguration.RetryTimeout < passedTime)
                    {
                        throw;
                    }
                    else
                    {
                        int remainingTimeout = (int)(providerConfiguration.RetryTimeout.TotalMilliseconds - passedTime.TotalMilliseconds);
                        // if remaining time is less than 1 sec than wait only for that much time and than give a last try
                        if (remainingTimeout < timeToSleepBeforeRetryInMiliseconds)
                        {
                            timeToSleepBeforeRetryInMiliseconds = remainingTimeout;
                        }
                    }

                    // First time try after 20 msec after that try after 1 second
                    System.Threading.Thread.Sleep(timeToSleepBeforeRetryInMiliseconds);
                    timeToSleepBeforeRetryInMiliseconds = 1000;
                }
            }
        }
        #endregion



    }
}
