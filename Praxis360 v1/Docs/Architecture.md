# Praxis360 Architecture

| Property | Value |
|----------|-------|
| Version | V1.0 |
| Status | Active |
| Owner | Praxis360 |
| Last Updated | 2026-07-13 |

## Related Documents

- Blueprint.md
- ProductBook.md
- DesignBible.md
- SprintBook.md

---

# Table of Contents

1. Architecture Philosophy
2. Project Structure
3. Application Layers
4. Data Flow
5. Dependency Injection
6. Naming Conventions
7. Shared Components
8. Services
9. Models
10. Pages
11. Scalability Principles
12. Development Rules
13. Definition of Done
14. References

---

# 1. Architecture Philosophy

Praxis360 follows a clean, modular and scalable architecture.

The objective is to make the project easy to understand, easy to maintain and easy to extend over many years.

Every new feature must integrate into the existing architecture rather than creating new patterns.

Simplicity is preferred over unnecessary abstraction.

---

# 2. Project Structure

Typical project structure:

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

# 3. Application Layers

## Pages

Responsibilities:

- Display UI
- Receive user interactions
- Call Services
- Compose Shared Components

Pages must never contain business logic.

---

## Shared Components

Responsibilities:

- Reusable UI
- Common layouts
- Navigation
- Cards
- Buttons
- Search components

If a component is reused more than once, it should generally belong in Shared.

---

## Models

Responsibilities:

- Represent data
- Define entities
- Transfer information

Models never contain business logic.

---

## Services

Responsibilities:

- Business rules
- Validation
- Filtering
- Data manipulation
- Communication with storage or APIs

Services should remain independent from the UI.

---

# 4. Data Flow

Application flow:

User

↓

Page

↓

Shared Components

↓

Service

↓

Models

↓

Storage / API

The flow should always remain simple and predictable.

---

# 5. Dependency Injection

Every Service must be registered in Program.cs.

Services are always injected through constructors.

Never instantiate Services manually.

Correct:

Constructor Injection

Incorrect:

new DocumentService()

---

# 6. Naming Conventions

Pages

Dashboard.razor

Index.razor

Portfolio.razor

Scanner.razor

Components

DocumentCard

CategoryCard

P360Card

Services

DocumentService

ScannerService

PortfolioService

Models

Document

Category

Portfolio

Naming must remain explicit.

---

# 7. Shared Components

Shared components should:

- Be reusable.
- Remain UI only.
- Avoid business logic.
- Keep parameters simple.

---

# 8. Services

Services should:

- Be focused.
- Respect the Single Responsibility Principle.
- Be testable.
- Be reusable.

---

# 9. Models

Models should:

- Contain data only.
- Stay lightweight.
- Avoid dependencies.

---

# 10. Pages

Pages orchestrate the application.

Their responsibilities are:

- Display information.
- Trigger actions.
- Coordinate components.

Business decisions belong inside Services.

---

# 11. Scalability Principles

The architecture must support future modules:

- Authentication
- OCR
- Notifications
- Cloud Synchronization
- AI Assistant
- Microsoft Graph

Without requiring major refactoring.

---

# 12. Development Rules

Always:

- Reuse existing components.
- Respect the architecture.
- Prefer readability.
- Keep files reasonably small.
- Keep methods focused.

Never:

- Duplicate code.
- Place business logic inside Razor pages.
- Create unnecessary abstractions.
- Break existing architecture.

---

# 13. Definition of Done

A feature is architecturally complete when:

- Layers are respected.
- Shared components are reused.
- Services contain business logic.
- Pages remain lightweight.
- Dependency Injection is respected.
- The project compiles successfully.

---

# 14. References

- Blueprint.md
- ProductBook.md
- DesignBible.md
- SprintBook.md