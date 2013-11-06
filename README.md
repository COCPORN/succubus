Succubus
========

A light .NET distributed application framework created on top of ZeroMQ.

About
-----

Succubus is a simple (at this point) implementation of convenience functions for .NET on top of ZeroMQ, presenting an interface similar to that of MassTransit.

Because it uses ZeroMQ, all brokers live in the Succubus process space, and you do not install anything besides the dependencies in the library.

Succubus is currently in a pre-release but usable state. The repository will go public once it reaches a 0.2-release.

Features
--------

These are the current and planned features of Succubus:

- Event publishing and consumption
- Synchronous calls with request/response
- Orchestration of synchronous messages
- Timeouts of synchronous messages
- Planned for 0.3:
	- Deferred messaging
	- Addressable commands
	- Work-item fan out

Why?
----

Succubus is implemented just for fun, don't expect to see any best-in-class implementations or super stable code, at least not at this point. It is intended to have few dependencies, be quick to develop with and accommodate some "special" use cases. Most of the things implemented by Succubus are fairly simple to implement directly on ZeroMQ, but the idea is to take some of the hassle out of it when working with C#.

Installation
------------

Get lastest version of Succubus from GitHub or NuGet (will be up soon).

Instantiation
-------------

To instantiate a handle to the bus, create an instance of `Succubus.Core.Bus`:
```C#
IBus bus = new Succubus.Core.Bus();
```

Each handle will have a separate channel to the message host. Each handle will also have separate event handlers, and synchronous message responses need to be handled on the same bus as the request.

Singleton instantiation
-----------------------

Succubus allows you to get a singleton instance in addition to newing up objects. Note that the singleton object will always (obviously) point to the same instance, while all newed objects will be different. These can co-exist and will use separate message channels:

```C#
Bus.Instance.Initialize(config => {
    config.StartMessageHost =  true;
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
    succubus.StartMessageHost = true;
});
```

When using the parameterless Initialize call, the bus will be initialized with default values.

Configuration
-------------

There are a number of methods that allow you to configure your bus instance before using it. These calls can be found in the `IBusConfigurator`-interface, but will be documented in detail in the future.

```C#
        #region Communication        
        string PublishAddress { get; set; }
        string SubscribeAddress { get; set; }
        #endregion

        #region Hosting
        string MessageHostPublishAddress { get; set; }
        string MessageHostSubscribeAddress { get; set; }

        bool StartMessageHost { get; set; }
        #endregion

        #region Filtering
        string Network { get; set;  }        
        #endregion

        #region Configuration sources
        void GetFromConfigurationFile();
        #endregion
```

Most of these should be self explanatory. Network is currently not implemented.

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

### Response filtering

You can setup response filtering through inheritance. Example:

```C#
public class BaseResponse 
{
    public string Message { get; set; }
}

public class SuccessResponse 
{
    public SuccessResponse() 
    {
        Message = "All went well";
    }
}

public class FailureResponse 
{
    public FailureResponse() 
    {
        Message = "What a catastrophic failure.";
    }
}

public class Request 
{
    public string Message { get; set; }
}
```

With these classes we can setup the handling of the static routes.

```C#
bus.ReplyTo<Request,BaseResponse>((req) =>
{
    if (failure)
    {
        return new FailureResponse();
    }
    else 
    {
        return new SuccessResponse();
    }
});

```

Then, on the client side, we can handle both responses separately:

```C#
bus.OnReply<Request, FailureResponse>((req, res) =>
{
    Console.WriteLine("Shoot, something went wrong processing the request: {0}", res.Message);
});

bus.OnReply<Request, SuccessResponse>((req, res) =>
{
    Console.WriteLine("Everything processed without a hitch: {0}", res.Message);
});

```

It is also possible to do:

```C#
bus.OnReply<Request, BaseResponse>((req, res) =>
{
    if (res is FailureResponse) {
        // ...
    } else if (res is SuccessResponse) {
        // ...
    }
});
```

This means you can route incoming responses based on interface-definitions and abstract classes, in addition to obviously concrete classes.

### Timeouts

Synchronized processing supports timeout handlers on a per-`Call` basis. Example:

```C#
bus.Call(new BasicRequest { Message = "Hello! "}, (req) => {
	Console.WriteLine("BasicRequest timed out for: {0}", req.Message)
}, 2500); // Timeout in milliseconds
```

If no parameters are provided, the call will time out silently after 
one minute. To make a response handler never time out, manually provide
the value of 0.

### Orchestration

Synchronous processing in Succubus opens up for complex orchestration of up to 7 responses. Example:

```C#
bus.OnReply<UpdateRequest, 
            ImageProcessed, 
            FriendNotified>((ur, ip, fn) =>
{
	Console.WriteLine("New profile image has been processed with response: {0}", ip.Status);
	Console.WriteLine("Friends have been notified with response: {0}", fn.Status);
});
	
```

### Deferrence (0.3)

Succubus supports deferring message handling. This is convenient when you are doing synchronous processing, but the response set needs to be handled in another context than where the request was posted.

To use, simply:

```C#
bus.ReplyTo<BasicRequest, BasicResponse>(req => new BasicResponse {
	Message = "From server: " + req.Message;
});

bus.Defer<BasicRequest, BasicResponse>();

Guid cId = bus.Call(new BasicRequest { Message = "Deferred "});

bus.Pickup<BasicRequest, BasicResponse>(cId, (req, res) =>
{

});
```

The call to `Pickup` will _block_. Also, deferred calls will by default be held for one minute, or be removed from the bus as soon as they are picked up.


Workload management
-------------------
