Succubus
========

A light .NET "service bus" implementation created on top of ØMQ/ZeroMQ.

About
-----

Succubus is a simple (at this point) implementation of convenience functions for .NET on top of ØMQ, presenting an interface similar to that of MassTransit. It is similar to an Enterprise Service Bus, but lacks a lot of features at its current state.

Because it uses ØMQ, all brokers live in the Succubus process space, and you do not install anything besides the dependencies in the library.

![A nice succubus](http://images3.wikia.nocookie.net/__cb20130714043348/valkyriecrusade/images/f/fe/Queen_Succubus_SR.PNG "A Succubus")


Getting started
---------------

A quickstart guide to getting started with Succubus.

Instantiation
-------------

To instantiate a handle to the bus, create an instance of `Succubus.Core.Bus`:

	IBus bus = new Succubus.Core.Bus();

Each handle will have a separate channel to the message host.

Singleton instantiation
-----------------------

Succubus allows you to get a singleton instance in addition to newing up objects. Note that the singleton object will always (obviously) point to the same instance, while all newed objects will be different. These can co-exist and will use separate message channels:

	Bus.Instance.Initialize(config => {
	    config.UseMessageHost();
	});

Initialization
--------------

Before using the bus, it needs to be initialized:

	Bus.Instance.Initialize();

The `Initialize`-call to the bus alternatively returns a configuration handle.

    bus.Initialize(succubus =>
    {
        succubus.UseMessageHost();               
    });

When using the parameterless Initialize call, the bus will be initialized with default values.

Configuration
-------------

There are a number of methods that allow you to configure your bus instance before using it. These calls can be found in the `IBusConfigurator`-interface, but will be documented in detail in the future.

    public interface IBusConfigurator
    {        
        void UseMessageHost(int publishPort = 9000, int subscribePort = 9001, bool setupHost = true);
        void UseMessageHost(IMessageHost messageHost);
        void SetNetwork(string networkName);
        void SetMessageHostname(string hostname);
    }

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

Synchronous processing
----------------------

Orchestration
-------------

Workload management
-------------------
