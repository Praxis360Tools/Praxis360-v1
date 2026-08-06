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

## Decisions durables — Story 3.2.8

Courte synthèse des décisions durables issues de la Story 3.2.8 :

- Le chargement de « Ma situation » s'effectue depuis les données SQLite persistées via les repositories, abandonnant définitivement le DemoSituationAssuranceVieDataService en runtime.
- Lorsque plusieurs clients existent dans la base de données, aucun client n'est sélectionné arbitrairement : une sélection explicite par l'utilisateur est requise via l'écran de sélection multi-client.
- L'identification d'un client via la route /patrimoine/{ClientId:guid} permet l'accès direct à la situation d'un client connu.
- Les six états UI distincts (Loading, ClientLoaded, NoClientsAvailable, MultipleClientsRequireSelection, ClientNotFound, ErrorLoading) garantissent une expérience utilisateur claire et sans ambiguïté.
- La persistance SQLite avec Entity Framework Core via LocalAppData est la source de vérité pour les données clients et contrats après import BRIO.
- Le SituationAssuranceVieService est enregistré avec une durée de vie Scoped, alignée avec la durée de vie des repositories.
- Le fallback assureur (Insurer.DisplayName → BRIO provenance RawInsurerName → "Compagnie non disponible") assure une UI cohérente même en l'absence de données assureur.
- Les tests d'intégration end-to-end couvrent le cycle complet : import BRIO → persistence → recréation du service → rechargement situation, garantissant la cohérence Domain-Repository-Service-UI.

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