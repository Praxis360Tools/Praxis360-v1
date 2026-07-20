# 06-finish-sprint

Purpose

Officially close a Sprint by summarizing achievements, reporting carried-over Stories, and identifying technical debt and next priorities.

When to Use

Invoke at the end of a Sprint when the team completes the timebox or the CTO decides to close the Sprint.

Primary User

CTO / Lead Software Architect, with assistance from GitHub Copilot for drafting the summary.

Workflow Position

Sprint closure; follows completion of all Story activities and reviews within the Sprint.

Documents to Read

Always in this order:

1. AGENTS.md
2. Docs/SprintBook.md
3. Story Completion Reports for Sprint Stories
4. Docs/Architecture.md
5. Docs/GitConvention.md

Required Inputs

- Sprint ID
- Story Completion Reports
- Open issues and blockers
- Test and build summaries

Actions Allowed

- Produce Sprint Summary report
- List objectives achieved and Stories completed
- List Stories carried forward and reasons
- Identify technical debt and proposed remediation
- Recommend next priorities

Actions Forbidden

- Modify source code or documentation as part of closure
- Commit or push changes

Expected Output

Produce a Sprint Summary containing:

- Objectives achieved
- Completed Stories
- Carried-over Stories
- Technical debt log
- Recommended next priorities

Blocking Conditions

BLOCKED — SPRINT CLOSURE when:

- Critical blockers remain unresolved that prevent accurate reporting
- Missing Story Completion Reports for key Stories

Next Prompt

- 05-start-sprint.md (for next Sprint) or 07-start-release.md if preparing a release
