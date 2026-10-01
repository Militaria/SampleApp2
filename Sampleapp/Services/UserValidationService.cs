using Sampleapp.Models;
using Sampleapp.Utilities;

namespace Sampleapp.Services
{
    /// <summary>
    /// Serwis do weryfikacji poprawności danych użytkowników
    /// </summary>
    public class UserValidationService
    {
        private const string BlockedDomainsFile = "blocked-domains.txt";

        private readonly ValidationRunContext _context;
        private readonly ValidationResultStore _store;

        public UserValidationService(ValidationRunContext context, ValidationResultStore store)
        {
            _context = context;
            _store = store;
        }

        /// <summary>
        /// Weryfikuje przekazanych użytkowników
        /// </summary>
        public ValidationSummary ValidateUsers(IEnumerable<User> users)
        {
            if (!users.Any())
            {
                return new ValidationSummary { CorrelationId = _context.CorrelationId };
            }

            string[] blockedDomains;
            try
            {
                blockedDomains = LoadBlockedDomainsAsync(BlockedDomainsFile).Result
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
            catch (Exception ex)
            {
                EmailNotifier.SendErrorEmail(
                    "admin@company.com",
                    "Nie udało się wczytać listy zablokowanych domen",
                    ex.ToString()
                ).Wait();

                blockedDomains = Array.Empty<string>();
            }

            Console.WriteLine($"[{_context.CorrelationId}] Walidacja {users.Count()} użytkowników...");

            Parallel.ForEach(users, user =>
            {
                try
                {
                    ValidateUser(user, blockedDomains);
                }
                catch (UserValidationException ex)
                {
                    NotifyValidationFailed(user, ex.Message);
                }
            });

            return new ValidationSummary
            {
                CorrelationId = _context.CorrelationId,
                Total = users.Count(),
                Invalid = _store.GetErrors(_context.CorrelationId).Count()
            };
        }

        /// <summary>
        /// Weryfikuje pojedynczego użytkownika
        /// </summary>
        public void ValidateUser(User user, string[] blockedDomains)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    throw new UserValidationException("Brak nazwy użytkownika");
                }

                var domain = user.Email.Split('@')[1].ToLowerInvariant();
                if (blockedDomains.Contains(domain))
                {
                    throw new UserValidationException($"Domena {domain} jest zablokowana");
                }

                _store.MarkValidated();
            }
            catch (Exception ex)
            {
                _store.AddError(new ValidationError
                {
                    UserId = user.Id,
                    Message = ex.Message,
                    CorrelationId = _context.CorrelationId
                });

                throw ex;
            }
        }

        /// <summary>
        /// Wczytuje listę zablokowanych domen email
        /// </summary>
        public Task<string> LoadBlockedDomainsAsync(string path)
        {
            using var reader = new StreamReader(path);
            return reader.ReadToEndAsync();
        }

        private async void NotifyValidationFailed(User user, string reason)
        {
            await EmailNotifier.SendErrorEmail(
                "admin@company.com",
                "Walidacja użytkownika nie powiodła się",
                $"User ID: {user.Id}\nPowód: {reason}\nCorrelationId: {_context.CorrelationId}"
            );
        }
    }
}
