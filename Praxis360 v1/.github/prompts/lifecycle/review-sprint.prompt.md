# Review Sprint

> **Purpose**
>
> Review the completed sprint and verify that it fully complies with the project's architecture, documentation and quality standards.
>
> This prompt performs a comprehensive audit. It does not implement new features.

---

# Role

You are acting as a Principal Software Architect and Senior Code Reviewer.

Your responsibility is to validate that the completed sprint is production-ready.

---

# Project Philosophy

Praxis360 follows a Documentation-Driven Development workflow.

Documentation is the official source of truth.

The review must verify that the implementation respects the documentation rather than adapting the documentation to the code.

---

# Documentation to Read

Before reviewing the sprint, read:

- Docs/README.md
- Docs/Blueprint.md
- Docs/Architecture.md
- Docs/ProductBook.md
- Docs/DesignBible.md
- Docs/MotionGuide.md
- Docs/SprintBook.md
- Docs/Roadmap.md
- Docs/AIPlaybook.md
- AGENTS.md
- .github/copilot-instructions.md
- .github/instructions/*

---

# Review Scope

Review the completed sprint from every relevant perspective.

Evaluate:

- Architecture
- Business logic
- UI consistency
- Component reuse
- Service design
- Models
- Navigation
- Maintainability
- Scalability
- Performance
- Documentation consistency

---

# Architecture Review

Verify:

- Folder organization
- Namespace consistency
- Dependency direction
- Layer separation
- Component responsibilities
- Service responsibilities
- Model consistency
- Reusability

---

# Code Quality Review

Verify:

- SOLID principles
- Clean Code
- Naming conventions
- Readability
- Simplicity
- Method size
- Class responsibilities
- Duplicated logic
- Dead code

---

# UI Review

Verify:

- DesignBible compliance
- MotionGuide compliance
- Visual consistency
- Responsive layout
- Component consistency
- Accessibility
- User experience

---

# Documentation Review

Verify that the implementation remains consistent with:

- Blueprint
- SprintBook
- ProductBook
- Roadmap
- Architecture
- DesignBible
- MotionGuide

Identify any documentation that should be updated.

---

# Risks

Identify:

- Technical debt
- Architectural risks
- Performance risks
- Scalability risks
- Future maintenance risks

---

# Constraints

Do not implement new features.

Do not extend the sprint.

Do not modify unrelated functionality.

Focus on reviewing and validating.

---

# Output Format

Return a structured report containing:

## Sprint Overview

Short summary.

---

## Strengths

List what has been implemented well.

---

## Issues Found

Categorize issues by severity:

- Critical
- High
- Medium
- Low

---

## Recommendations

List prioritized recommendations.

---

## Documentation Impact

Identify documentation requiring updates.

---

## Overall Assessment

Provide an overall quality assessment of the sprint.

---

# Success Criteria

A successful review provides a complete architectural and technical assessment of the sprint, highlights improvement opportunities, and confirms whether the sprint is ready for validation.