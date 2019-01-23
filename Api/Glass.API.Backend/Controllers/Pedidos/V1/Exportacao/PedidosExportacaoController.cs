// <copyright file="PedidosExportacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Pedidos.V1.Exportacao.PedidoExportar;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.Exportacao
{
    /// <summary>
    /// Controller de pedidos para exportação.
    /// </summary>
    [RoutePrefix("api/v1/pedidos/exportacao")]
    public partial class PedidosExportacaoController : BaseController
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
                return this.NaoEncontrado($"Pedido {id} não encontrado.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarIdFornecedor(int id)
        {
            if (id < 0)
            {
                return this.ErroValidacao("Identificador do fornecedor deve ser um número maior que zero.");
            }
            else if (id == 0)
            {
                return this.ErroValidacao("Selecione o fornecedor para consulta.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdFornecedor(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdFornecedor(id);

            if (validacao == null && !FornecedorDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado($"Fornecedor {id} não encontrado.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarIdProdutoPedido(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do produto do pedido deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdProdutoPedido(GDASession sessao, int idProdutoPedido)
        {
            var validacao = this.ValidarIdProdutoPedido(idProdutoPedido);

            if (validacao == null && !ProdutosPedidoDAO.Instance.Exists(sessao, (uint)idProdutoPedido))
            {
                return this.NaoEncontrado($"Produto de pedido não encontrado.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarExistenciaPedidoFornecedor(GDASession sessao, int idPedido, int idFornecedor)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarExistenciaIdPedido(sessao, idPedido)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarExistenciaIdFornecedor(sessao, idFornecedor)));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarExistenciaIdsFornecedoresPedidosEProdutosPedidoExportacao(GDASession sessao, IEnumerable<PedidoDto> pedidosExportacao)
        {
            IHttpActionResult validacao = null;

            if (pedidosExportacao == null)
            {
                return this.ErroValidacao("Informe os pedidos que serão exportados.");
            }

            foreach (var pedido in pedidosExportacao)
            {
                if (pedido.IdPedido == null)
                {
                    return this.ErroValidacao("Identificador do pedido é obrigatório.");
                }

                validacao = this.ValidarExistenciaIdPedido(sessao, (int)pedido.IdPedido);

                if (validacao != null)
                {
                    return validacao;
                }

                if (pedido.IdsProdutoPedido == null && !pedido.IdsProdutoPedido.Any())
                {
                    return this.ErroValidacao($"O pedido de número.: {pedido.IdPedido} não possui produtos a serem exportados ou não possui produtos selecionados.");
                }

                foreach (var produtoPedido in pedido.IdsProdutoPedido)
                {
                    validacao = this.ValidarExistenciaIdProdutoPedido(sessao, produtoPedido);

                    if (validacao != null)
                    {
                        return validacao;
                    }
                }
            }

            return null;
        }

        private IHttpActionResult ValidarExportacaoPedido(GDASession sessao, PedidosExportarDto exportacao)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ExportarImportarPedido))
            {
                return this.ErroValidacao("Exportação desativada no sistema.");
            }

            var validacao = this.ValidarExistenciaIdsFornecedoresPedidosEProdutosPedidoExportacao(sessao, exportacao.Pedidos);

            if (validacao != null)
            {
                return validacao;
            }

            return null;
        }
    }
}