using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Foody
{
    [TestFixture]
    public class Tests
    {
        private RestClient client;
        private const string createdFoodId;
        private const string baseUrl = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86/api/";

        [OneTimeSetUp]
        public void Setup()
        {
            string token = GetJwtToken("gabche1", "gabche123"); //to add my credentions
            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };
            client = new RestClient(options);
        }

        private string GetJwtToken(string username, string password)
        {
            var loginClient = new RestClient(baseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);

            request.AddJsonBody(new { username, password });
            var response = loginClient.Execute(request);

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);

            return json.GetProperty("accesToken").GetString() ?? string.Empty;
    }

        [Test, Order(1)]
        public void CreateFood_ShouldReturnCreated()
        {
            var food = new
            {
                // first we create it
                Name = "New Food",
                Description = "Delicious new food",
                Url = ""
            };

            //create the request
            var request = new RestRequest("api/Food/Create", MethodAccessException.Post);
            request.AddJsonBody(food);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            createdFoodId = json.GetProperty("foodid").GetString() ?? string.Empty;

            Assert.That(createdFoodId, Is.Not.Null.And.Not.Empty, "Food ID should not be null or empty.");
        }

        [Test, Order(2)]
        public void EditFoodTitle_ShouldReturnOk()
        {
            var changes = new[]
                {
                    new {path = "/name", op = "replace", value = "Updated food name" }
                };

            var request = new RestRequest($"/api/Food/Edit/{createdFoodId}", Method.Patch);

            request.AddJsonBody(changes);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var foods = JsonSerializer.Deserialize<List<object>>(response.Content);
            Assert.That(foods, Is.Not.Empty)
        

        

        }






        [OneTimeTearDown]
        public void Cleanup()
        {
            client?.Dispose();
        }
    }
 }
