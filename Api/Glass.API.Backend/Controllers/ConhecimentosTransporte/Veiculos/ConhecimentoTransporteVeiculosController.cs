// <copyright file="VeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ConhecimentosTransporte.Veiculos
{
    /// <summary>
    /// Controller de veículos para conhecimento de transporte.
    /// </summary>
    [RoutePrefix("api/v1/conhecimentosTransporte/veiculos")]
    public partial class ConhecimentoTransporteVeiculosController : BaseController
    {
        private IHttpActionResult ValidarPlaca(string placa)
        {
            if (string.IsNullOrEmpty(placa))
            {
                return this.ErroValidacao("Identificador do veículo deve ser informado.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaPlacaVeiculo(string placa)
        {
            var validacao = this.ValidarPlaca(placa);

            var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Global.Negocios.IVeiculoFluxo>();

            if (validacao == null && fluxo.ObtemVeiculo(placa) == null)
            {
                return this.NaoEncontrado("Veículo não encontrado.");
            }

            return validacao;
        }

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

            if (validacao == null && !Data.DAL.CTe.ProprietarioVeiculoDAO.Instance.Exists(sessao, idProprietarioVeiculo))
            {
                return this.NaoEncontrado("Proprietário de veículo não encontrado.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarExclusaoAssociacaoProprietarioVeiculo(GDASession sessao, int idProprietarioVeiculo, string placa)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarExistenciaIdProprietarioVeiculo(sessao, idProprietarioVeiculo)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarExistenciaPlacaVeiculo(placa)));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }
    }
}