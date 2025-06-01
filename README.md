# Discount Code Microservice

A lightweight gRPC-based service for managing discount codes. This microservice enables clients to generate and redeem discount codes with validation and database persistence.

## Technologies Used

- .NET 8
- gRPC
- Entity Framework Core
- SQLite
- Microsoft.Extensions.Logging with Serilog
- NUnit and Moq for unit testing

## Features

- Generate unique discount codes with configurable count and length
- Redeem discount codes safely with concurrency handling
- Configuration via `appsettings.json`
- Parallel client simulation for stress testing
