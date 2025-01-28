using System.Text;
using System.Text.Json;

namespace Inventory_Management_System.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            context.Request.EnableBuffering();
            string requestBody = string.Empty;
            if (context.Request.ContentLength > 0)
            {
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }
            }
            var originalResponseBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);
            var request = context.Request;
            var responseTime = DateTime.UtcNow - startTime;

            string id = string.Empty;
            memoryStream.Position = 0;
            if (context.Response.StatusCode == StatusCodes.Status201Created && context.Request.Method == "POST")
            {
                try
                {
                    string responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
                    memoryStream.Position = 0;
                    var jsonResponse = JsonDocument.Parse(responseBody);
                    if (jsonResponse.RootElement.TryGetProperty("id", out var idElement))
                    {
                        id = idElement.ToString();
                    }
                }
                catch(JsonException) { }
            }
            else if ((context.Response.StatusCode == StatusCodes.Status200OK && context.Request.Method == "GET") ||
                (context.Response.StatusCode == StatusCodes.Status200OK && context.Request.Method == "PUT") ||
                (context.Response.StatusCode == StatusCodes.Status204NoContent && context.Request.Method == "DELETE"))
            {
                id = context.Request.RouteValues.TryGetValue("id", out var idRoute) ? idRoute.ToString() : null;
            }
            await memoryStream.CopyToAsync(originalResponseBodyStream);

            Console.WriteLine(startTime.ToString());
            Console.WriteLine($"Request: {request.Method} {request.Path}");
            if (!string.IsNullOrEmpty(requestBody))
            {
                Console.WriteLine(requestBody);
            }
            if (!string.IsNullOrEmpty(id))
            {
                Console.WriteLine($"Response id: {id}");
            }
            Console.WriteLine($"Time taken: {responseTime.TotalSeconds} seconds");
        }
    }
}
