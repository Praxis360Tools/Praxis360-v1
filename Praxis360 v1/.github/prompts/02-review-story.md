# 02-review-story

Purpose

Orchestrate the global review of a Story by consolidating architecture and code reviews and producing a single decision: return to development or proceed to Documentation Sync.

When to Use

Use this prompt after 04-finish-story has produced the Story Completion Report and artifacts are available for review.

Primary User

Lead Software Architect coordinating reviewers; GitHub Copilot may prepare the consolidation report.

Workflow Position

Review orchestration: aggregates results from 08-architecture-review and 09-code-review and issues a consolidated decision.

Documents to Read

Always in this order:

1. AGENTS.md
2. Docs/AIWorkflow.md
3. Story Completion Report
4. Docs/Architecture.md
5. Docs/GitConvention.md

Required Inputs

- Story ID
- Story Completion Report
- Artifacts and code diffs
- Documentation Sync Report (if available)

Actions Allowed

- Consolidate architecture and code review findings
- Produce a single decision with required actions (approve, conditional approve, or reject)
- Request additional changes and route the Story back to Development

Actions Forbidden

- Merge, commit or push
- Modify source code or documentation
- Override Lead Software Architect decisions

Expected Output

Produce a Review Consolidation Report containing:

- Architecture verdict and required changes
- Code verdict and required changes
- Consolidated decision: Return to Development or Proceed to Documentation Sync
- List of corrective actions and responsible parties

Blocking Conditions

BLOCKED — ARCHITECT DECISION REQUIRED when:

- Architecture review unresolved
- Scope change detected requiring architect decision
- Critical regressions or security issues present

Next Prompt

- If approved or conditional: 03-sync-documentation.md
- If rejected: return to Development (development prompt sequence)
