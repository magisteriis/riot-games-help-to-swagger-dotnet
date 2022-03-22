using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace RiotGames.Help2Swagger.Extensions
{
    internal static class OpenApiExtensions
    {
        public static bool ContainsEnum(this IDictionary<string, OpenApiSchema> schemas, string key) =>
            schemas.ContainsKey(key) && schemas[key].Enum.Any();
    }
}
