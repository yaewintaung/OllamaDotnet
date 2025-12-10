using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// FIX 1: Use AddSingleton<IChatClient> for Dependency Injection
builder.Services.AddChatClient(
    new OllamaApiClient(new Uri("http://localhost:11434"), "deepseek-r1")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

// FIX 2: Use your 'AskRequest' record here, not the internal OllamaSharp 'ChatRequest'
app.MapPost(
    "/ask",
    async (AskRequest request, IChatClient client) =>
    {
        // FIX 3: Pass the prompt to CompleteAsync
        // We create a message list to be explicit and safe
        var messages = new List<ChatMessage> { new(ChatRole.User, request.Prompt) };
        Console.WriteLine(request.Prompt);
        var response = await client.GetResponseAsync(request.Prompt);

        return new { Answer = response.Messages[0].Contents[0] };
    }
);

app.Run();

// Your DTO
record AskRequest(string Prompt);
