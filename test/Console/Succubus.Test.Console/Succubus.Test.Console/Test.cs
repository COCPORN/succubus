using Succubus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuccubusTest.Console
{
    class Test
    {
        public Test() { }

        public void Run()
        {
            IBus bus = new Succubus.Core.Bus();

            bus.Initialize(Succubus =>
            {
                Succubus.UseMessageHost();
                
            });

            Thread.Sleep(3000);

#if false

            bus.OnReply<BasicRequest, BasicResponse>((request, response) => 
                System.Console.WriteLine("OnReply<TReq, TRes>: Got a response handled on static handler: {0} => {1}", 
                request.Message, 
                response.Message));
            bus.Call<BasicRequest>(new BasicRequest { Message = "This is a test of the static routes" });
#endif

            // Configure this process to handle some basic messages
            bus.On<BasicEvent>(
                (basicEvent) =>
                {
                    System.Console.WriteLine("On<BasicEvent>: Received BasicEvent: {0}", basicEvent.ToString());
                });

            bus.ReplyTo<BasicRequest, BasicResponse>((req) =>
            {
                return new BasicResponse { Message = "Reply from server: " + req.Message };
            });

            bus.Call<BasicRequest, BasicResponse>(
                new BasicRequest { Message = "Testing a call from the client" },
                response =>
                {
                    System.Console.WriteLine("Call<TReq, TRes>: Got a response handled on throwaway handler: {0}", response.Message);
                });

            bus.Publish(new BasicEvent { });
        }


    }
}
