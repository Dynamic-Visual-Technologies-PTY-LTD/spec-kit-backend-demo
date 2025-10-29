<!--
Sync Impact Report
Version change: undefined → 1.0.0
Modified principles: (initial adoption) none renamed
Added sections: Core Principles, Additional Constraints, Development Workflow & Quality Gates, Governance
Removed sections: Template placeholders only
Templates requiring updates:
  - .specify/templates/plan-template.md ✅ updated
  - .specify/templates/spec-template.md ✅ updated
  - .specify/templates/tasks-template.md ✅ updated
  - .specify/templates/agent-file-template.md ⚠ pending (auto-generated later)
Deferred TODOs: None
-->

# Plane Seat Viewer Constitution

## Core Principles

### I. Test-Driven Development (NON-NEGOTIABLE)
All production code MUST be preceded by failing automated tests. The Red → Green → Refactor
cycle is enforced: write tests, confirm failure, implement minimal code to pass, refactor
with tests green. No feature merges without full passing test suite. New tests MUST express
clear intent and be isolated. Regression reproduction requires adding a failing test before
the fix. Fast unit tests gate changes; integration/contract tests run in CI.

### II. Comprehensive Automated Testing
Every feature MUST provide: unit tests (logic/contracts), integration tests (data access,
API wiring), and contract tests (Minimal API + OpenAPI spec fidelity). Test coverage MUST
include all public endpoints and domain services. Flaky tests MUST be quarantined and fixed
before feature completion. Builds and test runs are automatically triggered on every push
and pull request. No manual QA replaces automated coverage.

### III. Data Access & Persistence Discipline
SQL Server is the authoritative data store. Entity Framework Core (EF Core) is the ONLY ORM
layer; no ad‑hoc SQL embedded in application code except performance‑justified, reviewed
raw queries. Migrations MUST be repeatable and versioned. Domain models map cleanly to EF
entities; avoid anemic models by placing behavior in domain services. Transactions MUST be
explicit for multi-entity changes. Lazy loading is disabled unless explicitly justified.

### IV. Documentation as a First-Class Artifact
All feature and architectural documentation MUST reside under `/docs`. Every new feature
adds or updates: overview, data model (if relevant), API contract (OpenAPI), and usage
examples. Docs MUST be updated in the same PR as code changes—no deferred doc tasks.
Outdated documentation MUST be removed or corrected immediately. Diagrams prefer text-first
formats (PlantUML / Mermaid) where applicable.

### V. Observability & Structured Telemetry
OpenTelemetry instrumentation MUST trace inbound API requests, database operations, and
critical domain workflows. Serilog provides structured logging (JSON for production,
human-readable for local). Logs MUST include correlation IDs. No sensitive PII in logs.
Failures MUST emit error-level logs with context. Traces + logs form a cohesive triad for
diagnostics. Metrics (e.g., request latency, error counts) SHOULD be exposed when infra
available.

### VI. Minimal API & OpenAPI Contract Integrity
The service uses .NET 10 Minimal APIs for lightweight endpoint definitions. OpenAPI (OAS)
spec generation MUST occur at build time; the Scalar UI MUST be available for interactive
exploration locally. Each endpoint MUST: define clear route, input validation, response
types, and documented error codes. Breaking contract changes REQUIRE a versioned path or
compatible evolution strategy.

## Additional Constraints

1. Technology Stack: .NET 10, SQL Server, EF Core, Serilog, OpenTelemetry, Minimal APIs,
	OpenAPI + Scalar.
2. Directory Structure: Source under `src/`, tests under `tests/`, documentation under
	`docs/` only.
3. Security & Compliance: Secrets MUST NOT be committed; configuration uses environment
	variables or secret managers.
4. Performance Expectations: Endpoint p95 latency SHOULD remain < 250ms for typical seat
	queries; slow queries MUST be traced and logged with timing.
5. Dependency Discipline: Prefer built-in BCL and framework features before adding packages.
6. CI Enforcement: Every PR MUST run build + full test suite + OpenAPI validation.

## Development Workflow & Quality Gates

1. Branch Naming: `feature/<short-kebab>` or `fix/<short-kebab>`.
2. Pull Requests MUST include: linked issue/feature, documentation updates, passing tests,
	and confirmation of observability instrumentation.
3. Quality Gates (all MUST pass):
	- TDD evidence: new tests existed before implementation (review diff shows tests first)
	- All required test categories pass (unit, integration, contract)
	- OpenAPI spec generated & served via Scalar locally
	- EF Core migrations updated/applied for persistence changes
	- Logging & tracing present for new endpoints/services
	- Documentation changes in `/docs`
4. Code Review Checklist includes verification of principles I–VI.
5. Fast Feedback: Aim < 5 min CI duration for PR validation; optimize tests if exceeded.
6. Refactoring: Allowed only with unchanged public contracts OR accompanied by a minor
	version bump justification in PR description.

## Governance

1. Supremacy: This Constitution supersedes conflicting prior informal practices.
2. Amendments: Proposed via PR modifying this file + Sync Impact Report. Require approval
	from at least one senior maintainer.
3. Versioning Policy (Semantic):
	- MAJOR: Principle removal/redefinition or governance process change breaking previous
	  expectations.
	- MINOR: New principle or substantial expansion of guidance.
	- PATCH: Wording clarity, typo fixes, non-semantic refinements.
4. Ratification: Initial adoption sets ratification date; subsequent amendments update
	Last Amended date only.
5. Compliance Reviews: Quarterly review ensures continued adherence; unresolved violations
	create remediation tasks before new feature work.
6. Exception Process: Temporary deviations require: written justification, scope, expiry
	date, and approval; documented in PR description.
7. Traceability: Each feature plan MUST list which principles it exercises and any
	exceptional handling.

**Version**: 1.0.0 | **Ratified**: 2025-10-29 | **Last Amended**: 2025-10-29

