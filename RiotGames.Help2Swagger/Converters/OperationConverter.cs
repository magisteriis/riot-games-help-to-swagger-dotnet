using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using RiotGames.Help;
using RiotGames.Help2Swagger.Extensions;

namespace RiotGames.Help2Swagger.Converters
{
    internal static class OperationConverter
    {
        public static OpenApiOperation Convert(KeyValuePair<string, HelpConsoleFunction> function, OpenApiDocument openApi, HelpFullDocument helpFull)
        {
            var (functionIdentifier, functionSchema) = function;
            var operation = new OpenApiOperation
            {
                OperationId = functionIdentifier,
                Description = functionSchema.Help,
                Summary = functionSchema.Description
            };

            foreach (var argument in functionSchema.Arguments.SelectMany(a => a)
                     //.OrderBy(a => a.Key)
                    )
            {
                var parameter = ParameterConverter.Convert(argument, functionSchema, operation, openApi);

                if (parameter == null) continue;

                if (parameter.In == ParameterLocation.Path)
                    parameter.Required = true; // Microsoft.OpenApi should set this, but doesn't right now.
                else
                    parameter.Required = !helpFull!.Functions
                        .Single(f => f.Name == function.Key)
                        .Arguments
                        .Single(a => a.Name == argument.Key)
                        .Optional;

                operation.Parameters.Add(parameter);
            }

            if (functionSchema.Returns != null)
            {
                var contentSchema = SchemaConverter.Convert(functionSchema.Returns, openApi);

                operation.Responses.Add("200", new OpenApiResponse
                {
                    Description = "Successful response",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = contentSchema
                        }
                    }
                });
            }
            else
            {
                operation.Responses.AddNoContent();
            }

            operation.Tags.AddRange(helpFull!.Functions.Single(f => f.Name == functionIdentifier).Tags
                .Where(t => t != "$remoting-binding-module").Select(t => t.Trim('$')));

            return operation;
        }
    }
}
