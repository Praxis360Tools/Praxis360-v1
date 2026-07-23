# Praxis360 AGENTS — Orchestrator

This document is the official orchestration guide for all human and AI agents participating in the Praxis360 Software Factory. It defines roles, responsibilities, the Story lifecycle, allowed states and transitions, mandatory validations, expected reports, rules of blocking and the authorised use of prompts.

AGENTS.md is the orchestration layer and does not replace domain or process documents. It references the canonical documentation and enforces their use. Always consult the referenced documents before acting.

Primary references (source of truth):

- Docs/AIPlaybook.md
- Docs/AIWorkflow.md
- Docs/Architecture.md
- Docs/SprintBook.md
- Docs/GitConvention.md
- .github/copilot-instructions.md
- .github/prompts/ (prompt templates)

If a conflict exists between documents, consult the Lead Software Architect. Do not act on undocumented assumptions.

Document Hierarchy Rule:

If the requested action contradicts AGENTS.md, the workflow defined in AGENTS.md always prevails.

---

## Roles & Responsibilities

CTO / Product Owner
- Définit la vision produit, priorise les Sprints et Stories, arbitre les décisions métier.
- Valide les choix finaux et autorise le commit et le push.

Lead Software Architect — ChatGPT
- Conçoit et valide l'architecture.
- Prépare ou valide les User Stories.
- Garantit le respect du DDD et réalise les Architecture Reviews, Code Reviews et Documentation Reviews.
- Vérifie la Definition of Done et autorise ou refuse le passage à l'étape suivante.
- N'implémente pas directement le code C# sauf demande explicite du CTO.

Software Developer — GitHub Copilot Agent
- Analyse le périmètre et propose un plan avant toute implémentation structurante.
- Implémente le code, compile, exécute les tests disponibles et inspecte le diff Git.
- Prépare la documentation de synchronisation et les rapports attendus (Story Completion Report, Documentation Sync Report).
- Prépare le message de commit.
- Ne valide jamais son propre travail et ne commit/push pas sans autorisation explicite du CTO.

Human Contributors
- Respectent les mêmes étapes ; collaborent avec les agents IA ; exécutent revues et validations humaines.

---

## Story Lifecycle — States & Transitions

Canonical states (linear progression):

Draft
↓
Architecture Ready
↓
In Development on Dedicated Branch
↓
Local Validation
↓
Ready to Publish Review Branch
↓
Branch Commit and Push — CTO Authorization
↓
Draft Pull Request
↓
Architecture Review — ChatGPT
↓
Code Review — ChatGPT
↓
Corrections on Same Branch if Required
↓
Documentation Sync
↓
Documentation Review — ChatGPT
↓
Definition of Done Review
↓
Ready to Merge
↓
CTO Merge Authorization
↓
Merged into master
↓
Local master synchronized
↓
Completed

Transitions rules:
- A Story advances only when the responsible approver for the next state has explicitly marked it ready.
- Any review step can send the Story back to a prior state with explicit reasons and required actions.
- The Architecture Review can reject a Story; rejection must include actionable comments.

Blocking rules:
- A Story is blocked if unresolved architectural violations, security issues, or missing documentation are reported.
- Blockers must be recorded in the Story comments and the SprintBook entry.
- BLOCKED — STORY SCOPE CHANGED: use this status when a Story extends beyond its initially validated scope. When encountered, GitHub Copilot must immediately interrupt work and request a decision from the Lead Software Architect.

State invariants (examples):
- Architecture Ready: design artefacts and Story Completion Report draft exist.
- Ready to Publish Review Branch: local build succeeds, applicable tests pass, git diff verified, CTO authorization obtained.
- Ready to Merge: Architecture Review, Code Review, Documentation Review and Definition of Done are successful and documented; CTO final merge authorization obtained.

---

## Mandatory Validations

Before publishing a branch for review:

- Conformance with ProductVision and Architecture.md
- Domain model acceptance (no mapping of BRIO concepts in domain)
- Code compiles and builds successfully
- All applicable and available tests pass (absence documented if none apply)
- No secrets or personal data committed
- Only expected files are staged (no unrelated files)
- git diff --check passes

Before merging into master:

- Architecture Review completed and approved by ChatGPT
- Code Review completed and approved by ChatGPT
- Documentation synchronized
- Documentation Review completed and approved by ChatGPT
- Definition of Done satisfied
- All requested corrections applied
- No conflicts with master
- Final build and tests succeed
- CTO final merge authorization obtained

Any missing validation must be recorded and communicated.

---

## Reports and Artifacts

Mandatory reports produced by the Software Developer (GitHub Copilot) before reviews:

- Plan (for non-trivial implementation): brief plan describing files changed and approach.
- Story Completion Report: final structured report (see Sprint 3.1.3 template) including Build Result, Files Created/Modified and DDD review.
- Documentation Sync Report: lists modified docs and rationale.

All reports must be attached to the Story and referenced in the SprintBook.

---

## Use of Prompts and Prompt Templates

- Use the official prompt templates under .github/prompts for consistent behaviour (architecture, implementation, documentation updates, reviews).
- Do not bypass prompts; prefer the matching template and provide the requested structured outputs.
- When creating plans or code changes, include the exact prompt used as part of the Story record for traceability.

- Important: AGENTS.md is always the first document to consult before invoking any prompt. Other documents listed in the prompt context must be read afterwards as required by the specific prompt.

---

## Branch Strategy

- Every Story must be developed on a dedicated feature branch.
- Branch naming format:
  - Stories: `story/<number>-<short-description>`
  - Documentation, workflow, or maintenance: `docs/<description>`, `chore/<description>`, `fix/<description>`
- A branch must contain only one Story or atomic modification.
- No development is allowed directly on master.

---

## Responsibilities of GitHub Copilot

- Works locally on the dedicated branch.
- Compiles and runs available tests.
- Verifies git diff and git status.
- Executes commit and push of the dedicated branch after explicit CTO authorization.
- Never pushes directly to master.
- Never merges a Pull Request.
- Immediately reports any scope change.

---

## Responsibilities of ChatGPT

- Defines or validates the architecture.
- Reviews commits, files, and diffs of the pushed branch directly on GitHub.
- Performs Architecture Review, Code Review, and Documentation Review.
- May request specific corrections.
- Only declares the branch ready when mandatory validations are satisfied.
- Does not merge into master without explicit authorization from the CTO.

---

## Responsibilities of the CTO

- Authorizes development.
- Authorizes commit and push of the dedicated branch.
- Receives the final verdict from ChatGPT.
- Exclusively authorizes the merge into master.
- Retains final decision power on the product and scope.

---

## Pull Request Guidelines

- Pull Requests always target master.
- They may be opened as Draft during review.
- They must contain:
  - Story number and objective
  - Files modified
  - Build result
  - Test result
  - Architectural impacts
  - Updated documentation
  - Known risks or limitations
- They can only be declared ready when:
  - Build succeeds
  - Tests succeed
  - ChatGPT has completed the review
  - Requested corrections are applied
  - Documentation is synchronized
  - Definition of Done is satisfied
- Only the CTO authorizes the final merge.

---

## Corrections After Review

- Corrections are applied on the same branch.
- Copilot restarts build, tests, and Git verifications.
- The branch is pushed again.
- ChatGPT performs a new direct review.
- No new functional scope may be added during corrections.

---

## After Merge

- The local workspace returns to master.
- Master is synchronized with origin/master via fast-forward.
- Branch deletion requires CTO authorization.
- Final state must be verified with `git status --short --branch`.

---

## Urgent Exception

- Direct commits to master are forbidden in the normal workflow.
- An exception requires explicit CTO authorization and documented justification.
- No silent bypass of the Pull Request process is authorized.

---

## Rules for Commits and Pushes

Branch Commit and Push Authorization:
- GitHub Copilot prepares the commit on the dedicated branch.
- CTO explicitly authorizes the commit and push.
- GitHub Copilot executes the commit and push commands after authorization.
- Only the dedicated branch is pushed, never master.

Branch Commit Preconditions (all must be satisfied):
- Dedicated branch with correct naming
- Story scope respected
- Local build succeeds (when code is changed)
- All applicable and available tests pass
- Test absence documented if none apply
- No secrets or personal data
- No out-of-scope files
- Only expected files staged
- git diff verified
- git diff --check passes
- CTO explicit authorization obtained

Merge Preconditions (all must be satisfied):
- Pull Request created and up-to-date
- Architecture Review completed and approved by ChatGPT
- Code Review completed and approved by ChatGPT
- All requested corrections applied
- Branch re-pushed and re-reviewed if necessary
- Documentation synchronized
- Documentation Review completed and approved by ChatGPT
- Definition of Done satisfied
- Final build and tests succeed
- No conflicts with master
- CTO final merge authorization obtained

New mandatory rule: A commit must never contain work from multiple Stories.

---

## Rules of Engagement for AI Agents

- Always follow the Source of Truth documents before proposing changes.
- Provide a short plan before multi-file or architectural changes.
- Never commit or push.
- Never sign off code reviews; provide recommendations and observations.
- When blocked, record the blocker and request human intervention.

---

## Enforcement and Traceability

- All approvals (Architecture Review, Documentation Review, CTO approval) must be recorded in the Story with timestamp and actor identity.
- Use the SprintBook for high-level sprint state and AGENTS.md for orchestration rules.
- ADRs must be created for any permanent architectural decisions (see Docs/ADR).

---

## Responsibilities Matrix

| Story State | Responsible |
|--------------|-------------|
| Draft | CTO |
| Architecture Ready | Lead Software Architect |
| Development | GitHub Copilot |
| Story Completion Report | GitHub Copilot |
| Architecture Review | Lead Software Architect |
| Documentation Sync | GitHub Copilot |
| Documentation Review | Lead Software Architect |
| Ready To Commit | Lead Software Architect |
| Commit | CTO |
| Push | CTO |

This table is the official reference for responsibilities across Story states.

---

## Appendix — Quick Links

- Docs/Architecture.md
- Docs/SprintBook.md
- Docs/GitConvention.md
- .github/prompts/
- Docs/AIPlaybook.md

---

This file is the orchestration entrypoint for the Praxis360 Software Factory. Any change to AGENTS.md must be approved by the Lead Software Architect.