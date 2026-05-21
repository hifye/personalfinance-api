namespace Catalog.Infrastructure.Persistance.Sql;

public class CatalogSql
{
    public const string GetCategoryById = """
                                          SELECT id              AS Id,
                                                 name            AS Name,
                                                 type            AS Type,
                                                 is_active       AS IsActive,
                                                 created_at      AS CreatedAt
                                          FROM catalog.categories
                                          WHERE id = @Id
                                          """;

    public const string GetCategoryDetails = """
                                             SELECT id              AS Id,
                                                    name            AS Name,
                                                    type            AS Type,
                                                    is_active       AS IsActive,
                                                    created_at      AS CreatedAt
                                             FROM catalog.categories
                                             WHERE id = @Id
                                             """;

    public const string GetCategoriesByUserId = """
                                                SELECT id              AS Id,
                                                     name              AS Name,
                                                     type              AS Type,
                                                     is_active         AS IsActive,
                                                     created_at        AS CreatedAt
                                                FROM catalog.categories
                                                WHERE user_id = @UserId
                                                ORDER BY created_at DESC
                                                """;

    public const string CreateCategory = """
                                         INSERT INTO catalog.categories 
                                             (user_id, 
                                              name, 
                                              type, 
                                              is_active, 
                                              created_at)
                                         VALUES (
                                              (@UserId), 
                                              (@Name),
                                              (@Type), 
                                              (@IsActive),
                                              (@CreatedAt))
                                         """;

    public const string UpdateCategory = """
                                         UPDATE catalog.categories
                                         SET name = @Name,
                                            type = @Type,
                                            is_active = @IsActive
                                         WHERE id = @Id
                                         """;

    public const string DeleteCategory = """
                                         UPDATE catalog.categories
                                         SET is_active = false
                                         WHERE id = @Id
                                         """;
}