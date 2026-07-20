# Documentation Map — Praxis360

Purpose

DocumentationMap.md is the canonical manifest that maps project areas and types of changes to the documents that must be reviewed or updated. It helps contributors and agents quickly determine which documentation artifacts are affected by a Story without describing document contents.

Scope

This document is used by AGENTS.md, the GitHub Copilot Agent, ChatGPT and the CTO to identify documentation impacts when planning, reviewing or completing work. It does not replace the documents it references; it only identifies relationships between changes and documents.

Documentation Responsibilities

Short descriptions of the major documents and their primary responsibilities:

- AGENTS.md — defines the roles, the responsibilities, the Story states and the official workflow. It is the primary orchestration reference for the Software Factory.
- Architecture.md — defines structural decisions, layering, domain boundaries and architectural principles.
- Blueprint.md — development rules and engineering guidelines for implementation.
- ProductVision.md — high-level product goals and business priorities.
- ProductBook.md — functional descriptions and business behaviour expectations.
- SprintBook.md — sprint records, objectives and completion summaries.
- DesignBible.md — UI components, design tokens and visual patterns.
- MotionGuide.md — animation and interaction rules tied to the design system.
- AIWorkflow.md — execution process and operational steps for AI contributions.
- AIPlaybook.md — AI collaboration philosophy, responsibilities and behaviour rules.
- DefinitionOfDone.md — canonical Definition of Done gating criteria for Stories.
- GitConvention.md — commit and branching conventions, responsibilities for commits/pushes.
- ADRs (Docs/ADR/) — recorded architectural decisions and their rationale.

Change Impact Matrix

The matrix below maps common types of modifications to the documents that should be reviewed or updated. Use this matrix as the starting point for Documentation Sync.

| Modification | Documentation to review/update |
|---------------|-------------------------------|
| Domain Types | Architecture.md, SprintBook.md, ADRs (if behavioural change) |
| Value Objects | Architecture.md, SprintBook.md, ADRs (if invariants change) |
| Entities | Architecture.md, ProductBook.md, SprintBook.md |
| Aggregates | Architecture.md, ProductBook.md, ADRs |
| Domain Services | Architecture.md, ProductBook.md |
| Application Services | ProductBook.md, Architecture.md |
| UI Components | DesignBible.md, SprintBook.md, ProductBook.md (if behaviour changed) |
| Design System tokens | DesignBible.md, MotionGuide.md, SprintBook.md |
| Animations / Motion | MotionGuide.md, DesignBible.md |
| Workflow IA / Agents | AGENTS.md, AIWorkflow.md, AIPlaybook.md |
| Commit / Git workflow changes | GitConvention.md, Docs/ADR (if policy changes) |
| Release process changes | SprintBook.md, README.md, Release notes templates |
| Security / Privacy changes | Architecture.md, DefinitionOfDone.md, ProductBook.md (if user data affected) |


Automatic Rules

These official rules guide Documentation Sync decisions:

- Any modification of the Domain Model requires an Architecture.md review.
- Any durable architectural decision must be evaluated for an ADR and recorded in Docs/ADR when applicable.
- Any visual or UI evolution must be compared to the DesignBible; significant deviations require DesignBible updates.
- Any animation change must be documented in MotionGuide.
- Every completed Story must update SprintBook.md (summary) or produce a Documentation Sync Report explaining omissions.
- Any modification to a governance document (AGENTS.md, DefinitionOfDone.md, DocumentationMap.md, GitConvention.md, AIWorkflow.md or AIPlaybook.md) must trigger a coherence review of the other governance documents to ensure alignment.

Documentation Review Process

How to use this map during a Story:

1. Consult AGENTS.md first to follow orchestration rules.
2. Use the Change Impact Matrix to identify the minimal set of documents to review.
3. Prepare Documentation Sync Report drafts for those documents only.
4. Submit the drafts to Documentation Review as part of the Story handover.

Constraints

- This document is intentionally stable and generic. It does not contain Story‑level content.
- It must not duplicate AGENTS.md, AIWorkflow.md or DefinitionOfDone.md; instead, it references them.
- It does not define policy or technical rules beyond documentation mapping.
