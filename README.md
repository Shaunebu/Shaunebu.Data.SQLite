Shaunebu.Data.SQLite 🗄️✨
=========================
![NuGet Version](https://img.shields.io/nuget/v/Shaunebu.Data.SQLite?color=blue&label=NuGet)
![NET Support](https://img.shields.io/badge/.NET%20-%3E%3D8.0-blueviolet) ![NET Support](https://img.shields.io/badge/.NET%20CORE-%3E%3D3.1-blueviolet) ![NET Support](https://img.shields.io/badge/.NET%20MAUI-%3E%3D%208.0-blueviolet) [![Support](https://img.shields.io/badge/support-buy%20me%20a%20coffee-FFDD00)](https://buymeacoffee.com/jcz65te)

**Shaunebu.Data.SQLite** provides a **lightweight, thread-safe SQLite manager** for .NET / .NET MAUI apps.  

It supports:
*   **CRUD operations** (single & batch)
    
*   **Table management** (`EnsureTableExistsAsync`, `ResetTableAsync`)
    
*   **Fluent query building** via `TableContext<T>`

* * *

🚀 Installation
---------------

Install via **NuGet**:

`PM> Install-Package Shaunebu.Data.SQLite`

**NuGet Link:** Shaunebu.Data.SQLite  
Compatible with **.NET 8+ / .NET MAUI 8+** projects.

* * *

⚙ Setup
-------

```csharp
using Shaunebu.Data.SQLite;
using Microsoft.Extensions.Logging;

// Optional: configure logging
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<SQLiteManagerService>();

// Get a database instance (singleton per file)
var db = SQLiteManagerService.GetInstance("mydatabase.db", logger);
```

> Multiple calls to `GetInstance("mydatabase.db")` return the same object (thread-safe singleton).

* * *

🧩 Architecture Overview
------------------------

```sql
+--------------------------+
| SQLiteManagerService     |
|--------------------------|
| - _connection            |
| - _logger                |
|--------------------------|
| + Insert<T>(...)         |
| + Update<T>(...)         |
| + Delete<T>(...)         |
| + EnsureTableExistsAsync |
| + ResetTableAsync        |
| + For<T>()               |
+-----------+--------------+
            |
            v
+--------------------------+
| TableContext<T>          |
|--------------------------|
| - _connection            |
| - _logger                |
|--------------------------|
| + InsertAsync(T entity)  |
| + InsertAsync(List<T>)   |
| + UpdateAsync(...)       |
| + DeleteAsync(...)       |
| + Where(...)             |
| + OrderBy(...)           |
| + OrderByDescending(...) |
| + ToListAsync()          |
| + FirstOrDefaultAsync()  |
+--------------------------+
```

*   `SQLiteManagerService` → main entry point per database file.
    
*   `TableContext<T>` → fluent query and CRUD API per table.
    
*   Both support logging via `Microsoft.Extensions.Logging.ILogger`.
    

* * *

🛠 Table Management
-------------------

| Method | Description |
| --- | --- |
| `EnsureTableExistsAsync<T>()` | Creates table if it does not exist. |
| `ResetTableAsync<T>()` | Drops and recreates the table. Useful for testing or clearing all data. |
| `DeleteAllAsync<T>()` | Deletes all rows from the table. |

**Example:**

```csharp
await db.EnsureTableExistsAsync<User>();
await db.ResetTableAsync<Product>();
await db.For<Product>().DeleteAllAsync();
```

* * *

⚡ CRUD Operations
-----------------

### Insert

```csharp
// Single
await db.Insert(new User { Name = "Alice" });

// Batch
await db.Insert(new List<User>
{
    new User { Name = "Bob" },
    new User { Name = "Charlie" }
});
```

### Update

```csharp
// Single
user.Name = "Alice Updated";
await db.Update(user);

// Batch
foreach (var u in users)
    u.Name += " Batch";
await db.Update(users);
```

### Delete

```csharp
// Single
await db.Delete(user);

// Batch
await db.Delete(usersToDelete);
```

* * *

🔎 Querying & Fluent API
------------------------

`TableContext<T>` exposes a fluent query API:

```csharp
var users = await db.For<User>()
                    .Where(u => u.Name.Contains("Alice"))
                    .OrderBy(u => u.Name)
                    .ToListAsync();

var firstUser = await db.For<User>()
                        .Where(u => u.Name == "Alice")
                        .FirstOrDefaultAsync();

var sortedProducts = await db.For<Product>()
                             .OrderByDescending(p => p.Price)
                             .ToListAsync();
```

**Supported Methods in `TableContext<T>`:**

| Method | Description |
| --- | --- |
| `Where(Expression<Func<T,bool>>)` | Filter results |
| `OrderBy<TKey>(Expression<Func<T,TKey>>)` | Sort ascending |
| `OrderByDescending<TKey>(Expression<Func<T,TKey>>)` | Sort descending |
| `ToListAsync()` | Executes the query and returns a list |
| `FirstOrDefaultAsync()` | Returns the first matching row or null |

* * *

🌐 TableContext Helper
----------------------

You can create a **table-specific context** from the manager:

```csharp
var usersTable = db.For<User>();
await usersTable.InsertAsync(new User { Name = "Diana" });

var allUsers = await usersTable.ToListAsync();
```

* * *

💡 Best Practices
-----------------

1.  Use `GetInstance(dbPath)` for each database; **singleton per file** ensures connection safety.
    
2.  `ResetTableAsync<T>()` is ideal for **testing or clearing tables**.
    
3.  Use `TableContext<T>` for **fluent queries** with LINQ expressions.
    
4.  Always catch exceptions if you want to handle logging yourself; the manager logs errors automatically if a logger is provided.
    

* * *


🔍 Comparison & Advantages
--------------------------

| Feature / Library | Shaunebu.Data.SQLite | SQLite-net pure | Entity Framework Core |
| --- | --- | --- | --- |
| **Fluent Table API** | ✅ `TableContext<T>` with LINQ | ❌ `.Table<T>()` + manual LINQ | ✅ LINQ + DbSet |
| **Batch Insert/Update/Delete** | ✅ Supported with automatic transactions | ❌ Only manual loop | ✅ Supported |
| **Reset / Ensure Table Exists** | ✅ `EnsureTableExistsAsync<T>()` / `ResetTableAsync<T>()` | ⚠️ Manual SQL required | ⚠️ `EnsureCreated()` or migrations |
| **Operation Logging** | ✅ Built-in with `ILogger` | ❌ Must catch exceptions manually | ✅ Supports `ILogger` |
| **Thread-safety** | ✅ Singleton per database file | ⚠️ Depends on implementation | ✅ DbContext per scope |
| **Weight & Complexity** | ✅ Lightweight, no EF Core dependency | ✅ Very lightweight | ❌ Heavy and more complex |
| **Fluent Query Builder** | ✅ `Where`, `OrderBy`, `OrderByDescending` | ⚠️ Manual LINQ | ✅ Full LINQ support |
| **Recommended Use** | Lightweight to medium MAUI/.NET apps | Micro-projects or prototypes | Large apps with complex models and migrations |

* * *

### 💡 Key Advantages of Shaunebu.Data.SQLite :

1.  **Simplicity:** easy to set up and use, perfect for MAUI or .NET projects without EF Core dependency.
    
2.  **Batch-friendly:** insert, update, and delete lists of entities efficiently using transactions.
    
3.  **Built-in Logging:** errors are automatically logged if you provide an `ILogger`.
    
4.  **Fluent API:** query tables using LINQ with clear and expressive methods (`Where`, `OrderBy`, `OrderByDescending`).
    
5.  **Flexibility:** use either the manager (`SQLiteManagerService`) or table-specific contexts (`TableContext<T>`).



🛠 Example: Full Flow
---------------------

```csharp
var userDb = SQLiteManagerService.GetInstance("users.db");
var productDb = SQLiteManagerService.GetInstance("products.db");

await userDb.EnsureTableExistsAsync<User>();
await productDb.EnsureTableExistsAsync<Product>();
await productDb.ResetTableAsync<Product>();

await userDb.Insert(new User { Name = "Alice" });
await productDb.Insert(new Product { Name = "Laptop", Price = 1200 });

var moreUsers = new List<User>
{
    new User { Name = "Bob" },
    new User { Name = "Charlie" }
};
await userDb.Insert(moreUsers);

var users = await userDb.For<User>().ToListAsync();
Console.WriteLine($"Users count: {users.Count}");

var userAlice = await userDb.For<User>()
                            .Where(u => u.Name == "Alice")
                            .FirstOrDefaultAsync();

userAlice!.Name = "Alice Updated";
await userDb.Update(userAlice);

// Delete example
await userDb.Delete(userAlice);

// Query with ordering
var sortedProducts = await productDb.For<Product>()
                                    .OrderByDescending(p => p.Price)
                                    .ToListAsync();

foreach (var p in sortedProducts)
    Console.WriteLine($"{p.Name} - {p.Price}");
```

* * *

🔗 References
-------------

*   [SQLite-net Async Documentation](https://github.com/praeclarum/sqlite-net)
    
*   [.NET MAUI + SQLite Guide](https://learn.microsoft.com/dotnet/maui/data-cloud/sqlite)