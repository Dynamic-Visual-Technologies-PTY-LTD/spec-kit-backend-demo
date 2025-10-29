# Tasks: Seat Attributes API

**Branch**: `001-seat-attributes-api`  
**Spec**: ./spec.md  
**Plan**: ./plan.md  
**Data Model**: ./data-model.md  
**Contracts**: ./contracts/openapi.yaml

---
## Phase 1: Setup
Purpose: Establish baseline solution enhancements, database configuration, and test project structure required before domain and endpoint implementation.

Independent Completion Criteria: Running `dotnet test` executes empty categorized test directories; DB connection string resolved via configuration; Serilog + OpenTelemetry packages installed and bootstrap logging works; Scalar UI loads (placeholder) without endpoint implementations.

Checklist:
- [ ] T001 Add EF Core, Serilog, OpenTelemetry, Scalar NuGet packages to `src/seat-viewer-api/seat-viewer-api.csproj`
- [ ] T002 Configure Serilog bootstrap logger in `src/seat-viewer-api/Program.cs` (console JSON in Development)
- [ ] T003 Add OpenTelemetry tracing setup (resource attributes, ASP.NET Core, HttpClient, EF Core instrumentation) in `src/seat-viewer-api/Program.cs`
- [ ] T004 Register Scalar UI middleware stub in `src/seat-viewer-api/Program.cs`
- [ ] T005 Create initial EF Core `SeatViewerDbContext` in `src/seat-viewer-infrastructure/SeatViewerDbContext.cs`
- [ ] T006 Wire DbContext (SQL Server) into DI in `src/seat-viewer-api/Program.cs` reading connection string from `appsettings.Development.json`
- [ ] T007 Create `appsettings.Development.json` connection string placeholder for LocalDb in `src/seat-viewer-api/appsettings.Development.json`
- [ ] T008 Add Directory structure for tests: create `tests/seat-viewer-tests/unit`, `tests/seat-viewer-tests/integration`, `tests/seat-viewer-tests/contract` (ensure `.csproj` includes them)
- [ ] T009 Add test project references to domain & infrastructure projects in `tests/seat-viewer-tests/seat-viewer-tests.csproj`
- [ ] T010 Initialize global usings for tests (xUnit, Moq) in `tests/seat-viewer-tests/GlobalUsings.cs`
- [ ] T011 Create placeholder README for feature docs in `docs/features/seat-attributes/README.md`
- [ ] T012 Add OpenAPI generation hook (builder.Services.AddEndpointsApiExplorer + AddSwaggerGen) in `src/seat-viewer-api/Program.cs`

---
## Phase 2: Foundational
Purpose: Define domain entities, migrations, and base validation logic enabling independent user story development.

Independent Completion Criteria: Database schema created via migrations; domain entities compile with validation helpers; infrastructure ready for CRUD operations; no endpoints yet.

Checklist:
- [ ] T013 Create `Seat` entity class in `src/seat-viewer-domain/Seats/Seat.cs`
- [ ] T014 Create `SeatNote` entity class in `src/seat-viewer-domain/SeatNotes/SeatNote.cs`
- [ ] T015 [P] Add validation static class for seat number pattern in `src/seat-viewer-domain/Validation/SeatValidation.cs`
- [ ] T016 [P] Add note text validation helper in `src/seat-viewer-domain/Validation/NoteValidation.cs`
- [ ] T017 Configure entity model (composite key, relationships) in `src/seat-viewer-infrastructure/Configurations/SeatEntityTypeConfiguration.cs`
- [ ] T018 Configure note entity model in `src/seat-viewer-infrastructure/Configurations/SeatNoteEntityTypeConfiguration.cs`
- [ ] T019 Add infrastructure service registration extension in `src/seat-viewer-infrastructure/Extensions/InfrastructureServiceCollectionExtensions.cs`
- [ ] T020 Invoke infrastructure registration in `src/seat-viewer-api/Program.cs`
- [ ] T021 Add initial migration `InitSeatsAndNotes` using CLI (creates `Migrations` folder under `src/seat-viewer-infrastructure`)
- [ ] T022 Apply migration at startup (Ensure `EnsureCreated` replaced by `Migrate`) in `src/seat-viewer-api/Program.cs`
- [ ] T023 Seed demo seat data (few sample seats) in `src/seat-viewer-infrastructure/Seed/SeatDataSeeder.cs`
- [ ] T024 Call seeder after migration in `src/seat-viewer-api/Program.cs`
- [ ] T025 Create domain service interface `ISeatQueryService` in `src/seat-viewer-domain/Seats/ISeatQueryService.cs`
- [ ] T026 [P] Implement `SeatQueryService` with EF Core queries in `src/seat-viewer-infrastructure/Seats/SeatQueryService.cs`
- [ ] T027 Create domain service interface `ISeatNoteService` in `src/seat-viewer-domain/SeatNotes/ISeatNoteService.cs`
- [ ] T028 [P] Implement `SeatNoteService` (create/list/update/delete) in `src/seat-viewer-infrastructure/SeatNotes/SeatNoteService.cs`

---
## Phase 3: User Story 1 - Retrieve Seat Attributes (Priority P1)
Goal: Provide seat attributes retrieval endpoint.

Independent Test Criteria: Calling GET /seats/{model}/{seat} returns correct attributes; invalid seat number returns 400; unknown seat returns 404; trace spans recorded.

Checklist:
- [ ] T029 [US1] Add contract test skeleton for seat retrieval in `tests/seat-viewer-tests/contract/SeatAttributesContractTests.cs`
- [ ] T030 [P] [US1] Add unit tests for `SeatValidation` in `tests/seat-viewer-tests/unit/SeatValidationTests.cs`
- [ ] T031 [P] [US1] Add integration tests for `SeatQueryService` happy & not-found cases in `tests/seat-viewer-tests/integration/SeatQueryServiceTests.cs`
- [ ] T032 [US1] Implement Minimal API endpoint map for GET seat in `src/seat-viewer-api/Endpoints/SeatEndpoints.cs`
- [ ] T033 [US1] Add OpenAPI annotations or ensure schema alignment in `src/seat-viewer-api/Endpoints/SeatEndpoints.cs`
- [ ] T034 [US1] Add logging + tracing spans in endpoint method in `src/seat-viewer-api/Endpoints/SeatEndpoints.cs`
- [ ] T035 [US1] Update OpenAPI document generation settings (ensure seat schemas discovered) in `src/seat-viewer-api/Program.cs`
- [ ] T036 [US1] Implement contract tests (assert status codes & payload shape) in `tests/seat-viewer-tests/contract/SeatAttributesContractTests.cs`
- [ ] T037 [US1] Implement unit tests verifying seat derived data (WindowConfirmed) in `tests/seat-viewer-tests/unit/SeatDerivedDataTests.cs`
- [ ] T038 [US1] Add performance timing assertion (p95 simulation) test placeholder in `tests/seat-viewer-tests/integration/SeatQueryPerformanceTests.cs`

---
## Phase 4: User Story 2 - Add Public Note to Seat (Priority P2)
Goal: Allow creation and listing of notes.

Independent Test Criteria: POST creates note (201); GET /notes returns list including new note; validation rejects >500 chars; invalid seat returns 404.

Checklist:
- [ ] T039 [US2] Add contract test skeleton for create/list notes in `tests/seat-viewer-tests/contract/SeatNotesCreateListContractTests.cs`
- [ ] T040 [P] [US2] Add unit tests for `NoteValidation` length rule in `tests/seat-viewer-tests/unit/NoteValidationTests.cs`
- [ ] T041 [P] [US2] Add integration tests for `SeatNoteService` create & list in `tests/seat-viewer-tests/integration/SeatNoteServiceCreateListTests.cs`
- [ ] T042 [US2] Implement Minimal API endpoints for POST & GET notes in `src/seat-viewer-api/Endpoints/SeatNoteEndpoints.cs`
- [ ] T043 [US2] Add logging + tracing spans for note create/list in `src/seat-viewer-api/Endpoints/SeatNoteEndpoints.cs`
- [ ] T044 [US2] Ensure OpenAPI schemas for SeatNote operations exposed in `src/seat-viewer-api/Program.cs`
- [ ] T045 [US2] Implement contract tests for note create/list in `tests/seat-viewer-tests/contract/SeatNotesCreateListContractTests.cs`
- [ ] T046 [US2] Add integration test for validation failure (>500 chars) in `tests/seat-viewer-tests/integration/SeatNoteValidationFailureTests.cs`
- [ ] T047 [US2] Add integration test for seat not found scenario in `tests/seat-viewer-tests/integration/SeatNoteSeatNotFoundTests.cs`

---
## Phase 5: User Story 3 - Update or Remove Public Note (Priority P3)
Goal: Allow updating and deleting existing notes (last-write-wins).

Independent Test Criteria: PUT updates note text, subsequent GET returns updated content; DELETE removes note (204); updating non-existent note returns 404; deleting non-existent note returns 404; last-write-wins verified via sequential updates.

Checklist:
- [ ] T048 [US3] Add contract test skeleton for update/delete note in `tests/seat-viewer-tests/contract/SeatNotesUpdateDeleteContractTests.cs`
- [ ] T049 [P] [US3] Add integration tests for update note in `tests/seat-viewer-tests/integration/SeatNoteServiceUpdateTests.cs`
- [ ] T050 [P] [US3] Add integration tests for delete note in `tests/seat-viewer-tests/integration/SeatNoteServiceDeleteTests.cs`
- [ ] T051 [US3] Implement Minimal API endpoints for PUT & DELETE notes in `src/seat-viewer-api/Endpoints/SeatNoteEndpoints.cs`
- [ ] T052 [US3] Add tracing spans/logs for update/delete operations in `src/seat-viewer-api/Endpoints/SeatNoteEndpoints.cs`
- [ ] T053 [US3] Implement contract tests for update/delete note in `tests/seat-viewer-tests/contract/SeatNotesUpdateDeleteContractTests.cs`
- [ ] T054 [US3] Add integration test for last-write-wins scenario in `tests/seat-viewer-tests/integration/SeatNoteLastWriteWinsTests.cs`
- [ ] T055 [US3] Add integration test for delete non-existent note result in `tests/seat-viewer-tests/integration/SeatNoteDeleteNotFoundTests.cs`

---
## Phase 6: Polish & Cross-Cutting Concerns
Purpose: Finalize documentation, performance, observability refinement, and cleanup.

Independent Completion Criteria: Docs directory populated; performance placeholder tests adjusted; logging reviewed; Scalar UI shows all endpoints; OpenAPI validated; seed data improved; code comments added for public APIs.

Checklist:
- [ ] T056 Add README usage examples referencing quickstart in `docs/features/seat-attributes/README.md`
- [ ] T057 [P] Add XML comments for public domain service interfaces in `src/seat-viewer-domain/Seats/ISeatQueryService.cs`
- [ ] T058 [P] Add XML comments for note service interface in `src/seat-viewer-domain/SeatNotes/ISeatNoteService.cs`
- [ ] T059 Add OpenAPI validation integration test (ensure all paths reachable) in `tests/seat-viewer-tests/contract/OpenApiValidationTests.cs`
- [ ] T060 Add additional seed notes for demo in `src/seat-viewer-infrastructure/Seed/SeatDataSeeder.cs`
- [ ] T061 [P] Review and refine logging structure (ensure correlation id included) in `src/seat-viewer-api/Program.cs`
- [ ] T062 Add error handling middleware enhancements (uniform problem details) in `src/seat-viewer-api/Middleware/ErrorHandlingMiddleware.cs`
- [ ] T063 Integrate custom error middleware into pipeline in `src/seat-viewer-api/Program.cs`
- [ ] T064 Add performance assertion refinements (simulate p95 < threshold) in `tests/seat-viewer-tests/integration/SeatQueryPerformanceTests.cs`
- [ ] T065 Final pass on configuration (ensure production Serilog settings placeholder) in `src/seat-viewer-api/appsettings.json`
- [ ] T066 Add documentation for observability strategy in `docs/features/seat-attributes/observability.md`

---
## Dependency Graph (User Story Order)
US1 -> US2 -> US3 (sequential due to notes depending on seat retrieval domain foundation). Setup & Foundational must complete before US1. Polish after all stories.

Graph:
```
Setup -> Foundational -> US1 -> US2 -> US3 -> Polish
```

---
## Parallel Execution Opportunities
Examples (distinct files, no ordering conflict):
- Validation helpers T015, T016 can run parallel after entity creation.
- Services implementations T026, T028 parallel after interfaces defined.
- Within US1: Unit tests T030 and integration tests T031 parallel to contract skeleton T029.
- Within US2: Validation unit test T040 parallel with service integration tests T041.
- Within US3: Update and delete integration tests T049, T050 parallel.
- XML doc tasks T057, T058 can run parallel.

---
## Implementation Strategy
1. MVP: Complete through Phase 3 (US1) delivering seat attribute retrieval.
2. Increment 2: Add note creation/list (Phase 4).
3. Increment 3: Add note update/delete (Phase 5).
4. Polish: Performance, documentation, observability refinement (Phase 6).

Tests-first rule: For each story phase implement test skeletons then production code until green.

---
## Summary & Validation
Total Tasks: 66
Per Story: US1 (10 tasks: T029-T038), US2 (9 tasks: T039-T047), US3 (8 tasks: T048-T055)
Setup (12 tasks: T001-T012), Foundational (16 tasks: T013-T028), Polish (11 tasks: T056-T066)
Parallelizable Tasks Marked with [P]: T015, T016, T026, T028, T030, T031, T040, T041, T049, T050, T057, T058, T061
Independent Test Criteria Provided Per Story: Yes
MVP Scope: US1 (Phase 3)
Format Validation: All tasks follow '- [ ] Txxx [P?] [Story?] Description with file path' rule (story labels only in story phases).

---
