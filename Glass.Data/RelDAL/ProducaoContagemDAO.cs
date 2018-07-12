using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Collections.Generic;
using System;
using System.Linq;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoContagemDAO : BaseDAO<ProducaoContagem, ProducaoContagemDAO>
    {
        /// <summary>
        /// SQL que recupera a contagem da produção.
        /// </summary>
        internal string SqlProducaoContagem(int altura, string codigoEtiqueta, string codigoPedidoCliente, DateTime? dataConfirmacaoPedidoFim, DateTime? dataConfirmacaoPedidoInicio,
            DateTime? dataEntregaFim, DateTime? dataEntregaInicio, DateTime? dataFabricaFim, DateTime? dataFabricaInicio, DateTime? dataLeituraFim, DateTime? dataLeituraInicio, float espessura,
            int fastDelivery, int idCarregamento, int idCliente, int idFuncionario, int idImpressao, int idPedido, IEnumerable<int> idsAplicacao, int idSetor, IEnumerable<int> idsProcesso,
            IEnumerable<int> idsRota, IEnumerable<int> idsSubgrupo, int largura, string nomeCliente, IEnumerable<int> pecasProducaoCanceladas, bool setoresAnteriores, bool setoresPosteriores,
            int situacaoPedido, IEnumerable<int> situacoes, int tipoEntrega, ProdutoPedidoProducaoDAO.TipoRetorno tipoRetorno, IEnumerable<int> tiposPedido)
        {
            #region Declaração de variáveis

            // Define se ao filtrar pela data de entrega será filtrado também pela data de fábrica
            var filtrarDataFabrica = Configuracoes.ProducaoConfig.BuscarDataFabricaConsultaProducao;
            var buscarNomeFantasia = Configuracoes.ProducaoConfig.TelaConsulta.BuscarNomeFantasiaConsultaProducao;
            var usarJoin = idSetor > 0 && (dataLeituraInicio > DateTime.MinValue || dataLeituraFim > DateTime.MinValue);
            var campos = string.Empty;
            var sql = string.Empty;
            var criterio = string.Empty;

            #endregion

            #region Consulta

            campos = string.Format(@"
                pp.IdPedido, ppp.IdSetor,
                CONCAT(CAST(ped.IdPedido AS CHAR), IF(ped.IdPedidoAnterior IS NOT NULL, CONCAT(' (', CONCAT(CAST(ped.IdPedidoAnterior AS CHAR), 'R)')), ''),
                    IF(ppp.IdPedidoExpedicao IS NOT NULL, CONCAT(' (Exp. ', CAST(ppp.IdPedidoExpedicao AS CHAR), ')'), '')) AS IdPedidoExibir,
                ROUND(IF(ped.TipoPedido={0}, ((((50 - IF(MOD(a.Altura, 50) > 0, MOD(a.Altura, 50), 50)) + a.Altura) * ((50 - IF(MOD(a.Largura, 50) > 0, MOD(a.Largura, 50), 50)) + a.Largura)) / 1000000) *
                    a.Qtde, pp.TotM2Calc) / (pp.Qtde * IF(ped.TipoPedido={0}, a.Qtde, 1)), 4) AS TotM2,
                '$$$' AS Criterio",
                // Posição 0.
                (int)Pedido.TipoPedidoEnum.MaoDeObra);

            sql = string.Format(@"SELECT {0} FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN liberarpedido lp ON (ped.IdLiberarPedido = lp.IdLiberarPedido)
                    {1}
                    {2}
                WHERE 1 ",
                // Posição 0.
                campos,
                // Posição 1.
                dataFabricaInicio > DateTime.MinValue || dataFabricaFim > DateTime.MinValue || filtrarDataFabrica ? "LEFT JOIN pedido_espelho pedEsp ON (ped.IdPedido = pedEsp.IdPedido)" : string.Empty,
                // Posição 2.
                usarJoin ? "LEFT JOIN leitura_producao lp1 ON (ppp.IdProdPedProducao = lp1.IdProdPedProducao)" : string.Empty);

            #endregion

            #region Filtros

            if (idCarregamento > 0)
            {
                sql += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento WHERE IdCarregamento={0})", idCarregamento);
                criterio += string.Format("Carregamento: {0}    ", idCarregamento);
            }

            if (idCarregamento > 0)
            {
                sql += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento WHERE IdCarregamento={0})", idCarregamento);
                criterio += string.Format("Carregamento: {0}    ", idCarregamento);
            }

            if (idPedido > 0)
            {
                sql += string.Format(" AND (ped.IdPedido={0}", idPedido);

                // Na vidrália/colpany não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira teria que mudar totalmente a forma de fazer o count.
                if (Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido && PedidoDAO.Instance.IsPedidoReposto(null, (uint)idPedido))
                {
                    sql += string.Format(" OR ped.IdPedidoAnterior={0}", idPedido);
                }

                if (PedidoDAO.Instance.IsPedidoExpedicaoBox(null, (uint)idPedido))
                {
                    sql += string.Format(" OR ppp.IdPedidoExpedicao={0}", idPedido);
                }

                sql += ")";
                criterio += string.Format("Pedido: {0}    ", idPedido);
            }

            if (!string.IsNullOrEmpty(codigoEtiqueta))
            {
                var idProdPedProducaoEtiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(codigoEtiqueta) ?? ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducaoCanc(null, codigoEtiqueta);

                sql += idProdPedProducaoEtiqueta > 0 ? string.Format(" AND ppp.IdProdPedProducao={0}", idProdPedProducaoEtiqueta) : " AND 0=1";
                criterio += string.Format("Etiqueta: {0}    ", codigoEtiqueta);
            }

            if (!string.IsNullOrEmpty(codigoPedidoCliente))
            {
                sql += " AND (ped.CodCliente LIKE ?codigoPedidoCliente OR pp.PedCli LIKE ?codigoPedidoCliente OR a.Ambiente LIKE ?codigoPedidoCliente) ";
                criterio += string.Format("Pedido Cliente/Ambiente: {0}    ", codigoPedidoCliente);
            }

            if ((idsRota?.Any(f => f > 0)).GetValueOrDefault())
            {
                sql += string.Format(" AND ped.IdCli IN (SELECT * FROM (SELECT IdCliente FROM rota_cliente WHERE IdRota IN ({0})) AS temp1)", string.Join(",", idsRota));
                criterio += string.Format("Rota: {0}    ", RotaDAO.Instance.ObtemCodRotas(string.Join(",", idsRota)));
            }

            if (idImpressao > 0)
            {
                sql += string.Format(@" AND IF(!COALESCE(ppp.PecaReposta, 0), ppp.IdImpressao={0}, COALESCE(ppp.NumEtiqueta, ppp.NumEtiquetaCanc) IN (SELECT * FROM (
                    SELECT CONCAT(IdPedido, '-', PosicaoProd, '.', ItemEtiqueta, '/', QtdeProd)
                    FROM produto_impressao WHERE !COALESCE(Cancelado, 0) AND IdImpressao={0}) AS temp))", idImpressao);
                criterio += string.Format("Num. Impressão: {0}    ", idImpressao);
            }

            if (idCliente > 0)
            {
                sql += string.Format(" AND ped.IdCli={0}", idCliente);
                criterio += string.Format("Cliente: {0} - {1}    ", idCliente, ClienteDAO.Instance.GetNome((uint)idCliente));
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var idsCliente = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);

                sql += string.Format(" AND ped.IdCli IN ({0})", idsCliente);
                criterio += string.Format("Cliente: {0}    ", nomeCliente);
            }

            if (idFuncionario > 0)
            {
                sql += string.Format(" AND ped.IdFunc={0}", idFuncionario);
                criterio += string.Format("Funcionário: {0}    ", FuncionarioDAO.Instance.GetNome((uint)idFuncionario));
            }
            
            if (situacoes?.Count() > 0)
            {
                var filtroSituacoes = " AND (0=1 ";
                var produtoPedidoProducaoFiltroSituacao = new ProdutoPedidoProducao();

                foreach (var situacao in situacoes)
                {
                    switch (situacao)
                    {
                        case 1:
                        case 2:
                            filtroSituacoes += string.Format(" OR ppp.Situacao={0}", situacao);
                            produtoPedidoProducaoFiltroSituacao.Situacao = situacao;
                            criterio += string.Format("Situação: {0}    ", produtoPedidoProducaoFiltroSituacao.DescrSituacao);
                            break;
                        case 3:
                            filtroSituacoes += string.Format(" OR (ppp.SituacaoProducao={0} AND ppp.Situacao={1})", (int)SituacaoProdutoProducao.Pendente, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            criterio += "Tipo: Peças pendentes    ";
                            break;
                        case 4:
                            filtroSituacoes += string.Format(" OR (ppp.SituacaoProducao={0} AND ppp.Situacao={1})", (int)SituacaoProdutoProducao.Pronto, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            criterio += "Tipo: Peças prontas    ";
                            break;
                        case 5:
                            filtroSituacoes += string.Format(" OR (ppp.SituacaoProducao={0} AND ppp.Situacao={1})", (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            criterio += "Tipo: Peças entregues    ";
                            break;
                    }
                }

                filtroSituacoes += ")";
                sql += filtroSituacoes;
            }

            if (situacaoPedido > 0)
            {
                sql += string.Format(" AND ped.Situacao={0}", situacaoPedido);
                criterio += string.Format("Situação Pedido: {0}    ", PedidoDAO.Instance.GetSituacaoPedido(situacaoPedido));
            }

            sql += string.Format(" AND pp.IdProdPedParent IS NULL");

            var descricaoSetor = idSetor > 0 ? Utils.ObtemSetor((uint)idSetor).Descricao : idSetor == -1 ? "Etiqueta não impressa" : string.Empty;

            if (dataLeituraInicio > DateTime.MinValue)
            {
                var formatoCriterioDataLeituraInicio = dataLeituraInicio.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm:ss";

                if (situacoes?.Any(f => f == (int)ProdutoPedidoProducao.SituacaoEnum.Perda) ?? false)
                {
                    sql += " AND ppp.DataPerda>=?dataLeituraInicio";
                    criterio += string.Format("Data perda início: {0}    ", dataLeituraInicio.Value.ToString(formatoCriterioDataLeituraInicio));
                }

                if (idSetor > 0)
                {
                    sql += string.Format(" AND lp1.IdSetor={0} AND lp1.DataLeitura>=?dataLeituraInicio", idSetor);
                    criterio += string.Format("Data {0}: a partir de {1}    ", descricaoSetor, dataLeituraInicio.Value.ToString(formatoCriterioDataLeituraInicio));
                }
            }

            if (dataLeituraFim > DateTime.MinValue)
            {
                var formatoCriterioDataLeituraFim = dataLeituraFim.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm:ss";

                if (situacoes?.Any(f => f == (int)ProdutoPedidoProducao.SituacaoEnum.Perda) ?? false)
                {
                    sql += " AND ppp.DataPerda<=?dataLeituraFim";
                    criterio += string.Format("Data perda término: {0}    ", dataLeituraFim.Value.ToString(formatoCriterioDataLeituraFim));
                }

                if (idSetor > 0)
                {
                    sql += string.Format(" AND lp1.IdSetor={0} AND lp1.DataLeitura<=?dataLeituraFim", idSetor);
                    criterio += string.Format("Data {0}: até {1}    ", descricaoSetor, dataLeituraFim.Value.ToString(formatoCriterioDataLeituraFim));
                }
            }

            if (dataEntregaInicio > DateTime.MinValue)
            {
                var formatoCriterioDataEntregaInicio = dataEntregaInicio.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm:ss";

                sql += " AND ped.DataEntrega>=?dataEntregaInicio";
                criterio += string.Format("Data Entrega início: {0}    ", dataEntregaInicio.Value.ToString(formatoCriterioDataEntregaInicio));
            }

            if (dataEntregaFim > DateTime.MinValue)
            {
                var formatoCriterioDataEntregaFim = dataEntregaFim.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm:ss";

                sql += " AND ped.DataEntrega<=?dataEntregaFim";
                criterio += string.Format("Data Entrega término: {0}    ", dataEntregaFim.Value.ToString(formatoCriterioDataEntregaFim));
            }

            if (dataFabricaInicio > DateTime.MinValue)
            {
                var formatoCriterioDataFabricaInicio = dataFabricaInicio.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm:ss";

                sql += " AND pedEsp.DataFabrica>=?dataFabricaInicio";
                criterio += string.Format("Data fábrica início: {0}    ", dataFabricaInicio.Value.ToString(formatoCriterioDataFabricaInicio));
            }

            if (dataFabricaFim > DateTime.MinValue)
            {
                var formatoCriterioDataFabricaFim = dataFabricaFim.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm:ss";

                sql += " AND pedEsp.DataFabrica<=?dataFabricaFim";
                criterio += string.Format("Data fábrica término: {0}    ", dataFabricaFim.Value.ToString(formatoCriterioDataFabricaFim));
            }

            if (dataConfirmacaoPedidoInicio > DateTime.MinValue || dataConfirmacaoPedidoFim > DateTime.MinValue)
            {
                var idsPedido = string.Empty;

                if (dataConfirmacaoPedidoInicio > DateTime.MinValue)
                {
                    var formatoCriterioConfirmacaoPedidoInicio = dataConfirmacaoPedidoInicio.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm:ss";
                    criterio += string.Format("Data conf. ped. início: {0}    ", dataConfirmacaoPedidoInicio.Value.ToString(formatoCriterioConfirmacaoPedidoInicio));
                }

                if (dataConfirmacaoPedidoFim > DateTime.MinValue)
                {
                    var formatoCriterioDataConfirmacaoPedidoFim = dataConfirmacaoPedidoFim.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm:ss";
                    criterio += string.Format("Data conf. ped. término: {0}    ", dataConfirmacaoPedidoFim.Value.ToString(formatoCriterioDataConfirmacaoPedidoFim));
                }

                idsPedido = PedidoDAO.Instance.ObtemIdsPelaDataConf(DateTime.Parse(dataConfirmacaoPedidoInicio.Value.ToString("dd/MM/yyy 00:00:00")),
                    DateTime.Parse(dataConfirmacaoPedidoFim.Value.ToString("dd/MM/yyy 23:59:59")));

                if (!string.IsNullOrEmpty(idsPedido))
                {
                    sql += string.Format(" AND ped.IdPedido IN ({0})", idsPedido);
                }
            }

            if (idsSubgrupo != null && idsSubgrupo.Any(f => f > 0))
            {
                sql += string.Format(" AND p.IdSubgrupoProd IN ({0})", idsSubgrupo);                
                criterio += string.Format("Subgrupo(s): {0}    ", string.Join(", ", idsSubgrupo.Where(f => f > 0).Select(f => SubgrupoProdDAO.Instance.GetDescricao(f))));
            }

            if (tipoEntrega > 0)
            {
                sql += string.Format(" AND ped.TipoEntrega={0}", tipoEntrega);

                foreach (GenericModel te in DataSources.Instance.GetTipoEntrega())
                {
                    if (te.Id == tipoEntrega)
                    {
                        criterio += string.Format("Tipo Entrega: {0}    ", te.Descr);
                        break;
                    }
                }
            }

            if (tiposPedido?.Count() > 0)
            {
                var critetioTipoPedido = new List<string>();
                var filtroTiposPedido = new List<Pedido.TipoPedidoEnum>();
                
                if (tiposPedido.Any(f => f == 1))
                {
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.Venda);
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.Revenda);
                    critetioTipoPedido.Add("Venda/Revenda");
                }
                
                if (tiposPedido.Any(f => f == 2))
                {
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.Producao);
                    critetioTipoPedido.Add("Produção");
                }
                
                if (tiposPedido.Any(f => f == 3))
                {
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.MaoDeObra);
                    critetioTipoPedido.Add("Mão-de-obra");
                }
                
                if (tiposPedido.Any(f => f == 4))
                {
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.MaoDeObraEspecial);
                    critetioTipoPedido.Add("Mão-de-obra Especial");
                }

                sql += string.Format(" AND ped.TipoPedido IN ({0})", string.Join(",", filtroTiposPedido.Select(f => (int)f)));
                criterio += string.Format("Tipo Pedido: {0}    ", string.Join(", ", critetioTipoPedido.ToArray()));
            }

            if (altura > 0)
            {
                sql += string.Format(" AND IF(ped.TipoPedido={0}, a.Altura, IF(pp.AlturaReal > 0, pp.AlturaReal, pp.Altura))={1}", (int)Pedido.TipoPedidoEnum.MaoDeObra, altura);
                criterio += string.Format("Altura da peça: {0}    ", altura);
            }

            if (largura > 0)
            {
                sql += string.Format(" AND IF(ped.TipoPedido={0}, a.Largura, IF(pp.Redondo, 0, IF(pp.LarguraReal > 0, pp.LarguraReal, pp.Largura)))={1}",
                    (int)Pedido.TipoPedidoEnum.MaoDeObra, largura);
                criterio += string.Format("Largura da peça: {0}    ", largura);
            }
            
            if (espessura > 0)
            {
                sql += " AND p.Espessura=?espessura";
                criterio += string.Format("Espessura: {0}    ", espessura);
            }

            if (idsProcesso?.Any(f => f > 0) ?? false)
            {
                sql += string.Format(" AND pp.IdProcesso IN ({0})", string.Join(",", idsProcesso));
                criterio += string.Format("Processo: {0}    ", EtiquetaProcessoDAO.Instance.GetCodInternoByIds(string.Join(",", idsProcesso)));
            }

            if (idsAplicacao?.Any(f => f > 0) ?? false)
            {
                sql += string.Format(" AND pp.IdAplicacao IN ({0})", string.Join(",", idsAplicacao));
                criterio += string.Format("Aplicação: {0}    ", EtiquetaAplicacaoDAO.Instance.GetCodInternoByIds(string.Join(",", idsAplicacao)));
            }

            if (tipoRetorno == ProdutoPedidoProducaoDAO.TipoRetorno.EntradaEstoque)
            {
                sql += string.Format(" AND (ppp.EntrouEstoque IS NULL OR ppp.EntrouEstoque=0) AND ped.TipoPedido={0}", (int)Pedido.TipoPedidoEnum.Producao);
            }
            else if (tipoRetorno == ProdutoPedidoProducaoDAO.TipoRetorno.AguardandoExpedicao)
            {
                sql += string.Format(@" AND ped.TipoPedido<>{0} AND ped.IdPedido IN 
                    (SELECT * FROM (SELECT IdPedido FROM produtos_liberar_pedido plp 
                        LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido) 
                    WHERE lp.Situacao<>{1}) AS temp) AND ppp.SituacaoProducao<>{2} AND ppp.Situacao={3}",
                    (int)Pedido.TipoPedidoEnum.Producao, (int)LiberarPedido.SituacaoLiberarPedido.Cancelado, (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
            }

            if (pecasProducaoCanceladas?.Count() > 0)
            {
                var criterioPecasProducaoCanceladas = new List<string>();
                var filtroSituacoesProducao = new List<ProdutoPedidoProducao.SituacaoEnum>();

                if (pecasProducaoCanceladas.Any(f => f == 0))
                {
                    filtroSituacoesProducao.Add(ProdutoPedidoProducao.SituacaoEnum.Producao);
                    filtroSituacoesProducao.Add(ProdutoPedidoProducao.SituacaoEnum.Perda);
                    criterioPecasProducaoCanceladas.Add("em produção");
                }

                if (pecasProducaoCanceladas.Any(f => f == 1))
                {
                    filtroSituacoesProducao.Add(ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra);
                    criterioPecasProducaoCanceladas.Add("canceladas (mão-de-obra)");
                }

                if (pecasProducaoCanceladas.Any(f => f == 2))
                {
                    filtroSituacoesProducao.Add(ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda);
                    criterioPecasProducaoCanceladas.Add("canceladas (venda)");
                }

                sql += string.Format(" AND ppp.Situacao IN ({0})", string.Join(",", filtroSituacoesProducao.Select(f => (int)f)));
                criterio += string.Format("Peças {0}    ", string.Join(", ", criterioPecasProducaoCanceladas));
            }
            else
            {
                sql += " AND 0=1";
            }

            if (fastDelivery > 0)
            {
                sql += string.Format(" AND {0}", fastDelivery == 1 ? "ped.Fastdelivery IS NOT NULL AND ped.Fastdelivery=1" : "(ped.Fastdelivery IS NULL OR ped.Fastdelivery=0)");
                criterio += string.Format("Pedido(s) {0} fast delivery      ", fastDelivery == 1 ? "com" : "sem");
            }

            if (idSetor > 0 || idSetor == -1)
            {
                if (!setoresPosteriores && !setoresAnteriores)
                {
                    if (idSetor > 0)
                    {
                        sql += string.Format(" AND ppp.IdSetor={0}", idSetor);

                        // Filtro para impressão de etiqueta
                        if (Utils.ObtemSetor((uint)idSetor).NumeroSequencia == 1)
                        {
                            sql += string.Format(" AND EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0} AND DataLeitura IS NOT NULL)", idSetor);
                        }
                    }

                    // Etiqueta não impressa
                    else if (idSetor == -1)
                    {
                        sql += " AND ppp.IdSetor=1 AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND DataLeitura IS NOT NULL)";
                    }
                }
                else
                {
                    if (setoresAnteriores)
                    {
                        if (idSetor == 1)
                        {
                            sql += " AND ppp.IdSetor=1 AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND DataLeitura IS NOT NULL)";
                        }
                        else
                        {
                            sql += string.Format(" AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0})", idSetor);
                        }

                        // Retorna apenas as peças de roteiro se o setor for de roteiro
                        if (Utils.ObtemSetor((uint)idSetor).SetorPertenceARoteiro)
                        {
                            sql += string.Format(" AND EXISTS (SELECT * FROM roteiro_producao_etiqueta WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0})", idSetor);
                        }
                    }
                    else if (setoresPosteriores)
                    {
                        if (idSetor == 1)
                        {
                            sql += " AND EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND DataLeitura IS NOT NULL)";
                        }

                        sql += string.Format(@" AND {0} <= ALL (SELECT NumSeq FROM setor WHERE IdSetor=ppp.IdSetor)
                            AND (SELECT COUNT(*) FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={1}) > 0", Utils.ObtemSetor((uint)idSetor).NumeroSequencia, idSetor);
                    }
                }

                criterio += string.Format("Setor: {0}{1}    ", descricaoSetor, setoresAnteriores ? " (só produtos que ainda não passaram por este setor)" :
                    setoresPosteriores ? " (inclui produtos que já passaram por este setor)" : string.Empty);
            }

            if (usarJoin)
            {
                sql += " GROUP BY ppp.IdProdPedProducao";
            }

            #endregion

            sql = string.Format(@"SELECT IdPedido, IdPedidoExibir, IdSetor, COUNT(*) AS NumeroPecas, SUM(TotM2) AS TotM2, Criterio
                FROM ({0}) AS producao_contagem
                GROUP BY IdPedidoExibir, IdSetor", sql.Replace("$$$", criterio.Trim()));

            return sql;
        }

        /// <summary>
        /// Recupera a contagem da produção.
        /// </summary>
        public ProducaoContagem[] PesquisarProducaoContagemRelatorio(bool aguardandoEntradaEstoque, bool aguardandoExpedicao, int altura, string codigoEtiqueta, string codigoPedidoCliente,
            DateTime? dataConfirmacaoPedidoFim, DateTime? dataConfirmacaoPedidoInicio, DateTime? dataEntregaFim, DateTime? dataEntregaInicio, DateTime? dataFabricaFim, DateTime? dataFabricaInicio,
            DateTime? dataLeituraFim, DateTime? dataLeituraInicio, float espessura, int fastDelivery, int idCarregamento, int idCliente, int idFuncionario, int idImpressao, int idPedido,
            IEnumerable<int> idsAplicacao, int idSetor, IEnumerable<int> idsProcesso, IEnumerable<int> idsRota, IEnumerable<int> idsSubgrupo, int largura, string nomeCliente,
            IEnumerable<int> pecasProducaoCanceladas, int situacaoPedido, IEnumerable<int> situacoes, int tipoEntrega, int tipoSituacoes, IEnumerable<int> tiposPedido)
        {
            var setoresAnteriores = tipoSituacoes == 1;
            var setoresPosteriores = tipoSituacoes == 2;
            var tipoRetorno = aguardandoExpedicao ? ProdutoPedidoProducaoDAO.TipoRetorno.AguardandoExpedicao : aguardandoEntradaEstoque ? ProdutoPedidoProducaoDAO.TipoRetorno.EntradaEstoque :
                ProdutoPedidoProducaoDAO.TipoRetorno.Normal;

            var retorno = objPersistence.LoadData(SqlProducaoContagem(altura, codigoEtiqueta, codigoPedidoCliente, dataConfirmacaoPedidoFim, dataConfirmacaoPedidoInicio, dataEntregaFim,
                dataEntregaInicio, dataFabricaFim, dataFabricaInicio, dataLeituraFim, dataLeituraInicio, espessura, fastDelivery, idCarregamento, idCliente, idFuncionario, idImpressao, idPedido,
                idsAplicacao, idSetor, idsProcesso, idsRota, idsSubgrupo, largura, nomeCliente, pecasProducaoCanceladas, setoresAnteriores, setoresPosteriores, situacaoPedido, situacoes, tipoEntrega,
                tipoRetorno, tiposPedido),
                ObterParametrosProducaoContagem(codigoEtiqueta, dataLeituraFim, dataLeituraInicio, dataEntregaFim, dataEntregaInicio, dataFabricaFim, dataFabricaInicio, nomeCliente,
                    codigoPedidoCliente, espessura)).ToList();

            if (retorno.Count > 0)
            {
                foreach (var setor in Utils.GetSetores)
                {
                    var item = new ProducaoContagem();
                    item.IdPedido = retorno[0].IdPedido;
                    item.IdPedidoExibir = retorno[0].IdPedidoExibir;
                    item.IdSetor = (uint)setor.IdSetor;
                    item.NumeroPecas = 0;
                    item.TotM2 = 0;
                    retorno.Add(item);
                }
            }

            return retorno.ToArray();
        }

        /// <summary>
        /// Obtém os parâmetros da consulta de contagem de produção.
        /// </summary>
        internal GDAParameter[] ObterParametrosProducaoContagem(string codigoEtiqueta, DateTime? dataLeituraFim, DateTime? dataLeituraInicio, DateTime? dataEntregaFim, DateTime? dataEntregaInicio,
            DateTime? dataFabricaFim, DateTime? dataFabricaInicio, string nomeCliente, string codigoPedidoCliente, float espessura)
        {
            var parametros = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(codigoEtiqueta))
            {
                parametros.Add(new GDAParameter("?codEtiqueta", codigoEtiqueta));
            }

            if (!string.IsNullOrEmpty(codigoPedidoCliente))
            {
                parametros.Add(new GDAParameter("?codigoPedidoCliente", string.Format("%{0}%", codigoPedidoCliente)));
            }

            if (espessura > 0)
            {
                parametros.Add(new GDAParameter("?espessura", espessura));
            }

            if (dataEntregaFim > DateTime.MinValue)
            {
                var formatoDataEntregaFim = dataEntregaFim.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy 23:59:59" : "dd/MM/yyyy HH:mm:ss";
                parametros.Add(new GDAParameter("?dataEntregaFim", dataEntregaFim.Value.ToString(formatoDataEntregaFim)));
            }

            if (dataEntregaInicio > DateTime.MinValue)
            {
                parametros.Add(new GDAParameter("?dataEntregaInicio", dataEntregaInicio));
            }

            if (dataFabricaFim > DateTime.MinValue)
            {
                var formatoDataFabricaFim = dataFabricaFim.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy 23:59:59" : "dd/MM/yyyy HH:mm:ss";
                parametros.Add(new GDAParameter("?dataFabricaFim", dataFabricaFim.Value.ToString(formatoDataFabricaFim)));
            }

            if (dataFabricaInicio > DateTime.MinValue)
            {
                parametros.Add(new GDAParameter("?dataFabricaInicio", dataFabricaInicio));
            }

            if (dataLeituraFim > DateTime.MinValue)
            {
                var formatoDataLeituraFim = dataLeituraFim.Value.ToString("HH:mm:ss") == "00:00:00" ? "dd/MM/yyyy 23:59:59" : "dd/MM/yyyy HH:mm:ss";
                parametros.Add(new GDAParameter("?dataLeituraFim", dataLeituraFim.Value.ToString(formatoDataLeituraFim)));
            }

            if (dataLeituraInicio > DateTime.MinValue)
            {
                parametros.Add(new GDAParameter("?dataLeituraInicio", dataLeituraInicio));
            }

            if (!string.IsNullOrEmpty(nomeCliente))
            {
                parametros.Add(new GDAParameter("?nomeCliente", string.Format("%{0}%", nomeCliente)));
            }

            return parametros.Count > 0 ? parametros.ToArray() : null;
        }
    }
}
