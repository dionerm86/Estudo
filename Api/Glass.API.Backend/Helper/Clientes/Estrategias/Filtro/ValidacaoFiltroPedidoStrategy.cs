// <copyright file="ValidacaoFiltroPedidoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Clientes.Estrategias.Filtro
{
    /// <summary>
    /// Classe com as estratégias de validação de filtro para o tipo 'Pedido'.
    /// </summary>
    internal class ValidacaoFiltroPedidoStrategy : IValidacaoFiltro
    {
        private readonly ApiController apiController;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ValidacaoFiltroPedidoStrategy"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        public ValidacaoFiltroPedidoStrategy(ApiController apiController)
        {
            this.apiController = apiController;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarAntesBusca(GDASession sessao, int? id, string nome)
        {
            return null;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarDepoisBusca(GDASession sessao, int? id, string nome, ref IEnumerable<Cliente> clientes)
        {
            if (id > 0)
            {
                var cliente = clientes.FirstOrDefault();

                if (cliente != null)
                {
                    var validacao = this.ValidarCliente(sessao, cliente);

                    if (validacao != null)
                    {
                        return validacao;
                    }
                }
            }
            else
            {
                clientes = clientes
                    .Where(cliente =>
                    {
                        return this.ValidarCliente(sessao, cliente) == null;
                    });
            }

            if (!clientes.Any())
            {
                return this.apiController.ErroValidacao("Cliente não encontrado.");
            }

            return null;
        }

        private IHttpActionResult ValidarCliente(GDASession sessao, Cliente cliente)
        {
            var validacoes = new Func<IHttpActionResult>[]
            {
                () => this.ValidarSituacaoCliente(cliente),
                () => this.ValidarLimiteFinanceiroCliente(sessao, cliente),
                () => this.ValidarObservacaoPedidoCliente(sessao, cliente),
            };

            foreach (var validacao in validacoes)
            {
                var resultado = validacao();

                if (resultado != null)
                {
                    return resultado;
                }
            }

            return null;
        }

        private IHttpActionResult ValidarSituacaoCliente(Cliente cliente)
        {
            if (cliente.Situacao == (int)SituacaoCliente.Inativo)
            {
                return this.apiController.ErroValidacao("Cliente inativo. Motivo: " + cliente.Obs);
            }

            if (cliente.Situacao == (int)SituacaoCliente.Cancelado)
            {
                return this.apiController.ErroValidacao("Cliente cancelado. Motivo: " + cliente.Obs);
            }

            if (cliente.Situacao == (int)SituacaoCliente.Bloqueado)
            {
                return this.apiController.ErroValidacao("Cliente bloqueado. Motivo: " + cliente.Obs);
            }

            IEnumerable<string> motivos;
            if (Data.GerenciadorSituacaoCliente.Gerenciador.VerificarBloqueio(null, cliente, out motivos))
            {
                return this.apiController.ErroValidacao($"Cliente bloqueado. Motivo: {string.Join(";", motivos)}");
            }

            return null;
        }

        private IHttpActionResult ValidarLimiteFinanceiroCliente(GDASession sessao, Cliente cliente)
        {
            var limiteCliente = Data.CalculadoraLimiteCredito.Calculadora.ObterLimite(sessao, cliente);
            var limiteDisponivel = limiteCliente - ContasReceberDAO.Instance.GetDebitos(sessao, (uint)cliente.IdCli, null);
            var temLimite = limiteCliente == 0 || limiteDisponivel > 0;

            if (FinanceiroConfig.BloquearEmissaoPedidoLimiteExcedido && !temLimite && FinanceiroConfig.PerguntarVendedorFinalizacaoFinanceiro)
            {
                return this.apiController.ErroValidacao("Cliente não possui limite suficiente para emitir pedidos.");
            }

            return null;
        }

        private IHttpActionResult ValidarObservacaoPedidoCliente(GDASession sessao, Cliente cliente)
        {
            string[] obs = ClienteDAO.Instance.ObterObsPedido(sessao, (uint)cliente.IdCli).Split(';');

            if (obs[0] == "Erro")
            {
                return this.apiController.ErroValidacao(obs[1]);
            }

            return null;
        }
    }
}
