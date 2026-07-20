# 10-hotfix

Purpose

Describe the emergency hotfix workflow: quick analysis, minimal corrective patch, essential verifications and rollback plan. Prepare minimal documentation for the hotfix but do not deploy without CTO authorization.

When to Use

Use only for urgent production incidents requiring a rapid corrective action approved by the CTO.

Primary User

CTO initiates; GitHub Copilot may prepare a minimal patch and verification steps; Lead Software Architect reviews and approves the fix before commit.

Workflow Position

Emergency workflow parallel to normal Story lifecycle; accelerates validation and deployment but remains subject to AGENTS.md and CTO approval.

Documents to Read

Always in this order:

1. AGENTS.md
2. Incident report and monitoring logs
3. Docs/Architecture.md
4. Docs/GitConvention.md

Required Inputs

- Incident description
- Severity and affected environments
- Minimal patch proposal or mitigation plan

Actions Allowed

- Prepare minimal corrective patch
- Run essential local verifications and smoke tests
- Draft rollback plan and minimal documentation update

Actions Forbidden

- Commit or push without explicit CTO authorization
- Perform extended refactorings or additional features in the hotfix

Expected Output

Produce a Hotfix Package containing:

- Quick analysis and impact
- Minimal patch or mitigation steps
- Essential verification checklist and smoke tests
- Rollback plan
- Minimal documentation note

Blocking Conditions

BLOCKED — HOTFIX when:

- Root cause unclear and immediate fix risk is high
- No CTO authorization is available

Next Prompt

- After preparation and approval: CTO executes commit & push, followed by monitoring and documentation updates via 03-sync-documentation or 06-finish-sprint as appropriate
