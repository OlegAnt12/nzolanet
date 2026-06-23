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
    /// <summary>
    /// Filtro de operação que força o Swagger a usar multipart/form-data com file uploads
    /// </summary>
    public class FormFileOperationFilter : IOperationFilter
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

            var schema = context.SchemaGenerator.GenerateSchema(formParameter.Type, context.SchemaRepository);
            
            // Detecta e processa tipos que contêm IFormFile
            if (ContainsFormFile(formParameter.Type))
            {
                // Flatten os campos aninhados para multipart/form-data
                FlattenNestedPropertiesForMultipart(schema, formParameter.Type);
            }
            
            // Força TODOS os IFormFile a serem string/binary recursivamente
            ProcessSchemaForFileUploads(schema, formParameter.Type);

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType { Schema = schema }
                }
            };
        }

        private bool ContainsFormFile(Type type)
        {
            if (type == null)
                return false;

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase);
            foreach (var prop in properties)
            {
                if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                    return true;

                if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                    return true;

                // Recursivamente verifica tipos aninhados
                if (!prop.PropertyType.IsValueType && prop.PropertyType != typeof(string) && !typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                {
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var itemType = prop.PropertyType.GetGenericArguments()[0];
                        if (ContainsFormFile(itemType))
                            return true;
                    }
                }
            }
            return false;
        }

        private void FlattenNestedPropertiesForMultipart(OpenApiSchema schema, Type type)
        {
            if (schema?.Properties == null || type == null)
                return;

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase);
            var flatProperties = new Dictionary<string, OpenApiSchema>(schema.Properties);

            foreach (var prop in properties)
            {
                var schemaKey = schema.Properties.Keys.FirstOrDefault(k =>
                    k.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)
                );

                if (string.IsNullOrEmpty(schemaKey))
                    continue;

                var schemaProp = schema.Properties[schemaKey];

                // Se é um tipo complexo (como ItemConteudoRequestDto), expande os seus campos
                if (schemaProp.Type == "object" && schemaProp.Properties != null && 
                    !prop.PropertyType.IsValueType && prop.PropertyType != typeof(string) && 
                    !typeof(IFormFile).IsAssignableFrom(prop.PropertyType) &&
                    !typeof(IEnumerable<IFormFile>).IsAssignableFrom(prop.PropertyType))
                {
                    // Remove o objeto pai da lista de propriedades
                    flatProperties.Remove(schemaKey);
                    
                    // Adiciona cada propriedade do objeto aninhado como uma propriedade de nível superior
                    foreach (var nestedProp in schemaProp.Properties)
                    {
                        flatProperties[nestedProp.Key] = nestedProp.Value;
                    }
                }
            }

            schema.Properties = flatProperties;
        }

        private void ProcessSchemaForFileUploads(OpenApiSchema schema, Type type)
        {
            if (schema?.Properties == null)
                return;

            var properties = type?.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase) ?? new PropertyInfo[] { };

            foreach (var prop in properties)
            {
                var schemaKey = schema.Properties.Keys.FirstOrDefault(k =>
                    k.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)
                );

                if (string.IsNullOrEmpty(schemaKey) || !schema.Properties.TryGetValue(schemaKey, out var schemaProp))
                    continue;

                // Caso 1: IFormFile direto
                if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                {
                    schemaProp.Type = "string";
                    schemaProp.Format = "binary";
                }
                // Caso 2: List<IFormFile>
                else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    schemaProp.Type = "array";
                    schemaProp.Items = new OpenApiSchema { Type = "string", Format = "binary" };
                }
                // Caso 3: List<ItemConteudoRequestDto> ou outro objeto que contém IFormFile
                else if (schemaProp.Type == "array" && schemaProp.Items?.Properties != null)
                {
                    var itemType = GetGenericListItemType(prop.PropertyType);
                    if (itemType != null)
                    {
                        ProcessSchemaForFileUploads(schemaProp.Items, itemType);
                    }
                }
            }

            // Depois da iteração, verifica novamente se há campos de ficheiros que foram expandidos
            var expandedFileFields = schema.Properties.Where(k => 
                k.Key.Equals("Ficheiros", StringComparison.OrdinalIgnoreCase)).ToList();
            
            foreach (var fileField in expandedFileFields)
            {
                if (fileField.Value?.Type == "array" && fileField.Value?.Items != null)
                {
                    fileField.Value.Items.Type = "string";
                    fileField.Value.Items.Format = "binary";
                }
            }
        }

        private Type GetGenericListItemType(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                return genericArgs.Length > 0 ? genericArgs[0] : null;
            }
            return null;
        }
    }

    /// <summary>
    /// Filtro de schema que marca IFormFile como string/binary em qualquer profundidade
    /// </summary>
    public class FormFileSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Se o próprio tipo é IFormFile
            if (context.Type == typeof(IFormFile))
            {
                schema.Type = "string";
                schema.Format = "binary";
                return;
            }

            if (schema.Properties == null)
                return;

            var contextType = context.Type;
            if (contextType == null)
                return;

            var properties = contextType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase);

            foreach (var property in properties)
            {
                var schemaKey = schema.Properties.Keys.FirstOrDefault(k =>
                    k.Equals(property.Name, StringComparison.OrdinalIgnoreCase)
                );

                if (string.IsNullOrEmpty(schemaKey) || !schema.Properties.TryGetValue(schemaKey, out var schemaProp))
                    continue;

                // IFormFile direto
                if (typeof(IFormFile).IsAssignableFrom(property.PropertyType))
                {
                    schemaProp.Type = "string";
                    schemaProp.Format = "binary";
                }
                // IEnumerable<IFormFile>
                else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(property.PropertyType) && 
                         property.PropertyType != typeof(string))
                {
                    schemaProp.Type = "array";
                    schemaProp.Items = new OpenApiSchema { Type = "string", Format = "binary" };
                }
                // List<T> ou IEnumerable<T>
                else if (schemaProp.Type == "array" && schemaProp.Items?.Properties != null)
                {
                    var itemType = GetGenericListItemType(property.PropertyType);
                    if (itemType != null && itemType != typeof(string))
                    {
                        // Processa recursivamente as propriedades do item
                        var itemProperties = itemType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase);
                        foreach (var itemProp in itemProperties)
                        {
                            if (typeof(IFormFile).IsAssignableFrom(itemProp.PropertyType))
                            {
                                var itemSchemaKey = schemaProp.Items.Properties.Keys.FirstOrDefault(k =>
                                    k.Equals(itemProp.Name, StringComparison.OrdinalIgnoreCase)
                                );

                                if (!string.IsNullOrEmpty(itemSchemaKey) && 
                                    schemaProp.Items.Properties.TryGetValue(itemSchemaKey, out var itemSchemaProp))
                                {
                                    itemSchemaProp.Type = "string";
                                    itemSchemaProp.Format = "binary";
                                }
                            }
                        }
                    }
                }
            }
        }

        private Type GetGenericListItemType(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                return genericArgs.Length > 0 ? genericArgs[0] : null;
            }
            return null;
        }
    }
}
