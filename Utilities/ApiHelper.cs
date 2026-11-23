
using System.Net.Http;
using System.Text.Json;

namespace AutomationExercise.Utilities
{
    public class ApiHelper
    {
        private static readonly HttpClient client = new()
        {
            BaseAddress = new Uri("https://automationexercise.com/api/")
        };

        public async Task CreateUser(
            string name,
            string email,
            string password)
        {
            var formData = new Dictionary<string, string>
            {
                { "name", name },
                { "email", email },
                { "password", password },
                { "title", "Mr" },
                { "birth_date", "10" },
                { "birth_month", "5" },
                { "birth_year", "1990" },
                { "firstname", "Test" },
                { "lastname", "User" },
                { "company", "QA Company" },
                { "address1", "Test Address 1" },
                { "address2", "Test Address 2" },
                { "country", "Canada" },
                { "zipcode", "12345" },
                { "state", "Ontario" },
                { "city", "Toronto" },
                { "mobile_number", "1234567890" }
            };


            const int maxRetries = 2;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                using var content = new FormUrlEncodedContent(formData);

                using var response = await client.PostAsync(
                    "createAccount",
                    content
                );

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    if (attempt == maxRetries)
                    {
                        throw new HttpRequestException(
                            "CreateUser failed: HTTP 503 after 3 attempts.");
                    }

                    TestContext.Out.WriteLine(
                        $"CreateUser returned HTTP 503. Retry {attempt + 1}/{maxRetries}");

                    await Task.Delay(3000);
                    continue;
                }

                await ValidateResponse(
                    response,
                    201,
                    "User created!"
                );

                return;
            }
        }

        public async Task DeleteUser(
            string email,
            string password)
        {
            var formData = new Dictionary<string, string>
            {
                { "email", email },
                { "password", password }
            };

            using var content = new FormUrlEncodedContent(formData);

            using var request = new HttpRequestMessage(
                HttpMethod.Delete,
                "deleteAccount"
            )
            {
                Content = content
            };

            using var response = await client.SendAsync(request);

            await ValidateResponse(
                response,
                200,
                "Account deleted!"
            );
        }

        private static async Task ValidateResponse(
            HttpResponseMessage response,
            int expectedCode,
            string expectedMessage)
        {
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(json);

            JsonElement root = document.RootElement;

            int actualCode = root
                .GetProperty("responseCode")
                .GetInt32();

            string? actualMessage = root
                .GetProperty("message")
                .GetString();

            if (actualCode != expectedCode ||
                actualMessage != expectedMessage)
            {
                throw new InvalidOperationException(
                    $"Unexpected API response. " +
                    $"Expected: {expectedCode} - {expectedMessage}. " +
                    $"Actual: {actualCode} - {actualMessage}"
                );
            }
        }
    }
}
