Succubus
========

A light .NET "service bus" implementation created on top of ØMQ/ZeroMQ.

About
-----

Succubus is a simple (at this point) implementation of convenience functions for .NET on top of ØMQ, presenting an interface similar to that of MassTransit. It is similar to an Enterprise Service Bus, but lacks a lot of features at its current state.

Because it uses ØMQ, all brokers live in the Succubus process space, and you do not install anything besides the dependencies in the library.

For more in-depth documentation, check the [wiki](https://github.com/COCPORN/succubus/wiki).

Getting started
---------------

A quickstart guide to getting started with Succubus.

Installation
------------

Get lastest version of Succubus from GitHub or NuGet.

Instantiation
-------------

To instantiate a handle to the bus, create an instance of `Succubus.Core.Bus`:
```C#
IBus bus = new Succubus.Core.Bus();
```

Each handle will have a separate channel to the message host.

Singleton instantiation
-----------------------

Succubus allows you to get a singleton instance in addition to newing up objects. Note that the singleton object will always (obviously) point to the same instance, while all newed objects will be different. These can co-exist and will use separate message channels:

```C#
Bus.Instance.Initialize(config => {
    config.UseMessageHost();
});
```

Initialization
--------------

Before using the bus, it needs to be initialized:

```C#
Bus.Instance.Initialize();
```

The `Initialize`-call to the bus alternatively returns a configuration handle.

```C#
bus.Initialize(succubus =>
{
    succubus.UseMessageHost();               
});
```

When using the parameterless Initialize call, the bus will be initialized with default values.

Configuration
-------------

There are a number of methods that allow you to configure your bus instance before using it. These calls can be found in the `IBusConfigurator`-interface, but will be documented in detail in the future.

```C#
public interface IBusConfigurator
{        
    void UseMessageHost(int publishPort = 9000, int subscribePort = 9001, bool setupHost = true);
    void UseMessageHost(IMessageHost messageHost);
    void SetNetwork(string networkName);
    void SetMessageHostname(string hostname);
}
```

Most of these should be self explanatory.

### UseMessageHost

Where to find the message host. 

### SetNetwork

This is the most coarse filter Succubus allows you to set. If you have multiple services running on the messagehost, you can set the network name to different strings, and the messages will not be published on other clients.

### SetMessageHostname

If you are running with a remote messagehost, you can set the hostname for the messagehost here.

### StartupMessageHost

If this configuration method is called, the bus will run a messagehost on localhost as part of this bus instance.

Events
------

Succubus supports publishing and consuming events. Events are agnostic to where they are posted from and who consumes them. A single event can have multiple consumers.

### Publishing events

Publish events by calling the `Publish`-method:

```C#
bus.Publish(new BasicEvent { Message = "Hi, there! "});
```

`BasicEvent` is a user defined POCO-class:

```C#
public class BasicEvent
{
    public string Message { get; set; }
}
```

### Consuming events

If you are interested in handling an event, use the `On`-method:

```C#
bus.On<BasicEvent>(e => {
	Console.WriteLine("Got event: {0}", e.Message)
});
```

Synchronous processing
----------------------

Succubus supports two types of synchronous calling; static and transient routes. A transient route is setup as the call is made, and is removed after a call has been processed, while a static route is permanent in the bus.

When using synchronous processing, the _client_ side of the interaction needs to use a variation of the `Call`-method and the _server_ side then replies using the `ReplyTo`-method.

### Replying to messages

When a message is presented from the bus after being sent with the `Call`-method, it can be handled with the `ReplyTo`-method:

```C#
bus.ReplyTo<BasicRequest, BasicResponse>(request => {
	return new BasicResponse { Message = request.Message + " echoed from server" };
});
```

Note that `BasicRequest` and `BasicResponse` are user defined POCO-classes; Succubus will handle any routing information behind the scenes.

### Transient routes

Transient routes are simple request/response-pairs.

```C#
bus.Call<Request, Response>(new Request { Message = "Hi from client"},
	response => {
		Console.WriteLine("Got response from server: {0}", response.Message);
	});
```

A transient call with request/response-parameters will register a route and wrap the request/response objects in a `SynchronousMessageFrame` which decorates the messages with `CorrelationId`s. If multiple responses are made to the same synchronous call, only the first will be handled in the defined response handler, while the other messages will be raised as events.

### Static routes

Static routes allow for reuse of handler structures and more advanced orchestration.

```C#
bus.OnReply<BasicRequest, BasicResponse>((request, response) => 
    Console.WriteLine("OnReply<TReq, TRes>: Got a response handled on static handler: {0} => {1}", 
    request.Message, 
    response.Message));
bus.Call<BasicRequest>(new BasicRequest { Message = "Hello from client"} );
```

Succubus will store the request until the response arrives, so both can be handled in the same context.

### Timeouts

Synchronized processing supports timeout handlers on a per-SynchronizationStack basis. A SynchronizationStack is an internal structure, and one is created for every set of OnReply's added to a request-type. In other words:

```C#
bus.OnReply<BasicRequest, BasicReponse>((request, response) => {
		// Got response from server
	},
	request => {
		// TimeoutHandler
		Console.WriteLine("The request timed out for: {0}", request.ToString());
	}, 1000); // Last parameter is the amount of milliseconds for the timeout
```	

The timeout parameters can also go into the `Call` for the synchronous processing:

```C#
bus.Call(new BasicRequest { Message = "Hello!"}, 2500);
```

For a given `SynchronizationStack`, the parameter in the OnReply takes prescedence.


### Orchestration

Synchronous processing in Succubus opens up for some complex orchestration. Example:

```C#
bus.OnReply<ImageProcessed, FriendNotified>((ip, fn) =>
{
	Console.WriteLine("New profile image has been processed with response: {0}", ip.Status);
	Console.WriteLine("Friends have been notified with response: {0}", fn.Status);
});
```

Succubus will orchestrate up to 7 response messages. Replies can also be chained.

```C#
bus.OnReply<UpdateRequest, 
            ImageProcessed, 
            FriendNotified>((ur, ip, fn) =>
{
	Console.WriteLine("New profile image has been processed with response: {0}", ip.Status);
	Console.WriteLine("Friends have been notified with response: {0}", fn.Status);
});
	
```

The `Then`-block is guaranteed not to be fired before the preceding `OnReply`-block. This makes it possible to do sequencial and partial handling of responses. This kind of orchestration is able to give you guarantees as to in _which order_ a set of messages are processed, regardless of the order in which they are delivered from the messagebus.

You can also have multiple `OnReply`/`Then`-blocks:

```C#
bus.OnReply<ImageProcessed, FriendNotified>((ip, fn) =>
{
	Console.WriteLine("New profile image has been processed with response: {0}", ip.Status);
	Console.WriteLine("Friends have been notified with response: {0}", fn.Status);
})
	.Then<DataStored>(ds => {
		Console.WriteLine("The data has been successfully stored.")	
	})
.OnReply<Error>(err => {
	Console.WriteLine("An error occurred handling update.");
});
```

Workload management
-------------------
