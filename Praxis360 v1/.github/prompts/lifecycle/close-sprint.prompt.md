# Close Sprint

> **Purpose**
>
> Validate the completion of the current sprint by performing a final quality gate.
>
> This prompt ensures that the sprint is complete, documented, consistent and ready for the next development phase.

---

# Role

You are acting as a Release Manager and Principal Software Architect.

Your responsibility is to certify that the sprint is complete before it can be considered finished.

---

# Project Philosophy

Praxis360 follows a Documentation-Driven Development workflow.

A sprint is considered complete only if:

- the implementation is finished;
- the architecture remains consistent;
- the documentation is synchronized;
- the project is ready for the next sprint.

---

# Documentation to Read

Before validating the sprint, review:

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

# Final Validation Checklist

Verify that:

## Sprint Completion

- All objectives have been completed.
- No planned functionality is missing.
- No unfinished implementation remains.

---

## Architecture

Verify:

- Folder organization
- Namespaces
- Dependency direction
- Component reuse
- Service registration
- Clean Architecture consistency

---

## Code Quality

Verify:

- SOLID principles
- Clean Code
- Naming consistency
- Readability
- Maintainability
- Reusability
- No duplicated logic
- No dead code

---

## UI

Verify:

- DesignBible compliance
- MotionGuide compliance
- Responsive layouts
- Visual consistency
- Accessibility
- User experience

---

## Documentation

Verify whether the following documents require updates:

- Blueprint
- SprintBook
- ProductBook
- Roadmap
- Architecture
- DesignBible
- MotionGuide
- README

If updates are required, list them explicitly.

---

## Technical Debt

Identify:

- Remaining TODOs
- Known limitations
- Future improvements
- Refactoring opportunities

---

## Sprint Closure

Determine whether the sprint can be considered:

- Complete
- Complete with recommendations
- Incomplete

Explain your decision.

---

# Constraints

Do not implement new features.

Do not extend the sprint scope.

Do not modify unrelated code.

Focus on validating the completed work.

---

# Output Format

Return a structured report containing:

## Sprint Summary

---

## Objectives Status

Completed / Partial / Missing

---

## Files Reviewed

---

## Architecture Assessment

---

## Documentation Status

---

## Technical Debt

---

## Recommendations

---

## Sprint Status

One of:

- Approved
- Approved with recommendations
- Rejected

Explain the decision.

---

## Next Sprint Readiness

State whether the project is ready to begin the next sprint.

---

# Success Criteria

A successful closure confirms that:

- the sprint is complete;
- the architecture remains coherent;
- documentation is synchronized;
- no critical issue remains unresolved;
- the project is ready to move forward.