Succubus
========

A .NET "service bus" implementation created on top of ZeroMQ.

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
