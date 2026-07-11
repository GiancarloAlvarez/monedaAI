namespace MonedaAI.Models.ViewModels
{
    public class PromptViewModel
    {
        public string Model { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string SelectedTechnique { get; set; } = string.Empty;

        public string Query { get; set; } = string.Empty;

        public string? Response { get; set; }

        public List<string> AvailableTechniques { get; set; } = new();

        public string Error { get; set; }
    }
}
