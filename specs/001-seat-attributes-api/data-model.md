# Data Model: Seat Attributes API

**Date**: 2025-10-29  
**Branch**: 001-seat-attributes-api

## Entities

### Seat
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| AircraftModel | string | required, max 100 | Aircraft model identifier (e.g., A320) |
| SeatNumber | string | required, pattern `[0-9]+[A-F]` | Row + seat letter |
| Position | string | required, enum (Aisle, Middle, Window) | Physical seat position category |
| HasWindow | bool | required | Indicates real window presence |
| PowerAvailable | bool | required | Seat has power access |
| PowerType | string? | if PowerAvailable then required; enum (AC, USB, USB-C) | Type of power outlet |
| HasInSeatScreen | bool | required | Seat includes personal screen |
| ExperienceSummary | string? | max 500 | Short qualitative summary (legroom, noise) |
| CreatedAt | datetime | required | Record creation timestamp |
| UpdatedAt | datetime | required | Last update timestamp |

Primary Key: (AircraftModel, SeatNumber)
Indexes: PK plus index on Position for potential filtering.

### SeatNote
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | int | identity PK | Surrogate primary key |
| AircraftModel | string | required | FK to Seat.AircraftModel |
| SeatNumber | string | required | FK to Seat.SeatNumber |
| Text | string | required, max 500 | Public note content (plain text) |
| CreatedAt | datetime | required | Creation timestamp |
| UpdatedAt | datetime | required | Last update (for concurrency observation) |

Foreign Key: (AircraftModel, SeatNumber) references Seat.
Index: Composite (AircraftModel, SeatNumber), include UpdatedAt for note list ordering.

## Relationships
- Seat 1..* SeatNotes

## State Transitions
- SeatNote: Create → Update (overwrite text, UpdatedAt) → (optional) Delete

## Validation Rules
- SeatNumber pattern: row digits + seat letter (A–F)
- PowerType required iff PowerAvailable = true
- Note Text length <= 500 characters
- ExperienceSummary trimmed; store null if empty

## Concurrency
- Last-write-wins: application saves note; UpdatedAt reflects final state; clients re-fetch after update.

## Derived Data
- WindowConfirmed: equals HasWindow AND Position == Window

## Open Questions (Deferred)
- Extended seat comfort metrics (legroom numeric) deferred.
- Multi-airline support: not required for initial scope.

## Summary
Entities and validation align with specification and constitutional principles (data discipline, testability).
