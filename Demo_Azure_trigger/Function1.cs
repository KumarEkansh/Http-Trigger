using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Demo_Azure_trigger
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public HttpResponseData Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Extract query parameters from req.Url
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string name = query["name"] ?? "Developer";

            // Create the response object using Isolated Worker syntax
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString($"Hello, {name}! Welcome to Azure Functions");

            return response;
        }
    }
}


//The House Analogy
//In-Process (Roommates): Your C# code and the Azure Functions core engine live in the exact same house (process). You share the kitchen, the plumbing, and the front door. If your code causes a major crash, the whole house goes down.

//Isolated Worker (Neighbors): Your C# code lives in its own separate apartment next door. Azure is in the main building. They talk to each other through a phone line (gRPC). If your apartment has an issue, Azure's building keeps running smoothly.

//1. In-Process Model (The Older Way)
//In the older In-Process model, your function runs directly inside the Azure Functions host process.

//How it works: Azure controls everything—including the version of .NET and the core system libraries your app uses.

//The Catch: Because your code is tightly bound to Azure's internal engine, you often run into version conflicts (like the assembly loading errors you saw earlier) when trying to use modern NuGet packages. Microsoft has phased out this model for newer .NET versions.

//2. Isolated Worker Model (The Modern Standard)
//In the Isolated Worker model, your function runs as an independent console application (which is why it requires a Program.cs file).

//How it works: Your app has complete control over its own startup, middleware, dependency injection, and NuGet packages. Azure just sends requests to your app and waits for a response.

//Why it's better:

//Total Control: You can use any modern .NET version (.NET 8, etc.) without being locked into Azure's internal update schedule.

//Stability: Package conflicts are isolated to your app, preventing mysterious runtime crashes.

//Future-Proof: This is the officially recommended standard for all modern .NET Azure Functions.