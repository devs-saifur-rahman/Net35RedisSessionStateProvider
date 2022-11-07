using RedisSessionProvider.redisWrapper.commands;
using RedisSessionProvider.redisWrapper.commands.Commands;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace RedisSessionProvider.redisWrapper
{


    public class RedisClient
    {
        private TcpClient client;
        private NetworkStream stream;
        private RedisReply reply;
        private IRedisCommand command;

        public string Host { get; }

        public int Port { get; }

        public object Return
        {
            get
            {
                return reply.Value;
            }
        }

        public RedisClient(string _host = "127.0.0.1", int _port = 6379)
        {
            if (string.IsNullOrEmpty(_host))
                throw new ArgumentNullException(nameof(_host));
            this.Host = _host;
            this.Port = _port;
            client = new TcpClient();

        }
        public void Connect()
        {
            try
            {
                client.Connect(Host, Port);
                stream = client.GetStream();
            }
            catch (Exception ex)
            {
                throw new RedisException("An existing connection was forcibly closed by remote host." + ex.Message);
            }
        }
        public void Close()
        {
            stream.Close();
            client.Close();
        }

        public void Append(string key, string value)
        {
            command = new AppendCommand() { Key = key, Value = value };
            Execute(command);
        }

        public void Del(params string[] keys)
        {
            command = new DelCommand() { Keys = keys };
            Execute(command);
        }

        public byte[] Dump(string key)
        {
            command = new DumpCommand() { Key = key };
            Execute(command);
            if (reply.Value != null)
                return null;
            else
                return Encoding.UTF8.GetBytes((string)reply.Value);
        }

        public void Echo(string message)
        {
            command = new EchoCommand() { Message = message };
            Execute(command);
        }

        public bool Exists(string key)
        {
            command = new ExistsCommand() { Key = key };
            Execute(command);
            return (bool)reply.Value;
        }

        public void Expire(string key, int timeout)
        {
            command = new ExpireCommand() { Key = key, Timeout = timeout };
            Execute(command);
        }

        public void ExpireAt(string key, DateTime ttl)
        {
            command = new ExpireAtCommand() { Key = key, TTL = ttl.Millisecond };
            Execute(command);
        }

        public string Get(string key)
        {
            command = new GetCommand() { Key = key };
            Execute(command);
            return (string)reply.Value;
        }
        //NLUA - lua sxcrip
        internal object ScriptEvaluate(string script, string[] redisKeyArgs, object[] redisValueArgs)
        {
            //throw new NotImplementedException();
            string args = " " +redisKeyArgs.Length.ToString() + " ";
            args += string.Join(" ", redisKeyArgs) + " ";

            //for (int i = 0; i < redisKeyArgs.Length; i++)
            //{
            //    args = script.Replace($"KEYS[{i + 1}]", "\"" + redisKeyArgs[i] + "\"");
            //}

            for (int i = 0; i < redisValueArgs.Length; i++)
            {
                args = args + " " +  redisValueArgs[i].ToString();
            }


            // Lua state = new Lua();

            //var returnVal = state.DoString(script);


            //Script scr = new Script();
            //DynValue res = scr.DoString(script);

            //DynValue res = script.Call(script.Globals["fact"], 4);
            var k = System.Environment.NewLine;
           //script= script.Replace(System.Environment.NewLine, ";");

           // script = script.Remove(script.IndexOf(';'),1);
            string evalScript = "EVAL " + "\"" + script + "\"" + args + k;

            //string evalScript = "EVAL \"return redis.call('SET',KEYS[1],ARGV[1]);\" 1 keyname_1 valname_1"+k;
            byte[] bytes = Encoding.UTF8.GetBytes(evalScript);
            try
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                bytes = new byte[client.ReceiveBufferSize + 1];
                stream.Read(bytes, 0, bytes.Length);
                var result = Encoding.UTF8.GetString(bytes);
                switch (result[0])
                {
                    case '$':
                        {
                            var length = Convert.ToInt32(result.Substring(1, result.IndexOf(Environment.NewLine) - 1));
                            if (length == -1)
                                reply = new RedisReply(RESPType.BulkString, null);
                            else
                                reply = new RedisReply(RESPType.BulkString, result.Substring(result.IndexOf(Environment.NewLine) + 2, length));
                            break;
                        }

                    case '+':
                        {
                            reply = new RedisReply(RESPType.SimpleString, result.Substring(1, result.IndexOf(Environment.NewLine) - 1));
                            break;
                        }

                    case ':':
                        {
                            reply = new RedisReply(RESPType.Integer, Convert.ToInt32(result.Substring(1, result.IndexOf(Environment.NewLine) - 1)));
                            break;
                        }

                    case '-':
                        {
                            reply = new RedisReply(RESPType.Error, result.Substring(1, result.IndexOf(Environment.NewLine) - 1));
                            throw new RedisException(reply.Value.ToString());
                            break;
                        }

                    case '*':
                        {
                            var count = Convert.ToInt32(result.Substring(1, result.IndexOf(Environment.NewLine) - 1));
                            var items = result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                            items.RemoveAt(0);
                            items.RemoveAll(i => i.StartsWith("$"));
                            items.RemoveAt(items.Count - 1);
                            reply = new RedisReply(RESPType.Array, items);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                throw new RedisException($"There is an internal error during executing '{command.GetCommand()}'.");
            }
            return reply;
        }



        public void PExpire(string key, int timeout)
        {
            command = new PExpireCommand() { Key = key, Timeout = timeout };
            Execute(command);
        }

        public void PExpireAt(string key, DateTime ttl)
        {
            command = new PExpireAtCommand() { Key = key, TTL = ttl.Millisecond };
            Execute(command);
        }

        public void Ping()
        {
            command = new PingCommand();
            Execute(command);
        }

        public void Set(string key, object value, TimeSpan? expireTime = default(TimeSpan?), bool? @override = default(Boolean?))
        {
            command = new SetCommand() { Key = key, Value = value.ToString(), ExpireTime = expireTime, Override = @override };
            Execute(command);
        }

        public DateTime Time()
        {
            command = new TimeCommand();
            Execute(command);

            return new DateTime(1970, 1, 1).AddSeconds(reply.Value.ToString()[0]).ToLocalTime();  //reply.Value[0]

        }

        private void Execute(IRedisCommand command)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(command.GetCommand() + Environment.NewLine);
            try
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                bytes = new byte[client.ReceiveBufferSize + 1];
                stream.Read(bytes, 0, bytes.Length);
                var result = Encoding.UTF8.GetString(bytes);
                switch (result[0])
                {
                    case '$':
                        {
                            var length = Convert.ToInt32(result.Substring(1, result.IndexOf(Environment.NewLine) - 1));
                            if (length == -1)
                                reply = new RedisReply(RESPType.BulkString, null);
                            else
                                reply = new RedisReply(RESPType.BulkString, result.Substring(result.IndexOf(Environment.NewLine) + 2, length));
                            break;
                        }

                    case '+':
                        {
                            reply = new RedisReply(RESPType.SimpleString, result.Substring(1, result.IndexOf(Environment.NewLine) - 1));
                            break;
                        }

                    case ':':
                        {
                            reply = new RedisReply(RESPType.Integer, Convert.ToInt32(result.Substring(1, result.IndexOf(Environment.NewLine) - 1)));
                            break;
                        }

                    case '-':
                        {
                            reply = new RedisReply(RESPType.Error, result.Substring(1, result.IndexOf(Environment.NewLine) - 1));
                            throw new RedisException(reply.Value.ToString());
                            break;
                        }

                    case '*':
                        {
                            var count = Convert.ToInt32(result.Substring(1, result.IndexOf(Environment.NewLine) - 1));
                            var items = result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                            items.RemoveAt(0);
                            items.RemoveAll(i => i.StartsWith("$"));
                            items.RemoveAt(items.Count - 1);
                            reply = new RedisReply(RESPType.Array, items);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                throw new RedisException($"There is an internal error during executing '{command.GetCommand()}'.");
            }
        }
    }

}
