# 08-architecture-review

Purpose

Evaluate architecture concerns only: DDD conformity, invariants, cross‑cutting impacts and the need for ADRs. Do not judge code quality.

When to Use

Invoke during the review phase for a Story when architectural validation is required (via 02-review-story orchestration).

Primary User

Lead Software Architect (decision authority); Copilot may prepare supporting analysis.

Workflow Position

Part of the review phase invoked by 02-review-story; produces an architectural verdict and required changes.

Documents to Read

Always in this order:

1. AGENTS.md
2. Docs/Architecture.md
3. Story Completion Report
4. Relevant ADRs in Docs/ADR
5. Docs/ProductVision.md (if relevant)

Required Inputs

- Story ID
- Story Completion Report
- Design artifacts and domain model changes
- Code diffs (for context only)

Actions Allowed

- Assess DDD conformance and invariants
- Identify cross‑cutting impacts and dependencies
- Recommend ADR creation when decisions are structural
- Specify required architecture changes or acceptance

Actions Forbidden

- Evaluate code quality or enforce coding styles
- Modify code or documentation
- Approve merges or pushes

Expected Output

Produce an Architecture Review Report with:

- Verdict: Accept / Reject / Conditional
- Rationale and evidence
- Required architectural changes
- Recommendation regarding ADR (yes/no)

Blocking Conditions

BLOCKED — ARCHITECT DECISION REQUIRED when:

- Multiple incompatible architecture options exist
- DDD violations are detected that cannot be resolved at review time

Next Prompt

- Return verdict to 02-review-story for consolidation
