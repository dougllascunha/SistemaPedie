using Microsoft.EntityFrameworkCore;
using pimFinal.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PedieDbContext>
    (options =>options.UseSqlServer(
        builder.Configuration.GetConnectionString("PedieConexão")));
//Parte para usar o carrinho 


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();//carrinho

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();//carrinho

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
