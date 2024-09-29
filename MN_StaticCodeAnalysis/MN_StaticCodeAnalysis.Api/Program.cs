var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

var personNames = new string[] { "Maciej", "Karol", "Piotr", "Jakub" };

app.MapGet("/personsAll", () => personNames);
    // .WithName("persons");

app.MapGet("/persons", (string name) => personNames.Where(x => x.Contains(name)));
    // .WithName("persons");

app.Run();
