namespace Auth.Infrastructure.Persistance.Sql;

public static class UserSql
{
    public const string GetUserById = """
                                      SELECT id 			AS Id,
                                      		 name 			AS Name,
                                      		 email 			AS Email,
                                      		 password_hash 	AS PasswordHash,
                                      		 created_at 	AS CreatedAt,
                                      		 updated_at 	AS UpdatedAt
                                      FROM auth.users
                                      WHERE id = @Id
                                      """;

    public const string GetUserByEmail = """
                                         SELECT id AS Id,
                                         	   	name AS Name,
                                         	   	email AS Email,
                                         	   	password_hash AS PasswordHash,
                                         	   	created_at AS CreatedAt,
                                         	   	updated_at AS UpdatedAt
                                         FROM auth.users
                                         WHERE email = @Email
                                         """;

    public const string CreateUser = """
                                     INSERT INTO auth.users(
                                     	name,
                                     	email,
                                     	password_hash,
                                     	created_at,
                                     	updated_at
                                     )
                                     VALUES(
                                            (@Name),
                                            (@Email),
                                            (@PasswordHash),
                                            (@CreatedAt),
                                            (@UpdatedAt))
                                     """;

    public const string UpdateUser = """
                                     UPDATE auth.users
                                     SET password_hash = @PasswordHash,
                                     updated_at = @UpdatedAt
                                     WHERE id = @Id
                                     """;

    public const string DeleteUser = """
                                     DELETE from auth.users
                                     WHERE id = @Id
                                     """;
}