using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RedisSessionProvider
{
    [Serializable]
    internal class RedisNull : ISerializable
    {
        public RedisNull()
        { }
        protected RedisNull(SerializationInfo info, StreamingContext context)
        { }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        { }
    }
}
