# Praxis360 Sprint Book

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
- Architecture.md
- Roadmap.md

---

# Table of Contents

1. Purpose
2. Sprint Methodology
3. Sprint Status
4. Sprint History
5. Current Sprint
6. Next Sprint
7. Completed Milestones
8. Technical Debt
9. Lessons Learned

---

# 1. Purpose

SprintBook is the official development journal of Praxis360.

It records every Sprint, the objectives achieved, major architectural decisions and the future development roadmap.

Each Sprint must leave the project in a better state than before.

---

# 2. Sprint Methodology

Praxis360 follows an incremental Sprint methodology.

Rules:

- One Sprint = One clear objective.
- Every Sprint must compile successfully.
- Documentation is updated before a Sprint is considered complete.
- Business vision always precedes technical implementation.
- No unfinished functionality is carried into the next Sprint.

---

# 3. Sprint Status

| Sprint | Status |
|---------|--------|
| Sprint 0 | ✅ Completed |
| Sprint 1 | ✅ Completed |
| Sprint 2 | ✅ Completed |
| Sprint 2.1 | ✅ Completed |
| Sprint 2.2 | ✅ Completed |
| Sprint 2.3 | ✅ Completed |
| Sprint 2.4 | ✅ Completed |
| Sprint 3.0 | ✅ Completed |
| Sprint 3.1 | 🚧 In Progress |

---

# 4. Sprint History

## Sprint 0 – Project Foundation

Objectives

- Create repository
- Configure development environment
- Define initial architecture

Status

Completed.

---

## Sprint 1 – Application Foundation

Objectives

- Dashboard
- Navigation
- Base layout

Status

Completed.

---

## Sprint 2 – Document Workspace

Objectives

- My Space
- Document management
- Shared components
- Services
- Scanner foundation

Status

Completed.

---

## Sprint 2.1 – Component Stabilization

Objectives

- Improve reusable components
- UI consistency

Status

Completed.

---

## Sprint 2.2 – Architecture Improvements

Objectives

- Improve project structure
- Refine services
- Improve maintainability

Status

Completed.

---

## Sprint 2.3 – Shared Components

Objectives

- Stabilize reusable components
- Improve design consistency

Status

Completed.

---

## Sprint 2.4 – AI Workspace

Objectives

- Complete project documentation
- Standardize AI workflow
- Introduce GitHub Copilot workflow
- Create Design Bible
- Create Motion Guide
- Create AI Playbook
- Organize project documentation

Status

Completed.

---

## Sprint 3.0 – Product Foundation

Objectives

- Redefine the product vision
- Introduce ProductVision.md
- Reposition Praxis360 around Life Insurance
- Replace Portfolio vision with Client Understanding
- Align documentation with the new business strategy

Major Decision

Praxis360 is no longer designed as a portfolio management application.

It becomes a premium client workspace focused on helping customers understand their Life Insurance situation through simple, reassuring and meaningful information.

Status

Completed.

---

## Sprint 3.1 – Domain Model

Objectives

Design the complete business domain before writing any implementation.

Focus areas:

- Insured-centric model
- Client Workspace
- Life Insurance contracts
- Customer-oriented business language
- Clean business architecture

Status

In Progress.

### Stories

- Story 3.1.1 — Domain Types Catalog — Done
- Story 3.1.2 — Domain Type Implementation — Done
- Story 3.1.3 — Value Objects — Completed

Summary: Implemented ContractNumber, Money, Percentage and DateRange value objects with DDD-compliant invariants and build validated.

---

# 5. Current Sprint

Sprint 3.1 – Domain Model

Goal

Design the complete business model that will serve as the foundation for every future feature.

No UI.

No Services.

No Imports.

No implementation.

Only the business domain.

---


# 6. Current Sprint

Sprint 3.2 — Première Situation Assurance Vie

Planned objectives

- construire une projection de lecture Assurance Vie
- transformer les données du Domain en informations compréhensibles
- alimenter progressivement l'interface « Ma situation »
- préserver la distinction entre données inconnues et valeurs nulles
- préparer les futures projections liées aux besoins de vie

Status

- In Progress

Stories

- Story 3.2.1 — Première projection « Situation Assurance Vie » — Implémentée (non committée)

  Statut de la Story 3.2.1

  - Implémentée
  - Compilée
  - Architecture Review validée
  - Corrections appliquées
  - Validation visuelle réussie
  - Documentation synchronisée
  - Prête pour commit et push
  - Pas encore commitée
  - Pas encore poussée

  Résumé fonctionnel

  - Création d'un ReadModel de situation Assurance Vie.
  - Création d'un service applicatif de projection.
  - Création d'une source de données de démonstration (hors Domain) et intégration par Dependency Injection.
  - Consommation de la projection dans la page Portfolio.razor et affichage sous le titre « Ma situation ».
  - La route technique /patrimoine est conservée temporairement.
  - Le libellé de navigation visible a été renommé en « Ma situation » (route inchangée).

  Contenu de la projection

  - Identité du client.
  - Nombre total de contrats.
  - Nombre de contrats en cours.
  - ReserveAcquise (nullable).
  - CapitalDeces (nullable).
  - RevenuGaranti (nullable).
  - Liste typée des contrats et de leurs statuts.

  Règles métier et traitement des statuts

  - Statuts considérés comme "en cours" : Active, PaidUp, Suspended.
  - Statuts non considérés comme en cours : Draft, Terminated, Matured.

  Gestion des valeurs inconnues

  - ReserveAcquise, CapitalDeces et RevenuGaranti restent null lorsque inconnus.
  - L'interface affiche « Non disponible » pour les valeurs nulles.
  - Aucune valeur inconnue n'est remplacée par zéro.

  Décisions architecturales (validées)

  - Aucune Entity SituationAssuranceVie n'a été créée dans le Domain : la situation est une projection de lecture.
  - Aucune règle métier n'est exécutée dans Razor.
  - Aucune heuristique fondée sur le nom textuel des enums n'est utilisée.
  - La source de démonstration reste provisoire et hors du Domain (DemoSituationAssuranceVieDataService).
  - Le service applicatif dépend temporairement de la source de démonstration.
  - Aucune interface ou repository d'infrastructure prématurée n'a été créée.

  Données de démonstration

  - Client : Jean Dupont.
  - Trois contrats : 1 Active, 1 Terminated, 1 PaidUp.
  - Résultat attendu : 3 contrats connus et 2 contrats en cours.

  Validation

  - Build local réussi (après arrêt du processus verrouillant l'exécutable).
  - Validation visuelle réussie sur la page accessible via /patrimoine.
  - Aucun comportement fonctionnel inattendu constaté.

	Prochaine étape

  - Validation documentaire finale
  - Commit et push après autorisation du CTO / Product Owner

---

# 7. Completed Milestones

- Project foundation
- Dashboard
- Document Workspace
- Scanner foundation
- Shared Components
- Documentation framework
- AI Workspace
- Product Vision
- Development Blueprint
- Architecture redesign

---

# 8. Technical Debt

Current priorities

- Complete Domain Model
- Implement BRIO mapping
- Expand Design Bible with component catalogue
- Create customer journey diagrams
- Add architecture diagrams

No critical technical debt identified.

---

# 9. Lessons Learned

Major lessons from Sprint 3:

Business vision must always precede technical implementation.

The customer experience is more important than exposing technical data.

The Domain Model should represent the customer's reality, not the structure of external systems.

Good documentation significantly improves AI-assisted development.

Praxis360 is not built around contracts.

It is built around customer understanding.