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
            IBus bus = new Omnibus.Core.Bus();

            bus.Initialize(omnibus =>
            {
                omnibus.UseMessageHost();
            });

            bus.OnReply<BasicRequest, BasicResponse>((request, response) => 
                System.Console.WriteLine("OnReply<TReq, TRes>: Got a response handled on static handler: {0} => {1}", 
                request.Message, 
                response.Message));
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
                    System.Console.WriteLine("Call<TReq, TRes>: Got a response handled on throwaway handler: {0}", response.Message);
                });
        }


    }
}
