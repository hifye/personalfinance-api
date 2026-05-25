-- =============================================================================
-- PersonalFinance — V4__create_catalog_tables.sql
-- Catalog schema tables (categories)
-- =============================================================================
-- CatalogType enum (Catalog.Domain.Enums.CatalogType):
--   None = 0 | Income = 1 | Expense = 2
-- =============================================================================

CREATE TABLE catalog.categories
(
    id         UUID PRIMARY KEY      DEFAULT gen_random_uuid(),
    user_id    UUID         NOT NULL,
    name       VARCHAR(100) NOT NULL,
    type       SMALLINT     NOT NULL,
    is_active  BOOLEAN      NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ  NOT NULL DEFAULT now(),

    CONSTRAINT categories_type_check
        CHECK (type IN (1, 2)),

    CONSTRAINT uq_categories_user_name
        UNIQUE (user_id, name)
);
