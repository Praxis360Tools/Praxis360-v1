# 00-start-session

Purpose

Define this prompt as the official entry point for any Praxis360 work session. It establishes session context and recommends the next prompt to invoke according to the validated Official Prompt Architecture.

When to Use

Use this prompt at the start of every new session, before beginning any Story, Sprint, Release or Hotfix work.

Primary User

GitHub Copilot Agent or a human contributor who will drive Copilot for the session.

Workflow Position

This is the workflow entry point. The prompt must decide which of the following is the next step:

- `05-start-sprint.md`
- `01-start-story.md`
- resume an already open Story (identify and point to its prompt/state)

Documents to Read

Read these documents in this exact order and consult others only as required by the context:

1. `AGENTS.md`
2. `Docs/AIWorkflow.md`
3. `Docs/SprintBook.md`
4. `Docs/Architecture.md`
5. `Docs/GitConvention.md`

If the requested action contradicts AGENTS.md, the workflow defined in AGENTS.md always prevails.

Required Inputs

- Session objective (short description)
- Story or Sprint identifier provided by the CTO, if applicable
- Any special context or constraints provided by the CTO for this session

This prompt must not require an external ticketing system; identifiers may be informal strings.

Actions Allowed

- Read and summarise the required documents
- Check the local Git status
- Identify the current Sprint (if any)
- Identify the current Story (if any)
- List local modifications and map them to a Story when possible
- Detect obvious blockers and report them
- Recommend exactly one next prompt to invoke
 - Propose a probable Story match when detected but never consider automatic detections as certain; final validation belongs to the CTO

Actions Forbidden

- Modify source code
- Modify documentation
- Create or modify any other prompt
- Commit changes
- Push to remote
- Declare a Story validated or completed
 - Record or log prompts used or automatically persist prompt inputs/outputs

Expected Output

Produce a short report containing exactly the following sections. Show only the structure — do not fill the sections here:

## Session Context

## Current Sprint

## Current Story

## Git Status Summary

## Documents Reviewed

## Detected Risks or Blockers

## Session Decision

## Recommended Next Prompt

## Final Status

Final Status must be one of the following values:

- `SESSION READY`
- `BLOCKED — CONTEXT REQUIRED`
- `BLOCKED — DOCUMENTATION CONFLICT`
- `BLOCKED — MULTIPLE ACTIVE STORIES`

Blocking Conditions

Block the session when any of the following applies:

- `AGENTS.md` is missing
- Required reference documents contradict each other
- Multiple Stories appear to be active and cannot be disambiguated
- The session objective is insufficient to determine the next step
- Local modifications appear to belong to another unidentified Story

Note: do not block solely because local modifications exist and are expected to be included in the current session.

Next Prompt

Recommend exactly one next prompt:

- `05-start-sprint.md` if no relevant Sprint is open
- `01-start-story.md` if a Sprint exists but no Story is active
- the prompt corresponding to the real state of an already active Story
- `10-hotfix.md` only if the CTO explicitly reports an urgent production incident

Validation

After creating this file, verify that only `.github/prompts/00-start-session.md` was added by this step, that existing prompts remain untouched, that AGENTS.md remains the primary orchestration document, and that this prompt's content is coherent with `Docs/AIWorkflow.md` and `Docs/GitConvention.md`.

Do not commit or push as part of this step.
