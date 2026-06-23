using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NzolaWebAPI.Helpers
{
    public class FormFileMappingFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formParameter = context.ApiDescription.ParameterDescriptions.FirstOrDefault(p =>
                p.Source?.Id?.Equals("Form", StringComparison.OrdinalIgnoreCase) == true
                && p.Type != null
                && !p.Type.IsPrimitive
                && p.Type != typeof(string)
            );

            if (formParameter?.Type == null)
                return;

            // Força a operação do Swagger a aceitar multipart/form-data com o formato correto
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(
                            formParameter.Type,
                            context.SchemaRepository
                        ),
                    },
                },
            };
        }
    }

}
