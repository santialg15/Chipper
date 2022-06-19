using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace ServerAdminLogic
{
    public class Program
    {
        private static GrpcChannel channel;
        private static Greeter.GreeterClient client;
        private static int entra = 1;

        public Program()
        {
            prueba();
        }
        public static async Task prueba()
        {
            
            if (entra == 1){
                channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
                {
                    MaxReceiveMessageSize = 5 * 1024 * 1024, // 5 MB
                    MaxSendMessageSize = 2 * 1024 * 1024 // 2 MB
                });
                client = new Greeter.GreeterClient(channel);
                entra = 2;
            }
            var response = await client.SayHelloAsync(
                new HelloRequest { Name = "GreeterClient" });

            var lala = response;
            Console.WriteLine(response.Message);
        }
    }
}
