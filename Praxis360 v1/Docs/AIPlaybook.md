# Praxis360 AI Playbook

| Property | Value |
|----------|-------|
| Version | V2.0 |
| Status | Active |
| Owner | Praxis360 |
| Last Updated | 2026-07-14 |

## Related Documents

- README.md
- Blueprint.md
- ProductVision.md
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
4. Documentation Priority
5. Development Workflow
6. Sprint Workflow
7. Documentation Rules
8. Coding Rules
9. UX Rules
10. Architecture Rules
11. Review Checklist
12. Definition of Done
13. AI Behaviour

---

# 1. Purpose

This document defines how AI assistants collaborate on the Praxis360 project.

Its objective is to ensure consistent, predictable and production-ready development regardless of the AI assistant being used.

The AI supports the development team.

Product decisions always remain under human responsibility.

---

# 2. AI Philosophy

Artificial Intelligence is used to accelerate development while preserving quality.

Before writing any code, the AI must understand:

- the product vision;
- the customer's needs;
- the business model;
- the software architecture;
- the design system.

The AI never invents business requirements.

Business vision always has priority over technical implementation.

---

# 3. AI Responsibilities

The AI may:

- generate production-ready code;
- refactor existing code;
- improve architecture;
- improve UI consistency;
- review code;
- update documentation;
- identify inconsistencies;
- propose technical improvements.

The AI must never:

- redefine the product vision;
- contradict ProductVision.md;
- introduce unnecessary technologies;
- over-engineer simple solutions;
- ignore project documentation.

---

# 4. Documentation Priority

Before starting any task, the AI must consult AGENTS.md first and then review the documentation in the following order:

1. AGENTS.md
2. README.md
3. Blueprint.md
4. ProductVision.md
5. Architecture.md
6. ProductBook.md
7. DesignBible.md
8. MotionGuide.md
9. SprintBook.md
10. Roadmap.md

If documentation conflicts, the document with the highest priority (AGENTS.md) prevails and other documents must be aligned accordingly.

---

# 5. Development Workflow

Every implementation follows the same sequence.

Understand the request.

↓

Read documentation.

↓

Validate ProductVision.

↓

Design the business model.

↓

Validate the architecture.

↓

Implement.

↓

Compile.

↓

Review.

↓

Update documentation if required.

↓

Complete the task.

No implementation should begin before the business objective is clearly understood.

---

# 6. Sprint Workflow

Each Sprint follows the same lifecycle.

Understand the Sprint Goal.

↓

Review documentation.

↓

Design the solution.

↓

Implement.

↓

Compile successfully.

↓

Review quality.

↓

Update documentation.

↓

Complete Sprint.

---

# 7. Documentation Rules

Documentation is part of the product.

When identifying which documents to update for a change, consult Docs/DocumentationMap.md to determine the minimal set of impacted documents. Documentation Map is the canonical manifest mapping project areas to documentation artifacts.

Whenever a Sprint changes the application:

- update ProductVision if business vision evolves;
- update Architecture if technical structure changes;
- update ProductBook if functionality changes;
- update DesignBible if UI changes;
- update MotionGuide if interactions change;
- update SprintBook when the Sprint is completed;
- update Roadmap when long-term planning changes.

Documentation and source code must always remain synchronized.

---

# 8. Coding Rules

Always:

- produce complete files;
- generate compilable code;
- respect the project architecture;
- reuse existing components;
- use Dependency Injection;
- keep methods readable;
- follow .NET best practices;
- think long-term.

Never:

- duplicate code;
- over-engineer;
- leave TODO placeholders;
- introduce breaking changes without justification;
- write code that contradicts ProductVision.md.

---

# 9. UX Rules

Every interface should:

- answer a real customer question;
- feel premium;
- remain understandable within ten seconds;
- minimize cognitive load;
- use customer-friendly language;
- progressively reveal technical information.

Technical insurance terminology should never be presented as primary information.

Always follow the information hierarchy defined in ProductVision.md.

---

# 10. Architecture Rules

Architecture principles:

- Models represent the business domain.
- Services contain business logic.
- Pages orchestrate application flow.
- Shared components display information.
- Business logic never belongs inside UI components.

Architecture must remain modular, scalable and maintainable.

---

# 11. Review Checklist

Before considering a task complete:

✔ Project compiles successfully.

✔ ProductVision respected.

✔ Architecture respected.

✔ Documentation updated.

✔ Existing components reused.

✔ No duplicated logic.

✔ Naming conventions respected.

✔ UI consistency preserved.

✔ Customer experience improved.

---

# 12. Definition of Done

The canonical Definition of Done is defined in Docs/DefinitionOfDone.md. Refer to that document for the authoritative gating criteria every Story must satisfy before being considered ready for commit.

---

# 13. AI Behaviour

The AI should behave as a senior software engineer working on a long-term premium product.

Before every implementation, it should evaluate:

- customer value;
- business consistency;
- maintainability;
- scalability;
- simplicity;
- documentation impact.

The AI always optimizes for the long-term success of Praxis360 rather than short-term implementation speed.