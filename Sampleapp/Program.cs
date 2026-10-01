using Microsoft.Extensions.DependencyInjection;
using Sampleapp.Services;

namespace Sampleapp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Aplikacja do pobierania danych z API ===\n");

            var dataService = new DataService();
            var reportService = new ReportService();


            Console.WriteLine("Pobieranie użytkowników...");
            var users = dataService.GetUsers();
            Console.WriteLine($"Pobrano {users.Count} użytkowników\n");


            Console.WriteLine("Pobieranie postów...");
            var posts = dataService.GetPosts();
            Console.WriteLine($"Pobrano {posts.Count} postów\n");


            if (posts.Any())
            {
                Console.WriteLine($"Pobieranie komentarzy dla posta ID={posts[0].Id}...");
                var comments = dataService.GetCommentsForPost(posts[0].Id);
                Console.WriteLine($"Pobrano {comments.Count} komentarzy\n");
            }

            // Generujemy raport
            Console.WriteLine("Generowanie raportu...");
            reportService.GenerateUserReport(users);

            // Weryfikacja użytkowników
            var services = new ServiceCollection();
            services.AddScoped<ValidationRunContext>();
            services.AddSingleton<ValidationResultStore>();
            services.AddSingleton<UserValidationService>();
            using var provider = services.BuildServiceProvider();

            const int batchSize = 5;
            for (var i = 0; i < users.Count; i += batchSize)
            {
                using var scope = provider.CreateScope();
                var validator = scope.ServiceProvider.GetRequiredService<UserValidationService>();

                var batch = users
                    .Skip(i)
                    .Take(batchSize)
                    .Select(u => dataService.GetUserById(u.Id));

                var summary = validator.ValidateUsers(batch);
                Console.WriteLine($"[{summary.CorrelationId}] Zweryfikowano {summary.Total}, niepoprawnych: {summary.Invalid}\n");
            }

            Console.WriteLine("\nAplikacja zakończona. Naciśnij Enter...");
            Console.ReadLine();
        }
    }
}
