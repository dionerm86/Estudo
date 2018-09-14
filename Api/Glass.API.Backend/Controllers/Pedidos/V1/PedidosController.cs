// <copyright file="PedidosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Pedidos.CadastroAtualizacao;
using Glass.Configuracoes;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1
{
    /// <summary>
    /// Controller de pedidos.
    /// </summary>
    [RoutePrefix("api/v1/pedidos")]
    public partial class PedidosController : BaseController
    {
        private IHttpActionResult ValidarIdPedido(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do pedido deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdPedido(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdPedido(id);

            if (validacao == null && !PedidoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Pedido não encontrado.");
            }

            return null;
        }

        private IHttpActionResult ValidarCadastroPedido(GDASession sessao, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoPedido(dados, "cadastro")));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarPedidosProntosNaoLiberados(sessao, dados.IdCliente.GetValueOrDefault())));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarAtualizacaoPedido(GDASession sessao, int id, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarIdPedido(id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoPedido(dados, "atualização")));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarSituacaoPedido(sessao, id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarTipoVendaObra(sessao, id, dados)));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarCadastroAtualizacaoPedido(CadastroAtualizacaoDto dados, string tipo)
        {
            if (dados == null)
            {
                return this.ErroValidacao(string.Format("É preciso informar os dados para {0} do pedido.", tipo));
            }

            return null;
        }

        private IHttpActionResult ValidarSituacaoPedido(GDASession sessao, int id)
        {
            var situacaoPedido = PedidoDAO.Instance.ObtemSituacao(sessao, (uint)id);

            if (situacaoPedido != Data.Model.Pedido.SituacaoPedido.Ativo && situacaoPedido != Data.Model.Pedido.SituacaoPedido.AtivoConferencia)
            {
                return this.ErroValidacao("O pedido não está ativo, não é possível alterá-lo!.");
            }

            return null;
        }

        private IHttpActionResult ValidarPedidosProntosNaoLiberados(GDASession sessao, int idCliente)
        {
            var idsPedidosBloqueados = PedidoDAO.Instance.GetIdsBloqueioEmissao(sessao, (uint)idCliente);

            if (!string.IsNullOrWhiteSpace(idsPedidosBloqueados))
            {
                return this.ErroValidacao($"Não é possível emitir este pedido, o(s) pedido(s) {string.Join(", ", idsPedidosBloqueados)} está(ão) pronto(s) há pelo menos {PedidoConfig.NumeroDiasPedidoProntoAtrasado} e ainda não foi(ram) liberado(s).");
            }

            return null;
        }

        private IHttpActionResult ValidarTipoVendaObra(GDASession sessao, int id, CadastroAtualizacaoDto dados)
        {
            if (PedidoConfig.DadosPedido.UsarControleNovoObra)
            {
                var tipoVendaAtual = PedidoDAO.Instance.ObtemTipoVenda(sessao, (uint)id);
                var qtdProdutosPedido = ProdutosPedidoDAO.Instance.CountInPedido((uint)id);

                if (tipoVendaAtual != (int)Data.Model.Pedido.TipoVendaPedido.Obra && dados.TipoVenda == Data.Model.Pedido.TipoVendaPedido.Obra && qtdProdutosPedido > 0)
                {
                    return this.ErroValidacao($"Não é possível escolher obra como tipo de venda se o pedido tiver algum produto cadastrado.");
                }

                if (tipoVendaAtual == (int)Data.Model.Pedido.TipoVendaPedido.Obra && dados.TipoVenda != Data.Model.Pedido.TipoVendaPedido.Obra && qtdProdutosPedido > 0)
                {
                    return this.ErroValidacao($"O tipo de venda do pedido deve manter como obra, pois já tem produtos cadastrados.");
                }
            }

            return null;
        }
    }
}
