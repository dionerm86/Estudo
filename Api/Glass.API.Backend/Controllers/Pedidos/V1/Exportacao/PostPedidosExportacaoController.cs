// <copyright file="PostPedidosExportacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.V1.Exportacao.PedidoExportar;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.Exportacao
{
    /// <summary>
    /// Controller de pedidos para exportação.
    /// </summary>
    public partial class PedidosExportacaoController : BaseController
    {
        /// <summary>
        /// Realiza a exportação de um ou mais pedidos.
        /// </summary>
        /// <param name="dados">Os dados para exportação dos pedidos.</param>
        /// <returns>Um status HTTP indicando se o pedido foi exportado.</returns>
        [HttpPost]
        [Route("exportar")]
        [SwaggerResponse(200, "Pedidos exportados.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.")]
        [SwaggerResponse(404, "Pedido não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExportarPedidos(PedidosExportarDto dados)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExportacaoPedido(sessao, dados);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fornecedor = FornecedorDAO.Instance.GetElement(sessao, (uint)(dados.IdFornecedor ?? 0));
                    var loja = LojaDAO.Instance.GetElement(sessao, UserInfo.GetUserInfo.IdLoja);

                    Dictionary<uint, bool> idsPedidosComOuSemBeneficiamentos = new Dictionary<uint, bool>();
                    Dictionary<uint, List<uint>> idsPedidosProdutosPedido = new Dictionary<uint, List<uint>>();
                    List<uint> idsProdutosPedidos = new List<uint>();
                    List<uint> idsProdutosPedido = new List<uint>();

                    foreach (var pedido in dados.Pedidos)
                    {
                        idsPedidosComOuSemBeneficiamentos.Add(
                            (uint)(pedido.IdPedido ?? 0),
                            pedido.ExportarBeneficiamento.GetValueOrDefault(false));

                        idsProdutosPedido = pedido.IdsProdutoPedido.Select(idpp => (uint)idpp).ToList();

                        idsPedidosProdutosPedido.Add((uint)(pedido.IdPedido ?? 0), idsProdutosPedido);

                        idsProdutosPedidos.AddRange(idsProdutosPedido);
                    }

                    uint[] listaIdsPedidos = new uint[idsPedidosComOuSemBeneficiamentos.Count];
                    idsPedidosComOuSemBeneficiamentos.Keys.CopyTo(listaIdsPedidos, 0);

                    byte[] buffer = UtilsExportacaoPedido.ConfigurarExportacao(idsPedidosComOuSemBeneficiamentos, idsProdutosPedidos.ToArray());

                    UtilsExportacaoPedido.CriarExportacao((uint)fornecedor.IdFornec, listaIdsPedidos, idsPedidosProdutosPedido);

                    var urlFornecedor = $"{fornecedor.UrlSistema.ToLower().Substring(0, fornecedor.UrlSistema.ToLower().LastIndexOf("/webglass")).TrimEnd('/')}/service/wsexportacaopedido.asmx";

                    object[] parametros = new object[] { loja.Cnpj, 1, buffer };
                    var retornoWebService = WebService.ChamarWebService(urlFornecedor, "SyncService", "EnviarPedidosFornecedor", parametros);

                    string[] dadosRetorno = retornoWebService as string[];

                    UtilsExportacaoPedido.ProcessarDadosExportacao(sessao, dadosRetorno, idsPedidosComOuSemBeneficiamentos);

                    sessao.Commit();
                    sessao.Close();

                    return this.Ok(dadosRetorno[1]);
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao("Erro ao exportar pedidos.", e);
                }
            }
        }

        /// <summary>
        /// Consulta a situação da exportação de um pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="idFornecedor">O identificador do fornecedor.</param>
        /// <returns>Um status HTTP com o resultado da busca.</returns>
        [HttpPost]
        [Route("consultarSituacao")]
        [SwaggerResponse(202, "Situação de exportação consultada e atualizada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.")]
        [SwaggerResponse(404, "Pedido não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ConsultarSituacaoExportacaoPedido(int idPedido, int idFornecedor)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaPedidoFornecedor(sessao, idPedido, idFornecedor);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fornecedor = FornecedorDAO.Instance.GetElement(sessao, (uint)idFornecedor);
                    var loja = LojaDAO.Instance.GetElement(UserInfo.GetUserInfo.IdLoja);

                    Dictionary<uint, bool> pedido = new Dictionary<uint, bool>();
                    pedido.Add((uint)idPedido, true);

                    byte[] buffer = UtilsExportacaoPedido.ConfigurarExportacao(pedido, new uint[] { });
                    var urlFornecedor = $"{fornecedor.UrlSistema.ToLower().Substring(0, fornecedor.UrlSistema.ToLower().LastIndexOf("/webglass")).TrimEnd('/')}/service/wsexportacaopedido.asmx";

                    object[] parametros = new object[] { loja.Cnpj, 1, buffer };

                    object retorno = WebService.ChamarWebService(urlFornecedor, "SyncService", "VerificarExportacaoPedidos", parametros);

                    UtilsExportacaoPedido.AtualizarPedidosExportacao(sessao, retorno as string[]);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito("A Situação do pedido foi atualizada.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao("Falha ao consultar situação.", ex);
                }
            }
        }
    }
}