# Research: Seat Attributes API

**Date**: 2025-10-29  
**Feature**: Seat Attributes API  
**Branch**: 001-seat-attributes-api  

## Decisions & Rationale

### Note Length Limit
- **Decision**: 500 characters max
- **Rationale**: Concise enough to avoid moderation overhead; sufficient detail for seat tips
- **Alternatives Considered**: 255 (too short), 1000 (risk of verbose entries), 2000 (storage bloat)

### Concurrency Strategy for Notes
- **Decision**: Last-write-wins
- **Rationale**: Minimizes complexity; acceptable for non-auth public data; low risk of conflict severity
- **Alternatives Considered**: Optimistic concurrency (adds version mgmt), Pessimistic locking (risk deadlocks), timestamp reject (higher user friction)

### Sanitization Approach
- **Decision**: Plain text only, strip markup
- **Rationale**: Simplifies security, avoids XSS concerns since notes are public and untrusted
- **Alternatives Considered**: Allow limited Markdown (adds parsing & sanitization), HTML whitelist (higher complexity)

### Data Model Keys
- **Decision**: Composite key (AircraftModel + SeatNumber) for Seat; SeatNote uses surrogate Id + composite FK
- **Rationale**: Natural uniqueness, efficient lookups
- **Alternatives Considered**: GUID primary for Seat (unnecessary indirection)

### Observability Scope
- **Decision**: Spans for: seat lookup, note create/update/delete, DB query; logging includes correlation id and elapsed ms
- **Rationale**: Core tracing coverage aligns with Principles V & VI; extensible later for metrics
- **Alternatives Considered**: Include metrics now (deferred until usage profile known)

### OpenAPI Style
- **Decision**: Separate endpoints under /seats; schemas: SeatAttributes, SeatNote
- **Rationale**: Clear hierarchical routing, predictable contract
- **Alternatives Considered**: /notes?model=...&seat=... (less semantic grouping)

## Unresolved / Deferred
- Metrics exposure (latency histogram, error counters) deferred until baseline traffic known
- Caching strategy (ETag or server-side) deferred

## Summary
All clarifications resolved; no blockers remain for Phase 1 design artifacts.
