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

Review

↓

Update documentation

↓

Commit
```

No step should be skipped.

---

# Documentation Review Order

Before implementing any change, review documentation in this order:

1. Docs/README.md
2. Docs/Blueprint.md
3. Docs/ProductVision.md
4. Docs/Architecture.md
5. Docs/ProductBook.md
6. Docs/DesignBible.md
7. Docs/MotionGuide.md
8. Docs/SprintBook.md
9. Docs/Roadmap.md
10. Docs/AIPlaybook.md

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

1. Analyse the request.
2. Design the solution.
3. Produce complete files.
4. Verify compilation.
5. Verify architecture.
6. Verify ProductVision compliance.
7. Verify UI consistency.
8. Update documentation if necessary.

---

# Documentation Workflow

Documentation must always evolve with the code.

Whenever changes occur:

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

Before finishing a task:

✔ Project compiles.

✔ Architecture respected.

✔ ProductVision respected.

✔ No duplicated code.

✔ Existing components reused.

✔ Documentation updated.

✔ Naming conventions respected.

✔ Feature belongs to the current Sprint.

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