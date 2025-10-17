using Microsoft.Extensions.Logging;
using Shaunebu.Data.SQLite;
using SQLite;

var userDb = SQLiteManagerService.GetInstance("users.db");
var productDb = SQLiteManagerService.GetInstance("products.db");

// =======================
// 1. Ensure / Reset Table
// =======================
await userDb.EnsureTableExistsAsync<User>();
await productDb.EnsureTableExistsAsync<Product>();

await userDb.ResetTableAsync<User>();      // limpia users
await productDb.ResetTableAsync<Product>(); // limpia products

// =======================
// 2. Insert single entity
// =======================
await userDb.Insert(new User { Name = "Alice" });
await productDb.Insert(new Product { Name = "Laptop", Price = 1200 });

// =======================
// 3. Insert batch
// =======================
await userDb.Insert(new List<User>
{
    new User { Name = "Bob" },
    new User { Name = "Charlie" }
});

await productDb.Insert(new List<Product>
{
    new Product { Name = "Phone", Price = 800 },
    new Product { Name = "Tablet", Price = 600 }
});

// =======================
// 4. Query all
// =======================
var users = await SQLiteManagerService.For<User>("users.db").ToListAsync();
var products = await SQLiteManagerService.For<Product>("products.db").ToListAsync();
Console.WriteLine($"Users count: {users.Count}");
Console.WriteLine($"Products count: {products.Count}");

// =======================
// 5. Query filtered
// =======================
var alice = await SQLiteManagerService.For<User>("users.db")
    .Where(u => u.Name == "Alice")
    .FirstOrDefaultAsync();
Console.WriteLine($"Found: {alice?.Name}");

// =======================
// 6. Update single
// =======================
if (alice != null)
{
    alice.Name = "Alice Updated";
    await userDb.Update(alice);
}

// =======================
// 7. Update batch
// =======================
foreach (var u in users)
    u.Name += " Batch";
await userDb.Update(users);

// =======================
// 8. Delete single
// =======================
if (alice != null)
    await userDb.Delete(alice);

// =======================
// 9. Delete batch
// =======================
var batchUsers = await SQLiteManagerService.For<User>("users.db")
    .Where(u => u.Name.Contains("Batch"))
    .ToListAsync();
await userDb.Delete(batchUsers);

// =======================
// 10. Ordered query
// =======================
var sortedProducts = await SQLiteManagerService.For<Product>("products.db")
    .OrderByDescending(p => p.Price)
    .ToListAsync();

Console.WriteLine("Products sorted by price:");
foreach (var p in sortedProducts)
    Console.WriteLine($"{p.Name} - {p.Price}");

// =======================
// 11. Drop table
// =======================
await userDb.ResetTableAsync<User>();
await productDb.ResetTableAsync<Product>();

Console.WriteLine("Shaunebu.Data.SQLite demo finished");
Console.ReadLine();








public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }

    public User() { }
}

public class Product
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }

    public Product() { }
}
