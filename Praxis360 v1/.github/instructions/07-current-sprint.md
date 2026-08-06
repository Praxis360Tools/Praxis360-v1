# Current Sprint

## Purpose

This document describes the active Sprint.

It must be updated at the beginning and at the end of every Sprint.

For Sprint history, see:

- Docs/SprintBook.md

---

## Sprint Information

Sprint

Sprint 3.2

Name

Situation Assurance Vie

Status

In Progress

---

## Sprint Goal

Build a professional AI-assisted development environment for Praxis360 by completing the project documentation and aligning GitHub Copilot with the project standards.

---

## Current Objectives

- Complete project documentation.
- Review GitHub instructions.
- Improve AGENTS.md.
- Improve Copilot prompts.
- Validate project architecture.
- Prepare Sprint 3.

---

## Success Criteria

The Sprint is complete when:

- Documentation is complete.
- GitHub instructions are aligned with Docs.
- AIPlaybook is validated.
- AGENTS.md is updated.
- Prompts are reviewed.
- Project compiles successfully.
- SprintBook has been updated.

## Stories

- Story 3.2.1 — Première projection « Situation Assurance Vie » — Terminée, validée, commitée et poussée sur origin/master
- Story 3.2.2 — Vue synthétique et détail des contrats d'assurance vie — Terminée, validée, commitée et poussée sur origin/master
- Story 3.2.3 — Import CSV contrôlé des contrats d'assurance vie — Completed
  - Phase 1 — Domain multi-source: Completed
  - Phase 2 — BRIO CSV reader and in-memory repositories: Completed
  - Phase 3 — BRIO business analysis and candidate generation: Completed (commit 0bf40ee)
  - Phase 4 — Controlled client selection/creation and application to in-memory repositories: Completed (commit db55fc8)
  - Cumulative validation: 18 of 18 scenarios passed
  - Capabilities: controlled import to in-memory repositories, client reconciliation, idempotent contracts
  - Limitations: in-memory only, no real persistence, no BRIO UI, no financial data added
- Story 3.2.4 — Prévisualisation contrôlée d'un fichier BRIO — Completed
  - Implementation commits: 97a4a32, 4d10c2e
  - Route: /imports/brio
  - Read-only preview interface for BRIO CSV files
  - Reuses IBrioFileReader and IBrioImportAnalyzer services
  - Displays analysis summary, client candidates, contract candidates, warnings, blocking errors
  - Security: generic error messages, explicit guards, resource cleanup
  - Build successful
  - Manual validation successful with anonymized BRIO CSV files (valid and error cases)
  - No automated test infrastructure available
  - Limitations: no contract application, no persistence, no client creation/modification, no financial data
- Story 3.2.5 — Controlled BRIO Contract Application — Completed
  - Implementation commit: dd57e5a, merge commit: 4b76ebe, PR #4
  - Controlled application flow with explicit client selection and destination choice
  - IClientSelectionService / ClientSelectionService / SelectableClient
  - UiStep state machine: Preview, SelectingClient, ChoosingDestination, Confirming, Applying, Completed
  - In-memory application with idempotent contract creation
  - Manual UI validation successful
  - Newly created clients immediately visible in existing-client list
  - Limitations: no real persistence, no financial data, no connection to "Ma situation"
- Story 3.2.6 — BRIO Synthetic Fixtures and Automated Coverage — Completed
  - PR #6 (functional), PR #7 (corrective)
  - Permanent Praxis360 v1.Tests project with xUnit infrastructure
  - Five fully synthetic CSV fixtures (only CSV files allowlisted for version control)
  - Coverage for BrioCsvFileReader, BrioImportAnalyzer, BrioContractApplicationService
  - 43 test cases: 43 passed, 0 failed, 0 skipped
  - Automated fixture validation and confidentiality guard
  - Limitations: no real persistence, no financial data, no Domain modification
- Story 3.2.8 — Situation reload from SQLite with multi-client selection — Ready for Architecture Review
  - Repository-backed async loading of "Ma situation" from persisted SQLite data
  - Explicit multi-client selection when multiple clients exist (no arbitrary selection)
  - Route-based client identification via /patrimoine/{ClientId:guid}
  - Six distinct UI states: Loading, ClientLoaded, NoClientsAvailable, MultipleClientsRequireSelection, ClientNotFound, ErrorLoading
  - Post-import "Voir Ma situation" link when BrioContractApplicationResult has ClientId and applied contracts
  - SituationAssuranceVieLoadResult wrapper with typed outcome enum
  - Full end-to-end integration test: BRIO import → persistence → service recreation → situation reload → exact assertion
  - 110 test cases: 110 passed, 0 failed, 0 skipped
  - Insurer fallback: Insurer.DisplayName → most recent BRIO provenance RawInsurerName → "Compagnie non disponible"
  - SituationAssuranceVieService registered as scoped lifetime (aligned with scoped repositories)
  - DemoSituationAssuranceVieDataService removed from runtime DI registration
  - Components created: SituationAssuranceVieLoadResult.cs, SituationAssuranceVieServiceSqliteIntegrationTests.cs
  - Components modified: SituationAssuranceVieService.cs, Portfolio.razor, Portfolio.razor.css, BrioImport.razor, BrioImport.razor.css, Program.cs
  - Limitations: Financial indicators remain null pending separate calculation feature, Manual validation of user database not performed

---

## Reference

See:

- Docs/SprintBook.md
- Docs/Roadmap.md
