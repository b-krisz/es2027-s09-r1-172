using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RESTAPI;

// TODO: VALIDATE URLS

internal class Program
{
    private static void Main(string[] args)
    {
        if (!TryParseArgs(args, out string jsonUrl, out string frUrl))
        {                
            return;
        }
        
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        
        builder.Services.AddHttpClient("JsonApi", client =>
        {
            client.BaseAddress = new Uri(jsonUrl);
        });
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(jsonUrl, frUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
            
        WebApplication app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseAuthorization();
        app.MapControllers();
        
        app.UseCors();
        app.Run();
    }

    private static bool TryParseArgs(string[] args, out string jsonUrl, out string frUrl) // TODO: MAKING ARGUMENTS OPTIONAL, NOT REQUIRED, FOR DEFAULT VALUES
    {
        jsonUrl = "";
        frUrl = "";
        
        Option<string> jsonOption = new("--json")
        {
            Description = "The JSONSERVER url (e.g http://localhost:3000)"
        };
        Option<string> frOption = new("--fr")
        {
            Description = "The FRONTEND url (e.g http://localhost:4000)"
        };

        RootCommand rootCommand = new("RESTAPI")
        {
            jsonOption,
            frOption
        };

        ParseResult parseResult = rootCommand.Parse(args);
        
        OptionResult? jsonResult = parseResult.GetResult(jsonOption);
        if (jsonResult == null || // There is a result
            jsonResult.Errors.Any() || // There are no errors 
            parseResult.GetValue(jsonOption) is not { } parsedJson)
        {
            Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, $"[X] No JSONSERVER URL provided!");
            return false;
        }
        Util.WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] JSONSERVER URL Loaded: {parsedJson}");
        jsonUrl = parsedJson;
        
        OptionResult? frResult = parseResult.GetResult(jsonOption);
        if (frResult == null || // There is a result
            frResult.Errors.Any() || // There are no errors 
            parseResult.GetValue(frOption) is not { } parsedFr)
        {
            Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, $"[X] No FRONTEND URL provided!");
            return false;
        }
        Util.WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] FRONTEND URL Loaded: {parsedFr}");
        frUrl = parsedFr;
        
        return true;
    }
}