using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoContagemDAO : BaseDAO<ProducaoContagem, ProducaoContagemDAO>
    {
        internal string SqlProducaoContagem(int altura, string codigoEtiqueta, string codigoPedidoCliente, string dataConfirmacaoPedidoFim, string dataConfirmacaoPedidoInicio,
            string dataEntregaFim, string dataEntregaInicio, string dataFabricaFim, string dataFabricaInicio, string dataLeituraFim, string dataLeituraInicio, float espessura, int fastDelivery,
            int idCarregamento, int idCliente, int idFuncionario, int idImpressao, int idPedido, string idsAplicacao, int idSetor, string idsProcesso, string idsRota, string idsSubgrupo, int largura,
            string nomeCliente, string pecasProducaoCanceladas, bool setoresAnteriores, bool setoresPosteriores, string situacao, int situacaoPedido, int tipoEntrega, string tipoPedido,
            ProdutoPedidoProducaoDAO.TipoRetorno tipoRetorno)
        {
            // Define se ao filtrar pela data de entrega será filtrado também pela data de fábrica
            var filtrarDataFabrica = Configuracoes.ProducaoConfig.BuscarDataFabricaConsultaProducao;
            var buscarNomeFantasia = Configuracoes.ProducaoConfig.TelaConsulta.BuscarNomeFantasiaConsultaProducao;
            var usarJoin = idSetor > 0 && (!string.IsNullOrEmpty(dataLeituraInicio) || !string.IsNullOrEmpty(dataLeituraFim));
            var campos = string.Empty;
            var sql = string.Empty;
            var filtroPedido = string.Empty;
            var criterio = string.Empty;
            var temp = new ProdutoPedidoProducao();

            campos = string.Format(@"
                pp.IdPedido, ppp.IdSetor,
                CONCAT(CAST(ped.IdPedido AS CHAR), IF(ped.IdPedidoAnterior IS NOT NULL, CONCAT(' (', CONCAT(CAST(ped.IdPedidoAnterior AS CHAR), 'R)')), ''),
                    IF(ppp.IdPedidoExpedicao IS NOT NULL, CONCAT(' (Exp. ', CAST(ppp.IdPedidoExpedicao AS CHAR), ')'), '')) AS IdPedidoExibir,
                ROUND(IF(ped.TipoPedido={0}, ((((50 - IF(MOD(a.Altura, 50) > 0, MOD(a.Altura, 50), 50)) + a.Altura) * ((50 - IF(MOD(a.Largura, 50) > 0, MOD(a.Largura, 50), 50)) + a.Largura)) / 1000000) *
                    a.Qtde, pp.TotM2Calc)/(pp.Qtde*IF(ped.TipoPedido={1}, a.Qtde, 1)), 4) AS TotM2", (int)Pedido.TipoPedidoEnum.MaoDeObra, (int)Pedido.TipoPedidoEnum.MaoDeObra);

            sql = string.Format(@"SELECT {0} FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN liberarpedido lp ON (ped.IdLiberarPedido = lp.IdLiberarPedido)
                    {1}
                    {2}
                WHERE 1 ?filtroAdicional?",
                campos,
                !string.IsNullOrEmpty(dataFabricaInicio) || !string.IsNullOrEmpty(dataFabricaFim) || filtrarDataFabrica ? "LEFT JOIN pedido_espelho pedEsp ON (ped.IdPedido = pedEsp.IdPedido)" : string.Empty,
                usarJoin ? "LEFT JOIN leitura_producao lp1 ON (ppp.IdProdPedProducao = lp1.IdProdPedProducao)" : string.Empty);
            
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
                filtroPedido += string.Format(" AND (ped.IdPedido={0}", idPedido);

                // Na vidrália/colpany não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira teria que mudar totalmente a forma de fazer o count.
                if (Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido && PedidoDAO.Instance.IsPedidoReposto((uint)idPedido))
                {
                    filtroPedido += string.Format(" OR ped.IdPedidoAnterior={0}", idPedido);
                }

                sql += filtroPedido;
                filtroPedido += ")";

                if (PedidoDAO.Instance.IsPedidoExpedicaoBox((uint)idPedido))
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
                filtroPedido += " AND ped.CodCliente LIKE ?codigoPedidoCliente";
                criterio += string.Format("Pedido Cliente/Ambiente: {0}    ", codigoPedidoCliente);
            }

            if (!string.IsNullOrEmpty(idsRota))
            {
                filtroPedido += string.Format(" AND ped.IdCli IN (SELECT * FROM (SELECT IdCliente FROM rota_cliente WHERE IdRota IN ({0})) AS temp1)", idsRota);
                sql += string.Format(" AND ped.IdCli IN (SELECT * FROM (SELECT IdCliente FROM rota_cliente WHERE IdRota IN ({0})) AS temp1)", idsRota);

                criterio += string.Format("Rota: {0}    ", RotaDAO.Instance.ObtemCodRotas(idsRota));
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
                filtroPedido += string.Format(" AND ped.IdCli={0}", idCliente);

                criterio += string.Format("Cliente: {0} - {1}    ", idCliente, ClienteDAO.Instance.GetNome((uint)idCliente));
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += string.Format(" AND ped.IdCli IN ({0})", ids);
                filtroPedido += string.Format(" AND ped.IdCli IN ({0})", ids);
                criterio += string.Format("Cliente: {0}    ", nomeCliente);
            }

            if (idFuncionario > 0)
            {
                sql += string.Format(" AND ped.IdFunc={0}", idFuncionario);
                filtroPedido += string.Format(" AND ped.IdFunc={0}", idFuncionario);
                criterio += string.Format("Funcionário: {0}    ", FuncionarioDAO.Instance.GetNome((uint)idFuncionario));
            }

            if (!string.IsNullOrEmpty(situacao))
            {
                var filtroSituacao = " AND (0=1";
                var situacoes = new List<string>(situacao.Split(','));

                foreach (string s in situacoes)
                {
                    switch (s)
                    {
                        case "1":
                        case "2":
                            filtroSituacao += string.Format(" OR ppp.Situacao={0}", s);
                            temp.Situacao = long.Parse(s);
                            criterio += string.Format("Situação: {0}    ", temp.DescrSituacao);
                            break;
                        case "3":
                            filtroSituacao += string.Format(" OR (ppp.SituacaoProducao={0} AND ppp.Situacao={1})", (int)SituacaoProdutoProducao.Pendente, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            criterio += "Tipo: Peças pendentes    ";
                            break;
                        case "4":
                            filtroSituacao += string.Format(" OR (ppp.SituacaoProducao={0} And ppp.Situacao={1})", (int)SituacaoProdutoProducao.Pronto, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            criterio += "Tipo: Peças prontas    ";
                            break;
                        case "5":
                            filtroSituacao += string.Format(" OR (ppp.SituacaoProducao={0} And ppp.Situacao={1})", (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            criterio += "Tipo: Peças entregues    ";
                            break;
                    }
                }

                filtroSituacao += ")";

                sql += filtroSituacao;
            }

            if (situacaoPedido > 0)
            {
                sql += string.Format(" AND ped.Situacao={0}", situacaoPedido);
                filtroPedido += string.Format(" AND ped.Situacao={0}", situacaoPedido);
                criterio += string.Format("Situação Pedido: {0}    ", PedidoDAO.Instance.GetSituacaoPedido(situacaoPedido));
            }

            sql += string.Format(" AND pp.IdProdPedParent IS NULL");

            var descricaoSetor = idSetor > 0 ? Utils.ObtemSetor((uint)idSetor).Descricao :
                idSetor == -1 ? "Etiqueta não impressa" : string.Empty;

            if (!string.IsNullOrEmpty(dataLeituraInicio))
            {
                if (string.Format(",{0},", situacao).Contains(string.Format(",{0},", (int)ProdutoPedidoProducao.SituacaoEnum.Perda)))
                {
                    sql += " AND ppp.DataPerda>=?dataLeituraInicio";
                    criterio += string.Format("Data perda início: {0}    ", dataLeituraInicio);
                }
                else if (idSetor > 0)
                {
                    sql += string.Format(" AND lp1.IdSetor={0} AND lp1.DataLeitura>=?dataLeituraInicio", idSetor);
                    criterio += string.Format("Data {0}: a partir de {1}    ", descricaoSetor, dataLeituraInicio);
                }
            }

            if (!string.IsNullOrEmpty(dataLeituraFim))
            {
                if ((string.Format(",{0},", situacao)).Contains(string.Format(",{0},", (int)ProdutoPedidoProducao.SituacaoEnum.Perda)))
                {
                    sql += " AND ppp.DataPerda<=?dataLeituraFim";
                    criterio += string.Format("Data perda término: {0}    ", dataLeituraFim);
                }
                else if (idSetor > 0)
                {
                    sql += string.Format(" AND lp1.IdSetor={0} AND lp1.DataLeitura<=?dataLeituraFim", idSetor);
                    criterio += string.Format("Data {0}: até {1}    ", descricaoSetor, dataLeituraFim);
                }
            }

            if (!string.IsNullOrEmpty(dataEntregaInicio))
            {
                sql += " AND ped.DataEntrega>=?dataEntregaInicio";
                filtroPedido += " AND ped.DataEntrega>=?dataEntregaInicio";
                criterio += string.Format("Data Entrega início: {0}    ", dataEntregaInicio);
            }

            if (!string.IsNullOrEmpty(dataEntregaFim))
            {
                sql += " AND ped.DataEntrega<=?dataEntregaFim";
                filtroPedido += " AND ped.DataEntrega<=?dataEntregaFim";
                criterio += string.Format("Data Entrega término: {0}    ", dataEntregaFim);
            }

            if (!string.IsNullOrEmpty(dataFabricaInicio))
            {
                sql += " AND pedEsp.DataFabrica>=?dataFabricaInicio";
                filtroPedido += " AND pedEsp.DataFabrica>=?dataFabricaInicio";
                criterio += string.Format("Data fábrica início: {0}    ", dataFabricaInicio);
            }

            if (!string.IsNullOrEmpty(dataFabricaFim))
            {
                sql += " AND pedEsp.DataFabrica<=?dataFabricaFim";
                filtroPedido += " AND pedEsp.DataFabrica<=?dataFabricaFim";
                criterio += string.Format("Data fábrica término: {0}    ", dataFabricaFim);
            }

            if (!string.IsNullOrEmpty(dataConfirmacaoPedidoInicio) || !string.IsNullOrEmpty(dataConfirmacaoPedidoFim))
            {
                string idsPedido;
                DateTime? dataIniConfPedUsar, dataFimConfPedUsar;

                if (!string.IsNullOrEmpty(dataConfirmacaoPedidoInicio))
                {
                    dataIniConfPedUsar = DateTime.Parse(dataConfirmacaoPedidoInicio);
                    criterio += string.Format("Data conf. ped. início: {0}    ", dataConfirmacaoPedidoInicio);
                }
                else
                {
                    dataIniConfPedUsar = null;
                }

                if (!string.IsNullOrEmpty(dataConfirmacaoPedidoFim))
                {
                    dataFimConfPedUsar = DateTime.Parse(string.Format("{0} 23:59:59", dataConfirmacaoPedidoFim));
                    criterio += string.Format("Data conf. ped. término: {0}    ", dataConfirmacaoPedidoFim);
                }
                else
                {
                    dataFimConfPedUsar = null;
                }

                idsPedido = PedidoDAO.Instance.ObtemIdsPelaDataConf(dataIniConfPedUsar, dataFimConfPedUsar);

                if (!string.IsNullOrEmpty(idsPedido))
                {
                    sql += string.Format(" AND ped.IdPedido IN ({0})", idsPedido);
                    filtroPedido += string.Format(" AND ped.IdPedido IN ({0})", idsPedido);
                }
            }

            if (!string.IsNullOrEmpty(idsSubgrupo) && idsSubgrupo != "0")
            {
                var descricaoSubgrupos = string.Empty;
                sql += string.Format(" AND p.IdSubgrupoProd IN ({0})", idsSubgrupo);

                foreach (var id in idsSubgrupo.Split(','))
                {
                    descricaoSubgrupos += string.Format("{0}, ", SubgrupoProdDAO.Instance.GetDescricao(id.StrParaInt()));
                }

                criterio += string.Format("Subgrupo(s): {0}    ", descricaoSubgrupos.TrimEnd(' ', ','));
            }

            if (tipoEntrega > 0)
            {
                sql += string.Format(" AND ped.TipoEntrega={0}", tipoEntrega);
                filtroPedido += string.Format(" AND ped.TipoEntrega={0}", tipoEntrega);

                foreach (GenericModel te in DataSources.Instance.GetTipoEntrega())
                {
                    if (te.Id == tipoEntrega)
                    {
                        criterio += string.Format("Tipo Entrega: {0}    ", te.Descr);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(tipoPedido) && tipoPedido != "0")
            {
                var tiposPedido = new List<int>();
                var critetioTipoPedido = new List<string>();

                tipoPedido = string.Format(",{0},", tipoPedido);

                if (tipoPedido.Contains(",1,"))
                {
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.Venda);
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.Revenda);
                    critetioTipoPedido.Add("Venda/Revenda");
                }

                if (tipoPedido.Contains(",2,"))
                {
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.Producao);
                    critetioTipoPedido.Add("Produção");
                }

                if (tipoPedido.Contains(",3,"))
                {
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.MaoDeObra);
                    critetioTipoPedido.Add("Mão-de-obra");
                }

                if (tipoPedido.Contains(",4,"))
                {
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.MaoDeObraEspecial);
                    critetioTipoPedido.Add("Mão-de-obra Especial");
                }

                sql += string.Format(" AND ped.TipoPedido IN ({0})", string.Join(",", tiposPedido.Select(f => f.ToString()).ToArray()));
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

            if (!string.IsNullOrEmpty(idsProcesso))
            {
                sql += string.Format(" AND pp.IdProcesso IN ({0})", idsProcesso);
                criterio += string.Format("Processo: {0}    ", EtiquetaProcessoDAO.Instance.GetCodInternoByIds(idsProcesso));
            }

            if (!string.IsNullOrEmpty(idsAplicacao))
            {
                sql += string.Format(" AND pp.IdAplicacao IN ({0})", idsAplicacao);
                criterio += string.Format("Aplicação: {0}    ", EtiquetaAplicacaoDAO.Instance.GetCodInternoByIds(idsAplicacao));
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

            if (!string.IsNullOrEmpty(pecasProducaoCanceladas))
            {
                var criterioPecasProd = string.Empty;
                var situacaoProd = string.Empty;

                if ((string.Format(",{0},", pecasProducaoCanceladas)).Contains(",0,"))
                {
                    situacaoProd += string.Format(",{0},{1}", (int)ProdutoPedidoProducao.SituacaoEnum.Producao, (int)ProdutoPedidoProducao.SituacaoEnum.Perda);
                    criterioPecasProd += ", em produção";
                }

                if ((string.Format(",{0},", pecasProducaoCanceladas)).Contains(",1,"))
                {
                    situacaoProd += string.Format(",{0}", (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra);
                    criterioPecasProd += ", canceladas (mão-de-obra)";
                }

                if ((string.Format(",{0},", pecasProducaoCanceladas)).Contains(",2,"))
                {
                    situacaoProd += string.Format(",{0}", (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda);
                    criterioPecasProd += ", canceladas (venda)";
                }

                sql += string.Format(" AND ppp.Situacao IN ({0})", situacaoProd.TrimStart(','));
                criterio += string.Format("Peças {0}    ", criterioPecasProd.Substring(", ".Length));
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

            sql.Replace("$$$", criterio.Trim());

            sql = string.Format(@"SELECT IdPedido, IdPedidoExibir, IdSetor, COUNT(*) AS NumeroPecas, SUM(TotM2) AS TotM2, Criterio
                FROM ({0}) AS producao_contagem
                GROUP BY IdPedidoExibir, IdSetor", sql);

            return sql;
        }

        public ProducaoContagem[] PesquisarProducaoContagemRelatorio(bool aguardandoEntradaEstoque, bool aguardandoExpedicao, int altura, string codigoEtiqueta, string codigoPedidoCliente, string dataConfirmacaoPedidoFim,
            string dataConfirmacaoPedidoInicio, string dataEntregaFim, string dataEntregaInicio, string dataFabricaFim, string dataFabricaInicio, string dataLeituraFim, string dataLeituraInicio,
            float espessura, int fastDelivery, int idCarregamento, int idCliente, int idFuncionario, int idImpressao, int idPedido, string idsAplicacao, int idSetor, string idsProcesso,
            string idsRota, string idsSubgrupo, int largura, string nomeCliente, string pecasProducaoCanceladas, string situacao, int situacaoPedido, int tipoEntrega, string tipoPedido,
            int tipoSituacoes)
        {
            var setoresAnteriores = tipoSituacoes == 1;
            var setoresPosteriores = tipoSituacoes == 2;
            var tipoRetorno = aguardandoExpedicao ? ProdutoPedidoProducaoDAO.TipoRetorno.AguardandoExpedicao : aguardandoEntradaEstoque ? ProdutoPedidoProducaoDAO.TipoRetorno.EntradaEstoque :
                ProdutoPedidoProducaoDAO.TipoRetorno.Normal;

            var retorno = objPersistence.LoadData(SqlProducaoContagem(altura, codigoEtiqueta, codigoPedidoCliente, dataConfirmacaoPedidoFim, dataConfirmacaoPedidoInicio, dataEntregaFim,
                dataEntregaInicio, dataFabricaFim, dataFabricaInicio, dataLeituraFim, dataLeituraInicio, espessura, fastDelivery, idCarregamento, idCliente, idFuncionario, idImpressao, idPedido,
                idsAplicacao, idSetor, idsProcesso, idsRota, idsSubgrupo, largura, nomeCliente, pecasProducaoCanceladas, setoresAnteriores, setoresPosteriores, situacao, situacaoPedido, tipoEntrega,
                tipoPedido, tipoRetorno),
                ProdutoPedidoProducaoDAO.Instance.GetParam(null, codigoEtiqueta, idsRota, dataLeituraInicio, dataLeituraFim, dataEntregaInicio, dataEntregaFim, dataFabricaInicio, dataFabricaFim,
                    nomeCliente, codigoPedidoCliente, null, null, espessura)).ToList();

            if (retorno.Count > 0)
            {
                foreach (var s in Utils.GetSetores)
                {
                    var item = new ProducaoContagem();
                    item.IdPedido = retorno[0].IdPedido;
                    item.IdPedidoExibir = retorno[0].IdPedidoExibir;
                    item.IdSetor = (uint)s.IdSetor;
                    item.NumeroPecas = 0;
                    item.TotM2 = 0;
                    retorno.Add(item);
                }
            }

            return retorno.ToArray();
        }
    }
}
