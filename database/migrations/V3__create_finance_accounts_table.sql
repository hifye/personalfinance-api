-- =============================================================================
-- PersonalFinance — V3__create_finance_tables.sql
-- Finance schema tables (accounts, transactions, recurring transactions)
-- =============================================================================
-- AccountType enum (Finance.Domain.Enums.AccountType):
--   None = 0 | Checking = 1 | Savings = 2 | Credit = 3
-- =============================================================================

CREATE TABLE finance.accounts
(
    id              UUID PRIMARY KEY        DEFAULT gen_random_uuid(),
    user_id         UUID           NOT NULL,
    name            VARCHAR(50)    NOT NULL,
    type            SMALLINT       NOT NULL,
    initial_balance NUMERIC(18, 2) NOT NULL,
    current_balance NUMERIC(18, 2) NOT NULL,
    is_active       BOOLEAN        NOT NULL DEFAULT true,
    created_at      TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated_at      TIMESTAMPTZ    NOT NULL DEFAULT now(),

    CONSTRAINT accounts_type_check
        CHECK (type IN (1, 2, 3)),

    CONSTRAINT uq_accounts_user_name
        UNIQUE (user_id, name)
);
