# Project Memory

## Purpose

This document records the permanent decisions of the Praxis360 project.

These decisions are considered stable and should remain valid across future Sprints unless explicitly changed.

For the complete development methodology, see:

- Docs/AIPlaybook.md

---

## Product

Always remember:

- Praxis360 is a premium .NET 10 MAUI Blazor mobile application.
- The application targets Belgian self-employed professionals.
- It is built for end users, not advisors.
- It is a long-term software product.

Long-term maintainability is always more important than short-term development speed.

---

## Development

Always remember:

- One Sprint equals one complete feature.
- Never start a new Sprint before finishing the current one.
- Every Sprint must compile successfully.
- Documentation is part of the product.
- Code should always be production-ready.

---

## Architecture

Always remember:

- Pages orchestrate.
- Services contain business logic.
- Models contain data only.
- Shared contains reusable UI components.
- Dependency Injection is mandatory.
- Existing architecture must be preserved.

---

## Design

Always remember:

- Premium before flashy.
- Simplicity before complexity.
- Consistency before originality.
- Reuse existing UI components.
- Follow the Design Bible.

---

## Coding

Always remember:

- Deliver complete files.
- Produce compilable code.
- Reuse existing code whenever possible.
- Avoid duplicated code.
- Keep solutions simple.
- Respect project naming conventions.

---

## Documentation

Always remember:

Docs/ is the official project knowledge base.

.github/ contains operational instructions for AI assistants.

Whenever documentation and code disagree, update the documentation as part of the Sprint.

---

## Long-Term Goal

Praxis360 should remain understandable, maintainable and scalable after many years of development.

Every contribution should improve the project rather than increase its complexity.

---

## Reference

See:

- Docs/AIPlaybook.md
- Docs/Blueprint.md
- Docs/Architecture.md