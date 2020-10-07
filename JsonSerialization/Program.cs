using System;
using System.IO;
using Newtonsoft.Json;

namespace JsonSerialization
{
    public sealed class JsonRequest
    {
        public JsonRequest(string methodName, object[] args, string[] argTypes)
        {
            MethodName = methodName;
            Args = args;
            ArgTypes = argTypes;
        }
        public string MethodName { get; set; }
        public object[] Args { get; set; }

        public string[] ArgTypes { get; set; }

        public object[] DeserializedArgs
        {
            get
            {
                var jsonSerializer = JsonSerializer.Create();
                object[] args;
                if (Args == null || ArgTypes == null)
                    args = null;
                else
                {
                    args = new object[Args.Length];
                    for (int i = 0; i < Args.Length; i++)
                    {
                        var argsStringReader = new StringReader(Args[i] as string);
                        args[i] = jsonSerializer.Deserialize(argsStringReader, Type.GetType(ArgTypes[i]));
                    }
                }
                return args;
            }
        }

        public static JsonRequest Parse(string s)
        {
            var stringReader = new StringReader(s);
            var jsonSerializer = JsonSerializer.Create();
            return (jsonSerializer.Deserialize(stringReader, typeof(JsonRequest)) as JsonRequest);
        }

        public static JsonRequest PackMethodSignature(string methodName, object[] args)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create();

            string[] argTypes;
            if (args == null)
                argTypes = null;
            else
            {
                argTypes = new string[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    argTypes[i] = args[i].GetType().AssemblyQualifiedName;

                    var argsStringWriter = new StringWriter();
                    jsonSerializer.Serialize(argsStringWriter, args[i]);
                    args[i] = argsStringWriter.ToString();
                }
            }

            return new JsonRequest(methodName, args, argTypes);
        }

        public static string FormInvokationRequest(string methodName, object[] args)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create();

            var stringWriter = new StringWriter();
            jsonSerializer.Serialize(stringWriter, JsonRequest.PackMethodSignature(methodName, args));

            return stringWriter.ToString();
        }
    }

    public static class JsonResponse
    {
        public static ReturnType DeserializeResponse<ReturnType>(string response)
        {
            var jsonSerializer = JsonSerializer.Create();
            var stringReader = new StringReader(response);
            return (ReturnType)jsonSerializer.Deserialize(stringReader, typeof(ReturnType));
        }

        public static string SerializeResponse(object response)
        {
            var jsonSerializer = JsonSerializer.Create();
            var stringWriter = new StringWriter();
            jsonSerializer.Serialize(stringWriter, response);
            return stringWriter.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
