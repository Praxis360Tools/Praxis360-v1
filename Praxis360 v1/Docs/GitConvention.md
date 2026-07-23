# Git Convention — Praxis360

## Purpose

Une convention Git centralise les bonnes pratiques de gestion de l'historique pour un projet maintenu sur plusieurs années. Elle permet de garantir traçabilité, lisibilité et responsabilité des changements, de faciliter les revues et d'éviter les ruptures dans la branche principale.

Refer to AGENTS.md for the official orchestration rules and to Docs/DefinitionOfDone.md for the canonical Definition of Done. Do not redefine those documents here; reference them when applying Git conventions.

## Principles

- Un commit = une Story
- Un commit doit être atomique
- Chaque commit de branche doit compiler et passer les tests applicables
- La fusion dans master exige les reviews complètes et la Definition of Done
- Le CTO autorise le commit de branche et la fusion finale
- GitHub Copilot exécute les commandes Git après autorisation explicite du CTO

## Branch Strategy

Current Strategy (V2)

- Every Story must be developed on a dedicated feature branch.
- Branch naming convention:
  - Stories: `story/<number>-<short-description>`
  - Documentation: `docs/<description>`
  - Maintenance: `chore/<description>`
  - Bug fixes: `fix/<description>`
- A branch must contain only one Story or one atomic modification.
- No development is allowed directly on master.
- Branches are reviewed via Pull Requests before merge.

Previous Strategy (V1 — deprecated):

- Single main development branch.
- Direct commits are authorized only after:
  - Architecture Review
  - Documentation Review
  - Definition of Done
  - CTO approval

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

## Branch Commit Checklist

- Branche dédiée avec nom correct
- Périmètre Story respecté
- Build local réussi (si code modifié)
- Tous tests applicables réussissent
- Absence de test documentée si aucun ne s'applique
- Aucun secret ni donnée personnelle
- Aucun fichier hors périmètre
- Seuls fichiers attendus stagés
- git diff vérifié
- git diff --check OK
- Commit atomique
- Autorisation CTO obtenue
- Commit et push exécutés
- Draft Pull Request créée

## Merge Checklist

- Pull Request à jour
- Architecture Review ChatGPT approuvée
- Code Review ChatGPT approuvée
- Corrections appliquées
- Branche repoussée et re-reviewée si nécessaire
- Documentation synchronisée
- Documentation Review ChatGPT approuvée
- Definition of Done satisfaite
- Build et tests finaux réussis
- Aucun conflit avec master
- Autorisation finale CTO obtenue
- Working tree propre après fusion

## Workflow

Story
↓
Architecture Ready
↓
Create dedicated branch
↓
Development on branch
↓
Local Validation
↓
Story Completion Report
↓
READY TO PUBLISH REVIEW BRANCH
↓
CTO authorizes commit and push
↓
Copilot executes commit on branch
↓
Copilot executes push on branch
↓
Create Draft Pull Request
↓
ChatGPT Architecture Review on GitHub
↓
ChatGPT Code Review on GitHub
↓
Corrections on same branch if needed
↓
Documentation Sync
↓
ChatGPT Documentation Review on GitHub
↓
Definition of Done Review
↓
READY TO MERGE
↓
CTO authorizes merge
↓
Merge into master
↓
Return to master locally
↓
Sync master with origin/master
↓
Working tree propre vérifié

New rule: A commit must never contain work from multiple Stories.
New rule: Development must occur on dedicated branches, never directly on master.
New rule: All work must be reviewed via Pull Request before merge into master.
New rule: CTO authorizes, Copilot executes Git commands.

## Responsibilities

CTO :

- Autorise le développement
- Autorise le commit et le push de la branche dédiée
- Reçoit le verdict final du Lead Software Architect (ChatGPT)
- Autorise exclusivement le merge dans master
- Conserve la décision finale sur le produit et le périmètre

GitHub Copilot :

- Travaille localement sur la branche dédiée
- Compile et exécute les tests disponibles
- Vérifie git diff et git status
- Prépare le message de commit
- Peut committer et pousser la branche dédiée uniquement après autorisation explicite du CTO
- Ne pousse jamais directement sur master
- Ne fusionne jamais une Pull Request
- Signale immédiatement tout changement de périmètre

Lead Software Architect (ChatGPT) :

- Définit ou valide l'architecture
- Consulte directement sur GitHub les commits, fichiers et diffs de la branche poussée
- Réalise Architecture Review, Code Review et Documentation Review
- Peut demander des corrections précises
- Ne déclare la branche prête que lorsque les validations obligatoires sont satisfaites
- Ne fusionne pas dans master sans autorisation explicite du CTO

## Constraints

Ce document reste simple et pragmatique. Pas de GitFlow, pas de rebase avancé, pas de squash imposé, pas de workflows Enterprise non utilisés aujourd'hui.
