// <copyright file="VeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Veiculos.V1
{
    /// <summary>
    /// Controller de veículos.
    /// </summary>
    [RoutePrefix("api/v1/veiculos")]
    public partial class VeiculosController : BaseController
    {
        private IHttpActionResult ValidarPlaca(string placa)
        {
            if (string.IsNullOrEmpty(placa))
            {
                return this.ErroValidacao("Identificador do veículo deve ser informado.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaPlacaVeiculo(GDASession sessao, string placa)
        {
            var validacao = this.ValidarPlaca(placa);

            if (validacao == null && !VeiculoDAO.Instance.Exists(sessao, placa))
            {
                return this.NaoEncontrado("Veículo não encontrado.");
            }

            return null;
        }
    }
}
