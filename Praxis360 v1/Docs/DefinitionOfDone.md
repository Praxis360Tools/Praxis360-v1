# Definition of Done — Praxis360

This document is the single authoritative Definition of Done for Praxis360.

No other project document may redefine the Definition of Done.

Other project documents must reference this document.

Purpose

Provide the canonical Definition of Done (DoD) for Praxis360. This document defines the single source of truth for the quality gates every Story must satisfy before being considered ready for the final commit. Other project documents must reference this DoD rather than redefining it.

Scope

This DoD applies to all development work within the Praxis360 codebase, including Stories, Hotfixes, Releases and Sprint work. It defines mandatory completion criteria and the official checklist used by reviewers and approvers. It does not contain Story‑specific rules or implementation details.

Mandatory Completion Criteria

Every Story must satisfy the following categories before it can progress to Ready for Commit:

- Functional Completion: the implemented functionality meets the acceptance criteria and delivers the expected business value.
- Domain Integrity (DDD): domain model changes preserve invariants, follow DDD principles and do not import external system concepts into the domain.
- Architecture Compliance: changes conform to the architecture principles and do not introduce unacceptable cross‑cutting impacts.
- Code Quality: code is readable, follows project coding standards, applies SOLID and avoids duplication.
- Build Validation: project builds successfully without errors.
- Testing: unit tests (when applicable) are added/updated and pass; integration tests pass where required.
- Documentation: affected documentation is updated (design, architecture, sprint book, user docs) or a Documentation Sync Report is provided explaining omissions.
- UX/UI Consistency: UI changes respect the DesignBible and MotionGuide; user flows remain coherent and accessible.
- Security & Privacy: no secrets, credentials, or personal data are introduced; security implications are assessed and mitigated.
- Git Hygiene: commits are atomic, scoped to a single Story, and follow the project's commit convention; working tree is clean before commit.
- Review Process: required reviews (architecture, code, documentation) have been performed and findings addressed.
- Ready for Commit: final approval by the responsible approver (Lead Software Architect / CTO as defined) is recorded.

Story Status

Official Story states for lifecycle tracking are:

- Draft
- Architecture Ready
- In Development
- Ready for Architecture Review
- Architecture Review
- Code Review
- Ready for Documentation Sync
- Documentation Sync
- Ready for Documentation Review
- Documentation Review
- Definition of Done Review
- Ready to Commit
- Committed
- Pushed
- Completed

Official DoD Checklist

Use this checklist as the minimum gating criteria for each Story. Reviewers must verify each applicable item before marking the Story as meeting the Definition of Done.

- Functional Completion
  - Acceptance criteria satisfied
  - Edge cases accounted for
- Domain Integrity (DDD)
  - Domain invariants preserved
  - No leakage of external system concepts into domain model
- Architecture Compliance
  - Conforms to Architecture.md principles
  - No unjustified cross‑cutting changes
- Code Quality
  - Readable and maintainable code
  - SOLID principles applied where relevant
  - No duplicated logic
- Build Validation
  - Solution builds successfully
  - No compiler warnings introduced (addressed if unavoidable)
- Testing
  - Unit tests added/updated for new behaviour
  - Existing tests pass
  - Integration tests run where applicable
- Documentation
  - Documentation Sync Report prepared when docs are impacted
  - Relevant Docs updated or planned updates documented
- UX/UI Consistency
  - UI matches DesignBible guidelines
  - Interaction flows validated
- Security & Privacy
  - No secrets in code or docs
  - Privacy implications assessed
  - Security review items addressed or logged
- Git Hygiene
  - Commit(s) scope the Story only
  - Commit message follows commit convention
  - Working tree clean before commit
- Review Process
  - Architecture Review performed (if required)
  - Code Review performed
  - Documentation Review performed
  - All review findings resolved or accepted with rationale
- Ready for Commit
  - Final approval recorded by responsible approver

Constraints

This document is intentionally generic and stable. It sets quality expectations without prescribing Story‑level details. It does not duplicate AGENTS.md or GitConvention.md; those documents reference this DoD for gating rules.
