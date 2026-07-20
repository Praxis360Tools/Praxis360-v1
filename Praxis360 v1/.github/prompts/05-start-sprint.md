# 05-start-sprint

Purpose

Officially initialize a new Sprint by registering objectives, prioritizing Stories and preparing the sprint backlog. This prompt prepares artifacts for the team but does not start development work.

When to Use

Invoke at the beginning of a new Sprint planning session or when the CTO authorizes sprint kickoff.

Primary User

CTO / Sprint Owner, assisted by GitHub Copilot for drafting and summarization.

Workflow Position

Sprint initialization; precedes Story creation and 01-start-story invocations.

Documents to Read

Always in this order:

1. AGENTS.md
2. Docs/SprintBook.md
3. Docs/Roadmap.md
4. Docs/ProductVision.md (if relevant)
5. Docs/GitConvention.md

Required Inputs

- Sprint ID or name
- Sprint goals and scope
- List of candidate Stories
- Timebox and resources constraints

Actions Allowed

- Draft the Sprint summary and objectives
- Prioritize Stories and suggest assignments
- Identify dependencies and risks
- Produce a sprint backlog draft for the team

Actions Forbidden

- Start development work
- Modify source code or documentation
- Commit or push changes

Expected Output

Produce a Sprint Initiation summary containing:

- Sprint goals
- Prioritized Stories list
- Sprint backlog draft
- Preliminary risks and dependencies
- Resource notes

Blocking Conditions

BLOCKED — SPRINT INIT REQUIRED when:

- Sprint goals are missing or unclear
- Critical dependencies unresolved
- Resource constraints prevent execution

Next Prompt

- 01-start-story.md for each Story prioritized in the Sprint
