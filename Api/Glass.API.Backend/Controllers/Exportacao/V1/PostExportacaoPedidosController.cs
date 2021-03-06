﻿// <copyright file="PostExportacaoPedidosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Exportacao.V1
{
    /// <summary>
    /// Controller de exportação de pedidos.
    /// </summary>
    public partial class ExportacaoPedidosController : BaseController
    {
        /// <summary>
        /// Consulta a situação da exportação de pedidos.
        /// </summary>
        /// <param name="id">O identificador da exportação de pedidos.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("{id:int}/consultarSituacao")]
        [SwaggerResponse(202, "Consulta realizada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(406, "Exportação não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ConsultarSituacaoExportacaoPedidos(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdExportacao(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var exportacao = ExportacaoDAO.Instance.GetElement(sessao, (uint)id);
                    var idsPedido = PedidoExportacaoDAO.Instance.PesquisarPedidosExportacao(sessao, (uint)id);

                    var loja = LojaDAO.Instance.GetElement(sessao, Data.Helper.UserInfo.GetUserInfo.IdLoja);
                    var fornecedor = FornecedorDAO.Instance.GetElement(exportacao.IdFornec);

                    var listaPedidos = new Dictionary<uint, bool>();
                    foreach (var item in idsPedido)
                    {
                        listaPedidos.Add(Glass.Conversoes.StrParaUint(item.ToString()), true);
                    }

                    byte[] buffer = Data.Helper.UtilsExportacaoPedido.ConfigurarExportacao(listaPedidos, new uint[] { });
                    var urlInicio = fornecedor.UrlSistema.ToLower().Substring(0, fornecedor.UrlSistema.ToLower().LastIndexOf("/webglass")).TrimEnd('/');
                    var urlFornecedor = string.Format("{0}{1}", urlInicio, "/service/wsexportacaopedido.asmx");

                    object[] parametros = new object[] { loja.Cnpj, 1, buffer };

                    // Consulta a situação do pedido
                    object retorno = WebService.ChamarWebService(urlFornecedor, "SyncService", "VerificarExportacaoPedidos", parametros);

                    Data.Helper.UtilsExportacaoPedido.AtualizarPedidosExportacao(retorno as string[]);

                    return this.Ok("A situação dos pedidos foi atualizada.");
                }
                catch (Exception ex)
                {
                    var idsPedido = PedidoExportacaoDAO.Instance.PesquisarPedidosExportacao(sessao, (uint)id);
                    var situacaoPedidos = string.Empty;

                    foreach (var item in idsPedido)
                    {
                        var situacao = PedidoExportacaoDAO.Instance.GetSituacaoExportacao((uint)item).ToString();
                        situacaoPedidos += $" O Pedido {item.ToString()} se encontra {situacao}. \n ";
                    }

                    return this.ErroValidacao($"Erro ao consultar situação da exportação. {situacaoPedidos}", ex);
                }
            }
        }
    }
}
