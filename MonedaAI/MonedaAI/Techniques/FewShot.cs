using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using MonedaAI.Configuration;

namespace MonedaAI.Techniques;

public class FewShot : IPromptingTechnique
{
    private readonly IChatClient _client;
    private readonly AssistantSettings _assistant;

    public FewShot(
        IChatClient client,
        IOptions<AssistantSettings> assistant)
    {
        _client = client;
        _assistant = assistant.Value;
    }

    public string Name => "Few-shot";

    public async Task<string> ExecuteAsync(
        string userQuery,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new(
                ChatRole.System,
                $"Eres un asistente de {_assistant.Domain}. " +
                "Responde en español claro. " +
                "Cuando el usuario indique montos, organízalos en una tabla Markdown."
            ),

            new(
                ChatRole.User,
                "Tengo ingresos de RD$60,000 y gastos de RD$45,000. ¿Cuál es mi balance?"
            ),

            new(
                ChatRole.Assistant,
                """
| Concepto | Monto |
|---|---:|
| Ingresos | RD$60,000 |
| Gastos | RD$45,000 |
| Balance disponible | RD$15,000 |

Tu balance es positivo: te quedan RD$15,000.
"""
),

        new(
            ChatRole.User,
            "Gasté RD$8,000 en comida, RD$5,000 en transporte y RD$7,000 en servicios."
        ),

        new(
            ChatRole.Assistant,
            """
| Categoría | Gasto |
|---|---:|
| Comida | RD$8,000 |
| Servicios | RD$7,000 |
| Transporte | RD$5,000 |
| Total | RD$20,000 |

"""
),

        new(ChatRole.User, userQuery)
    };

        var response = await _client.GetResponseAsync(
            messages,
            cancellationToken: cancellationToken);

        return response.Text ?? string.Empty;
    }
}