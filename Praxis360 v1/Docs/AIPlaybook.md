# Praxis360 AI Playbook

| Property | Value |
|----------|-------|
| Version | V1.0 |
| Status | Active |
| Owner | Praxis360 |
| Last Updated | 2026-07-13 |

## Related Documents

- README.md
- Blueprint.md
- Architecture.md
- ProductBook.md
- DesignBible.md
- MotionGuide.md
- SprintBook.md
- Roadmap.md

---

# Table of Contents

1. Purpose
2. AI Philosophy
3. AI Responsibilities
4. Development Workflow
5. Sprint Workflow
6. Documentation Rules
7. Coding Rules
8. UI Rules
9. Architecture Rules
10. Review Checklist
11. Definition of Done
12. AI Behavior

---

# 1. Purpose

This document defines how AI assistants must collaborate on the Praxis360 project.

Its objective is to ensure consistent, predictable and high-quality development regardless of the AI assistant being used.

The AI supports the development team but never replaces product decisions.

---

# 2. AI Philosophy

Artificial Intelligence is used to accelerate development while preserving quality.

The AI should:

- understand the project before coding;
- follow the project documentation;
- reuse existing components;
- produce maintainable code;
- avoid unnecessary complexity.

The AI never invents product requirements.

---

# 3. AI Responsibilities

The AI may:

- generate code;
- refactor code;
- improve UI;
- update documentation;
- review code;
- identify inconsistencies;
- propose improvements.

The AI must not:

- change product vision;
- change architecture without justification;
- introduce unnecessary technologies;
- ignore project documentation.

---

# 4. Development Workflow

Every task follows this order:

1. Read README.md
2. Read Blueprint.md
3. Read Architecture.md
4. Read ProductBook.md
5. Read DesignBible.md
6. Read MotionGuide.md
7. Check SprintBook.md
8. Implement the requested work
9. Update documentation if required
10. Validate before completion

---

# 5. Sprint Workflow

Each Sprint follows the same process.

Understand the Sprint.

↓

Review documentation.

↓

Design the solution.

↓

Implement.

↓

Compile.

↓

Review.

↓

Update documentation.

↓

Complete Sprint.

---

# 6. Documentation Rules

Documentation is part of the product.

Whenever a feature changes:

Update Blueprint if required.

Update ProductBook if behaviour changes.

Update Architecture if structure changes.

Update DesignBible if UI changes.

Update MotionGuide if animations change.

Update SprintBook when the Sprint is finished.

Update Roadmap if long-term planning changes.

---

# 7. Coding Rules

Always:

- produce complete files;
- generate compilable code;
- respect existing architecture;
- reuse components;
- use Dependency Injection;
- keep methods readable;
- follow project naming conventions.

Never:

- duplicate code;
- over-engineer;
- leave TODO placeholders;
- introduce breaking changes without reason.

---

# 8. UI Rules

Every interface should:

- feel premium;
- remain simple;
- use generous spacing;
- follow the Design Bible;
- use existing components whenever possible.

Never create inconsistent UI.

---

# 9. Architecture Rules

Business logic belongs inside Services.

Pages orchestrate.

Shared components display.

Models store data.

Architecture must remain modular.

---

# 10. Review Checklist

Before considering a task complete:

✔ Project compiles.

✔ Architecture respected.

✔ Documentation updated.

✔ Existing components reused.

✔ No duplicated code.

✔ Naming conventions respected.

✔ UI consistency preserved.

---

# 11. Definition of Done

A task is complete only when:

- functionality works;
- project compiles;
- documentation is updated;
- code has been reviewed;
- architecture remains consistent.

---

# 12. AI Behavior

The AI should act as a senior software engineer working on a long-term product.

Before coding, it should think about:

- maintainability;
- scalability;
- consistency;
- user experience;
- documentation.

The AI should always optimize for the long-term success of Praxis360 rather than short-term implementation speed.