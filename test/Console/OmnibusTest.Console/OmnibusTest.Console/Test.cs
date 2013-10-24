using Omnibus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmnibusTest.Console
{
    class Test
    {
        public Test() { }

        public void Run()
        {
            IBus bus = new Omnibus.Core.Omnibus();

            bus.Initialize(omnibus =>
            {
                omnibus.UseMessageHost();
            });

            bus.OnReply<BasicResponse>(response => System.Console.WriteLine("OnReply<TRes>: Got a response handled on static route: {0}", response.Message));
            bus.Call<BasicRequest>(new BasicRequest { Message = "This is a test of the static routes" });

            // Configure this process to handle some basic messages
            bus.On<BasicEvent>(
                (basicEvent) =>
                {
                    System.Console.WriteLine("On<BasicEvent>: Received BasicEvent: {0}", basicEvent.ToString());
                });

            bus.ReplyTo<BasicRequest, BasicResponse>((req) =>
            {
                return new BasicResponse { Message = req.Message };
            });

            bus.Call<BasicRequest, BasicResponse>(
                new BasicRequest { Message = "Testing a call from the client" },
                response =>
                {
                    System.Console.WriteLine("Call<TReq, TRes>: Got an answer from the server: {0}", response.Message);
                });
        }


    }
}
