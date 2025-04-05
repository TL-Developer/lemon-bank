# 💰 Bem Vindos a corretora LEMON BANK 💰

## Project Architecture

![alt text](image-1.png)

# Lemon Bank - B3 Market Data Producer

This project simulates a market data producer that generates stock market data and sends it to Kafka. It's part of a larger system that processes financial market data.

## Project Structure

```
lemon-bank/
├── b3/                    # .NET Core service that produces market data
│   ├── Worker.cs         # Background service that generates and sends market data
│   ├── Program.cs        # Application entry point
│   ├── appsettings.json  # Configuration file
│   └── Dockerfile        # Docker configuration for the .NET service
├── docker-compose.yml    # Docker Compose configuration
└── README.md             # This file
```

## Services

The system consists of the following services:

1. **b3** - .NET Core service that:
   - Generates simulated market data
   - Sends data to Kafka
   - Runs as a background service

2. **Kafka** - Message broker that:
   - Receives market data from the b3 service
   - Stores messages in topics
   - Runs on port 9092 (external) and 29092 (internal)

3. **Zookeeper** - Required by Kafka for:
   - Cluster coordination
   - Configuration management
   - Runs on port 2181

4. **Kafka UI** - Web interface for:
   - Monitoring Kafka topics
   - Viewing messages
   - Managing Kafka
   - Accessible at http://localhost:8080

## Prerequisites

- Docker
- Docker Compose

## Running the Project

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd lemon-bank
   ```

2. Start all services using Docker Compose:
   ```bash
   docker-compose up
   ```

3. Access the services:
   - Kafka UI: http://localhost:8080
   - Kafka broker: localhost:9092

## Development

### Building the .NET Service

The b3 service is automatically built by Docker Compose. However, if you need to build it separately:

```bash
cd b3
dotnet build
```

### Environment Variables

The following environment variables are used:

- `ASPNETCORE_ENVIRONMENT`: Set to "Development" in docker-compose.yml
- `Kafka__BootstrapServers`: Set to "kafka:29092" for internal Docker network communication

## Monitoring

You can monitor the system using:

1. **Kafka UI** (http://localhost:8080):
   - View topics
   - Monitor messages
   - Check consumer groups

2. **Docker Logs**:
   ```bash
   docker-compose logs -f b3
   ```

## Stopping the Services

To stop all services:

```bash
docker-compose down
```

## Troubleshooting

If you encounter connection issues:

1. Ensure all containers are running:
   ```bash
   docker-compose ps
   ```

2. Check container logs:
   ```bash
   docker-compose logs
   ```

3. Verify Kafka is accessible:
   ```bash
   docker-compose exec kafka kafka-topics --list --bootstrap-server localhost:9092
   ```
