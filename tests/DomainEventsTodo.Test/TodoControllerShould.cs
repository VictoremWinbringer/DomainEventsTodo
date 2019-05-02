using DomainEventsTodo.Domain;
using DomainEventsTodo.Repositories.Abstract;
using DomainEventsTodo.ViewModels;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DomainEventsTodo.Test
{
    public class TodoControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TodoDisplayVm _todo;
        private readonly string _root;
        private readonly TestServer _server;
        private readonly List<Todo> _mementoes = new List<Todo>();
        private readonly Mock<ITodoRepository> _repository = new Mock<ITodoRepository>();

        public TodoControllerShould()
        {
            SetupRepo();

            _server = CreateServer();

            _client = CreateClient(_server);

            _todo = new TodoDisplayVm
            {
                Description = "Bla bla bla"
            };

            _root = "api/v1/todo/";
        }
        

        [Fact]
        public async Task Crud()
        {
            await Create();
            await ReadAll();
            await Read();
            await Update();
            await Delete();
        }

        [Fact]
        public async Task Count_Equal_1()
        {
            _mementoes.Clear();

            _mementoes.Add(new Todo(new Guid(),String.Empty, false));

            var response = await _client.GetAsync(_root + "count");

            response.EnsureSuccessStatusCode();

            var result = int.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Not_Create_Duplicate_Description()
        {
            var todo = new TodoDisplayVm
            {
                Description = "Ololosh"
            };

            _mementoes.Clear();

            _mementoes.Add( new Todo(todo.Id,todo.Description,todo.IsComplete));


            var result = await _client.PostAsync(_root, CreateContent(todo));

            Assert.NotEqual(true, result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Not_Create_Too_Short_Description()
        {
            var todo = new TodoDisplayVm
            {
                Description = "O"
            };

            var response = await _client.PostAsync(_root, CreateContent(todo));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Not_Create_Empty_Description()
        {
            var todo = new TodoDisplayVm
            {
                Description = "       "
            };

            var response = await _client.PostAsync(_root, CreateContent(todo));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Not_Create_Nul_Description()
        {
            var todo = new TodoDisplayVm
            {
                Description = null
            };

            var response = await _client.PostAsync(_root, CreateContent(todo));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }


        [Fact]
        public async Task MakeComplete()
        {
            var todo = new TodoDisplayVm
            {
                Description = "MakeComplete"
            };

            var url = "http://localhost:8888/";

            var server = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls(url)
                .Build();

            Task.Run(() =>
            {
                server.Start();
            });

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:8888/hub")
                .WithConsoleLogger()
                .Build();

            connection.On<string>("Notify", data =>
            {
                Assert.Equal(todo.Description + " is complete", data);
            });

            await connection.StartAsync();

            var client = new HttpClient();

            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var create = await client.PostAsync(_root, CreateContent(todo));

            create.EnsureSuccessStatusCode();

            var id = (await FromContent(create.Content)).Id;

            var response = await client.PostAsync(_root + id + "/MakeComplete", new StringContent(""));

            response.EnsureSuccessStatusCode();

            var result = await Get(id, client);

            Assert.Equal(id, result.Id);
            Assert.Equal(todo.Description, result.Description);
            Assert.Equal(true, result.IsComplete);

            await client.DeleteAsync(_root + id);

        }

        [Fact]
        public async Task Get_Not_Accept_Default_Id()
        {
            var result = await _client.GetAsync(_root + default(Guid));

            Assert.True(!result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Delete_Accept_Default_Id()
        {
            var result = await _client.DeleteAsync(_root + default(Guid));

            result.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Put_Not_Accept_Default_Id()
        {
            var result = await _client.PutAsync(_root + default(Guid), CreateContent(_todo));

            Assert.True(!result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task MakeComplete_Not_Accept_Default_Id()
        {
            var result = await _client.PostAsync(_root + default(Guid) + "/MakeComplete", new StringContent(""));

            Assert.True(!result.IsSuccessStatusCode);
        }

        public void Dispose()
        {
            _mementoes.Clear();
            _client.Dispose();
            _server.Dispose();
        }


        #region Private methods

        private void SetupRepo()
        {
            _mementoes.Clear();

            _repository.Setup(r => r[It.IsAny<Guid>()])
                .Returns<Guid>(g =>
                {
                    var todo = _mementoes.FirstOrDefault(m => m.Id == g);

                    if (todo == null)
                        return null;
                    return todo;
                });

            _repository.Setup(r => r.Add(It.IsAny<Todo>()))
                .Callback<Todo>(todo => _mementoes.Add(todo));

            _repository.Setup(r => r.Replace(It.IsAny<Todo>()))
                .Callback<Todo>(todo =>
                {
                    var index = _mementoes.FindIndex(m => m.Id == todo.Id);

                    _mementoes[index] = todo;
                });

            _repository.Setup(r => r.Remove(It.IsAny<Guid>()))
                .Callback<Guid>(guid =>
                {
                    var todo = _mementoes.FirstOrDefault(m => m.Id == guid);

                    _mementoes.Remove(todo);

                });

            _repository.Setup(r => r.All())
                .Returns(_mementoes);

        }

        private TestServer CreateServer()
        {
            return new TestServer(CreateBuilder());
        }

        private IWebHostBuilder CreateBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .ConfigureServices(s => s.AddScoped<ITodoRepository>(services => _repository.Object));
        }

        private HttpClient CreateClient(TestServer server)
        {
            var client = server.CreateClient();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private string ToJson(TodoDisplayVm todo)
        {
            return JsonConvert.SerializeObject(todo);
        }

        private TodoDisplayVm FromJson(string todo)
        {
            return JsonConvert.DeserializeObject<TodoDisplayVm>(todo);
        }

        private StringContent CreateContent(TodoDisplayVm todo)
        {
            return new StringContent(ToJson(todo), Encoding.UTF8, "application/json");
        }

        private async Task<TodoDisplayVm> FromContent(HttpContent content)
        {
            return FromJson(await content.ReadAsStringAsync());
        }

        private async Task<TodoDisplayVm> Get(Guid id, HttpClient client)
        {
            var response = await client.GetAsync(_root + id);

            response.EnsureSuccessStatusCode();

            return await FromContent(response.Content);
        }

        private async Task<TodoDisplayVm> Get(Guid id)
        {
            var response = await _client.GetAsync(_root + id);

            response.EnsureSuccessStatusCode();

            return await FromContent(response.Content);
        }


        private async Task Create()
        {
            var response = await _client.PostAsync(_root, CreateContent(_todo));

            response.EnsureSuccessStatusCode();

            var result = await FromContent(response.Content);

            Assert.NotNull(result);
            Assert.NotEqual(default(Guid), result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.Equal(false, result.IsComplete);

            _todo.Id = result.Id;
        }

        private async Task ReadAll()
        {
            var result = await _client.GetAsync(_root);

            result.EnsureSuccessStatusCode();

            var array = JsonConvert.DeserializeObject<TodoDisplayVm[]>(await result.Content.ReadAsStringAsync());

            Assert.Contains(array, x => x.Id == _todo.Id);

        }

        private async Task Read()
        {
            var result = await Get(_todo.Id);

            Assert.Equal(_todo.Id, result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.Equal(_todo.IsComplete, result.IsComplete);
        }

        private async Task Update()
        {
            _todo.Description = "Foo Bar Baz";
            _todo.IsComplete = true;

            var response = await _client.PutAsync(_root + _todo.Id, CreateContent(_todo));

            response.EnsureSuccessStatusCode();

            var result = await Get(_todo.Id);

            Assert.Equal(_todo.Id, result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.NotEqual(_todo.IsComplete, result.IsComplete);
        }

        private async Task Delete()
        {
            var response = await _client.DeleteAsync(_root + _todo.Id);

            response.EnsureSuccessStatusCode();

            Assert.DoesNotContain(_mementoes, x => x.Id == _todo.Id);
        }

        #endregion
    }
}
