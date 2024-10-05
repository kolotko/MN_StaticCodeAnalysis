using System.Net.Http.Json;

namespace MN_StaticCodeAnalysis.FunctionalTests;

public class EndpointsTest
{
    private static readonly HttpClientHandler s_clientHandler = new()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    };
    private static readonly HttpClient s_httpClient = new (s_clientHandler) {BaseAddress = new("http://localhost:5243")};

    [Fact]
    public async Task ListPersons()
    {
        var persons = await s_httpClient.GetFromJsonAsync<List<string>>("/personsAll");
        Assert.NotNull(persons);
        Assert.Equal(4, persons.Count);
    }
    
    [Fact]
    public async Task GetPiotr()
    {
        var persons = await s_httpClient.GetFromJsonAsync<List<string>>("/persons?name=Piotr");
        var piotr = persons!.FirstOrDefault();
        Assert.NotNull(piotr);
        Assert.Equal("Piotr", piotr);
    }
}