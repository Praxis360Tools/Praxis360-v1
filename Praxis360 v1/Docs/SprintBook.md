# Praxis360 Sprint Book

| Property | Value |
|----------|-------|
| Version | V2.0 |
| Status | Active |
| Owner | Praxis360 |
| Last Updated | 2026-07-14 |

## Related Documents

- Blueprint.md
- ProductVision.md
- ProductBook.md
- Architecture.md
- Roadmap.md

---

# Table of Contents

1. Purpose
2. Sprint Methodology
3. Sprint Status
4. Sprint History
5. Current Sprint
6. Next Sprint
7. Completed Milestones
8. Technical Debt
9. Lessons Learned

---

# 1. Purpose

SprintBook is the official development journal of Praxis360.

It records every Sprint, the objectives achieved, major architectural decisions and the future development roadmap.

Each Sprint must leave the project in a better state than before.

---

# 2. Sprint Methodology

Praxis360 follows an incremental Sprint methodology.

Rules:

- One Sprint = One clear objective.
- Every Sprint must compile successfully.
- Documentation is updated before a Sprint is considered complete.
- Business vision always precedes technical implementation.
- No unfinished functionality is carried into the next Sprint.

---

# 3. Sprint Status

| Sprint | Status |
|---------|--------|
| Sprint 0 | ✅ Completed |
| Sprint 1 | ✅ Completed |
| Sprint 2 | ✅ Completed |
| Sprint 2.1 | ✅ Completed |
| Sprint 2.2 | ✅ Completed |
| Sprint 2.3 | ✅ Completed |
| Sprint 2.4 | ✅ Completed |
| Sprint 3.0 | ✅ Completed |
| Sprint 3.1 | 🚧 In Progress |

---

# 4. Sprint History

## Sprint 0 – Project Foundation

Objectives

- Create repository
- Configure development environment
- Define initial architecture

Status

Completed.

---

## Sprint 1 – Application Foundation

Objectives

- Dashboard
- Navigation
- Base layout

Status

Completed.

---

## Sprint 2 – Document Workspace

Objectives

- My Space
- Document management
- Shared components
- Services
- Scanner foundation

Status

Completed.

---

## Sprint 2.1 – Component Stabilization

Objectives

- Improve reusable components
- UI consistency

Status

Completed.

---

## Sprint 2.2 – Architecture Improvements

Objectives

- Improve project structure
- Refine services
- Improve maintainability

Status

Completed.

---

## Sprint 2.3 – Shared Components

Objectives

- Stabilize reusable components
- Improve design consistency

Status

Completed.

---

## Sprint 2.4 – AI Workspace

Objectives

- Complete project documentation
- Standardize AI workflow
- Introduce GitHub Copilot workflow
- Create Design Bible
- Create Motion Guide
- Create AI Playbook
- Organize project documentation

Status

Completed.

---

## Sprint 3.0 – Product Foundation

Objectives

- Redefine the product vision
- Introduce ProductVision.md
- Reposition Praxis360 around Life Insurance
- Replace Portfolio vision with Client Understanding
- Align documentation with the new business strategy

Major Decision

Praxis360 is no longer designed as a portfolio management application.

It becomes a premium client workspace focused on helping customers understand their Life Insurance situation through simple, reassuring and meaningful information.

Status

Completed.

---

## Sprint 3.1 – Domain Model

Objectives

Design the complete business domain before writing any implementation.

Focus areas:

- Insured-centric model
- Client Workspace
- Life Insurance contracts
- Customer-oriented business language
- Clean business architecture

Status

In Progress.

### Stories

- Story 3.1.1 — Domain Types Catalog — Done
- Story 3.1.2 — Domain Type Implementation — Done
- Story 3.1.3 — Value Objects — Completed

Summary: Implemented ContractNumber, Money, Percentage and DateRange value objects with DDD-compliant invariants and build validated.

---

# 5. Current Sprint

Sprint 3.1 – Domain Model

Goal

Design the complete business model that will serve as the foundation for every future feature.

No UI.

No Services.

No Imports.

No implementation.

Only the business domain.

---

# 6. Next Sprint

Sprint 3.2 – BRIO Integration

**Status**: In Progress

Story 3.2.3 — Import BRIO contracts (Completed)

Objective: enable the controlled import of BRIO contracts into Praxis360 after business validation and client identity reconciliation.

Phase 1 — Domain multi-source: Completed
- Domain multi-source foundation completed with external references and contract provenance
- Domain model supports external source tracking

Phase 2 — BRIO CSV reader and in-memory repositories: Completed
- Infrastructure layer: BrioCsvFileReader validates structure and produces BrioFileReadResult
- In-memory repositories ready for controlled application

Phase 3 — BRIO business analysis and candidate generation: Completed
- Commit: 0bf40ee
- Application layer: IBrioImportAnalyzer / BrioImportAnalyzer analyze business rules
- Validation output: BrioImportAnalysisResult with client/contract candidates and issues
- Business rules validated:
  - Client identity: INAMI → (Name+FirstName+BirthDate) → (Name+FirstName+Email) priority
  - Policy number validation across three expected occurrences
  - Product code mapping: FSPS/ESPSI → PLCI, EIP → EIP
  - Unknown codes retained with null mapping + Warning
  - Exact duplicate detection and grouping
- Functional validation: temporary external test harness executed (9/9 tests passed)
- Temporary harness not retained in repository
- No Domain entities created
- No repository writes performed
- No UI components added
- No financial data introduced

Phase 4 — Controlled client selection/creation and application to in-memory repositories: Completed
- Commit: db55fc8
- Application layer: IBrioContractApplicationService / BrioContractApplicationService
- Applies validated candidates to in-memory repositories
- Controlled client selection or creation
- Idempotent contract creation using external BRIO reference
- Result tracking: Created, AlreadyExisting, Skipped, Unresolved
- Outcome classification: Success, PartialSuccess, Failed
- Cumulative validation: 18 of 18 scenarios passed
- Capabilities delivered:
  - Controlled import of BRIO contracts to in-memory repositories
  - Client identity reconciliation with priority rules
  - Idempotent contract application
  - Unknown products/status retained as unresolved
  - No client created when no contract is creatable
- Limitations:
  - In-memory repositories only (no real persistence)
  - No BRIO user interface
  - No financial data added
  - Unknown values never guessed
  - MyPension out of scope
  - Scanner out of scope

Story 3.2.4 — BRIO Import Preview UI (Completed)
- Implementation commits: 97a4a32, 4d10c2e
- Delivers read-only preview interface for BRIO CSV files
- Route: /imports/brio accessible via "Importer BRIO" navigation entry
- UI Components:
  - Components/Pages/Imports/BrioImport.razor
  - Components/Pages/Imports/BrioImport.razor.css (isolated styling with ::deep for P360Card)
  - Components/Layout/NavMenu.razor (navigation entry)
- Reuses existing services:
  - IBrioFileReader for structural reading
  - IBrioImportAnalyzer for business analysis
- Displays analysis results:
  - Summary: lines analyzed, client candidates, contract candidates, warnings, blocking errors
  - Client grouping with contract details
  - Product type mapping display
  - Warnings and blocking errors separated
- Security and quality:
  - Generic error messages (no technical details exposed)
  - Explicit double-submission guard (_isAnalyzing)
  - File reference cleanup after processing (_selectedFile = null in finally)
  - Synchronous file selection handler (no async without await)
  - CSS isolation validated for P360Card component
- Validation:
  - Build successful
  - Manual validation with anonymized BRIO CSV files (valid file and file with errors)
  - No automated test infrastructure available
  - Code review approved
- Limitations:
  - Read-only preview only
  - No contract application (IBrioContractApplicationService not called)
  - No client creation or modification
  - No persistence
  - No financial data
  - No connection to "Ma situation"
  - Step C exists in engine but is not invoked by this page

Story 3.2.5 — BRIO Controlled Client Application (Completed)

- Implementation commit: dd57e5a
- Merge commit: 4b76ebe
- Pull Request: #4

Objective: Extend the read-only BRIO preview with a controlled application flow enabling explicit client candidate selection, destination choice (existing client vs new client), confirmation, and in-memory application to repositories.

Architecture:
- BrioImport.razor orchestrates UI workflow only
- IClientSelectionService / ClientSelectionService expose existing clients
- SelectableClient provides lightweight UI read model
- IBrioContractApplicationService applies validated contracts
- UiStep enum manages UI state machine
- Strict layering: Page → Application Service → Repository
- BrioImport.razor never injects IClientRepository directly

Components:
- Application/Interfaces/IClientSelectionService.cs (new)
- Application/Models/SelectableClient.cs (new)
- Application/Services/ClientSelectionService.cs (new)
- Program.cs (Singleton registration)
- Components/Pages/Imports/BrioImport.razor (extended)
- Components/Pages/Imports/BrioImport.razor.css (extended)

Functional Capabilities:
- Explicit BRIO client candidate selection
- Choice between existing Praxis360 client and new client creation
- Language selection for new clients (French, Dutch, English)
- Explicit confirmation before application
- In-memory application with idempotent contract creation
- Contextual result messages depend on ApplicationOutcome, ClientWasCreated, destination
- Newly created clients immediately visible in existing-client list
- Double-submission protection during application
- Reset blocked during Applying state
- No technical Guid exposed in UI

UiStep State Machine:
- Preview: initial analysis results
- SelectingClient: advisor selects BRIO client candidate
- ChoosingDestination: advisor chooses existing client or new client
- Confirming: confirmation screen before application
- Applying: in-memory application in progress
- Completed: result screen

Validation:
- Build successful
- git diff --check passed
- Exactly six files impacted
- Manual UI validation completed with test scenarios:
  • File with blocking errors: application correctly prevented
  • Valid file: processed successfully
  • New client creation: succeeded, two contracts created
  • Created client immediately available in existing-client selector
  • Idempotence verified: zero duplicates, contracts recognized as already existing
- No CSV or personal data files included
- Code review approved

Constraints respected:
- No Domain modification
- No repository or interface modification
- No lifetime changes for existing services
- No real persistence
- No financial data
- No connection to "Ma situation"
- BRIO preview preserved entirely
- No CSV files retained in repository

Story 3.2.6 — BRIO Synthetic Fixtures and Automated Coverage (Completed)

Objective:
Establish durable automated coverage for the BRIO import capabilities through a permanent xUnit project and five fully synthetic CSV fixtures.

Delivered:
- Permanent Praxis360 v1.Tests project
- Five fully synthetic CSV fixtures, the only CSV files currently allowlisted for version control; no production BRIO export is versioned
- Coverage for BrioCsvFileReader
- Coverage for BrioImportAnalyzer
- Coverage for BrioContractApplicationService
- Controlled fixture loading, construction, and validation utilities
- CSV files blocked by default with an explicit allowlist limited to the five approved fixtures
- File-size validation before full content reading
- Safe handling of null cells
- Case-insensitive InvalidColumnCount detection
- Validation of null, empty, and whitespace-only fixture names
- Five InlineData cases preserved

Final Validation:
- BrioCsvFileReaderTests: 7 [Fact] methods, 7 cases
- BrioImportAnalyzerTests: 12 [Fact] methods, 12 cases
- BrioContractApplicationServiceTests: 9 [Fact] methods, 9 cases
- BrioSyntheticDataGuardTests: 10 [Fact] methods and 1 [Theory] with 5 InlineData cases, for 15 cases
- Total: 38 [Fact] methods
- Total: 1 [Theory] method
- Total: 5 InlineData cases
- Total executed: 43
- Passed: 43
- Failed: 0
- Skipped: 0
- Build: 0 errors, 0 warnings

Confidentiality:
- Fixtures are fully synthetic
- No real data is used or versioned
- No unapproved fixture is permitted
- Git protection and automated fixture validation are active

Traceability:
- Functional PR #6
- Corrective PR #7
- Functional commit 86623a9
- Corrective commits 3aa4c8d, ab61f9a, and 2c32287
- Functional merge commit 691f5ea
- Corrective merge commit 6a42c4c

Constraints:
- No real persistence
- No financial data
- No Domain modification
- No new user-facing functionality
- Production behavior remained unchanged by PR #6 and PR #7

Story 3.2.8 — Situation reload from SQLite with multi-client selection (Ready for Architecture Review)

Objective:
Replace the demo-backed "Ma situation" with repository-backed loading from persisted SQLite data, implementing explicit multi-client selection and post-import navigation, with full end-to-end integration test coverage.

Implementation branch: story/3.2.8-situation-reload-from-sqlite

Architecture:
- SituationAssuranceVieService converted from demo synchronous logic to repository-backed async service
- New SituationAssuranceVieLoadResult wrapper distinguishes default-load outcomes
- Portfolio.razor rewritten as route-aware async UI state machine serving both `/patrimoine` and `/patrimoine/{ClientId:guid}`
- BrioImport.razor extended with conditional "Voir Ma situation" post-import navigation link
- SituationAssuranceVieService registered as scoped lifetime (aligned with scoped repositories)
- DemoSituationAssuranceVieDataService removed from runtime DI registration

Components created:
- Praxis360 v1/Models/SituationAssuranceVieLoadResult.cs
- Praxis360 v1.Tests/Application/Services/SituationAssuranceVieServiceSqliteIntegrationTests.cs

Components modified:
- Praxis360 v1/Services/SituationAssuranceVieService.cs
- Praxis360 v1/Components/Pages/Portfolio/Portfolio.razor
- Praxis360 v1/Components/Pages/Portfolio/Portfolio.razor.css
- Praxis360 v1/Components/Pages/Imports/BrioImport.razor
- Praxis360 v1/Components/Pages/Imports/BrioImport.razor.css
- Praxis360 v1/Program.cs

Functional capabilities:
- Repository-backed async loading of "Ma situation" from persisted SQLite data
- Explicit multi-client selection when multiple clients exist (no arbitrary selection)
- Route-based client identification via `/patrimoine/{ClientId:guid}`
- Six distinct UI states: Loading, ClientLoaded, NoClientsAvailable, MultipleClientsRequireSelection, ClientNotFound, ErrorLoading
- Post-import "Voir Ma situation" link when BrioContractApplicationResult has ClientId and applied contracts
- Financial indicators remain null (no financial data)
- Insurer fallback: Insurer.DisplayName → most recent BRIO provenance RawInsurerName → "Compagnie non disponible"

Load-result wrapper (SituationAssuranceVieLoadResult):
- Status enum: ClientLoaded, NoClientsAvailable, MultipleClientsRequireSelection, ClientNotFound
- Typed outcome for default-load flow without conflating loading, absence of clients, or multi-client selection

Integration test coverage:
- Full end-to-end test: BRIO import → persistence → service recreation → situation reload → exact assertion on read model
- Scenarios: no clients, one client, multiple clients, nonexistent ClientId, client without contracts
- CurrentContracts calculation verified (Active | PaidUp | Suspended)
- Insurer fallback verified with fixture having no insurer data
- No duplicates, deterministic candidate identification
- Fixture: BrioSynthetic.ValidCore.csv (ALPHA: 2 contracts INAMI-identified, BETA: 1 contract name+DOB-identified, GAMMA: 1 contract email-identified)

Build validation:
- Main project: build successful
- Test project: build successful
- Test history:
  - Story 3.2.7A baseline: 51/51 tests passing
  - Story 3.2.7B baseline: 97/97 tests passing
  - Story 3.2.8 current: 110/110 tests passing (12 integration tests + 1 defensive service test)

Manual validation (pre-corrections):
- Empty state verified on /patrimoine
- BrioSynthetic.ValidCore.csv imported successfully (4 lines analyzed, 3 client candidates, 4 contract candidates)
- DR. ALPHA SYNTHETIC ALPHA created as new client with language manually selected as Français via UI selector
- SYN-ALPHA-001 (PLCI) and SYN-ALPHA-002 (EIP) created and persisted
- Navigation to /patrimoine/{ClientId:guid} successful, both contracts visible
- Application shutdown and restart: client and both contracts reloaded from SQLite successfully

Final corrections applied after manual validation:
- Removed language selection UI control from BrioImport.razor
- Language.French now hardcoded in BrioImport.razor ApplyContracts() method call
- Confirmation message updated to remove obsolete "repositories en mémoire" reference
- EfCoreContractRepository.GetByClientIdAsync() updated with AsSplitQuery() to address EF Core warning 20504
- Whitespace-only insurer fallback logic hardened with string.IsNullOrWhiteSpace() check in SituationAssuranceVieService

Visual validation (post-corrections):
- Synthetic ALPHA client imported (2 contracts), application shutdown, restart: client and contracts successfully reloaded from SQLite
- Synthetic BETA client created, direct access via /patrimoine/{ClientId:guid}: correct contract displayed
- Generic route /patrimoine: multi-client selection cards for ALPHA and BETA displayed correctly
- Language selector confirmed absent from UI (Language.French imposed automatically in code)
- Destination label displays "Nouveau client"
- Confirmation message validated: "Cette action va enregistrer le client et ses contrats dans Praxis360. Confirmez-vous l'application ?"
- GAMMA client not applied during this verification
- EF Core AsSplitQuery observed in runtime: three separate queries (Contracts, ExternalReferences, ContractProvenances)
- No Microsoft.EntityFrameworkCore.Query[20504] warning during client loading

Quality checks:
- No DemoSituationAssuranceVieDataService reference in runtime DI
- No repository or IDbContextFactory injection in Blazor pages
- No GUID displayed in UI (DisplayName and DateOfBirth only)
- No arbitrary client selection
- No financial data replaced by zero (null preserved)
- No Domain modification
- No repository modification
- No migration
- No new package
- git diff --check passed
- Scoped lifetime for SituationAssuranceVieService

CSS changes:
- BrioImport.razor.css: .result-action and .result-action .btn-secondary styling for post-import link
- Portfolio.razor.css: .empty-state, .client-selection-list, .selectable-client-card, .client-name, .client-dob, .chevron-icon

Constraints respected:
- No user database manipulation
- No commit or push during implementation
- Demo service file preserved but not registered at runtime
- Preserves all existing modifications
- Multi-client selection reuses IClientSelectionService infrastructure
- BrioImport navigation uses link, not NavigationManager injection

Limitations:
- Manual validation of user database not performed (protocol available for later execution)
- Financial indicators remain null pending separate calculation feature
- Insurer fallback depends on available provenance data (fixture has no insurer names)

---

# 7. Completed Milestones

- Project foundation
- Dashboard
- Document Workspace
- Scanner foundation
- Shared Components
- Documentation framework
- AI Workspace
- Product Vision
- Development Blueprint
- Architecture redesign

---

# 8. Technical Debt

Current priorities

- Complete Domain Model
- Implement BRIO mapping
- Expand Design Bible with component catalogue
- Create customer journey diagrams
- Add architecture diagrams

No critical technical debt identified.

---

# 9. Lessons Learned

Major lessons from Sprint 3:

Business vision must always precede technical implementation.

The customer experience is more important than exposing technical data.

The Domain Model should represent the customer's reality, not the structure of external systems.

Good documentation significantly improves AI-assisted development.

Praxis360 is not built around contracts.

It is built around customer understanding.