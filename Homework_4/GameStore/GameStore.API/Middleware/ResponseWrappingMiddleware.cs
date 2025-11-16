using GameStore.Business.Responses;
using System.Text;
using System.Text.Json;

namespace GameStore.API.Middleware
{
    public class ResponseWrappingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseWrappingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                await _next(context);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }

            int statusCode = context.Response.StatusCode;
            memStream.Seek(0, SeekOrigin.Begin);
            string bodyText = await new StreamReader(memStream, Encoding.UTF8).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(bodyText) && (statusCode == StatusCodes.Status204NoContent || statusCode == 0))
            {
                context.Response.StatusCode = statusCode == 0 ? 204 : statusCode;
                return;
            }

            bool alreadyWrapped = false;
            try
            {
                using JsonDocument doc = JsonDocument.Parse(bodyText);
                if (doc.RootElement.TryGetProperty("isSuccess", out _))
                {
                    alreadyWrapped = true;
                }
            }
            catch { }

            object? newBodyObject;
            string newBodyJson;

            if (!alreadyWrapped)
            {
                if (statusCode >= 400)
                {
                    string errorMessage = !string.IsNullOrEmpty(bodyText)
                        ? bodyText
                        : statusCode == 404 ? "Recurso no encontrado"
                        : statusCode == 400 ? "Solicitud inválida"
                        : "Error";

                    newBodyObject = new ApiResponse<string>
                    {
                        IsSuccess = false,
                        Message = errorMessage,
                        Data = null,
                        StatusCode = statusCode
                    };
                }
                else
                {
                    object? deserializedBody = null;
                    if (!string.IsNullOrEmpty(bodyText))
                    {
                        try
                        {
                            deserializedBody = JsonSerializer.Deserialize<object>(bodyText);
                        }
                        catch
                        {
                            deserializedBody = bodyText;
                        }
                    }

                    newBodyObject = ApiResponse<object>.Success(deserializedBody!, statusCode);
                }

                newBodyJson = JsonSerializer.Serialize(newBodyObject);
            }
            else
            {
                newBodyJson = bodyText;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode == 0 ? 200 : statusCode;
            await context.Response.WriteAsync(newBodyJson, Encoding.UTF8);
        }
    }
}