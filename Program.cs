using Microsoft.EntityFrameworkCore;
using StockTracker.BackgroundServices;
using StockTracker.Components;
using StockTracker.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Add the controllers (this is the missing part)
builder.Services.AddControllers();  // <-- Add this line to register controller services
builder.Services.AddHttpClient();
// Register the background service
builder.Services.AddHostedService<StockCheckerService>();
builder.Services.AddFluentEmail(builder.Configuration);
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddBlazorBootstrap();

//configure Db context
builder.Services.AddDbContext<STdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

//setup API endpoints
app.MapControllers();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
