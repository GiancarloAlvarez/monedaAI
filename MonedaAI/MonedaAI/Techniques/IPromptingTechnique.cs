namespace MonedaAI.Techniques
{
    public interface IPromptingTechnique
    {
        string Name { get; }

        Task<string> ExecuteAsync(
            string userQuery,
            CancellationToken cancellationToken = default);
    }
}
