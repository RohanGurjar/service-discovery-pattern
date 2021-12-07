using NNanomsg.Protocols;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discovery
{
    class Program
    {
        const int DEFAULT_PORT = 5555;
        static TimeSpan defaultDeadline = new TimeSpan(0, 0, 0, 1);
        private static ServiceDiscovery _discovery;

        static void Main(string[] args)
        {
            Task.Run(() => ResolveService());
            StartDiscovery();
        }

        private static void ResolveService()
        {
            var reply = new ReplySocket();
            reply.Bind("tcp://127.0.0.1:1234");
            while (true)
            {
                var request = Encoding.Default.GetString(reply.Receive());
                if (request == "resolve")
                {
                    var serviceAddress = _discovery.Resolve("foo");
                    reply.Send(Encoding.ASCII.GetBytes(serviceAddress));
                }
            }
        }

        private static void StartDiscovery()
        {
            _discovery = new ServiceDiscovery(DEFAULT_PORT, defaultDeadline);
            _discovery.Bind();

            Console.WriteLine(
                $"Starting service discovery [port: {DEFAULT_PORT}, deadline: {defaultDeadline}]");

            while (true)
            {
                _discovery.Discover();
                Thread.Sleep(1000);
            }
        }
    }
}
