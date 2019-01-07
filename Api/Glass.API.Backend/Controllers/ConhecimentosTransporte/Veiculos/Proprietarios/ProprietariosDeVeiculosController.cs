// <copyright file="ProprietariosDeVeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL.CTe;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ConhecimentosTransporte.Veiculos.Proprietarios
{
    /// <summary>
    /// Controller de proprietários de veículos.
    /// </summary>
    [RoutePrefix("api/v1/conhecimentosTransporte/veiculos/proprietarios")]
    public partial class ProprietariosDeVeiculosController : BaseController
    {
        private IHttpActionResult ValidarIdProprietarioVeiculo(int idProprietarioVeiculo)
        {
            if (idProprietarioVeiculo <= 0)
            {
                return this.ErroValidacao("Identificador do proprietário de veículo deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdProprietarioVeiculo(GDASession sessao, int idProprietarioVeiculo)
        {
            var validacao = this.ValidarIdProprietarioVeiculo(idProprietarioVeiculo);

            if (validacao == null && !ProprietarioVeiculoDAO.Instance.Exists(sessao, idProprietarioVeiculo))
            {
                return this.NaoEncontrado("Proprietário de veículo não encontrado.");
            }

            return validacao;
        }
    }
}