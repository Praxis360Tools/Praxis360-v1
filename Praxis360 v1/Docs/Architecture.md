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

