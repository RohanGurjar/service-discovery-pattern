using NNanomsg.Protocols;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discovery
{
    internal class ServiceDiscovery
    {
        private readonly SurveyorSocket _surveyorSocket;
        private readonly int _port;
        private readonly TimeSpan _deadline = new TimeSpan(0, 0, 0, 1);
        private Dictionary<string, string> _services;

        public ServiceDiscovery(int port, TimeSpan deadline)
        {
            _surveyorSocket = new SurveyorSocket();
            _port = port;
            _deadline = deadline;
            _services = new Dictionary<string, string>();
        }

        internal void Bind()
        {
            _surveyorSocket.Bind($"tcp://*:{_port}");
            _surveyorSocket.SurveyorOptions.Deadline = _deadline;
        }

        internal Dictionary<string, string> Discover()
        {
            _surveyorSocket.Send(Encoding.ASCII.GetBytes("service query"));

            while (true)
            {
                byte[] response = _surveyorSocket.Receive();
                if (response == null)
                {
                    break;
                }
                var res = Encoding.Default.GetString(response);
                var data = res.Split('|');
                var service = data[0];
                var address = data[1];
                Console.WriteLine($"Received from service {service}:{address}");
                if (!_services.ContainsKey(service))
                {
                    _services.Add(service, address);
                }
                else
                {
                    _services[service] = address;
                }
                _surveyorSocket.Send(Encoding.ASCII.GetBytes("service registered"));
            }

            return _services;
        }

        internal string Resolve(string serviceName)
        {
            return _services[serviceName];
        }
    }
}
