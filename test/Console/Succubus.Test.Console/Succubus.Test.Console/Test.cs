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

            bus.Initialize(succubus =>
            {
                succubus.StartupMessageHost();
                succubus.SetNetwork("NETWORK");
            });

            Thread.Sleep(1000);

            bus.ReplyTo<BasicRequest, BasicResponse>(
                req => new BasicResponse {Message = "Reply from server: " + req.Message});

            bus.OnReply<Request1, Response1, Response2>((request, response1, response2) =>
                System.Console.WriteLine(
                    "OnReply<Request1, Response1, Response2>: The request {0} returned {1} and {2}",
                    request.Message, response1.Message, response2.Message));

            bus.OnReply<Request1, Response1, Response2, Response3>((request, response1, response2, response3) =>
                System.Console.WriteLine("OnReply<R,R1,R2,R3>: Req: {0} Res1: {1} Res2: {2}: Res3: {3}",
                    request.Message,
                    response1.Message,
                    response2.Message, response3.Message));

            bus.ReplyTo<Request1, Response3>(req => new Response3 {Message = "RESPONSE 3 " + req.Message});
            bus.ReplyTo<Request1, Response2>(req => new Response2 {Message = "RESPONSE 2 " + req.Message});
            bus.ReplyTo<Request1, Response1>(req => new Response1 {Message = "RESPONSE 1" + req.Message});

            bus.Call(new Request1 {Message = "Hohey!"});

            bus.OnReply<BasicRequest, BasicResponse>((request, response) =>
                System.Console.WriteLine("OnReply<TReq, TRes>: Got a response handled on static handler: {0} => {1}",
                    request.Message,
                    response.Message));
            bus.Call(new BasicRequest {Message = "This is a test of the static routes"});
            bus.Call(new BasicRequest {Message = "Habahaba zutzut"});

            // Configure this process to handle some basic messages
            bus.On<BasicEvent>(
                basicEvent => System.Console.WriteLine("On<BasicEvent>: Received BasicEvent: {0}", basicEvent.Message));


            bus.Call<BasicRequest, BasicResponse>(
                new BasicRequest {Message = "Testing a call from the client"},
                response =>
                    System.Console.WriteLine(
                        "Call<TReq, TRes>: Got a response handled on transient route handler: {0}", response.Message));

            bus.Publish(new BasicEvent {Message = "Hello, world!"});

            Thread.Sleep(500);
            bus.On<BasicEvent>(
                basicEvent2 => System.Console.WriteLine("On<BasicEvent>: HORY SHEET! {0}", basicEvent2.Message));
            bus.Publish(new BasicEvent {Message = "We meet again, world!"});
        }
    }
}