# RabbitMQ Simple Demo (.NET)

A simple demonstration of RabbitMQ message queue system using .NET/C# with a producer and consumer.

## Prerequisites

- .NET 6.0 SDK or later
- Docker (for running RabbitMQ)

## Setup

### 1. Start RabbitMQ Server

Using Docker Compose (recommended):
```bash
docker-compose up -d
```

Or using Docker directly:
```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

This starts RabbitMQ with:
- Port 5672: AMQP protocol
- Port 15672: Management UI (accessible at http://localhost:15672, credentials: guest/guest)

### 2. Restore NuGet Packages

```bash
dotnet restore
```

## Running the Demo

### Option 1: Simple Message Queue

1. Start the consumer (in one terminal):
```bash
cd Consumer
dotnet run
```

2. Send messages with the producer (in another terminal):
```bash
cd Producer
dotnet run
```

### Option 2: Work Queue with Multiple Workers

1. Start multiple consumers in separate terminals:
```bash
# Terminal 1
cd Consumer
dotnet run

# Terminal 2
cd Consumer
dotnet run
```

2. Send multiple messages:
```bash
cd Producer
dotnet run
```

## Project Structure

```
RabbitMQDemo/
├── Producer/          # Message producer console app
├── Consumer/          # Message consumer console app
├── Shared/            # Shared models and utilities
├── docker-compose.yml # RabbitMQ setup
└── README.md
```

## What's Happening?

- **Producer**: Sends messages to the `hello_queue`
- **Consumer**: Receives and processes messages from the queue
- **RabbitMQ**: Acts as the message broker, storing and routing messages

## Architecture

```
Producer → RabbitMQ Queue → Consumer(s)
```

Messages are distributed among multiple consumers in a round-robin fashion.

## Features Demonstrated

- Message publishing and consuming
- JSON serialization
- Manual message acknowledgment
- Durable queues (messages persist)
- Quality of Service (QoS) settings
- Multiple consumers with fair dispatch

## Cleanup

Stop and remove RabbitMQ container:
```bash
docker-compose down
```