using Aaron.Pina.Blog.Article._04.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("server-api", Configuration.TokenServer.HttpClientSettings)
                .AddHttpMessageHandler<TokenRefreshHandler>();
builder.Services.AddSingleton<TokenStore>();

var app = builder.Build();

app.MapGet("/", () => "Hello world!");

app.Run();
