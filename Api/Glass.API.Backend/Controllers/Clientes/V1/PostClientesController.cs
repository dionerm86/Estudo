// <copyright file="PostClientesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1
{
    /// <summary>
    /// Controller de clientes.
    /// </summary>
    public partial class ClientesController : BaseController
    {
        /// <summary>
        /// Ativa/inativa o cliente.
        /// </summary>
        /// <param name="id">O identificador do cliente que será ativado/inativado.</param>
        /// <returns>Um status HTTP indicando se o cliente foi ativado/inativado.</returns>
        [HttpPost]
        [Route("{id}/alterarSituacao")]
        [SwaggerResponse(202, "Situação do cliente alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na alteração da situação do cliente.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cliente não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarSituacao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdCliente(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    ClienteDAO.Instance.AlteraSituacao(sessao, (uint)id);

                    sessao.Commit();

                    return this.Aceito("Situação do cliente alterada com sucesso.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Ativa os clientes com base no filtro passado.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos clientes.</param>
        /// <returns>Um status HTTP indicando se os clientes foram ativados.</returns>
        [HttpPost]
        [Route("ativar")]
        [SwaggerResponse(202, "Cliente ativados.", Type = typeof(MensagemDto))]
        public IHttpActionResult Ativar([FromBody] Models.Clientes.Lista.FiltroDto filtro)
        {
            try
            {
                var resultado = ServiceLocator.Current
                    .GetInstance<Global.Negocios.IClienteFluxo>()
                    .AtivarClientesInativos(
                        filtro.Id,
                        filtro.NomeCliente,
                        filtro.CpfCnpj,
                        filtro.IdLoja,
                        filtro.Telefone,
                        filtro.Endereco,
                        filtro.Bairro,
                        filtro.IdCidade,
                        filtro.Tipo,
                        filtro.CodigoRota,
                        filtro.IdVendedor,
                        filtro.TipoFiscal != null ? filtro.TipoFiscal.ToArray() : null,
                        filtro.FormasPagamento,
                        filtro.PeriodoCadastroInicio,
                        filtro.PeriodoCadastroFim,
                        filtro.PeriodoSemCompraInicio,
                        filtro.PeriodoSemCompraFim,
                        filtro.PeriodoInativadoInicio,
                        filtro.PeriodoInativadoFim,
                        filtro.IdTabelaDescontoAcrescimo,
                        filtro.ApenasSemRota,
                        filtro.Uf);

                if (!resultado)
                {
                    return this.ErroValidacao(resultado.Message.ToString());
                }

                return this.Aceito("Situação do cliente alterada com sucesso.");
            }
            catch (Exception ex)
            {
                return this.ErroValidacao(ex.Message, ex);
            }
        }

        /// <summary>
        /// Altera o vendedor dos clientes com base no filtro passado.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos clientes.</param>
        /// <returns>Um status HTTP indicando se o vendedor os clientes foram alterados.</returns>
        [HttpPost]
        [Route("alterarVendedor")]
        [SwaggerResponse(202, "Vendedor alterado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarVendedor([FromBody] Models.Clientes.Lista.FiltroDto filtro)
        {
            try
            {
                var resultado = ServiceLocator.Current
                    .GetInstance<Global.Negocios.IClienteFluxo>()
                    .AlterarVendedorClientes(
                        filtro.Id,
                        filtro.NomeCliente,
                        filtro.CpfCnpj,
                        filtro.IdLoja,
                        filtro.Telefone,
                        filtro.Endereco,
                        filtro.Bairro,
                        filtro.IdCidade,
                        filtro.Tipo,
                        filtro.Situacao != null ? filtro.Situacao.Select(f => (int)f).ToArray() : null,
                        filtro.CodigoRota,
                        filtro.IdVendedor,
                        filtro.TipoFiscal != null ? filtro.TipoFiscal.ToArray() : null,
                        filtro.FormasPagamento,
                        filtro.PeriodoCadastroInicio,
                        filtro.PeriodoCadastroFim,
                        filtro.PeriodoSemCompraInicio,
                        filtro.PeriodoSemCompraFim,
                        filtro.PeriodoInativadoInicio,
                        filtro.PeriodoInativadoFim,
                        filtro.IdTabelaDescontoAcrescimo,
                        filtro.ApenasSemRota,
                        filtro.IdVendedorNovo.GetValueOrDefault(),
                        filtro.Uf);

                if (!resultado)
                {
                    return this.ErroValidacao(resultado.Message.ToString());
                }

                return this.Aceito("Vendedor alterado com sucesso.");
            }
            catch (Exception ex)
            {
                return this.ErroValidacao(ex.Message, ex);
            }
        }

        /// <summary>
        /// Altera a rota dos clientes com base no filtro passado.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos clientes.</param>
        /// <returns>Um status HTTP indicando se a rota foi alterada nos clientes.</returns>
        [HttpPost]
        [Route("alterarRota")]
        [SwaggerResponse(202, "Rota alterada.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarRota([FromBody] Models.Clientes.Lista.FiltroDto filtro)
        {
            try
            {
                var resultado = ServiceLocator.Current
                    .GetInstance<Global.Negocios.IClienteFluxo>()
                    .AlterarRotaClientes(
                        filtro.Id,
                        filtro.NomeCliente,
                        filtro.CpfCnpj,
                        filtro.IdLoja,
                        filtro.Telefone,
                        filtro.Endereco,
                        filtro.Bairro,
                        filtro.IdCidade,
                        filtro.Tipo,
                        filtro.Situacao != null ? filtro.Situacao.Select(f => (int)f).ToArray() : null,
                        filtro.CodigoRota,
                        filtro.IdVendedor,
                        filtro.TipoFiscal != null ? filtro.TipoFiscal.ToArray() : null,
                        filtro.FormasPagamento,
                        filtro.PeriodoCadastroInicio,
                        filtro.PeriodoCadastroFim,
                        filtro.PeriodoSemCompraInicio,
                        filtro.PeriodoSemCompraFim,
                        filtro.PeriodoInativadoInicio,
                        filtro.PeriodoInativadoFim,
                        filtro.IdTabelaDescontoAcrescimo,
                        filtro.ApenasSemRota,
                        filtro.IdRotaNova.GetValueOrDefault(),
                        filtro.Uf);

                if (!resultado)
                {
                    return this.ErroValidacao(resultado.Message.ToString());
                }

                return this.Aceito("Rota alterada com sucesso.");
            }
            catch (Exception ex)
            {
                return this.ErroValidacao(ex.Message, ex);
            }
        }
    }
}
