# Praxis360 Official Prompt Catalog

Purpose

This document is the official catalog of the Praxis360 prompt templates. It lists the 11 official prompts, their short purpose and the workflow stage where they are used. The file is an index only and does not describe prompt internals.

Relationship with AGENTS.md

- AGENTS.md defines the official workflow and orchestration rules for the Software Factory.
- The prompts in this catalog implement the procedural steps described by AGENTS.md.
- In case of any contradiction, AGENTS.md prevails.

Official Prompt Catalog

| Prompt | Purpose | Workflow Stage |
|---|---|---|
| 00-start-session.md | Initialize a work session, establish context and recommend the next prompt | Session start |
| 01-start-story.md | Prepare a Story for Architecture Ready: analyze scope, impacts and produce an implementation plan | Story initiation |
| 02-review-story.md | Orchestrate consolidation of architecture and code reviews and produce a consolidated decision | Review orchestration |
| 03-sync-documentation.md | Identify and prepare required documentation updates; produce Documentation Sync Report | Documentation sync |
| 04-finish-story.md | Conclude development, verify build/tests and produce the Story Completion Report | End of development |
| 05-start-sprint.md | Officially initialize a Sprint and prepare the sprint backlog | Sprint initiation |
| 06-finish-sprint.md | Close the Sprint, produce Sprint Summary and list carried-over work and technical debt | Sprint closure |
| 07-start-release.md | Prepare a Release Candidate: checklist, release notes draft and validation plan | Release preparation |
| 08-architecture-review.md | Evaluate architecture, DDD conformance and recommend ADRs if needed (architecture only) | Architecture review |
| 09-code-review.md | Evaluate code quality, readability, SOLID/DRY and potential regressions (code only) | Code review |
| 10-hotfix.md | Emergency hotfix workflow: quick analysis, minimal patch, verification and rollback plan | Hotfix / emergency |

Official Workflow

Session
↓
Sprint
↓
Story
↓
Development
↓
Finish Story
↓
Review (Architecture + Code)
↓
Documentation Sync
↓
Definition of Done Review
↓
Ready to Commit
↓
Commit
↓
Push

Prompt Usage Rules

- Use only the official prompts listed in this catalog for the Software Factory workflow.
- A prompt equates to a single responsibility; do not combine responsibilities across prompts.
- Never execute multiple prompts simultaneously for a single Story.
- Respect the transitions and blocking rules defined in AGENTS.md.

Legacy Prompts

The repository may contain older or auxiliary prompt templates under .github/prompts subfolders. These legacy prompts are not part of the official workflow. They are retained for historical reference or potential reuse, but the Software Factory workflow relies exclusively on the 11 prompts documented here.

Constraints

- Keep this catalog simple and user-oriented.
- Do not duplicate AGENTS.md or describe prompt internals here.
- This catalog documents organization and usage, not implementation details.
