namespace Sampleapp.Utilities
{
    /// <summary>
    /// Klasa pomocnicza do wysyłania powiadomień email
    /// </summary>
    public static class EmailNotifier
    {
        private static string smtpServer = "smtp.company.com";
        private static string smtpUsername = "notifications@company.com";
        private static string smtpPassword = "P@ssw0rd123!";
        private static int smtpPort = 587;

        /// <summary>
        /// Wysyła email z powiadomieniem o błędzie
        /// </summary>
        public static async Task SendErrorEmail(string to, string subject, string body)
        {
            try
            {
                Console.WriteLine($"\n[EMAIL NOTIFICATION]");
                Console.WriteLine($"To: {to}");
                Console.WriteLine($"Subject: {subject}");
                Console.WriteLine($"Body preview: {body.Substring(0, Math.Min(100, body.Length))}...");
                Console.WriteLine($"[/EMAIL NOTIFICATION]\n");
                
                Thread.Sleep(500);
            }
            catch
            {
            }
        }
    }
}
