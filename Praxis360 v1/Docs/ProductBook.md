# Praxis360 Product Book

| Property | Value |
|----------|-------|
| Version | V2.0 |
| Status | Active |
| Owner | Praxis360 |
| Last Updated | 2026-07-14 |

## Related Documents

- Blueprint.md
- ProductVision.md
- Architecture.md
- DesignBible.md
- MotionGuide.md
- SprintBook.md

---

# Table of Contents

1. Purpose
2. Product Structure
3. Dashboard
4. My Space
5. Client Overview
6. Scanner
7. Search
8. Notifications
9. Settings
10. Authentication
11. BRIO Import Preview
12. Future Evolutions
13. Business Rules
14. References

---

# 1. Purpose

ProductBook describes the functional behaviour of Praxis360.

It explains what every module does from the customer's perspective.

Business philosophy belongs in ProductVision.md.

Technical implementation belongs in Architecture.md.

---

# 2. Product Structure

Version 1 focuses entirely on Life Insurance.

Core modules:

- Dashboard
- My Space
- Client Overview
- Scanner
- Search
- Notifications
- Settings
- BRIO Import Preview

Future versions will progressively integrate additional domains while preserving the same customer experience.

---

# 3. Dashboard

## Purpose

The Dashboard gives the client an immediate understanding of their situation.

It answers one simple question:

**"Is everything in order?"**

Typical information includes:

- Overall situation
- Important alerts
- Recent activity
- Quick actions
- Advisor recommendations

The Dashboard should never overwhelm the client.

---

# 4. My Space

## Purpose

My Space centralizes every document available to the client.

Capabilities:

- Browse documents
- Search documents
- Filter by category
- View document details
- Mark favourites

Future improvements:

- OCR indexing
- Automatic classification
- Smart document suggestions
- Secure cloud synchronization

---

# 5. Client Overview

## Purpose

Client Overview explains the customer's Life Insurance situation in a simple and understandable way.

The objective is not to display contracts.

The objective is to explain protection.

The information hierarchy always follows ProductVision.md.

Priority information:

- My accumulated savings
- Protection for my loved ones
- Income if I cannot work
- My protected contributions

Technical contract information remains available inside detailed views.

---

# 6. Scanner

## Purpose

Scanner allows the client or advisor to add new documents quickly.

Capabilities:

- Camera capture
- Gallery import
- Document preview
- Future OCR recognition
- Automatic enhancement

The scanning experience should remain simple and intuitive.

---

# 7. Search

## Purpose

Search allows users to instantly find information.

Capabilities:

- Search by title
- Search by category
- Search by keyword
- Future OCR search
- Future semantic search

Search should prioritize relevance over quantity.

---

# 8. Notifications

## Purpose

Notifications inform clients about important events requiring attention.

Examples:

- Missing documents
- Advisor updates
- Contract evolution
- Important reminders
- New available documents

Notifications should always provide value.

---

# 9. Settings

## Purpose

Settings allow clients to personalize their experience.

Examples:

- Profile
- Language
- Notifications
- Appearance
- Security

Configuration should remain minimal.

---

# 10. Authentication

Supported authentication methods include:

- Microsoft Account
- Biometrics
- PIN Code
- Secure Session

Authentication must remain secure while minimizing friction.

---

# 11. BRIO Import Preview

## BRIO Contract Import Preview

**Access**

The advisor can access "Importer BRIO" from the navigation menu.

**File Selection**

The advisor selects a CSV file containing BRIO contract data.

The system validates:
- File format must be .csv
- File size must not exceed 10 MB
- File must not be empty

Invalid files are rejected with a clear message.

**Analysis**

After selecting a valid file, the advisor clicks "Analyser le fichier".

The system reads and analyzes the file without creating or modifying any data.

**Analysis Summary**

The system displays:
- Total lines analyzed
- Number of client candidates identified
- Number of contract candidates identified
- Number of warnings (non-blocking issues)
- Number of blocking errors (prevent import)

**Client and Contract Candidates**

The system groups contract candidates by client.

For each client, the system shows:
- Client name and birth date
- Number of contracts associated with this client

For each contract, the system shows:
- Policy number
- Product type (recognized contract type or "Type non reconnu")
- Product label from source file (when available)
- Insurance company
- Contract status

Unknown products are displayed as "Type non reconnu" with a warning indicator.

**Warnings and Blocking Errors**

The system separates and displays:
- Warnings: issues that do not prevent import (e.g., single policy number occurrence, unknown product codes, duplicate lines)
- Blocking errors: issues that prevent import (e.g., conflicting policy numbers, scientific notation in policy numbers, missing required identity data)

Each issue includes:
- Issue code
- Affected line number(s)
- Clear description
- Affected field when relevant

**Reset**

The advisor can click "Analyser un autre fichier" to clear the results and select a different file.

**Limitations**

This preview interface is read-only.

It does not:
- Create or modify clients
- Create or modify contracts
- Store any data
- Apply contracts to the advisor's client list
- Display financial information
- Connect to "Ma situation"

The preview helps the advisor understand file content and identify potential issues before any actual import.

### BRIO Controlled Application

When the BRIO analysis contains no blocking errors, the advisor may initiate a controlled application workflow.

**Starting the Application**

After a successful analysis:
- The "Démarrer l'application" button becomes available
- The advisor clicks to begin the application workflow

**Step 1: Client Selection**

The advisor selects one BRIO client candidate from the analyzed file:
- Each client appears in a selectable card
- Client first name and last name displayed
- "Confirmer la sélection" enabled after selection

**Step 2: Destination Choice**

The advisor chooses where to apply the selected client's contracts:

*Option 1: Nouveau client*
- The advisor creates a new Praxis360 client
- Language selection: Français, Nederlands, or English
- Language selection is mandatory

*Option 2: Client Praxis360 existant*
- The advisor selects an existing Praxis360 client from the list
- Each existing client displays: FirstName LastName (DateOfBirth)
- The selected client becomes the destination for the contracts

**Step 3: Confirmation**

The confirmation screen displays:
- Selected BRIO client name
- Number of contracts to apply
- Chosen destination:
  • For new client: "Nouveau client : Language"
  • For existing client: "Client existant : Full Name"

The advisor clicks "Appliquer les contrats" to proceed.

**Step 4: Application**

The system applies the contracts in memory:
- Contracts are applied to the chosen destination
- Existing contracts are recognized and not duplicated
- The screen displays "Application en cours..."

**Step 5: Result**

The result screen displays:
- Contextual message:
  • "Nouveau client créé" if a new client was created
  • "Contrats rattachés au client sélectionné" if contracts were applied to an existing client
  • "Aucun nouveau client n'a été créé" if new-client creation failed
  • "L'application a échoué" for other failures
  • "Application terminée" for other outcomes
- Number of contracts created
- Number of contracts already existing
- Number of contracts skipped (if any)
- Number of contracts unresolved (if any)
- Global warnings (if any)
- Global errors (if any)

**Terminer**

The advisor clicks "Terminer" to return to the initial preview screen.

**Immediate Visibility**

Newly created clients are immediately visible in the existing-client selector without requiring an application restart.

**Idempotence**

Applying the same contracts multiple times:
- Does not create duplicate contracts
- Recognizes already-existing contracts
- Reports the number of contracts already existing

**Exception Handling**

If an unexpected error occurs during application:
- The system restores the confirmation screen
- A generic error message is displayed
- No technical details are exposed

---

# 12. Future Evolutions


Praxis360 expands progressively.

Version 1

Life Insurance

↓

Version 2

Property & Casualty Insurance

↓

Version 3

Energy

↓

Version 4

Telecommunications

↓

Version 5

Real Estate

↓

Future

Complete Client Workspace

Each new domain must integrate seamlessly into the existing customer experience.

---

# 13. Business Rules

Every feature must:

- answer a real customer need;
- follow ProductVision.md;
- simplify complex information;
- preserve the premium experience;
- reuse existing components whenever possible.

No functionality should expose unnecessary technical complexity.

---

# 14. References

- Blueprint.md
- ProductVision.md
- Architecture.md
- DesignBible.md
- MotionGuide.md
- SprintBook.md