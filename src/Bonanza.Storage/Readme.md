# Some terms vfrom CQRS +ES

# Event Stream

Set of events representing the state of each Aggregate is captured in an append-only EventStream

Event stream of each Aggregate is usually persisted in EventStores, where they are uniquely distinguished, usually by the identity of the root entity

# Application service

When application service receives control, it loads an Aggregate and retrieves any supporting Domain Services needed by the Aggregate's business operation.

When App service delegates to the Aggregate business operation , the Aggregate's method produces Events as the outcome. Those events mutate state of the aggregate.

# Suscribers

In simplest implementation, there will be a back-ground processor that catches up with newly appended Events and publishes them to a messaging infrastructure (RabitMq,JMS,MSMQ, cloud queues), delivering them to all interested parties

# Command handler

Command handler effectively replaces Application Service method. Although it is roughly equivalent and may still be referred to as such.
Anyway, decoupling the client from service can enhance load balancing, enamble competing consumers, and support sustem partitioning.

We can spread the load by starting the same CommandHandler (semantically Applicayion Service) on any number of servers. Command Messages can be delivered to one of the several Command handlers.

