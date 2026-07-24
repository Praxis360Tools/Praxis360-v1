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

---

## Reference

See:

- Docs/SprintBook.md
- Docs/Roadmap.md
