# ADR-0001 — Domain Type Catalog V1

Status

Accepted

Context

The Domain Model requires a controlled set of enumerations representing fixed business concepts. A canonical source of truth for these domain enumerations is needed to ensure consistency across the codebase and prevent accidental coupling with external systems such as BRIO.

Decision

The Domain Type Catalog V1 is now the official source for all domain enumerations. The following domain types are included in V1: ContractStatus, ContractType, ContributionFrequency, BeneficiaryType, DocumentCategory, DocumentStatus, Currency, Language and Gender. These types are part of the Praxis360 Domain Model and must be implemented as simple enums without behavior or infrastructure dependencies.

Consequences

- All domain enumerations must reference the catalog as the authoritative source.
- Implementations must avoid adding behavior, validation or external dependencies to these types.
- BRIO remains an independent data source; mapping between BRIO and the domain must be implemented in services or adapters, not in the domain types.
- Future changes to the catalog must follow the ADR process and be recorded as subsequent ADRs.
