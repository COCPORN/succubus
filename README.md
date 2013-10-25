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

Succubus allows you to get a 

Initialization
--------------

Initialize the bus returns a configuration handle.

    bus.Initialize(succubus =>
    {
        succubus.UseMessageHost();               
    });

