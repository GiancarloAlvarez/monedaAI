using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MonedaAI.Configuration;
using MonedaAI.Models.ViewModels;
using MonedaAI.Techniques;

namespace MonedaAI.Controllers;

public class HomeController : Controller
{
    private readonly IEnumerable<IPromptingTechnique> _techniques;
    private readonly OllamaSettings _ollamaSettings;

    public HomeController(
        IEnumerable<IPromptingTechnique> techniques,
        IOptions<OllamaSettings> ollamaSettings)
    {
        _techniques = techniques;
        _ollamaSettings = ollamaSettings.Value;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(CreateModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(
        PromptViewModel model,
        CancellationToken cancellationToken)
    {
        model.Model = _ollamaSettings.Model;
        model.BaseUrl = _ollamaSettings.BaseUrl;
        model.AvailableTechniques = _techniques
            .Select(t => t.Name)
            .ToList();

        if (string.IsNullOrWhiteSpace(model.Query))
        {
            model.Error = "Escribe una consulta financiera.";
            return View(model);
        }

        var selectedTechnique = _techniques.FirstOrDefault(
            t => t.Name == model.SelectedTechnique);

        if (selectedTechnique is null)
        {
            model.Error = "Selecciona una técnica válida.";
            return View(model);
        }

        try
        {
            model.Response = await selectedTechnique.ExecuteAsync(
                model.Query,
                cancellationToken);
        }
        catch
        {
            model.Error =
                "No fue posible conectar con Ollama. " +
                "Verifica que esté instalado y ejecutándose.";
        }

        return View(model);
    }

    private PromptViewModel CreateModel()
    {
        var techniques = _techniques.Select(t => t.Name).ToList();

        return new PromptViewModel
        {
            Model = _ollamaSettings.Model,
            BaseUrl = _ollamaSettings.BaseUrl,
            AvailableTechniques = techniques,
            SelectedTechnique = techniques.FirstOrDefault() ?? string.Empty
        };
    }
}