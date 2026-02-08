# RabbitMQ Demo (.NET 10)

A simple RabbitMQ demo using .NET 10. One console app can run as a **producer** (sends messages) or **consumer** (receives messages) via command-line arguments.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- RabbitMQ server (e.g. via Docker)

## Project structure

```
RabbitMQ-demo/
├── Program.cs         # Entry point; producer and consumer logic
├── MessageModel.cs    # Shared message model (Id, Text, Timestamp)
├── rabbitmq-demo.csproj
├── .vscode/           # Launch and task config for VS Code / Cursor
└── README.md
```

## Setup

### 1. Start RabbitMQ

With Docker:

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```

- **5672** – AMQP
- **15672** – Management UI: http://localhost:15672 (login: `guest` / `guest`)

### 2. Restore and build

```bash
dotnet restore
dotnet build
```

## Running the demo

### Show usage (help)

```bash
dotnet run
# or
dotnet run help
```

### Run the consumer (then keep it running)

In one terminal:

```bash
dotnet run consumer
```

Leave this running. Stop with **Ctrl+C** when done (stop it before running `dotnet run` again in the same solution, or the build may fail because the exe is locked).

### Run the producer (send messages)

In another terminal:

```bash
dotnet run producer
```

Aliases: `dotnet run send` (producer), `dotnet run receive` (consumer).

## Configuration

Connection settings are in `Program.cs` at the top:

- **HostName**: `localhost`
- **Port**: `5672`
- **UserName** / **Password**: `guest` / `guest`
- **Queue**: `demo-queue`

Change these if your RabbitMQ runs elsewhere or uses different credentials.

## What it demonstrates

- **Producer**: Connects, declares `demo-queue`, publishes three JSON messages (`MessageModel`), then exits.
- **Consumer**: Connects, declares the same queue, consumes messages and prints them to the console until you press Ctrl+C.
- **MessageModel**: Simple type with `Id`, `Text`, and `Timestamp` (JSON serialized).

## Tech stack

- .NET 10
- [RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client) 7.2.0
- Async API: `CreateConnectionAsync`, `CreateChannelAsync`, `AsyncEventingBasicConsumer`, `BasicConsumeAsync`

## Cleanup

Stop and remove the RabbitMQ container:

```bash
docker stop rabbitmq
docker rm rabbitmq
```
