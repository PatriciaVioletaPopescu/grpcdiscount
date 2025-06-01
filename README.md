Discount Code Microservice

This is a lightweight gRPC-based service for managing discount codes. It allows clients to generate and redeem discount codes with validation and database persistence.

Technologies Used:
.NET 8
gRPC
Entity Framework Core
SQLite
Microsoft.Extensions.ILogger with Serilog
NUnit and Moq for unit testing

Features:
Generate unique discount codes with custom count and length
Use (consume) a discount code safely (concurrent-safe)
Configurable via appsettings.json
Parallel client simulation for stress testing