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
  - Visible connection to "Ma situation" is future work

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