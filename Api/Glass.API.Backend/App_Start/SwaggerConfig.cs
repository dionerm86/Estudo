// <copyright file="SwaggerConfig.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Swashbuckle.Application;
using System.IO;
using System.Web.Http;
using System.Web.Http.Description;

namespace Glass.API.Backend
{
    /// <summary>
    /// Classe com a configuração do Swagger.
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Realiza a configuração do Swagger.
        /// </summary>
        /// <param name="config">A configuração HTTP do site.</param>
        public static void Initialize(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.UseFullTypeNameInSchemaIds();

                c.MultipleApiVersions(
                    (apiDesc, targetApiVersion) => ResolveVersionSupportByRouteConstraint(apiDesc, targetApiVersion),
                    (vc) =>
                    {
                        vc.Version("v1", "API v1 de back-end do WebGlass");
                    });

                c.IncludeXmlComments(GetXmlCommentsPath());
            })
            .EnableSwaggerUi();
        }

        private static bool ResolveVersionSupportByRouteConstraint(ApiDescription apiDesc, string targetApiVersion)
        {
            return apiDesc.RelativePath.ToLowerInvariant().Contains(targetApiVersion.ToLowerInvariant());
        }

        private static string GetXmlCommentsPath()
        {
            return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin\\Glass.API.Backend.xml");
        }
    }
}
