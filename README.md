# eventstore-bank-demo

This repo contains a simplified bank simulation project using Eventsourcing and showcasing Event Store features

## What and why

I work for a modern cloud-based fintech clearing bank in the UK and we recently produced some internal eventing best practice principals outlining what domain services must, should and could do when communcating with services in other domains via events. 

I have built an eventsourced system using Event Store in the past and wanted to demonstrate how these best practice principals could potentially be achieved using Event Store, plus some other benefits along the way.

For anyone else interested in eventsourcing and/or Event Store, there dont seem to be many decent sized example projects available, so feel free to use this and of course contribute if you spot something I'm doing wrong or that could be improved upon.

THIS IS DEMO CODE, please don't judge me!

## The scenario

This repo models a simplified bank with several seperate 'domains' (these are bascially aspnetcore api projects):

* accounts-api - this 'domain' owns some accounts and their transactions and therefore balance ledgers
* sanctions-api - this 'domain' owns a list of sanctioned names and should be checked before allowing any transaction to be created in or out
* payment-scheme-domain - this 'domain' represents a payment scheme such as Bacs, it will process inbound payments from the 'scheme' and send outbound payments to the 'scheme'

* paymentscheme-simulator - this component mocks a payment scheme such as Bacs etc, it creates events to initiate inbound payments and recieves outbound payments

There are also some shared projects

* events - contains all event definition classes and stream names etc
* infrastructure - contains reusable implementations
* data-seeder - contains code to initially seed accounts and create persistent subscriptions etc, to be run once up-front

#### Some assumptions / aims

* Event Store is shared, but most streams should be private to a domain
* As far as possible each domain should be separate

## Payment flow

in eventsourcing anything of interest to the system is modeled as an event, with a name which describes what has happened in past tense:

happy path -> `paymentReceivedEvent` -> `paymentValidatedEvent` -> `sanctionsCheckedEvent` -> `accountStatusCheckedEvent` -> `balanceUpdatedEvent`

if payment not valid then `paymentReturnedEvent` or `paymentFailedEvent`

if sanctioned == true -> `paymentHeldEvent` -> manual override (continue) or `paymentFailedEvent`

if accountStatus == blocked or closed or insufficientFunds -> `paymentFailedEvent`

## Event Store / eventsourcing Features to be demonstrated

* immutable events in streams, writing, reading and subscribing
* builtin Event Store projections
* custom js Projections
* read models - a component which represents an aggregate by maintaining state read from events
* competing consumers - a server managed subscription, where events are dealt to one or more subscribers via a strategy such as round robin, enables parallel processing a bit like a message bus
* replay message / DLs for competing consumers
* fetch historical state
* request old individual events
* mutable store backed by immutable stream - how to back an effectively mutable collection using imutable events and snapshots
* add new functionality which works from beginning of time (fraud analysis pattern??)

## ToDo

- [x] define events
- [x] basic eventstore connection
- [x] basic payment-scheme-simulator to simulate inbound payment events
- [ ] payment-scheme-simulator API (different types of payment, select account, invalid payment, payment which will fail etcpayment-)
- [ ] generic payment scheme domain, with API for initiating payments and event handlers as per above
- [x] sanctions API
- [x] add initial Accounts API
- [ ] add transactions/balance to accounts-api, using ReadModels
- [ ] setup a jsProjection for OpenAccounts with status?
- [ ] add a read model (projection) of accounts and statuses data in a scheme
- [ ] add account time machine
- [ ] add event replay / request old events
- [ ] UI blazor wasm?
- [ ] demonstrate how the various handlers/subscriptions etc can be tested
- [ ] demonstrate potential strategies for scheduling future events?

