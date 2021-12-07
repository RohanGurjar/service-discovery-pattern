using NNanomsg.Protocols;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    class Program
    {
        const string DEFAULT_DISCOVERY_HOST = "localhost";
        const int DEFAULT_DISCOVERY_PORT = 5555;
        const string DEFAULT_SERVICE_PROTOCOL = "tcp";
        const string DEFAULT_DISCOVERY_PROTOCOL = "tcp";
        const string DEFAULT_SERVICE_HOST = "localhost";

        static void Main(string[] args)
        {
            string serviceName = args[0];
            int servicePort = Convert.ToInt32(args[1]);
            Task.Run(() => RegisterService(serviceName, servicePort));
            StartService(serviceName, servicePort);
        }

        private static void RegisterService(string name, int port)
        {
            var socket = new RespondentSocket();
            socket.Connect($"{DEFAULT_DISCOVERY_PROTOCOL}://{DEFAULT_DISCOVERY_HOST}:{DEFAULT_DISCOVERY_PORT}");

            var serviceAddress =
                $"{DEFAULT_SERVICE_PROTOCOL}://{DEFAULT_SERVICE_HOST}:{port}";
            Console.WriteLine(
                $"Starting service registration [service: {name} {serviceAddress}, " +
                $"discovery: {DEFAULT_DISCOVERY_HOST}:{DEFAULT_DISCOVERY_PORT}]");

            while (true)
            {
                byte[] msg = socket.Receive();
                if (msg != null)
                {
                    if (string.Equals(Encoding.Default.GetString(msg), "service query"))
                    {
                        byte[] response = Encoding.ASCII.GetBytes($"{name}|{serviceAddress}");
                        socket.Send(response);
                    }
                    else if (string.Equals(Encoding.Default.GetString(msg), "service registered"))
                    { 
                        break;
                    }
                }
            }
        }

        private static void StartService(string name, int port)
        {
            var repSocket = new ReplySocket();
            repSocket.Bind($"{DEFAULT_SERVICE_PROTOCOL}://*:{port}");

            Console.WriteLine($"Starting service {name}");

            while (true)
            {
                byte[] req = repSocket.Receive();
                Console.WriteLine($"Request: {Encoding.Default.GetString(req)}");
                repSocket.Send(Encoding.ASCII.GetBytes($"From service: {name}: The answer is 42"));
            }
        }
    }
}
