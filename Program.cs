using System;

namespace Server
{
    using Microsoft.AspNetCore.Builder;
    using System.IO;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            List<string> messages = new();

            app.MapGet("/", () => "OK");

            app.MapPost("/send", async (HttpContext context) =>
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
                string msg = await reader.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(msg))
                    messages.Add(msg);

                return Results.Ok();
            });

            app.MapGet("/messages", () =>
            {
                return string.Join("\n", messages);
            });

            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            app.Run($"http://0.0.0.0:{port}");
        }
    }
}
