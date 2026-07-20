# 04-finish-story

Purpose

Conclude the development phase of a Story by preparing and producing the Story Completion Report and verifying build results when code was changed. Prepare inputs for the review phase but do not validate or approve the Story.

When to Use

Use this prompt when local development for a Story is complete and the developer is ready to hand over artifacts for review.

Primary User

GitHub Copilot Agent preparing the handover; the human developer verifies the artifacts.

Workflow Position

End of Development. Produces the Story Completion Report and hands off to the review orchestration (02-review-story).

Documents to Read

Always in this order:

1. AGENTS.md
2. Docs/AIWorkflow.md
3. Docs/SprintBook.md
4. Docs/Architecture.md
5. Docs/GitConvention.md

Required Inputs

- Story ID
- List of files changed
- Build result and test results (if applicable)
- Short implementation notes
- Documentation references for changes

Actions Allowed

- Run local build and tests
- Collect build and test outputs
- Produce Story Completion Report with required sections
- List files created and modified
- Indicate documentation impacted
- Flag risks and open issues for reviewers

Actions Forbidden

- Approve or validate the Story
- Commit or push changes
- Modify source files or documentation
- Create the Architecture Review decision

Expected Output

Produce the Story Completion Report containing at minimum:

- Development: summary of changes
- Build: results
- Tests: results
- Documentation: impacted items
- Files Created/Modified
- DDD Review notes (if applicable)
- Risks and open issues

Blocking Conditions

BLOCKED — DEVELOPMENT INCOMPLETE when:

- Build fails
- Tests fail (if present) and no mitigation provided
- Required implementation artifacts are missing

Next Prompt

- 02-review-story.md
