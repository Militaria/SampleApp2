using Sampleapp.Models;
using Sampleapp.Utilities;

namespace Sampleapp.Services
{
    /// <summary>
    /// Serwis do generowania raportów
    /// </summary>
    public class ReportService
    {
        /// <summary>
        /// Generuje raport użytkowników
        /// </summary>
        public void GenerateUserReport(List<User> users)
        {
            try
            {
                Console.WriteLine("\n--- RAPORT UŻYTKOWNIKÓW ---");
                
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}");
                    Console.WriteLine($"Nazwa: {user.Name}");
                    Console.WriteLine($"Email: {user.Email}");
                    Console.WriteLine($"Firma: {user.Company?.Name}");
                    Console.WriteLine("---");
                }
                
                Console.WriteLine($"\nŁącznie użytkowników: {users.Count}");
            }
            catch (Exception ex)
            {
                EmailNotifier.SendErrorEmail(
                    "admin@company.com",
                    "Błąd generowania raportu",
                    $"Nie udało się wygenerować raportu.\n\nSzczegóły:\n{ex.Message}\n\n{ex.StackTrace}"
                ).Wait();
                
                Console.WriteLine("Wystąpił błąd podczas generowania raportu. Administrator został powiadomiony mailowo.");
            }
        }

        /// <summary>
        /// Eksportuje dane do pliku
        /// </summary>
        public void ExportToFile(string filename, string content)
        {
            try
            {
                File.WriteAllText(filename, content);
                Console.WriteLine($"Dane wyeksportowane do: {filename}");
            }
            catch (Exception ex)
            {
                EmailNotifier.SendErrorEmail(
                    "admin@company.com",
                    "Export failed",
                    $"File: {filename}\nError: {ex.ToString()}"
                ).Wait();
            }
        }
    }
}
