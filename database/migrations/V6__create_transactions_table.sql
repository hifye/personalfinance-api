-- =============================================================================
-- PersonalFinance — V6__create_transactions_table.sql
-- Finance schema - transactions (main transactional data)
-- =============================================================================
-- TransactionType enum (Finance.Domain.Enums.TransactionType):
--   None = 0 | Income = 1 | Expense = 2
-- =============================================================================

CREATE TABLE finance.transactions
(
    id                       UUID PRIMARY KEY        DEFAULT gen_random_uuid(),
    user_id                  UUID           NOT NULL,
    account_id               UUID           NOT NULL,
    category_id              UUID,
    recurring_transaction_id UUID,
    amount                   NUMERIC(18, 2) NOT NULL,
    type                     SMALLINT       NOT NULL,
    description              VARCHAR(250),
    transaction_date         DATE           NOT NULL,
    created_at               TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated_at               TIMESTAMPTZ,

    CONSTRAINT transactions_type_check
        CHECK (type IN (1, 2)),

    CONSTRAINT fk_transactions_accounts
        FOREIGN KEY (account_id)
            REFERENCES finance.accounts (id)
            ON DELETE RESTRICT,

    CONSTRAINT fk_transactions_categories
        FOREIGN KEY (category_id)
            REFERENCES catalog.categories (id)
            ON DELETE SET NULL,

    CONSTRAINT fk_transactions_recurring
        FOREIGN KEY (recurring_transaction_id)
            REFERENCES finance.recurring_transactions (id)
            ON DELETE SET NULL
);

-- -------

CREATE INDEX idx_transactions_user_id
    ON finance.transactions (user_id);

CREATE INDEX idx_transactions_account_id
    ON finance.transactions (account_id);

CREATE INDEX idx_transactions_category_id
    ON finance.transactions (category_id);

CREATE INDEX idx_transactions_date
    ON finance.transactions (transaction_date);

CREATE INDEX idx_transactions_user_date
    ON finance.transactions (user_id, transaction_date);
