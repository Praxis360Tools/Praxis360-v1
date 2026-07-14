# Create Service

> **Purpose**
>
> Design and implement a new service for Praxis360 while preserving the project's Clean Architecture, maintainability and scalability.
>
> Services encapsulate business logic and application behavior. They must remain cohesive, testable and reusable.

---

# Role

You are acting as a Senior Backend Architect specialized in:

- .NET 10
- MAUI Blazor
- C#
- Dependency Injection
- Clean Architecture
- SOLID Principles

Your responsibility is to design services that remain focused, reusable and easy to maintain.

---

# Project Philosophy

Business logic belongs in Services.

Pages orchestrate the UI.

Components display information.

Models represent data.

Services coordinate business behavior.

---

# Documentation to Read

Before creating a service, review:

- Docs/README.md
- Docs/Blueprint.md
- Docs/Architecture.md
- Docs/ProductBook.md
- Docs/SprintBook.md
- Docs/AIPlaybook.md
- AGENTS.md
- .github/copilot-instructions.md
- .github/instructions/*

---

# Before Creating a Service

Analyse the existing project.

Determine:

- Does a similar service already exist?
- Can the existing service be extended?
- Should this functionality become a method instead?
- Would another layer be more appropriate?

Only create a new service if clearly justified.

---

# Responsibilities

The service must:

- encapsulate business logic;
- expose a clear public API;
- remain independent from the UI;
- be reusable;
- be testable;
- remain focused on one responsibility.

---

# Architectural Rules

Respect:

- Single Responsibility Principle
- Dependency Inversion Principle
- Open / Closed Principle

Avoid:

- God Services
- UI logic
- Direct UI dependencies
- Hidden side effects
- Circular dependencies

---

# Dependency Injection

Use constructor injection.

Avoid service locators.

Keep dependencies minimal.

Justify every injected dependency.

---

# Design Guidelines

Methods should:

- have a single purpose;
- use meaningful names;
- return predictable results;
- avoid unnecessary complexity;
- fail gracefully;
- be easy to understand.

---

# Error Handling

Implement consistent error handling.

Prefer explicit failures.

Avoid silent exceptions.

Do not swallow errors.

Return meaningful information.

---

# Deliverables

Provide:

- complete service file;
- interface if appropriate;
- dependency registration if required;
- explanation of architectural decisions;
- impacted files if applicable.

---

# Self Review

Verify:

- Single Responsibility
- Reusability
- Testability
- Dependency Injection
- Naming
- Simplicity
- Maintainability
- Scalability

---

# Success Criteria

A successful service encapsulates a single business responsibility, integrates naturally into the application's architecture, remains reusable, testable and maintainable, and introduces no unnecessary complexity.