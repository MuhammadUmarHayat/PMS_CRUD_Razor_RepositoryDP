using Books_Management_System.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Books_Management_System.Data
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection is missing.");
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = new List<Product>();

            const string sql = """
                SELECT Id, Title, [description], [image],
                       price, quantity, [status]
                FROM dbo.products
                ORDER BY Id DESC;
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command = new SqlCommand(sql, connection);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            const string sql = """
                SELECT Id, Title, [description], [image],
                       price, quantity, [status]
                FROM dbo.products
                WHERE Id = @Id;
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapProduct(reader);
            }

            return null;
        }

        public async Task<int> CreateAsync(Product product)
        {
            const string sql = """
                INSERT INTO dbo.products
                    (Title, [description], [image],
                     price, quantity, [status])
                OUTPUT INSERTED.Id
                VALUES
                    (@Title, @Description, @Image,
                     @Price, @Quantity, @Status);
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command = new SqlCommand(sql, connection);

            AddProductParameters(command, product);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            const string sql = """
                UPDATE dbo.products
                SET Title = @Title,
                    [description] = @Description,
                    [image] = @Image,
                    price = @Price,
                    quantity = @Quantity,
                    [status] = @Status
                WHERE Id = @Id;
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command = new SqlCommand(sql, connection);

            AddProductParameters(command, product);

            command.Parameters.Add("@Id", SqlDbType.Int).Value =
                product.Id;

            await connection.OpenAsync();

            int rows = await command.ExecuteNonQueryAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = """
                DELETE FROM dbo.products
                WHERE Id = @Id;
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            int rows = await command.ExecuteNonQueryAsync();

            return rows > 0;
        }

        private static Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                description = reader.GetString(
                    reader.GetOrdinal("description")),
                image = reader.GetString(reader.GetOrdinal("image")),
                price = reader.GetInt32(reader.GetOrdinal("price")),
                quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                status = reader.GetString(reader.GetOrdinal("status"))
            };
        }

        private static void AddProductParameters(
            SqlCommand command, Product product)
        {
            command.Parameters.Add("@Title", SqlDbType.VarChar, 50)
                .Value = product.Title;

            command.Parameters.Add("@Description", SqlDbType.VarChar, 50)
                .Value = product.description;

            command.Parameters.Add("@Image", SqlDbType.VarChar, 255)
                .Value = product.image;

            command.Parameters.Add("@Price", SqlDbType.Int)
                .Value = product.price;

            command.Parameters.Add("@Quantity", SqlDbType.Int)
                .Value = product.quantity;

            command.Parameters.Add("@Status", SqlDbType.VarChar, 100)
                .Value = product.status;
        }
    }
}
