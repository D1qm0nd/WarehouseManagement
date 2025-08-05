using Database;
using Microsoft.EntityFrameworkCore;

DbContextOptionsBuilder<WarehouseDbContext> CreateOptions()
{
    var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
    var connectionString = Environment.GetEnvironmentVariable("WAREHOUSE_DB_CONNECTION_STRING");
    optionsBuilder.UseNpgsql(connectionString);
    return optionsBuilder;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var options = CreateOptions();

try
{
    var _ = new WarehouseDbContext(options);
    _.Database.EnsureCreated();
    _.Database.Migrate();
    _.Dispose();
}
catch (Exception ex)
{
}


builder.Services.AddScoped<WarehouseDbContext>(_ => new WarehouseDbContext(options));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();