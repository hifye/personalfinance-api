-- =============================================================================
-- PersonalFinance — V5__create_recurring_transactions_table.sql
-- Finance schema - recurring transactions
-- =============================================================================
-- TransactionType enum (Finance.Domain.Enums.TransactionType):
--   None = 0 | Income = 1 | Expense = 2
--
-- RecurringFrequency enum (Finance.Domain.Enums.RecurringFrequency):
--   None = 0 | Daily = 1 | Weekly = 2 | Monthly = 3 | Yearly = 4
-- =============================================================================

CREATE TABLE finance.recurring_transactions
(
    id              UUID PRIMARY KEY        DEFAULT gen_random_uuid(),
    user_id         UUID           NOT NULL,
    account_id      UUID           NOT NULL,
    category_id     UUID,
    amount          NUMERIC(18, 2) NOT NULL,
    type            SMALLINT       NOT NULL,
    description     VARCHAR(250),
    frequency       SMALLINT       NOT NULL,
    start_date      DATE           NOT NULL,
    end_date        DATE,
    next_occurrence DATE           NOT NULL,
    is_active       BOOLEAN        NOT NULL DEFAULT true,
    created_at      TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated_at      TIMESTAMPTZ,

    CONSTRAINT recurring_transactions_type_check
        CHECK (type IN (1, 2)),

    CONSTRAINT recurring_transactions_frequency_check
        CHECK (frequency IN (1, 2, 3, 4)),

    CONSTRAINT fk_recurring_accounts
        FOREIGN KEY (account_id)
            REFERENCES finance.accounts (id)
            ON DELETE RESTRICT,

    CONSTRAINT fk_recurring_categories
        FOREIGN KEY (category_id)
            REFERENCES catalog.categories (id)
            ON DELETE SET NULL
);
