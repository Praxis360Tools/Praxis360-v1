# 03-sync-documentation

Purpose

Identify and prepare necessary documentation updates after a Story has passed review and is ready for documentation synchronization. Produce the Documentation Sync Report with the proposed edits.

When to Use

Invoke after 02-review-story approves proceed to documentation synchronization.

Primary User

GitHub Copilot Agent prepares drafts; Lead Software Architect reviews and validates documentation impact.

Workflow Position

Documentation synchronization step between review completion and Documentation Review.

Documents to Read

Always in this order:

1. AGENTS.md
2. DocumentationMap.md (if present)
3. Docs/SprintBook.md
4. Docs/Architecture.md
5. Docs/GitConvention.md

Required Inputs

- Story ID
- Story Completion Report
- List of files changed
- Any notes on domain or UI changes that affect docs

Actions Allowed

- Identify only the documents that must be updated
- Prepare documentation drafts or diffs for the affected documents
- Produce Documentation Sync Report listing changes and rationale

Actions Forbidden

- Modify documents outside the identified impacted set
- Commit or push documentation changes
- Approve final documentation review

Expected Output

Produce a Documentation Sync Report containing:

- List of documents to change
- Proposed diffs or draft text
- Rationale for each change
- Impact level (Minor / Major)

Blocking Conditions

BLOCKED — DOCUMENTATION CONFLICT when:

- Documentation references contradict each other
- Required documentation is missing or incomplete

Next Prompt

- Documentation Review (human) or return to Development if documentation cannot be prepared
