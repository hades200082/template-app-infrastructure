# What are commands?

In CQRS and Event Driven Development (EDD) an command is defined as a notification sent by one service and 
received/acted upon by one (and only one) other services. The service that acts upon the command may be the sending
service but more commonly it is an external service.

Commands in a message bus based system will typically not expect a response of any kind. If responses are required
then the handling service should issue its own command or event in response and the sending service should handle that
in turn.

In Mediator pattern an event would be referred to as a "Command" and they are "Sent". 

Such a command may have only a single subscriber/handler.

