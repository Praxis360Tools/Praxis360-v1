# 01-start-story

Purpose

This prompt initializes a Story after session context validation. It prepares the work for the Architecture Ready state by analyzing scope, identifying impacted documents and producing an implementation plan.

When to Use

Use this prompt once the session has been initialized (00-start-session) and the session context confirms starting a Story. Trigger at the beginning of a Story lifecycle to reach Architecture Ready.

Primary User

GitHub Copilot Agent, assisted by the Lead Software Architect and the CTO when required.

Workflow Position

Transition point to move a Story into the Architecture Ready state. The prompt prepares artifacts required by Architecture Review.

Documents to Read

Always read these documents in this exact order and consult others only as needed:

1. AGENTS.md
2. Architecture.md
3. SprintBook.md
4. ProductVision.md (if relevant)
5. ProductBook.md (if relevant)
6. DocumentationMap.md (if present)
7. GitConvention.md

Required Inputs

At minimum, the prompt requires:

- Story ID
- Objective
- Acceptance criteria
- Specific constraints
- Business context

Actions Allowed

The prompt may:

- Analyze the Story scope
- Propose an implementation plan (high level)
- Identify files likely to be impacted
- Identify documents likely to require updates
- Flag risks and cross‑cutting concerns
- Recommend a review level (Standard, Targeted, Full)

Actions Forbidden

The prompt must never:

- Write code
- Modify files
- Update documentation
- Create a commit
- Create a Story Completion Report

Expected Output

The prompt must produce a report containing exactly the following sections:

## Story Summary

## Acceptance Criteria

## Domain Impact

## Architecture Impact

## Files Likely to Change

## Documentation Impact

## Risks

## Recommended Review Level

## Implementation Plan
Implementation Plan

1. Analysis

2. Domain Changes

3. Files to Create

4. Files to Modify

5. Documentation Impact

6. Risks

7. Validation

Likely Files

Provide a list of likely files with a confidence level for each entry: High, Medium or Low.

Example:
- Domain/ValueObjects/ContractNumber.cs — High
- Domain/Types/SomeType.cs — Medium
- Docs/Architecture.md — Low

## Story Readiness

Provide only one value: `READY` or `BLOCKED`

Blocking Conditions

If any of the following apply, the prompt must output the status:

BLOCKED — ARCHITECT DECISION REQUIRED

Trigger conditions:

- Ambiguous scope
- Insufficient acceptance criteria
- Contradictory documentation
- Multiple architecture solutions with no clear preference
- Story clearly exceeds its initial validated scope

Next Prompt

If the Story is validated and ready:

- Development by GitHub Copilot begins (development phase)

After development completes:

- 04-finish-story.md

Constraints

- Keep the prompt simple and readable
- Prepare work but do not execute it
- Respect AGENTS.md; in case of contradiction AGENTS.md prevails

Validation

After creating this prompt file, verify that only `.github/prompts/01-start-story.md` was created by this step and that no other prompt was modified. Do not commit or push as part of this step.
