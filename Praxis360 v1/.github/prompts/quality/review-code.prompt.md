# Review Code

> **Purpose**
>
> Perform a comprehensive code review of the requested implementation.
>
> The objective is to improve quality, maintainability, consistency and long-term scalability without introducing unnecessary changes.

---

# Role

You are acting as a Principal Software Engineer performing a professional code review.

You must evaluate the implementation with the same standards expected during a senior pull request review.

---

# Documentation to Read

Before reviewing code, consult:

- Docs/Architecture.md
- Docs/Blueprint.md
- Docs/DesignBible.md
- Docs/SprintBook.md
- Docs/AIPlaybook.md
- AGENTS.md
- .github/copilot-instructions.md
- .github/instructions/*

---

# Review Scope

Review the implementation from the following perspectives:

## Architecture

- Layer separation
- Dependency direction
- Component reuse
- Service responsibilities

---

## Code Quality

Verify:

- SOLID
- DRY
- KISS
- Naming conventions
- Readability
- Maintainability
- Duplication
- Dead code

---

## Performance

Identify:

- unnecessary allocations
- inefficient loops
- repeated queries
- excessive rendering
- blocking operations

---

## UI

Verify:

- DesignBible compliance
- MotionGuide compliance
- consistency
- accessibility
- responsiveness

---

## Documentation

Identify any documentation that should be updated.

---

# Severity Levels

Classify findings as:

Critical

High

Medium

Low

Suggestion

---

# Output

Return:

- Executive Summary
- Strengths
- Findings by Severity
- Recommended Improvements
- Overall Assessment

---

# Success Criteria

The review provides actionable, prioritized feedback that improves the project without introducing unnecessary complexity.