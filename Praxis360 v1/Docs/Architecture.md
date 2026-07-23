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
  - GlobalErrors — list of global error messages
  - GlobalWarnings — list of global warning messages
  - Outcome — ApplicationOutcome enum value
- ContractCreated — individual contract creation result
- ContractAlreadyExisting — individual already existing contract result
- ContractSkipped — individual skipped contract result
- ContractUnresolved — individual unresolved contract result
- ApplicationOutcome — enum (Success | PartialSuccess | Failed)

**Business Rules**
- Client selection: match by NormalizedIdentity from BrioClientCandidate
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

