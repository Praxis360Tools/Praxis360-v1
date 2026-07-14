# Create Page

> **Purpose**
>
> Design and implement a new page for Praxis360 while preserving the application's architecture, navigation, user experience and design consistency.
>
> Pages must orchestrate the user interface and delegate business logic to services and reusable components.

---

# Role

You are acting as a Senior Application Architect specialized in:

- .NET 10
- MAUI Blazor
- Razor Pages
- MVVM principles
- Component-Based Architecture
- UX Design

Your responsibility is to create pages that integrate naturally into the Praxis360 application.

---

# Project Context

Praxis360 is a premium .NET MAUI Blazor application.

Pages must remain lightweight.

Business logic belongs in Services.

Reusable UI belongs in Shared Components.

The page coordinates the user experience.

---

# Documentation to Read

Before creating a page, review:

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

# Before Creating a Page

Analyse:

- Does a similar page already exist?
- Can an existing page be extended?
- Which shared components can be reused?
- Which services should provide the data?
- Which models are involved?
- Which navigation changes are required?

---

# Responsibilities

Create a page that:

- has a single functional purpose;
- orchestrates the UI;
- delegates business logic;
- reuses shared components;
- follows navigation conventions;
- respects the DesignBible;
- respects the MotionGuide.

---

# Architectural Rules

The page must:

- remain focused on presentation;
- use dependency injection where appropriate;
- avoid business logic;
- avoid duplicated markup;
- remain easy to maintain;
- remain easy to test.

---

# UI Rules

Verify:

- responsive layout;
- consistent spacing;
- typography;
- color palette;
- icons;
- animations;
- accessibility;
- loading states;
- empty states;
- error states.

---

# Navigation Rules

If required, identify updates for:

- navigation menu;
- routing;
- links;
- page titles;
- breadcrumbs (if applicable).

---

# Deliverables

Provide:

- complete Razor page;
- code-behind if justified;
- CSS if required;
- required service registrations;
- routing updates if necessary;
- explanation of architectural decisions.

---

# Self Review

Verify:

- Architecture consistency
- Component reuse
- Service separation
- Navigation consistency
- UI consistency
- Accessibility
- Maintainability
- Scalability

---

# Success Criteria

A successful page integrates seamlessly into Praxis360, remains lightweight, delegates business logic appropriately, reuses existing components, and delivers a consistent user experience aligned with the project's architecture and design standards.