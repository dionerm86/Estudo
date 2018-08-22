// <copyright file="BaseController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Atributos;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;

namespace Glass.API.Backend.Controllers
{
    /// <summary>
    /// Controller base para a API.
    /// </summary>
    [Authorization]
    [CacheFilter]
    [SwaggerResponseRemoveDefaults]
    [SwaggerResponse(500, "Erro interno do servidor (inesperado).", Type = typeof(MensagemDto))]
    public abstract class BaseController : ApiController
    {
    }
}
