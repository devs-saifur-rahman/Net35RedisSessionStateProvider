using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedisSessionProvider
{
    internal class ValueWrapper
    {
        object _actualValue;
        byte[] _serializedvalue;

        public ValueWrapper(byte[] serializedvalue)
        {
            _serializedvalue = serializedvalue;
        }

        public ValueWrapper(object actualValue)
        {
            _actualValue = actualValue;
        }

        public object GetActualValue(RedisUtility utility)
        {
            if (_actualValue == null)
            {
                _actualValue = utility.GetObjectFromBytes(_serializedvalue);
            }
            return _actualValue;
        }

        public void SetActualValue(object actualValue)
        {
            _actualValue = actualValue;
            // Null serialized value just for completeness
            _serializedvalue = null;
        }

        // This method should be used in test projects only
        internal object GetActualValue()
        {
            return _actualValue;
        }

        // This method should be used in test projects only
        internal object GetSerializedvalue()
        {
            return _serializedvalue;
        }
    }

}
