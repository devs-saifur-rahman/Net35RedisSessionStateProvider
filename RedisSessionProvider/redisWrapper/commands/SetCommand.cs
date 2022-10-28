using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedisSessionProvider.redisWrapper.commands
{

    public class SetCommand : IRedisCommand
    {
        public string Name
        {
            get
            {
                return "SET";
            }
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public TimeSpan? ExpireTime { get; set; }

        public bool? Override { get; set; }

        public string GetCommand()
        {
            if (ExpireTime.HasValue)
            {
                if (Override.HasValue)
                    return $"SET {Key} {Value} EX {ExpireTime.Value.TotalSeconds} {((bool)Override ? "XX" : "NX" )}";
                else
                    return $"SET {Key} {Value} EX {ExpireTime.Value.TotalSeconds}";
            }
            else if (Override.HasValue)
                return $"SET {Key} {Value} {((bool)Override ? "XX" : "NX")}";
            else
                return $"SET {Key} {Value}";
        }
    }

}
