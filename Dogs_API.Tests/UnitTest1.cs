using Dogs_API.Controllers;
using Dogs_API.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Dogs_API.Tests
{
    public class Tests
    {
        private HttpClient _httpClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var testServer = new TestServerBuilder().Build();
            _httpClient = testServer.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _httpClient.Dispose();
        }

        [Test]
        public async Task Ping_ShouldReturnSuccessMessage()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/ping");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Dogs house service. Version 1.0.1", content);
        }

        [Test]
        public async Task GetDogs_ShouldReturnListOfDogs()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/dogs");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var dogs = JsonConvert.DeserializeObject<List<Dog>>(content);
            Assert.AreEqual(2, dogs.Count);
            Assert.AreEqual("Neo", dogs[0].Name);
            Assert.AreEqual("red & amber", dogs[0].Color);
            Assert.AreEqual(22, dogs[0].TailLength);
            Assert.AreEqual(32, dogs[0].Weight);
            Assert.AreEqual("Jessy", dogs[1].Name);
            Assert.AreEqual("black & white", dogs[1].Color);
            Assert.AreEqual(7, dogs[1].TailLength);
            Assert.AreEqual(14, dogs[1].Weight);
        }

        [Test]
        public async Task GetDogs_WithSorting_ShouldReturnSortedDogs()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/dogs?attribute=weight&order=desc");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var dogs = JsonConvert.DeserializeObject<List<Dog>>(content);
            Assert.AreEqual(2, dogs.Count);
            Assert.AreEqual("Neo", dogs[0].Name);
            Assert.AreEqual("Jessy", dogs[1].Name);
        }

        [Test]
        public async Task GetDogs_WithPagination_ShouldReturnCorrectPageOfDogs()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/dogs?pageNumber=2&limit=1");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var dogs = JsonConvert.DeserializeObject<List<Dog>>(content);
            Assert.AreEqual(1, dogs.Count);
            Assert.AreEqual("Jessy", dogs[0].Name);
        }

        [Test]
        public async Task CreateDog_WithValidData_ShouldReturnCreatedStatus()
        {
            // Arrange
            var dog = new Dog
            {
                Name = "Doggy",
                Color = "red",
                TailLength = 173,
                Weight = 33
            };
            var requestBody = JsonConvert.SerializeObject(dog);
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/dogs");
            request.Content = requestContent;

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public async Task CreateDog_WithDuplicateName_ShouldReturnConflictStatus()
        {
            // Arrange
            var dog = new Dog
            {
                Name = "Neo",
                Color = "red",
                TailLength = 173,
                Weight = 33
            };
            var requestBody = JsonConvert.SerializeObject(dog);
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/dogs");
            request.Content = requestContent;

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Test]
        public async Task CreateDog_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var dog = new Dog
            {
                Name = "Doggy",
                Color = "red",
                TailLength = -5, // Invalid tail length
                Weight = 33
            };
            var requestBody = JsonConvert.SerializeObject(dog);
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/dogs");
            request.Content = requestContent;

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task CreateDog_WithInvalidJson_ShouldReturnBadRequest()
        {
            // Arrange
            var requestBody = "Invalid JSON";
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/dogs");
            request.Content = requestContent;

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}