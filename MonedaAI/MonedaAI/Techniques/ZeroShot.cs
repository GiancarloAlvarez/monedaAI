

    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.Options;
    using MonedaAI.Configuration;
    using MonedaAI.Techniques;

    namespace PromptingDemo.Techniques;

    public class ZeroShot : IPromptingTechnique
    {
        private readonly IChatClient _client;
        private readonly AssistantSettings _assistant;

        public ZeroShot(
            IChatClient client,
            IOptions<AssistantSettings> assistant)
        {
            _client = client;
            _assistant = assistant.Value;
        }

        public string Name => "Zero-shot";

        public async Task<string> ExecuteAsync(
            string userQuery,
            CancellationToken cancellationToken = default)
        {
            var prompt = $$"""
            Eres un asistente de {{_assistant.Domain}}.

            Responde de forma clara y breve.
            Si el usuario proporciona ingresos o gastos, identifica el balance cuando sea posible.
            No inventes montos que el usuario no haya indicado.
            Si el usuario solicita una tabla, usa una tabla en formato Markdown.

            Consulta:
            {{userQuery}}
            """;

            var response = await _client.GetResponseAsync(
                prompt,
                cancellationToken: cancellationToken);

        return response.Text ?? string.Empty;
    }
    }

