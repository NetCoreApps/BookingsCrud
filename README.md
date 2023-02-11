# BookingsCrud

Leverage [AutoQuery CRUD](https://docs.servicestack.net/autoquery-crud) and [Locode](https://docs.servicestack.net/locode/) with [User Admin UI](https://docs.servicestack.net/admin-ui-users) to create a multi-user ASP.NET Core Booking System from scratch within minutes with full Audit History, fine-grained permissions, declarative validation, run adhoc queries & export to Excel by just defining code-first high-performance AutoQuery CRUD Typed APIs - ServiceStack does the rest!

> YouTube: [youtu.be/rSFiikDjGos](https://youtu.be/rSFiikDjGos)

[![Build a Bookings API with a user friendly Admin UI in minutes!](https://img.youtube.com/vi/rSFiikDjGos/0.jpg)](https://www.youtube.com/watch?v=rSFiikDjGos)

## Download & Run

The quickest way to run this Booking CRUD Example is to download & run it:

```bash
x download NetCoreApps/BookingsCrud
cd BookingsCrud\Acme
dotnet run
```

## Creating new Empty Web Project

Install [app dotnet tool](https://docs.servicestack.net/netcore-windows-desktop) and create empty [web](https://github.com/NetCoreApps/BookingsCrud) project:

```bash
$ dotnet tool install -g x
$ x new web Acme
```

## Mix in desired features

You can then easily [layer on additional functionality](https://docs.servicestack.net/mix-tool) to integrate with your preferred technology of your Environment's infrastructure, e.g. In order for this project to be self-hosting it utilizes the embedded SQLite database, which we can configure along with configuration to enable popular Authentication providers and an RDBMS SQLite Auth Repository with:

```bash
$ x mix auth auth-db sqlite
```

But if you wanted to enable [Sign in with Apple](https://docs.servicestack.net/signin-with-apple) and use SQL Server you'll instead run:

```bash
$ x mix auth-ext auth-db sqlserver
```

You can view all DB and Auth options available by searching for available mix gists by tag:

```bash
$ x mix #db
$ x mix #auth
```

### Configure Database

Typically the only configuration that needs updating is your DB connection string in 
[Configure.Db.cs](https://github.com/NetCoreApps/BookingsCrud/blob/main/Acme/Configure.Db.cs), in this case it's changed to use a persistent SQLite DB:

```csharp
services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
    Configuration.GetConnectionString("DefaultConnection") 
        ?? "bookings.sqlite",
    SqliteDialect.Provider));
```

You'll also want to create RDBMS tables for any that doesn't exist:

```csharp
using var db = appHost.Resolve<IDbConnectionFactory>().Open();
db.CreateTableIfNotExists<Booking>();
```

### Create Booking CRUD Services

The beauty of AutoQuery is that we only need to focus on the definition of our Data Models using C# POCOs which
OrmLite will use to create and query the RDBMS tables with whilst AutoQuery generates the Typed API implementations enabling us to build full functional high-performance systems with rich querying capabilities, declarative validation & authorization permissions and rich integrations with the most popular platforms without needing to write any logic.

The `Booking` class defines the Data Model whilst the remaining AutoQuery & CRUD Services define the typed inputs, outputs and behavior of each API available that Queries and Modifies the `Booking` table.

An added utilized feature are the `[AutoApply]` attributes which applies generic behavior to AutoQuery Services.
The `Behavior.Audit*` behaviors below depend on the same property names used in the 
[AuditBase.cs](https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.Interfaces/AuditBase.cs) 
class where:

  - `Behavior.AuditQuery` - adds an [Ensure AutoFilter](https://docs.servicestack.net/autoquery-crud#autofilter) to filter out any deleted records
  - `Behavior.AuditCreate` - populates the `Created*` and `Modified*` properties with the Authenticated user info
  - `Behavior.AuditModify` - populates the `Modified*` properties with the Authenticated user info
  - `Behavior.AuditSoftDelete` - changes the behavior of the default **Real Delete** to a **Soft Delete** by  
   populating the `Deleted*` properties

```csharp
public class Booking : AuditBase
{
    [AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public RoomType RoomType { get; set; }
    public int RoomNumber { get; set; }
    public DateTime BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    public decimal Cost { get; set; }
    public string Notes { get; set; }
    public bool? Cancelled { get; set; }
}

public enum RoomType
{
    Single,
    Double,
    Queen,
    Twin,
    Suite,
}

[AutoApply(Behavior.AuditQuery)]
public class QueryBookings : QueryDb<Booking>
{
    public int[] Ids { get; set; }
}

[ValidateHasRole("Employee")]
[AutoApply(Behavior.AuditCreate)]
public class CreateBooking
    : ICreateDb<Booking>, IReturn<IdResponse>
{
    public string Name { get; set; }
    [ApiAllowableValues(typeof(RoomType))]
    public RoomType RoomType { get; set; }
    [ValidateGreaterThan(0)]
    public int RoomNumber { get; set; }
    public DateTime BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    [ValidateGreaterThan(0)]
    public decimal Cost { get; set; }
    public string Notes { get; set; }
}

[ValidateHasRole("Employee")]
[AutoApply(Behavior.AuditModify)]
public class UpdateBooking
    : IPatchDb<Booking>, IReturn<IdResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
    [ApiAllowableValues(typeof(RoomType))]
    public RoomType? RoomType { get; set; }
    [ValidateGreaterThan(0)]
    public int? RoomNumber { get; set; }
    public DateTime? BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    [ValidateGreaterThan(0)]
    public decimal? Cost { get; set; }
    public bool? Cancelled { get; set; }
    public string Notes { get; set; }
}

[ValidateHasRole("Manager")]
[AutoApply(Behavior.AuditSoftDelete)]
public class DeleteBooking : IDeleteDb<Booking>, IReturnVoid
{
    public int Id { get; set; }
}
```

### Run in ServiceStack Studio

After defining your AutoQuery APIs, start your App then you can use the built-in Locode UI to manage Bookings at:

### [https://localhost:5001/locode/](https://localhost:5001/locode/)

![](https://raw.githubusercontent.com/ServiceStack/docs/master/docs/images/locode/bookings-locode.png)