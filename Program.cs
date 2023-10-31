

namespace HW46
{
    public class Program
    {
        public const string ConnectionString =
            "Server=localhost;port=5432; DataBase=shop; UserId =postgres; password=AvisaT53@;";

        static void Main(string[] args)
        {
            Console.WriteLine("Введите Id покупателя");
            var customerById = GetCustomerById(Console.ReadLine());
            Console.WriteLine(
                $"Customer by Id : id - {customerById.Id} name - {customerById.FirstName}, lastname - {customerById.LastName}, age - {customerById.Age}");
            var customers = GetAllCustomers();
            foreach (var customer in customers)
            {
                Console.WriteLine($"id - {customer.Id} name - {customer.FirstName}, lastname - {customer.LastName}, age - {customer.Age}");
            }

            Console.WriteLine("Это были задания до 4.");
            Console.ReadKey();
            Console.WriteLine("Приступаем к 4 заданию");
            Console.WriteLine("Создаем копии таблицы(приставка 2)");
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var command = new NpgsqlCommand(string.Empty, connection);
                command.CommandText = @"
CREATE TABLE Customers2 (
ID SERIAL PRIMARY KEY,
FirstName VARCHAR(50),
LastName VARCHAR(50),
Age INTEGER);
CREATE TABLE Products2 (
ID SERIAL PRIMARY KEY,
Name VARCHAR(50),
Description VARCHAR(255),
StockQuantity INTEGER,
Price DECIMAL(10, 2)
);
CREATE TABLE Orders2 (
ID SERIAL PRIMARY KEY,
CustomerID INTEGER REFERENCES Customers2(ID),
ProductID INTEGER REFERENCES Products2(ID),
Quantity integer);";
                connection.Open();
                command.ExecuteNonQuery();
            }
            var customersArray = new[] {
                ("John", "Doe", 35),
                ("Jane", "Smith", 28),
                ("Michael", "Johnson", 42),
                ("Emily", "Davis", 31),
                ("Daniel", "Wilson", 39),
                ("Olivia", "Anderson", 26),
                ("William", "Brown", 34),
                ("Sophia", "Taylor", 37),
                ("James", "Miller", 29),
                ("Ava", "Martinez", 33)
            };
            var newCustomers = CreateCustomers(customersArray);
            var productArray = new[]
            {
                ("Product1", "Description1", 10, 9.99),
                ("Product2", "Description2", 5, 19.99),
                ("Product3", "Description3", 15, 14.99),
                ("Product4", "Description4", 8, 24.99),
                ("Product5", "Description5", 12, 11.99),
                ("Product6", "Description6", 3, 29.99),
                ("Product7", "Description7", 20, 7.99),
                ("Product8", "Description8", 6, 16.99),
                ("Product9", "Description9", 18, 13.99),
                ("Product10", "Description10", 9, 22.99)
            };
            var newProducts = CreateProducts(productArray);
            var ordersArray = new[]
            {
                (1, 1, 2),
                (2, 3, 1),
                (3, 2, 3),
                (4, 5, 2),
                (5, 4, 1),
                (6, 7, 4),
                (7, 10, 2),
                (8, 8, 1),
                (9, 6, 3),
                (10, 9, 2)
            };
            var newOrders = CreateOrders(ordersArray);
            InsertCustomers(newCustomers);
            InsertProducts(newProducts);
            InsertOrders(newOrders);
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var command = new NpgsqlCommand(string.Empty, connection);
                command.CommandText = "CREATE INDEX idx_orders_productid2 ON Orders2(ProductID);";
                connection.Open();
                command.ExecuteNonQuery();
            }

            var result = GetOldManWhoBuyFirstProduct();
            foreach (var oldManWhoBuyFirstProduct in result)
            {

                Console.WriteLine($@"
result:
{oldManWhoBuyFirstProduct.CustomerId} - customerId
{oldManWhoBuyFirstProduct.FirstName} - firstName
{oldManWhoBuyFirstProduct.LastName} - lastName
{oldManWhoBuyFirstProduct.ProductId} - productId
{oldManWhoBuyFirstProduct.Quantity} - quantity
{oldManWhoBuyFirstProduct.Price} - price 
");
            }
        }

        private static IEnumerable<OldManWhoBuyFirstProduct> GetOldManWhoBuyFirstProduct()
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            const string sql = @"
        SELECT c.ID AS CustomerID, c.FirstName, c.LastName, o.ProductID, o.Quantity AS ProductQuantity, p.Price AS ProductPrice
        FROM Customers c
        JOIN Orders o ON c.ID = o.CustomerID
        JOIN Products p ON o.ProductID = p.ID
        WHERE c.Age > 30 AND o.ProductID = 1";

            return connection.Query<OldManWhoBuyFirstProduct>(sql);
        }

        private static void InsertCustomers(IEnumerable<Customer> newCustomers)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @"insert into Customers2 (FirstName, LastName, Age) values (@FirstName, @LastName, @Age);";
            foreach (var customer in newCustomers)
            {
                connection.Execute(commandText, new { customer.FirstName, customer.LastName, customer.Age });
            }
        }
        private static void InsertProducts(IEnumerable<Product> newProducts)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @"insert into Products2 (Name, Description, StockQuantity, Price) values (@Name, @Description, @StockQuantity, @Price);";
            foreach (var product in newProducts)
            {
                connection.Execute(commandText, new { product.Name, product.Description, product.StockQuantity, product.Price });
            }
        }
        private static void InsertOrders(IEnumerable<Order> newOrders)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @"insert into Orders2 (CustomerId, ProductId, Quantity) values (@CustomerId, @ProductId, @Quantity);";
            foreach (var order in newOrders)
            {
                connection.Execute(commandText, new { order.CustomerId, order.ProductId, order.Quantity });
            }
        }
        private static IEnumerable<Order> CreateOrders((int, int, int)[] ordersArray)
        {
            return ordersArray.Select(order => new Order(order)).ToList();
        }

        private static IEnumerable<Product> CreateProducts((string, string, int, double)[] productArray)
        {
            return productArray.Select(product => new Product(product)).ToList();
        }

        private static IEnumerable<Customer> CreateCustomers((string, string, int)[] customersArray)
        {
            return customersArray.Select(customer => new Customer(customer)).ToList();
        }

        private static Customer GetCustomerById(string? customerid)
        {
            if (!int.TryParse(customerid, out var id))
            {
                throw new ArgumentException("Не число", nameof(customerid));
            }
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @$"select id, firstname, lastname, age from customers where id = @id;";
            return connection.QueryFirst<Customer>(commandText, new { id });
        }


        public static IEnumerable<Customer> GetAllCustomers()
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @"select Id, Firstname, Lastname, Age from Customers;";
            return connection.Query<Customer>(commandText);
        }

    }
}