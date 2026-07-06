namespace Finance.Infrastructure.Persistance.Sql;

public static class RecurringTransactionSql
{
    public const string GetRecurringTransactionById = """
                                                      SELECT id                     AS Id,
                                                             account_id             AS AccountId,
                                                             category_id            AS CategoryId,
                                                             amount                 AS Amount,
                                                             type                   AS Type,
                                                             description            AS Description,
                                                             frequency              AS Frequency,
                                                             start_date             AS StartDate,
                                                             end_date               AS EndDate,
                                                             next_occurrence        AS NextOccurrence,
                                                             is_active              AS IsActive,
                                                             created_at             AS CreatedAt
                                                      FROM finance.recurring_transactions
                                                      WHERE id = @Id AND user_id = @UserId
                                                      """;

    public const string GetRecurringTransactionDetails = """
                                                         SELECT id                        AS Id,
                                                                account_id                AS AccountId,
                                                                category_id               AS CategoryId,
                                                                amount                    AS Amount,
                                                                type                      AS Type,
                                                                description               AS Description,
                                                                frequency                 AS Frequency,
                                                                start_date                AS StartDate,
                                                                end_date                  AS EndDate,
                                                                next_occurrence           AS NextOccurrence,
                                                                is_active                 AS IsActive,
                                                                created_at                AS CreatedAt
                                                         FROM finance.recurring_transactions
                                                         WHERE id = @Id AND user_id = @UserId
                                                         """;

    public const string GetRecurringTransactionsByUserId = """
                                                           SELECT id                        AS Id,
                                                                  account_id                AS AccountId,
                                                                  category_id               AS CategoryId,
                                                                  amount                    AS Amount,
                                                                  type                      AS Type,
                                                                  description               AS Description,
                                                                  frequency                 AS Frequency,
                                                                  next_occurrence           AS NextOccurrence,
                                                                  end_date                  AS EndDate,
                                                                  is_active                 AS IsActive
                                                           FROM finance.recurring_transactions
                                                           WHERE user_id = @UserId
                                                           ORDER BY created_at DESC
                                                           """;


    public const string CreateRecurringTransaction = """
                                                     INSERT INTO finance.recurring_transactions 
                                                         (user_id, 
                                                          account_id, 
                                                          category_id, 
                                                          amount, type, 
                                                          description, 
                                                          frequency, 
                                                          next_occurrence, 
                                                          start_date, 
                                                          end_date, 
                                                          is_active, 
                                                          created_at)
                                                     VALUES (
                                                          (@UserId), 
                                                          (@AccountId), 
                                                          (@CategoryId), 
                                                          (@Amount), 
                                                          (@Type), 
                                                          (@Description), 
                                                          (@Frequency), 
                                                          (@NextOccurrence), 
                                                          (@StartDate), 
                                                          (@EndDate), 
                                                          (@IsActive), 
                                                          (@CreatedAt))
                                                     """;

    public const string UpdateRecurringTransaction = """
                                                     UPDATE finance.recurring_transactions
                                                     SET amount = @Amount,
                                                        type = @Type,
                                                        description = @Description,
                                                        frequency = @Frequency,
                                                        is_active = @IsActive
                                                     WHERE id = @Id AND user_id = @UserId
                                                     """;

    public const string DeleteRecurringTransaction = """
                                                     UPDATE finance.recurring_transactions
                                                     SET is_active = false
                                                     WHERE id = @Id AND user_id = @UserId
                                                     """;
}