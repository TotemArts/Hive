# Hive.Endpoints.Server
This is the enpoint that will receive the incoming data from the GameServers, process it from strings to classes and create events

## Explanation of projects
### API
All incoming and outgoing traffic go through the API, this turns the incoming traffic into commands to send to the worker to process

### Contracts
API and Worker import this in order to publish commands and events

### Domain
This is the information on which Infrastructure relies upon, contains the interfaces for the data in the repositories, and the api of the repositories

### Infrastructure
This is the project which contains the interaction with any database, called repositories

### Worker
This is the project that actually handles incoming commands, processes them and turns them into events
