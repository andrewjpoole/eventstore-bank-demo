# eventstore-bank-demo
simple bank simulation project showing eventstore features

Features to be demonstrated
========================
/ immutable events in streams
* jsProjections
* read models - for accounts projections for a scheme domain
/ competing consumers on payment validation/processing? -> model flow of one scheme (bacs)
* fetch state from past for an account?
* replay message / DLs for competing consumers
* snapshots
* request old events?
/ mutable store backed by immutable stream?
* add new functionality which works from beginning of time (fraud analysis pattern??)

Domains
-------

Accounts (new accounts, status changes, closures etc, balances etc)
 - stream Account-[sortcode]-[accountNumber]-details (Name, Address, State, Details)
 - stream Account-[sortcode]-[accountNumber]-transactions (CreditTransaction, DebitTransaction)

BScheme (batched bulk payments simplified DC and DD etc)

//CScheme (low volume high value single payments in and out (initiated by customer via API))

//FScheme (simplified FPS frequent low value payments in and out (initiated by customer via API))

Sanctions (list of restricted individuals, changed manually via API)

Audit? held payments, failed payments etc (js projections)

Scenarios:
==========
paymentReceivedEvent -> paymentValidatedEvent -> sanctionsCheckedEvent -> accountStatusCheckedEvent -> balanceUpdatedEvent
- if payment not valid then paymentReturnedEvent or paymentFailedEvent?
- if sanctioned == true -> paymentHeldEvent -> continue or paymentFailedEvent
- if accountStatus == blocked or closed or insufficientFunds -> paymentFailedEvent

ToDo
========
/ define events
/ basic eventstore connection
/ basic payment-scheme-simulator to simulate inbound payment events
- payment-scheme-simulator API (different types of payment, select account, invalid payment, payment which will fail etcpayment-)
- generic payment scheme domain, with API for initiating payments and event handlers as per above
/ sanctions API
/ add initial Accounts API
- add transactions/balance to accounts-api, using ReadModels
- setup a jsProjection for OpenAccounts with status?
- add a read model (projection) of accounts and statuses data in a scheme
- add account time machine
- add event replay / request old events
- UI blazor wasm?

