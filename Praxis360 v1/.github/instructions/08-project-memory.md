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

## Decisions durables — Story 3.2.1

Courte synthèse des décisions durables issues de la Story 3.2.1 :

- « Ma situation » est construite au moyen de projections de lecture et non d'une Entity Domain SituationAssuranceVie.
- Une donnée inconnue ne doit jamais être transformée en valeur zéro.
- ReserveAcquise, CapitalDeces et RevenuGaranti restent indisponibles tant que le Domain ne fournit pas de données explicites permettant de les calculer.
- Aucune heuristique basée sur le nom textuel des enums n'est autorisée.
- BRIO ou Portima peuvent être cités comme sources externes futures mais ne doivent jamais définir ou contaminer le Domain Praxis360.
- La source actuelle de démonstration est provisoire et reste hors du Domain.
- Le nom produit visible est « Ma situation » ; la route /patrimoine est temporairement conservée pour préserver la navigation existante.

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