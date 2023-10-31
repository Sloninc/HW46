using Dapper;
using Npgsql;

namespace HW46
{
    public class Program
    {
        public const string ConnectionString =
            "Server=localhost;port=5432; DataBase=Shop; UserId =postgres;";
        public const string ConnectionString2 =
                    "Server=localhost;port=5432; DataBase=Shop2; UserId =postgres;";

        static void Main()
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

            Console.WriteLine("Задание 4.");
            Console.ReadKey();
            using (var connection = new NpgsqlConnection(ConnectionString2))
            {
                var command = new NpgsqlCommand(string.Empty, connection);
                command.CommandText = @"
                    CREATE TABLE Customers (
                    ID SERIAL PRIMARY KEY,
                    FirstName VARCHAR(50),
                    LastName VARCHAR(50),
                    Age INTEGER);
                    CREATE TABLE Products (
                    ID SERIAL PRIMARY KEY,
                    Name VARCHAR(50),
                    Description VARCHAR(255),
                    StockQuantity INTEGER,
                    Price DECIMAL(10, 2)
                        );
                    CREATE TABLE Orders (
                    ID SERIAL PRIMARY KEY,
                    CustomerID INTEGER REFERENCES Customers(ID),
                    ProductID INTEGER REFERENCES Products(ID),
                    Quantity integer);";
                connection.Open();
                command.ExecuteNonQuery();
            }
            var customersArray = new[] {
                ("Никифоров", "Авраам", 34),
                ("Архипов", "Петр", 56),
                ("Семёнов", "Семёнов", 23),
                ("Лихачёв", "Вальтер", 30),
                ("Агафонов", "Мирослав", 19),
                ("Панов", "Клим", 45),
                ("Мухин", "Адольф", 38),
                ("Беспалов", "Казимир", 36),
                ("Туров", "Богдан", 45),
                ("Богданов", "Василий", 37),
                ("Шестаков", "Даниил", 31),
                ("Сысоев", "Гавриил", 57),
                ("Николаев", "Давид", 34),
                ("Панов", "Богдан", 46),
                ("Кабанов", "Гордей", 38),
                ("Чернов", "Исак", 39),
                ("Корнилов", "Тарас", 40),
                ("Тихонов", "Эдуард", 50),
                ("Белозёров", "Гарри", 31),
                ("Гаврилов", "Петр", 33)
            };
            var newCustomers = CreateCustomers(customersArray);
            var productArray = new[]
            {
                ("Яйца", "Синявинская птицефабрика", 1000, 89.00),
                ("Чеснок", "Китай", 60, 46.00),
                ("Сахар", "Тростниковый Кубинский", 985, 120.00),
                ("Пудра сахарная", "Для десертов", 580, 86.00),
                ("Сливки", "Пискаревский завод", 470, 180.00),
                ("Масло растительное", "Дары кубани", 730, 78.00),
                ("Куриное филе", "Синявинская птицефабрика", 800, 230.00),
                ("Приправа", "Адыгейская", 60, 132.00),
                ("Лук репчатый", "Фирма Лето", 790, 34.00),
                ("Майонез", "На перепелиных яйцах", 670, 101.00)
            };
            var newProducts = CreateProducts(productArray);
            var ordersArray = new[]
            {
                (1, 1, 10),
                (2, 10, 1),
                (3, 9, 2),
                (4, 7, 4),
                (5, 8, 1),
                (6, 6, 2),
                (7, 3, 3),
                (8, 2, 4),
                (9, 6, 1),
                (10, 4, 1),
                (11, 4, 2),
                (12, 6, 1),
                (13, 9, 1),
                (14, 5, 2),
                (15, 10, 4),
                (16, 7, 5),
                (17, 3, 2),
                (18, 1, 30),
                (19, 2, 3),
                (20, 8, 2)
            };
            var newOrders = CreateOrders(ordersArray);
            InsertCustomers(newCustomers);
            InsertProducts(newProducts);
            InsertOrders(newOrders);
            using (var connection = new NpgsqlConnection(ConnectionString2))
            {
                var command = new NpgsqlCommand(string.Empty, connection);
                command.CommandText = "CREATE INDEX idx_orders_productid ON Orders(ProductID);";
                connection.Open();
                command.ExecuteNonQuery();
            }

            var result = GetPplOverThirtyBuyProdOne();
            foreach (var pplOverThirtyBuyProdOne in result)
            {

                Console.WriteLine($@"
                    result:
                    {pplOverThirtyBuyProdOne.CustomerId} - customerId
                    {pplOverThirtyBuyProdOne.FirstName} - firstName
                    {pplOverThirtyBuyProdOne.LastName} - lastName
                    {pplOverThirtyBuyProdOne.ProductId} - productId
                    {pplOverThirtyBuyProdOne.Quantity} - quantity
                    {pplOverThirtyBuyProdOne.Price} - price 
                    ");
            }
            Console.ReadLine();
        }

        private static IEnumerable<PplOverThirtyBuyProdOne> GetPplOverThirtyBuyProdOne()
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            const string sql = @"
                SELECT c.ID AS CustomerID, c.FirstName, c.LastName, o.ProductID, o.Quantity AS ProductQuantity, p.Price AS ProductPrice
                FROM Customers c
                JOIN Orders o ON c.ID = o.CustomerID
                JOIN Products p ON o.ProductID = p.ID
                WHERE c.Age > 30 AND o.ProductID = 1";

            return connection.Query<PplOverThirtyBuyProdOne>(sql);
        }

        private static void InsertCustomers(IEnumerable<Customer> newCustomers)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @"insert into Customers (FirstName, LastName, Age) values (@FirstName, @LastName, @Age);";
            foreach (var customer in newCustomers)
            {
                connection.Execute(commandText, new { customer.FirstName, customer.LastName, customer.Age });
            }
        }
        private static void InsertProducts(IEnumerable<Product> newProducts)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @"insert into Products (Name, Description, StockQuantity, Price) values (@Name, @Description, @StockQuantity, @Price);";
            foreach (var product in newProducts)
            {
                connection.Execute(commandText, new { product.Name, product.Description, product.StockQuantity, product.Price });
            }
        }
        private static void InsertOrders(IEnumerable<Order> newOrders)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @"insert into Orders (CustomerId, ProductId, Quantity) values (@CustomerId, @ProductId, @Quantity);";
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


        private static IEnumerable<Customer> GetAllCustomers()
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            const string commandText = @"select Id, Firstname, Lastname, Age from Customers;";
            return connection.Query<Customer>(commandText);
        }

    }
}