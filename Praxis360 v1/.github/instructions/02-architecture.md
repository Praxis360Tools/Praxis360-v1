# Architecture

## Purpose

This document summarizes the architectural rules of Praxis360.

For the complete technical documentation, see:

- Docs/Architecture.md

---

## Architecture Principles

Praxis360 follows a simple, modular and scalable architecture.

Every new feature must integrate into the existing structure.

Never introduce alternative architectural patterns.

---

## Layers

Pages

- UI orchestration only
- No business logic

Shared Components

- Reusable UI only
- Presentation layer

Models

- Data only
- No business logic

Services

- Business logic
- Validation
- Data manipulation

---

## Dependency Injection

- Every Service is registered in Program.cs.
- Always use constructor injection.
- Never instantiate Services manually.

---

## Core Rules

Always:

- Reuse existing components.
- Keep business logic inside Services.
- Keep Pages lightweight.
- Keep Models simple.
- Respect the existing architecture.

Never:

- Duplicate components.
- Place business logic in Razor pages.
- Reference UI from Services.
- Introduce unnecessary abstractions.

---

## Development Checklist

Before completing a task:

- Architecture respected.
- Existing components reused.
- Dependency Injection respected.
- Project compiles successfully.

---

## Reference

See Docs/Architecture.md for the complete architecture documentation.