# ADR-0002 — Value Object Conventions

Status

Accepted

Context

To ensure consistent modelling of small domain concepts and to avoid implicit or scattered validation rules, Praxis360 adopts a set of conventions for Value Objects. These conventions must be stable and documented so that future contributors apply the same principles.

Decision

1. Value Objects shall be immutable and implement equality by value.
2. Value Objects must validate their invariants at creation time and preserve them for the lifetime of the instance.
3. Percentage values are stored internally as a decimal fraction (e.g. 0.05 represents 5%) and must be created via explicit factory methods (FromFraction, FromPercent). V1 restricts values to the inclusive range 0%..100%.
4. Money encapsulates a decimal Amount and a Currency (Domain Type). Arithmetic between Money instances is only allowed when currencies match. No automatic currency conversion is performed by Money.
5. Date ranges used in the domain shall use DateOnly for Start and optional End. When End is present it must not be earlier than Start.

Consequences

- Implementations must place Value Objects under Domain/ValueObjects and avoid dependencies to UI or infrastructure.
- Any behaviour beyond simple, well-justified utilities (such as basic Contains for DateRange) should be placed in Domain Services or Aggregates.
- Decisions about currency conversion, rounding policies or extended percentage semantics must be formalized via ADRs before being implemented.
- The conventions reduce ambiguity when mapping external sources (BRIO) to the Domain Model and centralize validation logic inside the Domain.
