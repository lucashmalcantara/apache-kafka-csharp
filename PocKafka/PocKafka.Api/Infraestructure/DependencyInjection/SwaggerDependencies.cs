using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;

namespace PocKafka.Api.Infraestructure.DependencyInjection
{
    public static class SwaggerDependencies
    {
		private const string ApiName = "PocKafka.Api";

		public static IServiceCollection AddSwaggerDependencies(this IServiceCollection services)
        {
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = ApiName,
					Version = "v1",
					Description = "This API shows how to produce events in Apache Kafka message broker.",
					Contact = new OpenApiContact
					{
						Name = "Lucas Alcântara"
					},
				});

				AddCommentsPathForTheSwaggerJsonAndUi(options);
			});

			return services;
        }

		private static void AddCommentsPathForTheSwaggerJsonAndUi(SwaggerGenOptions options)
        {
			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			options.IncludeXmlComments(xmlPath);
		}

		public static IApplicationBuilder AddSwaggerDependencies(this IApplicationBuilder app)
		{
			app.UseSwagger();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiName);

				// To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
				c.RoutePrefix = string.Empty;
			});

			return app;
		}
	}
}
