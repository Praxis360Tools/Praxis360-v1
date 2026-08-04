# Praxis360 Architecture

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
- DesignBible.md
- SprintBook.md

---

# Table of Contents

1. Architecture Philosophy
2. Architectural Principles
3. Project Structure
4. Application Layers
5. Domain Model
6. Data Flow
7. Dependency Injection
8. Naming Conventions
9. Shared Components
10. Services
11. Models
12. Pages
13. Scalability Principles
14. Development Rules
15. Definition of Done
16. References

---

# 1. Architecture Philosophy

Praxis360 follows a clean, modular and scalable architecture.

The architecture is business-driven.

The software must model the customer's reality, not the structure of external systems.

BRIO is a data source.

Praxis360 owns its business model.

---

# 2. Architectural Principles

The architecture follows these principles:

- Business first.
- Domain-driven thinking.
- Modular components.
- Loose coupling.
- High cohesion.
- Reusability.
- Maintainability.
- Scalability.

Every technical decision should simplify future evolution.

---

# 3. Project Structure

Praxis360-v1

- Components/
- Models/
- Pages/
- Services/
- Shared/
- Resources/
- wwwroot/

Documentation

- Docs/
- .github/

Each folder has a single responsibility.

---

# 4. Application Layers

## Pages

Display information.

Coordinate interactions.

Call Services.

Never contain business logic.

---

## Shared Components

Reusable UI components.

No business logic.

---

## Services

Business logic.

Calculations.

Transformations.

Validation.

Imports.

Communication with storage.

---

## Models

Represent the business domain.

Models describe Praxis360 concepts.

Not BRIO concepts.

---

## Testing Infrastructure

Praxis360 v1.Tests is the permanent xUnit project for automated test coverage.

Project Organization:
- Application/: tests for application services
- Infrastructure/: tests for infrastructure components
- TestSupport/: test utilities and fixture management

BRIO Coverage:
- BrioCsvFileReader: structural CSV validation
- BrioImportAnalyzer: business analysis and candidate generation
- BrioContractApplicationService: controlled contract application

Fixture Management:
- Fixtures/Brio/Synthetic/: five approved synthetic CSV files
- Fixtures are exclusively synthetic
- Loading is limited to an explicit allowlist
- Automated integrity and confidentiality guard
- Controlled synthetic line construction
- Strict 50,000-byte limit enforced before full read
- All CSV files are blocked from version control by default
- Only five approved fixtures authorized
- No real client data is required or permitted in the test fixtures

---

# 5. Domain Model

The Domain Model is the foundation of Praxis360.

Every feature starts with the business model.

The application is centred around the insured person.

Contracts belong to an insured.

The client workspace belongs to the insured.

External systems adapt to the domain model.

The domain model never adapts to external systems.

---

# 6. Data Flow

User

↓

Page

↓

Shared Components

↓

Service

↓

Domain Model

↓

Storage / API

---

# 7. Dependency Injection

Every Service is registered in Program.cs.

Constructor Injection only.

Never instantiate Services manually.

---

# 8. Naming Conventions

Names must represent business concepts.

Use explicit names.

Avoid technical abbreviations.

Prefer customer-oriented terminology whenever possible.

---

# 9. Shared Components

Shared components:

- reusable
- UI only
- lightweight
- composable

---

# 10. Services

Services:

- encapsulate business logic
- remain testable
- remain independent from UI
- expose clear responsibilities

---

# 11. Models

Models represent the business domain.

Models remain lightweight.

Business calculations belong inside Services.

---

# 12. Pages

Pages orchestrate.

Pages never calculate.

Pages never implement business rules.

---

# 13. Scalability Principles

The architecture supports future modules:

Version 1

Life Insurance

↓

Version 2

Property & Casualty

↓

Version 3

Energy

↓

Version 4

Telecommunications

↓

Version 5

Real Estate

without architectural redesign.

---

# 14. Development Rules

Always:

- follow ProductVision
- design the business model first
- reuse components
- reuse services
- keep code readable

Never:

- duplicate business logic
- bypass architecture
- couple UI with business rules

---

# 15. Definition of Done

Architecture is complete when:

- business model respected
- architecture respected
- project compiles
- reusable components used
- documentation updated

---

# 16. References

- Blueprint.md
- ProductVision.md
- ProductBook.md
- SprintBook.md

---

## Domain Types

The Domain Type Catalog V1 defines the official set of domain enumerations used across the Praxis360 Domain Model. These types are part of the Domain Model and represent fixed business concepts. They do not contain behavior, do not depend on external systems and remain independent from BRIO which is only a data source.

Domain Type Catalog V1:

- ContractStatus
- ContractType
- ContributionFrequency
- BeneficiaryType
- DocumentCategory
- DocumentStatus
- Currency
- Language
- Gender

---

## Value Objects

Value Objects represent small, immutable concepts of the Domain Model that encapsulate business invariants and ensure value-based equality. In Praxis360 they belong to the Domain layer, are independent from BRIO and from any infrastructure concerns, and are placed under Domain/ValueObjects.

Conventions for Value Objects in Praxis360:

- Value Objects are immutable and implement equality by value.
- Validation of invariants is performed at creation time and preserved afterwards.
- Value Objects do not depend on external systems or persistence concerns.

Value Objects validated in Sprint 3.1.3:

- ContractNumber — identifies a contract in the domain; value must be provided, trimmed (start/end) and equality is based on the preserved value.
- Money — encapsulates Amount (decimal) and Currency (Domain Type); operations (addition, subtraction) are allowed only between amounts that share the same Currency; no automatic conversion is performed.
- Percentage — stored internally as a decimal fraction (for example 0.05 represents 5%); creation is explicit via factory methods (FromFraction, FromPercent); values are limited to 0%..100% in V1.
- DateRange — uses DateOnly for Start and optional End; End cannot be earlier than Start; equality is based on both dates.

---

## BRIO Import Pipeline

Story 3.2.3 introduces a controlled multi-step import pipeline for BRIO CSV data. This pipeline validates business rules before any application to repositories and maintains strict separation between external data sources and the Domain Model.

### Pipeline Architecture

The BRIO import pipeline operates in three distinct steps:

**Step A — Structural Reading (Infrastructure)**
- BrioCsvFileReader validates CSV structure and produces BrioFileReadResult
- Infrastructure/FileReaders/BrioCsvFileReader.cs
- Validates CSV structure and expected column count, and handles UTF-8 BOM input
- Reports structural errors

**Step B — Business Analysis (Application)**
- IBrioImportAnalyzer / BrioImportAnalyzer validate business rules and produce BrioImportAnalysisResult
- Application/Services/BrioImportAnalyzer.cs
- Validates client identity, policy number references, product codes
- Detects duplicates and data quality issues
- Creates client and contract candidates
- Distinguishes Warnings (non-blocking) from BlockingErrors (prevent import)

**Step C — Controlled Application (Completed - Phase 4)**
- IBrioContractApplicationService / BrioContractApplicationService apply validated candidates to in-memory repositories
- Application/Services/BrioContractApplicationService.cs
- Controlled client selection or creation based on identity matching
- Idempotent contract creation using external BRIO reference
- Result tracking: Created, AlreadyExisting, Skipped, Unresolved
- Outcome classification: Success, PartialSuccess, Failed

### Phase 3 Scope — Business Analysis

Phase 3 (commit 0bf40ee) completes Step B with the following components:

**Application Models**
- BrioImportAnalysisResult — consolidated analysis containing analyzed lines, client candidates, contract candidates and all issues
- BrioAnalyzedLine — one CSV line analysis with normalized values and attached issues
- BrioClientCandidate — potential client with normalized identity and demographic data
- BrioContractCandidate — potential contract with normalized policy number and source line references
- ImportAnalysisIssue — validation issue with code, severity and context
- ImportIssueSeverity — enum (Warning | BlockingError)

**Business Rules**
- Client identity: INAMI → (Name+FirstName+BirthDate) → (Name+FirstName+Email) priority
- Policy number validation across three expected occurrences (columns 7, 30, 43)
- Scientific notation detection → BlockingError
- Conflicting references → BlockingError
- Single occurrence → Warning (contract still created)
- Product code mapping: FSPS/ESPSI → PLCI, EIP → EIP
- Unknown codes → null mapping + Warning
- Exact duplicate detection (all 62 cells identical) → Warning
- Lines with same client + policy grouped via SourceLineNumbers

**Result Properties**
- HasBlockingErrors prevents Step C application
- CanProceed requires no blocking errors AND at least one contract candidate

**Validation**
- Functional validation performed with a temporary external test harness
- 9 of 9 tests passed
- Temporary harness not retained in repository

**Constraints Phase 3**
- No Domain entities created
- No repository writes performed
- No financial data introduced
- No UI components added

**Constants**
- BrioColumnPositions — CSV column index definitions
- BrioProductCodeMapping — static product code to ContractType mapping

### Phase 4 Scope — Controlled Application

Phase 4 (commit db55fc8) completes Step C with the following components:

**Application Service**
- IBrioContractApplicationService / BrioContractApplicationService — applies validated candidates to in-memory repositories
- ApplyToExistingClientAsync method — applies contracts to an existing client
- ApplyWithNewClientAsync method — creates a new client and applies contracts

**Application Models**
- BrioContractApplicationResult — consolidated result with the following properties:
  - ClientId — the affected client identifier
  - ClientWasCreated — indicates if a new client was created
  - ContractsCreated — list of ContractCreated results
  - ContractsAlreadyExisting — list of ContractAlreadyExisting results
  - ContractsSkipped — list of ContractSkipped results
  - ContractsUnresolved — list of ContractUnresolved results
  - GlobalErrors — IReadOnlyList<ImportAnalysisIssue> containing global errors
  - GlobalWarnings — IReadOnlyList<ImportAnalysisIssue> containing global warnings
  - Outcome — ApplicationOutcome enum value
- ContractCreated — individual contract creation result
- ContractAlreadyExisting — individual already existing contract result
- ContractSkipped — individual skipped contract result
- ContractUnresolved — individual unresolved contract result
- ApplicationOutcome — enum (Success | PartialSuccess | Failed)

**Business Rules**
- Client selection: the caller explicitly selects a BrioClientCandidate using its normalized identity and either targets an existing Praxis360 client or requests controlled creation of a new client
- Client creation: controlled creation only when at least one contract is creatable
- Contract idempotence: external BRIO reference prevents duplicates
- Unresolved products: contracts with unknown product codes are not created
- Unresolved status: contracts with unknown status codes are not created
- Outcome Success: all contracts created or already existing
- Outcome PartialSuccess: mix of Created/AlreadyExisting with Unresolved
- Outcome Failed: no contracts applied

**Validation**
- Cumulative functional validation: 18 of 18 scenarios passed
- Idempotence validated
- Controlled client creation validated
- Application to existing client validated
- Unknown products retained as unresolved
- Unknown status retained as unresolved
- Blocked lines correctly processed
- Missing sources correctly processed
- No client created when no contract is creatable
- AlreadyExisting + Unresolved treated as PartialSuccess
- No normalized identity exposed in BRIO_CLIENT_CANDIDATE_NOT_FOUND

**Constraints Phase 4**
- In-memory repositories only
- No real persistence
- No BRIO user interface
- No financial data added without business validation
- Unknown values never guessed
- MyPension remains out of scope
- Scanner remains out of scope
- Visible connection to "Ma situation" remains future work

### Story 3.2.4 — BRIO Import Preview UI

Story 3.2.4 (implementation commits: 97a4a32, 4d10c2e) delivers a read-only UI interface for controlled BRIO file preview. This interface provides visibility into Steps A and B without triggering Step C application.

**UI Components**
- Components/Pages/Imports/BrioImport.razor — preview page at route /imports/brio
- Components/Pages/Imports/BrioImport.razor.css — isolated styling
- Components/Layout/NavMenu.razor — navigation entry "Importer BRIO"

**Architecture**
- Page orchestrates IBrioFileReader then IBrioImportAnalyzer
- No business rules duplicated in the page layer
- No calls to IBrioContractApplicationService
- No repository writes performed
- Analysis and preview results held in memory only
- File size limit: 10 MB
- IBrowserFile reference released after processing (finally block sets _selectedFile = null)
- Technical error details never exposed to user (generic error messages only)
- Double-submission prevention via explicit _isAnalyzing guard
- HandleFileSelected is synchronous (no async without await)

**UI Behavior**
- File selection with .csv validation
- Explicit "Analyser le fichier" action required
- Analysis summary display: lines analyzed, client candidates, contract candidates, warnings, blocking errors
- Results grouped by client candidate without exposing normalized identities
- Warnings and blocking errors separated and displayed
- Contract candidates shown with product type mapping
- Unknown products displayed as unmapped with warning
- Reset capability to analyze another file
- Reuses P360Card component with CSS isolation via ::deep selectors

**Data Flow**
- User selects CSV file → HandleFileSelected validates extension/size
- User clicks Analyze → AnalyzeFile opens stream → IBrioFileReader.ReadAsync
- Read result → IBrioImportAnalyzer.AnalyzeAsync
- Analysis result → UI display with grouping and formatting
- No persistence, no Step C application, no Domain entity creation

**Validation**
- Build successful
- Manual validation with anonymized BRIO CSV files (valid and error cases)
- No automated test infrastructure available
- Code review approved (security, resource cleanup, CSS isolation)

**Constraints Story 3.2.4**
- Read-only preview interface only
- No contract application
- No client creation or modification
- No persistence
- No financial data
- No connection to "Ma situation"
- Step C (IBrioContractApplicationService) exists in the engine but is not called by this page

### Story 3.2.5 — BRIO Controlled Client Application

Story 3.2.5 (implementation commit: dd57e5a, merge commit: 4b76ebe, PR #4) extends the BRIO preview UI with a controlled application workflow. It introduces explicit client selection, destination choice, confirmation, and in-memory repository application while preserving strict architectural layering.

**New Application Components**

- Application/Interfaces/IClientSelectionService.cs — service contract for listing existing clients
- Application/Models/SelectableClient.cs — lightweight UI read model for existing clients (ClientId, FirstName, LastName, DateOfBirth, DisplayName)
- Application/Services/ClientSelectionService.cs — service implementation that calls IClientRepository.GetAllAsync() and maps entities to SelectableClient

**Modified UI Components**

- Components/Pages/Imports/BrioImport.razor — extended with controlled application workflow
- Components/Pages/Imports/BrioImport.razor.css — extended with selection, confirmation, applying and result styles

**Modified Composition Root**

- Program.cs — registered IClientSelectionService / ClientSelectionService as Singleton

**Architecture Flow**

Page Layer:
  BrioImport.razor injects IClientSelectionService and IBrioContractApplicationService
  → Never injects IClientRepository directly

Application Layer:
  ClientSelectionService injects IClientRepository
  → Calls IClientRepository.GetAllAsync()
  → Maps Client entities to SelectableClient read models

  BrioContractApplicationService applies validated candidates to repositories

Repository Layer:
  IClientRepository and IContractRepository remain in-memory
  → InMemoryClientRepository and InMemoryContractRepository unchanged

**UI State Machine**

Local UiStep enum manages workflow:

  Preview
    → User analyzes BRIO CSV file
    → If no blocking errors: "Démarrer l'application" available

  SelectingClient
    → User selects one BRIO client candidate
    → "Confirmer la sélection" enabled after selection

  ChoosingDestination
    → User chooses:
      • Nouveau client (language selection: French, Dutch, English)
      • Client Praxis360 existant (list from IClientSelectionService)

  Confirming
    → Displays selected BRIO client name, contract count, destination
    → "Appliquer les contrats" triggers application

  Applying
    → Application in progress
    → Reset blocked

  Completed
    → Result screen with contextual message
    → "Terminer" returns to Preview

**Functional Workflow**

1. Advisor selects and analyzes BRIO CSV file (Story 3.2.4 preview)
2. If no blocking errors: "Démarrer l'application" button becomes available
3. Advisor selects one BRIO client candidate
4. Advisor chooses destination:
   - Nouveau client (French/Dutch/English language selection)
   - Client Praxis360 existant (list populated via IClientSelectionService)
5. Confirmation screen displays:
   - Selected BRIO client name
   - Number of associated contracts
   - Chosen destination
6. Advisor confirms application
7. System applies contracts in memory via IBrioContractApplicationService
8. Result screen displays:
   - Contextual message (depends on ApplicationOutcome, ClientWasCreated, destination)
   - Contracts created
   - Contracts already existing
   - Contracts skipped
   - Contracts unresolved
   - Global warnings and errors
   - No technical Guid displayed

**Data Flow**

Existing Client Selection:
  BrioImport.razor → IClientSelectionService.GetSelectableClientsAsync()
  → ClientSelectionService → IClientRepository.GetAllAsync()
  → List<Client> mapped to List<SelectableClient>
  → UI displays FirstName LastName (DateOfBirth)

New Client Application:
  BrioImport.razor → IBrioContractApplicationService.ApplyWithNewClientAsync(analysisResult, selectedClientIdentity, language)
  → BrioContractApplicationService creates new Client entity
  → Applies contracts idempotently using external BRIO reference
  → Returns BrioContractApplicationResult

Existing Client Application:
  BrioImport.razor → IBrioContractApplicationService.ApplyToExistingClientAsync(analysisResult, selectedClientIdentity, existingClientId)
  → BrioContractApplicationService retrieves existing Client
  → Applies contracts idempotently
  → Returns BrioContractApplicationResult

**Exception Handling**

ApplyContracts() method uses try-catch:
  On exception:
    → Restores _currentStep = UiStep.Confirming
    → Displays generic error message
    → No technical details exposed

**Result Message Logic**

The result message displayed depends on ApplicationOutcome, ClientWasCreated, and the selected destination:

- ClientWasCreated == true:
  "Nouveau client créé"

- Destination == ExistingClient with contracts applied or already existing:
  "Contrats rattachés au client sélectionné"

- Destination == NewClient, ClientWasCreated == false, Outcome == Failed:
  "Aucun nouveau client n'a été créé"

- Other Outcome == Failed:
  "L'application a échoué"

- Other results:
  "Application terminée"

This logic ensures the user receives accurate feedback based on the actual application result. The generic error message from the ApplyContracts() catch block handles unexpected exceptions and restores the Confirming state.

**Idempotence**

Contracts applied multiple times to the same client:
  → External BRIO reference prevents duplicates
  → Already-existing contracts recognized
  → No duplicate entities created
  → Result reflects ContractsAlreadyExisting count

**Immediate Visibility**

Newly created clients:
  → Immediately available in IClientSelectionService
  → No application restart required
  → Validated manually: same file analyzed twice, new client appeared in existing-client list

**Validation**

- Build successful
- Manual UI validation with anonymized test files:
  • File with blocking errors: application blocked
  • Valid anonymized file: application succeeded
  • New client created: two contracts created
  • Client immediately visible in existing-client selector
  • Idempotence: zero duplicates, contracts recognized as already existing
- Code review approved

**Constraints Story 3.2.5**

- In-memory repositories only (no real persistence)
- No Domain entity modifications
- No repository interface modifications
- No existing service lifetime changes
- No financial data added
- No connection to "Ma situation"
- BRIO preview (Story 3.2.4) preserved entirely
- No CSV files included in repository

### Story 3.2.7A — EF Core Infrastructure and Database Schema

Story 3.2.7A implements the foundational EF Core infrastructure for SQLite persistence without modifying runtime service registration.

**Persistence Entities**

- Infrastructure/Persistence/Entities/ClientPersistence.cs
- Infrastructure/Persistence/Entities/ContractPersistence.cs
- Infrastructure/Persistence/Entities/SituationAssuranceViePersistence.cs

Persistence entities represent the database schema and include EF Core configuration (table names, primary keys, relationships, indexes). These entities use simple types (Guid, string, DateTime, decimal) and contain no Domain logic.

**Mappers**

- Infrastructure/Persistence/Mappers/ClientMapper.cs
- Infrastructure/Persistence/Mappers/ContractMapper.cs
- Infrastructure/Persistence/Mappers/SituationAssuranceVieMapper.cs

Mappers provide bidirectional conversion between Domain entities and persistence entities. Domain entities remain unchanged and never reference persistence types.

**DbContext**

- Infrastructure/Persistence/AppDbContext.cs

AppDbContext configures SQLite provider, defines DbSet properties for each persistence entity, and applies entity configurations via fluent API in OnModelCreating.

**Migrations**

- Infrastructure/Persistence/Migrations/20260126120000_InitialCreate.cs (.cs and .Designer.cs files)

InitialCreate migration creates Clients, Contracts, and SituationsAssuranceVie tables with appropriate columns, primary keys, foreign keys, and indexes.

**Validation**

- 19 new automated tests in Praxis360 v1.Tests/Infrastructure/Persistence/
  - ClientMapperTests.cs (7 tests)
  - ContractMapperTests.cs (7 tests)
  - SituationAssuranceVieMapperTests.cs (5 tests)

All mapper tests verify correctness of Domain ↔ Persistence conversion including Guid preservation, nullable fields, and enumeration mappings.

**Constraints Story 3.2.7A**

- No runtime service registration changes (in-memory repositories remain active)
- No Domain entity modifications
- No repository interface changes
- No UI changes
- AppDbContext registered but not used at runtime

### Story 3.2.7B — EF Core Runtime Repositories and Database Initialization

Story 3.2.7B activates EF Core repositories as the runtime persistence implementation, replacing in-memory repositories in the composition root.

**EF Core Repositories**

- Infrastructure/Persistence/Repositories/EfCoreClientRepository.cs
- Infrastructure/Persistence/Repositories/EfCoreContractRepository.cs

EF Core repositories implement domain repository interfaces using AppDbContext. Operations use mappers for Domain ↔ Persistence conversion. All async operations include atomic SaveChangesAsync calls to ensure consistency.

**Persistence Service**

- Application/Interfaces/IBrioPersistenceService.cs
- Application/Services/BrioPersistenceService.cs renamed to InMemoryBrioPersistenceService.cs (preserved but unused)
- Infrastructure/Persistence/Services/EfCoreBrioPersistenceService.cs

EfCoreBrioPersistenceService implements IBrioPersistenceService with atomic transaction behavior. SaveAllAsync wraps all SaveChangesAsync calls in a single transaction to ensure atomicity across Client and Contract repositories.

**Database Initialization**

- Infrastructure/Persistence/Services/DatabaseInitializer.cs
- Infrastructure/Persistence/Services/LocalAppDataDatabasePathResolver.cs

DatabaseInitializer ensures database creation and migration application at application startup. LocalAppDataDatabasePathResolver provides platform-agnostic database file path using Environment.SpecialFolder.LocalApplicationData (%LOCALAPPDATA% on Windows). Database file location: `%LOCALAPPDATA%\Praxis360\praxis360.db`.

**Composition Root**

Program.cs modified to:
- Register AppDbContext with SQLite provider
- Register EF Core repositories (EfCoreClientRepository, EfCoreContractRepository)
- Register EfCoreBrioPersistenceService as IBrioPersistenceService
- Register DatabaseInitializer as Singleton
- Call DatabaseInitializer.InitializeAsync() before app.Run()

In-memory implementations (InMemoryClientRepository, InMemoryContractRepository, InMemoryBrioPersistenceService) remain in the codebase but are not registered in DI container.

**Validation**

- Build successful
- Test results after Story 3.2.7B merge: 97/97 tests passing (baseline for Story 3.2.8)
- New tests in Praxis360 v1.Tests/Infrastructure/Persistence/Repositories/:
  - EfCoreClientRepositoryTests.cs (6 tests)
  - EfCoreContractRepositoryTests.cs (7 tests)

All repository tests use in-memory SQLite databases (Data Source=:memory:) and verify CRUD operations, atomicity, and mapper integration.

**Constraints Story 3.2.7B**

- No Domain entity modifications
- No repository interface changes
- No UI changes
- In-memory implementations preserved in codebase but unused at runtime
- Database file stored in LocalApplicationData (no connection strings in appsettings.json)

### Story 3.2.8 — SQLite-Backed Insurance Situation Display

Story 3.2.8 (Ready for Architecture Review) connects the SQLite persistence layer to the Portfolio UI, replacing the demo data service with real persistence-backed data flow.

**Data Flow**

SQLite database
→ EfCoreClientRepository + EfCoreContractRepository (via AppDbContext)
→ SituationAssuranceVieService
→ SituationAssuranceVieReadModel
→ Portfolio.razor

Portfolio.razor loads insurance situations via SituationAssuranceVieService.GetSituationForClientAsync(ClientId). Service injects IClientRepository and IContractRepository, queries Client and Contract entities, converts them to read models, and returns them to the UI. No separate SituationAssuranceVie repository exists.

**Route and Client Selection**

Portfolio.razor route: `/patrimoine/{ClientId:guid}`

UI behavior:
- Zero clients in database → empty state displayed
- One client in database → Portfolio automatically loads that client's data
- Multiple clients in database → Portfolio displays client selector dropdown populated via IClientSelectionService.GetSelectableClientsAsync()

Client selection stored in browser localStorage for persistence across page refreshes.

**Insurance Company Fallback**

SituationAssuranceVieService.DetermineInsurerDisplayName() implements fallback logic:
- First preference: ContratVie.Insurer.DisplayName if Insurer aggregate exists
- Fallback: most recent BRIO provenance from ContratVie.Provenances collection (ordered by ImportedAtUtc descending), using Provenance.RawInsurerName
- Final fallback: "Compagnie non disponible" if no insurer information available

Note: ContractPersistenceMapper.ToDomain() currently reconstructs Insurer as null, so fallback to RawInsurerName from BRIO provenance is the active path.

**Service Updates**

SituationAssuranceVieService.cs modified to:
- Implement insurance company fallback logic (DetermineInsurerDisplayName method)
- Query IClientRepository and IContractRepository instead of generating demo data
- Add GetSituationForDefaultClientAsync() for zero/one/multiple client scenarios
- Return null when no client exists for ClientId

**Demo Service Status**

DemoSituationAssuranceVieDataService remains in codebase but is not registered in Program.cs. This service is preserved for potential future demo scenarios but has no runtime impact.

**UI Updates**

Portfolio.razor and Portfolio.razor.css modified to:
- Remove demo data warning banner
- Integrate real client selection via IClientSelectionService
- Display insurance situations from SQLite via SituationAssuranceVieService
- Handle empty states appropriately

BrioImport.razor and BrioImport.razor.css modified to add navigation link to Portfolio after successful contract application.

**Validation**

- Build successful (main project and test project)
- Test history:
  - Story 3.2.7A baseline: 51/51 tests passing
  - Story 3.2.7B baseline: 97/97 tests passing
  - Story 3.2.8 current: 106/106 tests passing
- 9 new integration tests in SituationAssuranceVieServiceSqliteIntegrationTests.cs:
  - EndToEnd_BrioImportAndReloadFromSqlite_ShouldConstructAccurateSituationReadModel
  - GetSituationForClientAsync_WithMultipleContracts_ReturnsAggregatedReadModel
  - GetSituationForClientAsync_WithoutContracts_ReturnsEmptySituation
  - GetSituationForDefaultClientAsync_WithZeroClients_ReturnsNoClientsAvailable
  - GetSituationForDefaultClientAsync_WithOneClient_ReturnsClientLoaded
  - GetSituationForDefaultClientAsync_WithMultipleClients_ReturnsMultipleClientsRequireSelection
  - DetermineInsurerDisplayName_WithInsurerAggregate_ReturnsDisplayName
  - DetermineInsurerDisplayName_WithoutInsurerAggregate_FallsBackToRawInsurerName
  - DetermineInsurerDisplayName_WithoutAnyInsurerInfo_ReturnsDefaultMessage
- Manual validation: Portfolio displays insurance situations from SQLite with correct fallback behavior

**Constraints Story 3.2.8**

- No Domain entity modifications
- No repository interface changes
- No new persistence infrastructure (reuses Story 3.2.7 implementation)
- No financial calculation changes
- DemoSituationAssuranceVieDataService preserved but unused
- "Ma situation" integration remains future work

**Runtime Persistence Status**

As of Story 3.2.8, the application uses EF Core/SQLite for runtime persistence:
- Client and Contract data persists to `%LOCALAPPDATA%\Praxis360\praxis360.db`
- BRIO import workflow creates persistent records via EfCoreBrioPersistenceService
- Portfolio UI displays insurance situations by loading Client and Contract entities via EfCoreClientRepository and EfCoreContractRepository
- SituationAssuranceVie is constructed dynamically from Client and Contract data; no separate SituationAssuranceVie persistence exists
- In-memory repositories are no longer used at runtime

