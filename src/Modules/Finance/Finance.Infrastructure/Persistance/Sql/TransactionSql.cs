namespace Finance.Infrastructure.Persistance.Sql;

public static class TransactionSql
{
    public const string GetTransactionById = """
                                             SELECT id                              AS Id,
                                                    account_id                      AS AccountId,
                                                    category_id                     AS CategoryId,
                                                    recurring_transaction_id        AS RecurringId,
                                                    amount                          AS Amount,
                                                    type                            AS Type,
                                                    description                     AS Description,
                                                    transaction_date                AS TransactionDate,
                                                    created_at                      AS CreatedAt,
                                                    updated_at                      AS UpdatedAt
                                             FROM finance.transactions
                                             WHERE id = @Id AND user_id = @UserId
                                             """;

    public const string GetTransactionDetails = """
                                                SELECT id                               AS Id,
                                                       account_id                       AS AccountId,
                                                       category_id                      AS CategoryId,
                                                       recurring_transaction_id         AS RecurringId,
                                                       amount                           AS Amount,
                                                       type                             AS Type,
                                                       description                      AS Description,
                                                       transaction_date                 AS TransactionDate,
                                                       created_at                       AS CreatedAt,
                                                       updated_at                       AS UpdatedAt
                                                FROM finance.transactions
                                                WHERE id = @Id AND user_id = @UserId
                                                """;

    public const string GetTransactionsByUserId = """
                                                  SELECT id                              AS Id,
                                                         account_id                      AS AccountId,
                                                         category_id                     AS CategoryId,
                                                         recurring_transaction_id        AS RecurringId,
                                                         amount                          AS Amount,
                                                         type                            AS Type,
                                                         description                     AS Description,
                                                         transaction_date                AS TransactionDate,
                                                         updated_at                      AS UpdatedAt
                                                  FROM finance.transactions
                                                  WHERE user_id = @UserId
                                                  ORDER BY created_at DESC
                                                  """;

    public const string GetTransactionSummary = """
                                                SELECT 
                                                    COALESCE(SUM(amount) FILTER (WHERE type = @IncomeType), 0)  AS TotalIncome,
                                                    COALESCE(SUM(amount) FILTER (WHERE type = @ExpenseType), 0) AS TotalExpense,
                                                    COALESCE(SUM(amount) FILTER (WHERE type = @IncomeType), 0) - 
                                                    COALESCE(SUM(amount) FILTER (WHERE type = @ExpenseType), 0) AS Balance
                                                FROM finance.transactions
                                                WHERE user_id = @UserId
                                                  AND transaction_date >= @StartDate
                                                  AND transaction_date <= @EndDate
                                                """;

    public const string CreateTransaction = """
                                            INSERT INTO finance.transactions 
                                                (user_id,
                                                 account_id, 
                                                 category_id, 
                                                 recurring_transaction_id, 
                                                 amount, type,
                                                 description, 
                                                 transaction_date, 
                                                 created_at)
                                            VALUES (
                                                 (@UserId),
                                                 (@AccountId),
                                                 (@CategoryId),
                                                 (@RecurringId),
                                                 (@Amount),
                                                 (@Type), 
                                                 (@Description),
                                                 (@TransactionDate),
                                                 (@CreatedAt))
                                            """;

    public const string UpdateTransaction = """
                                            UPDATE finance.transactions
                                            SET type = @Type,
                                                description = @Description,
                                                updated_at = @UpdatedAt
                                            WHERE id = @Id AND user_id = @UserId
                                            """;

    public const string DeleteTransaction = """
                                            DELETE FROM finance.transactions    
                                            WHERE id = @Id AND user_id = @UserId
                                            """;
}