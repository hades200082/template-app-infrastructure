# What are events?

In CQRS and Event Driven Development (EDD) an event is defined as a notification sent by one service and 
received/acted upon by zero or more services (potentially also including the sending service).

In Mediator pattern an event would be referred to as a "Notification" and is "Published". 

Such a notification may have multiple subscribers/handlers.