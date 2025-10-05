using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace OrderApp.Controllers.ExternalServices
{
    public class FirebaseHelper
    {
        private readonly JObject _serviceAccount;
        private readonly string _projectId;
        private readonly HttpClient _httpClient;

        public FirebaseHelper(string serviceAccountFilePath)
        {
            if (!File.Exists(serviceAccountFilePath))
                throw new FileNotFoundException("Service account file không tồn tại!", serviceAccountFilePath);

            var json = File.ReadAllText(serviceAccountFilePath);
            _serviceAccount = JObject.Parse(json);
            _projectId = _serviceAccount["project_id"]?.ToString() ?? throw new Exception("Service account JSON thiếu project_id!");
            _httpClient = new HttpClient();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            string clientEmail = _serviceAccount["client_email"]?.ToString();
            string privateKey = _serviceAccount["private_key"]?.ToString();

            var initializer = new ServiceAccountCredential.Initializer(clientEmail)
            {
                Scopes = new[] { "https://www.googleapis.com/auth/firebase.messaging" }
            }.FromPrivateKey(privateKey);

            var credential = new ServiceAccountCredential(initializer);

            if (!await credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None))
                throw new Exception("Không thể lấy access token!");

            return credential.Token.AccessToken;
        }

        // 🔹 Gửi notification tới 1 token
        public async Task SendNotificationToTokenAsync(string token, string title, string body, string imageUrl = null, Dictionary<string, string> data = null)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var message = new
                {
                    message = new
                    {
                        token = token,
                        notification = new
                        {
                            title = title,
                            body = body,
                            image = imageUrl
                        },
                        data = data
                    }
                };

                var json = JsonConvert.SerializeObject(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Send to token result: " + result);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error sending FCM message: {result}");
            }
        }

        // 🔹 Subscribe token vào 1 topic
        public async Task SubscribeTokenToTopicAsync(string token, string topic)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = $"https://iid.googleapis.com/v1:batchAdd";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var body = new
                {
                    to = $"/topics/{topic}",
                    registration_tokens = new[] { token }
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Subscribe result: " + result);
            }
        }
        // 🔹 Gửi notification tới 1 topic
        public async Task SendNotificationToTopicAsync(string topic, string title, string body)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var message = new
                {
                    message = new
                    {
                        topic = topic,
                        notification = new
                        {
                            title = title,
                            body = body
                        }
                    }
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Send to topic result: " + result);
            }
        }
        public async Task SendMessageToTopicAsync(string topic, string title, string body, string imageUrl = null, Dictionary<string, string> data = null)
        {
            var url = "https://fcm.googleapis.com/v1/projects/" + _projectId + "/messages:send";

            var message = new
            {
                message = new
                {
                    topic = topic,
                    notification = new
                    {
                        title = title,
                        body = body,
                        image = imageUrl
                    },
                    data = data // optional key-value
                }
            };

            var json = JsonConvert.SerializeObject(message);

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error sending FCM message: {result}");

                Console.WriteLine("Send to topic result: " + result);
            }
        }
        // 🔹 Xoá token khỏi 1 topic
        public async Task UnsubscribeTokenFromTopicAsync(string token, string topic)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = "https://iid.googleapis.com/v1:batchRemove";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var body = new
                {
                    to = $"/topics/{topic}",
                    registration_tokens = new[] { token }
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Unsubscribe result: " + result);
            }
        }
    }
}