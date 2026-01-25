using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RESTAPI;

internal class Program
{
    private static void Main(string[] args)
    {
        if (!JsonArgParse(args, out string jsonUrl))
        {                
            Util.WriteLineColor(Console.Error, ConsoleColor.DarkYellow, $"[X] No JSONSERVER URL provided!");
            return;
        }
        Util.WriteLineColor(Console.Out, ConsoleColor.Green, $"[OK] JSONSERVER URL Loaded: {jsonUrl}");
        Config.Application["jsonUrl"] =  jsonUrl;
        
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
            
        WebApplication app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }

    private static bool JsonArgParse(string[] args, out string jsonUrl)
    {
        Option<string> jsonOption = new("--json")
        {
            Description = "The json server url (e.g http://localhost:3000)"
        };

        RootCommand rootCommand = new("RESTAPI")
        {
            jsonOption
        };

        ParseResult parseResult = rootCommand.Parse(args);
        OptionResult? jsonResult = parseResult.GetResult(jsonOption);
        if (jsonResult != null && // There is a result
            !jsonResult.Errors.Any() // There are no errors 
            && parseResult.GetValue(jsonOption) is { } parsedJson)
        {
            jsonUrl = parsedJson;
            return true;
        }

        jsonUrl = "";
        return false;
    }
}