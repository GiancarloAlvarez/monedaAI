using Microsoft.Extensions.AI;
using MonedaAI.Configuration;
using MonedaAI.Techniques;
using OllamaSharp;
using PromptingDemo.Techniques;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<OllamaSettings>(
    builder.Configuration.GetSection("Ollama"));

builder.Services.Configure<AssistantSettings>(
    builder.Configuration.GetSection("Assistant"));

// 2. CORRECCIÓN: Obtener los valores para el IChatClient
// Usamos el operador null-forgiving (!) o una validación
var ollamaSection = builder.Configuration.GetSection("Ollama");
var ollamaSettings = ollamaSection.Get<OllamaSettings>();

if (ollamaSettings == null)
{
    // Esto te ayudará a saber si el problema es que no leyó el archivo
    throw new Exception("La sección 'Ollama' no se encontró en appsettings.json. Revisa el archivo.");
}

// 3. Registro del cliente
builder.Services.AddSingleton<IChatClient>(_ =>
    new OllamaApiClient(
        new Uri(ollamaSettings.BaseUrl),
        ollamaSettings.Model
    )
);

builder.Services.AddScoped<IPromptingTechnique, ZeroShot>();
builder.Services.AddScoped<IPromptingTechnique, FewShot>();
builder.Services.AddScoped<IPromptingTechnique, ChainOfThought>();
builder.Services.AddScoped<IPromptingTechnique, RolePrompting>();



// Add services to the container.
builder.Services.AddControllersWithViews();

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
