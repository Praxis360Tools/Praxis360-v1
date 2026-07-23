# Praxis360 AI Workflow

| Property | Value |
|----------|-------|
| Version | V2.0 |
| Status | Active |
| Owner | Praxis360 |
| Last Updated | 2026-07-14 |

## Related Documents

- AGENTS.md
- AIPlaybook.md
- Blueprint.md
- ProductVision.md
- Architecture.md
- SprintBook.md

---

# Purpose

This document describes the operational workflow every AI assistant must follow when contributing to Praxis360.

Unlike AGENTS.md, which defines behaviour and responsibilities, this document defines the execution process for every development task.

---

# Standard Workflow

Every task must follow this sequence.

```
Understand the request

↓

Read the documentation

↓

Validate ProductVision

↓

Review the current Sprint

↓

Create dedicated branch (story/<number>-<description>)

↓

Design the Domain Model (if required)

↓

Validate the architecture

↓

Search for reusable components

↓

Search for reusable services

↓

Implement

↓

Compile

↓

Run applicable tests

↓

Local validation

↓

Request CTO authorization

↓

Commit on branch (after CTO authorization)

↓

Push branch (after CTO authorization)

↓

Create Draft Pull Request

↓

ChatGPT Architecture Review on GitHub

↓

ChatGPT Code Review on GitHub

↓

Apply corrections on same branch if needed

↓

Update documentation

↓

ChatGPT Documentation Review on GitHub

↓

Definition of Done Review

↓

CTO approves merge

↓

Merge into master

↓

Return to master and synchronize
```

No step should be skipped.

---


# Documentation Review Order

Before implementing any change, review documentation in this order. Always consult AGENTS.md first as the orchestration master, then follow the remaining list:

1. AGENTS.md
2. Docs/README.md
3. Docs/Blueprint.md
4. Docs/ProductVision.md
5. Docs/Architecture.md
6. Docs/ProductBook.md
7. Docs/DesignBible.md
8. Docs/MotionGuide.md
9. Docs/SprintBook.md
10. Docs/Roadmap.md
11. Docs/AIPlaybook.md

---

# Development Checklist

Before creating anything new:

□ Is there an existing component?

□ Is there an existing service?

□ Is there an existing model?

□ Is the feature already planned?

□ Does ProductVision support it?

Only create new elements when reuse is not appropriate.

---

# Coding Workflow

For every implementation:

1. Create or switch to the dedicated branch.
2. Analyse the request.
3. Design the solution.
4. Produce complete files.
5. Verify compilation.
6. Run all applicable and available tests.
7. Verify architecture.
8. Verify ProductVision compliance.
9. Verify UI consistency.
10. Verify git diff and git status.
11. Verify only expected files are staged.
12. Prepare commit message.
13. Request CTO authorization.
14. Execute commit and push after authorization.
15. Create Draft Pull Request.
16. Wait for ChatGPT reviews on GitHub.
17. Apply corrections if requested.
18. Update documentation after code stabilization.
19. Wait for final CTO merge authorization.

---


# Documentation Workflow

Documentation must always evolve with the code.

When preparing documentation updates, use Docs/DocumentationMap.md to determine the minimal set of documents to review and update for the change. Whenever changes occur:

- ProductVision → business changes
- Blueprint → development rules
- Architecture → structural changes
- ProductBook → functional behaviour
- DesignBible → UI changes
- MotionGuide → interactions
- SprintBook → Sprint completion
- Roadmap → long-term planning

---

# Review Checklist

Before publishing the branch:

✓ Project compiles.

✓ All applicable tests pass (absence documented if none apply).

✓ Architecture respected.

✓ ProductVision respected.

✓ No duplicated code.

✓ Existing components reused.

✓ Naming conventions respected.

✓ Feature belongs to the current Sprint.

✓ Only expected files staged.

✓ git diff verified.

✓ git diff --check passes.

Before merge into master:

✓ ChatGPT Architecture Review approved.

✓ ChatGPT Code Review approved.

✓ All corrections applied.

✓ Documentation synchronized.

✓ ChatGPT Documentation Review approved.

✓ Definition of Done satisfied.

✓ CTO final authorization obtained.

---

# Sprint Rule

Never implement functionality planned for a future Sprint.

Always complete the current Sprint before moving to the next one.

---

# Goal

Every AI contribution should improve:

- customer experience;
- code quality;
- architectural consistency;
- documentation quality;
- long-term maintainability.

The objective is not to generate code faster.

The objective is to continuously improve Praxis360.