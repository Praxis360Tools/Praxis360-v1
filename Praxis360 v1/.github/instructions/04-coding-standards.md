# Coding Standards

## Purpose

This document defines the coding standards used throughout Praxis360.

For the complete development process, see:

- Docs/AIPlaybook.md
- Docs/Architecture.md

---

## General Principles

Always write code that is:

- Readable
- Maintainable
- Consistent
- Scalable
- Production-ready

Code should prioritize clarity over cleverness.

---

## C# Standards

Always:

- Use modern C#
- Use file-scoped namespaces
- Use Dependency Injection
- Use explicit naming
- Use async/await where appropriate
- Keep methods focused on a single responsibility

Prefer:

- Constructor injection
- Expression-bodied members when they improve readability
- Collection expressions where appropriate
- Pattern matching when it improves clarity

---

## Project Standards

Always:

Always:

- Deliver complete files.
- Produce compilable code.
- Preserve existing functionality.
- Reuse existing components.
- Respect the current architecture.
- Follow the Design System.
- Prefer extending existing code over creating parallel implementations.
- Keep solutions as simple as possible.

Never:

Never:

- Leave TODO placeholders.
- Duplicate code.
- Introduce unnecessary abstractions.
- Create unused code.
- Break existing functionality.
- Introduce new libraries without justification.
- Modify unrelated code when implementing a feature.

---

## Documentation

Whenever code changes require it:

- Update Docs.
- Keep documentation synchronized with the implementation.

Documentation is considered part of the product.

---

## Reference

See:

- Docs/Architecture.md
- Docs/AIPlaybook.md