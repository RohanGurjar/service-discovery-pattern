using NNanomsg.Protocols;
using System;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceAddress;
            using (var req = new RequestSocket())
            {
                req.Connect("tcp://127.0.0.1:1234");
                req.Send(Encoding.ASCII.GetBytes("resolve"));
                serviceAddress = Encoding.Default.GetString(req.Receive());
            }

            using (var serviceReq = new RequestSocket())
            {
                serviceReq.Connect(serviceAddress);
                while (true)
                {
                    serviceReq.Send(Encoding.ASCII.GetBytes("Hello"));
                    Console.WriteLine($"Received: {Encoding.Default.GetString(serviceReq.Receive())}");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
