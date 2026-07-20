# 07-start-release

Purpose

Prepare a Release Candidate by assembling the release checklist, drafting release notes and defining a validation plan. This prompt prepares the release artifacts but does not perform deployment.

When to Use

Invoke when the team and CTO decide to create a release candidate from a set of completed Stories or a Sprint.

Primary User

CTO / Release Manager with assistance from GitHub Copilot for drafting notes and checklist.

Workflow Position

Release preparation; follows Sprint closure or after designated Stories are approved for release.

Documents to Read

Always in this order:

1. AGENTS.md
2. Docs/SprintBook.md
3. Story Completion Reports for included Stories
4. Docs/Architecture.md
5. Docs/GitConvention.md

Required Inputs

- Release ID or name
- List of included Stories or commits
- Target release window and environments
- Rollback constraints and requirements

Actions Allowed

- Draft Release Checklist
- Draft Release Notes
- Define Validation Plan and smoke tests
- Identify rollout constraints and rollback plan

Actions Forbidden

- Perform deployment or push release branches without CTO authorization
- Modify production configuration directly

Expected Output

Produce release artifacts including:

- Release Checklist
- Release Notes Draft
- Validation Plan and smoke tests
- Rollback plan

Blocking Conditions

BLOCKED — RELEASE PREPARATION when:

- Critical unresolved regressions exist
- Release scope is unclear or missing required approvals

Next Prompt

- 06-finish-sprint.md (if release tied to sprint) or engage release process with human approvals
