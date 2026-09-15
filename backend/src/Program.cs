var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/alive", () => Results.Text("estou vivo", "text/plain"));

app.Run();

public partial class Program;
