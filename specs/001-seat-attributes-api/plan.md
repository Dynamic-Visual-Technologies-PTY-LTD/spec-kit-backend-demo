# Implementation Plan: Seat Attributes API

**Branch**: `001-seat-attributes-api` | **Date**: 2025-10-29 | **Spec**: ./spec.md
**Input**: Feature specification from `./spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Minimal API endpoints for seat attribute retrieval and public note management. Includes
GET seat attributes, GET/POST/PUT/DELETE seat notes. Last-write-wins concurrency for note
updates. EF Core with SQL Server; OpenAPI + Scalar UI; Observability via OpenTelemetry and
Serilog. TDD enforced: write failing unit, integration, contract tests first.

## Technical Context

**Language/Version**: .NET 10 (C# 13)  
**Primary Dependencies**: EF Core, Serilog, OpenTelemetry, Scalar, xUnit, Moq  
**Storage**: SQL Server (tables: Seats, SeatNotes)  
**Testing**: xUnit (unit/integration), contract tests (OpenAPI + HTTP), Moq for service isolation  
**Target Platform**: Cross-platform .NET backend (container-ready)  
**Project Type**: Single backend API service  
**Performance Goals**: p95 < 250ms for seat GET; p95 < 300ms for note CRUD  
**Constraints**: Plain text notes <= 500 chars; last-write-wins; no auth; minimal dependencies  
**Scale/Scope**: <10k seats; <100 notes per seat; moderate traffic

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Principles Coverage:
I (TDD): Plan includes failing tests first for each endpoint + concurrency.
II (Comprehensive Testing): Unit (validation/domain), integration (DbContext queries), contract (OpenAPI + status codes).
III (Data Access): EF Core only; migrations; composite keys; no lazy loading.
IV (Documentation): Feature docs under `/docs/features/seat-attributes/*` delivered with code.
V (Observability): Spans + structured logs defined; correlation id; error logging.
VI (Minimal API & OpenAPI): Routes defined; OpenAPI generation; Scalar UI reference.

Gate Status: PASS.

## Project Structure

### Documentation (this feature)

```text
specs/001-seat-attributes-api/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md (later via /speckit.tasks)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
src/
├── seat-viewer-api/
├── seat-viewer-domain/
├── seat-viewer-infrastructure/

tests/seat-viewer-tests/
├── unit/
├── integration/
└── contract/
```

**Structure Decision**: Use existing solution projects. Domain entities + services in `seat-viewer-domain`; persistence/migrations in `seat-viewer-infrastructure`; Minimal API endpoints + OpenAPI in `seat-viewer-api`; tests segregated by category.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| Last-write-wins concurrency | Simplicity and acceptable for public notes | Optimistic concurrency adds version mgmt complexity |
| (Potential) Repository abstraction (deferred) | Keep code lean initially | Adds indirection without current benefit |
