using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace InventoryDemo.Extensions
{
    public static class ProblemDetailsExtensions
    {
		public static void UseProblemDetailsExceptionHandler(this IApplicationBuilder app)
		{
			app.UseExceptionHandler(builder =>
			{
				builder.Run(async context =>
				{
					var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

					if (exceptionHandlerFeature is not null)
					{
						var exception = exceptionHandlerFeature.Error;

						var problemDetails = new ProblemDetails
						{
							Instance = context.Request.HttpContext.Request.Path
						};
						
						if (exception is BadHttpRequestException badHttpRequestException)
						{
							problemDetails.Title = "The request is invalid";
							problemDetails.Status = StatusCodes.Status400BadRequest;
							problemDetails.Detail = badHttpRequestException.Message;
						}
						else
						{
							problemDetails.Title = exception.Message;
							problemDetails.Status = StatusCodes.Status500InternalServerError;
							problemDetails.Detail = exception.Demystify().ToString();
						}

						context.Response.StatusCode = problemDetails.Status.Value;
						context.Response.ContentType = "application/problem+json";

						var json = JsonSerializer.Serialize(problemDetails);
						await context.Response.WriteAsync(json);
					}
				});
			});
		}
	}
}
