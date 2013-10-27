using System;
using System.Threading;
using Succubus.Core;
using Succubus.Interfaces;

namespace SuccubusTest.Console
{
    internal class Test
    {
        public void Run()
        {
            IBus bus = new Bus();
            IBus bus2 = new Bus();
            bus2.Initialize();

            bus2.On<BasicEvent>(e => System.Console.WriteLine("Bus2::On<BasicEvent>: {0}", e.Message));

            // SETUP BUS WITH HOST

            bus.Initialize(succubus =>
            {
                succubus.StartupMessageHost();
                succubus.SetNetwork("NETWORK");
            });

            Thread.Sleep(1000);

            // SETUP ReplyTo-HANDLERS

            bus.ReplyTo<BasicRequest, BasicResponse>(req =>
            {
                Thread.Sleep(100);
                return new BasicResponse
                {

                    Message = "Reply from server: " + req.Message
                };
            });

            bus.ReplyTo<Request1, Response3>(req => new Response3 { Message = "RESPONSE 3 " + req.Message });
            bus.ReplyTo<Request1, Response2>(req => new Response2 { Message = "RESPONSE 2 " + req.Message });
            bus.ReplyTo<Request1, Response1>(req => new Response1 { Message = "RESPONSE 1" + req.Message });

            // SETUP STATIC ROUTES

            bus.OnReply<Request1, Response1, Response2>((request, response1, response2) =>
                System.Console.WriteLine(
                    "OnReply<Request1, Response1, Response2>: The request {0} returned {1} and {2}",
                    request.Message, response1.Message, response2.Message));

            bus.OnReply<Request1, Response1, Response2, Response3>((request, response1, response2, response3) =>
                System.Console.WriteLine("OnReply<R,R1,R2,R3>: Req: {0} Res1: {1} Res2: {2}: Res3: {3}",
                    request.Message,
                    response1.Message,
                    response2.Message, response3.Message));

            bus.OnReply<BasicRequest, BasicResponse>((request, response) => System.Console.WriteLine(
                "NOTICE! OnReply<TReq, TRes>: Got a response handled on static handler: {0} => {1}",
                request.Message,
                response.Message));


            // MAKE CALLS

            bus.Call(new Request1 { Message = "STATIC Request1" });

            bus.Call(new BasicRequest { Message = "TIMEOUTTEST BasicRequest 1" }, (req) => System.Console.WriteLine("Call timed out! {0}", req.Message), 10);
            bus.Call(new BasicRequest { Message = "TIMEOUTTEST BasicRequest 2" }, (req) => System.Console.WriteLine("Call timed out! {0}", req.Message), 145);
            bus.Call(new BasicRequest { Message = "TIMEOUTTEST BasicRequest 3" }, (req) => System.Console.WriteLine("Call timed out! {0}", req.Message), 1000);
            bus.Call(new BasicRequest { Message = "TIMEOUTTEST BasicRequest 4" }, (req) => System.Console.WriteLine("Call timed out! {0}", req.Message), 50);
            bus.Call(new BasicRequest { Message = "TIMEOUTTEST BasicRequest 5" }, (req) => System.Console.WriteLine("Call timed out! {0}", req.Message), 1);

            //(bus as Bus).DumpTimeoutTable();
            //Thread.Sleep(50);
            //(bus as Bus).DumpTimeoutTable();

            //bus.Call(new BasicRequest {Message = "STATIC BasicRequest 2"});

            // SETUP EVENT HANDLERS

            bus.On<BasicEvent>(
                basicEvent => System.Console.WriteLine("On<BasicEvent>: Received BasicEvent: {0}", basicEvent.Message));


            // TRANSIENT ROUTING

            bus.Call<BasicRequest, BasicResponse>(
                new BasicRequest { Message = "Testing a call from the client" },
                response =>
                    System.Console.WriteLine(
                        "Call<TReq, TRes>: Got a response handled on transient route handler: {0}", response.Message));

            // FIRE EVENTS

            bus.Publish(new BasicEvent { Message = "Hello, world!" });

            Thread.Sleep(500);

            // ADD NEW EVENT TO EXISTING TO CHECK MULTICAST

            bus.On<BasicEvent>(
                basicEvent2 => System.Console.WriteLine("On<BasicEvent>2: {0}", basicEvent2.Message));
            bus.Publish(new BasicEvent { Message = "We meet again, world!" });
            bus.Call(new BasicRequest { Message = "TimeoutReq" }, (req) => System.Console.WriteLine("Call timed out! {0}", req.Message), 1);
            bus.Call(new BasicRequest { Message = "STATIC BasicRequest 1" }, (req) => System.Console.WriteLine("Call timed out! {0}", req.Message), 1000);
            
        }
    }
}