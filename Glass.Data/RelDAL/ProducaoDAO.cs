using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoDAO : BaseDAO<Producao, ProducaoDAO>
    {
        //private ProducaoDAO() { }

        internal string SqlSetores(bool incluirPerda, bool dataAsString, string filtro, GDAParameter[] parametros, bool selecionar)
        {
            var sql = string.Empty;
            var campoData = string.Empty;
            var campos = string.Empty;
            var numeroSetorPerda = 0;

            sql = string.Format(@"
                SELECT lp1.IdProdPedProducao FROM leitura_producao lp1
                    LEFT JOIN produto_pedido_producao ppp1 ON (lp1.IdProdPedProducao=ppp1.IdProdPedProducao)
                    LEFT JOIN setor s1 ON (lp1.IdSetor=s1.IdSetor)
                    LEFT JOIN produtos_pedido_espelho ppe1 ON (ppp1.IdProdPed=ppe1.IdProdPed)
                    LEFT JOIN produto p1 ON (ppe1.IdProd=p1.IdProd)
                    LEFT JOIN pedido ped1 ON (ppe1.IdPedido=ped1.IdPedido)
                    LEFT JOIN cliente c1 ON (ped1.IdCli=c1.Id_Cli)
                WHERE 1 {0}", filtro);

            filtro = GetValoresCampo(sql, "IdProdPedProducao", parametros);

            if (string.IsNullOrWhiteSpace(filtro))
            {
                filtro = "0";
            }

            campoData = dataAsString ? "DATE_FORMAT({0}, '%d-%m-%Y')" : "{0}";
            campos = selecionar ? string.Format(@"dados.IdProdPedProducao, dados.IdSetor, dados.TipoSetor, dados.NumSeqSetor, dados.NomeSetor, dados.SituacaoProducao, {0} AS DataSetor, dados.IdFunc,
                ppe1.IdPedido", string.Format(campoData, "dados.DataLeitura")) : "dados.IdProdPedProducao, ppe1.IdPedido, dados.IdSetor";
            numeroSetorPerda = Utils.GetSetores.Max(x => x.NumeroSequencia) + 1;

            sql = string.Format(@"
                SELECT {1} FROM (
                    SELECT * FROM (
                        SELECT d.*, lp.DataLeitura, lp.IdFuncLeitura AS IdFunc FROM (
                            SELECT ppp.IdProdPedProducao, s.IdSetor, s.Tipo AS TipoSetor, s.NumSeq AS NumSeqSetor, s.Descricao AS NomeSetor, ppp.IdProdPed, ppp.SituacaoProducao
                            FROM produto_pedido_producao ppp, setor s
		                    WHERE ppp.IdProdPedProducao IN ({0}) AND s.Situacao={2}
                        ) AS d
                            LEFT JOIN leitura_producao lp ON (d.IdProdPedProducao=lp.IdProdPedProducao AND d.IdSetor=lp.IdSetor)
                        
                        UNION ALL

                        SELECT IdProdPedProducao, NULL AS IdSetor, NULL AS TipoSetor, CAST({3} AS SIGNED) AS NumSeqSetor, 'Perda' AS NomeSetor, IdProdPed, NULL AS SituacaoProducao,
                            DataPerda AS DataLeitura, IdFuncPerda AS IdFunc
                        FROM produto_pedido_producao
                        WHERE IdProdPedProducao IN ({0})
                    ) AS temp
                    ORDER BY IdProdPedProducao, NumSeqSetor
                ) dados
                    LEFT JOIN produtos_pedido_espelho ppe1 ON (dados.IdProdPed=ppe1.IdProdPed)", "{0}", campos, (int)Situacao.Ativo, numeroSetorPerda);

            return string.Format(sql, filtro);
        }

        /// <summary>
        /// SQL da consulta que retorna os dados de produção para o relatório de produção.
        /// </summary>
        private string SqlProducao(bool aguardandoEntradaEstoque, bool aguardandoExpedicao, int altura, string codigoEtiqueta, string codigoPedidoCliente, string codigoRota, DateTime? dataEntregaFim,
            DateTime? dataEntregaInicio, DateTime? dataLeituraFim, DateTime? dataLeituraInicio, int idCarregamento, int idCliente, int idCorVidro, int idFuncionario, int idImpressao,
            int idLiberarPedido, int idPedido, int idPedidoImportado, int idSetor, int idSubgrupo, int largura, string nomeCliente, bool pecasCanceladas, bool selecionar, bool setoresAnteriores,
            bool setoresPosteriores, int situacao, int tipoEntrega, IEnumerable<int> tiposPedido)
        {
            #region Declaração de variáveis

            var filtro = string.Empty;
            var filtroInterno = string.Empty;
            var criterio = string.Empty;
            var descricaoSetor = string.Empty;
            var camposUnion = string.Empty;
            var sql = string.Empty;
            var temp = new ProdutoPedidoProducao();

            #endregion

            #region Filtros

            if (idCarregamento > 0)
            {
                filtro += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento where IdCarregamento={0})", idCarregamento);
                filtroInterno += string.Format(" AND ppp1.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento where IdCarregamento={0})", idCarregamento);
                criterio += string.Format("Carregamento: {0}    ", idCarregamento);
            }

            if (idLiberarPedido > 0)
            {
                filtro += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM produtos_liberar_pedido WHERE IdLiberarPedido={0})", idLiberarPedido);
                filtroInterno += string.Format(" AND ppp1.IdProdPedProducao IN (SELECT IdProdPedProducao FROM produtos_liberar_pedido WHERE IdLiberarPedido={0})", idLiberarPedido);
                criterio += string.Format("Liberação: {0}    ", idLiberarPedido);
            }

            if (idPedido > 0)
            {
                filtro += string.Format(" AND (ped.IdPedido={0} OR ped.IdPedidoAnterior={0} OR ppp.IdPedidoExpedicao={0})", idPedido);
                filtroInterno += string.Format(" AND (ped1.IdPedido={0} OR ped1.IdPedidoAnterior={0} OR ppp1.IdPedidoExpedicao={0})", idPedido);
                criterio += string.Format("Pedido: {0}    ", idPedido);
            }

            if (idPedidoImportado > 0)
            {
                filtro += " AND ped.CodCliente=?idPedidoImportado";
                filtroInterno += " AND ped1.CodCliente=?idPedidoImportado";
                criterio += string.Format("Pedido importado: {0}    ", idPedidoImportado);
            }

            if (idImpressao > 0)
            {
                filtro += string.Format(" AND ppe.IdProdPed IN (SELECT IdProdPed FROM produto_impressao WHERE IdImpressao={0})", idImpressao);
                filtroInterno += string.Format(" AND ppe1.IdProdPed IN (SELECT IdProdPed FROM produto_impressao WHERE IdImpressao={0})", idImpressao);
                criterio += string.Format("Impressão: {0}    ", idImpressao);
            }

            if (!string.IsNullOrEmpty(codigoPedidoCliente))
            {
                filtro += " AND (ped.CodCliente LIKE ?codigoPedidoCliente OR ppe.PedCli LIKE ?codigoPedidoCliente)";
                filtroInterno += " AND (ped1.CodCliente LIKE ?codigoPedidoCliente OR ppe1.PedCli LIKE ?codigoPedidoCliente)";
                criterio += string.Format("Ped. Cli: {0}    ", codigoPedidoCliente);
            }

            if (!string.IsNullOrEmpty(codigoRota))
            {
                filtro += " AND ped.IdCli IN (SELECT IdCliente FROM rota_cliente WHERE IdRota IN (SELECT IdRota FROM rota WHERE CodInterno LIKE ?codigoRota))";                
                filtroInterno += " AND ped1.IdCli IN (SELECT IdCliente FROM rota_cliente WHERE IdRota IN (SELECT IdRota FROM rota WHERE CodInterno LIKE ?codigoRota))";
                criterio += string.Format("Rota: {0}    ", codigoRota);
            }

            if (idCliente > 0)
            {
                filtro += string.Format(" AND ped.IdCli={0}", idCliente);
                filtroInterno += string.Format(" AND ped1.IdCli={0}", idCliente);
                criterio += string.Format("Cliente: {0}    ", ClienteDAO.Instance.GetNome((uint)idCliente));
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var idsCliente = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);

                filtro += string.Format(" AND c.Id_Cli IN ({0})", idsCliente);
                filtroInterno += string.Format(" AND c1.Id_Cli IN ({0})", idsCliente);
                criterio += string.Format("Cliente: {0}    ", nomeCliente);
            }

            if (!string.IsNullOrEmpty(codigoEtiqueta))
            {
                filtro += " AND ppp.NumEtiqueta=?codigoEtiqueta";
                filtroInterno += " AND ppp1.NumEtiqueta=?codigoEtiqueta";
                criterio += string.Format("Etiqueta: {0}    ", codigoEtiqueta);
            }

            if (idFuncionario > 0)
            {
                filtro += string.Format(" AND ped.IdFunc={0}", idFuncionario);
                filtroInterno += string.Format(" AND ped1.IdFunc={0}", idFuncionario);
                criterio += string.Format("Funcionário: {0}    ", FuncionarioDAO.Instance.GetNome((uint)idFuncionario));
            }

            if (situacao > 0)
            {
                if (situacao <= 2)
                {
                    filtro += string.Format(" AND ppp.Situacao={0}", situacao);
                    filtroInterno += string.Format(" AND ppp1.Situacao={0}", situacao);
                    temp.Situacao = situacao;
                    criterio += string.Format("Situação: {0}    ", temp.DescrSituacao);
                }
                else
                {
                    var filtroSituacao = situacao == 3 ? (int)SituacaoProdutoProducao.Pendente : situacao == 4 ? (int)SituacaoProdutoProducao.Pronto : situacao == 5 ? (int)SituacaoProdutoProducao.Entregue : 0;

                    filtro += string.Format(" AND ppp.Situacao={0} AND dados.SituacaoProducao IN ({1})", (int)ProdutoPedidoProducao.SituacaoEnum.Producao, filtroSituacao);
                    filtroInterno += string.Format(" AND ppp1.Situacao={0} AND ppp1.SituacaoProducao IN ({1})", (int)ProdutoPedidoProducao.SituacaoEnum.Producao, filtroSituacao);                                        
                    criterio += string.Format("Situação: {0}    ", situacao == 3 ? "Pendente" : situacao == 4 ? "Pronta" : situacao == 5 ? "Entregue" : string.Empty);
                }
            }

            descricaoSetor = idSetor > 0 ? Utils.ObtemSetor((uint)idSetor).Descricao : idSetor == -1 ? "Etiqueta não impressa" : string.Empty;

            if (idSetor > 0 || idSetor == -1)
            {
                if (!setoresPosteriores && !setoresAnteriores)
                {
                    if (idSetor > 0)
                    {
                        filtro += string.Format(" AND ppp.IdSetor={0}", idSetor);
                        filtroInterno += string.Format(" AND ppp1.IdSetor={0}", idSetor);

                        // Filtro para impressão de etiqueta
                        if (Utils.ObtemSetor((uint)idSetor).NumeroSequencia == 1)
                        {
                            filtro += string.Format(" AND EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0})", idSetor);
                            filtroInterno += string.Format(" AND EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp1.IdProdPedProducao AND IdSetor={0})", idSetor);
                        }
                    }
                    // Etiqueta não impressa
                    else if (idSetor == -1)
                    {
                        filtro += " AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao)";
                        filtroInterno += " AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp1.IdProdPedProducao)";
                    }
                }
                else
                {
                    if (setoresAnteriores)
                    {
                        if (idSetor == 1)
                        {
                            filtro += " AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao)";
                            filtroInterno += " AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp1.IdProdPedProducao)";
                        }
                        else
                        {
                            filtro += string.Format(" AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0})", idSetor);
                            filtroInterno += string.Format(" And not exists (select * from leitura_producao where idProdPedProducao=ppp1.idProdPedProducao and idSetor={0})", idSetor);
                        }

                        // Retorna apenas as peças de roteiro se o setor for de roteiro
                        if (Utils.ObtemSetor((uint)idSetor).SetorPertenceARoteiro)
                        {
                            filtro += string.Format(" AND EXISTS (SELECT * FROM roteiro_producao_etiqueta WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0})", idSetor);
                            filtroInterno += string.Format(" AND EXISTS (SELECT * FROM roteiro_producao_etiqueta WHERE IdProdPedProducao=ppp1.IdProdPedProducao AND IdSetor={0})", idSetor);
                        }
                    }
                    else if (setoresPosteriores)
                    {
                        filtro += string.Format(" AND {0} <= ALL (SELECT NumSeq FROM setor WHERE IdSetor=ppp.IdSetor) ", Utils.ObtemSetor((uint)idSetor).NumeroSequencia);
                        filtroInterno += string.Format(" AND {0} <= ALL (SELECT NumSeq FROM setor WHERE IdSetor=ppp1.IdSetor) ", Utils.ObtemSetor((uint)idSetor).NumeroSequencia);
                    }
                }

                criterio += string.Format("Setor: {0}{1}    ", descricaoSetor, setoresAnteriores ? " (só produtos que ainda não passaram por este setor)" :
                    setoresPosteriores ? " (inclui produtos que já passaram por este setor)" : string.Empty);
            }

            if (pecasCanceladas)
            {
                filtro += string.Format(" AND ped.Situacao={0}", (int)Pedido.SituacaoPedido.Cancelado);
                filtroInterno += string.Format(" AND ped1.Situacao={0}", (int)Pedido.SituacaoPedido.Cancelado);
                criterio += "Peças canceladas    ";
            }
            else
            {
                filtro += string.Format(" AND ped.Situacao<>{0}", (int)Pedido.SituacaoPedido.Cancelado);
                filtroInterno += string.Format(" AND ped1.Situacao<>{0}", (int)Pedido.SituacaoPedido.Cancelado);
            }

            if (dataLeituraInicio.HasValue && dataLeituraInicio > DateTime.MinValue)
            {
                if (situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
                {
                    filtro += " AND ppp.DataPerda>=?dataLeituraInicio";
                    filtroInterno += " AND ppp1.DataPerda>=?dataLeituraInicio";
                    criterio += string.Format("Data perda início: {0}    ", dataLeituraInicio.Value.ToString("dd/MM/yyyy"));
                }
                else if (idSetor > 0)
                {
                    filtro += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM leitura_producao WHERE IdSetor={0} AND DataLeitura>=?dataLeituraInicio)", idSetor);
                    filtroInterno += string.Format(" AND ppp1.IdProdPedProducao IN (SELECT IdProdPedProducao FROM leitura_producao WHERE IdSetor={0} AND DataLeitura>=?dataLeituraInicio)", idSetor);
                    criterio += string.Format("Data {0} início: {1}    ", descricaoSetor, dataLeituraInicio.Value.ToString("dd/MM/yyyy"));
                }
            }

            if (dataLeituraFim.HasValue && dataLeituraFim > DateTime.MinValue)
            {
                if (situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
                {
                    filtro += " AND ppp.DataPerda<=?dataLeituraFim";
                    filtroInterno += " AND ppp1.DataPerda<=?dataLeituraFim";
                    criterio += string.Format("Data perda término: {0}    ", dataLeituraFim.Value.ToString("dd/MM/yyyy"));
                }
                else if (idSetor > 0)
                {
                    filtro = dataLeituraInicio.HasValue && dataLeituraInicio > DateTime.MinValue ? string.Format("{0} AND DataLeitura<=?dataLeituraFim)", filtro.TrimEnd(')')) :
                        string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM leitura_producao WHERE IdSetor={0} AND DataLeitura<=?dataLeituraFim)", idSetor);
                    filtroInterno = dataLeituraInicio.HasValue && dataLeituraInicio > DateTime.MinValue ? string.Format("{0} AND DataLeitura<=?dataLeituraFim)", filtroInterno.TrimEnd(')')) :
                        string.Format(" AND ppp1.IdProdPedProducao IN (SELECT IdProdPedProducao FROM leitura_producao WHERE IdSetor={0} AND DataLeitura<=?dataLeituraFim)", idSetor);
                    criterio += string.Format("Data {0} início: {1}    ", descricaoSetor, dataLeituraFim.Value.ToString("dd/MM/yyyy"));
                }
            }

            if (dataEntregaInicio.HasValue && dataEntregaInicio > DateTime.MinValue)
            {
                filtro += " AND ped.DataEntrega>=?dataEntregaInicio";
                filtroInterno += " AND ped1.DataEntrega>=?dataEntregaInicio";
                criterio += string.Format("Data entrega início: {0}    ", dataEntregaInicio.Value.ToString("dd/MM/yyyy"));
            }

            if (dataEntregaFim.HasValue && dataEntregaFim > DateTime.MinValue)
            {
                filtro += " AND ped.DataEntrega<=?dataEntregaFim";
                filtroInterno += " AND ped1.DataEntrega<=?dataEntregaFim";
                criterio += string.Format("Data Entrega término: {0}    ", dataEntregaFim.Value.ToString("dd/MM/yyyy"));
            }

            if (idSubgrupo > 0)
            {
                filtro += string.Format(" AND p.IdSubgrupoProd={0}", idSubgrupo);
                filtroInterno += string.Format(" AND p1.IdSubgrupoProd={0}", idSubgrupo);
                criterio += string.Format("Subgrupo: {0}    ", SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo));
            }

            if (tipoEntrega > 0)
            {
                filtro += string.Format(" AND ped.TipoEntrega={0}", tipoEntrega);
                filtroInterno += string.Format(" AND ped1.TipoEntrega={0}", tipoEntrega);

                foreach (var te in DataSources.Instance.GetTipoEntrega())
                {
                    if (te.Id == tipoEntrega)
                    {
                        criterio += string.Format("Tipo Entrega: {0}    ", te.Descr);
                        break;
                    }
                }
            }

            if (aguardandoEntradaEstoque)
            {
                filtro += string.Format(" AND (EntrouEstoque IS NULL OR EntrouEstoque=0) AND ped.TipoPedido={0}", (int)Pedido.TipoPedidoEnum.Producao);

                if (tipoEntrega > 0)
                {
                    filtroInterno += string.Format(" AND ped1.TipoEntrega={0}", tipoEntrega);
                }

                criterio += "Aguardando entrada no estoque    ";
            }
            else if (aguardandoExpedicao)
            {
                filtro += string.Format(@" AND ped.TipoPedido<>{0} AND ped.IdPedido IN
                        (SELECT * FROM (SELECT IdPedido FROM produtos_liberar_pedido plp
                            LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido) 
                        WHERE lp.Situacao<>{1}) AS temp) AND
                    dados.SituacaoProducao<>{2} AND ppp.Situacao={3}",
                    (int)Pedido.TipoPedidoEnum.Producao, (int)LiberarPedido.SituacaoLiberarPedido.Cancelado, (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

                filtroInterno += string.Format(@" AND ped1.TipoPedido<>{0} AND ped1.IdPedido IN
                    (SELECT * FROM (SELECT IdPedido FROM produtos_liberar_pedido plp 
                    LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido) 
                    WHERE lp.Situacao<>{1}) AS temp1)
                    AND ppp1.SituacaoProducao<>{2} AND ppp1.Situacao={3}",
                    (int)Pedido.TipoPedidoEnum.Producao, (int)LiberarPedido.SituacaoLiberarPedido.Cancelado, (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

                criterio += "Aguardando expedição    ";
            }

            if (tiposPedido?.Count() > 0)
            {
                var critetioTipoPedido = new List<string>();
                var tiposPedidoFiltrar = new List<Pedido.TipoPedidoEnum>();
                
                if (tiposPedido.Any(f => f == 1))
                {
                    tiposPedidoFiltrar.Add(Pedido.TipoPedidoEnum.Venda);
                    tiposPedidoFiltrar.Add(Pedido.TipoPedidoEnum.Revenda);
                    critetioTipoPedido.Add("Venda/Revenda");
                }

                if (tiposPedido.Any(f => f == 2))
                {
                    tiposPedidoFiltrar.Add(Pedido.TipoPedidoEnum.Producao);
                    critetioTipoPedido.Add("Produção");
                }

                if (tiposPedido.Any(f => f == 3))
                {
                    tiposPedidoFiltrar.Add(Pedido.TipoPedidoEnum.MaoDeObra);
                    critetioTipoPedido.Add("Mão-de-obra");
                }

                if (tiposPedido.Any(f => f == 4))
                {
                    tiposPedidoFiltrar.Add(Pedido.TipoPedidoEnum.MaoDeObraEspecial);
                    critetioTipoPedido.Add("Mão-de-obra Especial");
                }

                filtro += string.Format(" AND ped.TipoPedido IN ({0})", string.Join(",", tiposPedidoFiltrar.Select(f => (int)f)));
                filtroInterno += string.Format(" AND ped1.TipoPedido IN ({0})", string.Join(",", tiposPedidoFiltrar.Select(f => (int)f)));
                criterio += string.Format("Tipo Pedido: {0}    ", string.Join(", ", critetioTipoPedido.ToArray()));
            }

            if (idCorVidro > 0)
            {
                filtro += string.Format(" AND p.IdCorVidro={0}", idCorVidro);
                filtroInterno += string.Format(" AND p1.IdCorVidro={0}", idCorVidro);
                criterio += string.Format("Cor: {0}    ", CorVidroDAO.Instance.GetNome((uint)idCorVidro));
            }

            if (altura > 0)
            {
                filtro += string.Format(" AND IF(ppe.AlturaReal > 0, ppe.AlturaReal, ppe.Altura)={0}", altura);
                filtroInterno += string.Format(" AND IF(ppe1.AlturaReal > 0, ppe1.AlturaReal, ppe1.Altura)={0}", altura);
                criterio += string.Format("Altura da peça: {0}    ", altura);
            }

            if (largura > 0)
            {
                filtro += string.Format(" AND IF(ppe.LarguraReal > 0, ppe.LarguraReal, ppe.Largura)={0}", largura);
                filtroInterno += string.Format(" AND IF(ppe1.LarguraReal > 0, ppe1.LarguraReal, ppe1.Largura)={0}", largura);
                criterio += string.Format("Largura da peça: {0}    ", largura);
            }

            #endregion

            #region Consulta

            camposUnion = selecionar ? @"ppp1.IdProdPedProducao, NULL AS IdSetor, s1.Tipo AS TipoSetor, (SELECT MAX(NumSeq)+2 FROM setor) AS NumSeqSetor, 'Prev. Entrega' AS NomeSetor,
                ppp1.SituacaoProducao, CONCAT(DATE_FORMAT(ped1.DataEntrega, '%d-%m-%Y'), IF(ped1.DataEntregaOriginal IS NOT NULL AND ped1.DataEntrega<>ped1.DataEntregaOriginal,
                CONCAT(' (', DATE_FORMAT(ped1.DataEntregaOriginal, '%d-%m-%Y'), ')'), '')) AS DataSetor, ped1.IdFunc, ped1.IdPedido" : "ppp1.IdProdPedProducao, ppe.IdPedido, NULL AS IdSetor";

            sql = string.Format(@"SELECT ppp.IdProdPedProducao, ppe.IdPedido, ped.CodCliente AS PedCli, ped.TipoPedido={0} AS IsPedidoMaoDeObra, ped.TipoPedido={1} AS IsPedidoProducao,
                    ped.IdCli AS IdCliente, ppe.IdProd, ppe.IdProdPed, IF(ppe.AlturaReal > 0, ppe.AlturaReal, ppe.Altura) AS Altura, IF(ppe.LarguraReal > 0, ppe.LarguraReal, ppe.Largura) AS Largura,
                    ppp.NumEtiqueta, c.Nome AS NomeCliente, p.CodInterno AS CodInternoProduto, p.Descricao AS DescrProduto, dados.IdSetor, dados.NomeSetor, dados.DataSetor, dados.NumSeqSetor,
                    ppp.Situacao AS SituacaoProducao, s.Cor AS CorSetor, ppp.TipoPerda AS TipoPerdaProducao, ppp.Obs AS ObsPerdaProducao, ped.IdPedidoAnterior, ppp.IdPedidoExpedicao, '$$$' AS Criterio
                FROM produtos_pedido_espelho ppe
                    LEFT JOIN pedido ped ON (ppe.IdPedido=ped.IdPedido)
                    LEFT JOIN produto_pedido_producao ppp ON (ppe.IdProdPed=ppp.IdProdPed)
                    LEFT JOIN cliente c ON (ped.IdCli=c.Id_Cli)
                    LEFT JOIN produto p ON (ppe.IdProd=p.IdProd)
                    INNER JOIN (
                        {2}                                        
                        UNION ALL SELECT {3}
                        FROM produto_pedido_producao ppp1
                            LEFT JOIN produtos_pedido_espelho ppe1 ON (ppp1.IdProdPed=ppe1.IdProdPed)
                            LEFT JOIN pedido ped1 ON (ppe1.IdPedido=ped1.IdPedido)
                            LEFT JOIN cliente c1 ON (ped1.IdCli=c1.Id_Cli)
                            INNER JOIN produto p1 ON (ppe1.IdProd=p1.IdProd)
                            INNER JOIN setor s1 ON (ppp1.IdSetor=s1.IdSetor)
                        WHERE 1 {1}
                    ) AS dados ON (ppp.IdProdPedProducao=dados.IdProdPedProducao)
                    INNER JOIN setor s ON (ppp.IdSetor=s.IdSetor)
                WHERE 1 {0}", (int)Pedido.TipoPedidoEnum.MaoDeObra, (int)Pedido.TipoPedidoEnum.Producao,
                SqlSetores(true, true, filtroInterno, ObterParametrosProducao(codigoEtiqueta, codigoPedidoCliente, codigoRota, dataEntregaFim, dataEntregaInicio, dataLeituraFim, dataLeituraInicio,
                    idPedidoImportado, nomeCliente), selecionar), camposUnion);

            sql = string.Format(sql, filtro, filtroInterno);
            sql = !selecionar ? string.Format("SELECT COUNT(*) FROM ({0}) AS temp", sql) : sql;

            #endregion

            return sql.Replace("$$$", criterio);
        }
        
        /// <summary>
        /// Consulta que retorna os dados de produção para o relatório de produção.
        /// </summary>
        public IList<Producao> PesquisarProducaoRelatorio(bool aguardandoEntradaEstoque, bool aguardandoExpedicao, int altura, string codigoEtiqueta, string codigoPedidoCliente, string codigoRota,
            DateTime? dataEntregaFim, DateTime? dataEntregaInicio, DateTime? dataLeituraFim, DateTime? dataLeituraInicio, int idCarregamento, int idCliente, int idCorVidro, int idFuncionario,
            int idImpressao, int idLiberarPedido, int idPedido, int idPedidoImportado, int idSetor, int idSubgrupo, int largura, string nomeCliente, bool pecasCanceladas, int situacao,
            int tipoEntrega, IEnumerable<int> tiposPedido, int tipoSituacao)
        {
            var setoresAnteriores = tipoSituacao == 1;
            var setoresPosteriores = tipoSituacao == 2;

            return objPersistence.LoadData(SqlProducao(aguardandoEntradaEstoque, aguardandoExpedicao, altura, codigoEtiqueta, codigoPedidoCliente, codigoRota, dataEntregaFim, dataEntregaInicio,
                dataLeituraFim, dataLeituraInicio, idCarregamento, idCliente, idCorVidro, idFuncionario, idImpressao, idLiberarPedido, idPedido, idPedidoImportado, idSetor, idSubgrupo, largura,
                nomeCliente, pecasCanceladas, true, setoresAnteriores, setoresPosteriores, situacao, tipoEntrega, tiposPedido)).ToList();
        }

        /// <summary>
        /// Preenche os parâmetros da consulta que retorna os dados de produção para o relatório de produção.
        /// </summary>
        private GDAParameter[] ObterParametrosProducao(string codigoEtiqueta, string codigoPedidoCliente, string codigoRota, DateTime? dataEntregaFim, DateTime? dataEntregaInicio, DateTime? dataLeituraFim,
            DateTime? dataLeituraInicio, int idPedidoImportado, string nomeCliente)
        {
            var parametros = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(codigoEtiqueta))
            {
                parametros.Add(new GDAParameter("?codigoEtiqueta", codigoEtiqueta));
            }

            if (!string.IsNullOrEmpty(codigoPedidoCliente))
            {
                parametros.Add(new GDAParameter("?codigoPedidoCliente", string.Format("%{0}%", codigoPedidoCliente)));
            }

            if (!string.IsNullOrEmpty(codigoRota))
            {
                parametros.Add(new GDAParameter("?codigoRota", codigoRota));
            }

            if (dataEntregaFim.HasValue && dataEntregaFim > DateTime.MinValue)
            {
                parametros.Add(new GDAParameter("?dataEntregaFim", DateTime.Parse(dataEntregaFim.Value.ToString("dd/MM/yyyy 23:59:59"))));
            }

            if (dataEntregaInicio.HasValue && dataEntregaInicio > DateTime.MinValue)
            {
                parametros.Add(new GDAParameter("?dataEntregaInicio", DateTime.Parse(dataEntregaInicio.Value.ToString("dd/MM/yyyy 00:00:00"))));
            }

            if (dataLeituraFim.HasValue && dataLeituraFim > DateTime.MinValue)
            {
                parametros.Add(new GDAParameter("?dataLeituraFim", DateTime.Parse(dataLeituraFim.Value.ToString("dd/MM/yyyy 23:59:59"))));
            }

            if (dataLeituraInicio.HasValue && dataLeituraInicio > DateTime.MinValue)
            {
                parametros.Add(new GDAParameter("?dataLeituraInicio", DateTime.Parse(dataLeituraInicio.Value.ToString("dd/MM/yyyy 00:00:00"))));
            }

            if (idPedidoImportado > 0)
            {
                parametros.Add(new GDAParameter("?idPedidoImportado", idPedidoImportado));
            }

            if (!string.IsNullOrEmpty(nomeCliente))
            {
                parametros.Add(new GDAParameter("?nomeCliente", string.Format("%{0}%", nomeCliente)));
            }

            return parametros.Count > 0 ? parametros.ToArray() : null;
        }
    }
}
