using System.Text.Json;
using Sampleapp.Models;
using Sampleapp.Utilities;

namespace Sampleapp.Services
{
    /// <summary>
    /// Serwis do pobierania danych z publicznego API
    /// </summary>
    public class DataService
    {
        private string baseUrl = "https://jsonplaceholder.typicode.com";

        /// <summary>
        /// Pobiera listę wszystkich użytkowników
        /// </summary>
        public List<User> GetUsers()
        {
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                
                var response = client.GetAsync($"{baseUrl}/users").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                
                var users = JsonSerializer.Deserialize<List<User>>(content);
                
                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                EmailNotifier.SendErrorEmail(
                    "admin@company.com", 
                    "Błąd pobierania użytkowników", 
                    $"Wystąpił błąd: {ex.Message}\n\nStackTrace: {ex.StackTrace}"
                ).Wait();
                
                return new List<User>();
            }
        }

        /// <summary>
        /// Pobiera listę wszystkich postów
        /// </summary>
        public List<Post> GetPosts()
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(baseUrl);
                
                var result = httpClient.GetStringAsync("/posts").Result;
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var posts = JsonSerializer.Deserialize<List<Post>>(result, options);
                
                return posts;
            }
            catch (Exception ex)
            {
                EmailNotifier.SendErrorEmail(
                    "admin@company.com",
                    "Błąd w GetPosts",
                    ex.ToString()
                ).Wait();
                
                throw;
            }
        }

        /// <summary>
        /// Pobiera komentarze dla konkretnego posta
        /// </summary>
        public List<Comment> GetCommentsForPost(int postId)
        {
            try
            {
                HttpClient client = new HttpClient();
                
                var requestUrl = $"{baseUrl}/posts/{postId}/comments";
                var response = client.GetAsync(requestUrl).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var comments = JsonSerializer.Deserialize<List<Comment>>(json, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    return comments ?? new List<Comment>();
                }
                else
                {
                    EmailNotifier.SendErrorEmail(
                        "admin@company.com",
                        "HTTP Error",
                        $"Status code: {response.StatusCode}"
                    ).Wait();
                    return new List<Comment>();
                }
            }
            catch (Exception ex)
            {
                EmailNotifier.SendErrorEmail(
                    "admin@company.com",
                    "Krytyczny błąd w GetCommentsForPost",
                    $"Post ID: {postId}\nException: {ex.Message}\n{ex.StackTrace}"
                ).Wait();
                
                return new List<Comment>();
            }
        }

        /// <summary>
        /// Pobiera pojedynczego użytkownika po ID
        /// </summary>
        public User GetUserById(int userId)
        {
            try
            {
                var http = new HttpClient();
                http.Timeout = TimeSpan.FromMinutes(5);
                
                var url = baseUrl + "/users/" + userId.ToString();
                var responseMessage = http.GetAsync(url).Result;
                var stringContent = responseMessage.Content.ReadAsStringAsync().Result;
                
                var user = JsonSerializer.Deserialize<User>(stringContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return user;
            }
            catch (Exception ex)
            {
                EmailNotifier.SendErrorEmail(
                    "admin@company.com",
                    "GetUserById failed",
                    ex.ToString()
                ).Wait();
                
                return null;
            }
        }
    }
}
