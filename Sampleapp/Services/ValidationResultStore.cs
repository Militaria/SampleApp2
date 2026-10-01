using Sampleapp.Models;

namespace Sampleapp.Services
{
    /// <summary>
    /// Magazyn wyników walidacji współdzielony przez całą aplikację
    /// </summary>
    public class ValidationResultStore
    {
        private readonly List<ValidationError> _errors = new List<ValidationError>();
        private int _validatedCount;

        public void AddError(ValidationError error)
        {
            _errors.Add(error);
        }

        public void MarkValidated()
        {
            _validatedCount++;
        }

        public int ValidatedCount => _validatedCount;

        public IEnumerable<ValidationError> GetErrors(Guid correlationId)
        {
            return _errors.Where(e => e.CorrelationId == correlationId);
        }
    }
}
