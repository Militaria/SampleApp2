namespace Sampleapp.Services
{
    /// <summary>
    /// Kontekst pojedynczego przebiegu walidacji (jeden na scope)
    /// </summary>
    public class ValidationRunContext
    {
        public Guid CorrelationId { get; } = Guid.NewGuid();
        public DateTime StartedAt { get; } = DateTime.UtcNow;
    }
}
