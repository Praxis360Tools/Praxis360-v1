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
11. Future Evolutions
12. Business Rules
13. References

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

# 11. Future Evolutions

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

# 12. Business Rules

Every feature must:

- answer a real customer need;
- follow ProductVision.md;
- simplify complex information;
- preserve the premium experience;
- reuse existing components whenever possible.

No functionality should expose unnecessary technical complexity.

---

# 13. References

- Blueprint.md
- ProductVision.md
- Architecture.md
- DesignBible.md
- MotionGuide.md
- SprintBook.md