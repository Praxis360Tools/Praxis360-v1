# Git Convention — Praxis360

## Purpose

Une convention Git centralise les bonnes pratiques de gestion de l'historique pour un projet maintenu sur plusieurs années. Elle permet de garantir traçabilité, lisibilité et responsabilité des changements, de faciliter les revues et d'éviter les ruptures dans la branche principale.

Refer to AGENTS.md for the official orchestration rules and to Docs/DefinitionOfDone.md for the canonical Definition of Done. Do not redefine those documents here; reference them when applying Git conventions.

## Principles

- Un commit = une Story
- Un commit doit être atomique
- Chaque commit doit compiler
- Chaque commit doit respecter la Definition of Done
- Aucun commit ne peut être réalisé avant les différentes reviews
- Le CTO reste responsable du commit final

## Branch Strategy

Current Strategy (V1)

- Single main development branch.
- Direct commits are authorized only after:
  - Architecture Review
  - Documentation Review
  - Definition of Done
  - CTO approval

Future Evolution :

La stratégie pourra évoluer vers des feature branches et des Pull Requests formelles lorsque l'organisation l'exigera.

## Commit Convention

Format officiel :

<type>(<scope>): <summary>

Le corps du commit est fortement recommandé pour toute Story non triviale ; il doit détailler le pourquoi, les impacts et la checklist minimale.

## Commit Types

- feat : nouvelle fonctionnalité
- fix : correction de bug
- refactor : refactorisation sans changement fonctionnel
- docs : documentation
- test : tests
- build : changements liés à la build ou CI
- chore : tâches diverses (config, scripts)

## Commit Scopes

Scopes officiels à utiliser : domain, ui, shared, scanner, dashboard, portfolio, documents, architecture, docs, ai, workflow, infra, application, infrastructure, settings

## Commit Examples

Exemple minimal :

feat(domain): implement Value Objects

Exemple complet :

feat(domain): add ContractNumber value object

Added ContractNumber value object to Domain/ValueObjects. Validates non-empty values and trims ends. Ensures equality by value.

Checklist:
- Build OK
- Story validated
- Documentation synchronized

## Commit Checklist

- Build OK
- Story validée
- Documentation synchronisée
- Definition of Done validée
- Git Status propre
- Aucun fichier parasite
- Commit atomique
- Push autorisé

## Workflow

Story
↓
Architecture Ready
↓
Development
↓
Story Completion Report
↓
Architecture Review
↓
Documentation Sync
↓
Documentation Review
↓
Definition of Done
↓
READY TO COMMIT
↓
Commit
↓
Push

New rule: A commit must never contain work from multiple Stories.

## Responsibilities

CTO :

- valide le commit
- exécute le commit
- exécute le push

GitHub Copilot :

- prépare le message de commit

Lead Software Architect :

- valide la Story avant autorisation du commit

## Constraints

Ce document reste simple et pragmatique. Pas de GitFlow, pas de rebase avancé, pas de squash imposé, pas de workflows Enterprise non utilisés aujourd'hui.
