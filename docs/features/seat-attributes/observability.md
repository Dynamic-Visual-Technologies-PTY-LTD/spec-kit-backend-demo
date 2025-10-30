# Observability Strategy

## Overview

The Seat Viewer API implements comprehensive observability through structured logging, distributed tracing, and correlation ID tracking. This document outlines the observability architecture, configuration, and best practices.

## Logging Architecture

### Structured Logging with Serilog

The API uses **Serilog** for structured logging with the following configuration:

- **Console Sink**: Real-time log output during development and production
- **File Sink**: Rolling daily log files with 30-day retention (production)
- **Structured Format**: JSON-compatible property enrichment for log aggregation

**Key Features:**
- Correlation ID tracking via `HttpContext.TraceIdentifier`
- Request/response logging with timing information
- Exception details with stack traces (development only)
- Environment-specific log levels

### Log Levels

| Environment | Default Level | Microsoft Overrides | EF Core Overrides |
|-------------|---------------|---------------------|-------------------|
| Development | Debug | Warning | Information |
| Production | Information | Warning | Warning |

### Log Enrichment

All logs are enriched with:
- **CorrelationId**: Request trace identifier for tracking across services
- **Environment**: Deployment environment (Development/Staging/Production)
- **Application**: "SeatViewerAPI"
- **MachineName**: Server hostname (production)
- **ThreadId**: Execution thread for concurrency troubleshooting

**Example Log Entry:**
```json
{
  "Timestamp": "2025-10-29T10:30:45.123Z",
  "Level": "Information",
  "Message": "Retrieving seat attributes",
  "Properties": {
    "CorrelationId": "00-abc123...-def456...-00",
    "AircraftModel": "A320",
    "SeatNumber": "12A",
    "Environment": "Production"
  }
}
```

---

## Distributed Tracing

### OpenTelemetry Configuration

The API integrates **OpenTelemetry** for distributed tracing with the following instrumentation:

1. **ASP.NET Core Instrumentation**
   - HTTP request/response tracing
   - Exception recording
   - Correlation ID tagging (`http.request.correlation_id`)

2. **HTTP Client Instrumentation**
   - Outbound HTTP calls (for future service-to-service communication)

3. **Entity Framework Core Instrumentation**
   - Database query tracing
   - SQL statement logging (development only)

**Service Identity:**
- Service Name: `seat-viewer-api`
- Service Version: `0.1.0`

### Trace Spans

Each endpoint creates Activity spans for key operations:

**Example Span Hierarchy:**
```
HTTP GET /seats/A320/12A
├─ SeatEndpoints.GetSeatAttributes
│  ├─ SeatValidation.IsValidSeatNumber
│  └─ SeatQueryService.GetSeatAsync
│     └─ EF Core: SELECT FROM Seats WHERE...
```

**Span Tags:**
- `seat.aircraft_model`: Aircraft model identifier
- `seat.seat_number`: Seat number
- `http.request.correlation_id`: Request trace identifier
- `validation.result`: Validation success/failure

---

## Correlation ID Strategy

### Request Tracking

Every HTTP request is assigned a unique **Correlation ID** via `HttpContext.TraceIdentifier`:

- **Format**: W3C Trace Context format (`00-{trace-id}-{span-id}-{flags}`)
- **Propagation**: Automatically included in response headers
- **Enrichment**: Added to all logs and trace spans

### Cross-Service Correlation

For future microservice architectures:
1. Extract `traceparent` header from incoming requests
2. Propagate correlation ID to downstream services via HTTP headers
3. Link parent/child spans in distributed traces

**Example Usage:**
```http
GET /seats/A320/12A HTTP/1.1
traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01

HTTP/1.1 200 OK
traceresponse: 00-4bf92f3577b34da6a3ce929d0e0e4736-b9c7c989f97918e1-01
```

---

## Error Tracking

### Global Error Handling Middleware

The `ErrorHandlingMiddleware` provides:
- **Uniform Problem Details**: RFC 7807 compliant error responses
- **Exception Logging**: Detailed error logs with correlation IDs
- **Environment-Specific Details**: Stack traces in development only
- **HTTP Status Mapping**: Consistent status codes for exception types

**Error Response Example:**
```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Bad Request",
  "status": 400,
  "detail": "Invalid seat number format. Expected format: digits + letter A-F",
  "instance": "/seats/A320/INVALID",
  "traceId": "00-abc123...-def456...-00"
}
```

---

## Monitoring & Alerting

### Key Metrics to Monitor

1. **Request Performance**
   - P50, P95, P99 latency per endpoint
   - Request rate (req/sec)
   - Error rate (5xx responses)

2. **Database Performance**
   - Query execution time
   - Connection pool utilization
   - Failed transactions

3. **Application Health**
   - Memory usage
   - CPU utilization
   - Unhandled exceptions

### Recommended Observability Tools

| Tool | Purpose | Integration |
|------|---------|-------------|
| **Application Insights** | Azure-native APM | OpenTelemetry exporter |
| **Prometheus** | Metrics collection | OpenTelemetry metrics exporter |
| **Grafana** | Visualization | Prometheus data source |
| **Seq** | Structured log aggregation | Serilog Seq sink |
| **Jaeger** | Distributed tracing | OpenTelemetry OTLP exporter |

---

## Configuration Examples

### Production Logging Configuration

**appsettings.Production.json:**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "ApplicationInsights",
        "Args": {
          "instrumentationKey": "{INSTRUMENTATION_KEY}",
          "restrictedToMinimumLevel": "Information"
        }
      }
    ]
  }
}
```

### OpenTelemetry OTLP Exporter

To export traces to Jaeger or Application Insights:

```csharp
.WithTracing(tracing => tracing
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("https://your-collector:4318");
        options.Protocol = OtlpExportProtocol.HttpProtobuf;
    }));
```

---

## Troubleshooting Guide

### Viewing Logs Locally

**Console Output:**
```bash
dotnet run --project src/seat-viewer-api
# Logs appear in console with structured format
```

**File Logs:**
```bash
# Logs are written to logs/seat-viewer-api-{date}.log
tail -f logs/seat-viewer-api-20251029.log
```

### Extracting Correlation IDs

To trace a specific request across logs:

```bash
# grep for correlation ID in log files
grep "00-abc123...-def456...-00" logs/*.log

# Or query structured log platform (e.g., Seq)
CorrelationId = "00-abc123...-def456...-00"
```

### Performance Investigation

1. Check P95 latency in performance tests
2. Review slow query logs (EF Core logging)
3. Analyze trace spans for bottlenecks
4. Profile database execution plans

---

## Best Practices

### Development
- Use **Debug** level for detailed troubleshooting
- Enable SQL logging to debug query performance
- Review trace spans in console exporter

### Production
- Use **Information** level minimum
- Export logs to centralized aggregation platform
- Configure alerting for error rate thresholds
- Monitor correlation IDs for distributed transaction tracking
- Implement log retention policies (30+ days recommended)

### Security
- Never log sensitive data (passwords, tokens, PII)
- Sanitize user input in log messages
- Use structured logging properties to avoid log injection
- Restrict stack traces to non-production environments

---

## Future Enhancements

1. **Metrics Instrumentation**
   - Add OpenTelemetry metrics for request rates, latencies
   - Expose Prometheus `/metrics` endpoint

2. **Health Checks**
   - Implement `/health` endpoint with database connectivity checks
   - Add readiness/liveness probes for Kubernetes

3. **Custom Correlation**
   - Support custom correlation ID headers for API consumers
   - Propagate user context for multi-tenant scenarios

4. **Real-Time Monitoring**
   - Integrate Application Insights for Azure deployments
   - Configure alert rules for critical failures

---

## References

- [Serilog Documentation](https://serilog.net/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [RFC 7807: Problem Details](https://tools.ietf.org/html/rfc7807)
- [W3C Trace Context](https://www.w3.org/TR/trace-context/)
