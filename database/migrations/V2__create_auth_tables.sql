-- =============================================================================
-- PersonalFinance — V2__create_auth_tables.sql
-- Authentication schema tables
-- =============================================================================

CREATE TABLE auth.users
(
    id            UUID PRIMARY KEY      DEFAULT gen_random_uuid(),
    name          VARCHAR(200) NOT NULL,
    email         VARCHAR(100) NOT NULL,
    password_hash VARCHAR      NOT NULL,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at    TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE UNIQUE INDEX idx_users_email
    ON auth.users (email);

-- -------

CREATE TABLE auth.refresh_tokens
(
    id         UUID PRIMARY KEY     DEFAULT gen_random_uuid(),
    user_id    UUID        NOT NULL,
    token_hash VARCHAR     NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    is_revoked BOOLEAN     NOT NULL DEFAULT false,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT fk_refresh_tokens_users
        FOREIGN KEY (user_id)
            REFERENCES auth.users (id)
            ON DELETE CASCADE
);

CREATE UNIQUE INDEX idx_refresh_token_hash
    ON auth.refresh_tokens (token_hash);

CREATE INDEX idx_refresh_tokens_user_id
    ON auth.refresh_tokens (user_id);
