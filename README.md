# ScyllaDB Verification Project

A .NET 9 console application demonstrating connectivity and basic operations with ScyllaDB Cloud using the DataStax C# Driver.

## Overview

This project provides a simple demonstration of:
- Connecting to ScyllaDB Cloud
- Creating keyspaces and tables
- Performing batch inserts with parallel processing
- Executing queries
- Basic performance metrics tracking

## Prerequisites

- .NET 9.0 SDK
- ScyllaDB Cloud account
- DataStax C# Driver for Apache Cassandra

## Features

- Cloud connectivity demonstration
- Keyspace and table creation
- Parallel data insertion
- Basic query operations
- Performance metrics logging
- Connection pooling

## Getting Started

1. Update the connection configuration in `Program.cs` with your ScyllaDB Cloud credentials:
```csharp
.AddContactPoints("your-node-endpoints")
.WithAuthProvider(new PlainTextAuthProvider("username", "password"))
```

2. Run the application:
```bash
dotnet run
```

## Code Structure

The application demonstrates:
- ScyllaDB connection setup
- Database schema creation
- Parallel data insertion using `Parallel.For`
- Basic data querying
- Performance measurement using `Stopwatch`

## Performance Notes

The application includes performance measurement for:
- Batch insert operations
- Query operations
- Average write time per record

## Security Note

Please ensure to keep your ScyllaDB credentials secure and never commit them directly in the code. Consider using environment variables or secure configuration management in production environments.

## Dependencies

- Cassandra.Driver: DataStax C# Driver for Apache Cassandra
- System.Diagnostics.DiagnosticSource: For performance monitoring
