# 09-code-review

Purpose

Assess code quality concerns: readability, SOLID, DRY, potential risks and regressions. Do not evaluate architectural decisions.

When to Use

Run as part of the Story review phase after 04-finish-story and in coordination with 02-review-story orchestration.

Primary User

Lead Software Architect and assigned human reviewers; GitHub Copilot may prepare a pre-review summary.

Workflow Position

Code-focused review during the review phase; outputs are consolidated by 02-review-story.

Documents to Read

Always in this order:

1. AGENTS.md
2. Docs/GitConvention.md
3. Story Completion Report
4. Code diffs and relevant test results
5. Docs/DesignBible.md (if present)

Required Inputs

- Story ID
- Code diff
- Build and test logs
- Style and coding guidelines references

Actions Allowed

- Identify code smells, violations of SOLID/DRY
- Highlight potential regressions and test gaps
- Recommend refactorings or additional tests

Actions Forbidden

- Judge architectural decisions
- Modify code or tests
- Approve merges or perform commits

Expected Output

Produce a Code Review Report containing:

- Code verdict: Accept / Reject / Conditional
- Findings with severity levels
- Suggested fixes or refactorings
- Test coverage or gaps

Blocking Conditions

BLOCKED — CODE ISSUE when:

- Critical tests fail
- Security vulnerabilities detected
- Codebase stability is at risk

Next Prompt

- Return findings to 02-review-story for consolidation; if rejected, route back to Development
