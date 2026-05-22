namespace Infrastructure.Data.Sql;

public class AccountSql
{
    public const string GetAccountById = """
                                         SELECT id              AS Id,
                                                user_id         AS UserId,
                                                name            AS Name,
                                                type            AS Type,
                                                initial_balance AS InitialBalance,
                                                current_balance AS CurrentBalance,
                                                is_active       AS IsActive,
                                                created_at      AS CreatedAt
                                         FROM finance.accounts
                                         WHERE id = @Id
                                         """;

    public const string GetAccountDetails = """
                                                SELECT id              AS Id,
                                                       name            AS Name,
                                                       type            AS Type,
                                                       current_balance AS CurrentBalance,
                                                       is_active       AS IsActive,
                                                       created_at      AS CreatedAt
                                                FROM finance.accounts
                                                WHERE id = @Id
                                                """;

    public const string GetAccountsByUserId = """
                                              SELECT id              AS Id,
                                                     name            AS Name,
                                                     type            AS Type,
                                                     current_balance AS CurrentBalance,
                                                     is_active       AS IsActive,
                                                     created_at      AS CreatedAt
                                              FROM finance.accounts
                                              WHERE user_id = @UserId
                                              ORDER BY created_at DESC
                                              """;

    public const string CreateAccount = """
                                        INSERT INTO finance.accounts 
                                        (user_id, 
                                        name,
                                        type, 
                                        initial_balance,
                                        current_balance,
                                        is_active, 
                                        created_at)
                                        VALUES (
                                        (@UserId),
                                        (@Name), 
                                        (@Type), 
                                        (@InitialBalance), 
                                        (@CurrentBalance), 
                                        (@IsActive), 
                                        (@CreatedAt))
                                        """;

    public const string UpdateAccount = """
                                        UPDATE finance.accounts
                                        SET type = @Type,
                                            is_active = @IsActive
                                        WHERE id = @Id
                                        """;

    public const string DeleteAccount = """
                                        UPDATE finance.accounts
                                        SET is_active = false
                                        WHERE id = @Id
                                        """;
}