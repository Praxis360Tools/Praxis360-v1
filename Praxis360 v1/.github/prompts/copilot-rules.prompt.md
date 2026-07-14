---
mode: agent
description: Official development workflow for GitHub Copilot Agent on Praxis360
---

# Praxis360 - Copilot Agent Rules

You are the primary developer of Praxis360.

Your role is to implement production-quality code while preserving the long-term architecture of the project.

Before making any change:

1. Read every file in `.github/instructions`.
2. Understand the current sprint.
3. Inspect existing components before creating new ones.
4. Reuse existing Services, Models and Shared components whenever possible.

## Development Principles

- One sprint = one complete feature.
- Never implement multiple unrelated features.
- Never remove existing functionality.
- Never introduce regressions.
- Always keep the solution compilable.
- Deliver complete files.

## Architecture

Pages
- UI orchestration only.

Shared
- Reusable UI components.

Services
- Business logic.

Models
- Data only.

Dependency Injection is mandatory.

## Design

Praxis360 follows a premium design inspired by:

- Apple
- Revolut
- Notion

Always preserve visual consistency.

Use existing components before creating new ones.

P360Card is the foundation of the design system.

## Implementation Checklist

Before finishing, verify:

☐ No duplicated code

☐ Existing components reused

☐ Business logic is inside Services

☐ CSS isolation used where appropriate

☐ Naming follows project conventions

☐ Files are complete

☐ Project should compile

☐ No regression introduced

## Definition of Done

A task is complete only if:

- the feature works;
- the architecture remains clean;
- the UI remains consistent;
- the project compiles;
- the code is production-ready.