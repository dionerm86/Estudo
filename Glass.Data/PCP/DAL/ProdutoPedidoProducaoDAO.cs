using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using System.Reflection;
using Glass.Configuracoes;
using Microsoft.Practices.ServiceLocation;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed class ProdutoPedidoProducaoDAO : BaseDAO<ProdutoPedidoProducao, ProdutoPedidoProducaoDAO>
    {
        #region Classe de suporte

        public class ContagemPecas
        {
            private readonly long _pendentes, _prontas, _entregues, _perdidas, _canceladas;
            private readonly double _totMPendentesCalc, _totMProntasCalc, _totMEntreguesCalc, _totMPerdidasCalc, _totMCanceladasCalc;
            private readonly double _totMPendentes, _totMProntas, _totMEntregues, _totMPerdidas, _totMCanceladas;

            public long Pendentes
            {
                get { return _pendentes; }
            }

            public double TotMPendentes
            {
                get { return _totMPendentes; }
            }

            public double TotMPendentesCalc
            {
                get { return _totMPendentesCalc; }
            }

            public long Prontas
            {
                get { return _prontas; }
            }

            public double TotMProntas
            {
                get { return _totMProntas; }
            }

            public double TotMProntasCalc
            {
                get { return _totMProntasCalc; }
            }

            public long Entregues
            {
                get { return _entregues; }
            }

            public double TotMEntregues
            {
                get { return _totMEntregues; }
            }

            public double TotMEntreguesCalc
            {
                get { return _totMEntreguesCalc; }
            }

            public long Perdidas
            {
                get { return _perdidas; }
            }

            public double TotMPerdidas
            {
                get { return _totMPerdidas; }
            }

            public double TotMPerdidasCalc
            {
                get { return _totMPerdidasCalc; }
            }

            public long Canceladas
            {
                get { return _canceladas; }
            }

            public double TotMCanceladas
            {
                get { return _totMCanceladas; }
            }

            public double TotMCanceladasCalc
            {
                get { return _totMCanceladasCalc; }
            }

            internal ContagemPecas(string retornoSql)
            {
                if (String.IsNullOrEmpty(retornoSql) || retornoSql.IndexOf(';') == -1)
                    return;

                string[] dados = retornoSql.Split(';');

                _pendentes = Convert.ToInt64(dados[0].StrParaFloat());
                _totMPendentes = Math.Round(Glass.Conversoes.StrParaDouble(dados[1]), 2);
                _totMPendentesCalc = Math.Round(Glass.Conversoes.StrParaDouble(dados[2]), 2);

                _prontas = Convert.ToInt64(dados[3].StrParaFloat());
                _totMProntas = Math.Round(Glass.Conversoes.StrParaDouble(dados[4]), 2);
                _totMProntasCalc = Math.Round(Glass.Conversoes.StrParaDouble(dados[5]), 2);

                _entregues = Convert.ToInt64(dados[6].StrParaFloat());
                _totMEntregues = Math.Round(Glass.Conversoes.StrParaDouble(dados[7]), 2);
                _totMEntreguesCalc = Math.Round(Glass.Conversoes.StrParaDouble(dados[8]), 2);

                _perdidas = Convert.ToInt64(dados[9].StrParaFloat());
                _totMPerdidas = Math.Round(Glass.Conversoes.StrParaDouble(dados[10]), 2);
                _totMPerdidasCalc = Math.Round(Glass.Conversoes.StrParaDouble(dados[11]), 2);

                _canceladas = Convert.ToInt64(dados[12].StrParaFloat());
                _totMCanceladas = Math.Round(Glass.Conversoes.StrParaDouble(dados[13]), 2);
                _totMCanceladasCalc = Math.Round(Glass.Conversoes.StrParaDouble(dados[14]), 2);
            }
        }

        #endregion

        #region Retorna listagem

        public enum TipoRetorno : long
        {
            Normal,
            EntradaEstoque,
            AguardandoExpedicao
        }

        /// <summary>
        /// Enumerador com as possibilidades do filtro de produto de composição.
        /// </summary>
        public enum ProdutoComposicao : long
        {
            /// <summary>
            /// Pai/filho de outro produto ou um produto sem envolvimento com composição.
            /// </summary>
            ProdutoComOuSemIdProdPedParent,

            /// <summary>
            /// Filho de um vidro Duplo/Laminado.
            /// </summary>
            ProdutoComIdProdPedParent,

            /// <summary>
            /// Pai de produtos de composição ou um produto sem envolvimento com composição.
            /// </summary>
            ProdutoSemIdProdPedParent
        }

        internal string Sql(uint idProdPedProducao, string idsProdPedProducao, int idCarregamento, string idLiberarPedido, string idPedido,
            string idPedidoImportado, uint idProdPed, string codEtiqueta, string codRota, uint idImpressao, string codPedCli,
            uint idCliente, string nomeCliente, string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, string dataIniFabr,
            string dataFimFabr, string dataIniConfPed, string dataFimConfPed, int idSetor, string situacao, int situacaoPedido,
            string tipoPedido, bool setoresAnteriores, bool setoresPosteriores, bool disponiveisLeituraSetor, string idsSubgrupos,
            uint tipoEntrega, string pecasProdCanc, uint idFunc, uint idCorVidro, int altura, int largura, float espessura, string idsProc,
            string idsApl, string idsBenef, TipoRetorno tipoRetorno, uint idTurno, string planoCorte, string numEtiquetaChapa,
            uint fastDelivery, bool calcPeso, bool calcM2, bool pecaParadaProducao, bool pecasRepostas, uint idLoja,
            ProdutoComposicao produtoComposicao, uint idProdPedParent, uint idProdPedProducaoParent, int? idClassificacao, bool selecionar,
            bool somenteIdsPedidos, out bool temFiltro, out string filtroAdicional)
        {
            // Define se ao filtrar pela data de entrega será filtrado também pela data de fábrica
            bool filtrarDataFabrica = ProducaoConfig.BuscarDataFabricaConsultaProducao;
            bool buscarNomeFantasia = ProducaoConfig.TelaConsulta.BuscarNomeFantasiaConsultaProducao;

            string campos = selecionar ? @"
                ppp.*, pp.idPedido, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.altura, if(pp.alturaReal > 0, pp.alturaReal, 
                pp.altura)) as Altura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.largura, if(pp.Redondo, 0, 
                if (pp.larguraReal > 0, pp.larguraReal, pp.largura))) as Largura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.Ambiente, concat(p.Descricao, if(pp.Redondo and " + (!BenefConfigDAO.Instance.CobrarRedondo()).ToString() + @", ' REDONDO', ''))) 
                as DescrProduto, p.CodInterno, ped.dataEntrega, ped.dataEntregaOriginal, cli.id_cli as IdCliente, 
                
                /* Retorna o nome do cliente concatenado com o nome externo e rota externa */
                Concat(" + (buscarNomeFantasia ? "Coalesce(cli.nomeFantasia, cli.nome)" : "cli.nome") + @", ' ', 
                if(ped.clienteExterno is not null and ped.clienteExterno<>'', Concat('(', ped.clienteExterno, ')'), ''),
                if(ped.rotaExterna is not null and ped.rotaExterna<>'', Concat('(', ped.rotaExterna, ')'), '')) as nomeCliente,

                apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, concat(cast(ped.IdPedido as char), 
                if(ped.IdPedidoAnterior is not null, concat(' (', concat(cast(ped.IdPedidoAnterior as char), 'R)')), ''), 
                if(ppp.idPedidoExpedicao is not null, concat(' (Exp. ', cast(ppp.idPedidoExpedicao as char), ')'), '')) as IdPedidoExibir, 
                pp.ValorVendido as ValorUnit, ped.CodCliente, round(
                    if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", (
                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
                    * a.qtde, pp.TotM2Calc)/(pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as TotM2, 
                round(
                    /*Caso o pedido seja mão de obra o m2 da peça deve ser considerado*/
                    if(ped.tipoPedido=3, (
                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
                    * a.qtde, pp.TotM)/(pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as totM, " +

                (calcPeso ? "pp.peso/(pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", a.qtde, 1))" : "0") + @" as Peso,

                (ped.situacao= " + (int)Pedido.SituacaoPedido.Cancelado + @") as PedidoCancelado, 
                ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @" as PedidoMaoObra, 
                If(lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado + @", lp.dataLiberacao, null) as DataLiberacaoPedido, 
                ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @" as PedidoProducao, cast(" + (int)tipoRetorno + @" as signed) as Tipo,
                s.Descricao as DescrSetor, '$$$' as Criterio, s.tipo as tipoSetor" +
                (PCPConfig.ControleCavalete ? ", c.CodInterno AS NumCavalete" : "") +
                (filtrarDataFabrica ? ", pedEsp.dataFabrica as DATAENTREGAFABRICA" : "") : somenteIdsPedidos ? "ped.IdPedido " : "count(distinct ppp.idProdPedProducao)";

            bool usarJoin = (idSetor > 0 && (!String.IsNullOrEmpty(dataIni) || !String.IsNullOrEmpty(dataFim))) || idTurno > 0;

            string sql = @"
                SELECT " + campos + @"
                FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)                    
                    LEFT JOIN cliente cli ON (ped.idCli = cli.id_Cli)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN setor s ON (ppp.idSetor = s.idSetor)
                    LEFT JOIN liberarpedido lp ON (ped.IdLiberarPedido = lp.IdLiberarPedido)
                    LEFT JOIN etiqueta_aplicacao apl ON (if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.idAplicacao, pp.idAplicacao) = apl.idAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.idProcesso, pp.idProcesso) = prc.idProcesso) ";

            if (!String.IsNullOrEmpty(dataIniFabr) || !String.IsNullOrEmpty(dataFimFabr) || filtrarDataFabrica)
                sql += " LEFT JOIN pedido_espelho pedEsp ON (ped.idPedido = pedEsp.idPedido)";

            if (usarJoin)
                sql += " LEFT JOIN leitura_producao lp1 ON (ppp.idProdPedProducao = lp1.idProdPedProducao)";

            if (PCPConfig.ControleCavalete)
                sql += " LEFT JOIN cavalete c ON (ppp.IdCavalete = c.IdCavalete)";

            sql += " WHERE 1 ?filtroAdicional?";

            string criterio = "";
            ProdutoPedidoProducao temp = new ProdutoPedidoProducao();

            temFiltro = !selecionar;
            filtroAdicional = "";

            string filtroPedido = "";

            if (idProdPedProducao > 0)
                filtroAdicional += " and ppp.idProdPedProducao=" + idProdPedProducao;
            else if (!String.IsNullOrEmpty(idsProdPedProducao))
                filtroAdicional += " and ppp.idProdPedProducao in (" + idsProdPedProducao + ")";

            if (idCarregamento > 0)
            {
                filtroAdicional += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento WHERE IdCarregamento={0})", idCarregamento);
                criterio += string.Format("Carregamento: {0}    ", idCarregamento);
                temFiltro = true;
            }

            if (idCarregamento > 0)
            {
                filtroAdicional += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento WHERE IdCarregamento={0})", idCarregamento);
                criterio += string.Format("Carregamento: {0}    ", idCarregamento);
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idLiberarPedido) && idLiberarPedido != "0")
            {
                string idsPedidoStr = String.Empty;
                var idsPedido = PedidoDAO.Instance.GetIdsByLiberacao(idLiberarPedido.StrParaUint());
                foreach (uint id in idsPedido)
                    idsPedidoStr += id + ",";

                filtroAdicional += " And ped.idPedido In (" + idsPedidoStr.TrimEnd(',') + ")";
                criterio += "Liberação: " + idLiberarPedido + "    ";
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                filtroAdicional += " and ped.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idPedido) && idPedido != "0")
            {
                filtroPedido += " And (ped.idPedido=" + idPedido;

                // Na vidrália/colpany não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira
                // teria que mudar totalmente a forma de fazer o count
                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido &&
                    PedidoDAO.Instance.IsPedidoReposto(idPedido.StrParaUint()))
                    filtroPedido += " Or ped.IdPedidoAnterior=" + idPedido;

                sql += filtroPedido;
                filtroPedido += ")";

                if (PedidoDAO.Instance.IsPedidoExpedicaoBox(idPedido.StrParaUint()))
                    sql += " Or ppp.idPedidoExpedicao=" + idPedido;

                sql += ")";

                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idPedidoImportado) && idPedidoImportado != "0")
            {
                sql += " And (ped.codCliente=?idPedidoImportado And ped.importado=1)";
                criterio += "Pedido importado: " + idPedidoImportado + "    ";
                temFiltro = true;
            }

            if (idProdPed > 0)
                filtroAdicional += " And ppp.idProdPed=" + idProdPed;

            if (!String.IsNullOrEmpty(codEtiqueta))
            {
                uint? id = ObtemIdProdPedProducao(codEtiqueta) ?? ObtemIdProdPedProducaoCanc(null, codEtiqueta);

                filtroAdicional += id > 0 ? " and ppp.idProdPedProducao=" + id : " and false";
                criterio += "Etiqueta: " + codEtiqueta + "    ";
            }

            if (!String.IsNullOrEmpty(codPedCli))
            {
                sql += " And (ped.codCliente like ?codCliente Or pp.pedCli like ?codCliente or a.ambiente like ?codCliente) ";

                filtroPedido += " and ped.codCliente like ?codCliente";

                criterio += "Pedido Cliente/Ambiente: " + codPedCli + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                filtroPedido += " And ped.idCli In (select * from (Select idCliente From rota_cliente Where idRota In (" + codRota + ")) as temp1)";
                sql += " And ped.idCli In (select * from (Select idCliente From rota_cliente Where idRota In (" + codRota + ")) as temp1)";

                criterio += "Rota: " + RotaDAO.Instance.ObtemCodRotas(codRota) + "    ";
                temFiltro = true;
            }

            if (idImpressao > 0)
            {
                filtroAdicional += " And if(!coalesce(ppp.pecaReposta,false), ppp.idImpressao=" + idImpressao +
                    @", coalesce(ppp.numEtiqueta, ppp.numEtiquetaCanc) in (select * from (
                    select concat(idPedido, '-', posicaoProd, '.', itemEtiqueta, '/', qtdeProd)
                    from produto_impressao where !coalesce(cancelado,false) and idImpressao=" + idImpressao + ") as temp))";

                criterio += "Num. Impressão: " + idImpressao + "    ";
            }

            if (idCliente > 0)
            {
                sql += " And ped.idCli=" + idCliente;
                filtroPedido += " And ped.idCli=" + idCliente;

                criterio += "Cliente: " + idCliente + " - " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And ped.idCli in (" + ids + ")";
                filtroPedido += " And ped.idCli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
                temFiltro = true;
            }

            if (idFunc > 0)
            {
                sql += " And ped.idFunc=" + idFunc;
                filtroPedido += " And ped.idFunc=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(situacao))
            {
                string where = " And(0";

                List<string> situacoes = new List<string>(situacao.Split(','));

                foreach (string s in situacoes)
                {
                    switch (s)
                    {
                        case "1":
                        case "2":
                            where += " OR ppp.Situacao=" + s;
                            temp.Situacao = long.Parse(s);
                            criterio += "Situação: " + temp.DescrSituacao + "    ";
                            break;
                        case "3":
                            where += " OR (ppp.SituacaoProducao=" + (int)SituacaoProdutoProducao.Pendente + " And ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + ")";
                            criterio += "Tipo: Peças pendentes    ";
                            break;
                        case "4":
                            where += " OR (ppp.SituacaoProducao=" + (int)SituacaoProdutoProducao.Pronto + " And ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + ")";
                            criterio += "Tipo: Peças prontas    ";
                            break;
                        case "5":
                            where += " OR (ppp.SituacaoProducao=" + (int)SituacaoProdutoProducao.Entregue + " And ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + ")";
                            criterio += "Tipo: Peças entregues    ";
                            break;
                    }
                }

                where += ")";

                if (temFiltro)
                    sql += where;
                else
                    filtroAdicional += where;
            }

            if (situacaoPedido > 0)
            {
                sql += " And ped.situacao=" + situacaoPedido;
                filtroPedido += " And ped.situacao=" + situacaoPedido;
                criterio += "Situação Pedido: " + PedidoDAO.Instance.GetSituacaoPedido(situacaoPedido) + "    ";
                temFiltro = true;
            }

            /* Chamado 49413. */
            if (produtoComposicao > 0)
            {
                switch (produtoComposicao)
                {
                    case ProdutoComposicao.ProdutoComIdProdPedParent:
                        sql += string.Format(" AND pp.IdProdPedParent IS NOT NULL");
                        temFiltro = true;
                        break;

                    case ProdutoComposicao.ProdutoSemIdProdPedParent:
                        sql += string.Format(" AND pp.IdProdPedParent IS NULL");
                        temFiltro = true;
                        break;
                }
            }

            if (idProdPedParent > 0)
            {
                sql += " AND pp.IdProdPedParent =" + idProdPedParent;
                temFiltro = true;
            }

            if (idProdPedProducaoParent > 0)
            {
                sql += " AND ppp.IdProdPedProducaoParent =" + idProdPedProducaoParent;
                temFiltro = true;
            }

            string descricaoSetor = idSetor > 0 ? Utils.ObtemSetor((uint)idSetor).Descricao :
                idSetor == -1 ? "Etiqueta não impressa" : String.Empty;

            if (!String.IsNullOrEmpty(dataIni))
            {
                if (("," + situacao + ",").Contains("," + (int)ProdutoPedidoProducao.SituacaoEnum.Perda + ","))
                {
                    filtroAdicional += " And ppp.dataPerda>=?dataIni";
                    criterio += "Data perda início: " + dataIni + "    ";
                }
                else if (idSetor > 0)
                {
                    sql += " And lp1.idSetor=" + idSetor + " and lp1.dataLeitura>=?dataIni";
                    criterio += "Data " + descricaoSetor + ": a partir de " + dataIni + "    ";
                    temFiltro = true;
                }
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                if (("," + situacao + ",").Contains("," + (int)ProdutoPedidoProducao.SituacaoEnum.Perda + ","))
                {
                    filtroAdicional += " And ppp.dataPerda<=?dataFim";
                    criterio += "Data perda término: " + dataFim + "    ";
                }
                else if (idSetor > 0)
                {
                    sql += " And lp1.idSetor=" + idSetor + " and lp1.dataLeitura<=?dataFim";
                    criterio = !String.IsNullOrEmpty(dataFim) ? criterio.TrimEnd() + " até " + dataFim + "    " :
                        criterio + " Data " + descricaoSetor + ": até " + dataFim + "    ";

                    temFiltro = true;
                }
            }

            if (!String.IsNullOrEmpty(dataIniEnt))
            {
                sql += " And ped.dataEntrega>=?dataIniEnt";
                filtroPedido += " And ped.dataEntrega>=?dataIniEnt";

                criterio += "Data Entrega início: " + dataIniEnt + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFimEnt))
            {
                sql += " And ped.dataEntrega<=?dataFimEnt";
                filtroPedido += " And ped.dataEntrega<=?dataFimEnt";

                criterio += "Data Entrega término: " + dataFimEnt + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIniFabr))
            {
                sql += " And (pedEsp.dataFabrica>=?dataIniFabr)";
                filtroPedido += " And (pedEsp.dataFabrica>=?dataIniFabr)";

                criterio += "Data fábrica início: " + dataIniFabr + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFimFabr))
            {
                sql += " And pedEsp.dataFabrica<=?dataFimFabr";
                filtroPedido += " And pedEsp.dataFabrica<=?dataFimFabr";

                criterio += "Data fábrica término: " + dataFimFabr + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIniConfPed) || !String.IsNullOrEmpty(dataFimConfPed))
            {
                string idsPedido;
                DateTime? dataIniConfPedUsar, dataFimConfPedUsar;

                if (!String.IsNullOrEmpty(dataIniConfPed))
                {
                    dataIniConfPedUsar = DateTime.Parse(dataIniConfPed);
                    criterio += "Data conf. ped. início: " + dataIniConfPed + "    ";
                }
                else
                    dataIniConfPedUsar = null;

                if (!String.IsNullOrEmpty(dataFimConfPed))
                {
                    dataFimConfPedUsar = DateTime.Parse(dataFimConfPed + " 23:59");
                    criterio += "Data conf. ped. término: " + dataFimConfPed + "    ";
                }
                else
                    dataFimConfPedUsar = null;

                idsPedido = PedidoDAO.Instance.ObtemIdsPelaDataConf(dataIniConfPedUsar, dataFimConfPedUsar);

                if (!String.IsNullOrEmpty(idsPedido))
                {
                    sql += " And ped.idPedido In (" + idsPedido + ")";
                    filtroPedido += " And ped.idPedido In (" + idsPedido + ")";
                }

                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idsBenef))
            {
                string redondo = BenefConfigDAO.Instance.TemBenefRedondo(idsBenef) ? " or pp.redondo=true" : "";
                filtroAdicional += " and (ppp.idProdPed in (select distinct idProdPed from produto_pedido_espelho_benef where idBenefConfig in (" + idsBenef + ")) " + redondo + ")";

                criterio += "Beneficiamentos: " + BenefConfigDAO.Instance.GetDescrBenef(idsBenef) + "    ";
            }

            if (!string.IsNullOrEmpty(idsSubgrupos) && idsSubgrupos != "0")
            {
                string descricaoSubgrupos = "";
                sql += " And p.IdSubgrupoProd In (" + idsSubgrupos + ")";

                foreach (var id in idsSubgrupos.Split(','))
                    descricaoSubgrupos += SubgrupoProdDAO.Instance.GetDescricao(id.StrParaInt()) + ", ";

                criterio += "Subgrupo(s): " + descricaoSubgrupos.TrimEnd(' ', ',') + "    ";
                temFiltro = true;
            }

            if (tipoEntrega > 0)
            {
                sql += " and ped.TipoEntrega=" + tipoEntrega;
                filtroPedido += " and ped.TipoEntrega=" + tipoEntrega;

                foreach (GenericModel te in Helper.DataSources.Instance.GetTipoEntrega())
                    if (te.Id == tipoEntrega)
                    {
                        criterio += "Tipo Entrega: " + te.Descr + "    ";
                        break;
                    }

                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(tipoPedido) && tipoPedido != "0")
            {
                var tiposPedido = new List<int>();
                var critetioTipoPedido = new List<string>();

                tipoPedido = "," + tipoPedido + ",";

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

                sql += " AND ped.tipoPedido IN(" + string.Join(",", tiposPedido.Select(f => f.ToString()).ToArray()) + ")";
                criterio += "Tipo Pedido: " + string.Join(", ", critetioTipoPedido.ToArray()) + "    ";

                temFiltro = true;
            }

            if (altura > 0)
            {
                sql += " And if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", a.altura, if(pp.alturaReal > 0, pp.alturaReal, pp.altura))=" + altura;
                criterio += "Altura da peça: " + altura + "    ";
                temFiltro = true;
            }

            if (largura > 0)
            {
                sql += " And if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", a.largura, if(pp.Redondo, 0, if (pp.larguraReal > 0, pp.larguraReal, pp.largura)))=" + largura;
                criterio += "Largura da peça: " + largura + "    ";
                temFiltro = true;
            }

            if (idCorVidro > 0)
            {
                sql += " And p.idCorVidro=" + idCorVidro;
                criterio += "Cor: " + CorVidroDAO.Instance.GetNome(idCorVidro) + "    ";
                temFiltro = true;
            }

            if (espessura > 0)
            {
                sql += " And p.espessura=?esp";
                criterio += "Espessura: " + espessura + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idsProc))
            {
                sql += " and pp.idProcesso in (" + idsProc + ")";
                criterio += "Processo: " + EtiquetaProcessoDAO.Instance.GetCodInternoByIds(idsProc) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idsApl))
            {
                sql += " and pp.idAplicacao in (" + idsApl + ")";
                criterio += "Aplicação: " + EtiquetaAplicacaoDAO.Instance.GetCodInternoByIds(idsApl) + "    ";
                temFiltro = true;
            }

            if (tipoRetorno == TipoRetorno.EntradaEstoque)
            {
                sql += " and ppp.entrouEstoque=false and ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao;
                temFiltro = true;
            }
            else if (tipoRetorno == TipoRetorno.AguardandoExpedicao)
            {
                sql += @" and ped.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + @" and ped.idPedido in 
                    (select * from (select idPedido from produtos_liberar_pedido plp 
                    left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido) 
                    where lp.situacao<>" + (int)LiberarPedido.SituacaoLiberarPedido.Cancelado + @") as temp)
                    And ppp.situacaoProducao<>" + (int)SituacaoProdutoProducao.Entregue + " And ppp.Situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(pecasProdCanc))
            {
                string criterioPecasProd = "";
                string situacaoProd = "";

                if (("," + pecasProdCanc + ",").Contains(",0,"))
                {
                    situacaoProd += "," + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + "," + (int)ProdutoPedidoProducao.SituacaoEnum.Perda;
                    criterioPecasProd += pecaParadaProducao ? ", com produção parada" : ", em produção";
                }

                if (("," + pecasProdCanc + ",").Contains(",1,"))
                {
                    situacaoProd += "," + (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra;
                    criterioPecasProd += ", canceladas (mão-de-obra)";
                }

                if (("," + pecasProdCanc + ",").Contains(",2,"))
                {
                    situacaoProd += "," + (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda;
                    criterioPecasProd += ", canceladas (venda)";
                }

                filtroAdicional += " And ppp.situacao in (" + situacaoProd.TrimStart(',') + ")";
                criterio += "Peças " + criterioPecasProd.Substring(", ".Length) + "    ";
            }
            else
                sql += " And false";

            if (idTurno > 0)
            {
                string inicio = TurnoDAO.Instance.ObtemValorCampo<string>("inicio", "idTurno=" + idTurno);
                string termino = TurnoDAO.Instance.ObtemValorCampo<string>("termino", "idTurno=" + idTurno);
                string descricao = TurnoDAO.Instance.ObtemValorCampo<string>("descricao", "idTurno=" + idTurno);

                if (TimeSpan.Parse(inicio) <= TimeSpan.Parse(termino))
                    sql += " and lp1.idSetor=ppp.idSetor and lp1.DataLeitura>=cast(concat(date_format(lp1.DataLeitura, '%Y-%m-%d'), ' " + inicio + "') as datetime) and lp1.DataLeitura<=cast(concat(date_format(lp1.DataLeitura, '%Y-%m-%d'), ' " + termino + "') as datetime)";
                else
                    sql += " and lp1.idSetor=ppp.idSetor and (lp1.DataLeitura>=cast(concat(date_format(lp1.DataLeitura, '%Y-%m-%d'), ' " + inicio + "') as datetime) or lp1.DataLeitura<cast(concat(date_format(lp1.DataLeitura, '%Y-%m-%d'), ' " + termino + "') as datetime))";

                criterio += "Turno: " + descricao + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(planoCorte))
            {
                sql += " AND ppp.PlanoCorte=?planoCorte";
                criterio += "Plano de corte: " + planoCorte + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(numEtiquetaChapa))
            {
                sql += @" and coalesce(ppp.numEtiqueta, ppp.numEtiquetaCanc) in (select * from (
                    select pip.numEtiqueta
                    from produto_impressao pip
                        inner join chapa_corte_peca ccp on (pip.idProdImpressao=ccp.idProdImpressaoPeca)
                        inner join produto_impressao pic on (ccp.idProdImpressaoChapa=pic.idProdImpressao)
                    where pic.numEtiqueta=?etiquetaChapa) as temp)";

                criterio += "Número Etiqueta Chapa: " + numEtiquetaChapa + "    ";
                temFiltro = true;
            }

            if (fastDelivery > 0)
            {
                sql += " and ped.fastdelivery=" + (fastDelivery == 1 ? "True" : "false");
                criterio += "Pedido(s) " + (fastDelivery == 1 ? "com" : "sem") + " Fast Delivery      ";
                temFiltro = true;
            }

            if (pecaParadaProducao)
            {
                sql += " AND ppp.pecaParadaProducao = true";
                temFiltro = true;
            }

            if (pecasRepostas)
            {
                sql += " and ppp.pecaReposta";
                temFiltro = true;
            }

            /* Chamado 45622. */
            if (idClassificacao > 0)
            {
                sql += string.Format(" AND pp.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0}) ", idClassificacao.Value);
                temFiltro = true;
            }

            if (idSetor > 0 || idSetor == -1)
            {
                if (!setoresPosteriores && !setoresAnteriores && !disponiveisLeituraSetor)
                {
                    if (idSetor > 0)
                    {
                        filtroAdicional += " And ppp.idSetor=" + idSetor;

                        // Filtro para impressão de etiqueta
                        if (Utils.ObtemSetor((uint)idSetor).NumeroSequencia == 1)
                        {
                            sql += " and exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao and idSetor=" + idSetor + " and dataLeitura is not null)";
                            temFiltro = true;
                        }
                    }

                    // Etiqueta não impressa
                    else if (idSetor == -1)
                    {
                        sql += " and ppp.idsetor=1 and not exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao and dataLeitura is not null)";
                        temFiltro = true;
                    }
                }
                else
                {
                    if (setoresAnteriores)
                    {
                        if (idSetor == 1)
                        {
                            sql += " and ppp.idsetor=1 and not exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao and dataLeitura is not null)";
                            temFiltro = true;
                        }
                        else
                        {
                            sql += " And not exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao and idSetor=" + idSetor + ")";
                            temFiltro = true;
                        }

                        // Retorna apenas as peças de roteiro se o setor for de roteiro
                        if (Utils.ObtemSetor((uint)idSetor).SetorPertenceARoteiro)
                        {
                            sql += " and exists (select * from roteiro_producao_etiqueta where idProdPedProducao=ppp.idProdPedProducao and idSetor=" + idSetor + ")";
                            temFiltro = true;
                        }
                    }
                    else if (setoresPosteriores)
                    {
                        if (idSetor == 1)
                        {
                            sql += " and exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao and dataLeitura is not null)";
                            temFiltro = true;
                        }

                        filtroAdicional +=
                            string.Format(@" AND {0} <= ALL (SELECT NumSeq FROM setor WHERE IdSetor=ppp.IdSetor)
                                AND (SELECT COUNT(*) FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={1}) > 0",
                                Utils.ObtemSetor((uint)idSetor).NumeroSequencia, idSetor);
                    }
                    else if (disponiveisLeituraSetor)
                    {
                        if (idSetor <= 1)
                        {
                            sql += " and not exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao and dataLeitura is not null)";
                            temFiltro = true;
                        }
                        else
                        {
                            sql +=
                                string.Format(@"
                                    AND EXISTS
                                    (
                                        SELECT ppp1.*
                                        FROM produto_pedido_producao ppp1
	                                        INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp1.IdProdPedProducao)
                                        WHERE rpe.IdSetor = {0}
	                                        AND ppp1.idProdPedProducao = ppp.idProdPedProducao
                                            AND ppp1.IdSetor =
                                                /* Se o setor filtrado for o primeiro setor do roteiro, busca somente as peças que estiverem no setor Impressão de Etiqueta. */
                                                IF ({0} =
                                                    (
    	                                                SELECT rpe.IdSetor
		                                                FROM produto_pedido_producao ppp2
			                                                INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp2.IdProdPedProducao)
    		                                                INNER JOIN setor s ON (rpe.IdSetor = s.IdSetor)
		                                                WHERE ppp2.IdProdPedProducao = ppp.IdProdPedProducao
                                                            AND ppp2.IdProdPedProducao IN (SELECT lp1.IdProdPedProducao FROM leitura_producao lp1)
		                                                ORDER BY s.NumSeq ASC
		                                                LIMIT 1
                                                    ), 1,
                                                    /* Senão, busca o próximo setor a ser lido no roteiro. */
                                                    (
    	                                                SELECT rpe.IdSetor
		                                                FROM produto_pedido_producao ppp2
			                                                INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp2.IdProdPedProducao)
    		                                                INNER JOIN setor s ON (rpe.IdSetor = s.IdSetor)
		                                                WHERE ppp2.IdProdPedProducao = ppp.IdProdPedProducao
			                                                AND s.NumSeq < (SELECT NumSeq FROM setor WHERE IdSetor = {0})
		                                                ORDER BY s.NumSeq DESC
		                                                LIMIT 1
                                                    ))
                                    )", idSetor);
                            temFiltro = true;
                        }
                    }
                }

                criterio += "Setor: " + descricaoSetor + (setoresAnteriores ? " (só produtos que ainda não passaram por este setor)" :
                    setoresPosteriores ? " (inclui produtos que já passaram por este setor)" : disponiveisLeituraSetor ? " (Apenas produtos disponíveis para leitura neste setor no momento)" : "") + "    ";
            }

            if (usarJoin && selecionar)
                sql += " group by ppp.idProdPedProducao";

            if (somenteIdsPedidos)
                sql += " GROUP BY ped.IdPedido";

            return sql.Replace("$$$", criterio.Trim());
        }

        private void GetSetores(ref ProdutoPedidoProducao[] retorno)
        {
            GetSetores(null, ref retorno);
        }

        private void GetSetores(GDASession sessao, ref ProdutoPedidoProducao[] retorno)
        {
            if (retorno.Length == 0)
                return;

            List<SetorProducao> dados = new List<SetorProducao>(SetorProducaoDAO.Instance.GetLeiturasSetores(sessao, retorno));

            Dictionary<int, PropertyInfo> propSetor = new Dictionary<int, PropertyInfo>(),
                propFunc = new Dictionary<int, PropertyInfo>(),
                propCorte = new Dictionary<int, PropertyInfo>();

            // Percorre todas as peças
            for (int i = 0; i < retorno.Length; i++)
            {
                // Busca as leituras da peça atual
                uint idProdPedProducao = retorno[i].IdProdPedProducao;
                SetorProducao s = dados.Find(x => x.IdProdPedProducao == idProdPedProducao);

                if (s == null)
                    continue;

                string dataLeitura = "";
                string nomeFuncLeitura = "";
                string setorCorte = "";

                int numSetor = 0;
                for (int j = 0; j < Utils.GetSetores.Length; j++)
                {
                    if (!Utils.GetSetores[j].ExibirRelatorio)
                        continue;

                    if (!propSetor.ContainsKey(numSetor))
                    {
                        propSetor.Add(numSetor, typeof(SetorProducao).GetProperty("Setor" + numSetor));
                        propFunc.Add(numSetor, typeof(SetorProducao).GetProperty("NomeFunc" + numSetor));
                        propCorte.Add(numSetor, typeof(SetorProducao).GetProperty("SetorCorte" + numSetor));
                    }

                    dataLeitura += Conversoes.ConverteValor<string>(propSetor[numSetor].GetValue(s, null)) + ",";
                    nomeFuncLeitura += Conversoes.ConverteValor<string>(propFunc[numSetor].GetValue(s, null)) + ",";
                    setorCorte += Conversoes.ConverteValor<string>(propCorte[numSetor].GetValue(s, null)) + ",";
                    numSetor++;
                }

                retorno[i].GroupDataLeitura = dataLeitura.TrimEnd(',');
                retorno[i].GroupIdSetor = s.IdsSetores;
                retorno[i].GroupNomeFuncLeitura = nomeFuncLeitura.TrimEnd(',');
                retorno[i].GroupSetorCorte = setorCorte.TrimEnd(',');
            }
        }

        private void GetNumChapaCorte(ref ProdutoPedidoProducao[] retorno)
        {
            GetNumChapaCorte(null, ref retorno);
        }

        private void GetNumChapaCorte(GDASession sessao, ref ProdutoPedidoProducao[] retorno)
        {
            if (retorno == null || retorno.Length == 0)
                return;

            string ids = String.Join(",", retorno.Select(x => x.IdProdPedProducao.ToString()).ToArray());

            ids = GetValoresCampo(sessao, @"
                select cast(concat(pi.idProdImpressao, '&', ppp.idProdPedProducao) as char) as valor
                from produto_pedido_producao ppp
                    inner join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                    inner join produto_impressao pi on (ppe.idPedido=pi.idPedido and ppe.idProdPed=pi.idProdPed
                        and ppp.numEtiqueta=pi.numEtiqueta)
                where pi.cancelado=false 
                    and ppp.idProdPedProducao in (" + ids + ")", "valor");

            if (String.IsNullOrEmpty(ids))
                return;

            Dictionary<uint, uint> idProdPedProducao = new Dictionary<uint, uint>();

            var dados = ids.Split(',', '&');
            for (int i = 0; i < dados.Length; i += 2)
                if (!idProdPedProducao.ContainsKey(dados[i].StrParaUint()))
                    idProdPedProducao.Add(dados[i].StrParaUint(), dados[i + 1].StrParaUint());

            ids = String.Join(",", idProdPedProducao.Keys.Select(x => x.ToString()).ToArray());

            if (String.IsNullOrEmpty(ids) || String.IsNullOrEmpty(ids.Trim(',')))
                return;

            ids = GetValoresCampo(sessao, @"
                SELECT CAST(CONCAT(idProdImpressaoPeca, '&', GROUP_CONCAT(DISTINCT Coalesce(numEtiqueta, '') SEPARATOR ';'), '&',
                    GROUP_CONCAT(DISTINCT Coalesce(numNfe, '') SEPARATOR ';'), '&', GROUP_CONCAT(Coalesce(lote, '') SEPARATOR ';')) AS CHAR) AS valor
                FROM
                    (SELECT ccp.idProdImpressaoPeca, pic.numEtiqueta,
                        if(pic.idNf IS NOT NULL, nf.numeroNFe, '') AS numNfe, Coalesce(pnf.lote, '') as lote
                    FROM chapa_corte_peca ccp
                        INNER JOIN produto_impressao pic ON (ccp.idProdImpressaoChapa=pic.idProdImpressao)
                        LEFT JOIN retalho_producao rp ON (pic.IdRetalhoProducao = rp.idRetalhoProducao)
                        LEFT JOIN produtos_nf pnf ON (COALESCE(pic.idProdNf, rp.IdProdNf) = pnf.idProdNf)
                        LEFT JOIN nota_fiscal nf ON (pnf.idNf = nf.idNf)
                    WHERE ccp.idProdImpressaoPeca IN (" + ids + @")
                    ORDER BY ccp.numSeq DESC) AS TEMP
                GROUP BY idProdImpressaoPeca", "valor", "~");

            if (String.IsNullOrEmpty(ids) || String.IsNullOrEmpty(ids.Trim(',')))
                return;

            Dictionary<uint, string> numEtiquetaChapa = new Dictionary<uint, string>();
            Dictionary<uint, string> numeroNFeChapa = new Dictionary<uint, string>();
            Dictionary<uint, string> loteChapa = new Dictionary<uint, string>();

            dados = ids.Split('~', '&');
            for (int i = 0; i < dados.Length; i += 4)
            {
                if (dados.Length == i + 1)
                    continue;

                numEtiquetaChapa.Add(dados[i].StrParaUint(), dados[i + 1]);
                numeroNFeChapa.Add(dados[i].StrParaUint(), dados[i + 2]);
                loteChapa.Add(dados[i].StrParaUint(), dados[i + 3]);
            }

            foreach (uint id in numEtiquetaChapa.Keys)
            {
                if (!numEtiquetaChapa.ContainsKey(id))
                    continue;

                int index = Array.FindIndex(retorno, x => x.IdProdPedProducao == idProdPedProducao[id]);
                if (index < 0 || index >= retorno.Length)
                    continue;

                retorno[index].NumEtiquetaChapa = numEtiquetaChapa[id];
                retorno[index].NumeroNFeChapa = numeroNFeChapa[id];
                retorno[index].LoteChapa = loteChapa[id];
            }
        }

        public IList<ProdutoPedidoProducao> GetListConsulta(int idCarregamento, string idLiberarPedido, uint idPedido,
            string idPedidoImportado, uint idImpressao, string codPedCli, string codRota, uint idCliente, string nomeCliente,
            string numEtiqueta, string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, string dataIniFabr,
            string dataFimFabr, string dataIniConfPed, string dataFimConfPed, int idSetor, string situacao, int situacaoPedido,
            int tipoSituacoes, string idsSubgrupos, uint tipoEntrega, string pecasProdCanc, uint idFunc, string tipoPedido,
            uint idCorVidro, int altura, int largura, float espessura, string idsProc, string idsApl, bool aguardExpedicao,
            bool aguardEntrEstoque, string idsBenef, string planoCorte, string numEtiquetaChapa, uint fastDelivery,
            bool pecaParadaProducao, bool pecasRepostas, uint idLoja, int? produtoComposicao, uint idProdPedProducaoParent, int pageIndex,
            string sortExpression, int startRow, int pageSize)
        {
            var listaVazia = ProducaoConfig.TelaConsulta.TelaVaziaPorPadrao;

            // Caso não seja utilizado nenhum filtro, retornar uma listagem vazia, para a tela carregar mais rápido
            if (listaVazia && FiltrosVazios(idCarregamento, idLiberarPedido.StrParaUint(), idPedido, idPedidoImportado, idImpressao,
                codRota, codPedCli, idCliente, nomeCliente, numEtiqueta, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr,
                dataFimFabr, dataIniConfPed, dataFimConfPed, idSetor, situacao, situacaoPedido, tipoSituacoes, idsSubgrupos, tipoEntrega,
                pecasProdCanc, idFunc, tipoPedido, idCorVidro, altura, largura, espessura, idsProc, idsApl, aguardExpedicao,
                aguardEntrEstoque, idsBenef, planoCorte, numEtiquetaChapa, fastDelivery, pecaParadaProducao, pecasRepostas, idLoja,
                idProdPedProducaoParent))
                return new List<ProdutoPedidoProducao>();

            bool situacoesAnteriores = tipoSituacoes == 1;
            bool situacoesPosteriores = tipoSituacoes == 2;
            bool disponiveisLeituraSetor = tipoSituacoes == 3;

            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(0, null, idCarregamento, idLiberarPedido, idPedido.ToString(), idPedidoImportado, 0, numEtiqueta, codRota,
                idImpressao, codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr, dataFimFabr,
                dataIniConfPed, dataFimConfPed, idSetor, situacao, situacaoPedido, tipoPedido, situacoesAnteriores, situacoesPosteriores,
                disponiveisLeituraSetor, idsSubgrupos, tipoEntrega, pecasProdCanc, idFunc, idCorVidro, altura, largura, espessura, idsProc,
                idsApl, idsBenef,
                aguardExpedicao ? TipoRetorno.AguardandoExpedicao : aguardEntrEstoque ? TipoRetorno.EntradaEstoque : TipoRetorno.Normal, 0,
                planoCorte, numEtiquetaChapa, fastDelivery, false, true, pecaParadaProducao, pecasRepostas, idLoja,
                (ProdutoComposicao)produtoComposicao.GetValueOrDefault(), 0, idProdPedProducaoParent, null, true, false, out temFiltro,
                out filtroAdicional).Replace(FILTRO_ADICIONAL, temFiltro ? filtroAdicional : "");

            GDAParameter[] lstParam = GetParam(idPedidoImportado, numEtiqueta, codRota, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr, dataFimFabr,
                nomeCliente, codPedCli, planoCorte, numEtiquetaChapa, espessura);

            string sort = GetListaConsultaSort(idPedido, codRota, pecasProdCanc, sortExpression, temFiltro, ref filtroAdicional);

            int numeroRegistros;
            sql = GetSqlWithLimit(sql, sort, pageIndex, pageSize, "ppp", filtroAdicional,
                !temFiltro, !String.IsNullOrEmpty(sortExpression) || idPedido > 0 || (ProducaoConfig.TelaConsulta.OrdenarPeloNumSeqSetor &&
                !String.IsNullOrEmpty(codRota)), out numeroRegistros, lstParam);

            var retorno = objPersistence.LoadData(sql, lstParam).ToArray();

            SetInfoPaging(sort, pageIndex, pageSize);

            GetSetores(ref retorno);
            GetNumChapaCorte(ref retorno);
            return retorno;
        }

        private string GetListaConsultaSort(uint idPedido, string codRota, string pecasProdCanc, string sortExpression, bool temFiltro, ref string filtroAdicional)
        {
            var sort = string.IsNullOrEmpty(sortExpression) ? (ProducaoConfig.TelaConsulta.OrdenarPeloNumSeqSetor && temFiltro ? "s.NumSeq ASC" : "ppp.IdProdPedProducao DESC") : sortExpression;

            if (sort == "pp.IdPedido DESC" && !temFiltro && pecasProdCanc != "0,1,2")
            {
                var pecasCanc = pecasProdCanc == "1";
                filtroAdicional = string.Format("pp.IdProdPed IN (SELECT DISTINCT IdProdPed{0} FROM produto_pedido_producao)", pecasCanc ? "Canc" : string.Empty);
            }

            if (idPedido > 0 || (ProducaoConfig.TelaConsulta.OrdenarPeloNumSeqSetor && !string.IsNullOrEmpty(codRota)))
            {
                sort = "s.NumSeq ASC, ppp.IdProdPedProducao DESC";
            }

            return sort;
        }

        public int GetCountConsulta(int idCarregamento, string idLiberarPedido, uint idPedido, string idPedidoImportado, uint idImpressao,
            string codPedCli, string codRota, uint idCliente, string nomeCliente, string numEtiqueta, string dataIni, string dataFim,
            string dataIniEnt, string dataFimEnt, string dataIniFabr, string dataFimFabr, string dataIniConfPed, string dataFimConfPed,
            int idSetor, string situacao, int situacaoPedido, int tipoSituacoes, string idsSubgrupos, uint tipoEntrega,
            string pecasProdCanc, uint idFunc, string tipoPedido, uint idCorVidro, int altura, int largura, float espessura,
            string idsProc, string idsApl, bool aguardExpedicao, bool aguardEntrEstoque, string idsBenef, string planoCorte,
            string numEtiquetaChapa, uint fastDelivery, bool pecaParadaProducao, bool pecasRepostas, uint idLoja, int? produtoComposicao,
            uint idProdPedProducaoParent, int pageIndex)
        {
            if (FiltrosVazios(idCarregamento, idLiberarPedido.StrParaUint(), idPedido, idPedidoImportado, idImpressao, codRota, codPedCli,
                idCliente, nomeCliente, numEtiqueta, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr, dataFimFabr, dataIniConfPed,
                dataFimConfPed, idSetor, situacao, situacaoPedido, tipoSituacoes, idsSubgrupos, tipoEntrega, pecasProdCanc, idFunc,
                tipoPedido, idCorVidro, altura, largura, espessura, idsProc, idsApl, aguardExpedicao, aguardEntrEstoque, idsBenef,
                planoCorte, numEtiquetaChapa, fastDelivery, pecaParadaProducao, pecasRepostas, idLoja, idProdPedProducaoParent))
                return 10000;

            bool situacoesAnteriores = tipoSituacoes == 1;
            bool situacoesPosteriores = tipoSituacoes == 2;
            bool disponiveisLeituraSetor = tipoSituacoes == 3;

            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(0, null, idCarregamento, idLiberarPedido, idPedido.ToString(), idPedidoImportado, 0, numEtiqueta, codRota,
                idImpressao, codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr, dataFimFabr,
                dataIniConfPed, dataFimConfPed, idSetor, situacao, situacaoPedido, tipoPedido, situacoesAnteriores, situacoesPosteriores,
                disponiveisLeituraSetor, idsSubgrupos, tipoEntrega, pecasProdCanc, idFunc, idCorVidro, altura, largura, espessura,
                idsProc, idsApl, idsBenef,
                aguardExpedicao ? TipoRetorno.AguardandoExpedicao : aguardEntrEstoque ? TipoRetorno.EntradaEstoque : TipoRetorno.Normal, 0,
                planoCorte, numEtiquetaChapa, fastDelivery, false, false, pecaParadaProducao, pecasRepostas, idLoja,
                (ProdutoComposicao)produtoComposicao.GetValueOrDefault(), 0, idProdPedProducaoParent, null, false, false, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            GDAParameter[] lstParam = GetParam(idPedidoImportado, numEtiqueta, codRota, dataIni, dataFim, dataIniEnt, dataFimEnt,
                dataIniFabr, dataFimFabr, nomeCliente, codPedCli, planoCorte, numEtiquetaChapa, espessura);

            // Fica muito mais rápido sem usar a otimização (GetCountWithInfoPaging())
            return objPersistence.ExecuteSqlQueryCount(sql, lstParam);
        }

        /// <summary>
        /// Retorna a quantidade peças prontas/pendentes utilizando os mesmos filtros da listagem
        /// </summary>
        public ContagemPecas GetCountBySituacao(int idCarregamento, uint idLiberarPedido, uint idPedido, string idPedidoImportado,
            uint idImpressao, string codRota, string codPedCli, uint idCliente, string nomeCliente, string numEtiqueta, string dataIni,
            string dataFim, string dataIniEnt, string dataFimEnt, string dataIniFabr, string dataFimFabr, string dataIniConfPed,
            string dataFimConfPed, int idSetor, string situacao, int situacaoPedido, int tipoSituacoes, string idsSubgrupos,
            uint tipoEntrega, string pecasProdCanc, uint idFunc, string tipoPedido, uint idCorVidro, int altura, int largura,
            float espessura, string idsProc, string idsApl, bool aguardExpedicao, bool aguardEntrEstoque, string idsBenef,
            string planoCorte, string numEtiquetaChapa, uint fastDelivery, bool pecaParadaProducao, bool pecasRepostas, uint idLoja,
            int? produtoComposicao)
        {
            if (FiltrosVazios(idCarregamento, idLiberarPedido, idPedido, idPedidoImportado, idImpressao, codRota, codPedCli, idCliente,
                nomeCliente, numEtiqueta, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr, dataFimFabr, dataIniConfPed,
                dataFimConfPed, idSetor, situacao, situacaoPedido, tipoSituacoes, idsSubgrupos, tipoEntrega, pecasProdCanc, idFunc,
                tipoPedido, idCorVidro, altura, largura, espessura, idsProc, idsApl, aguardExpedicao, aguardEntrEstoque, idsBenef,
                planoCorte, numEtiquetaChapa, fastDelivery, pecaParadaProducao, pecasRepostas, idLoja, 0))
                return new ContagemPecas(null);

            bool situacoesAnteriores = tipoSituacoes == 1;
            bool situacoesPosteriores = tipoSituacoes == 2;
            bool disponiveisLeituraSetor = tipoSituacoes == 3;

            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(0, null, idCarregamento, idLiberarPedido.ToString(), idPedido.ToString(), idPedidoImportado, 0, numEtiqueta,
                codRota, idImpressao, codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr,
                dataFimFabr, dataIniConfPed, dataFimConfPed, idSetor, situacao, situacaoPedido, tipoPedido, situacoesAnteriores,
                situacoesPosteriores, disponiveisLeituraSetor, idsSubgrupos, tipoEntrega, pecasProdCanc, idFunc, idCorVidro, altura,
                largura, espessura, idsProc, idsApl, idsBenef,
                aguardExpedicao ? TipoRetorno.AguardandoExpedicao : aguardEntrEstoque ? TipoRetorno.EntradaEstoque : TipoRetorno.Normal, 0,
                planoCorte, numEtiquetaChapa, fastDelivery, false, true, pecaParadaProducao, pecasRepostas, idLoja,
                (ProdutoComposicao)produtoComposicao.GetValueOrDefault(), 0, 0, null, false, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            string campoPendentes = "situacaoProducao=" + (int)SituacaoProdutoProducao.Pendente +
                " And situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
            string campoProntas = "situacaoProducao=" + (int)SituacaoProdutoProducao.Pronto +
                " And situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
            string campoEntregues = "situacaoProducao=" + (int)SituacaoProdutoProducao.Entregue +
                " And situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
            string campoPerdas = "situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Perda;
            string campoCanceladas = "situacao>" + (int)ProdutoPedidoProducao.SituacaoEnum.Perda;

            string campoTotMPendentes = "if(" + campoPendentes + ", totM, 0)";
            string campoTotMProntas = "if(" + campoProntas + ", totM, 0)";
            string campoTotMEntregues = "if(" + campoEntregues + ", totM, 0)";
            string campoTotMPerdas = "if(" + campoPerdas + ", totM, 0)";
            string campoTotMCanceladas = "if(" + campoCanceladas + ", totM, 0)";

            string campoTotMPendentesCalc = "if(" + campoPendentes + ", totMCalc, 0)";
            string campoTotMProntasCalc = "if(" + campoProntas + ", totMCalc, 0)";
            string campoTotMEntreguesCalc = "if(" + campoEntregues + ", totMCalc, 0)";
            string campoTotMPerdasCalc = "if(" + campoPerdas + ", totMCalc, 0)";
            string campoTotMCanceladasCalc = "if(" + campoCanceladas + ", totMCalc, 0)";

            sql = String.Format(@"select cast(concat(sum({0}), ';', sum({1}), ';', sum({2}), ';', sum({3}), ';', sum({4}), ';', sum({5}), ';', sum({6}), ';', sum({7}),
                ';', sum({8}), ';', sum({9}), ';', sum({10}), ';', sum({11}), ';', sum({12}), ';', sum({13}), ';', sum({14})) as char) 
                from (select ppp.idProdPedProducao, ppp.situacaoProducao, 
                ppp.situacao, round(
                    /*Caso o pedido seja mão de obra o m2 da peça deve ser considerado*/
                    if(ped.tipoPedido=3, (
                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
                    * a.qtde, pp.TotM2Calc)/(pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as totMCalc,
                    round(
                    /*Caso o pedido seja mão de obra o m2 da peça deve ser considerado*/
                    if(ped.tipoPedido=3, (
                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
                    * a.qtde, pp.TotM)/(pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as totM " +
                sql.Substring(sql.ToLower().IndexOf(" from ", StringComparison.Ordinal)) + ") as temp",

                campoPendentes, campoTotMPendentes, campoTotMPendentesCalc,
                campoProntas, campoTotMProntas, campoTotMProntasCalc,
                campoEntregues, campoTotMEntregues, campoTotMEntreguesCalc,
                campoPerdas, campoTotMPerdas, campoTotMPerdasCalc,
                campoCanceladas, campoTotMCanceladas, campoTotMCanceladasCalc);

            return new ContagemPecas(ExecuteScalar<string>(sql, GetParam(idPedidoImportado, numEtiqueta, codRota, dataIni, dataFim, dataIniEnt,
                dataFimEnt, dataIniFabr, dataFimFabr, nomeCliente, codPedCli, planoCorte, numEtiquetaChapa, espessura)));
        }

        private bool FiltrosVazios(int idCarregamento, uint idLiberarPedido, uint idPedido, string idPedidoImportado, uint idImpressao,
            string codRota, string codPedCli, uint idCliente, string nomeCliente, string numEtiqueta, string dataIni, string dataFim,
            string dataIniEnt, string dataFimEnt, string dataIniFabr, string dataFimFabr, string dataIniConfPed, string dataFimConfPed,
            int idSetor, string situacao, int situacaoPedido, int tipoSituacoes, string idsSubgrupos, uint tipoEntrega,
            string pecasProdCanc, uint idFunc, string tipoPedido, uint idCorVidro, int altura, int largura, float espessura,
            string idsProc, string idsApl, bool aguardExpedicao, bool aguardEntrEstoque, string idsBenef, string planoCorte,
            string numEtiquetaChapa, uint fastDelivery, bool pecaParadaProducao, bool pecasRepostas, uint idLoja,
            uint idProdPedProducaoParent)
        {
            var prodCanc = "0" + (PCPConfig.ExibirPecasCancMaoObraPadrao ? ",1" : "");

            return idCarregamento == 0 && idLiberarPedido == 0 && idPedido == 0 && string.IsNullOrEmpty(idPedidoImportado) &&
                idImpressao == 0 && string.IsNullOrEmpty(codRota) && string.IsNullOrEmpty(codPedCli) && idCliente == 0 &&
                string.IsNullOrEmpty(nomeCliente) && string.IsNullOrEmpty(numEtiqueta) && string.IsNullOrEmpty(dataIni) &&
                string.IsNullOrEmpty(dataFim) && string.IsNullOrEmpty(dataIniEnt) && string.IsNullOrEmpty(dataFimEnt) &&
                string.IsNullOrEmpty(dataIniConfPed) && string.IsNullOrEmpty(dataFimConfPed) && idSetor == 0 &&
                string.IsNullOrEmpty(situacao) && situacaoPedido == 0 && tipoSituacoes == 0 && string.IsNullOrEmpty(idsSubgrupos) &&
                tipoEntrega == 0 && pecasProdCanc == prodCanc && idFunc == 0 && string.IsNullOrEmpty(tipoPedido) && idCorVidro == 0 &&
                altura == 0 & largura == 0 && string.IsNullOrEmpty(idsProc) && string.IsNullOrEmpty(idsApl) && !aguardExpedicao &&
                !aguardEntrEstoque && espessura == 0.0 && string.IsNullOrEmpty(idsBenef) && string.IsNullOrEmpty(dataIniFabr) &&
                string.IsNullOrEmpty(dataFimFabr) && string.IsNullOrEmpty(planoCorte) && string.IsNullOrEmpty(numEtiquetaChapa) &&
                fastDelivery == 0 && !pecaParadaProducao && !pecasRepostas && idLoja == 0 && idProdPedProducaoParent == 0;
        }

        public ProdutoPedidoProducao GetByEtiqueta(string codEtiqueta)
        {
            return GetByEtiqueta(null, codEtiqueta);
        }

        public ProdutoPedidoProducao GetByEtiqueta(GDASession session, string codEtiqueta)
        {
            try
            {
                // Valida a etiqueta
                ValidaEtiquetaProducao(session, ref codEtiqueta);
            }
            catch
            {
                return null;
            }

            ProdutoPedidoProducao[] retorno = { objPersistence.LoadOneData(session, "Select * From produto_pedido_producao Where numEtiqueta=?codEtiqueta",
                new GDAParameter("?codEtiqueta", codEtiqueta)) };

            GetSetores(session, ref retorno);
            return retorno[0];
        }

        public ProdutoPedidoProducao[] GetForRpt(int idCarregamento, uint idLiberarPedido, uint idPedido, string idPedidoImportado,
            uint idImpressao, string codPedCli, string codRota, uint idCliente, string nomeCliente, string numEtiqueta, string dataIni,
            string dataFim, string dataIniEnt, string dataFimEnt, string dataIniFabr, string dataFimFabr, string dataIniConfPed,
            string dataFimConfPed, int idSetor, string situacao, int situacaoPedido, int tipoSituacoes, string idsSubgrupos,
            uint tipoEntrega, string pecasProdCanc, uint idFunc, string tipoPedido, uint idCorVidro, int altura, int largura,
            float espessura, string idsProc, string idsApl, bool aguardExpedicao, bool aguardEntrEstoque, string idsBenef,
            string planoCorte, string numEtiquetaChapa, uint fastDelivery, bool pecaParadaProducao, bool pecasRepostas, uint idLoja,
            int? produtoComposicao)
        {
            bool situacoesAnteriores = tipoSituacoes == 1;
            bool situacoesPosteriores = tipoSituacoes == 2;
            bool disponiveisLeituraSetor = tipoSituacoes == 3;

            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(0, null, idCarregamento, idLiberarPedido.ToString(), idPedido.ToString(), idPedidoImportado, 0, numEtiqueta,
                codRota, idImpressao, codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, dataFimEnt, dataIniFabr,
                dataFimFabr, dataIniConfPed, dataFimConfPed, idSetor, situacao, situacaoPedido, tipoPedido, situacoesAnteriores,
                situacoesPosteriores, disponiveisLeituraSetor, idsSubgrupos, tipoEntrega, pecasProdCanc, idFunc, idCorVidro, altura,
                largura, espessura, idsProc, idsApl, idsBenef,
                aguardExpedicao ? TipoRetorno.AguardandoExpedicao : aguardEntrEstoque ? TipoRetorno.EntradaEstoque : TipoRetorno.Normal, 0,
                planoCorte, numEtiquetaChapa, fastDelivery, true, true, pecaParadaProducao, pecasRepostas, idLoja,
                (ProdutoComposicao)produtoComposicao.GetValueOrDefault(), 0, 0, null, true, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            var sort = GetListaConsultaSort(idPedido, codRota, pecasProdCanc, null, temFiltro, ref filtroAdicional);

            var ordenar = (!temFiltro || (idPedido > 0 || (ProducaoConfig.TelaConsulta.OrdenarPeloNumSeqSetor &&
                !String.IsNullOrEmpty(codRota)))) && !string.IsNullOrEmpty(sort) ? " order by " + sort : string.Empty;

            sql += ordenar;

            if (objPersistence != null)
            {
                var retorno = objPersistence.LoadData(sql, GetParam(idPedidoImportado, numEtiqueta, codRota, dataIni, dataFim, dataIniEnt,
                    dataFimEnt, dataIniFabr, dataFimFabr, nomeCliente, codPedCli, planoCorte, numEtiquetaChapa, espessura)).ToArray();

                GetNumChapaCorte(ref retorno);
                return retorno;
            }

            return null;
        }

        internal GDAParameter[] GetParam(string idPedidoImportado, string codEtiqueta, string codRota, string dataIni, string dataFim,
            string dataIniEnt, string dataFimEnt, string dataIniFabr, string dataFimFabr, string nomeCliente, string codPedCli,
            string planoCorte, string numEtiquetaChapa, float espessura)
        {
            var lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(idPedidoImportado))
                lstParam.Add(new GDAParameter("?idPedidoImportado", idPedidoImportado));

            if (!string.IsNullOrEmpty(codEtiqueta))
                lstParam.Add(new GDAParameter("?codEtiqueta", codEtiqueta));

            if (!string.IsNullOrEmpty(codRota))
                lstParam.Add(new GDAParameter("?codRota", codRota));

            if (!string.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", (dataIni.Length == 10 ? DateTime.Parse(dataIni + " 00:00") : DateTime.Parse(dataIni))));

            if (!string.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", (dataFim.Length == 10 ? DateTime.Parse(dataFim + " 23:59:59") : DateTime.Parse(dataFim))));

            if (!string.IsNullOrEmpty(dataIniEnt))
                lstParam.Add(new GDAParameter("?dataIniEnt", DateTime.Parse(dataIniEnt + " 00:00")));

            if (!string.IsNullOrEmpty(dataFimEnt))
                lstParam.Add(new GDAParameter("?dataFimEnt", DateTime.Parse(dataFimEnt + " 23:59")));

            if (!string.IsNullOrEmpty(dataIniFabr))
                lstParam.Add(new GDAParameter("?dataIniFabr", DateTime.Parse(dataIniFabr + " 00:00")));

            if (!string.IsNullOrEmpty(dataFimFabr))
                lstParam.Add(new GDAParameter("?dataFimFabr", DateTime.Parse(dataFimFabr + " 23:59")));

            if (!string.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!string.IsNullOrEmpty(codPedCli))
                lstParam.Add(new GDAParameter("?codCliente", "%" + codPedCli + "%"));

            if (!string.IsNullOrEmpty(planoCorte))
                lstParam.Add(new GDAParameter("?planoCorte", planoCorte));

            if (!string.IsNullOrEmpty(numEtiquetaChapa))
                lstParam.Add(new GDAParameter("?etiquetaChapa", numEtiquetaChapa));

            if (espessura > 0)
                lstParam.Add(new GDAParameter("?esp", espessura));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #region Consulta Simplificada

        /// <summary>
        /// SQL da consulta simplificada
        /// </summary>
        private string SqlSimplificado(int idLiberarPedido, string idPedidoImportado, int idLoja, int idFunc, string tipoPedido, string pecasProdCanc,
            TipoRetorno tipoRetorno, string dataIniConfPed, string dataFimConfPed, int fastDelivery, List<GDAParameter> parametros, bool selecionar)
        {
            var criterio = new List<string>();
            var where = "";

            // Define se ao filtrar pela data de entrega será filtrado também pela data de fábrica
            bool filtrarDataFabrica = ProducaoConfig.BuscarDataFabricaConsultaProducao;
            bool buscarNomeFantasia = ProducaoConfig.TelaConsulta.BuscarNomeFantasiaConsultaProducao;

            string campos = selecionar ? @"
                ppp.*, pp.idPedido, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.altura, if(pp.alturaReal > 0, pp.alturaReal, 
                pp.altura)) as Altura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.largura, if(pp.Redondo, 0, 
                if (pp.larguraReal > 0, pp.larguraReal, pp.largura))) as Largura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.Ambiente, concat(p.Descricao, if(pp.Redondo and " + (!BenefConfigDAO.Instance.CobrarRedondo()).ToString() + @", ' REDONDO', ''))) 
                as DescrProduto, p.CodInterno, ped.dataEntrega, ped.dataEntregaOriginal, cli.id_cli as IdCliente, 
                
                /* Retorna o nome do cliente concatenado com o nome externo e rota externa */
                Concat(" + (buscarNomeFantasia ? "Coalesce(cli.nomeFantasia, cli.nome)" : "cli.nome") + @", ' ', 
                if(ped.clienteExterno is not null and ped.clienteExterno<>'', Concat('(', ped.clienteExterno, ')'), ''),
                if(ped.rotaExterna is not null and ped.rotaExterna<>'', Concat('(', ped.rotaExterna, ')'), '')) as nomeCliente,

                apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, concat(cast(ped.IdPedido as char), 
                if(ped.IdPedidoAnterior is not null, concat(' (', concat(cast(ped.IdPedidoAnterior as char), 'R)')), ''), 
                if(ppp.idPedidoExpedicao is not null, concat(' (Exp. ', cast(ppp.idPedidoExpedicao as char), ')'), '')) as IdPedidoExibir, 
                pp.ValorVendido as ValorUnit, ped.CodCliente,
                round(if(ped.tipoPedido=3, ((((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) + a.altura) * 
                    ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000) * a.qtde, pp.TotM2Calc) / 
                    (pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as TotM2, 
                pp.peso/(pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)) as Peso,
                (ped.situacao= " + (int)Pedido.SituacaoPedido.Cancelado + @") as PedidoCancelado, 
                (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @") as PedidoMaoObra, 
                If(lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado + @", lp.dataLiberacao, null) as DataLiberacaoPedido, 
                ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @" as PedidoProducao, cast(" + (int)tipoRetorno + @" as signed) as Tipo,
                s.Descricao as DescrSetor, '$$$' as Criterio, s.tipo as tipoSetor" : "count(distinct ppp.idProdPedProducao)";

            var sql = @"
                SELECT " + campos + @"
                FROM produto_pedido_producao ppp
                    LEFT JOIN setor s ON (ppp.idSetor = s.idSetor)
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN cliente cli ON (ped.idCli = cli.id_Cli)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN liberarpedido lp ON (ped.idLiberarPedido = lp.idLiberarPedido)
                    LEFT JOIN etiqueta_aplicacao apl ON (if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.idAplicacao, pp.idAplicacao) = apl.idAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.idProcesso, pp.idProcesso) = prc.idProcesso)
                WHERE 1 AND pp.IdProdPedParent IS NULL
                ";

            #region Filtros

            if (idLiberarPedido > 0)
            {
                var sqlLiberarPedido = @"
                    SELECT IdPedido 
                    FROM produtos_liberar_pedido 
                    WHERE IdLiberarPedido = " + idLiberarPedido + @"
                    GROUP BY IdPedido";

                var ids = ExecuteMultipleScalar<int>(sqlLiberarPedido);

                if (ids.Count > 0)
                {
                    where += string.Format(" AND ped.IdPedido IN ({0})", string.Join(",", ids.Select(f => f.ToString())));
                    criterio.Add("Liberação: " + idLiberarPedido);
                }
            }

            if (!string.IsNullOrWhiteSpace(idPedidoImportado) && idPedidoImportado != "0")
            {
                where += " AND ped.codCliente = ?idPedidoImportado AND ped.importado = 1";
                parametros.Add(new GDAParameter("?idPedidoImportado", idPedidoImportado));
                criterio.Add("Pedido importado: " + idPedidoImportado);
            }

            if (idLoja > 0)
            {
                where += " AND ped.idLoja=" + idLoja;
                criterio.Add("Loja: " + LojaDAO.Instance.GetNome((uint)idLoja));
            }

            if (idFunc > 0)
            {
                where += " AND ped.idFunc=" + idFunc;
                criterio.Add("Funcionário: " + FuncionarioDAO.Instance.GetNome((uint)idFunc));
            }

            if (!string.IsNullOrWhiteSpace(tipoPedido) && tipoPedido != "0")
            {
                var tiposPedido = new List<int>();
                var critetioTipoPedido = new List<string>();

                tipoPedido = "," + tipoPedido + ",";

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

                sql += string.Format(" AND ped.tipoPedido IN ({0})", string.Join(",", tiposPedido.Select(f => f.ToString())));
                criterio.Add("Tipo Pedido: " + string.Join(", ", critetioTipoPedido));
            }

            if (!string.IsNullOrWhiteSpace(pecasProdCanc))
            {
                var sitProd = new List<int>();
                var critetioSitProd = new List<string>();

                pecasProdCanc = "," + pecasProdCanc + ",";

                if (pecasProdCanc.Contains(",0,"))
                {
                    sitProd.Add((int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                    sitProd.Add((int)ProdutoPedidoProducao.SituacaoEnum.Perda);

                    critetioSitProd.Add("em produção");
                }

                if (pecasProdCanc.Contains(",1,"))
                {
                    sitProd.Add((int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra);

                    critetioSitProd.Add("canceladas (mão-de-obra)");
                }

                if (pecasProdCanc.Contains(",2,"))
                {
                    sitProd.Add((int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda);

                    critetioSitProd.Add("canceladas (venda)");
                }

                where += string.Format(" AND ppp.situacao in ({0})", string.Join(",", sitProd.Select(f => f.ToString())));
                criterio.Add("Peças " + string.Join(", ", critetioSitProd));
            }
            else
                sql += " AND 0";

            if (tipoRetorno == TipoRetorno.EntradaEstoque)
                sql += " AND ppp.EntrouEstoque = 0 and ped.TipoPedido = " + (int)Pedido.TipoPedidoEnum.Producao;

            if (!string.IsNullOrWhiteSpace(dataIniConfPed) || !string.IsNullOrWhiteSpace(dataFimConfPed))
            {
                DateTime? dataIniConfPedUsar = null, dataFimConfPedUsar = null;

                if (!string.IsNullOrWhiteSpace(dataIniConfPed))
                {
                    dataIniConfPedUsar = DateTime.Parse(dataIniConfPed);
                    criterio.Add("Data conf. ped. início: " + dataIniConfPed);
                }

                if (!string.IsNullOrWhiteSpace(dataFimConfPed))
                {
                    dataFimConfPedUsar = DateTime.Parse(dataFimConfPed + " 23:59");
                    criterio.Add("Data conf. ped. término: " + dataFimConfPed);
                }

                var idsPedido = PedidoDAO.Instance.ObtemIdsPelaDataConf(dataIniConfPedUsar, dataFimConfPedUsar);

                if (!string.IsNullOrWhiteSpace(idsPedido))
                    sql += string.Format(" AND ped.idPedido IN ({0})", idsPedido);
            }

            if (fastDelivery > 0)
            {
                sql += " AND ped.fastdelivery = " + (fastDelivery == 1 ? "1" : "0");
                criterio.Add("Pedido(s) " + (fastDelivery == 1 ? "com" : "sem") + " Fast Delivery");
            }

            #endregion

            sql += where;

            if (selecionar)
                sql += " GROUP BY ppp.IdProdPedProducao";

            return sql;
        }

        /// <summary>
        /// Método de busca do consulta produção simplificado
        /// </summary>
        public ProdutoPedidoProducao[] GetListConsultaSimplificado(int idLiberarPedido, string idPedidoImportado, int idLoja, int idFunc, string tipoPedido, string pecasProdCanc,
            bool aguardEntrEstoque, string dataIniConfPed, string dataFimConfPed, int fastDelivery,
            string sortExpression, int startRow, int pageSize)
        {
            var listaVazia = ProducaoConfig.TelaConsulta.TelaVaziaPorPadrao;

            // Caso não seja utilizado nenhum filtro, retornar uma listagem vazia, para a tela carregar mais rápido
            if (listaVazia && FiltrosVazios(0, (uint)idLiberarPedido, 0, idPedidoImportado, 0, null, null, 0, null, null, null, null, null, null, null,
                null, dataIniConfPed, dataFimConfPed, 0, null, 0, 0, null, 0, pecasProdCanc, (uint)idFunc, tipoPedido, 0, 0, 0, 0, null, null, false,
                aguardEntrEstoque, null, null, null, (uint)fastDelivery, false, false, (uint)idLoja, 0))
                return new ProdutoPedidoProducao[0];

            var tipoRetorno = aguardEntrEstoque ? TipoRetorno.EntradaEstoque : TipoRetorno.Normal;
            var parametros = new List<GDAParameter>();

            var sql = SqlSimplificado(idLiberarPedido, idPedidoImportado, idLoja, idFunc, tipoPedido, pecasProdCanc, tipoRetorno, dataIniConfPed, dataFimConfPed, fastDelivery, parametros, true);

            var retorno = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, parametros.ToArray()).ToArray();

            GetSetores(ref retorno);
            GetNumChapaCorte(ref retorno);
            return retorno;
        }

        /// <summary>
        /// Método de contagem de registros do consulta produção simplificado
        /// </summary>
        public int GetListConsultaSimplificadoCount(int idLiberarPedido, string idPedidoImportado, int idLoja, int idFunc, string tipoPedido, string pecasProdCanc,
            bool aguardEntrEstoque, string dataIniConfPed, string dataFimConfPed, int fastDelivery)
        {
            var listaVazia = ProducaoConfig.TelaConsulta.TelaVaziaPorPadrao;

            // Caso não seja utilizado nenhum filtro, retornar uma listagem vazia, para a tela carregar mais rápido
            if (listaVazia && FiltrosVazios(0, (uint)idLiberarPedido, 0, idPedidoImportado, 0, null, null, 0, null, null, null, null, null, null, null,
                null, dataIniConfPed, dataFimConfPed, 0, null, 0, 0, null, 0, pecasProdCanc, (uint)idFunc, tipoPedido, 0, 0, 0, 0, null, null, false,
                aguardEntrEstoque, null, null, null, (uint)fastDelivery, false, false, (uint)idLoja, 0))
                return 0;

            var tipoRetorno = aguardEntrEstoque ? TipoRetorno.EntradaEstoque : TipoRetorno.Normal;
            var parametros = new List<GDAParameter>();

            var sql = SqlSimplificado(idLiberarPedido, idPedidoImportado, idLoja, idFunc, tipoPedido, pecasProdCanc, tipoRetorno, dataIniConfPed, dataFimConfPed, fastDelivery, parametros, false);

            return objPersistence.ExecuteSqlQueryCount(sql, parametros.ToArray());
        }

        /// <summary>
        /// Retorna a quantidade peças prontas/pendentes utilizando os mesmos filtros da listagem
        /// </summary>
        public ContagemPecas GetContagemPecasSimplificado(int idLiberarPedido, string idPedidoImportado, int idLoja, int idFunc, string tipoPedido, string pecasProdCanc,
            bool aguardEntrEstoque, string dataIniConfPed, string dataFimConfPed, int fastDelivery)
        {
            var listaVazia = ProducaoConfig.TelaConsulta.TelaVaziaPorPadrao;

            // Caso não seja utilizado nenhum filtro, retornar uma listagem vazia, para a tela carregar mais rápido
            if (listaVazia && FiltrosVazios(0, (uint)idLiberarPedido, 0, idPedidoImportado, 0, null, null, 0, null, null, null, null, null, null, null,
                null, dataIniConfPed, dataFimConfPed, 0, null, 0, 0, null, 0, pecasProdCanc, (uint)idFunc, tipoPedido, 0, 0, 0, 0, null, null, false,
                aguardEntrEstoque, null, null, null, (uint)fastDelivery, false, false, (uint)idLoja, 0))
                return new ContagemPecas(null);

            var tipoRetorno = aguardEntrEstoque ? TipoRetorno.EntradaEstoque : TipoRetorno.Normal;
            var parametros = new List<GDAParameter>();

            var sql = SqlSimplificado(idLiberarPedido, idPedidoImportado, idLoja, idFunc, tipoPedido, pecasProdCanc, tipoRetorno, dataIniConfPed, dataFimConfPed, fastDelivery, parametros, true);

            string campoPendentes = "situacaoProducao=" + (int)SituacaoProdutoProducao.Pendente + " And situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
            string campoProntas = "situacaoProducao=" + (int)SituacaoProdutoProducao.Pronto + " And situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
            string campoEntregues = "situacaoProducao=" + (int)SituacaoProdutoProducao.Entregue + " And situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
            string campoPerdas = "situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Perda;
            string campoCanceladas = "situacao>" + (int)ProdutoPedidoProducao.SituacaoEnum.Perda;

            string campoTotMPendentes = "if(" + campoPendentes + ", totM, 0)";
            string campoTotMProntas = "if(" + campoProntas + ", totM, 0)";
            string campoTotMEntregues = "if(" + campoEntregues + ", totM, 0)";
            string campoTotMPerdas = "if(" + campoPerdas + ", totM, 0)";
            string campoTotMCanceladas = "if(" + campoCanceladas + ", totM, 0)";

            string campoTotMPendentesCalc = "if(" + campoPendentes + ", totMCalc, 0)";
            string campoTotMProntasCalc = "if(" + campoProntas + ", totMCalc, 0)";
            string campoTotMEntreguesCalc = "if(" + campoEntregues + ", totMCalc, 0)";
            string campoTotMPerdasCalc = "if(" + campoPerdas + ", totMCalc, 0)";
            string campoTotMCanceladasCalc = "if(" + campoCanceladas + ", totMCalc, 0)";

            sql = string.Format(@"select cast(concat(sum({0}), ';', sum({1}), ';', sum({2}), ';', sum({3}), ';', sum({4}), ';', sum({5}), ';', sum({6}), ';', sum({7}),
                ';', sum({8}), ';', sum({9}), ';', sum({10}), ';', sum({11}), ';', sum({12}), ';', sum({13}), ';', sum({14})) as char) 
                from (select ppp.idProdPedProducao, ppp.situacaoProducao, 
                ppp.situacao, round(
                    /*Caso o pedido seja mão de obra o m2 da peça deve ser considerado*/
                    if(ped.tipoPedido=3, (
                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
                    * a.qtde, pp.TotM2Calc)/(pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as totMCalc,
                    round(
                    /*Caso o pedido seja mão de obra o m2 da peça deve ser considerado*/
                    if(ped.tipoPedido=3, (
                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
                    * a.qtde, pp.TotM)/(pp.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as totM " +
                sql.Substring(sql.ToLower().IndexOf(" from ", StringComparison.Ordinal)) + ") as temp",

                campoPendentes, campoTotMPendentes, campoTotMPendentesCalc,
                campoProntas, campoTotMProntas, campoTotMProntasCalc,
                campoEntregues, campoTotMEntregues, campoTotMEntreguesCalc,
                campoPerdas, campoTotMPerdas, campoTotMPerdasCalc,
                campoCanceladas, campoTotMCanceladas, campoTotMCanceladasCalc);

            return new ContagemPecas(ExecuteScalar<string>(sql, parametros.ToArray()));
        }

        #endregion

        #endregion

        #region Recuperação dos ids de pedidos para o relatório de produção

        /// <summary>
        /// SQL da consulta que retorna os IDs dos pedidos através dos produtos de produção.
        /// </summary>
        internal string SqlIdsPedidoRelatorioProducao(int altura, string codigoEtiqueta, string codigoEtiquetaChapa, string codigoPedidoCliente, DateTime? dataConfirmacaoPedidoFim,
            DateTime? dataConfirmacaoPedidoInicio, DateTime? dataEntregaFim, DateTime? dataEntregaInicio, DateTime? dataFabricaFim, DateTime? dataFabricaInicio, DateTime? dataLeituraFim,
            DateTime? dataLeituraInicio, bool disponiveisLeituraSetor, float espessura, int fastDelivery, int idCarregamento, int idCliente, int idCorVidro, int idFuncionario, int idImpressao,
            int idLiberarPedido, int idLoja, int idPedido, int idPedidoImportado, IEnumerable<int> idsAplicacao, IEnumerable<int> idsBeneficiamento, int idSetor, IEnumerable<int> idsProcesso,
            IEnumerable<int> idsRota, IEnumerable<int> idsSubgrupo, int largura, string nomeCliente, bool pecaParadaProducao, string pecasProducaoCanceladas, bool pecasRepostas, string planoCorte,
            ProdutoComposicao produtoComposicao, bool setoresAnteriores, bool setoresPosteriores, int situacaoPedido, IEnumerable<int> situacoes, int tipoEntrega, TipoRetorno tipoRetorno,
            IEnumerable<int> tiposPedido)
        {
            #region Declaração de variáveis

            // Define se ao filtrar pela data de entrega será filtrado também pela data de fábrica
            var filtrarDataFabrica = ProducaoConfig.BuscarDataFabricaConsultaProducao;
            var usarJoin = idSetor > 0 && (dataLeituraInicio > DateTime.MinValue || dataLeituraFim > DateTime.MinValue);
            var sql = string.Empty;

            #endregion

            #region Consulta

            sql = string.Format(@"SELECT ped.IdPedido FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN cliente cli ON (ped.IdCli = cli.Id_Cli)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN setor s ON (ppp.IdSetor = s.IdSetor)
                    LEFT JOIN liberarpedido lp ON (ped.IdLiberarPedido = lp.IdLiberarPedido)
                    LEFT JOIN etiqueta_aplicacao apl ON (IF(ped.TipoPedido={0}, a.IdAplicacao, pp.IdAplicacao) = apl.IdAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (IF(ped.TipoPedido={0}, a.IdProcesso, pp.IdProcesso) = prc.IdProcesso) ",
                // Posição 0.
                (int)Pedido.TipoPedidoEnum.MaoDeObra);

            #endregion

            #region Filtros

            if (filtrarDataFabrica || dataFabricaInicio > DateTime.MinValue|| dataFabricaFim > DateTime.MinValue)
            {
                sql += " LEFT JOIN pedido_espelho pedEsp ON (ped.IdPedido = pedEsp.IdPedido)";
            }

            if (usarJoin)
            {
                sql += " LEFT JOIN leitura_producao lp1 ON (ppp.IdProdPedProducao = lp1.IdProdPedProducao)";
            }
            
            sql += " WHERE 1 ";
            
            if (idCarregamento > 0)
            {
                sql += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento WHERE IdCarregamento={0})", idCarregamento);
            }

            if (idCarregamento > 0)
            {
                sql += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento WHERE IdCarregamento={0})", idCarregamento);
            }

            if (idLiberarPedido > 0)
            {
                var idsPedidoPelaLiberacao = PedidoDAO.Instance.GetIdsByLiberacao((uint)idLiberarPedido);

                if (idsPedidoPelaLiberacao?.Count() > 0)
                {
                    sql += string.Format(" AND ped.IdPedido IN ({0})", string.Join(",", idsPedidoPelaLiberacao));
                }
            }

            if (idLoja > 0)
            {
                sql += string.Format(" AND ped.IdLoja={0}", idLoja);
            }

            if (idPedido > 0)
            {
                sql += string.Format(" AND (ped.IdPedido={0}", idPedido);

                // Na vidrália/colpany não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira
                // teria que mudar totalmente a forma de fazer o count
                if (ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido && PedidoDAO.Instance.IsPedidoReposto((uint)idPedido))
                {
                    sql += string.Format(" OR ped.IdPedidoAnterior={0}", idPedido);
                }

                if (PedidoDAO.Instance.IsPedidoExpedicaoBox((uint)idPedido))
                {
                    sql += string.Format(" OR ppp.IdPedidoExpedicao={0}", idPedido);
                }

                sql += ")";
            }

            if (idPedidoImportado > 0)
            {
                sql += " AND ped.CodCliente=?idPedidoImportado AND ped.Importado IS NOT NULL AND ped.Importado=1";
            }

            if (!string.IsNullOrEmpty(codigoEtiqueta))
            {
                var idProdPedProducaoPelaEtiqueta = ObtemIdProdPedProducao(codigoEtiqueta) ?? ObtemIdProdPedProducaoCanc(null, codigoEtiqueta);

                sql += idProdPedProducaoPelaEtiqueta > 0 ? string.Format(" AND ppp.IdProdPedProducao={0}", idProdPedProducaoPelaEtiqueta) : " AND 0=1";
            }

            if (!string.IsNullOrEmpty(codigoPedidoCliente))
            {
                sql += " AND (ped.CodCliente LIKE ?codigoPedidoCliente OR pp.PedCli LIKE ?codigoPedidoCliente OR a.Ambiente LIKE ?codigoPedidoCliente)";
            }

            if (idsRota?.Count() > 0)
            {
                sql += string.Format(" AND ped.IdCli IN (SELECT * FROM (SELECT IdCliente FROM rota_cliente WHERE IdRota IN ({0})) AS temp1)", string.Join(",", idsRota));
            }

            if (idImpressao > 0)
            {
                sql += string.Format(@" AND IF(!COALESCE(ppp.PecaReposta, 0), ppp.IdImpressao={0}, COALESCE(ppp.NumEtiqueta, ppp.NumEtiquetaCanc) IN
                    (SELECT * FROM (SELECT CONCAT(IdPedido, '-', PosicaoProd, '.', ItemEtiqueta, '/', QtdeProd)
                        FROM produto_impressao WHERE !COALESCE(Cancelado, 0) AND IdImpressao={0}) AS temp))", idImpressao);
            }

            if (idCliente > 0)
            {
                sql += string.Format(" AND ped.IdCli={0}", idCliente);
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var idsCliente = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);

                sql += string.Format(" AND ped.IdCli IN ({0})", idsCliente);
            }

            if (idFuncionario > 0)
            {
                sql += string.Format(" AND ped.IdFunc={0}", idFuncionario);
            }

            if (situacoes?.Count() > 0)
            {
                var sqlSituacoes = " AND (0=1 ";

                foreach (var situacao in situacoes)
                {
                    switch (situacao)
                    {
                        case 1:
                        case 2:
                            sqlSituacoes += string.Format(" OR ppp.Situacao={0}", situacao);
                            break;
                        case 3:
                            sqlSituacoes += string.Format(" OR (ppp.SituacaoProducao={0} AND ppp.Situacao={1})", (int)SituacaoProdutoProducao.Pendente, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            break;
                        case 4:
                            sqlSituacoes += string.Format(" OR (ppp.SituacaoProducao={0} AND ppp.Situacao={1})", (int)SituacaoProdutoProducao.Pronto, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            break;
                        case 5:
                            sqlSituacoes += string.Format(" OR (ppp.SituacaoProducao={0} AND ppp.Situacao={1})", (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                            break;
                    }
                }

                sqlSituacoes += ")";                
                sql += sqlSituacoes;
            }

            if (situacaoPedido > 0)
            {
                sql += string.Format(" AND ped.Situacao={0}", situacaoPedido);
            }

            /* Chamado 49413. */
            if (produtoComposicao > 0)
            {
                switch (produtoComposicao)
                {
                    case ProdutoComposicao.ProdutoComIdProdPedParent:
                        sql += " AND pp.IdProdPedParent IS NOT NULL";
                        break;

                    case ProdutoComposicao.ProdutoSemIdProdPedParent:
                        sql += " AND pp.IdProdPedParent IS NULL";
                        break;
                }
            }
            
            if (dataLeituraInicio > DateTime.MinValue)
            {
                if (situacoes?.Any(f => f == (int)ProdutoPedidoProducao.SituacaoEnum.Perda) ?? false)
                {
                    sql += " AND ppp.DataPerda>=?dataLeituraInicio";
                }

                if (idSetor > 0)
                {
                    sql += string.Format(" AND lp1.IdSetor={0} AND lp1.DataLeitura>=?dataLeituraInicio", idSetor);
                }
            }

            if (dataLeituraFim > DateTime.MinValue)
            {
                if (situacoes?.Any(f => f == (int)ProdutoPedidoProducao.SituacaoEnum.Perda) ?? false)
                {
                    sql += " AND ppp.DataPerda<=?dataLeituraFim";
                }

                if (idSetor > 0)
                {
                    sql += string.Format(" AND lp1.IdSetor={0} AND lp1.DataLeitura<=?dataLeituraFim", idSetor);
                }
            }

            if (dataEntregaInicio > DateTime.MinValue)
            {
                sql += " AND ped.DataEntrega>=?dataEntregaInicio";
            }

            if (dataEntregaFim > DateTime.MinValue)
            {
                sql += " AND ped.DataEntrega<=?dataEntregaFim";
            }

            if (dataFabricaInicio > DateTime.MinValue)
            {
                sql += " AND (pedEsp.DataFabrica>=?dataFabricaInicio)";
            }

            if (dataFabricaFim > DateTime.MinValue)
            {
                sql += " AND pedEsp.DataFabrica<=?dataFabricaFim";
            }

            if (dataConfirmacaoPedidoInicio > DateTime.MinValue || dataConfirmacaoPedidoFim > DateTime.MinValue)
            {
                var idsPedidoPelaDataConfirmacao = PedidoDAO.Instance.ObtemIdsPelaDataConf(dataConfirmacaoPedidoInicio, dataConfirmacaoPedidoFim);

                if (!string.IsNullOrEmpty(idsPedidoPelaDataConfirmacao))
                {
                    sql += string.Format(" AND ped.IdPedido IN ({0})", idsPedidoPelaDataConfirmacao);
                }
            }

            if (idsBeneficiamento?.Count() > 0)
            {
                var redondo = BenefConfigDAO.Instance.TemBenefRedondo(idsBeneficiamento) ? " OR pp.Redondo=1" : string.Empty;

                sql += string.Format(" AND (ppp.IdProdPed IN (SELECT DISTINCT IdProdPed FROM produto_pedido_espelho_benef WHERE IdBenefConfig IN ({0})) {1})", string.Join(",",  idsBeneficiamento),
                    redondo);
            }

            if ((idsSubgrupo?.Any(f => f > 0)).GetValueOrDefault())
            {
                sql += string.Format(" AND p.IdSubgrupoProd IN ({0})", idsSubgrupo);
            }

            if (tipoEntrega > 0)
            {
                sql += string.Format(" AND ped.TipoEntrega={0}", tipoEntrega);
            }

            if (tiposPedido?.Count() > 0)
            {
                var filtroTiposPedido = new List<Pedido.TipoPedidoEnum>();

                if (tiposPedido.Any(f => f == 1))
                {
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.Venda);
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.Revenda);
                }

                if (tiposPedido.Any(f => f == 2))
                {
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.Producao);
                }

                if (tiposPedido.Any(f => f == 3))
                {
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.MaoDeObra);
                }

                if (tiposPedido.Any(f => f == 4))
                {
                    filtroTiposPedido.Add(Pedido.TipoPedidoEnum.MaoDeObraEspecial);
                }

                if (filtroTiposPedido.Count > 0)
                {
                    sql += string.Format(" AND ped.TipoPedido IN ({0})", string.Join(",", filtroTiposPedido.Select(f => (int)f)));
                }
            }

            if (altura > 0)
            {
                sql += string.Format(" AND IF(ped.TipoPedido={0}, a.Altura, IF(pp.AlturaReal > 0, pp.AlturaReal, pp.Altura))={1}", (int)Pedido.TipoPedidoEnum.MaoDeObra, altura);
            }

            if (largura > 0)
            {
                sql += string.Format(" AND IF(ped.TipoPedido={0}, a.Largura, IF(pp.Redondo, 0, IF(pp.LarguraReal > 0, pp.LarguraReal, pp.Largura)))={1}", (int)Pedido.TipoPedidoEnum.MaoDeObra, largura);
            }

            if (idCorVidro > 0)
            {
                sql += string.Format(" AND p.IdCorVidro={0}", idCorVidro);
            }

            if (espessura > 0)
            {
                sql += " AND p.Espessura=?espessura";
            }

            if ((idsProcesso?.Any(f => f > 0)).GetValueOrDefault())
            {
                sql += string.Format(" AND pp.IdProcesso IN ({0})", idsProcesso);
            }

            if ((idsAplicacao?.Any(f => f > 0)).GetValueOrDefault())
            {
                sql += string.Format(" AND pp.IdAplicacao IN ({0})", idsAplicacao);
            }

            if (tipoRetorno == TipoRetorno.EntradaEstoque)
            {
                sql += string.Format(" AND COALESCE(ppp.EntrouEstoque, 0)=0 AND ped.TipoPedido={0}", (int)Pedido.TipoPedidoEnum.Producao);
            }
            else if (tipoRetorno == TipoRetorno.AguardandoExpedicao)
            {
                sql += string.Format(@" AND ped.TipoPedido<>{0} AND ped.IdPedido IN
                    (SELECT * FROM (SELECT IdPedido FROM produtos_liberar_pedido plp 
                        LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido) 
                    WHERE lp.Situacao<>{1}) AS temp) AND ppp.SituacaoProducao<>{2} AND ppp.Situacao={3}",
                    (int)Pedido.TipoPedidoEnum.Producao, (int)LiberarPedido.SituacaoLiberarPedido.Cancelado, (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
            }

            if (!string.IsNullOrEmpty(pecasProducaoCanceladas))
            {
                var filtroSituacoesProducao = new List<ProdutoPedidoProducao.SituacaoEnum>();

                if (pecasProducaoCanceladas.Split(',').Any(f => f == "0"))
                {
                    filtroSituacoesProducao.Add(ProdutoPedidoProducao.SituacaoEnum.Producao);
                    filtroSituacoesProducao.Add(ProdutoPedidoProducao.SituacaoEnum.Perda);
                }

                if (pecasProducaoCanceladas.Split(',').Any(f => f == "1"))
                {
                    filtroSituacoesProducao.Add(ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra);
                }

                if (pecasProducaoCanceladas.Split(',').Any(f => f == "2"))
                {
                    filtroSituacoesProducao.Add(ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda);
                }

                sql += string.Format(" AND ppp.Situacao IN ({0})", string.Join(",", filtroSituacoesProducao.Select(f => (int)f)));
            }
            else
            {
                sql += " AND 0=1";
            }
            
            if (!string.IsNullOrEmpty(planoCorte))
            {
                sql += " AND ppp.PlanoCorte=?planoCorte";
            }

            if (!string.IsNullOrEmpty(codigoEtiquetaChapa))
            {
                sql += @" AND COALESCE(ppp.NumEtiqueta, ppp.NumEtiquetaCanc) IN (SELECT * FROM (
                    SELECT pip.NumEtiqueta
                    FROM produto_impressao pip
                        INNER JOIN chapa_corte_peca ccp ON (pip.IdProdImpressao=ccp.IdProdImpressaoPeca)
                        INNER JOIN produto_impressao pic ON (ccp.IdProdImpressaoChapa=pic.IdProdImpressao)
                    WHERE pic.NumEtiqueta=?codigoEtiquetaChapa) AS temp)";
            }

            if (fastDelivery > 0)
            {
                sql += string.Format(" AND COALESCE(ped.FastDelivery, 0)={0}", fastDelivery == 1 ? "1" : "0");
            }

            if (pecaParadaProducao)
            {
                sql += " AND ppp.PecaParadaProducao IS NOT NULL AND ppp.PecaParadaProducao=1";
            }

            if (pecasRepostas)
            {
                sql += " AND ppp.PecaReposta";
            }
            
            if (idSetor > 0 || idSetor == -1)
            {
                if (!setoresPosteriores && !setoresAnteriores && !disponiveisLeituraSetor)
                {
                    if (idSetor > 0)
                    {
                        sql += string.Format(" AND ppp.IdSetor={0}", idSetor);

                        // Filtro para impressão de etiqueta.
                        if (Utils.ObtemSetor((uint)idSetor).NumeroSequencia == 1)
                        {
                            sql += string.Format(" AND EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0} AND DataLeitura IS NOT NULL)", idSetor);
                        }
                    }
                    // Etiqueta não impressa.
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

                        sql += string.Format(@" AND {0} <= ALL (SELECT NumSeq FROM setor WHERE IdSetor=ppp.IdSetor) AND
                            (SELECT COUNT(*) FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={1}) > 0", Utils.ObtemSetor((uint)idSetor).NumeroSequencia, idSetor);
                    }
                    else if (disponiveisLeituraSetor)
                    {
                        if (idSetor <= 1)
                        {
                            sql += " AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND DataLeitura IS NOT NULL)";
                        }
                        else
                        {
                            sql +=
                                string.Format(@"
                                    AND EXISTS
                                    (
                                        SELECT ppp1.*
                                        FROM produto_pedido_producao ppp1
	                                        INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp1.IdProdPedProducao)
                                        WHERE rpe.IdSetor = {0}
	                                        AND ppp1.IdProdPedProducao = ppp.IdProdPedProducao
                                            AND ppp1.IdSetor =
                                                /* Se o setor filtrado for o primeiro setor do roteiro, busca somente as peças que estiverem no setor Impressão de Etiqueta. */
                                                IF ({0} =
                                                    (
    	                                                SELECT rpe.IdSetor
		                                                FROM produto_pedido_producao ppp2
			                                                INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp2.IdProdPedProducao)
    		                                                INNER JOIN setor s ON (rpe.IdSetor = s.IdSetor)
		                                                WHERE ppp2.IdProdPedProducao = ppp.IdProdPedProducao
                                                            AND ppp2.IdProdPedProducao IN (SELECT lp1.IdProdPedProducao FROM leitura_producao lp1)
		                                                ORDER BY s.NumSeq ASC
		                                                LIMIT 1
                                                    ), 1,
                                                    /* Senão, busca o próximo setor a ser lido no roteiro. */
                                                    (
    	                                                SELECT rpe.IdSetor
		                                                FROM produto_pedido_producao ppp2
			                                                INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp2.IdProdPedProducao)
    		                                                INNER JOIN setor s ON (rpe.IdSetor = s.IdSetor)
		                                                WHERE ppp2.IdProdPedProducao = ppp.IdProdPedProducao
			                                                AND s.NumSeq < (SELECT NumSeq FROM setor WHERE IdSetor = {0})
		                                                ORDER BY s.NumSeq DESC
		                                                LIMIT 1
                                                    ))
                                    )", idSetor);
                        }
                    }
                }
            }

            #endregion

            return string.Format("{0} GROUP BY ped.IdPedido", sql);
        }

        /// <summary>
        /// Consulta que retorna os IDs dos pedidos através dos produtos de produção.
        /// </summary>
        public List<int> ObterIdsPedidoRelatorioProducao(bool aguardandoEntradaEstoque, bool aguardandoExpedicao, int altura, string codigoEtiqueta, string codigoEtiquetaChapa,
            string codigoPedidoCliente, DateTime? dataConfirmacaoPedidoFim, DateTime? dataConfirmacaoPedidoInicio, DateTime? dataEntregaFim, DateTime? dataEntregaInicio, DateTime? dataFabricaFim,
            DateTime? dataFabricaInicio, DateTime? dataLeituraFim, DateTime? dataLeituraInicio, float espessura, int fastDelivery, int idCarregamento, int idCliente, int idCorVidro,
            int idFuncionario, int idImpressao, int idLiberarPedido, int idLoja, int idPedido, int idPedidoImportado, IEnumerable<int> idsAplicacao, IEnumerable<int> idsBeneficiamento, int idSetor,
            IEnumerable<int> idsProcesso, IEnumerable<int> idsRota, IEnumerable<int> idsSubgrupo, int largura, string nomeCliente, bool pecaParadaProducao, string pecasProducaoCanceladas,
            bool pecasRepostas, string planoCorte, ProdutoComposicao produtoComposicao, int situacaoPedido, IEnumerable<int> situacoes, int tipoEntrega, IEnumerable<int> tiposPedido, int tipoSituacao)
        {
            var listaVazia = ProducaoConfig.TelaConsulta.TelaVaziaPorPadrao;
            var setoresAnteriores = tipoSituacao == 1;
            var setoresPosteriores = tipoSituacao == 2;
            var disponiveisLeituraSetor = tipoSituacao == 3;
            var tipoRetorno = aguardandoExpedicao ? TipoRetorno.AguardandoExpedicao : aguardandoEntradaEstoque ? TipoRetorno.EntradaEstoque : TipoRetorno.Normal;
            var sql = string.Empty;
            GDAParameter[] parametros;

            // Caso não seja utilizado nenhum filtro, retornar uma listagem vazia, para a tela carregar mais rápido.
            if (listaVazia && FiltrosVazios(idCarregamento, (uint)idLiberarPedido, (uint)idPedido, idPedidoImportado.ToString(), (uint)idImpressao,
                idsRota?.Count() > 0 ? string.Join(",", idsRota) : string.Empty, codigoPedidoCliente, (uint)idCliente, nomeCliente, codigoEtiqueta,
                dataLeituraInicio > DateTime.MinValue ? dataLeituraInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                dataLeituraFim > DateTime.MinValue ? dataLeituraFim.Value.ToString("dd/MM/yyyy"): string.Empty,
                dataEntregaInicio > DateTime.MinValue ? dataEntregaInicio.Value.ToString("dd/MM/yyyy"): string.Empty,
                dataEntregaFim > DateTime.MinValue ? dataEntregaFim.Value.ToString("dd/MM/yyyy"): string.Empty,
                dataFabricaInicio > DateTime.MinValue ? dataFabricaInicio.Value.ToString("dd/MM/yyyy"): string.Empty,
                dataFabricaFim > DateTime.MinValue ? dataFabricaFim.Value.ToString("dd/MM/yyyy"): string.Empty,
                dataConfirmacaoPedidoInicio > DateTime.MinValue ? dataConfirmacaoPedidoInicio.Value.ToString("dd/MM/yyyy"): string.Empty,
                dataConfirmacaoPedidoFim > DateTime.MinValue ? dataConfirmacaoPedidoFim.Value.ToString("dd/MM/yyyy"): string.Empty,
                idSetor, string.Join(",", situacoes), situacaoPedido, tipoSituacao, idsSubgrupo?.Count() > 0 ? string.Join(",", idsSubgrupo) : string.Empty, (uint)tipoEntrega, pecasProducaoCanceladas,
                (uint)idFuncionario, tiposPedido.Count() > 0 ? string.Join(",", tiposPedido) : string.Empty, (uint)idCorVidro, altura, largura, espessura,
                idsProcesso?.Count() > 0 ? string.Join(",", idsProcesso) : string.Empty, idsAplicacao?.Count() > 0 ? string.Join(",", idsAplicacao) : string.Empty, aguardandoExpedicao,
                aguardandoEntradaEstoque, idsBeneficiamento?.Count() > 0 ? string.Join(",", idsBeneficiamento) : string.Empty, planoCorte, codigoEtiquetaChapa, (uint)fastDelivery, pecaParadaProducao,
                pecasRepostas, (uint)idLoja, 0))
            {
                return new List<int>();
            }

            sql = SqlIdsPedidoRelatorioProducao(altura, codigoEtiqueta, codigoEtiquetaChapa, codigoPedidoCliente, dataConfirmacaoPedidoFim, dataConfirmacaoPedidoInicio, dataEntregaFim,
                dataEntregaInicio, dataFabricaFim, dataFabricaInicio, dataLeituraFim, dataLeituraInicio, disponiveisLeituraSetor, espessura, fastDelivery, idCarregamento, idCliente, idCorVidro,
                idFuncionario, idImpressao, idLiberarPedido, idLoja, idPedido, idPedidoImportado, idsAplicacao, idsBeneficiamento, idSetor, idsProcesso, idsRota, idsSubgrupo, largura, nomeCliente,
                pecaParadaProducao, pecasProducaoCanceladas, pecasRepostas, planoCorte, produtoComposicao, setoresAnteriores, setoresPosteriores, situacaoPedido, situacoes, tipoEntrega, tipoRetorno,
                tiposPedido);

            parametros = ObterParametrosIdsPedidoRelatorioProducao(codigoEtiquetaChapa, codigoPedidoCliente, dataEntregaFim, dataEntregaInicio, dataFabricaFim, dataFabricaInicio, dataLeituraFim,
                dataLeituraInicio, espessura, idPedidoImportado, planoCorte);

            return ExecuteMultipleScalar<int>(sql, parametros);
        }

        /// <summary>
        /// Preenche os parâmetros da consulta que retorna os IDs dos pedidos através dos produtos de produção.
        /// </summary>
        internal GDAParameter[] ObterParametrosIdsPedidoRelatorioProducao(string codigoEtiquetaChapa, string codigoPedidoCliente, DateTime? dataEntregaFim, DateTime? dataEntregaInicio,
            DateTime? dataFabricaFim, DateTime? dataFabricaInicio, DateTime? dataLeituraFim, DateTime? dataLeituraInicio, float espessura, int idPedidoImportado, string planoCorte)
        {
            var parametros = new List<GDAParameter>();
            
            if (!string.IsNullOrEmpty(codigoEtiquetaChapa))
            {
                parametros.Add(new GDAParameter("?codigoEtiquetaChapa", codigoEtiquetaChapa));
            }

            if (!string.IsNullOrEmpty(codigoPedidoCliente))
            {
                parametros.Add(new GDAParameter("?codigoPedidoCliente", string.Format("%{0}%", codigoPedidoCliente)));
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

            if (espessura > 0)
            {
                parametros.Add(new GDAParameter("?espessura", espessura));
            }

            if (idPedidoImportado > 0)
            {
                parametros.Add(new GDAParameter("?idPedidoImportado", idPedidoImportado));
            }

            if (!string.IsNullOrEmpty(planoCorte))
            {
                parametros.Add(new GDAParameter("?planoCorte", planoCorte));
            }

            return parametros.Count > 0 ? parametros.ToArray() : null;
        }

        #endregion

        #region Pesquisa produtos de produção filhos

        /// <summary>
        /// SQL da consulta que retorna os produtos de produção filhos para a tela de consulta de produção.
        /// </summary>
        internal string SqlProdutosProducaoFilho(int idProdPedProducaoParent, bool selecionar)
        {
            #region Declaração de variáveis

            var campos = string.Empty;
            var sql = string.Empty;
            // Define se ao filtrar pela data de entrega será filtrado também pela data de fábrica.
            var filtrarDataFabrica = ProducaoConfig.BuscarDataFabricaConsultaProducao;

            #endregion

            #region Consulta
            
            campos = selecionar ? string.Format(@"ppp.IdProdPedProducao, ppp.IdProdPed, ppp.Situacao, ppp.IdImpressao, ppp.PlanoCorte, ppp.NumEtiqueta, ppp.NumEtiquetaCanc, ppp.DataPerda, ppp.Obs,
                    ppp.SituacaoProducao, ppp.IdSetor, ppp.PecaReposta, ppp.TipoPerda, ppp.IdSubtipoPerda, ppp.TipoPerdaRepos, ppp.IdSubtipoPerdaRepos, ppp.DadosReposicaoPeca, ppp.PecaParadaProducao,
                    ped.IdPedido, ped.TipoPedido={0} AS PedidoMaoObra, ped.Situacao={1} AS PedidoCancelado, ped.DataEntrega, ped.DataEntregaOriginal, p.CodInterno,
                    CONCAT(p.Descricao, IF(pp.Redondo AND {2}, ' REDONDO', '')) AS DescrProduto,
                    IF(ped.TipoPedido={0}, a.Altura, IF(pp.AlturaReal > 0, pp.AlturaReal, pp.Altura)) AS Altura,
                    IF(ped.TipoPedido={0}, a.Largura, IF(pp.Redondo, 0, IF (pp.LarguraReal > 0, pp.LarguraReal, pp.Largura))) AS Largura,
                    IF(lp.Situacao={4}, lp.DataLiberacao, NULL) AS DataLiberacaoPedido, apl.CodInterno AS CodAplicacao, prc.CodInterno AS CodProcesso{3}",
                // Posição 0.
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                // Posição 1.
                (int)Pedido.SituacaoPedido.Cancelado,
                // Posição 2.
                (!BenefConfigDAO.Instance.CobrarRedondo()).ToString(),
                // Posição 3.
                filtrarDataFabrica ? ", ped_esp.DataFabrica AS DataEntregaFabrica" : string.Empty,
                // Posição 4.
                (int)LiberarPedido.SituacaoLiberarPedido.Liberado) :
                "COUNT(DISTINCT ppp.IdProdPedProducao)";

            sql = string.Format(@"SELECT {0}
                FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN liberarpedido lp ON (ped.IdLiberarPedido = lp.IdLiberarPedido)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN etiqueta_aplicacao apl ON (if(ped.tipoPedido={1}, a.idAplicacao, pp.idAplicacao) = apl.idAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (if(ped.tipoPedido={1}, a.idProcesso, pp.idProcesso) = prc.idProcesso)
                    {2}
                WHERE 1",
                // Posição 0.
                campos,
                // Posição 1.
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                // Posição 2.
                filtrarDataFabrica ? "LEFT JOIN pedido_espelho ped_esp ON (ped.IdPedido = pedEsp.IdPedido)" : string.Empty);

            #endregion

            #region Filtros

            sql += string.Format(" AND pp.IdProdPedParent IS NOT NULL AND ppp.Situacao IN ({0}, {1})", (int)ProdutoPedidoProducao.SituacaoEnum.Producao, (int)ProdutoPedidoProducao.SituacaoEnum.Perda);

            if (idProdPedProducaoParent > 0)
            {
                sql += string.Format(" AND ppp.IdProdPedProducaoParent={0}", idProdPedProducaoParent);
            }

            #endregion

            return sql;
        }

        /// <summary>
        /// Consulta que retorna os produtos de produção filhos para a tela de consulta de produção.
        /// </summary>
        public IList<ProdutoPedidoProducao> PesquisarProdutosProducaoFilho(int idProdPedProducaoParent, string sortExpression, int startRow, int pageSize)
        {
            var sql = string.Empty;
            var numeroRegistros = 0;
            ProdutoPedidoProducao[] produtosPedidoProducao;

            // Caso não seja utilizado nenhum filtro, retornar uma listagem vazia, para a tela carregar mais rápido.
            if (idProdPedProducaoParent <= 0)
            {
                return new List<ProdutoPedidoProducao>();
            }
            
            produtosPedidoProducao = objPersistence.LoadData(GetSqlWithLimit(SqlProdutosProducaoFilho(idProdPedProducaoParent, true), sortExpression, 0, pageSize, "ppp", string.Empty, false,
                !string.IsNullOrEmpty(sortExpression), out numeroRegistros)).ToArray();

            SetInfoPaging(sortExpression, 0, pageSize);
            GetSetores(ref produtosPedidoProducao);
            GetNumChapaCorte(ref produtosPedidoProducao);

            return produtosPedidoProducao;
        }

        /// <summary>
        /// Quantidade de registros retornados através da consulta que retorna os produtos de produção filhos para a tela de consulta de produção.
        /// </summary>
        public int PesquisarProdutosProducaoFilhoCount(int idProdPedProducaoParent)
        {
            if (idProdPedProducaoParent <= 0)
            {
                return 10000;
            }

            // Fica muito mais rápido sem usar a otimização (GetCountWithInfoPaging())
            return objPersistence.ExecuteSqlQueryCount(SqlProdutosProducaoFilho(idProdPedProducaoParent, false));
        }

        #endregion

        #region Pesquisa para reposição de peça

        /// <summary>
        /// SQL da consulta que retorna os produtos de produção para a tela de reposição de peça.
        /// </summary>
        internal string SqlProdutosProducaoReposicaoPeca(string codigoEtiqueta, out string filtroAdicional, int idPedido, int idSetor, int idTurno, bool selecionar, int situacao, out bool temFiltro)
        {
            #region Declaração de variáveis

            temFiltro = !selecionar;
            filtroAdicional = string.Empty;
            var sql = string.Empty;
            var campos = string.Empty;
            var usarJoin = idTurno > 0;

            #endregion

            #region Consulta

            campos = selecionar ? string.Format(@"ppp.IdProdPedProducao, ppp.IdProdPed, ppp.NumEtiqueta, s.Descricao AS DescrSetor, CONCAT(p.Descricao,
                IF(pp.Redondo AND {0}, ' REDONDO', '')) AS DescrProduto, IF(ped.TipoPedido={1}, a.Altura, IF(pp.AlturaReal > 0, pp.AlturaReal, pp.Altura)) AS Altura,
                IF(ped.TipoPedido={1}, a.Largura, IF(pp.Redondo, 0, IF (pp.LarguraReal > 0, pp.LarguraReal, pp.Largura))) AS Largura,
                ROUND(IF(ped.TipoPedido={1}, ((((50 - IF(MOD(a.Altura, 50) > 0, MOD(a.Altura, 50), 50)) + a.Altura) *
                    ((50 - IF(MOD(a.Largura, 50) > 0, MOD(a.Largura, 50), 50)) + a.Largura)) / 1000000) * a.Qtde, pp.TotM2Calc) / (pp.Qtde * IF(ped.TipoPedido={1}, a.Qtde, 1)), 4) AS TotM2",
                // Posição 0.
                (!BenefConfigDAO.Instance.CobrarRedondo()).ToString(),
                // Posição 1.
                (int)Pedido.TipoPedidoEnum.MaoDeObra) :
                "COUNT(DISTINCT ppp.IdProdPedProducao)";

            sql = string.Format(@"
                SELECT {0}
                FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN
                        (
                            SELECT p.IdProd, p.Descricao FROM produto p
                        ) p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN 
                        (
                            SELECT s.IdSetor, s.Descricao FROM setor s
                        ) s ON (ppp.IdSetor = s.IdSetor)
                    {1}
                WHERE 1 ?filtroAdicional?",
                // Posição 0.
                campos,
                // Posição 1.
                usarJoin ? " LEFT JOIN leitura_producao lp1 ON (ppp.IdProdPedProducao = lp1.IdProdPedProducao)" : string.Empty);

            #endregion

            #region Filtros

            if (idPedido > 0)
            {
                sql += string.Format(" AND (ped.IdPedido={0}", idPedido);

                // Na vidrália/colpany não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira teria que mudar totalmente a forma de fazer o count.
                if (ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido && PedidoDAO.Instance.IsPedidoReposto((uint)idPedido))
                {
                    sql += string.Format(" OR ped.IdPedidoAnterior={0}", idPedido);
                }
                
                if (PedidoDAO.Instance.IsPedidoExpedicaoBox((uint)idPedido))
                {
                    sql += string.Format(" OR ppp.IdPedidoExpedicao={0}", idPedido);
                }

                sql += ")";

                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(codigoEtiqueta))
            {
                var idProdPedProducao = ObtemIdProdPedProducao(codigoEtiqueta) ?? ObtemIdProdPedProducaoCanc(null, codigoEtiqueta);

                filtroAdicional += idProdPedProducao > 0 ? string.Format(" AND ppp.IdProdPedProducao={0}", idProdPedProducao) : " AND FALSE";
            }

            if (situacao > 0)
            {
                if (situacao == 1 || situacao == 2)
                {
                    sql += string.Format(" AND ppp.Situacao={0}", situacao);
                    temFiltro = true;
                }
                else if (situacao == 3)
                {
                    sql += string.Format(" AND ppp.SituacaoProducao={0} AND ppp.Situacao={1}", (int)SituacaoProdutoProducao.Pendente, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                    temFiltro = true;
                }
                else if (situacao == 4)
                {
                    sql += string.Format(" AND ppp.SituacaoProducao={0} AND ppp.Situacao={1}", (int)SituacaoProdutoProducao.Pronto, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                    temFiltro = true;
                }
                else if (situacao == 5)
                {
                    sql += string.Format(" AND ppp.SituacaoProducao={0} AND ppp.Situacao={1}", (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                    temFiltro = true;
                }
            }

            filtroAdicional += string.Format(" AND ppp.Situacao IN ({0},{1})", (int)ProdutoPedidoProducao.SituacaoEnum.Producao, (int)ProdutoPedidoProducao.SituacaoEnum.Perda);

            if (idTurno > 0)
            {
                var inicioTurno = TurnoDAO.Instance.ObtemValorCampo<string>("Inicio", string.Format("IdTurno={0}", idTurno));
                var terminoTurno = TurnoDAO.Instance.ObtemValorCampo<string>("Termino", string.Format("IdTurno={0}", idTurno));
                var descricaoTurno = TurnoDAO.Instance.ObtemValorCampo<string>("Descricao", string.Format("IdTurno={0}", idTurno));

                if (TimeSpan.Parse(inicioTurno) <= TimeSpan.Parse(terminoTurno))
                {
                    sql += string.Format(@" AND lp1.IdSetor = ppp.IdSetor AND lp1.DataLeitura >= CAST(CONCAT(DATE_FORMAT(lp1.DataLeitura, '%Y-%m-%d'), ' {0}') AS DATETIME)
                        AND lp1.DataLeitura <= CAST(CONCAT(DATE_FORMAT(lp1.DataLeitura, '%Y-%m-%d'), ' {1}') AS DATETIME)", inicioTurno, terminoTurno);
                }
                else
                {
                    sql += string.Format(@" AND lp1.IdSetor = ppp.IdSetor AND (lp1.DataLeitura >= CAST(CONCAT(DATE_FORMAT(lp1.DataLeitura, '%Y-%m-%d'), ' {0}') AS DATETIME)
                        OR lp1.DataLeitura < CAST(CONCAT(DATE_FORMAT(lp1.DataLeitura, '%Y-%m-%d'), ' {1}') AS DATETIME))", inicioTurno, terminoTurno);
                }

                temFiltro = true;
            }

            if (idSetor > 0 || idSetor == -1)
            {
                if (idSetor > 0)
                {
                    filtroAdicional += string.Format(" AND ppp.IdSetor={0}", idSetor);

                    // Filtro para impressão de etiqueta
                    if (Utils.ObtemSetor((uint)idSetor).NumeroSequencia == 1)
                    {
                        sql += string.Format(" AND EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0} AND DataLeitura IS NOT NULL)", idSetor);
                        temFiltro = true;
                    }
                }
                // Etiqueta não impressa.
                else if (idSetor == -1)
                {
                    sql += " AND ppp.IdSetor=1 AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND DataLeitura IS NOT NULL)";
                    temFiltro = true;
                }

            }

            if (usarJoin && selecionar)
            {
                sql += " GROUP BY ppp.IdProdPedProducao";
            }

            #endregion

            return sql;
        }

        /// <summary>
        /// Consulta que retorna os produtos de produção para a tela de reposição de peça.
        /// </summary>
        public ProdutoPedidoProducao[] PesquisarProdutosProducaoReposicaoPeca(string codigoEtiqueta, int idPedido, int idSetor, int idTurno, int pageSize, int situacao, string sortExpression,
            int startRow)
        {
            var temFiltro = false;
            var filtroAdicional = string.Empty;
            var sql = string.Empty;
            var numeroRegistros = 0;
            ProdutoPedidoProducao[] produtosPedidoProducao;

            if (idPedido == 0)
            {
                return null;
            }

            sql = SqlProdutosProducaoReposicaoPeca(codigoEtiqueta, out filtroAdicional, idPedido, idSetor, idTurno, true, situacao, out temFiltro)
                .Replace(FILTRO_ADICIONAL, temFiltro ? filtroAdicional : string.Empty);

            sortExpression = string.IsNullOrEmpty(sortExpression) ? "ppp.IdProdPedProducao DESC" : sortExpression;

            if (sortExpression == "pp.IdPedido DESC" && !temFiltro)
            {
                filtroAdicional = "pp.IdProdPed IN (SELECT DISTINCT IdProdPed FROM produto_pedido_producao)";
            }

            produtosPedidoProducao = objPersistence.LoadData(GetSqlWithLimit(sql, sortExpression, startRow, pageSize, "ppp", filtroAdicional, !temFiltro, false, out numeroRegistros)).ToArray();

            SetInfoPaging(sortExpression, startRow, pageSize);
            GetSetores(ref produtosPedidoProducao);

            return produtosPedidoProducao;
        }

        /// <summary>
        /// Quantidade de registros retornados através da consulta que retorna os produtos de produção para a tela de reposição de peça.
        /// </summary>
        public int PesquisarProdutosProducaoReposicaoPecaCount(string codigoEtiqueta, int idPedido, int idSetor, int idTurno, int situacao)
        {
            var temFiltro = false;
            var filtroAdicional = string.Empty;
            var sql = string.Empty;

            if (idPedido == 0)
            {
                return 0;
            }

            sql = SqlProdutosProducaoReposicaoPeca(codigoEtiqueta, out filtroAdicional, idPedido, idSetor, idTurno, false, situacao, out temFiltro)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : string.Empty);

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        #endregion

        #region Pesquisa para marcação de peça pronta

        /// <summary>
        /// SQL da consulta que retorna os produtos de produção para a tela de marcação de peça pronta.
        /// </summary>
        internal string SqlProdutosProducaoMarcarPecaPronta(string codigoEtiqueta, int idPedido, int idSetor, bool perda, bool selecionar)
        {
            #region Declaração de variáveis
            
            // Mostra as peças em todos os setores, se for marcação de perda.
            idSetor = perda ? 0 : idSetor;
            var situacao = perda ? ProdutoPedidoProducao.SituacaoEnum.Producao : (ProdutoPedidoProducao.SituacaoEnum?)null;
            var campos = string.Empty;
            var sql = string.Empty;

            #endregion

            #region Consulta

            campos = selecionar ? string.Format(@"ppp.IdProdPedProducao, ppp.NumEtiqueta, ppp.IdProdPed,
                IF(ped.TipoPedido={0}, a.Altura, IF(pp.AlturaReal > 0, pp.AlturaReal, pp.Altura)) AS Altura,
                IF(ped.TipoPedido={0}, a.Largura, IF(pp.Redondo, 0, IF(pp.LarguraReal > 0, pp.LarguraReal, pp.Largura))) AS Largura,
                CONCAT(p.Descricao, IF(pp.Redondo AND {1}, ' REDONDO', '')) AS DescrProduto,
                ROUND(IF(ped.TipoPedido={0}, ((((50 - IF(MOD(a.Altura, 50) > 0, MOD(a.Altura, 50), 50)) + a.Altura) *
                    ((50 - IF(MOD(a.Largura, 50) > 0, MOD(a.Largura, 50), 50)) + a.Largura)) / 1000000) *
                    a.Qtde, pp.TotM2Calc) / (pp.Qtde * IF(ped.TipoPedido={0}, a.Qtde, 1)), 4) AS TotM2,
                s.Descricao AS DescrSetor",
                // Posição 0.
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                // Posição 1.
                (!BenefConfigDAO.Instance.CobrarRedondo()).ToString()) :
                "COUNT(DISTINCT ppp.IdProdPedProducao)";

            sql = string.Format(@"SELECT {0}
                FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN setor s ON (ppp.IdSetor = s.IdSetor)
                WHERE ppp.Situacao IN ({1}, {2})",
                // Posição 0.
                campos,
                // Posição 1.
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                // Posição 2.
                (int)ProdutoPedidoProducao.SituacaoEnum.Perda);

            #endregion

            #region Filtros

            if (idPedido > 0)
            {
                sql += string.Format(" AND (ped.IdPedido={0}", idPedido);

                // Na vidrália/colpany não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira teria que mudar totalmente a forma de fazer o count.
                if (ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido && PedidoDAO.Instance.IsPedidoReposto((uint)idPedido))
                {
                    sql += string.Format(" OR ped.IdPedidoAnterior={0}", idPedido);
                }
                
                if (PedidoDAO.Instance.IsPedidoExpedicaoBox((uint)idPedido))
                {
                    sql += string.Format(" OR ppp.IdPedidoExpedicao={0}", idPedido);
                }

                sql += ")";
            }

            if (!string.IsNullOrEmpty(codigoEtiqueta))
            {
                var idProdPedProducao = ObtemIdProdPedProducao(null, codigoEtiqueta) ?? ObtemIdProdPedProducaoCanc(null, codigoEtiqueta);

                sql += idProdPedProducao > 0 ? string.Format(" AND ppp.IdProdPedProducao={0}", idProdPedProducao) : " AND 0=1";
            }

            if (situacao != null)
            {             
                sql += string.Format(" AND ppp.Situacao={0}", (int)situacao.Value);
            }

            if (idSetor > 0 || idSetor == -1)
            {
                if (idSetor == 1)
                {
                    sql += " AND ppp.IdSetor=1 AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND DataLeitura IS NOT NULL)";
                }
                else
                {
                    sql += string.Format(" AND NOT EXISTS (SELECT * FROM leitura_producao WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0})", idSetor);
                }

                // Retorna apenas as peças de roteiro se o setor for de roteiro.
                if (Utils.ObtemSetor((uint)idSetor).SetorPertenceARoteiro)
                {
                    sql += string.Format(" AND EXISTS (SELECT * FROM roteiro_producao_etiqueta WHERE IdProdPedProducao=ppp.IdProdPedProducao AND IdSetor={0})", idSetor);
                }
            }

            #endregion

            return sql;
        }

        /// <summary>
        /// Consulta que retorna os produtos de produção para a tela de marcação de peça pronta.
        /// </summary>
        public IList<ProdutoPedidoProducao> PesquisarProdutosProducaoMarcarPecaPronta(string codigoEtiqueta, int idPedido, int idSetor, int pageSize, bool perda, string sortExpression, int startRow)
        {
            var sql = string.Empty;
            var numeroRegistros = 0;
            ProdutoPedidoProducao[] produtosPedidoProducao;

            sql = SqlProdutosProducaoMarcarPecaPronta(codigoEtiqueta, idPedido, idSetor, perda, true);
            sortExpression = string.IsNullOrEmpty(sortExpression) ? "ppp.IdProdPedProducao DESC" : sortExpression;

            produtosPedidoProducao = objPersistence.LoadData(GetSqlWithLimit(sql, sortExpression, !string.IsNullOrEmpty(codigoEtiqueta) ? 0 : startRow, pageSize, "ppp", string.Empty, false, false,
                out numeroRegistros)).ToArray();
            SetInfoPaging(sortExpression, startRow, pageSize);
            GetSetores(ref produtosPedidoProducao);

            return produtosPedidoProducao.ToList();
        }

        /// <summary>
        /// Quantidade de registros retornados através da consulta que retorna os produtos de produção para a tela de marcação de peça pronta.
        /// </summary>
        public int PesquisarProdutosProducaoMarcarPecaProntaCount(string codigoEtiqueta, int idPedido, int idSetor, bool perda)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlProdutosProducaoMarcarPecaPronta(codigoEtiqueta, idPedido, idSetor, perda, false));
        }

        #endregion

        #region Pesquisa para relatório de liberação (peças canceladas)

        /// <summary>
        /// SQL da consulta que recupera as peças canceladas para o relatório de liberação.
        /// </summary>
        internal string SqlProdutosProducaoRelatorioLiberacao(string idsProdPedProducao)
        {
            #region Declaração de variáveis

            var campos = string.Empty;
            var sql = string.Empty;

            #endregion

            #region Consulta

            campos = string.Format(@"ppp.IdProdPedProducao, ppp.IdProdPed, pp.IdPedido, ppp.IdImpressao, ppp.Situacao, ppp.NumEtiqueta, ppp.NumEtiquetaCanc, ppp.PecaReposta, ppp.Obs,
                    ppp.TipoPerda, ppp.IdSubtipoPerda, ppp.TipoPerdaRepos, ppp.IdSubtipoPerdaRepos, CONCAT(p.Descricao, IF(pp.Redondo AND {0}, ' REDONDO', ''))) AS DescrProduto,
                    ped.TipoPedido={1} AS PedidoMaoObra, IF(ped.TipoPedido={1}, a.Altura, IF(pp.AlturaReal > 0, pp.AlturaReal, pp.Altura)) AS Altura,
                    IF(ped.TipoPedido={1}, a.Largura, IF(pp.Redondo, 0, if (pp.LarguraReal > 0, pp.LarguraReal, pp.Largura))) AS Largura",
                // Posição 0.
                (!BenefConfigDAO.Instance.CobrarRedondo()).ToString(),
                // Posição 1.
                (int)Pedido.TipoPedidoEnum.MaoDeObra);

            sql = string.Format(@"
                SELECT {0}
                FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                WHERE pp.IdProdPedParent IS NULL AND ppp.Situacao IN ({1})",
                // Posição 0.
                campos,
                // Posição 1.
                (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra);

            #endregion

            #region Filtros

            if (!string.IsNullOrEmpty(idsProdPedProducao))
            {
                sql += string.Format(" AND ppp.IdProdPedProducao IN ({0})", idsProdPedProducao);
            }

            #endregion

            return sql;
        }

        /// <summary>
        /// Recupera as peças canceladas para o relatório de liberação.
        /// </summary>
        public IList<ProdutoPedidoProducao> PesquisarProdutosProducaoRelatorioLiberacao(int idLiberarPedido, bool apenasMaoDeObra)
        {
            var sql = string.Empty;
            var itemEtiqueta = string.Empty;

            if (!PCPConfig.ExibirPecasCancLiberacao)
            {
                return new ProdutoPedidoProducao[0];
            }

            // Recupera os IdProdPed da liberação.
            sql = string.Format(@"SELECT plp.IdProdPed FROM produtos_liberar_pedido plp
                    INNER JOIN pedido ped ON (plp.IdPedido=ped.IdPedido)
                WHERE plp.IdLiberarPedido={0}", idLiberarPedido);

            if (apenasMaoDeObra)
            {
                sql += string.Format(" AND ped.TipoPedido={0}", (int)Pedido.TipoPedidoEnum.MaoDeObra);
            }

            sql = GetValoresCampo(sql, "IdProdPed");

            if (string.IsNullOrEmpty(sql))
            {
                return new ProdutoPedidoProducao[0];
            }

            // Recupera os IdProdPedProducao das etiquetas canceladas dos produtos da liberação
            sql = string.Format(@"SELECT ppp.IdProdPedProducao
                FROM produto_pedido_producao ppp
                    INNER JOIN produtos_pedido pp ON (ppp.IdProdPed=pp.IdProdPedEsp)
                    INNER JOIN pedido ped ON (pp.IdPedido=ped.IdPedido)
                WHERE pp.IdProdPed IN ({0})
                    AND ped.Situacao={1}", sql, (int)Pedido.SituacaoPedido.Cancelado);

            sql = GetValoresCampo(sql, "IdProdPedProducao");

            if (string.IsNullOrEmpty(sql))
            {
                return new ProdutoPedidoProducao[0];
            }

            itemEtiqueta = string.Format("CAST(SUBSTR({0}, 1, INSTR({0}, '/') - 1) AS SIGNED)",
                "SUBSTR(COALESCE(ppp.NumEtiquetaCanc, ppp.NumEtiqueta), INSTR(COALESCE(ppp.NumEtiquetaCanc, ppp.NumEtiqueta), '.') + 1)");

            // Busca somente as etiquetas canceladas que estão na liberação.
            sql = string.Format(@"SELECT IdProdPedProducao FROM produto_pedido_producao ppp
                WHERE ppp.IdProdPedProducao IN ({0}) AND
                    ((SELECT COUNT(*) FROM produtos_liberar_pedido WHERE IdLiberarPedido = {1} AND IdProdPedProducao = ppp.IdProdPedProducao) > 0 OR
                    (SELECT COALESCE(SUM(plp.QtdeCalc), 0) FROM produtos_liberar_pedido plp
                        INNER JOIN produtos_pedido pp ON (plp.IdProdPed = pp.IdProdPed)
                    WHERE plp.IdLiberarPedido <= {1} AND plp.IdProdPedProducao IS NULL AND pp.IdProdPedEsp=ppp.IdProdPed) >= {2})", sql, idLiberarPedido, itemEtiqueta);

            sql = GetValoresCampo(sql, "IdProdPedProducao");

            if (string.IsNullOrEmpty(sql))
            {
                return new ProdutoPedidoProducao[0];
            }

            return objPersistence.LoadData(SqlProdutosProducaoRelatorioLiberacao(sql)).ToList();
        }

        #endregion

        #region Pesquisa para acesso externo (E-Commerce)

        /// <summary>
        /// SQL da consulta que recupera os produtos de produção para a consulta de produção do E-Commerce.
        /// </summary>
        internal string SqlProdutosProducaoAcessoExterno(string codigoPedidoCliente, int idPedido, bool selecionar)
        {
            #region Declaração de variáveis

            var campos = string.Empty;
            var sql = string.Empty;
            var idCliente = 0;
            var filtrarDataFabrica = ProducaoConfig.BuscarDataFabricaConsultaProducao;

            if ((UserInfo.GetUserInfo?.IdCliente).GetValueOrDefault() == 0)
            {
                return string.Empty;
            }
            else
            {
                idCliente = (int)UserInfo.GetUserInfo.IdCliente;
            }

            #endregion

            #region Consulta

            campos = selecionar ? string.Format(@"ppp.IdProdPedProducao, ppp.IdProdPed, ppp.Situacao, ppp.PlanoCorte, ppp.NumEtiqueta, ppp.NumEtiquetaCanc, ppp.DataPerda, ppp.Obs, ppp.IdSetor,
                    ppp.TipoPerda, ppp.IdSubtipoPerda, ppp.PecaReposta, ppp.TipoPerdaRepos, ppp.IdSubtipoPerdaRepos, apl.CodInterno AS CodAplicacao, prc.CodInterno AS CodProcesso, p.CodInterno,
                    CONCAT(p.Descricao, IF(pp.Redondo AND {0}, ' REDONDO', ''))) AS DescrProduto,
                    IF(ped.TipoPedido={1}, a.Altura, IF(pp.AlturaReal > 0, pp.AlturaReal, pp.Altura)) AS Altura,
                    IF(ped.TipoPedido={1}, a.Largura, IF(pp.Redondo, 0, IF(pp.LarguraReal > 0, pp.LarguraReal, pp.Largura))) AS Largura,
                    CONCAT(CAST(ped.IdPedido AS CHAR), IF(ped.IdPedidoAnterior IS NOT NULL, CONCAT(' (', CONCAT(CAST(ped.IdPedidoAnterior AS CHAR), 'R)')), ''),
                        IF(ppp.IdPedidoExpedicao IS NOT NULL, CONCAT(' (Exp. ', CAST(ppp.IdPedidoExpedicao AS CHAR), ')'), '')) AS IdPedidoExibir,
                    IF(lp.Situacao={2}, lp.DataLiberacao, NULL) AS DataLiberacaoPedido,
                    ped.CodCliente, ped.TipoPedido={1} AS PedidoMaoObra, ped.TipoPedido={3} AS PedidoProducao, ped.Situacao={4} AS PedidoCancelado, ped.DataEntrega, ped.DataEntregaOriginal{5}",
                // Posição 0.
                (!BenefConfigDAO.Instance.CobrarRedondo()).ToString(),
                // Posição 1.
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                // Posição 2.
                (int)LiberarPedido.SituacaoLiberarPedido.Liberado,
                // Posição 3.
                (int)Pedido.TipoPedidoEnum.Producao,
                // Posição 4.
                (int)Pedido.SituacaoPedido.Cancelado,
                // Posição 5.
                filtrarDataFabrica ? ", ped_esp.DataFabrica AS DataEntregaFabrica" : string.Empty) :
                "COUNT(DISTINCT ppp.IdProdPedProducao)";

            sql = string.Format(@"
                SELECT {0}
                FROM produto_pedido_producao ppp
                    LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    LEFT JOIN produto p ON (pp.IdProd = p.IdProd)
                    LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN liberarpedido lp ON (ped.IdLiberarPedido = lp.IdLiberarPedido)
                    LEFT JOIN etiqueta_aplicacao apl ON (IF(ped.TipoPedido={1}, a.IdAplicacao, pp.IdAplicacao) = apl.IdAplicacao)
                    LEFT JOIN etiqueta_processo prc ON (IF(ped.TipoPedido={1}, a.IdProcesso, pp.IdProcesso) = prc.IdProcesso)
                WHERE ped.IdCli={2} AND ppp.Situacao IN ({3}, {4}) AND pp.IdProdPedParent IS NULL",
                // Posição 0.
                campos,
                // Posição 1.
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                // Posição 2.
                idCliente,
                // Posição 3.
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                // Posição 4.
                (int)ProdutoPedidoProducao.SituacaoEnum.Perda,
                // Posição 5.
                filtrarDataFabrica ? " LEFT JOIN pedido_espelho ped_esp ON (ped.IdPedido = ped_esp.IdPedido)" : string.Empty);

            #endregion

            #region Filtros

            if (idPedido > 0)
            {
                sql += string.Format(" AND (ped.IdPedido={0}", idPedido);

                // Na vidrália/colpany não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira
                // teria que mudar totalmente a forma de fazer o count
                if (ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido && PedidoDAO.Instance.IsPedidoReposto((uint)idPedido))
                {
                    sql += string.Format(" OR ped.IdPedidoAnterior={0}", idPedido);
                }

                if (PedidoDAO.Instance.IsPedidoExpedicaoBox((uint)idPedido))
                {
                    sql += string.Format(" OR ppp.IdPedidoExpedicao={0}", idPedido);
                }

                sql += ")";
            }

            if (!string.IsNullOrWhiteSpace(codigoPedidoCliente))
            {
                sql += " AND (ped.CodCliente LIKE ?codigoPedidoCliente OR pp.PedCli LIKE ?codigoPedidoCliente OR a.Ambiente LIKE ?codigoPedidoCliente)";
            }

            #endregion

            return sql;
        }

        /// <summary>
        /// Recupera os produtos de produção para a consulta de produção do E-Commerce.
        /// </summary>
        public IList<ProdutoPedidoProducao> PesquisarProdutosProducaoAcessoExterno(string codigoPedidoCliente, int idPedido, string sortExpression, int startRow, int pageSize)
        {
            GDAParameter[] parametros;
            var sql = string.Empty;
            var numeroRegistros = 0;
            var sort = string.IsNullOrWhiteSpace(sortExpression) ? "ppp.IdProdPedProducao DESC" : sortExpression;
            ProdutoPedidoProducao[] produtosPedidoProducao;
                        
            sql = SqlProdutosProducaoAcessoExterno(codigoPedidoCliente, idPedido, true);
            parametros = ObterParametrosProdutosProducaoAcessoExterno(codigoPedidoCliente);
            sql = GetSqlWithLimit(sql, sort, 0, pageSize, "ppp", sql.Substring(sql.ToLower().IndexOf("where") + "where".Length), false, !string.IsNullOrEmpty(sortExpression) || idPedido > 0,
                out numeroRegistros, parametros);
            produtosPedidoProducao = objPersistence.LoadData(sql, parametros).ToArray();

            SetInfoPaging(sort, 0, pageSize);
            GetSetores(ref produtosPedidoProducao);
            GetNumChapaCorte(ref produtosPedidoProducao);

            return produtosPedidoProducao;
        }

        /// <summary>
        /// Recupera a quantidade de produtos de produção para a consulta de produção do E-Commerce.
        /// </summary>
        public int PesquisarProdutosProducaoAcessoExternoCount(string codigoPedidoCliente, int idPedido)
        {            
            return objPersistence.ExecuteSqlQueryCount(SqlProdutosProducaoAcessoExterno(codigoPedidoCliente, idPedido, false), ObterParametrosProdutosProducaoAcessoExterno(codigoPedidoCliente));
        }

        /// <summary>
        /// Recupera os parâmetros da consulta de produção do E-Commerce.
        /// </summary>
        internal GDAParameter[] ObterParametrosProdutosProducaoAcessoExterno(string codigoPedidoCliente)
        {
            var parametros = new List<GDAParameter>();

            if (!string.IsNullOrWhiteSpace(codigoPedidoCliente))
            {
                parametros.Add(new GDAParameter("?codigoPedidoCliente", string.Format("%{0}%", codigoPedidoCliente)));
            }

            return parametros.Count > 0 ? parametros.ToArray() : null;
        }

        #endregion

        #region Retorna a lista de etiquetas pelo número do pedido

        internal string SqlByPedido(uint idPedido, bool selecionar)
        {
            var sql = @"
            SELECT " + (selecionar ? "ppp.NumEtiqueta" : "COUNT(*)") + @"
                FROM produto_pedido_producao ppp
                    INNER JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
                    WHERE ppp.situacao in (" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + ", " + (int)ProdutoPedidoProducao.SituacaoEnum.Perda + @") 
                        AND pp.idPedido = " + idPedido;

            return sql;
        }

        public string[] GetEtiquetasByPedido(GDASession sessao, uint idPedido)
        {
            if (idPedido == 0)
                return null;

            return ExecuteMultipleScalar<string>(sessao, SqlByPedido(idPedido, true)).ToArray();
        }

        public int GetCountByPedido(GDASession sessao, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, SqlByPedido(idPedido, false));
        }

        /// <summary>
        /// Retorna a quantidade de vidros de estoque entregues no pedido de revenda passado
        /// </summary>
        public int ObtemQtdVidroEstoqueEntreguePorPedido(GDASession sessao, uint idPedido)
        {
            var retorno = objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From produto_pedido_producao Where idPedidoExpedicao=" + idPedido);

            return retorno + objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From produto_impressao Where idPedidoExpedicao=" + idPedido);
        }

        /// <summary>
        /// Retorna a quantidade de vidros de estoque entregues no pedido de revenda passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemQtdVidroEstoqueEntreguePorPedido(uint idPedido)
        {
            return ObtemQtdVidroEstoqueEntreguePorPedido(null, idPedido);
        }

        #endregion

        #region Retorna a lista de etiquetas pelo plano de corte

        public string[] GetEtiquetasByPlanoCorte(string planoCorte)
        {
            return GetEtiquetasByPlanoCorte(null, planoCorte);
        }

        public string[] GetEtiquetasByPlanoCorte(GDASession sessao, string planoCorte)
        {
            if (String.IsNullOrEmpty(planoCorte))
                return new string[0];

            string sql = @"
                select pi.numEtiqueta
                from produto_impressao pi
                    inner join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                where ie.situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa + @" 
                    and !coalesce(pi.cancelado, false) 
                    and pi.planoCorte=?pc";

            return ExecuteMultipleScalar<string>(sessao, sql, new GDAParameter("?pc", planoCorte)).ToArray();
        }

        #endregion

        #region Consulta de produção por m²

        private string SqlAgrupada(uint idPedido, uint idLoja, uint idFunc, uint idRota, string codPedCli, uint idCliente, string nomeCliente,
            string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, int situacao, uint idSetor, int tipo,
            bool setoresPosteriores, uint idSubgrupo, string idsSubgrupo, bool agruparDia, bool agruparProdPed, string tipoCliente, bool selecionar)
        {
            string campoQtde = "if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @"
                and ap.idAmbientePedido is not null, (Coalesce(ap.qtde,1)*pp.qtde), pp.Qtde)";

            string campoCalcTotM2 = @"round(
                /*Caso o pedido seja mão de obra o m2 da peça deve ser considerado*/
                if(ped.tipoPedido=3, (
                (((50 - If(Mod(ap.altura, 50) > 0, Mod(ap.altura, 50), 50)) +
                ap.altura) * ((50 - If(Mod(ap.largura, 50) > 0, Mod(ap.largura, 50), 50)) + ap.largura)) / 1000000)             
                * ap.qtde, " + (PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? "ppo.totM2Calc" : "ppo.totM") + ")/(ppo.qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", ap.qtde, 1))*producao.qtde, 4)";

            string campoTotal = @"cast((pp.Total + coalesce(pp.valorBenef, 0))/
                 " + campoQtde + "*producao.Qtde as decimal(12,2))";

            string camposGerais = @"
                Select null as idProdPedProducao, idprodped, null as idSetor, null as idFuncPerda, null as idPedidoExpedicao, null as situacao, 
                    null as dataPerda, null as numEtiqueta, null as planoCorte, null as tipoPerda, null as obs, false as entrouEstoque, DescrProduto, 
                    codInterno, Sum(TotM2) as TotM2, cast(sum(Qtde) as decimal(12,2)) as Qtde, Cor, espessura, idGrupoProd, idSubgrupoProd, 
                    null as numEtiquetaCanc, null as idImpressao, descrSubgrupoProd, null as idFuncRepos, null as idSetorRepos, null as pecaReposta, 
                    null as tipoPerdaRepos, null as dataRepos, false as canceladoAdmin, CodProcesso, CodAplicacao, null as dadosReposicaoPeca, 
                    dataAgrupar, Sum(Total) as Total, idSubTipoPerda, idSubTipoPerdaRepos, '$$$' as Criterio, round(2.5 * Espessura * Sum(TotM2), 2) as Peso
                From (";

            string sql = camposGerais + @"
                SELECT prod.Descricao AS DescrProduto, prod.CodInterno, " + campoCalcTotM2 + @" AS TotM2, 
                    SUM(producao.Qtde) AS Qtde, prod.DescricaoCor AS Cor, prod.Espessura, prod.IdGrupoProd, prod.IdSubgrupoProd,
                    prod.DescricaoSubGrupo AS DescrSubgrupoProd, pp.IdProd, pp.IdProdPed, 
                    producao.DataAgrupar, " + campoTotal + @" AS Total, 
                    producao.IdSubTipoPerda, producao.IdSubTipoPerdaRepos, Coalesce(apl.CodInterno,apl2.CodInterno) as CodAplicacao, Coalesce(prc.CodInterno,prc2.CodInterno) as CodProcesso
                FROM produtos_pedido_espelho pp
                    INNER JOIN produtos_pedido ppo ON (pp.IdProdPed=ppo.IdProdPedEsp)
                    LEFT JOIN ambiente_pedido_espelho ap ON (pp.IdAmbientePedido=ap.IdAmbientePedido)
                    LEFT JOIN etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao)
                    LEFT JOIN etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                    LEFT JOIN etiqueta_aplicacao apl2 On (ap.idAplicacao=apl2.idAplicacao)
                    LEFT JOIN etiqueta_processo prc2 On (ap.idProcesso=prc2.idProcesso) 

                    LEFT JOIN ( 
		                SELECT p.IdProd,
                            p.CodInterno,
			                p.Descricao,
                            p.Espessura,
                            p.IdGrupoProd,
                            p.IdSubgrupoProd,
                            cv.Descricao AS DescricaoCor,
                            s.Descricao AS Descricaosubgrupo
                        FROM produto p
			                LEFT JOIN subgrupo_prod s ON (p.IdSubgrupoProd=s.IdSubgrupoProd)
			                LEFT JOIN cor_vidro cv ON (p.IdCorVidro=cv.IdCorVidro)
                    ) AS prod ON (pp.IdProd=prod.IdProd)

                    LEFT JOIN pedido ped ON (pp.IdPedido=ped.IdPedido) " +

                    (idCliente == 0 && String.IsNullOrEmpty(nomeCliente) && String.IsNullOrEmpty(tipoCliente) ? "" :
                    " INNER JOIN cliente cli ON (ped.IdCli=cli.Id_Cli) ") +

                    @" INNER JOIN (
                        SELECT ppp.IdProdPed, f.IdLoja, ppp.IdPedidoExpedicao, ppp.IdSubTipoPerda, ppp.IdSubTipoPerdaRepos, lp.IdFuncLeitura,
                            COUNT(*) AS Qtde, Date(lp.DataLeitura) AS DataAgrupar
                        FROM produto_pedido_producao ppp
                            INNER JOIN produtos_pedido_espelho ppe ON (ppp.IdProdPed=ppe.IdProdPed)
                            INNER JOIN (
		                        SELECT lp.IdProdPedProducao, lp.DataLeitura, lp.IdFuncLeitura
                                FROM leitura_producao lp
                                WHERE 1 ?whereLp
                            ) AS lp ON (ppp.IdProdPedProducao=lp.IdProdPedProducao)
                            INNER JOIN funcionario f ON (lp.IdFuncleitura=f.IdFunc)

                            " + (idPedido > 0 ? " INNER JOIN pedido ped ON (ppe.IdPedido=ped.IdPedido) " : "") + @"

                        WHERE 1 ?wherePpp

                        /* Este Group By deve ter 2 espaços entre as duas palavras, pois, ao utilizar o método LoadDataWithSortExpression
                           é verificado a primeira ocorrência de 'Group By' e é inserido o Where antes desta ocorrência */

                        group  by ppp.idProdPed
                    ) as producao on (pp.idProdPed=producao.idProdPed)
                Where ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;

            ProdutoPedidoProducao temp = new ProdutoPedidoProducao();
            string wherePpp = String.Empty, wherePpe = String.Empty, whereLp = String.Empty;
            string criterio = "";

            if (idPedido > 0)
            {
                string filtroPedido = " And (ped.idPedido=" + idPedido;

                // Na vidrália/colpany não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira
                // teria que mudar totalmente a forma de fazer o count
                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido &&
                    PedidoDAO.Instance.IsPedidoReposto(idPedido))
                    filtroPedido += " Or ped.IdPedidoAnterior=" + idPedido;

                var isPedExped = PedidoDAO.Instance.IsPedidoExpedicaoBox(idPedido);
                if (isPedExped)
                    filtroPedido += " Or ppp.idPedidoExpedicao=" + idPedido;

                filtroPedido += ")";

                if (!isPedExped)
                    sql += filtroPedido;

                wherePpp += filtroPedido;
                wherePpe += filtroPedido;

                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idLoja > 0)
            {
                sql += " And producao.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idFunc > 0)
            {
                whereLp += " And lp.idFuncLeitura=" + idFunc;
                criterio += "Funcionário Setor: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (idRota > 0)
            {
                sql += " And ped.idCli In (Select idCliente From rota_cliente Where idRota=" + idRota + ")";
                criterio += "Rota: " + RotaDAO.Instance.ObtemValorCampo<string>("codInterno", "idRota=" + idRota) + "    ";
            }

            if (!String.IsNullOrEmpty(codPedCli))
            {
                sql += " And ped.codCliente=?codCliente";
                criterio += "Cód. Pedido Cliente: " + codPedCli + "    ";
            }

            if (idCliente > 0)
            {
                sql += " And ped.idCli=" + idCliente;
                criterio += "Cliente: " + idCliente + " - " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And cli.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            else if (!string.IsNullOrEmpty(tipoCliente))
            {
                sql += " And cli.IdTipoCliente in (" + tipoCliente + ")";

                string[] ids = tipoCliente.Split(',');

                string[] descTipo = new string[ids.Length];

                for (int i = 0; i < ids.Length; i++)
                {
                    descTipo[i] = TipoClienteDAO.Instance.GetNome(ids[i].StrParaUint());
                }

                criterio += "Tipo de Cliente: " + string.Join(", ", descTipo) + "    ";
            }

            if (situacao > 0)
            {
                wherePpp += " And ppp.Situacao=" + situacao;
                temp.Situacao = situacao;
                criterio += "Situação: " + temp.DescrSituacao + "    ";
            }

            if (idSetor > 0)
            {
                if (!setoresPosteriores)
                    wherePpp += " And ppp.idSetor=" + idSetor;
                else
                    whereLp += " And lp.idSetor=" + idSetor;

                criterio += "Setor: " + Utils.ObtemSetor(idSetor).Descricao + (setoresPosteriores ? " (inclui produtos que já passaram por este setor)" : "") + "    ";
            }

            // 1-Peças Pendentes 2-Peças Prontas
            if (tipo == 1)
            {
                wherePpp += " And ppp.situacaoProducao=" + (int)SituacaoProdutoProducao.Pendente +
                    " And ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
                criterio += "Tipo: Peças pendentes    ";
            }
            else if (tipo == 2)
            {
                wherePpp += " And ppp.sitaucaoProducao=" + (int)SituacaoProdutoProducao.Pronto +
                    " And ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
                criterio += "Tipo: Peças prontas    ";
            }

            if (idPedido == 0 && !String.IsNullOrEmpty(dataIni) && idSetor > 0)
            {
                whereLp += " And lp.dataLeitura>=?dataIni";
                criterio += "Data " + Utils.ObtemSetor(idSetor).Descricao + " início: " + dataIni + "    ";
            }

            if (idPedido == 0 && !String.IsNullOrEmpty(dataFim) && idSetor > 0)
            {
                whereLp += " And lp.dataLeitura<=?dataFim";
                criterio += "Data " + Utils.ObtemSetor(idSetor).Descricao + " término: " + dataFim + "    ";
            }

            if (idPedido == 0 && !String.IsNullOrEmpty(dataIniEnt))
            {
                sql += " And ped.dataEntrega>=?dataIniEnt";
                criterio += "Data Entrega início: " + dataIniEnt + "    ";
            }

            if (idPedido == 0 && !String.IsNullOrEmpty(dataFimEnt))
            {
                sql += " And ped.dataEntrega<=?dataFimEnt";
                criterio += "Data Entrega término: " + dataFimEnt + "    ";
            }

            if (idSubgrupo > 0)
            {
                sql += " And prod.IdSubgrupoProd=" + idSubgrupo;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo) + "    ";
            }

            if (!string.IsNullOrEmpty(idsSubgrupo) && idsSubgrupo != "0")
            {
                string descricaoSubgrupos = "";
                sql += " And prod.IdSubgrupoProd In (" + idsSubgrupo + ")";

                foreach (var id in idsSubgrupo.Split(','))
                    descricaoSubgrupos += SubgrupoProdDAO.Instance.GetDescricao(id.StrParaInt()) + ", ";

                criterio += "Subgrupo(s): " + descricaoSubgrupos.TrimEnd(' ', ',') + "    ";
            }

            sql = sql.Replace("?wherePpp", wherePpp).Replace("?wherePpe", wherePpe).Replace("?whereLp", whereLp).Replace("$$$", criterio.Trim()) +
                " group by pp.idprodped) as tbl group by " + (!agruparProdPed ? "idProd" : "idProdPed");

            if (agruparDia && !agruparProdPed)
                sql += ", dataAgrupar";

            return selecionar ? sql : "select count(*) from (" + sql + ") as temp";
        }

        public IList<ProdutoPedidoProducao> GetForRptConsultaAgrupada(uint idPedido, uint idLoja, uint idFunc, uint idRota,
            uint idCliente, string nomeCliente, string dataIni, string dataFim, int situacao, uint idSetor, string idsSubgrupo, bool setoresPosteriores,
            bool agruparDia, bool agruparProdPed, string tipoCliente)
        {
            return objPersistence.LoadData(SqlAgrupada(idPedido, idLoja, idFunc, idRota, null, idCliente, nomeCliente,
                dataIni, dataFim, null, null, situacao, idSetor, 0, setoresPosteriores, 0, idsSubgrupo, agruparDia, agruparProdPed, tipoCliente, true),
                GetParam(null, null, null, dataIni, dataFim, null, null, null, null, nomeCliente, null, null, null, 0)).ToList();
        }

        public IList<ProdutoPedidoProducao> GetListConsultaAgrupada(uint idPedido, uint idLoja, uint idFunc, uint idRota,
            uint idCliente, string nomeCliente, string dataIni, string dataFim, int situacao, uint idSetor, string idsSubgrupo, bool setoresPosteriores,
            string tipoCliente, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlAgrupada(idPedido, idLoja, idFunc, idRota, null, idCliente, nomeCliente,
                dataIni, dataFim, null, null, situacao, idSetor, 0, setoresPosteriores, 0, idsSubgrupo, false, false, tipoCliente, true),
                sortExpression, startRow, pageSize, GetParam(null, null, null, dataIni, dataFim, null, null, null, null, nomeCliente, null, null, null, 0));
        }

        public int GetCountConsultaAgrupada(uint idPedido, uint idLoja, uint idFunc, uint idRota, uint idCliente,
            string nomeCliente, string dataIni, string dataFim, int situacao, uint idSetor, string idsSubgrupo, bool setoresPosteriores,
            string tipoCliente)
        {
            return GetCountWithInfoPaging(SqlAgrupada(idPedido, idLoja, idFunc, idRota, null, idCliente, nomeCliente,
                dataIni, dataFim, null, null, situacao, idSetor, 0, setoresPosteriores, 0, idsSubgrupo, false, false, tipoCliente, true),
                GetParam(null, null, null, dataIni, dataFim, null, null, null, null, nomeCliente, null, null, null, 0));
        }

        #endregion

        #region Valida a etiqueta

        /// <summary>
        /// Valida a etiqueta para produção.
        /// Se a etiqueta estiver inválida, lança uma exceção.
        /// </summary>
        public void ValidaEtiquetaProducao(GDASession session, ref string etiqueta)
        {
            try
            {
                if (etiqueta == null)
                    throw new NullReferenceException("Etiqueta não pode ser vazia.");

                etiqueta = etiqueta.ToUpper().Trim();
                if (etiqueta == string.Empty)
                    throw new NullReferenceException("Etiqueta não pode ser vazia.");

                // Não valida etiquetas de chapa, pedido, volume ou retalho.
                if (etiqueta[0] == 'C' || etiqueta[0] == 'P' || etiqueta[0] == 'R' || etiqueta[0] == 'M')
                {
                    if (etiqueta.Length == 1)
                        throw new Exception("Etiqueta inválida.");

                    return;
                }

                // A posição do item nunca deve ser 0, sempre deve ter um valor preenchido
                if (etiqueta.Contains("-0."))
                    throw new Exception("Etiqueta inválida, não possui posição do item.");

                int[] dadosEtiqueta = Array.ConvertAll(etiqueta.Split('-', '.', '/', '='), x => x.StrParaInt());
                if (dadosEtiqueta[2] > dadosEtiqueta[3])
                    throw new IndexOutOfRangeException("Etiqueta não pode ser do tipo '1-1.2/1'. Identificador da peça maior que a quantidade de produtos. Etiqueta: " + etiqueta);

                if (etiqueta.Split('-', '.', '/').Length > 4)
                    throw new Exception("Etiqueta inválida: " + etiqueta);

                /* Chamado 64283. */
                if (ExecuteScalar<bool>(session, string.Format("SELECT COUNT(*)>1 FROM produto_pedido_producao WHERE Situacao={0} AND NumEtiqueta=?numEtiqueta",
                    (int)ProdutoPedidoProducao.SituacaoEnum.Producao), new GDAParameter("?numEtiqueta", etiqueta)))
                    throw new Exception(string.Format("A etiqueta {0} está duplicada no sistema. Entre em contato com o suporte do software WebGlass.", etiqueta));
            }
            catch (Exception ex)
            {
                throw new Exception(MensagemAlerta.FormatErrorMsg("A etiqueta informada não é valida.", ex));
            }
        }

        #endregion

        #region Atualiza Situação

        /// <summary>
        /// Retira todos os produtos desta liberação de expedição.
        /// </summary>
        public void RetiraExpedicaoByPedido(GDASession sessao, uint idLiberacao)
        {
            string sql = @"
                Delete From leitura_producao  
                Where idSetor In (
                    select * from (
                        Select idSetor 
                        From setor 
                        Where tipo=" + (int)TipoSetor.Entregue + @"
                    ) as temp
                ) 
                And idProdPedProducao In (
                    select * from (
                        Select idProdPedProducao 
                        From produto_pedido_producao 
                        Where idProdPed In (
                            select * from (
                                Select idProdPed 
                                From produtos_pedido_espelho 
                                Where idPedido In (
                                    select * from (
                                        Select idPedido 
                                        From produtos_liberar_pedido 
                                        Where idLiberarPedido=" + idLiberacao + @"
                                    ) as temp
                                )
                            ) as temp
                        )
                    ) as temp
                )";

            // Exclui as leituras feitas na expedição
            objPersistence.ExecuteCommand(sessao, sql);

            sql = @"
                Update produto_pedido_producao ppp 
                Set ppp.idSetor=(
                    Select lprod.idSetor From leitura_producao lprod  
                        Inner Join setor s On (lprod.idSetor=s.idSetor)
                    Where idProdPedProducao=ppp.idProdPedProducao
                    Order By s.numSeq desc Limit 1
                )
                Where idProdPed In (
                    select * from (
                        Select idProdPed 
                        From produtos_pedido_espelho 
                        Where idPedido In (
                            select * from (
                                Select idPedido 
                                From produtos_liberar_pedido 
                                Where idLiberarPedido=" + idLiberacao + @"
                            ) as temp
                        )
                    ) as temp
                )";

            // Volta a peça para a posição anterior à excluída
            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Atualiza a situação da peça passada, de acordo com o tipo de funcionário passado
        /// </summary>
        public string AtualizaSituacaoComTransacao(uint idFunc, string codMateriaPrima, string codEtiqueta, uint idSetor, bool perda,
            bool retornarEstoque, uint? tipoPerda, uint? subtipoPerda, string obs, uint? idPedidoNovo, uint idRota,
            List<string> etiquetasMateriaPrima, uint? idCarregamento, bool temPlanoCorte, string etqCavalete, int idFornada)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = AtualizaSituacao(transaction, idFunc, codMateriaPrima, codEtiqueta, idSetor, perda, retornarEstoque, tipoPerda, subtipoPerda,
                        obs, idPedidoNovo, idRota, etiquetasMateriaPrima, idCarregamento, temPlanoCorte, etqCavalete, false, idFornada);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Atualiza a situação da peça passada, de acordo com o tipo de funcionário passado
        /// </summary>
        public string AtualizaSituacao(GDASession sessao, uint idFunc, string codMateriaPrima, string codEtiqueta, uint idSetor, bool perda,
            bool retornarEstoque, uint? tipoPerda, uint? subtipoPerda, string obs, uint? idPedidoNovo, uint idRota,
            List<string> etiquetasMateriaPrima, uint? idCarregamento, bool temPlanoCorte, string etqCavalete, bool cancTrocaDev, int idFornada)
        {
            uint idLeituraProd = 0, idProdPedProducao = 0;

            try
            {
                // Valida a etiqueta
                ValidaEtiquetaProducao(sessao, ref codEtiqueta);

                // Muda todos os caracteres para maiúsculo, em alguns leitores o N ou o C são lidos em letra minúscula
                codEtiqueta = codEtiqueta.ToUpper();
                codMateriaPrima = !string.IsNullOrEmpty(codMateriaPrima) ? codMateriaPrima.ToUpper() : string.Empty;

                //Verifica se a leitur esta vindo do carregamento
                var isCarregamento = idCarregamento.GetValueOrDefault(0) > 0;

                if (!string.IsNullOrEmpty(codMateriaPrima) && codMateriaPrima[0] != 'N' && codMateriaPrima[0] != 'R' &&
                    !LeituraProducaoDAO.Instance.PassouSetorLaminado(sessao, codMateriaPrima))
                    throw new Exception("Etiqueta de matéria prima inválida");

                // Bloqueia leitura de planos de corte com chapa genérica
                if (ProducaoConfig.BloquearLeituraPlanoCorteLoteGenerico && (codEtiqueta[0].ToString() == "M" || codEtiqueta[0].ToString() == "C") &&
                    codMateriaPrima == "N0-0.0/0")
                    throw new Exception("Não é permitido usar chapa genérica para leitura de planos de corte");

                // Bloqueia leitura de peças avulsas com chapa genérica
                if (ProducaoConfig.BloquearLeituraPecaLoteGenerico && codMateriaPrima == "N0-0.0/0")
                    throw new Exception("Não é permitido usar chapa genérica para leitura de peças");

                //Busca o setor que sera efetuada a leitura
                Setor setor = Utils.ObtemSetor(idSetor);

                if (setor == null)
                    throw new ArgumentNullException("Setor não encontrado.");

                //Chamado 22680 - Por algum motivo na Exp. Balcão o idFunc veio zerado
                if (idFunc == 0)
                    throw new ArgumentNullException("Nenhum funcionário foi informado.");

                //Estava ocorrendo um erro que estava pegando o id de outro funcionario no momento da leitura
                var idsFuncSetor = FuncionarioSetorDAO.Instance.GetSetores(sessao, idFunc);
                if (FuncionarioDAO.Instance.ObtemIdTipoFunc(sessao, idFunc) == (uint)Utils.TipoFuncionario.MarcadorProducao &&
                    idsFuncSetor.Where(f => f.IdSetor == idSetor).Count() <= 0)
                {
                    var nomeFunc = FuncionarioDAO.Instance.GetNome(sessao, idFunc);
                    throw new Exception("O funcionário " + nomeFunc + " não tem permissão para efetuar leitura no setor " + setor.Descricao);
                }

                //Verifica se a materia-prima informada ja teve outras leituras.
                if (!string.IsNullOrEmpty(codMateriaPrima) && setor.Corte && (codEtiqueta[0] == 'C' || !temPlanoCorte))
                {
                    if (ChapaCortePecaDAO.Instance.ChapaPossuiPlanoCorteVinculado(sessao, codMateriaPrima) &&
                        ProducaoConfig.TelaMarcacaoPeca.ImpedirLeituraChapaComPlanoCorteVinculado)
                        throw new Exception("Falha na leitura. A matéria-prima informada já possui um plano de corte vinculado.");

                    if (codEtiqueta[0] == 'C' && ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(sessao, codMateriaPrima) &&
                        ProducaoConfig.TelaMarcacaoPeca.ImpedirLeituraChapaComPlanoCorteVinculado)
                        throw new Exception("A matéria-prima informada já possui uma ou mais etiquetas vinculadas.");

                    if (ProducaoConfig.BloquearLeituraPecaNaChapaDiasDiferentes && codEtiqueta[0] != 'P' && codEtiqueta[0] != 'C')
                    {
                        var dias = ChapaCortePecaDAO.Instance.ObtemDiasLeitura(sessao, codMateriaPrima);

                        if (dias.Count() >= 1 && dias[0].Date != DateTime.Now.Date)
                            throw new Exception("A matéria-prima informada já possui uma ou mais etiquetas vinculadas.");
                    }
                }

                // Verifica se a etiqueta é uma etiqueta de pedido, desde que a empresa permita leitura de peças desta forma
                if (codEtiqueta[0] == 'P')
                {
                    if (ProducaoConfig.TelaMarcacaoPeca.ImpedirLeituraTodasPecasPedido)
                        throw new Exception("Não é permitido marcar leitura em todas as peças do pedido de uma só vez.");

                    if (setor.Tipo == TipoSetor.Entregue && ProducaoConfig.TelaMarcacaoPeca.ImpedirLeituraSetorEntregueTodasPecasPedido)
                        throw new Exception("Não é permitido marcar leitura, em setores do tipo Entregue, em todas as peças do pedido de uma só vez.");

                    uint idPedidoStr = codEtiqueta.Substring(1).StrParaUint();

                    if (idPedidoStr == 0)
                        throw new Exception("Etiqueta inválida.");

                    /* Chamado 44741. */
                    if (SetorDAO.Instance.IsLaminado(sessao, idSetor))
                        throw new Exception("Não é permitido marcar leitura em todas as peças do pedido de uma só vez, no setor configurado como Laminado.");

                    string separador = "<br />";
                    /* Chamado 44250. */
                    // string[] etiquetas = GetEtiquetasByPedido(sessao, idPedidoStr);
                    string[] etiquetas = GetEtiquetasByPedido(sessao, idPedidoStr);
                    string retornoPedido = "";

                    #region Salva na tabela de controle

                    var idLeitura = LeituraEtiquetaPedidoPlanoCorteDAO.Instance.Insert(sessao, new LeituraEtiquetaPedidoPlanoCorte()
                    {
                        NumEtiquetaLida = codEtiqueta
                    });

                    foreach (var e in etiquetas)
                    {
                        EtiquetaLidaPedidoPlanoCorteDAO.Instance.Insert(sessao, new EtiquetaLidaPedidoPlanoCorte()
                        {
                            IdLeituraEtiquetaPedPlanoCorte = idLeitura,
                            NumEtiquetaReal = e
                        });
                    }

                    #endregion

                    string msg = String.Empty;

                    foreach (string e in etiquetas)
                    {
                        try
                        {
                            // Deve chamar o método AtualizaSituacaoComTransacao para que caso um pedido seja lido pela metade em algum setor, seja possível
                            // ler o restante, pois com a transação no método acima deste não permite ler as peças restantes.
                            retornoPedido += separador + AtualizaSituacaoComTransacao(idFunc, codMateriaPrima, e, idSetor, perda, retornarEstoque, tipoPerda,
                                subtipoPerda, obs, idPedidoNovo, idRota, null, idCarregamento, false, etqCavalete, idFornada);
                        }
                        catch (Exception ex)
                        {
                            // Salva o erro mas continua lendo as outras peças, para que as que não tenham sido lidas sejam lidas 
                            // quando ler peças usando P + Número do pedido
                            msg += MensagemAlerta.FormatErrorMsg("Falha ao marcar peça.", ex);
                        }
                    }

                    if (!String.IsNullOrEmpty(msg))
                        throw new Exception(msg);
                    else if (retornoPedido == "")
                        throw new Exception("Esse pedido não possui peças para serem marcadas nesse setor.");

                    return retornoPedido.Substring(separador.Length);
                }

                if (codEtiqueta.Contains('='))
                {
                    string separador = "<br />";
                    var posicao = codEtiqueta.Split('-')[1].Split('/')[0].Split('.')[0].StrParaInt();

                    var intervaloEtiquetas = codEtiqueta.Split('/')[1].Split('=');
                    var inicioIntervalo = intervaloEtiquetas[0].StrParaInt();
                    var fimIntervalo = intervaloEtiquetas[1].StrParaInt();

                    var produtosImpressao = ProdutoImpressaoDAO.Instance.GetByIdPedido(sessao, posicao, codEtiqueta.Split('-')[0].StrParaInt()).OrderBy(f => f.ItemEtiqueta);
                    var etiquetas = new List<string>();

                    if (inicioIntervalo > fimIntervalo)
                        throw new Exception("item inicial maior que item final.");

                    if (fimIntervalo > produtosImpressao.Count())
                        throw new Exception("Número de itens deve maior que a quantidade existente na posição.");

                    foreach (var item in produtosImpressao)
                    {
                        if (item.ItemEtiqueta >= inicioIntervalo && item.ItemEtiqueta <= fimIntervalo)
                            etiquetas.Add(item.IdPedido.ToString() + "-" + posicao + "." + item.ItemEtiqueta + "/" + item.QtdeProd);
                    }

                    string retornoPedido = "";

                    #region Salva na tabela de controle


                    var idLeitura = LeituraEtiquetaPedidoPlanoCorteDAO.Instance.Insert(sessao, new LeituraEtiquetaPedidoPlanoCorte()
                    {
                        NumEtiquetaLida = codEtiqueta
                    });

                    #endregion

                    string msg = String.Empty;

                    foreach (var etiqueta in etiquetas)
                    {
                        EtiquetaLidaPedidoPlanoCorteDAO.Instance.Insert(sessao, new EtiquetaLidaPedidoPlanoCorte()
                        {
                            IdLeituraEtiquetaPedPlanoCorte = idLeitura,
                            NumEtiquetaReal = etiqueta
                        });
                    }

                    foreach (var etiqueta in etiquetas)
                    {
                        try
                        {
                            // Deve chamar o método AtualizaSituacaoComTransacao para que caso um pedido seja lido pela metade em algum setor, seja possível
                            // ler o restante, pois com a transação no método acima deste não permite ler as peças restantes.
                            retornoPedido += separador + AtualizaSituacaoComTransacao(idFunc, codMateriaPrima, etiqueta, idSetor, perda, retornarEstoque, tipoPerda,
                                subtipoPerda, obs, idPedidoNovo, idRota, null, idCarregamento, false, etqCavalete, idFornada);
                        }
                        catch (Exception ex)
                        {
                            msg = Glass.MensagemAlerta.FormatErrorMsg("Falha ao marcar peça.", ex);
                            break;
                        }
                    }

                    if (!String.IsNullOrEmpty(msg))
                        throw new Exception(msg);
                    else if (retornoPedido == "")
                        throw new Exception("Esse pedido não possui peças para serem marcadas nesse setor.");

                    return retornoPedido.Substring(separador.Length);
                }

                if (setor.Corte && codEtiqueta[0] == 'C')
                {
                    if (perda)
                        throw new Exception("Não é possível marcar perda em todas as peças desse plano de corte.");

                    if (codEtiqueta.Length == 1)
                        throw new Exception("Informe um plano de corte válido.");

                    string separador = "<br />";
                    string[] etiquetas = GetEtiquetasByPlanoCorte(sessao, codEtiqueta.Substring(1));
                    string retornoPlanoCorte = "";
                    string msg = String.Empty;

                    #region Salva na tabela de controle

                    var idLeitura = LeituraEtiquetaPedidoPlanoCorteDAO.Instance.Insert(sessao, new LeituraEtiquetaPedidoPlanoCorte()
                    {
                        NumEtiquetaLida = codEtiqueta
                    });

                    foreach (var e in etiquetas)
                    {
                        EtiquetaLidaPedidoPlanoCorteDAO.Instance.Insert(sessao, new EtiquetaLidaPedidoPlanoCorte()
                        {
                            IdLeituraEtiquetaPedPlanoCorte = idLeitura,
                            NumEtiquetaReal = e
                        });
                    }

                    #endregion

                    foreach (string e in etiquetas)
                    {
                        try
                        {
                            retornoPlanoCorte += separador + AtualizaSituacao(sessao, idFunc, codMateriaPrima, e, idSetor, perda, retornarEstoque, tipoPerda,
                                subtipoPerda, obs, idPedidoNovo, idRota, null, idCarregamento, true, etqCavalete, cancTrocaDev, idFornada);
                        }
                        catch (Exception ex)
                        {
                            if (ProducaoConfig.TelaMarcacaoPeca.CancelarLeiturasSeUmaFalhar)
                                throw ex;

                            if (ex.Message == "Etiqueta da matéria-prima não encontrada.")
                                throw new Exception("Etiqueta da matéria-prima não encontrada.");
                            else if (!msg.Contains(Glass.MensagemAlerta.FormatErrorMsg("", ex)))
                                msg += Glass.MensagemAlerta.FormatErrorMsg("", ex);
                        }
                    }

                    if (retornoPlanoCorte == "")
                        throw new Exception("Esse plano de corte não possui peças para serem marcadas nesse setor." + msg);

                    // Caso tenha ocorrido algum erro na leitura, salva na tabela de erros
                    if (!String.IsNullOrEmpty(msg))
                        ErroDAO.Instance.InserirFromException(String.Format("AtualizaSituacao - Chapa: {0} Plano: {1}", codMateriaPrima, codEtiqueta),
                            new Exception(msg));

                    /* Chamado 17508. */
                    retornoPlanoCorte = !string.IsNullOrEmpty(msg) ? msg + "-" + retornoPlanoCorte : retornoPlanoCorte;

                    return retornoPlanoCorte.Substring(separador.Length);
                }

                if (PecaEstaCancelada(sessao, codEtiqueta))
                    throw new Exception("Esta peça teve sua impressão cancelada pelo PCP.");

                //Verifica se a etiqueta esta com a produção parada
                var producaoParada = ProdutoPedidoProducaoDAO.Instance.VerificaPecaProducaoParada(sessao, codEtiqueta);
                if (!string.IsNullOrEmpty(producaoParada) && producaoParada.Split(';')[0] == "true")
                {
                    throw new Exception("Esta peça esta com a produção parada. \n\nMotivo: " + producaoParada.Split(';')[1]);
                }

                uint idPedido = codEtiqueta.Split('-')[0].StrParaUint();
                idProdPedProducao = ObtemIdProdPedProducao(sessao, codEtiqueta).GetValueOrDefault(0);

                // Verifica se a etiqueta está em produção ou existe
                if (idProdPedProducao == 0)
                    throw new Exception("Etiqueta não existe ou ainda não foi impressa no sistema.");

                if (idPedido > 0 && !IsPecaReposta(sessao, codEtiqueta, false) && !ProdutoImpressaoDAO.Instance.EstaImpressa(sessao, codEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                    throw new Exception("A peça ainda não foi impressa.");

                var idRetalhoProducao = UsoRetalhoProducaoDAO.Instance.ObtemIdRetalhoProducao(sessao, idProdPedProducao);
                var idLojaConsiderar = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0 ?
                    PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido) : FuncionarioDAO.Instance.ObtemIdLoja(sessao, idFunc);
                var idPedidoRevenda = PedidoDAO.Instance.ObterIdPedidoRevenda(sessao, (int)idPedido);

                // Faz validações caso seja o setor de corte, utilize controle de chapa e o código da chapa não seja N0-0.0/0
                // utilizado para permitir ler peças que não tenham chapa
                if (setor.Corte && !perda && PCPConfig.Etiqueta.UsarControleChapaCorte && codMateriaPrima != "N0-0.0/0")
                {
                    codMateriaPrima = codMateriaPrima != null ? codMateriaPrima.Trim() : string.Empty;

                    if (String.IsNullOrEmpty(codMateriaPrima))
                        throw new Exception("Informe a matéria-prima usada para o corte.");

                    var tipoEtiquetaMateriaPrima = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codMateriaPrima);

                    // Só verifica se a chapa foi perdida se a etiqueta de matéria-prima
                    // for uma etiquta de nota fiscal (chapa de vidro)
                    if (tipoEtiquetaMateriaPrima == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal &&
                        PerdaChapaVidroDAO.Instance.IsPerda(sessao, codMateriaPrima))
                        throw new Exception("A etiqueta da matéria-prima esta marcada como perdida.");

                    uint idProdPed = ObtemIdProdPed(sessao, idProdPedProducao);
                    uint idProd = 0;

                    if (PedidoDAO.Instance.IsMaoDeObra(sessao, idPedido))
                        idProd = AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProd", "idAmbientePedido=" + ProdutosPedidoEspelhoDAO.Instance.ObtemIdAmbientePedido(sessao, idProdPed));
                    else
                        idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProd", "idProdPed=" + idProdPed);

                    if (idRetalhoProducao > 0)
                    {
                        RetalhoProducao r = RetalhoProducaoDAO.Instance.Obter(sessao, idRetalhoProducao.Value);
                        if (r.NumeroEtiqueta.ToLower() != codMateriaPrima.ToLower())
                            throw new Exception("Deve ser usado como matéria-prima para esta peça o retalho '" + r.NumeroEtiqueta + "'.");
                    }

                    uint idProdImpressaoChapa = 0;
                    uint? idRetalhoProducaoProdImpressao = idRetalhoProducao;

                    idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, codMateriaPrima, tipoEtiquetaMateriaPrima);

                    if (idProdImpressaoChapa == 0)
                        throw new Exception("Etiqueta da matéria-prima não encontrada.");

                    //Se for etiqueta de laminado verifica se o mesmo esta pronto
                    if (tipoEtiquetaMateriaPrima == ProdutoImpressaoDAO.TipoEtiqueta.Pedido && !PecaEstaPronta(sessao, codMateriaPrima))
                        throw new Exception("A peça da matéria-prima não esta pronta.");

                    idRetalhoProducaoProdImpressao = idRetalhoProducao != null ? idRetalhoProducao :
                        ProdutoImpressaoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idRetalhoProducao", "idProdImpressao=" + idProdImpressaoChapa);

                    // Verifica se o tamanho da peça é menor que o tamanho do retalho
                    if (tipoEtiquetaMateriaPrima == ProdutoImpressaoDAO.TipoEtiqueta.Retalho)
                    {
                        var idRetalho = ProdutoImpressaoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idRetalhoProducao", "idProdImpressao=" + idProdImpressaoChapa);

                        if (idRetalho != null && UsoRetalhoProducaoDAO.Instance.ObtemIdRetalhoProducao(sessao, idProdPedProducao) != idRetalho)
                        {
                            var alturaPeca = ProdutosPedidoEspelhoDAO.Instance.ObtemAlturaProducao(sessao, idProdPed);
                            var larguraPeca = ProdutosPedidoEspelhoDAO.Instance.ObtemLarguraProducao(sessao, idProdPed);

                            var r = RetalhoProducaoDAO.Instance.Obter(sessao, idRetalho.Value);
                            if ((r.Altura < alturaPeca || r.Largura < larguraPeca) &&
                                (r.Altura < larguraPeca || r.Largura < alturaPeca))
                                throw new Exception("O tamanho da peça é maior que o tamanho do retalho.");

                            var m2Peca = Global.CalculosFluxo.ArredondaM2(sessao, larguraPeca, (int)alturaPeca, 1, (int)idProd, false, 0, false);
                            var m2DisponivelRetalho = (float)(r.TotM - r.TotMUsando);

                            if (m2DisponivelRetalho < m2Peca)
                                throw new Exception(string.Format("O M2 da peça é maior que o M2 disponível no retalho. M2 disponível: {0} | M2 peça: {1}",
                                    m2DisponivelRetalho, m2Peca));
                        }
                    }

                    // Verifica se o retalho selecionado atende à peça
                    //if (idRetalhoProducao == null && idRetalhoProducaoProdImpressao > 0)
                    //{
                    //    List<RetalhoProducao> retalhos = RetalhoProducaoDAO.Instance.ObterRetalhosProducao(sessao, idProdPed, false);
                    //    if (!retalhos.Exists(r => r.IdRetalhoProducao == idRetalhoProducaoProdImpressao.GetValueOrDefault()))
                    //        throw new Exception("O retalho selecionado como matéria-prima não pode ser usado pela peça indicada.");
                    //}

                    uint idProdChapa = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, idProdImpressaoChapa).GetValueOrDefault(),
                        idProdBaixa = idProdChapa;

                    if (idRetalhoProducaoProdImpressao > 0)
                    {
                        var situacao = RetalhoProducaoDAO.Instance.ObtemSituacao(sessao, idRetalhoProducaoProdImpressao.Value);

                        if (situacao == SituacaoRetalhoProducao.Cancelado)
                            throw new Exception("Esse retalho está cancelado.");

                        idProdBaixa = ProdutoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProdOrig", "idProd=" + idProdChapa);
                    }

                    uint? idProdBase = ProdutoDAO.Instance.ObtemValorCampo<uint?>(sessao, "IdProdBase", "IdProd=" + idProdBaixa);

                    //Para efetuar a leitura no corte deve haver um dos seguintes vinculos entre a etiqueta e a chapa
                    //Produto da etiqueta e produto da chapa iguais.
                    //Produto da etiqueta ter como matéria-prima a chapa.
                    //Produto da etiqueta ser igual ao produto base da chapa
                    //produto da etiqueta ter como matéria-prima o produto base da chapa
                    if (!(idProd == idProdBaixa || ProdutoBaixaEstoqueDAO.Instance.IsMateriaPrima(sessao, idProd, idProdBaixa) ||
                        idProd == idProdBase.GetValueOrDefault(0) ||
                        ProdutoBaixaEstoqueDAO.Instance.IsMateriaPrima(sessao, idProd, idProdBase.GetValueOrDefault(0))))
                    {
                        throw new Exception("O produto da peça (" + ProdutoDAO.Instance.ObtemDescricao(sessao, (int)idProd) +
                                     ") é diferente do produto da chapa (" + ProdutoDAO.Instance.ObtemDescricao(sessao, (int)idProdChapa) +
                                     "), ou não o contém como matéria-prima.");
                    }

                    // Verifica se tem estoque do produto que irá ser efetuada a baixa (Exceto se for retalho)
                    if ((!setor.Corte || codMateriaPrima[0] != 'R') && (idProdImpressaoChapa == 0 || (idProdImpressaoChapa > 0 && !ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(sessao, idProdImpressaoChapa))))
                    {
                        var idNf = ProdutoImpressaoDAO.Instance.ObtemIdNf(sessao, idProdImpressaoChapa);
                        var idLojaNotaFiscal = NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNf.GetValueOrDefault(0));
                        var idLojaFuncionario = FuncionarioDAO.Instance.ObtemIdLoja(sessao, idFunc);

                        //Verifica se a chapa teve entrada e recupera a loja que foi feito a movimentação pois a loja que deu entrada manual pode não ser é a mesma da nota fiscal
                        var idLojaMovEstoque = (uint?)objPersistence.ExecuteScalar(sessao, string.Format("SELECT idLoja FROM mov_estoque WHERE idNf={0} AND idProd={1} AND tipoMov={2}",
                            idNf.GetValueOrDefault(0), idProdBaixa, (int)MovEstoque.TipoMovEnum.Entrada));

                        var idLojaMovEstoqueChapa = idLojaMovEstoque ?? (idLojaNotaFiscal == 0 ? idLojaFuncionario : idLojaNotaFiscal);

                        /* Chamado 63113.
                         * Busca o pedido de revenda associado ao pedido de produção, para que a reserva do pedido seja considerada no momento de verificar se o produto possui estoque ou não. */

                        var qtdEstoque = ProdutoLojaDAO.Instance.GetEstoque(sessao, idLojaMovEstoqueChapa, idProdBaixa, (uint?)idPedidoRevenda, false, false, false);
                        var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)idProdBaixa);
                        var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)idProdBaixa);
                        var bloqueiaEstoque = GrupoProdDAO.Instance.BloquearEstoque(sessao, idGrupoProd, idSubgrupoProd);

                        if (qtdEstoque <= 0 && bloqueiaEstoque)
                            throw new Exception(string.Format("Não há estoque da matéria-prima ({0}) da peça ({1}).", ProdutoDAO.Instance.ObtemDescricao(sessao, (int)idProdBaixa),
                                ProdutoDAO.Instance.ObtemDescricao(sessao, (int)idProd)));
                    }
                }

                if ((setor.Tipo == TipoSetor.Entregue || setor.Tipo == TipoSetor.ExpCarregamento) &&
                    PedidoDAO.Instance.IsProducao(sessao, idPedido) && !ObtemValorCampo<bool>(sessao, "entrouEstoque", "idProdPedProducao=" + idProdPedProducao))
                    throw new Exception("Não é possível marcar essa peça como entregue porque ela não deu entrada no estoque.");

                //Se usar o controle de carregamento e o setor for expedição so pode dar saida
                //se o pedido for de entrega de balcão.
                if (setor.Tipo == TipoSetor.Entregue && OrdemCargaConfig.UsarControleOrdemCarga &&
                    PedidoDAO.Instance.ObtemTipoEntrega(sessao, idPedidoNovo ?? idPedido) != (int)Pedido.TipoEntregaPedido.Balcao &&
                    !PedidoDAO.Instance.IsMaoDeObra(sessao, idPedidoNovo ?? idPedido))
                {
                    throw new Exception("O pedido desta peça não é do tipo entrega Balcão.");
                }

                /* Chamado 49082. */
                if (setor.Tipo == TipoSetor.Entregue)
                {
                    var pecaComposicao = VerificarProdutoPedidoProducaoPossuiPai(sessao, (int)idProdPedProducao);

                    if (pecaComposicao)
                        throw new Exception("Não é possível ler peças de composição no setor Entregue.");
                }

                // Verifica se já foi marcada perda para esta peça
                if (objPersistence.ExecuteSqlQueryCount(sessao, @"Select Count(*) From produto_pedido_producao Where idProdPedProducao=" + idProdPedProducao +
                    " And situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Perda) > 0)
                    throw new Exception("Esta peça já foi marcada como perda.");

                if (perda && tipoPerda == null)
                    throw new Exception("Defina o tipo da perda para continuar.");

                if (perda && Glass.Configuracoes.ProducaoConfig.ObrigarMotivoPerda && String.IsNullOrEmpty(obs))
                    throw new Exception("Defina o motivo da perda para continuar.");

                var releitura = false;

                if (PCPConfig.PermirtirLerEtqBoxDuasVezesSeTransferencia && (setor.Tipo == TipoSetor.Entregue || setor.Tipo == TipoSetor.ExpCarregamento))
                    releitura = FoiLidoOCTransferencia(sessao, idProdPedProducao);

                // Verifica se esta peça já está na situação a ser alterada
                if (!perda && !releitura && LeituraProducaoDAO.Instance.VerificaLeituraPeca(sessao, idProdPedProducao, idSetor))
                    throw new Exception("Esta peça já entrou neste setor.");

                /* Chamado 23146. */
                var setoresObrigatorios = ((List<Setor>)SetorDAO.Instance.ObtemSetoresObrigatorios(sessao, idProdPedProducao));

                // Verifica se este setor impede que sejam colocadas peças no mesmo caso a peça não tenha passado em algum setor 
                // relacionado ao beneficiamento que a peça possua
                if (!isCarregamento && !perda && (setor.ImpedirAvanco || PCPConfig.ObrigarLeituraSetorImpedirAvanco) &&
                    SetorDAO.Instance.ObtemSetoresObrigatoriosNaoLidos(sessao, idProdPedProducao, idSetor).Count > 0 &&
                    (setoresObrigatorios == null || setoresObrigatorios.Count == 0))
                    throw new Exception("Esta peça não pode entrar neste setor, verifique os setores obrigatórios nos quais a mesma não passou.");

                // Verifica se há setores de roteiro de produção que não foram marcados anteriormente,
                // caso o setor impeça o avanço ou a configuração que garante a sequência do roteiro esteja marcada
                if (!isCarregamento)
                {
                    var setoresRestantes = SetorDAO.Instance.ObtemSetoresRestantes(sessao, idProdPedProducao, idSetor);
                    if (!perda && (setor.ImpedirAvanco || PCPConfig.UtilizarSequenciaRoteiroProducao) &&
                        setoresRestantes != null && setoresRestantes.Count > 0)
                        throw new Exception("Esta peça não pode entrar neste setor, verifique os setores do roteiro nos quais a mesma não passou.");
                }

                // Não permite que a peça seja lida em um setor fora do roteiro
                if (!perda && setor.SetorPertenceARoteiro && !setor.PermitirLeituraForaRoteiro)
                {
                    Setor setorObrig = setoresObrigatorios.Find(s => s.IdSetor == idSetor);

                    if (setorObrig == null)
                        throw new Exception("Esta peça não pode entrar neste setor, verifique os setores do roteiro que a mesma deve passar.");
                }

                // Verifica se este setor obriga a informar rota e se a rota foi informada
                if (!perda && setor != null && setor.InformarRota)
                {
                    if (idRota == 0)
                        throw new Exception("Informe a rota antes de marcar esta peça neste setor.");

                    if (!RotaDAO.Instance.PedidoPertenceARota(sessao, idRota, idPedido))
                        throw new Exception("Esta peça não pertence à rota informada.");
                }

                //Verifica se este setor obriga a informar cavalete e se o mesmo foi informado
                if (!perda && setor != null && setor.InformarCavalete)
                {
                    if (string.IsNullOrWhiteSpace(etqCavalete))
                        throw new Exception("A etiqueta do cavalete não foi informada.");

                    if (!CavaleteDAO.Instance.CavaleteExiste(sessao, etqCavalete))
                        throw new Exception("A etiqueta do cavalete informada é invalida.");
                }

                //Verifica se o setor gerencia as fornadas
                if (Configuracoes.PCPConfig.GerenciamentoFornada && setor.Forno && setor.GerenciarFornada)
                {
                    if (idFornada == 0)
                        throw new Exception("Informe o número da fornada.");

                    var m2Lido = FornadaDAO.Instance.ObterM2Lido(sessao, idFornada);
                    var capacidadeForno = ((decimal)setor.Altura * (decimal)setor.Largura) / 1000000m;

                    uint idProdPed = ObtemIdProdPed(sessao, idProdPedProducao);
                    uint idProd = 0;

                    if (PedidoDAO.Instance.IsMaoDeObra(sessao, idPedido))
                        idProd = AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProd", "idAmbientePedido=" + ProdutosPedidoEspelhoDAO.Instance.ObtemIdAmbientePedido(sessao, idProdPed));
                    else
                        idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProd", "idProdPed=" + idProdPed);

                    var alturaPeca = ProdutosPedidoEspelhoDAO.Instance.ObtemAlturaProducao(sessao, idProdPed);
                    var larguraPeca = ProdutosPedidoEspelhoDAO.Instance.ObtemLarguraProducao(sessao, idProdPed);
                    var m2Peca = Global.CalculosFluxo.ArredondaM2(sessao, larguraPeca, (int)alturaPeca, 1, (int)idProd, false, 0, false);

                    if ((m2Lido + (decimal)m2Peca) > capacidadeForno)
                        throw new Exception(string.Format("A peça ultrapassa a capacidade do forno.<br>  Capacidade: {0}m²<br> Lido: {1}m²<br>  Peça: {2}m²", Math.Round(capacidadeForno, 2), Math.Round(m2Lido, 2), Math.Round(m2Peca, 2)));
                }

                // Validação que permite apenas peças prontas no setor de expedição
                if (!perda && !isCarregamento && setor != null && setor.Tipo == TipoSetor.Entregue && !releitura &&
                    Glass.Configuracoes.PCPConfig.BloquearExpedicaoApenasPecasProntas)
                {
                    var situacao = ObtemSituacaoProducao(sessao, idProdPedProducao);
                    if (situacao != Glass.Data.Model.SituacaoProdutoProducao.Pronto)
                        throw new Exception("Apenas peças prontas podem ser expedidas.");
                }

                // Se a empresa obriga o financeiro a liberar pedido para entrega, verifica se o mesmo está liberado,
                // caso o setor lido seja entrega
                if (FinanceiroConfig.UsarControleLiberarFinanc && (setor.Tipo == TipoSetor.Entregue || setor.Tipo == TipoSetor.ExpCarregamento)
                    && !PedidoDAO.Instance.IsLiberadoEntregaFinanc(sessao, idPedido))
                    throw new Exception("O financeiro não liberou este pedido para entrega.");

                // Busca o produto ao qual se refere a etiqueta 
                // (Deve buscar somente visiveis por causa de um erro ao buscar etiqueta de box na expedição)
                ProdutosPedidoEspelho prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null, ObtemIdProdPed(sessao, idProdPedProducao), true);

                // Variável que contém o id do produto que será expedido no pedido novo
                uint? idProdutoNovo = null;

                if (!perda && (setor.Tipo == TipoSetor.Entregue || setor.Tipo == TipoSetor.ExpCarregamento) &&
                    PedidoDAO.Instance.IsProducao(sessao, idPedido))
                {
                    if (idPedidoNovo.GetValueOrDefault(0) == 0)
                        throw new Exception("Indique o pedido de venda/revenda que contém esse produto.");
                    else if (!PedidoDAO.Instance.IsVenda(sessao, idPedidoNovo.Value))
                        throw new Exception("Apenas pedidos de venda/revenda podem ser utilizados como pedido novo.");

                    //Chamado 55051.
                    if (idPedidoRevenda.GetValueOrDefault(0) > 0 && idPedidoRevenda.Value != idPedidoNovo.Value)
                    {
                        throw new Exception(string.Format("A etiqueta {0} não pode ser expedida com pedido de revenda {1}, ela esta vinculada a outro pedido.", codEtiqueta, idPedidoNovo.Value));
                    }
                    //Verifica se o pedido de revenda popssui um pedido de producao corte e se o idpedido produção é igual do pedido que está expedindo
                    else if (PedidoDAO.Instance.GerarPedidoProducaoCorte(sessao, idPedidoNovo.Value) &&
                        !PedidoDAO.Instance.ObterIdsPedidoProducaoPeloIdPedidoRevenda(sessao, (int)idPedidoNovo.Value).Contains((int)idPedido))
                    {
                        throw new Exception(string.Format("A etiqueta {0} não pode ser expedida com pedido de revenda {1}, pois ela não esta vinculada ao pedido {2}.", codEtiqueta, idPedidoNovo.Value, idPedido));
                    }

                    bool encontrado = false;

                    // Carrega os produtos
                    var prodPed = ProdutosPedidoDAO.Instance.GetByPedido(sessao, idPedidoNovo.Value);

                    var idsSubGrupoChapaVidro = SubgrupoProdDAO.Instance.ObterSubgruposChapaVidro(sessao);

                    // Soma a quantidade total do idProd passado nos produtos do pedido de revenda
                    var qtdTotalProd = prodPed
                        .Where(f => f.IdProd == prodPedEsp.IdProd)
                        .Sum(f => f.Qtde);

                    // Procura o produto no pedido de revenda
                    var produtoNovo = prodPed.Where(f =>
                        f.IdProd == prodPedEsp.IdProd &&
                        f.Altura == prodPedEsp.Altura &&
                        f.Largura == prodPedEsp.Largura &&
                        /* Chamado 63214. */
                        (cancTrocaDev ? true : f.Qtde - f.QtdSaida > 0) &&
                        qtdTotalProd - GetQtdeLiberadaByPedProd(sessao, f.IdPedido, null, f.IdProd) > 0);

                    if (produtoNovo.Count() > 0)
                    {
                        encontrado = true;
                        idProdutoNovo = produtoNovo.FirstOrDefault().IdProdPed;
                    }

                    if (!encontrado)
                    {
                        var pedidoNovoGeraProducaoCorte = idPedidoNovo > 0 ? PedidoDAO.Instance.GerarPedidoProducaoCorte(sessao, idPedidoNovo.GetValueOrDefault()) : false;

                        /* Chamado 61302. */
                        // Verifica se o pedido de produção foi gerado através de um pedido de revenda e verifica se o pedido novo está associado ao pedido de produção da etiqueta que está sendo lida.
                        if (idPedidoNovo > 0 && PedidoDAO.Instance.IsProducao(sessao, idPedido) && ((idPedidoRevenda.GetValueOrDefault() == 0 && !pedidoNovoGeraProducaoCorte) || idPedidoRevenda == idPedidoNovo.Value))
                        {
                            foreach (var p in prodPed)
                            {
                                var idProdBase = ProdutoDAO.Instance.ObtemValorCampo<uint?>(sessao, "IdProdBase", "IdProd=" + p.IdProd);
                                var idProdBaixa = ProdutoDAO.Instance.ObtemValorCampo<uint?>(sessao, "IdProdOrig", "IdProd=" + p.IdProd);
                                var tipoSubgrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)p.IdProd);

                                if ((tipoSubgrupoProd != TipoSubgrupoProd.ChapasVidro || idPedidoNovo == idPedidoRevenda) && (idProdBase > 0 || idProdBaixa > 0) &&
                                    !(p.IdProd == idProdBaixa.GetValueOrDefault() || ProdutoBaixaEstoqueDAO.Instance.IsMateriaPrima(sessao, p.IdProd, idProdBaixa.GetValueOrDefault()) ||
                                    p.IdProd == idProdBase.GetValueOrDefault() || ProdutoBaixaEstoqueDAO.Instance.IsMateriaPrima(sessao, p.IdProd, idProdBase.GetValueOrDefault())))
                                {
                                    encontrado = true;
                                    idProdutoNovo = p.IdProdPed;
                                    break;
                                }
                            }
                        }

                        if (!encontrado)
                            throw new Exception(
                                string.Format("Produto não encontrado, já expedido no pedido de venda {0} ou não há peças disponíveis no pedido {0} com largura e altura {1}x{2}.", idPedidoNovo.Value, prodPedEsp.Largura, prodPedEsp.Altura));
                    }
                }
                else
                    idPedidoNovo = null;

                // Verifica se a peça já foi liberada, caso esteja tentando marcar como entregue
                if (!perda && PedidoConfig.LiberarPedido && (!PedidoDAO.Instance.IsProducao(sessao, idPedido) || idPedidoNovo != null || idProdutoNovo != null) &&
                    ((ProducaoConfig.ExpedirSomentePedidosLiberados && setor.Tipo == TipoSetor.Entregue) ||
                    (setor.Tipo == TipoSetor.ExpCarregamento && ProducaoConfig.ExpedirSomentePedidosLiberadosNoCarregamento)))
                {
                    uint idPedidoVerificar = idPedidoNovo != null ? idPedidoNovo.Value : idPedido;

                    Pedido.SituacaoPedido situacaoPedido = PedidoDAO.Instance.ObtemSituacao(sessao, idPedidoVerificar);

                    bool liberado = !Liberacao.DadosLiberacao.LiberarPedidoProdutos ? situacaoPedido == Pedido.SituacaoPedido.Confirmado :
                        situacaoPedido == Pedido.SituacaoPedido.Confirmado ||
                        (LiberarPedidoDAO.Instance.IsProdutoPedidoLiberado(sessao, idProdutoNovo, prodPedEsp.IdProdPed, idProdPedProducao));

                    if (liberado && idPedidoNovo > 0)
                    {
                        Pedido.SituacaoPedido situacao = PedidoDAO.Instance.ObtemSituacao(sessao, idPedidoNovo.Value);
                        if (situacao != Pedido.SituacaoPedido.Confirmado && situacao != Pedido.SituacaoPedido.LiberadoParcialmente)
                            liberado = false;
                    }

                    if (!liberado)
                        throw new Exception("Este " +
                            (!Liberacao.DadosLiberacao.LiberarPedidoProdutos ?
                                "pedido" : string.Format("produto ({0})", ObtemEtiqueta(sessao, idProdPedProducao))) +
                            " ainda não foi liberado.");
                }

                //Se o setor for de laminado e não for perda
                if (setor.Laminado && !perda)
                {
                    // Verifica se foi informado as matérias-primas
                    if (etiquetasMateriaPrima == null || etiquetasMateriaPrima.Count == 0)
                        throw new Exception("Nenhuma matéria-prima foi informada.");

                    var prodsMateriaPrima = new List<Produto>();

                    foreach (var emp in etiquetasMateriaPrima)
                    {
                        string etiqueta = emp;

                        //Verifica se a uma etiqueta valida.
                        ValidaEtiquetaProducao(sessao, ref etiqueta);

                        //Busca o tipo de etiqueta
                        var tipoEtiquetaChapa = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(etiqueta);

                        if (tipoEtiquetaChapa == ProdutoImpressaoDAO.TipoEtiqueta.Pedido)
                        {
                            var idProdPedProducaoParent = ObterIdProdPedProducaoParentByEtiqueta(sessao, etiqueta);
                            if (idProdPedProducaoParent.GetValueOrDefault(0) == 0)
                                throw new Exception("A etiqueta: " + etiqueta + "não esta vinculada a nenhum produto do subgrupo Vidro Duplo ou Laminado.");

                            var idProdPedMatPrima = ProdutoImpressaoDAO.Instance.ObtemIdProdPed(sessao, etiqueta);
                            var idProdPedProducaoMatPrima = ObtemIdProdPedProducao(sessao, etiqueta);
                            var produtoComp = ProdutoDAO.Instance.GetByIdProd(sessao, ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(sessao, idProdPedMatPrima));

                            /* Chamado 23316. */
                            if (produtoComp == null)
                                throw new Exception(string.Format("Etiqueta {0} é inválida ou está cancelada.", etiqueta));

                            //Verifica se a materia-prima esta pronta
                            if (Instance.ObtemSituacaoProducao(sessao, idProdPedProducaoMatPrima.GetValueOrDefault(0)) == SituacaoProdutoProducao.Pendente)
                                throw new Exception("A matéria-prima: " + etiqueta + " não pode ser usada, pois a mesma não esta pronta.");

                            //Verifica se a etiqueta já foi utilizada
                            if (ChapaCortePecaDAO.Instance.ValidarChapa(sessao, produtoComp) &&
                                ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(sessao, ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, etiqueta, tipoEtiquetaChapa)))
                                throw new Exception("A matéria-prima: " + etiqueta + " não pode ser usada, pois a mesma já foi utilizada.");

                            if (idProdPedProducaoParent.Value != idProdPedProducao)
                            {
                                var dicComp = EtiquetasVinculoProdComposicao(sessao, idProdPedProducao, etiquetasMateriaPrima);
                                var idRevinculuar = ObterIdProdPedProducaoReveinculoComposicao(sessao, idProdPedProducao, idProdPedMatPrima, dicComp.Where(f => f.Value).Select(f => f.Key).ToList());

                                if (idRevinculuar.GetValueOrDefault(0) == 0)
                                    throw new Exception(string.Format("Etiqueta {0} não esta vinculada a composição da etiqueta {1}.", etiqueta, codEtiqueta));

                                var posEtq = ObtemValorCampo<string>(sessao, "PosEtiquetaParent", "IdProdPedProducao = " + idProdPedProducaoMatPrima);
                                posEtq = codEtiqueta + posEtq.Substring(posEtq.IndexOf("- "));

                                objPersistence
                                    .ExecuteCommand(sessao, string.Format("UPDATE produto_pedido_producao SET IdProdPedProducaoParent = {0}, PosEtiquetaParent = '{1}' WHERE IdProdPedProducao = {2}",
                                    idProdPedProducao, posEtq, idProdPedProducaoMatPrima));

                                posEtq = ObtemValorCampo<string>(sessao, "PosEtiquetaParent", "IdProdPedProducao = " + idRevinculuar.Value);
                                posEtq = ObtemValorCampo<string>(sessao, "NumEtiqueta", "IdProdPedProducao = " + idProdPedProducaoParent.Value) + posEtq.Substring(posEtq.IndexOf("- "));

                                objPersistence
                                    .ExecuteCommand(sessao, string.Format("UPDATE produto_pedido_producao SET IdProdPedProducaoParent = {0}, PosEtiquetaParent = '{1}' WHERE IdProdPedProducao = {2}",
                                    idProdPedProducaoParent.Value, posEtq, idRevinculuar.Value));
                            }

                            prodsMateriaPrima.Add(produtoComp);
                        }
                        else
                        {
                            //Verifica se a etiqueta é de NF
                            if (tipoEtiquetaChapa != ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal)
                                throw new Exception("Apenas etiquetas de nota fiscal podem ser usadas como matéria-prima");

                            uint idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, etiqueta, tipoEtiquetaChapa);

                            var produto = ProdutoDAO.Instance.GetByIdProd(sessao, ProdutosNfDAO.Instance.GetIdProdByEtiquetaAtiva(sessao, etiqueta));

                            /* Chamado 23316. */
                            if (produto == null)
                                throw new Exception(string.Format("Etiqueta {0} é inválida ou está cancelada.", etiqueta));

                            //Verifica se a etiqueta já foi utilizada
                            if (ChapaCortePecaDAO.Instance.ValidarChapa(sessao, produto) && ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(sessao, idProdImpressaoChapa))
                                throw new Exception("A matéria-prima: " + etiqueta + " não pode ser usada, pois a mesma já foi utilizada.");

                            prodsMateriaPrima.Add(produto);
                        }
                    }

                    //Busca as matérias-primas da etiqueta informada.
                    var idProd = ProdutoDAO.Instance.ObtemIdProdByEtiqueta(sessao, codEtiqueta);

                    var subGrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idProd);
                    if (subGrupoProd == TipoSubgrupoProd.VidroDuplo || subGrupoProd == TipoSubgrupoProd.VidroLaminado)
                    {
                        var pecas = ProdutosPedidoEspelhoDAO.Instance.ObterFilhosComposicaoByEtiqueta(sessao, codEtiqueta, true);

                        foreach (var pr in pecas.GroupBy(f => f.IdProd).Select(f => new { DescrProduto = f.FirstOrDefault().DescrProduto, IdProd = f.Key, Qtde = f.Sum(x => x.Qtde) }))
                        {
                            if (prodsMateriaPrima.Where(p => p.IdProd == pr.IdProd).Count() != pr.Qtde)
                                throw new Exception("A matéria-prima: " + pr.DescrProduto + " não foi informada ou com quantidade insuficiente.");
                        }
                    }
                    else
                    {
                        var prodBaixaEstoque = ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, idProd, ProdutoBaixaEstoqueDAO.TipoBuscaProduto.ApenasProducao);

                        //Verifica se as matérias-primas foram informadas e com quantidade correta
                        foreach (var pbe in prodBaixaEstoque)
                        {
                            if (prodsMateriaPrima.Where(p => p.IdProd == pbe.IdProdBaixa).Count() != pbe.Qtde)
                            {
                                var descProd = ProdutoDAO.Instance.GetCodInterno(sessao, pbe.IdProdBaixa) + " - "
                                    + ProdutoDAO.Instance.GetDescrProduto(sessao, pbe.IdProdBaixa);

                                throw new Exception("A matéria-prima: " + descProd + " não foi informada ou com quantidade insuficiente.");
                            }
                        }
                    }

                    //Faz a ligação das matérias-primas com a peça
                    ChapaCortePecaDAO.Instance.Inserir(sessao, codEtiqueta, etiquetasMateriaPrima);
                }

                DateTime dataLeitura = DateTime.Now;

                // Atualiza setor da peça
                if (!perda)
                {
                    var idCavalete = CavaleteDAO.Instance.ObterIdCavalete(sessao, etqCavalete);
                    LeituraProducao leituraProd = LeituraProducaoDAO.Instance.LeituraPeca(sessao, idProdPedProducao, idSetor, idFunc, dataLeitura, releitura, idCavalete);

                    objPersistence.ExecuteScalar(sessao, "Update produto_pedido_producao Set idSetor=" + idSetor +
                        (idPedidoNovo > 0 ? ", idPedidoExpedicao=" + idPedidoNovo.Value : "") +
                        (idCavalete.GetValueOrDefault(0) > 0 ? ", IdCavalete=" + idCavalete.Value : "") +
                        (idFornada > 0 ? ", IdFornada=" + idFornada : "") +
                        " Where idProdPedProducao=" + idProdPedProducao);

                    // Ocorria um erro ao marcar a leitura da peça sem que o setor fosse atualizado.
                    var setorAtual = ObtemValorCampo<uint>(sessao, "idSetor", "idProdPedProducao=" + idProdPedProducao);

                    if (setorAtual != idSetor)
                        throw new Exception(String.Format("A leitura não foi efetuada, setores divergentes. Setor Atual: {0}, Novo setor: {1}",
                             Utils.ObtemSetor(setorAtual).Descricao, Utils.ObtemSetor(idSetor).Descricao));

                    try
                    {
                        if (releitura)
                        {
                            var leituraExp = LeituraProducaoDAO.Instance.GetByProdPedProducao(sessao, idProdPedProducao)
                                .Where(f => f.IdSetor == SetorDAO.Instance.ObtemIdSetorEntrega(sessao) ||
                                    f.IdSetor == SetorDAO.Instance.ObtemIdSetorExpCarregamento(sessao)).FirstOrDefault();

                            if (leituraExp != null)
                                LeituraProducaoDAO.Instance.Delete(sessao, leituraExp);
                        }

                        idLeituraProd = LeituraProducaoDAO.Instance.Insert(sessao, leituraProd);

                        if (idLeituraProd <= 0)
                            throw new Exception("Falha ao inserir leitura.");
                    }
                    catch (Exception ex)
                    {
                        if (idLeituraProd <= 0)
                            idLeituraProd = LeituraProducaoDAO.Instance.ObtemIdLeituraProd(sessao, idProdPedProducao, setorAtual);

                        throw new Exception(String.Format("A leitura não foi efetuada. Erro: {0}",
                            ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")), ex);
                    }
                }
                else if (perda) // Atualiza peça, marcando-a como perda e retirando/recolocando no estoque
                {
                    var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, idPedido);

                    //verifica se a perda é da chapa
                    if (setor.Corte && !string.IsNullOrEmpty(codMateriaPrima))
                    {
                        PerdaChapaVidroDAO.Instance.MarcaPerdaChapaVidro(sessao, codMateriaPrima, tipoPerda.Value, subtipoPerda, obs);
                    }
                    else
                    {
                        string sp = subtipoPerda > 0 ? ", idSubtipoPerda=" + subtipoPerda : "";

                        string sql = "Update produto_pedido_producao Set Situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Perda +
                            ", DataPerda=?dataLeitura, obs=?obs, tipoPerda=" + tipoPerda.Value + sp + ", idFuncPerda=" + idFunc +
                            " Where idProdPedProducao=" + idProdPedProducao;

                        // Atualiza peça
                        objPersistence.ExecuteScalar(sessao, sql, new GDAParameter("?obs", obs), new GDAParameter("?dataLeitura", dataLeitura));

                        // Marca a situação produção do pedido como pendente
                        objPersistence.ExecuteCommand(sessao, "update pedido set situacaoProducao=" + (int)Pedido.SituacaoProducaoEnum.Pendente + " where idPedido=" + idPedido);

                        // Diminui a quantidade de ambientes
                        if (pedido.MaoDeObra)
                        {
                            uint idAmbientePedido = AmbientePedidoEspelhoDAO.Instance.GetIdAmbienteByEtiqueta(sessao, codEtiqueta);
                            AmbientePedidoEspelhoDAO.Instance.PerdaEtiquetaMaoObra(sessao, idAmbientePedido);

                            // Marca a peça como cancelada
                            objPersistence.ExecuteCommand(sessao, "update produto_pedido_producao set situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra +
                                " where idProdPedProducao=" + idProdPedProducao);

                            /* Chamado 36117.
                             * Marca o produto da impressão como cancelado. */
                            objPersistence.ExecuteCommand(sessao, "UPDATE produto_impressao SET Cancelado=1 WHERE NumEtiqueta=?numEtiqueta",
                                new GDAParameter("?numEtiqueta", codEtiqueta));

                            //Exclui a peça do carregamento caso exista
                            var idCar = ItemCarregamentoDAO.Instance.ObtemValorCampo<uint?>(sessao, "IdCarregamento", "idProdPedProducao= " + idProdPedProducao);
                            if (idCar.GetValueOrDefault(0) > 0)
                            {
                                objPersistence.ExecuteCommand(sessao, "DELETE FROM item_carregamento WHERE idProdPedProducao = " + idProdPedProducao);
                                CarregamentoDAO.Instance.AtualizaCarregamentoCarregado(sessao, idCar.Value, codEtiqueta);
                            }
                        }

                        // Se não for para retornar ao estoque, for pedido de produção e já tiver entrado em estoque,
                        // retira esta peça do estoque.
                        if (!retornarEstoque && pedido.Producao && EntrouEmEstoque(sessao, codEtiqueta))
                        {
                            // Busca o produto ao qual se refere a etiqueta
                            uint idProdPed = ObtemIdProdPed(sessao, idProdPedProducao);

                            ProdutosPedidoEspelho pp = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(sessao, idProdPed);

                            float qtdeOriginal = pp.Qtde;
                            float m2Calc;

                            try
                            {
                                pp.Qtde = 1;
                                m2Calc = Helper.Calculos.CalculoM2.Instance.Calcular(sessao, pedido, pp, true);
                            }
                            finally
                            {
                                pp.Qtde = qtdeOriginal;
                            }

                            bool m2 = new List<int> { (int)Glass.Data.Model.TipoCalculoGrupoProd.M2, (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto }.Contains(Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)pp.IdProd));

                            // Baixa o box
                            MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, pp.IdProd, idLojaConsiderar, idProdPedProducao, 1, 0, false, true, true);

                            // Credita a matéria-prima do box
                            MovEstoqueDAO.Instance.CreditaEstoqueProducao(sessao, pp.IdProd, idLojaConsiderar, idProdPedProducao, (decimal)(m2 ? m2Calc : 1), true, true);

                            // Marca que este produto entrou em estoque
                            objPersistence.ExecuteCommand(sessao, "Update produto_pedido_producao Set entrouEstoque=true Where idProdPedProducao=" + idProdPedProducao);
                        }
                    }

                    // Criado para que caso a empresa não gere o clone ao inserir o produto no pedido espelho
                    // o mesmo seja gerado ao marcar a peça como perdida, pois, ao fazer a areposição
                    // é necessário que exista o produto na tabela produtos_pedido para buscá-la.
                    if (!PCPConfig.CriarClone && ExecuteScalar<bool>(sessao, "Select Count(*)=0 From produtos_pedido Where idProdPedEsp=" + prodPedEsp.IdProdPed + " And invisivelPedido=1"))
                        ProdutosPedidoEspelhoDAO.Instance.CriarClone(sessao, pedido, prodPedEsp, false, true);
                }

                // Atualiza a situação de produção da peça.
                AtualizaSituacaoPecaNaProducao(sessao, idProdPedProducao, null, false);

                try
                {
                    // Atualiza a situação do pedido
                    if (perda || setor.Tipo != TipoSetor.Pendente)
                    {
                        var situacao =
                            perda ? (SituacaoProdutoProducao?)null :
                            setor.Tipo == TipoSetor.ExpCarregamento ? SituacaoProdutoProducao.Entregue :
                            PedidoDAO.Instance.IsProducao(sessao, idPedido) ? (SituacaoProdutoProducao?)null :
                            setor.Tipo == TipoSetor.Entregue ? SituacaoProdutoProducao.Entregue :
                            SituacaoProdutoProducao.Pronto;

                        PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, idPedido, situacao, dataLeitura);
                    }
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException("AlterarSituaçãoProduçãoPedido. Etiqueta:" + codEtiqueta + " IdSetor:" + idSetor, ex);

                    throw ex;
                }

                // Faz a ligação entre a peça e a chapa
                if (!perda && setor.Corte && PCPConfig.Etiqueta.UsarControleChapaCorte && codMateriaPrima != "N0-0.0/0")
                {
                    var tipoEtiquetaChapa = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codMateriaPrima);
                    var idProdImpressaoChapa = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, codMateriaPrima, tipoEtiquetaChapa);
                    var qtdeLeiturasChapaPedidoRevenda = ChapaCortePecaDAO.Instance.QtdeLeituraChapaPedidoRevenda(sessao, idProdImpressaoChapa, (uint)idPedidoRevenda.Value);

                    ChapaCortePecaDAO.Instance.Inserir(sessao, codMateriaPrima, codEtiqueta, temPlanoCorte, false);
                    ChapaTrocadaDevolvidaDAO.Instance.MarcarChapaComoUtilizada(sessao, codMateriaPrima);

                    /* Chamado 54054.
                     * Caso a chapa esteja sendo lida pela primeira vez, no pedido de revenda, baixa a quantidade de saída do produto e subtrai a quantidade em Reserva. */
                    if (idPedidoRevenda > 0 && qtdeLeiturasChapaPedidoRevenda == 0)
                    {
                        #region Ajusta estoque do pedido de revenda que gerou o pedido produção de corte

                        var idProdChapa = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, idProdImpressaoChapa);
                        var produtoPedidoRevenda = ProdutosPedidoDAO.Instance.GetByPedido(sessao, (uint)idPedidoRevenda).FirstOrDefault(f => f.IdProd == idProdChapa && f.Qtde > f.QtdSaida);
                        var idLojaPedidoRevenda = (int)PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedidoRevenda);
                        var idProdQtdeReserva = new Dictionary<int, float>();

                        if (idProdChapa.GetValueOrDefault() == 0)
                            throw new Exception(string.Format("Não foi possível recuperar o produto da matéria-prima. Verifique se a etiqueta {0} está impressa.", codMateriaPrima));

                        /* Chamado 65651. */
                        if (produtoPedidoRevenda == null || produtoPedidoRevenda.IdProdPed == 0)
                            throw new Exception(string.Format("Não foi possível recuperar o produto do pedido de revenda de número {2}. {0} {1}",
                                "Verifique se o pedido de revenda contém o produto da matéria-prima informada (código: {3}).",
                                idPedidoRevenda, ProdutoDAO.Instance.GetCodInterno(sessao, (int)idProdChapa.Value)));

                        // Atualiza o Qtd Saída dos produtos do pedido de revenda.
                        ProdutosPedidoDAO.Instance.MarcarSaida(sessao, produtoPedidoRevenda.IdProdPed, 1, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, ObtemEtiqueta(idProdPedProducao));

                        // Verifica o tipo de cálculo do produto.
                        var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedidoRevenda.IdProd);

                        // Verifica o tipo de cálculo do produto.
                        var m2Calc = Global.CalculosFluxo.ArredondaM2(sessao, produtoPedidoRevenda.Largura, (int)produtoPedidoRevenda.Altura, produtoPedidoRevenda.Qtde, 0, produtoPedidoRevenda.Redondo, 0, true);

                        // Ajusta o campo RESERVA do produto loja.
                        ProdutoLojaDAO.Instance.TirarReserva(sessao, idLojaPedidoRevenda, new Dictionary<int, float>() { { (int)produtoPedidoRevenda.IdProd, 1F } }, null, null, null, null,
                            (int)idPedidoRevenda, null, null, "ProdutoPedidoProducaoDAO - AtualizaSituacao");

                        #endregion
                    }
                }

                string retorno = "";

                if (idSetor > 0)
                {
                    if (!PedidoDAO.Instance.IsMaoDeObra(sessao, idPedido))
                    {
                        // Atualiza o estoque do produto da etiqueta
                        if (!perda)
                            AtualizaEstoqueEtiqueta(sessao, codEtiqueta, idSetor, idPedido, idPedidoNovo, idProdutoNovo, idCarregamento, cancTrocaDev);

                        // Verifica se há algum produto para ser baixado junto com esse
                        // O idProdutoNovo Não é mais o idProdPedEsp, agora ele é o idProdPed da peça do pedido de venda
                        //if (setor.Tipo == (int)Setor.TipoEnum.Entregue && idProdutoNovo != null)
                        //{
                        //    ProdutoPedidoProducao produto = ProdutoPedidoProducaoDAO.Instance.GetByProdPed(idProdutoNovo.Value);
                        //    if (produto != null)
                        //        retorno = AtualizaSituacao(produto.NumEtiqueta, idSetor, false, false, null, obs, null, idRota) + "<br />";
                        //}

                        retorno += prodPedEsp.DescrProduto + " " + prodPedEsp.Largura + "x" + prodPedEsp.Altura;
                    }
                    else
                    {
                        // Busca o ambiente ao qual se refere a etiqueta
                        AmbientePedidoEspelho ambPesEsp = AmbientePedidoEspelhoDAO.Instance.GetAmbienteByEtiqueta(sessao, codEtiqueta);
                        retorno = ambPesEsp.PecaVidro;
                    }
                }

                return retorno + " (" + codEtiqueta + ")";
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Esta peça já entrou neste setor.") && !ex.Message.Contains("Esta peça não pode entrar neste setor"))
                    ErroDAO.Instance.InserirFromException("AtualizaSituacao - Etiqueta: " + codEtiqueta + " Setor: " + idSetor +
                        " ppp: " + idProdPedProducao + " Leitura: " + idLeituraProd + " perda: " + perda +
                        (idPedidoNovo.GetValueOrDefault(0) > 0 ? " Pedido Exp: " + idPedidoNovo.Value : ""), ex);

                throw ex;
            }
        }

        public string MarcaExpedicaoChapaRetalhoRevendaComTransacao(string codEtiquetaChapa, uint idPedidoExpedicao, uint idSetor)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = MarcaExpedicaoChapaRetalhoRevenda(transaction, codEtiquetaChapa, idPedidoExpedicao, idSetor);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public string MarcaExpedicaoChapaRetalhoRevenda(GDASession sessao, string codEtiquetaChapa, uint idPedidoExpedicao, uint idSetor)
        {
            var tipoEtiqueta = ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(codEtiquetaChapa);

            var idRetalho = tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho ?
               codEtiquetaChapa.Substring(1, codEtiquetaChapa.IndexOf('-') - 1).StrParaUint() : 0;

            var prodImpressao = ProdutoImpressaoDAO.Instance.ObtemProdImpressaoParaExpedicao(sessao, codEtiquetaChapa);
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedidoExpedicao);

            //Verifica se o retalho informado pode ser expedido
            if (tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho &&
                RetalhoProducaoDAO.Instance.ObtemSituacao(sessao, idRetalho) != SituacaoRetalhoProducao.Disponivel)
                throw new Exception("O retalho informado não esta disponivel para uso.");

            //Verifica se o pedido de expedição foi informado
            if (idPedidoExpedicao == 0)
                throw new Exception("Indique o pedido de revenda que contém esse produto.");

            if (PedidoDAO.Instance.ObtemTipoEntrega(sessao, idPedidoExpedicao) != (int)Pedido.TipoEntregaPedido.Balcao)
                throw new Exception("O pedido informado não é do tipo entrega balcão.");

            // Muda todos os caracteres para maiúsculo, em alguns leitores o N ou o C são lidos em letra minúscula
            codEtiquetaChapa = codEtiquetaChapa.ToUpper();

            // Verifica se a etiqueta existe
            if (prodImpressao == null)
                throw new Exception("Etiqueta não existe ou ainda não foi impressa no sistema.");

            //Verifica se a etiqueta ja foi expedida
            if (ProdutoImpressaoDAO.Instance.EstaExpedida(sessao, prodImpressao.IdProdImpressao))
                throw new Exception("Esta etiqueta ja foi expedida no sistema.");

            if (ChapaCortePecaDAO.Instance.ChapaPossuiLeitura(sessao, prodImpressao.IdProdImpressao))
                throw new Exception("Esta etiqueta já foi utilizada no setor de corte.");

            // Se a empresa obriga o financeiro a liberar pedido para entrega, verifica se o mesmo está liberado,
            if (FinanceiroConfig.UsarControleLiberarFinanc && !PedidoDAO.Instance.IsLiberadoEntregaFinanc(sessao, idPedidoExpedicao))
                throw new Exception("O financeiro não liberou este pedido para entrega.");

            // Busca o produto ao qual se refere a etiqueta 
            //var prodNf = ProdutosNfDAO.Instance.GetProdNfByEtiqueta(codEtiquetaChapa);
            // var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(Glass.Conversoes.StrParaUint(codEtiquetaChapa.Split('-')[0]));

            // Variável que contém o id do produto que será expedido no pedido novo
            uint? idProdutoNovo = null;

            var encontrado = false;

            var prodPed = ProdutosPedidoDAO.Instance.GetByPedido(sessao, idPedidoExpedicao);
            prodPed = MetodosExtensao.ToArray(Glass.MetodosExtensao.Agrupar(prodPed, new[] { "IdProd" }, new[] { "Qtde" }));

            foreach (var p in prodPed)
            {
                if (p.IdProd == prodImpressao.IdProd && p.Altura == prodImpressao.Altura && p.Largura == prodImpressao.Largura)
                    if ((p.Qtde - GetQtdeLiberadaByPedProd(sessao, p.IdPedido, idProdutoNovo, p.IdProd)) >= 1)
                    {
                        encontrado = true;
                        idProdutoNovo = p.IdProdPed;
                        break;
                    }
            }

            if (!encontrado)
                throw new Exception("Produto não encontrado ou já expedido no pedido de revenda " + idPedidoExpedicao + ".");

            // Verifica se a peça já foi liberada, caso esteja tentando marcar como entregue
            if (PedidoConfig.LiberarPedido)
            {
                var situacaoPedido = PedidoDAO.Instance.ObtemSituacao(sessao, idPedidoExpedicao);

                var liberado = !Liberacao.DadosLiberacao.LiberarPedidoProdutos ? situacaoPedido == Pedido.SituacaoPedido.Confirmado :
                     situacaoPedido == Pedido.SituacaoPedido.Confirmado ||
                     (LiberarPedidoDAO.Instance.IsProdutoPedidoLiberado(sessao, idProdutoNovo, null, 0));

                if (liberado)
                {
                    var situacao = PedidoDAO.Instance.ObtemSituacao(sessao, idPedidoExpedicao);
                    if (situacao != Pedido.SituacaoPedido.Confirmado && situacao != Pedido.SituacaoPedido.LiberadoParcialmente)
                        liberado = false;
                }

                if (!liberado)
                    throw new Exception("Este " +
                        (!Liberacao.DadosLiberacao.LiberarPedidoProdutos ?
                            "pedido" : string.Format("produto (ID: {0})", idProdutoNovo)) +
                        " ainda não foi liberado.");
            }

            //Marca no produto_impressão o id do pedido de expedição
            ProdutoImpressaoDAO.Instance.AtualizaPedidoExpedicao(sessao, idPedidoExpedicao, prodImpressao.IdProdImpressao);

            //Faz o vinculo da chapa no corte para que a mesma não possa ser usada novamente
            ChapaCortePecaDAO.Instance.Inserir(sessao, codEtiquetaChapa, null, false, true);

            /* Chamado 52270. */
            // Marca quantos produtos do pedido saíram do estoque.
            var idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(sessao, PedidoDAO.Instance.ObtemIdLoja(sessao, idPedidoExpedicao), idPedidoExpedicao, null, null, false);
            ProdutosPedidoDAO.Instance.MarcarSaida(sessao, (uint)idProdutoNovo, 1, idSaidaEstoque, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

            //Baixa o estoque da peça
            MovEstoqueDAO.Instance.BaixaEstoquePedido(sessao, (uint)prodImpressao.IdProd, idLoja, idPedidoExpedicao, idProdutoNovo.Value, 1, 0, false, null);

            //Atualiza a situação do pedido
            PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, idPedidoExpedicao, null, DateTime.Now);

            //Marca o retalho como vendido
            if (tipoEtiqueta == ProdutoImpressaoDAO.TipoEtiqueta.Retalho)
                RetalhoProducaoDAO.Instance.AlteraSituacao(sessao, idRetalho, SituacaoRetalhoProducao.Vendido);

            // Executa o sql para retirar da liberação/reserva depois que marcar saída nos produtos, para que atualize corretamente a coluna
            // reserva/liberação
            if (!PedidoDAO.Instance.IsProducao(sessao, idPedidoExpedicao))
            {
                if (PedidoConfig.LiberarPedido)
                    ProdutoLojaDAO.Instance.TirarLiberacao(sessao, (int)PedidoDAO.Instance.ObtemIdLoja(sessao, idPedidoExpedicao),
                        new Dictionary<int, float> { { (int)prodImpressao.IdProd, 1 } }, null, null, null, null, (int)idPedidoExpedicao,
                        null, null, "ProdutoPedidoProducaoDAO - MarcaExpedicaoChapaRetalhoRevenda");
                else
                    ProdutoLojaDAO.Instance.TirarReserva(sessao, (int)PedidoDAO.Instance.ObtemIdLoja(sessao, idPedidoExpedicao),
                        new Dictionary<int, float> { { (int)prodImpressao.IdProd, 1 } }, null, null, null, null, (int)idPedidoExpedicao,
                        null, null, "ProdutoPedidoProducaoDAO - MarcaExpedicaoChapaRetalhoRevenda");
            }

            return prodImpressao.CodInternoProd + " - " + prodImpressao.DescrProduto + " " + prodImpressao.Largura + "x" + prodImpressao.Altura;
        }

        public void AtualizaEstoqueEtiqueta(GDASession sessao, string codEtiqueta, uint idSetor, uint idPedido, uint? idPedidoNovo, uint? idProdPedRevenda, uint? idCarregamento, bool cancTrocaDev)
        {
            // Valida a etiqueta
            ValidaEtiquetaProducao(sessao, ref codEtiqueta);

            var idProdPedProducao = ObtemIdProdPedProducao(sessao, codEtiqueta).GetValueOrDefault(0);
            var login = UserInfo.GetUserInfo;
            if (login == null)
                throw new Exception("Não foi possível recuperar os dados do usuário.");

            var idLojaConsiderar = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0 ?
                PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido) : login.IdLoja;
            var setor = Utils.ObtemSetor(idSetor);

            // Johan - Dia 30/01/13 - Atividade: 2126
            // Foi colocado o segundo parâmetro como true porque estava trazendo o produto errado
            // se houvesse etiquetas canceladas e o pedido fosse alterado, removendo algum produto.
            // Caso isso ocorresse, a posição das etiquetas impressas e canceladas era levada em
            // consideração ao buscar a posição do produto. Então o produto errado era retornado,
            // uma vez que a sua posição teria mudado em virtude do produto removido do pedido.
            var prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null, ObtemIdProdPed(idProdPedProducao), true);

            var m2Calc = Global.CalculosFluxo.ArredondaM2(sessao, prodPedEsp.Largura, (int)prodPedEsp.Altura, 1, 0, prodPedEsp.Redondo);

            var areaMinimaProd = ProdutoDAO.Instance.ObtemAreaMinima(sessao, (int)prodPedEsp.IdProd);

            var idCliente = PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido);

            var m2CalcAreaMinima = Glass.Global.CalculosFluxo.CalcM2Calculo(sessao, idCliente, (int)prodPedEsp.Altura, prodPedEsp.Largura,
                1, (int)prodPedEsp.IdProd, prodPedEsp.Redondo, prodPedEsp.Beneficiamentos.CountAreaMinimaSession(sessao), areaMinimaProd, false,
                prodPedEsp.Espessura, true);

            var m2 = new List<int> { (int)TipoCalculoGrupoProd.M2, (int)TipoCalculoGrupoProd.M2Direto }
                .Contains(GrupoProdDAO.Instance.TipoCalculo(sessao, (int)prodPedEsp.IdGrupoProd, (int)prodPedEsp.IdSubgrupoProd));

            // Se for pedido de produção e o setor estiver marcado para dar entrada de estoque
            if (SetorDAO.Instance.IsEntradaEstoque(sessao, idSetor) && PedidoDAO.Instance.IsProducao(sessao, idPedido) &&
                !Instance.EntrouEmEstoque(sessao, codEtiqueta))
            {
                MovEstoqueDAO.Instance.CreditaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaConsiderar, idProdPedProducao, 1, false, true);

                // Só baixa apenas se a peça possuir produto para baixa associado
                MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaConsiderar, idProdPedProducao,
                    (decimal)(m2Calc > 0 && !PecaPassouSetorLaminado(sessao, codEtiqueta) ? m2Calc : 1), (decimal)(m2 && !PecaPassouSetorLaminado(sessao, codEtiqueta) ? m2CalcAreaMinima : 0), true, false, true);

                // Marca que este produto entrou em estoque
                objPersistence.ExecuteCommand(sessao, "Update produto_pedido_producao Set entrouEstoque=true Where idProdPedProducao=" + idProdPedProducao);
            }

            // Marca saída do estoque em caso de expedição
            if (setor.Tipo == TipoSetor.Entregue || setor.Tipo == TipoSetor.ExpCarregamento)
            {
                if ((idPedidoNovo > 0 && Liberacao.Estoque.SaidaEstoqueBoxLiberar) || cancTrocaDev)
                    return;

                var prodPed = ProdutosPedidoDAO.Instance.GetByProdPedEsp(sessao, prodPedEsp.IdProdPed, false);

                if (prodPed != null)
                {
                    MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaConsiderar, idProdPedProducao,
                        (decimal)(m2 ? m2Calc : 1), (decimal)(m2 ? m2CalcAreaMinima : 0),
                        !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)prodPed.IdGrupoProd, (int)prodPed.IdSubgrupoProd), true, true);

                    var numEtiqueta = ObtemEtiqueta(sessao, idProdPedProducao);

                    // Marca saída desta peça no ProdutosPedido do pedido de PRODUÇÃO
                    ProdutosPedidoDAO.Instance.MarcarSaida(sessao, prodPed.IdProdPed, 1, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, numEtiqueta);

                    // Marca saída desta peça no ProdutosPedido do pedido de REVENDA desde que o pedido produção não seja para corte.
                    if (idProdPedRevenda > 0 && !PedidoDAO.Instance.IsPedidoProducaoCorte(sessao, idPedido))
                        ProdutosPedidoDAO.Instance.MarcarSaida(sessao, idProdPedRevenda.Value, 1, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, numEtiqueta);

                    if (idPedidoNovo != null)
                    {
                        // Se for tranferencia, credita o estoque da loja que esta recebendo a transferencia
                        var idPedTransferencia = idPedidoNovo.GetValueOrDefault(0) > 0 ? idPedidoNovo.Value : idPedido;
                        if (idCarregamento != null && (idCarregamento.GetValueOrDefault(0) > 0 && OrdemCargaDAO.Instance.TemTransferencia(sessao, idCarregamento.Value, idPedTransferencia)))
                        {
                            var idLojaTransferencia = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedTransferencia);

                            MovEstoqueDAO.Instance.CreditaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaTransferencia, idProdPedProducao,
                                (decimal)(m2 ? m2Calc : 1), !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)prodPed.IdGrupoProd, (int)prodPed.IdSubgrupoProd), true);
                        }
                    }
                }

                var idPedidoReservaLiberacao = idPedidoNovo.GetValueOrDefault() > 0 ? idPedidoNovo.Value : idPedido;

                // Executa o sql para retirar da liberação/reserva depois que marcar saída nos produtos, para que atualize corretamente a coluna
                // reserva/liberação
                if (!PedidoDAO.Instance.IsProducao(sessao, idPedidoReservaLiberacao))
                {
                    var idLojaReservaLiberacao = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedidoReservaLiberacao);
                    var idProdTirarReservaLiberacao = idProdPedRevenda > 0 ? ProdutosPedidoDAO.Instance.ObtemIdProd(sessao, idProdPedRevenda.Value) : prodPedEsp.IdProd;

                    if (PedidoConfig.LiberarPedido)
                        ProdutoLojaDAO.Instance.TirarLiberacao(sessao, (int)idLojaReservaLiberacao,
                            new Dictionary<int, float>() { { (int)idProdTirarReservaLiberacao, m2 ? m2Calc : 1 } }, null, null, null, null,
                            (int)idPedidoReservaLiberacao, null, null, "ProdutoPedidoProducaoDAO - AtualizaEstoqueEtiqueta");
                    else
                        ProdutoLojaDAO.Instance.TirarReserva(sessao, (int)idLojaReservaLiberacao,
                            new Dictionary<int, float>() { { (int)idProdTirarReservaLiberacao, m2 ? m2Calc : 1 } }, null, null, null, null,
                            (int)idPedidoReservaLiberacao, null, null, "ProdutoPedidoProducaoDAO - AtualizaEstoqueEtiqueta");
                }
            }
        }

        public bool ProdutoSaiuEstoque(uint idLiberarPedido, uint idPedido, uint idProdPedEsp)
        {
            return ProdutoSaiuEstoque(null, idLiberarPedido, idPedido, idProdPedEsp);
        }

        public bool ProdutoSaiuEstoque(GDASession sessao, uint idLiberarPedido, uint idPedido, uint idProdPedEsp)
        {
            SaidaEstoque se = null;

            if (PedidoConfig.LiberarPedido)
            {
                if (idLiberarPedido > 0)
                    se = SaidaEstoqueDAO.Instance.GetByLiberacao(sessao, idLiberarPedido);
            }
            else
                se = SaidaEstoqueDAO.Instance.GetByPedido(idPedido);

            ProdutoSaidaEstoque[] prodSe = se != null ? ProdutoSaidaEstoqueDAO.Instance.GetForRpt(sessao, se.IdSaidaEstoque) : null;

            ProdutoSaidaEstoque pse = prodSe == null || prodSe.Length == 0 ? null :
                Array.Find<ProdutoSaidaEstoque>(prodSe, prod => prod.IdProdPed == idProdPedEsp);

            return pse != null && pse.IdProdSaidaEstoque > 0;
        }

        #endregion

        #region Marca entrada de estoque

        /// <summary>
        /// Marca entrada de estoque
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public string MarcaEntradaEstoque(string codEtiqueta)
        {
            // Valida a etiqueta
            ValidaEtiquetaProducao(null, ref codEtiqueta);

            // Verifica se a etiqueta é uma etiqueta de pedido
            if (codEtiqueta[0] == 'P')
            {
                string separador = "<br />";
                string[] etiquetas = GetEtiquetasByPedido(null, codEtiqueta.Substring(1).StrParaUint());
                string retornoPedido = "";

                try
                {
                    foreach (string e in etiquetas)
                        retornoPedido += separador + MarcaEntradaEstoque(e);
                }
                catch { }

                if (retornoPedido == "")
                    throw new Exception("Esse pedido não possui peças para entrar em estoque.");

                return retornoPedido.Substring(separador.Length);
            }

            if (EntrouEmEstoque(null, codEtiqueta))
                throw new Exception("Esta peça já entrou em estoque.");

            uint idPedido = Glass.Conversoes.StrParaUint(codEtiqueta.Split('-')[0]);
            var pedido = PedidoEspelhoDAO.Instance.GetElement(null, idPedido);

            // Verifica se a etiqueta está em produção ou existe
            if (!PecaEstaEmProducao(codEtiqueta))
            {
                if (PecaEstaCancelada(codEtiqueta))
                    throw new Exception("Esta peça teve sua impressão cancelada pelo PCP.");
                else
                    throw new Exception("Etiqueta não existe ou ainda não foi impressa no sistema.");
            }

            // Busca o produto ao qual se refere a etiqueta
            var idProdPedProd = ObtemIdProdPedProducao(codEtiqueta);
            if (idProdPedProd != null)
            {
                uint idProdPedProducao = idProdPedProd.Value;
                uint idProdPed = ObtemIdProdPed(idProdPedProducao);

                LoginUsuario login = UserInfo.GetUserInfo;
                ProdutosPedidoEspelho pp = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(idProdPed);

                float m2Calc = CalculoM2.Instance.Calcular(null, pedido, pp, true);

                bool m2 = new[] { (int)TipoCalculoGrupoProd.M2, (int)TipoCalculoGrupoProd.M2Direto }
                    .Contains(GrupoProdDAO.Instance.TipoCalculo((int)pp.IdProd));
                
                float m2CalcAreaMinima = CalculoM2.Instance.CalcularM2Calculo(null, pedido, pp,
                    false, true, pp.Beneficiamentos.CountAreaMinima);

                MovEstoqueDAO.Instance.CreditaEstoqueProducao(pp.IdProd, login.IdLoja, idProdPedProducao, 1, false, true);

                MovEstoqueDAO.Instance.BaixaEstoqueProducao(pp.IdProd, login.IdLoja, idProdPedProducao,
                    (decimal)(m2 ? m2Calc : 1), (decimal)(m2 ? m2CalcAreaMinima : 0), true, true, true);

                // Marca que este produto entrou em estoque
                objPersistence.ExecuteCommand("Update produto_pedido_producao Set entrouEstoque=true Where idProdPedProducao=" + idProdPedProducao);

                string descrProduto = ProdutoDAO.Instance.ObtemDescricao((int)pp.IdProd);

                string retorno = descrProduto + " " + pp.Largura + "x" + pp.Altura;

                return retorno + " (" + codEtiqueta + ")";
            }

            throw new Exception("Esse pedido não possui peças para entrar em estoque.");
        }

        /// <summary>
        /// Verifica se a peça passada já entrou em estoque
        /// </summary>
        public bool EntrouEmEstoque(GDASession sessao, string codEtiqueta)
        {
            // Valida a etiqueta
            ValidaEtiquetaProducao(sessao, ref codEtiqueta);

            string sql = "Select Count(*) From produto_pedido_producao Where numEtiqueta=?numEtiqueta And entrouEstoque=true And situacao=" +
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?numEtiqueta", codEtiqueta)) > 0;
        }

        #endregion

        #region Marca Peça Reposta

        /// <summary>
        /// Marca Peça Reposta
        /// </summary>
        public string MarcarPecaReposta(string numChapa, string numEtiqueta, uint idSetorRepos, uint idFuncPerda,
            DateTime dataPerda, uint tipoPerdaRepos, uint? subtipoPerdaRepos, string obs, bool forcarPerda)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Verifica se a etiqueta é uma etiqueta de pedido
                    if (numEtiqueta[0] == 'P')
                        throw new Exception("Não é possível marcar reposição de peças por pedido.");

                    if (Glass.Configuracoes.ProducaoConfig.ObrigarMotivoPerda && String.IsNullOrEmpty(obs))
                        throw new Exception("Defina o motivo da perda para continuar.");

                    string retorno = "";
                    uint idPedido = Glass.Conversoes.StrParaUint(numEtiqueta.Split('-')[0]);
                    bool isMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(transaction, idPedido);

                    uint idProdPedProducao = ObtemIdProdPedProducao(transaction, numEtiqueta).GetValueOrDefault();
                    uint idProdPedProducaoParent = ObterIdProdPedProducaoParent(transaction, idProdPedProducao).GetValueOrDefault();

                    if (idProdPedProducao == 0)
                        throw new Exception(string.Format("Não foi possível recuperar o produto de produção da etiqueta {0}.", numEtiqueta));

                    if (ExecuteScalar<int>(transaction, $"SELECT count(*) FROM produto_pedido_producao WHERE IdProdPedProducaoParent={idProdPedProducao}") > 0)
                        throw new Exception($"Não é possível marcar reposição em produtos pais de composição. Cancele a impressão da etiqueta e faça a reposição das peças filhas.");

                    var numEtiquetaParent = ObtemValorCampo<string>(transaction, "NumEtiqueta", "idProdPedProducao=" + idProdPedProducaoParent);

                    if (idProdPedProducaoParent > 0 && !string.IsNullOrWhiteSpace(numEtiquetaParent) &&
                        ProdutoImpressaoDAO.Instance.EstaImpressa(transaction, numEtiquetaParent, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                        throw new Exception($"Não é possível marcar reposição em produtos de composição caso o produto pai esteja impresso. Cancele a impressão da etiqueta pai: {numEtiquetaParent}");

                    /* Chamado 51854. */
                    if (SetorDAO.Instance.ObterSituacao(transaction, (int)ObtemIdSetor(transaction, idProdPedProducao)) == Situacao.Inativo)
                        throw new Exception(string.Format("O último setor lido na etiqueta {0} está inativo, ative-o para marcá-la peça como reposta.", numEtiqueta));

                    //Verifica se a peça tem leitura no carregamento, se tiver deve estornar antes de marcar perda.
                    var itens = ItemCarregamentoDAO.Instance.GetByIdProdPedProducao(transaction, idProdPedProducao);
                    var carregamentos = "";
                    foreach (var item in itens)
                        if (item.Carregado)
                            carregamentos += item.IdCarregamento + ", ";

                    if (!string.IsNullOrEmpty(carregamentos))
                        throw new Exception("Não é possível marcar resposição, pois a peça tem leitura no carregamento " + carregamentos.Trim().Trim(',') +
                            ". Efetue o estorno antes.");

                    if (PedidoDAO.Instance.ObtemSituacao(transaction, idPedido) == Pedido.SituacaoPedido.Confirmado && PedidoDAO.Instance.GetTipoPedido(transaction, idPedido) == Pedido.TipoPedidoEnum.MaoDeObra)
                        throw new Exception("Não é possível marcar perda em peças de pedido mão de obra liberados.");

                    // Valida a etiqueta
                    ValidaEtiquetaProducao(transaction, ref numEtiqueta);

                    if (PecaEstaCancelada(transaction, numEtiqueta, true))
                        throw new Exception("Não é possível repor uma peça cancelada.");

                    // Se a perda for forçada ou se o pedido for mão-de-obra, marca a perda usando o método original
                    // (se o pedido for mão-de-obra atualiza a quantidade de ambientes)
                    if (forcarPerda || isMaoDeObra)
                    {
                        if (isMaoDeObra && GetCountByPedido(transaction, idPedido) == 1)
                            throw new Exception("Não é possível marcar reposição de mão de obra caso o pedido possua somente um produto de mão de obra. Cancele o pedido.");

                        retorno = AtualizaSituacao(transaction, idFuncPerda, numChapa, numEtiqueta, idSetorRepos, true, false, tipoPerdaRepos, subtipoPerdaRepos, obs, null, 0, null, null, false, null, false, 0);

                        if (LiberarPedidoDAO.Instance.IsPedidoLiberado(transaction, idPedido) && isMaoDeObra)
                            objPersistence.ExecuteCommand(transaction, "Update pedido Set situacao=" + (int)Pedido.SituacaoPedido.Confirmado + " Where idPedido=" + idPedido);

                        transaction.Commit();
                        transaction.Close();

                        return retorno;
                    }

                    if (!LeuProducao(transaction, numEtiqueta))
                        throw new Exception("Não é possível repor uma peça que ainda não foi impressa.");

                    if (obs != null && obs.Length > 250)
                        throw new Exception("O campo motivo da reposição não pode ter mais que 250 caracteres.");

                    // Busca o produto ao qual se refere a etiqueta
                    ProdutosPedidoEspelho prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(transaction, null, ObtemIdProdPed(transaction, idProdPedProducao), true);

                    // Monta a string que possibilita voltar a situação da peça
                    string dadosReposicao = ObtemValorCampo<uint?>(transaction, "idPedidoExpedicao", "idProdPedProducao=" + idProdPedProducao) + "~" +
                        ObtemValorCampo<uint>(transaction, "idSetor", "idProdPedProducao=" + idProdPedProducao) + "~" +
                        ObtemValorCampo<string>(transaction, "obs", "idProdPedProducao=" + idProdPedProducao) + "~" +
                        ObtemValorCampo<int>(transaction, "situacaoProducao", "idProdPedProducao=" + idProdPedProducao);

                    ProdutoImpressao dados = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(transaction, numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);
                    dadosReposicao += dados == null ? "~~~" :
                        String.Format("~{0}~{1}~{2}", dados.IdImpressao, dados.PlanoCorte, dados.PosicaoArqOtimiz);

                    var leituras = LeituraProducaoDAO.Instance.GetByProdPedProducao(transaction, idProdPedProducao);

                    foreach (LeituraProducao lp in leituras)
                        dadosReposicao += "~" + lp.IdFuncLeitura + "!" + lp.IdSetor + "!" + lp.DataLeitura;

                    // Salva os dados atuais da reposição (se houverem) na tabela dados_reposicao
                    DadosReposicaoDAO.Instance.Empilha(transaction, idProdPedProducao);

                    // Marca que este produto foi reposto
                    string sp = subtipoPerdaRepos > 0 ? ", idSubtipoPerdaRepos=" + subtipoPerdaRepos : "";
                    objPersistence.ExecuteCommand(transaction, "Update produto_pedido_producao Set idPedidoExpedicao=null, pecaReposta=true, tipoPerdaRepos=" +
                        tipoPerdaRepos + sp + ", obs=?obs, dataRepos=?dataPerda, situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao +
                        ", idSetor=1, situacaoProducao=" + (int)SituacaoProdutoProducao.Pendente + ", idSetorRepos=" + idSetorRepos +
                        ", idFuncRepos=" + idFuncPerda + ", dadosReposicaoPeca=?dadosReposicao Where idProdPedProducao=" + idProdPedProducao,
                        new GDAParameter("?obs", obs), new GDAParameter("?dadosReposicao", dadosReposicao), new GDAParameter("?dataPerda", dataPerda));

                    //Cancela a associação com o retalho caso ela exista.
                    UsoRetalhoProducaoDAO.Instance.CancelarAssociacao(transaction, idProdPedProducao);

                    // Exclui leituras feitas nesta peça
                    objPersistence.ExecuteCommand(transaction, "Delete From leitura_producao Where idProdPedProducao=" + idProdPedProducao);

                    #region Remove a associação da peça ou da chapa

                    var quantidadeLeiturasChapa = ChapaCortePecaDAO.Instance.QtdeLeiturasChapa(transaction, dados.IdProdImpressao);

                    // O ideal é agregar à esta condição o mesmo código do método RetiraPecaSituacao, para que a reposição fique correta.
                    // Além disso, temos que salvar os dados da chapa nos dados da reposição, pois se o usuário voltar a situação da peça, tudo deve ser revertido.
                    if (quantidadeLeiturasChapa > 0)
                    {
                        ChapaCortePecaDAO.Instance.MarcarPecaRepostaChapaCortePeca(transaction, idProdPedProducao);
                    }

                    if (prodPedEsp.IsProdFilhoLamComposicao)
                    {
                        ChapaCortePecaDAO.Instance.DeleteByIdProdImpressaoChapa(transaction, dados.IdProdImpressao);
                    }
                    else
                    {
                        ChapaCortePecaDAO.Instance.DeleteByIdProdImpressaoPeca(transaction, dados.IdProdImpressao, idProdPedProducao);
                    }

                    #endregion

                    // Atualiza a situação da produção do pedido para pendente, desde que não seja mão de obra
                    if (!PedidoDAO.Instance.IsMaoDeObra(transaction, idPedido))
                        objPersistence.ExecuteCommand(transaction, "update pedido set dataPronto=null, situacaoProducao=" + (int)Pedido.SituacaoProducaoEnum.Pendente + " where idPedido=" + idPedido);

                    //Remove o plano de corte da impressão e a posição do plano de corte
                    if (dados != null)
                    {
                        // Remove dos produtos_impressao
                        objPersistence.ExecuteCommand(transaction, @"
                            UPDATE produto_impressao 
                            set planoCorte=null, posicaoArqOtimiz=null 
                            WHERE idProdImpressao=" + dados.IdProdImpressao);

                        /* Chamado 23141.
                         * Remove o plano de corte do produto de produção. */
                        objPersistence.ExecuteCommand(transaction, @"
                            UPDATE produto_pedido_producao SET Planocorte=NULL
                            WHERE PlanoCorte=?planoCorte AND IdProdPedProducao=?idProdPedProducao",
                            new GDAParameter("?planoCorte", dados.PlanoCorte),
                            new GDAParameter("?idProdPedProducao", idProdPedProducao));
                    }

                    var temEntradaEstoque = false;

                    foreach (var idSetor in leituras.Select(f => f.IdSetor))
                    {
                        if (SetorDAO.Instance.IsEntradaEstoque(transaction, idSetor))
                        {
                            temEntradaEstoque = true;
                            break;
                        }
                    }

                    // Altera o estoque (caso seja um pedido de produção e ter passado por algum setor entrada de estoque
                    if (PedidoDAO.Instance.IsProducao(transaction, idPedido) && temEntradaEstoque)
                    {
                        string codEtiqueta = ObtemEtiqueta(transaction, idProdPedProducao);
                        LoginUsuario login = UserInfo.GetUserInfo;

                        float m2Calc = Glass.Global.CalculosFluxo.ArredondaM2(transaction, prodPedEsp.Largura, (int)prodPedEsp.Altura, 1, 0, prodPedEsp.Redondo);
                        bool m2 = new List<int>
                        {
                            (int)Glass.Data.Model.TipoCalculoGrupoProd.M2,
                            (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto
                        }
                        .Contains(Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodPedEsp.IdGrupoProd, (int)prodPedEsp.IdSubgrupoProd));

                        MovEstoqueDAO.Instance.BaixaEstoqueProducao(transaction, prodPedEsp.IdProd, login.IdLoja, idProdPedProducao, 1, 0, false, false, true);

                        // Só baixa apenas se a peça possuir produto para baixa associado
                        MovEstoqueDAO.Instance.CreditaEstoqueProducao(transaction, prodPedEsp.IdProd, login.IdLoja, idProdPedProducao, (decimal)(m2Calc > 0 ? m2Calc : 1), true, true);

                        // Marca que este produto não entrou em estoque
                        objPersistence.ExecuteCommand(transaction, "Update produto_pedido_producao Set entrouEstoque=false Where idProdPedProducao=" + idProdPedProducao);
                    }


                    retorno = prodPedEsp.DescrProduto + " " + prodPedEsp.Largura + "x" + prodPedEsp.Altura;

                    transaction.Commit();
                    transaction.Close();

                    return "PEÇA REPOSTA: " + retorno + " (" + numEtiqueta + ")";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    ErroDAO.Instance.InserirFromException("Falha ao marcar peça reposta", ex);
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao marcar peça reposta.", ex));
                }
            }
        }

        /// <summary>
        /// Verifica se a peça pode ser reposta pelo controle de reposição por pedido
        /// </summary>
        public bool PodeReporPeca(uint idProdPed, string etiqueta)
        {
            // Busca todos os produtos que já possam ter reposto o produto passado
            string idsProdPedRepos = ProdutosPedidoDAO.Instance.GetValoresCampo("Select idProdPed From produtos_pedido Where idprodPedAnterior=" +
                idProdPed + " and numEtiquetaRepos=?numEtiqueta", "idProdPed", new GDAParameter("?numEtiqueta", etiqueta));

            if (String.IsNullOrEmpty(idsProdPedRepos))
                return true;

            return objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_pedido_producao Where idProdPed in (" + idsProdPedRepos + ")" +
                " And numEtiqueta=?numEtiqueta And situacao<>" + (int)ProdutoPedidoProducao.SituacaoEnum.Perda,
                new GDAParameter("?numEtiqueta", etiqueta)) == 0;
        }

        #endregion

        #region Insere nova peça

        public void InserePeca(uint? idImpressao, string numEtiqueta, string planoCorte, uint idFunc, bool dataVazia)
        {
            InserePeca(null, idImpressao, numEtiqueta, planoCorte, idFunc, dataVazia);
        }

        public void InserePeca(GDASession session, uint? idImpressao, string numEtiqueta, string planoCorte, uint idFunc, bool dataVazia)
        {
            InserePeca(session, idImpressao, numEtiqueta, planoCorte, idFunc, dataVazia, null, 0, null, null);
        }

        /// <summary>
        /// Insere uma nova peça no controle de produção
        /// </summary>
        public void InserePeca(GDASession session, uint? idImpressao, string numEtiqueta, string planoCorte, uint idFunc, bool dataVazia,
            bool? isPecaReposta, uint idProdPedEsp, string numEtiquetaParent, string posEtiquetaParent)
        {
            // Valida a etiqueta
            ValidaEtiquetaProducao(session, ref numEtiqueta);

            if (isPecaReposta == null)
                isPecaReposta = IsPecaReposta(session, numEtiqueta, true);

            int? idProdBaixaEst = 0;

            // Deixa esta validação aqui, o motivo é chamar menos vezes a verificação de peça reposta
            if (dataVazia && isPecaReposta.Value)
                return;

            bool atualizarSitPedido = false;
            uint idPedido = Glass.Conversoes.StrParaUint(numEtiqueta.Split('-')[0]);

            DateTime? dataLeitura = dataVazia ? null : (DateTime?)DateTime.Now;
            uint idProdPedProducao = 0;

            if (isPecaReposta.Value)
            {
                var idProdPedProducaoPorSituacao = ObtemIdProdPedProducaoPorSituacao(session, numEtiqueta, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);
                if (
                    idProdPedProducaoPorSituacao != null)
                    idProdPedProducao = idProdPedProducaoPorSituacao.Value;

                LeituraProducao leituraProd = LeituraProducaoDAO.Instance.LeituraPeca(session, idProdPedProducao, 1, idFunc, dataLeitura, false, null);
                if (LeituraProducaoDAO.Instance.Insert(session, leituraProd) == 0)
                    throw new Exception("Falha ao inserir peça no setor impr. etiqueta.");

                // Atualiza o idImpressao da peça reposta
                if (idImpressao.GetValueOrDefault(0) > 0)
                    if (idImpressao != null)
                        objPersistence.ExecuteCommand(session, "Update produto_pedido_producao Set idImpressao=" + idImpressao.Value + " Where idProdPedProducao=" + idProdPedProducao);

                atualizarSitPedido = true;
            }
            // Verifica se a etiqueta já está em produção
            else if (EstaEmProducao(session, numEtiqueta))
            {
                idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(session, numEtiqueta).GetValueOrDefault();
                if (idProdPedProducao == 0)
                    throw new Exception("Etiqueta não encontrada.");

                // Caso esta peça tenha sido gerada pela otimização, atualiza o id da impressão
                if (idImpressao > 0)
                    objPersistence.ExecuteCommand(session, "update produto_pedido_producao set idImpressao=" + idImpressao +
                        " where idProdPedProducao=" + idProdPedProducao);

                //Estava ocorrendo um erro que não existia a leitura no momento de fazer o update para setar a data e func.
                if (LeituraProducaoDAO.Instance.ObtemIdLeituraProd(session, idProdPedProducao, 1) == 0)
                {
                    LeituraProducao leituraProd = LeituraProducaoDAO.Instance.LeituraPeca(session, idProdPedProducao, 1, idFunc, dataLeitura, false, null);

                    if (LeituraProducaoDAO.Instance.Insert(session, leituraProd) == 0)
                        throw new Exception("Falha ao inserir peça no setor impr. etiqueta. Etq: " + numEtiqueta + " IdProdPedProducao: " + idProdPedProducao);
                }
                else
                {
                    // Caso esta peça tenha sido gerada pela otimização, atualiza a data da leitura
                    objPersistence.ExecuteCommand(session, @"Update leitura_producao Set dataLeitura=?dataLeitura, idFuncLeitura=?idFunc 
                    Where dataLeitura is null And idProdPedProducao=" + idProdPedProducao,
                        new GDAParameter("?dataLeitura", dataLeitura), new GDAParameter("?idFunc", idFunc));
                }

                atualizarSitPedido = true;
            }
            else
            {
                try
                {
                    // Busca o produto ao qual se refere a etiqueta
                    // A opção true foi modificada dia 15/02/13, para que ao gerar peças na produção após gerar arquivo de exportação
                    // as peças inseridas estavam ficando incorretas, por estarem considerando as peças invisíveis no pedido.
                    if (idProdPedEsp == 0)
                    {
                        var prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(session, numEtiqueta, null, true);

                        if (prodPedEsp == null)
                            throw new Exception("Produto pedido espelho não encontrado. Etq: " + numEtiqueta);

                        idProdPedEsp = prodPedEsp.IdProdPed;
                        idProdBaixaEst = prodPedEsp.IdProdBaixaEst;
                    }

                    uint? idProdPedProducaoParent = null;

                    if (!string.IsNullOrEmpty(numEtiquetaParent))
                        idProdPedProducaoParent = ObtemIdProdPedProducao(session, numEtiquetaParent);

                    // Insere um novo ProdutoPedidoProducao
                    ProdutoPedidoProducao prodPedProducao = new ProdutoPedidoProducao
                    {
                        IdImpressao = idImpressao,
                        IdProdPed = idProdPedEsp,
                        NumEtiqueta = numEtiqueta,
                        Situacao = (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                        IdSetor = 1,
                        PlanoCorte = planoCorte,
                        SituacaoProducao = (int)Glass.Data.Model.SituacaoProdutoProducao.Pendente,
                        IdProdPedProducaoParent = idProdPedProducaoParent,
                        PosEtiquetaParent = posEtiquetaParent,
                        IdProdBaixaEst = idProdBaixaEst
                    };

                    // Verifica mais uma vez se a peça já está em produção devido a um problema ocorrido de duplicar peças na produção
                    if (!EstaEmProducao(session, numEtiqueta))
                    {
                        // Estão ocorrendo três erros misteriosos ao chamar esse insert, "The given key was not present in the dictionary", 
                        // "Probable I/O race condition..." e "Index out of range", alterei para tentar inserir 5 vezes e caso ocorra erro, salva o mesmo no banco
                        var cont = 1;
                        while (true)
                        {
                            try
                            {
                                idProdPedProducao = Insert(session, prodPedProducao);
                                break;
                            }
                            catch
                            {
                                Thread.Sleep(500);

                                if (cont++ == 3)
                                    throw;
                            }
                        }

                        if (idProdPedProducao == 0)
                            throw new Exception("O produto pedido produção não foi inserido.");

                        LeituraProducao leituraProd = LeituraProducaoDAO.Instance.LeituraPeca(session, idProdPedProducao, 1, idFunc, dataLeitura, false, null);

                        if (LeituraProducaoDAO.Instance.Insert(session, leituraProd) == 0)
                            throw new Exception("Falha ao inserir peça no setor impr. etiqueta. Etq: " + numEtiqueta + " IdProdPedProducao: " + idProdPedProducao);

                        atualizarSitPedido = !dataVazia;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Falha ao inserir peça na produção. Etq: " + numEtiqueta + " PPP: " + idProdPedProducao + " - " + ex.Message +
                    (ex.InnerException != null ? " - " + ex.InnerException.Message : ""), ex);
                }
            }

            #region Ajusta referência produto filho/pai

            // Recupera o produto de produção cancelado da etiqueta que está sendo inserida.
            var idsProdPedProducaoCancelado = ExecuteMultipleScalar<int>(session, "SELECT IdProdPedProducao FROM produto_pedido_producao WHERE NumEtiquetaCanc=?numEtiquetaCanc", new GDAParameter("?numEtiquetaCanc", numEtiqueta));

            /* Chamado 46799.
             * Se a impressão tiver sido cancelada e estiver imprimindo novamente é preciso atualizar a referência dos produtos filhos/pai.
             * A verificação é feita somente se o produto de produção atual não possuir filho ou pai. */
            if (idsProdPedProducaoCancelado != null && idsProdPedProducaoCancelado.Count > 0
                && !VerificarProdutoPedidoProducaoPossuiFilho(session, (int)idProdPedProducao) && !VerificarProdutoPedidoProducaoPossuiPai(session, (int)idProdPedProducao))
            {
                foreach (var idProdPedProducaoCancelado in idsProdPedProducaoCancelado)
                {
                    // Atualiza o produto de produção pai do produto de produção filho.
                    if (VerificarProdutoPedidoProducaoPossuiFilho(session, idProdPedProducaoCancelado))
                        AtualizarFilhoDoProdutoPedidoProducaoPai(session, idProdPedProducaoCancelado, (int)idProdPedProducao);

                    // Atualiza o produto de produção filho do produto de produção pai.
                    if (VerificarProdutoPedidoProducaoPossuiPai(session, idProdPedProducaoCancelado))
                    {
                        // Recupera o produto de produção pai da peça filha cancelada.
                        var idProdPedProducaoParentCancelado = ObtemValorCampo<int>(session, "IdProdPedProducaoParent", "NumEtiquetaCanc=?numEtiquetaCanc", new GDAParameter("?numEtiquetaCanc", numEtiqueta));
                        // Recupera o campo PosEtiquetaParent do produto filho cancelado.
                        var posEtiquetaParentCancelado = ObterPosEtiquetaParent(session, idProdPedProducaoCancelado);
                        // Atualiza o produto de produção pai da peça filha impressa.
                        AtualizarPaiDoProdutoPedidoProducaoFilho(session, idProdPedProducaoParentCancelado, (int)idProdPedProducao, posEtiquetaParentCancelado);
                    }
                }
            }

            #endregion

            // Atualiza os setores onde a peça deve seguir pelo roteiro
            if (idProdPedProducao > 0)
                RoteiroProducaoEtiquetaDAO.Instance.InserirRoteiroEtiqueta(session, idProdPedProducao);

            // Não faz sentido atualizar a situação da peça ou do pedido quando ainda está inserido as peças (Chamado 17930, lentidão)
            //AtualizaSituacaoPecaNaProducao(idProdPedProducao, dataLeitura, atualizarSitPedido);
            if (idPedido > 0 && !dataVazia)
                objPersistence.ExecuteCommand(session, "update pedido set situacaoProducao=" + (int)Pedido.SituacaoProducaoEnum.Pendente + " where idPedido=" + idPedido);
        }

        #endregion

        #region Produto de produção de Vidro Duplo/Laminado

        /// <summary>
        /// Verifica se o produto de produção é pai de outro produto de produção.
        /// </summary>
        public bool VerificarProdutoPedidoProducaoPossuiFilho(GDASession session, int idProdPedProducao)
        {
            return objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM produto_pedido_producao WHERE IdProdPedProducaoParent={0}",
                idProdPedProducao)) > 0;
        }

        /// <summary>
        /// Atualiza os filhos do produto de produção pai.
        /// </summary>
        public void AtualizarFilhoDoProdutoPedidoProducaoPai(GDASession session, int idProdPedProducaoAntigo, int idProdPedProducaoNovo)
        {
            objPersistence.ExecuteCommand(session, string.Format("UPDATE produto_pedido_producao SET IdProdPedProducaoParent={0} WHERE IdProdPedProducaoParent={1}",
                idProdPedProducaoNovo, idProdPedProducaoAntigo));
        }

        /// <summary>
        /// Verifica se o produto de produção é filho de outro produto de produção.
        /// </summary>
        public bool VerificarProdutoPedidoProducaoPossuiPai(GDASession session, int idProdPedProducao)
        {
            return objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM produto_pedido_producao WHERE IdProdPedProducaoParent > 0 AND IdProdPedProducao={0}",
                idProdPedProducao)) > 0;
        }

        /// <summary>
        /// Atualiza o pai do produto de produção filho.
        /// </summary>
        public void AtualizarPaiDoProdutoPedidoProducaoFilho(GDASession session, int idProdPedProducaoPai, int idProdPedProducaoFilho, string posEtiquetaParent)
        {
            objPersistence.ExecuteCommand(session, string.Format("UPDATE produto_pedido_producao SET IdProdPedProducaoParent={0}, PosEtiquetaParent=?posEtiquetaParent WHERE IdProdPedProducao={1}",
                idProdPedProducaoPai, idProdPedProducaoFilho), new GDAParameter("?posEtiquetaParent", posEtiquetaParent));
        }

        /// <summary>
        /// Obter o valor do campo PosEtiquetaParent.
        /// </summary>
        public string ObterPosEtiquetaParent(GDASession session, int idProdPedProducao)
        {
            var retorno = objPersistence.ExecuteScalar(session, "SELECT PosEtiquetaParent FROM produto_pedido_producao WHERE IdProdPedProducao=" + idProdPedProducao);

            return retorno != null && !string.IsNullOrEmpty(retorno.ToString()) ? retorno.ToString() : string.Empty;
        }

        /// <summary>
        /// Obtém os filhos do produto de producao.
        /// </summary>
        public List<int> ObterIdProdPedProducaoFilhoPeloIdProdPedProducaoParent(GDASession session, int idProdPedProducao)
        {
            var retorno = ExecuteMultipleScalar<int>(session, string.Format("SELECT IdProdPedProducao FROM produto_pedido_producao WHERE IdProdPedProducaoParent={0}", idProdPedProducao));
            return retorno == null || retorno.Count == 0 ? new List<int>() : retorno.ToList();
        }

        /// <summary>
        /// Verifica se o produto de produção possui peças filhas pendentes.
        /// </summary>
        public bool VerificarPaiPossuiFilhosPendentes(GDASession session, int idProdPedProducao)
        {
            return objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM produto_pedido_producao WHERE IdProdPedProducaoParent={0} AND SituacaoProducao={1} AND Situacao={2}",
                idProdPedProducao, (int)SituacaoProdutoProducao.Pendente, (int)ProdutoPedidoProducao.SituacaoEnum.Producao)) > 0;
        }

        #endregion

        #region Verfica se a etiqueta já está em produção

        /// <summary>
        /// Verfica se a etiqueta já está em produção
        /// </summary>
        public bool EstaEmProducao(GDASession session, string numEtiqueta)
        {
            // Valida a etiqueta
            ValidaEtiquetaProducao(session, ref numEtiqueta);

            return objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From produto_pedido_producao Where numEtiqueta=?numEtiqueta And situacao=" +
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                new GDAParameter("?numEtiqueta", numEtiqueta)) > 0;
        }

        /// <summary>
        /// Verfica se alguma peça deste item já está em produção
        /// </summary>
        public bool EstaEmProducao(uint idProdPed)
        {
            return objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_pedido_producao Where idProdPed=" + idProdPed + " And situacao=" +
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao) > 0;
        }

        /// <summary>
        /// Verifica se a peça passada foi lida em algum setor da produção
        /// </summary>
        public bool LeuProducao(GDASession sessao, string numEtiqueta)
        {
            // Valida a etiqueta
            ValidaEtiquetaProducao(sessao, ref numEtiqueta);

            return objPersistence.ExecuteSqlQueryCount(@"
                Select Count(*) 
                From leitura_producao 
                Where dataLeitura is not null 
                    And idprodPedProducao=(
                        Select idprodpedproducao from produto_pedido_producao Where Situacao = ?sit AND numEtiqueta=?numEtiqueta limit 1
                    )", new GDAParameter("?numEtiqueta", numEtiqueta), new GDAParameter("?sit", (int)ProdutoPedidoProducao.SituacaoEnum.Producao)) > 0;
        }

        #endregion

        #region Verifica se a peça esta pronta

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se a peça informada esta pronta
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public bool PecaEstaPronta(string codEtiqueta)
        {
            return PecaEstaPronta(null, codEtiqueta);
        }

        /// <summary>
        /// Verifica se a peça informada esta pronta
        /// </summary>
        public bool PecaEstaPronta(GDASession sessao, string codEtiqueta)
        {
            /* Chamado 16551.
             * A peça foi lida no carregamento sem antes passar por todos os setores
             * definidos no cadastro do roteiro, isso ocorreu porque a peça havia sido
             * lida em um setor que efetua a baixa no estoque. Portanto, nós alteramos
             * este sql para considerar como peça pronta as peças nas situações Pronto e Entregue,
             * ao invés de considerar, também, peças que deram baixa no estoque. */
            /*string sql = @"
                SELECT COUNT(*)
                FROM produto_pedido_producao ppp
                    INNER JOIN setor s ON (ppp.idSetor = s.idSetor)
                WHERE ppp.numEtiqueta=?codEtiqueta AND (ppp.situacaoProducao IN (" + (int)SituacaoProdutoProducao.Pronto + "," +
                    (int)SituacaoProdutoProducao.Entregue + ") OR ppp.entrouEstoque)";*/
            var sql = @"
                SELECT COUNT(*)
                FROM produto_pedido_producao ppp
                    INNER JOIN setor s ON (ppp.idSetor = s.idSetor)
                WHERE ppp.numEtiqueta=?codEtiqueta AND ppp.situacaoProducao IN (" + (int)SituacaoProdutoProducao.Pronto + "," +
                    (int)SituacaoProdutoProducao.Entregue + ")";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?codEtiqueta", codEtiqueta)) > 0;
        }

        /// <summary>
        /// Retorna a quantidade de peças de produção prontas, com base na lista de produtos de produção informados.
        /// </summary>
        public int ObterQuantidadePecasProntas(GDASession sessao, List<int> idsProdPedProducao)
        {
            if (idsProdPedProducao == null || !idsProdPedProducao.Any(f => f > 0))
                return 0;

            var sql = string.Format(@"SELECT COUNT(*)
                FROM produto_pedido_producao ppp
                    INNER JOIN setor s ON (ppp.IdSetor = s.IdSetor)
                WHERE ppp.IdProdPedProducao IN ({0}) AND ppp.SituacaoProducao IN ({1},{2});",
                string.Join(",", idsProdPedProducao.Where(f => f > 0).ToList()), (int)SituacaoProdutoProducao.Pronto, (int)SituacaoProdutoProducao.Entregue);

            return objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        #endregion

        #region Verifica a quantidade do produto que já foi liberada para um pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna a quantidade de produtos já liberados para um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idProdPed">Utilizado caso haja mais de um produto pedido com o mesmo produto</param>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public int GetQtdeLiberadaByPedProd(uint idPedido, uint? idProdPed, uint idProd)
        {
            return GetQtdeLiberadaByPedProd(null, idPedido, idProdPed, idProd);
        }

        /// <summary>
        /// Retorna a quantidade de produtos já liberados para um pedido.
        /// </summary>
        public int GetQtdeLiberadaByPedProd(GDASession sessao, uint idPedido, uint? idProdPed, uint idProd)
        {
            string sql = "select count(*) from produto_pedido_producao where idPedidoExpedicao=" + idPedido;

            if (idProdPed == null)
                sql += " and idProdPed in (select idProdPed from produtos_pedido_espelho where idProd=" + idProd + ")";
            else
                sql += " and idProdPed=" + idProdPed.Value;

            var retorno = objPersistence.ExecuteSqlQueryCount(sessao, sql);

            sql = @"
                SELECT COUNT(*)
                FROM produto_impressao
                WHERE idPedidoExpedicao=" + idPedido + @"
                    AND idProdNf in (SELECT idProdNf FROM produtos_nf WHERE idProd=" + idProd + ")";

            return retorno + objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        #endregion

        #region Busca plano de corte pelo número da etiqueta

        /// <summary>
        /// Busca plano de corte pelo número da etiqueta
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public string GetPlanoCorte(string numEtiqueta)
        {
            // Valida a etiqueta
            ValidaEtiquetaProducao(null, ref numEtiqueta);

            object planoCorte = objPersistence.ExecuteScalar("Select planoCorte From produto_pedido_producao Where numEtiqueta=?numEtiqueta",
                new GDAParameter[] { new GDAParameter("?numEtiqueta", numEtiqueta) });

            return planoCorte != null ? planoCorte.ToString() : null;
        }

        #endregion

        #region Obtem valores dos campos

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o id da tabela com base na etiqueta.
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public uint? ObtemIdProdPedProducao(string codEtiqueta)
        {
            return ObtemIdProdPedProducao(null, codEtiqueta);
        }

        public List<uint> ObtemIdsProdPedProducaoByIdProdImpressao(GDASession session, string idsProdImpressao)
        {
            var sql = @"
                SELECT DISTINCT ppp.IdProdPedProducao
                FROM produto_impressao pi
                    INNER JOIN produto_pedido_producao ppp ON (ppp.NumEtiqueta = pi.NumEtiqueta)
                    LEFT JOIN chapa_corte_peca ccp ON (ccp.IdProdImpressaoPeca = pi.IdProdImpressao)
                WHERE ccp.IdChapaCortePeca IS NULL AND pi.IdProdImpressao IN (" + idsProdImpressao + ")";

            return ExecuteMultipleScalar<uint>(session, sql);
        }

        /// <summary>
        /// Retorna o id da tabela com base na etiqueta.
        /// </summary>
        public uint? ObtemIdProdPedProducao(GDASession sessao, string codEtiqueta)
        {
            try
            {
                // Valida a etiqueta
                ValidaEtiquetaProducao(sessao, ref codEtiqueta);
            }
            catch
            {
                return null;
            }

            object id = objPersistence.ExecuteScalar(sessao,
                "Select idProdPedProducao From produto_pedido_producao Where numEtiqueta=?codEtiqueta And situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                new GDAParameter("?codEtiqueta", codEtiqueta));

            return id != null ? Glass.Conversoes.StrParaUintNullable(id.ToString()) : null;
        }

        public uint? ObtemIdProdPedProducaoCanc(GDASession session, string numEtiquetaCanc)
        {
            try
            {
                // Valida a etiqueta
                ValidaEtiquetaProducao(null, ref numEtiquetaCanc);
            }
            catch
            {
                return null;
            }

            return ExecuteScalar<uint?>(session, "Select idProdPedProducao From produto_pedido_producao Where numEtiquetaCanc=?numEtiquetaCanc",
                new GDAParameter("?numEtiquetaCanc", numEtiquetaCanc));
        }

        public bool PedidoProducaoGeradoPorPedidoRevenda(GDASession sessao, uint idPedido)
        {
            return objPersistence.ExecuteScalar(sessao,
                "Select coalesce(IdPedidoRevenda, 0) > 0 From pedido Where IdPedido=?idPedido",
                new GDAParameter("?idPedido", idPedido)).ToString().StrParaInt() > 0;
        }

        public uint? ObtemIdProdPedProducaoPorSituacao(GDASession sessao, string codEtiqueta, int situacao)
        {
            try
            {
                // Valida a etiqueta
                ValidaEtiquetaProducao(sessao, ref codEtiqueta);
            }
            catch
            {
                return null;
            }

            object id = objPersistence.ExecuteScalar(sessao,
                "Select idProdPedProducao From produto_pedido_producao Where numEtiqueta=?codEtiqueta and situacao=?sit",
                new GDAParameter("?codEtiqueta", codEtiqueta), new GDAParameter("?sit", situacao));

            return id != null ? Glass.Conversoes.StrParaUintNullable(id.ToString()) : null;
        }

        /// <summary>
        /// Retorna a quantidade de produtos expedidos do produto e pedido de revenda passados
        /// </summary>
        public int ObtemQtdLiberadaRevenda(GDASession session, uint idPedido, uint idProd)
        {
            var sql = String.Format(@"
                Select Count(*) 
                From produto_pedido_producao ppp 
                    Inner Join produtos_pedido_espelho ppe On (ppp.idProdPed=ppe.idProdPed)
                Where ppp.idPedidoExpedicao={0} and ppe.idProd={1}", idPedido, idProd);

            return ExecuteScalar<int>(session, sql);
        }

        public uint? ObtemIdProdPedProducaoPorSituacao(string codEtiqueta, int situacao)
        {
            return ObtemIdProdPedProducaoPorSituacao(null, codEtiqueta, situacao);
        }

        public uint? ObterIdProdPedProducaoParent(GDASession session, uint idProdPedProducao)
        {
            return ObtemValorCampo<uint?>(session, "IdProdPedProducaoParent", string.Format("IdProdPedProducao={0}", idProdPedProducao));
        }

        public string ObterNumEtiqueta(GDASession session, uint idProdPedProducao)
        {
            return ObtemValorCampo<string>(session, "NumEtiqueta", string.Format("IdProdPedProducao={0}", idProdPedProducao));
        }

        public uint ObtemIdSetor(uint idProdPedProducao)
        {
            return ObtemIdSetor(null, idProdPedProducao);
        }

        public uint ObtemIdSetor(GDASession session, uint idProdPedProducao)
        {
            return ObtemValorCampo<uint>(session, "idSetor", "idProdPedProducao=" + idProdPedProducao);
        }

        public SituacaoProdutoProducao ObtemSituacaoProducao(GDASession sessao, uint idProdPedProducao)
        {
            return (SituacaoProdutoProducao)ObtemValorCampo<int>(sessao, "situacaoProducao", "idProdPedProducao=" + idProdPedProducao);
        }

        public SituacaoProdutoProducao ObtemSituacaoProducao(GDASession sessao, string codEtiqueta)
        {
            uint? idProdPedProducao = ObtemIdProdPedProducao(codEtiqueta);
            return (SituacaoProdutoProducao)ObtemValorCampo<int>(sessao, "situacaoProducao", "idProdPedProducao=" + idProdPedProducao.GetValueOrDefault(0));
        }

        public SituacaoProdutoProducao ObtemSituacaoProducao(uint idProdPedProducao)
        {
            return ObtemSituacaoProducao(null, idProdPedProducao);
        }

        /// <summary>
        /// Obtem o Id da impressão da etiqueta.
        /// </summary>
        public uint ObtemIdImpressaoByEtiqueta(GDASession session, string codEtiqueta)
        {
            uint? idProdPedProducao = ObtemIdProdPedProducao(session, codEtiqueta);
            return idProdPedProducao > 0 ? ObtemValorCampo<uint>(session, "idImpressao", "idProdPedProducao=" + idProdPedProducao) : 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtem o número da etiqueta.
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public string ObtemEtiqueta(uint idProdPedProducao)
        {
            return ObtemEtiqueta(null, idProdPedProducao);
        }

        /// <summary>
        /// Obtem o número da etiqueta.
        /// </summary>
        public string ObtemEtiqueta(GDASession sessao, uint idProdPedProducao)
        {
            return ObtemValorCampo<string>(sessao, "numEtiqueta", "idProdPedProducao=" + idProdPedProducao);
        }

        /// <summary>
        /// Obtem os números das etiqutas do cliente pelo IdProdPedEsp
        /// </summary>
        public string ObtemEtiquetasCliente(GDASession sessao, uint idProdPedEsp)
        {
            var sql = "numEtiquetaCliente where idProdPed=" + idProdPedEsp;

            var numEtiquetas = objPersistence.LoadData(sessao, sql);

            return numEtiquetas != null ? string.Join(",", numEtiquetas.Select(f => f.NumEtiquetaCliente)) : string.Empty;
        }

        /// <summary>
        /// Obtem o número da etiqueta.
        /// </summary>
        public string ObtemEtiquetaChapa(GDASession session, int idProdPedProducao)
        {
            object retorno = objPersistence.ExecuteScalar(session,
                string.Format(
                    @"SELECT pichapa.NumEtiqueta FROM produto_pedido_producao ppp
                        INNER JOIN produto_impressao pi ON (ppp.NumEtiqueta=pi.NumEtiqueta)
                        INNER JOIN chapa_corte_peca ccp ON (pi.IdProdImpressao=ccp.IdProdImpressaoPeca)
                        INNER JOIN produto_impressao pichapa ON (ccp.IdProdImpressaoChapa=pichapa.IdProdImpressao)
                    WHERE ppp.IdProdPedProducao={0} ORDER BY IdChapaCortePeca DESC LIMIT 1 ", idProdPedProducao));

            return retorno != null ? retorno.ToString() : string.Empty;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtem o IdProdPed.
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public uint ObtemIdProdPed(uint idProdPedProducao)
        {
            return ObtemIdProdPed(null, idProdPedProducao);
        }

        /// <summary>
        /// Obtem o IdProdPed.
        /// </summary>
        public uint ObtemIdProdPed(GDASession sessao, uint idProdPedProducao)
        {
            return ObtemValorCampo<uint>(sessao, "idProdPed", "idProdPedProducao=" + idProdPedProducao);
        }

        /// <summary>
        /// Obtem o IdProdPed.
        /// </summary>
        public uint ObtemIdProdPed(GDASession sessao, string etiqueta)
        {
            return ObtemValorCampo<uint>(sessao, "idProdPed", "NumEtiqueta=?etq", new GDAParameter("?etq", etiqueta));
        }

        /// <summary>
        /// Obtem o número do pedido, pela etiqueta.
        /// </summary>
        public uint ObtemIdPedido(GDASession sessao, string etiqueta)
        {
            var idProdPedProducao = ObtemIdProdPedProducao(sessao, etiqueta).GetValueOrDefault();

            if (idProdPedProducao == 0)
                return 0;

            return ObtemIdPedido(sessao, idProdPedProducao);
        }

        /// <summary>
        /// Obtem o número do pedido.
        /// </summary>
        public uint ObtemIdPedido(GDASession sessao, uint idProdPedProducao)
        {
            uint idProdPed = ObtemIdProdPed(sessao, idProdPedProducao);
            return ProdutosPedidoEspelhoDAO.Instance.ObtemIdPedido(sessao, idProdPed);
        }

        /// <summary>
        /// Obtem o total de m2 que devem passar no setor.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public TotaisSetor ObtemTotaisSetor(uint idSetor)
        {
            using (var session = new GDASession())
            {
                var setores = CurrentPersistenceObject.LoadResult(session,
                    "SELECT IdSetor, NumSeq FROM Setor WHERE Situacao=?situacao ORDER BY NumSeq",
                    new GDAParameter("?situacao", Glass.Situacao.Ativo))
                    .Select(f => new
                    {
                        IdSetor = f.GetInt32("IdSetor"),
                        NumSeq = f.GetInt32("NumSeq")
                    }).ToArray();


                string comandoTotal =
                    @" /*ProdutoPedidoProducaoDAO.ObtemTotaisSetor()*/
                    SELECT 
	                    COUNT(*) as Qtde,
	                    SUM(Round(if(ped.tipoPedido=3 /* MaoDeObra */, (
	                    /*Caso o pedido for de mão de obra então o m2 da peça é cosiderado*/
	                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
	                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
	                    * a.qtde, pp.TotM2Calc) / (pp.qtde * If(ped.tipoPedido=3 /* MaoDeObra */, a.qtde, 1)), 4)) As TotM2
                    FROM produto_pedido_producao ppp
                        INNER JOIN produtos_pedido_espelho pp ON (ppp.idProdPed = pp.idProdPed)
                        INNER JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                        LEFT JOIN ambiente_pedido_espelho a ON (pp.idAmbientePedido=a.idAmbientePedido)
                        INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.idProdPedProducao = ppp.idProdPedProducao AND rpe.IdSetor = ?idSetor) 
                        INNER JOIN setor srot ON (rpe.IdSetor = srot.IdSetor)
                        INNER JOIN setor s ON (ppp.IdSetor = s.IdSetor)
                    WHERE s.NumSeq < srot.NumSeq
                        AND ppp.situacao = ?situacaoPPP
                        AND  ppp.IdSetor <> rpe.IdSetor 
                        AND ppp.situacaoProducao = ?situacaoProducaoPPP";

                var total = CurrentPersistenceObject.LoadResult(session, comandoTotal,
                    new GDAParameter("?idSetor", idSetor),
                    new GDAParameter("?situacaoPPP", ProdutoPedidoProducao.SituacaoEnum.Producao),
                    new GDAParameter("?situacaoProducaoPPP", SituacaoProdutoProducao.Pendente))
                    .Select(f => new
                    {
                        Quatidade = f.GetInt32("Qtde"),
                        TotalM2 = f.IsDBNull("TotM2") ? 0 : f.GetDouble("TotM2")
                    }).First();

                var comandoTotaisMomento =
                    @"SELECT 
	                    COUNT(*) as Qtde,
	                    SUM(Round(if(ped.tipoPedido=3 /* MaoDeObra */, (
	                    /*Caso o pedido for de mão de obra então o m2 da peça é cosiderado*/
	                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
	                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
	                    * a.qtde, pp.TotM2Calc) / (pp.qtde * If(ped.tipoPedido=3 /* MaoDeObra */, a.qtde, 1)), 4)) As TotM2
                    FROM produto_pedido_producao ppp
                        INNER JOIN produtos_pedido_espelho pp ON (ppp.idProdPed = pp.idProdPed)
                        INNER JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                        LEFT JOIN ambiente_pedido_espelho a ON (pp.idAmbientePedido=a.idAmbientePedido)
                    WHERE ppp.situacao = ?situacaoPPP
                        AND ppp.situacaoProducao = ?situacaoProducaoPPP
	                    -- Recupera o setor anterio com base no roteiro
                        AND ppp.IdSetor = (
			                    SELECT rpe.IdSetor
			                    FROM roteiro_producao_etiqueta rpe
			                    INNER JOIN setor s ON (s.IdSetor = ?idSetor)
			                    INNER JOIN setor srot ON (rpe.IdSetor = srot.IdSetor)
			                    WHERE rpe.idProdPedProducao = ppp.idProdPedProducao AND srot.NumSeq < s.NumSeq
			                    ORDER BY srot.NumSeq DESC LIMIT 1)";

                var totalMomento = CurrentPersistenceObject.LoadResult(session, comandoTotaisMomento,
                    new GDAParameter("?idSetor", idSetor),
                    new GDAParameter("?situacaoPPP", ProdutoPedidoProducao.SituacaoEnum.Producao),
                    new GDAParameter("?situacaoProducaoPPP", SituacaoProdutoProducao.Pendente))
                    .Select(f => new
                    {
                        Quatidade = f.GetInt32("Qtde"),
                        TotalM2 = f.IsDBNull("TotM2") ? 0 : f.GetDouble("TotM2")
                    }).First();

                return new TotaisSetor
                {
                    TotalPecas = total.Quatidade,
                    TotalPecasM2 = total.TotalM2,
                    TotalPecasMomento = totalMomento.Quatidade,
                    TotalPecasMomentoM2 = totalMomento.TotalM2
                };

            }
        }

        /// <summary>
        /// Obtem o total de m2 do setor.
        /// </summary>
        public float ObtemTotM2Setor(int idSetor)
        {
            return ObtemTotM2Setor(idSetor, null, null, null, true);
        }

        /// <summary>
        /// Obtem o total de m2 do setor.
        /// </summary>
        /// <returns>Retorna o total de m2 do setor</returns>
        public float ObtemTotM2Setor(int idSetor, int? idClassificacao, string dataIniFabr, string dataFimFabr, bool selecionar)
        {
            var sql = @"
                SELECT ROUND(IF(ped.tipoPedido = " + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", ((((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) + a.altura) * 
                    ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000) * a.qtde, pp.TotM2Calc) / 
                    (pp.qtde * if(ped.tipoPedido = " + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as totM
             FROM produto_pedido_producao ppp
				INNER JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
				INNER JOIN produto p ON (pp.IdProd = p.IdProd)
                INNER JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                INNER JOIN pedido_espelho pedEsp On (ped.idPedido = pedEsp.idPedido)  
				LEFT JOIN ambiente_pedido_espelho a On (pp.idAmbientePedido = a.idAmbientePedido)
                    WHERE 1  
                    AND ppp.IdSetor = " + idSetor + @" 
                    AND ppp.Situacao IN (" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + ", " + (int)ProdutoPedidoProducao.SituacaoEnum.Perda + @") 
                    AND exists (SELECT * FROM leitura_producao where IdProdPedProducao = ppp.IdProdPedProducao AND idSetor = " + idSetor + @" AND dataLeitura IS NOT NULL) 
                    AND pp.IdProdPedParent IS NULL";

            if (!string.IsNullOrWhiteSpace(dataIniFabr))
                sql += " AND pedEsp.DataFabrica >= ?dataIniFabr";

            if (!string.IsNullOrWhiteSpace(dataIniFabr))
                sql += " AND pedEsp.DataFabrica <= ?dataFimFabr";

            if (idClassificacao.GetValueOrDefault(0) > 0)
                sql += " AND pp.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao = " + idClassificacao.Value + ")";

            if (selecionar)
                sql = "SELECT sum(totM) FROM (" + sql + ") As tmp";
            else
                sql = "SELECT Count(*) FROM (" + sql + ") As tmp";

            return ExecuteScalar<float>(sql, new GDAParameter("?dataIniFabr", dataIniFabr), new GDAParameter("?dataFimFabr", dataFimFabr));
        }

        /// <summary>
        /// Obtem o total de m2 lido na data específica do setor.
        /// </summary>
        /// <returns>Retorna o total de m2 lido na data específica do setor.</returns>
        public float ObtemTotM2LidoSetor(int idSetor, DateTime? dataIni, DateTime? dataFim)
        {
            return ObtemTotM2LidoSetor(idSetor, null, dataIni, dataFim, true);
        }

        /// <summary>
        /// Obtem o total de m2 lido na data específica do setor.
        /// </summary>
        /// <returns>Retorna o total de m2 lido na data específica do setor.</returns>
        public float ObtemTotM2LidoSetor(int idSetor, int? idClassificacao, DateTime? dataIni, DateTime? dataFim, bool selecionar)
        {
            var lstParam = new List<GDAParameter>();

            var sql = string.Format(@"
                Select {0} From (
                    Select Round(if(ped.tipoPedido=3, (
                        /*Caso o pedido for de mão de obra então o m2 da peça é cosiderado*/
                        (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
                        a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
                        * a.qtde, ppo.TotM2Calc) / (pp.qtde * If(ped.tipoPedido=3, a.qtde, 1)), 4) As TotM2
                From pedido ped
                    Inner Join produtos_pedido_espelho pp On (ped.idPedido=pp.idPedido)
	                Inner Join produtos_pedido ppo On (ppo.idProdPedEsp=pp.idProdPed)
                    Inner Join produto_pedido_producao ppp ON (ppp.idProdPed=pp.idProdPed)
                    Left Join ambiente_pedido_espelho a ON (pp.idAmbientePedido=a.idAmbientePedido)
                    Left Join leitura_producao lp1 ON (ppp.idProdPedProducao=lp1.idProdPedProducao)
                Where lp1.idSetor={1}", selecionar ? "SUM(TotM2)" : "COUNT(*)", idSetor);

            if (dataIni != null)
            {
                sql += " And lp1.dataLeitura>=?dataIni";
                lstParam.Add(new GDAParameter("?dataIni", dataIni));
            }

            if (dataFim != null)
            {
                sql += " And lp1.dataLeitura<=?dataFim";
                lstParam.Add(new GDAParameter("?dataFim", dataFim));
            }

            /* Chamado 45622. */
            if (idClassificacao > 0)
                sql += string.Format(" AND pp.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0}) ", idClassificacao.Value);

            sql += " Group By ppp.idProdPedProducao) As temp";

            return ExecuteScalar<float>(sql, lstParam.ToArray());
        }

        /// <summary>
        /// Obtem o total de m2 das peças impressas que devem ficar prontas no dia conforme a data de fábrica dos pedidos em conferência.
        /// </summary>
        /// <returns>Retorna o total de m2 das peças impressas que devem ficar prontas no dia conforme a data de fábrica dos pedidos em conferência.</returns>
        public float ObtemM2MetaProdDia()
        {
            var sql = @"
                Select Cast(Sum(ppe.totM / ppe.qtde) As Decimal(10, 2)) From produto_pedido_producao ppp
                    Left Join produtos_pedido_espelho ppe ON (ppp.idprodped=ppe.idprodped)
                    Left Join pedido_espelho pe ON (ppe.idpedido=pe.idpedido)
                Where Date_Format(pe.datafabrica, '%d-%M-%Y')=Date_Format(Now(), '%d-%M-%Y')
                    And ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

            return ExecuteScalar<float>(sql);
        }

        /// <summary>
        /// Obtem o total de m2 das peças impressas que devem ficar prontas no dia conforme a data de fábrica dos pedidos em conferência.
        /// </summary>
        /// <returns>Retorna o total de m2 das peças impressas que devem ficar prontas no dia conforme a data de fábrica dos pedidos em conferência.</returns>
        public float ObtemM2MetaProdDiaClassificacao(int idClassificacao)
        {
            var sql = @"
                Select Cast(Sum(ppe.totM / ppe.qtde) As Decimal(10, 2)) From produto_pedido_producao ppp
                    Left Join produtos_pedido_espelho ppe ON (ppp.idprodped=ppe.idprodped)
                    Left Join pedido_espelho pe ON (ppe.idpedido=pe.idpedido)
                Where Date_Format(pe.datafabrica, '%d-%M-%Y')=Date_Format(Now(), '%d-%M-%Y') " +
                    string.Format("AND ppe.IdProcesso IN (SELECT IdRoteiroProducao FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao = {0})", idClassificacao) +
                    " And ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

            return ExecuteScalar<float>(sql);
        }

        /// <summary>
        /// Obtem o id do pedido de expedição
        /// </summary>
        public uint? ObtemIdPedidoExpedicao(string codEtiqueta)
        {
            return ObtemIdPedidoExpedicao(null, codEtiqueta);
        }

        /// <summary>
        /// Obtem o id do pedido de expedição
        /// </summary>
        public uint? ObtemIdPedidoExpedicao(GDASession session, string codEtiqueta)
        {
            uint? idProdPedProducao = ObtemIdProdPedProducao(session, codEtiqueta);
            return idProdPedProducao > 0 ? ObtemIdPedidoExpedicao(session, idProdPedProducao.Value) : null;
        }

        /// <summary>
        /// Obtem o id do pedido de expedição
        /// </summary>
        public uint? ObtemIdPedidoExpedicao(uint idProdPedProducao)
        {
            return ObtemIdPedidoExpedicao(null, idProdPedProducao);
        }

        /// <summary>
        /// Obtem o id do pedido de expedição
        /// </summary>
        public uint? ObtemIdPedidoExpedicao(GDASession session, uint idProdPedProducao)
        {
            return ObtemValorCampo<uint?>(session, "idPedidoExpedicao", "idProdPedProducao=" + idProdPedProducao);
        }

        public bool IsProdLamComposicao(string numEtiqueta)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM produto_pedido_producao ppp
                    INNER JOIN produtos_pedido_espelho ppe ON (ppp.IdProdPed = ppe.IdProdPed)
                    INNER JOIN produto p ON (p.IdProd = ppe.IdProd)
                    INNER JOIN subgrupo_prod sgp ON (sgp.IdSubgrupoProd = p.IdSubgrupoProd)
                WHERE sgp.TipoSubgrupo IN (" + (int)TipoSubgrupoProd.VidroDuplo + "," + (int)TipoSubgrupoProd.VidroLaminado + @")
                    AND ppp.numEtiqueta = ?etq";

            return objPersistence.ExecuteSqlQueryCount(sql, new GDAParameter("?etq", numEtiqueta)) > 0;
        }

        public uint? ObterIdProdPedProducaoParentByEtiqueta(GDASession sessao, string numEtiqueta)
        {
            return ObtemValorCampo<uint?>("idProdPedProducaoParent", "numEtiqueta=?etq", new GDAParameter("?etq", numEtiqueta));
        }

        public string ObterPosEtiquetaParent(GDASession sessao, uint idProdPedProducao)
        {
            return ObtemValorCampo<string>("PosEtiquetaParent", "idProdPedProducao=" + idProdPedProducao);
        }

        public string ObterDescrProdEtiqueta(GDASession sessao, uint idProdPedProducao)
        {
            var sql = @"
            SELECT concat(p.CodInterno, ' - ', if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                    a.Ambiente, concat(p.Descricao, if(pp.Redondo and " + (!BenefConfigDAO.Instance.CobrarRedondo()).ToString() + @", ' REDONDO', ''))) , ' (', COALESCE(ppp.NumEtiqueta, ''), ')')
            FROM produto_pedido_producao ppp
	            INNER JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
	            INNER JOIN produto p ON (pp.IdProd = p.IdProd)
                INNER JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                LEFT JOIN ambiente_pedido_espelho a ON (pp.idAmbientePedido = a.idAmbientePedido)
            WHERE ppp.IdProdPedProducao = " + idProdPedProducao;

            return ExecuteScalar<string>(sessao, sql);
        }

        #endregion

        #region Volta o setor de uma peça

        /// <summary>
        /// Volta uma peça de perda para produção.
        /// </summary>
        public void VoltarPerdaProducao(GDASession sessao, string numEtiqueta, bool salvarLog)
        {
            // Valida a etiqueta
            ValidaEtiquetaProducao(sessao, ref numEtiqueta);

            var id = ObtemIdProdPedProducao(sessao, numEtiqueta) ?? 0;
            var item = id > 0 ? GetElementByPrimaryKey(id) : null;

            objPersistence.ExecuteCommand(sessao, @"update produto_pedido_producao set dataPerda=null, tipoPerda=null, idSubtipoPerda=null, obs=null,
                idFuncPerda=null, situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + " where numEtiqueta=?numEtiqueta",
                new GDAParameter("?numEtiqueta", numEtiqueta));

            AtualizaSituacaoPecaNaProducao(sessao, id, null, true);

            if (item != null && salvarLog)
                LogAlteracaoDAO.Instance.LogProdPedProducao(sessao, item, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        private static readonly object _voltarPecaLock = new object();

        public void VoltarPeca(uint idProdPedProducao, uint? idCarregamento, bool salvarLog)
        {
            lock (_voltarPecaLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        Dictionary<int, SituacaoProdutoProducao> idsPedidoSituacaoProducao = null;

                        VoltarPeca(transaction, idProdPedProducao, idCarregamento, salvarLog, ref idsPedidoSituacaoProducao);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Método usado para voltar peça na tela de consulta de produção.
        /// </summary>
        public void VoltarPeca(GDASession sessao, uint idProdPedProducao, uint? idCarregamento, bool salvarLog)
        {
            Dictionary<int, SituacaoProdutoProducao> idsPedidoSituacaoProducao = null;

            VoltarPeca(sessao, idProdPedProducao, idCarregamento, salvarLog, ref idsPedidoSituacaoProducao);
        }

        /// <summary>
        /// Método usado para voltar peça na tela de consulta de produção.
        /// </summary>
        public void VoltarPeca(GDASession sessao, uint idProdPedProducao, uint? idCarregamento, bool salvarLog,
            ref Dictionary<int, SituacaoProdutoProducao> idsPedidoSituacaoProducao)
        {
            try
            {
                if (ObtemValorCampo<int>(sessao, "situacao", "idProdPedProducao=" + idProdPedProducao) == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
                    VoltarPerdaProducao(sessao, ObtemEtiqueta(sessao, idProdPedProducao), salvarLog);

                // Se a peça estiver reposta, volta para a situação antes da reposição desde que o setor atual seja impressão de etiqueta
                else if (IsPecaReposta(sessao, idProdPedProducao, false) && !String.IsNullOrEmpty(GetDadosReposicaoPeca(sessao, idProdPedProducao)) &&
                    (ObtemValorCampo<uint>(sessao, "idSetor", "idProdPedProducao=" + idProdPedProducao) == 1 ||
                    LeituraProducaoDAO.Instance.ObtemUltimoSetorLido(sessao, idProdPedProducao) == 0))
                {
                    // Chamado 16193: Não permite desfazer a perda da peça caso tenha retalhos associados
                    foreach (var r in RetalhoProducaoDAO.Instance.ObterLista(sessao, idProdPedProducao))
                        if (UsoRetalhoProducaoDAO.Instance.PossuiAssociacao(sessao, r.IdRetalhoProducao, 0))
                            throw new Exception("Não é possível voltar essa peça ao estado anterior pois a mesma gerou retalhos.");

                    VoltarPecaReposta(sessao, idProdPedProducao);
                }
                else
                    RetiraPecaSituacao(sessao, idProdPedProducao, idCarregamento, ref idsPedidoSituacaoProducao);

                foreach (var r in RetalhoProducaoDAO.Instance.ObterLista(sessao, idProdPedProducao))
                {
                    RetalhoProducaoDAO.Instance.AlteraSituacao(sessao, r.IdRetalhoProducao, SituacaoRetalhoProducao.Cancelado);
                    var idPedido = ObtemIdPedido(sessao, idProdPedProducao);
                    var idLojaConsiderar = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0 ?
                    PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido) : UserInfo.GetUserInfo.IdLoja;
                    MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, r.IdProd, idLojaConsiderar,
                        r.IdProdPedProducaoOrig.GetValueOrDefault(), 1, 0, false, false, true);
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("VoltarPeca - idProdPedProducao: " + idProdPedProducao, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Retira peça de sua situação atual
        /// </summary>
        public void RetiraPecaSituacao(GDASession sessao, uint idProdPedProducao, uint? idCarregamento, bool trocaDevolucao = false)
        {
            Dictionary<int, SituacaoProdutoProducao> idsPedidoSituacaoProducao = null;

            RetiraPecaSituacao(sessao, idProdPedProducao, idCarregamento, ref idsPedidoSituacaoProducao, trocaDevolucao);
        }

        /// <summary>
        /// Retira peça de sua situação atual
        /// </summary>
        public void RetiraPecaSituacao(GDASession sessao, uint idProdPedProducao, uint? idCarregamento,
            ref Dictionary<int, SituacaoProdutoProducao> idsPedidoSituacaoProducao, bool trocaDevolucao = false)
        {
            int situacao = ObtemValorCampo<int>(sessao, "situacao", "idProdPedProducao=" + idProdPedProducao);
            var situacaoProducao = ObtemValorCampo<int>(sessao, "situacaoProducao", "idProdPedProducao=" + idProdPedProducao);

            if (situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
            {
                string numEtiqueta = ObtemEtiqueta(sessao, idProdPedProducao);
                VoltarPerdaProducao(sessao, numEtiqueta, true);
            }
            else
            {
                var idPedido = ObtemIdPedido(sessao, idProdPedProducao);

                var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);
                var naoIgnorar = !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(sessao, idLoja);

                /* Chamado 43181. */
                if (Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar && situacaoProducao == (int)SituacaoProdutoProducao.Pronto &&
                    PedidoDAO.Instance.ObtemSituacao(sessao, idPedido) == Pedido.SituacaoPedido.Confirmado && !PedidoDAO.Instance.IsProducao(sessao, idPedido))
                    throw new Exception("Não é possível voltar o setor dessa peça, pois, ela está liberada, pronta e são permitidos somente produtos prontos em liberações.");

                var item = GetElementByPrimaryKey(sessao, idProdPedProducao);

                /* Chamado 55897. */
                if (item.IdProdPedProducaoParent > 0)
                {
                    var numEtiquetaPai = ObterNumEtiqueta(sessao, item.IdProdPedProducaoParent.Value);

                    if (!string.IsNullOrWhiteSpace(numEtiquetaPai) && ProdutoImpressaoDAO.Instance.EstaImpressa(sessao, numEtiquetaPai, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                        throw new Exception("Não é possível voltar o setor de uma peça de composição que esteja associada à uma peça composta impressa.");
                }

                uint idSetor = LeituraProducaoDAO.Instance.ObtemUltimoSetorLido(sessao, idProdPedProducao);
                var setor = Utils.ObtemSetor(idSetor);
                var tipoSetor = setor.Tipo;

                // Exclui último setor lido
                if (idSetor > 1)
                {
                    uint idLeituraProd = ExecuteScalar<uint>(sessao, "select idLeituraProd from leitura_producao where idSetor=" + idSetor +
                        " and idProdPedProducao=" + idProdPedProducao);

                    var leitura = LeituraProducaoDAO.Instance.GetElementByPrimaryKey(sessao, idLeituraProd);
                    item.DataLeituraSetorVoltarPeca = leitura.DataLeitura;
                    item.NomeFuncLeituraSetorVoltarPeca = FuncionarioDAO.Instance.GetNome(sessao, leitura.IdFuncLeitura);

                    objPersistence.ExecuteCommand(sessao, "Delete from leitura_producao Where idLeituraProd=" + leitura.IdLeituraProd);
                }

                // Recupera o idPedidoExpedicao para alterar a situação da produção do pedido de revenda de box, se for o caso
                uint idPedidoExpedicao = ObtemValorCampo<uint>(sessao, "idPedidoExpedicao", "idProdPedProducao=" + idProdPedProducao);

                // Atualiza setor da peça
                uint idSetorNovo = LeituraProducaoDAO.Instance.ObtemUltimoSetorLido(sessao, idProdPedProducao);
                var idCavaleteNovo = LeituraProducaoDAO.Instance.ObtemUltimoCavaleteLido(sessao, idProdPedProducao);
                var removerFornada = setor.Forno && setor.GerenciarFornada;
                objPersistence.ExecuteCommand(sessao, "Update produto_pedido_producao Set idSetor=" + idSetorNovo +
                    ", IdCavalete = " + (idCavaleteNovo.GetValueOrDefault(0) == 0 ? "null" : idCavaleteNovo.Value.ToString()) +
                    (removerFornada ? ", IdFornada = NULL" : "") +
                    (tipoSetor == TipoSetor.Entregue || tipoSetor == TipoSetor.ExpCarregamento ? ", idPedidoExpedicao=null" : "") +
                    " Where idProdPedProducao=" + idProdPedProducao);

                AtualizaSituacaoPecaNaProducao(sessao, idProdPedProducao, null, idCarregamento.GetValueOrDefault() == 0, ref idsPedidoSituacaoProducao);

                var idLojaConsiderar = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0 ?
                    PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido) : UserInfo.GetUserInfo.IdLoja;

                // Altera o estoque (caso seja um pedido de produção e o setor atual marcar entrada no estoque e
                // o anterior não marcar
                if (PedidoDAO.Instance.IsProducao(sessao, idPedido) && setor.EntradaEstoque && !Utils.ObtemSetor(idSetorNovo).EntradaEstoque)
                {
                    string codEtiqueta = ObtemEtiqueta(sessao, idProdPedProducao);
                    ProdutosPedidoEspelho prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null, ObtemIdProdPed(sessao, idProdPedProducao), true);

                    float m2Calc = Glass.Global.CalculosFluxo.ArredondaM2(sessao, prodPedEsp.Largura, (int)prodPedEsp.Altura, 1, 0, prodPedEsp.Redondo);
                    bool m2 = new List<int> { (int)Glass.Data.Model.TipoCalculoGrupoProd.M2, (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto }.Contains(Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)prodPedEsp.IdGrupoProd, (int)prodPedEsp.IdSubgrupoProd));

                    MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaConsiderar, idProdPedProducao, 1, 0, false, false, true);

                    // Só baixa apenas se a peça possuir produto para baixa associado
                    MovEstoqueDAO.Instance.CreditaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaConsiderar, idProdPedProducao, (decimal)(m2Calc > 0 && !PecaPassouSetorLaminado(sessao, codEtiqueta) ? m2Calc : 1), true, true);

                    // Marca que este produto entrou em estoque
                    objPersistence.ExecuteCommand(sessao, "Update produto_pedido_producao Set entrouEstoque=false Where idProdPedProducao=" + idProdPedProducao);
                }

                // Chamado 12925. O erro pode ter ocorrido ao recuperar o código do usuário na classe LogAlteracaoDAO, por isso, criamos uma sobrecarga do
                // método para receber o código do usuário. Caso o erro ocorra novamente iremos desfazer esta alteração e olhar mais a fundo o que pode ser.
                LogAlteracaoDAO.Instance.LogProdPedProducao(sessao, item, LogAlteracaoDAO.SequenciaObjeto.Atual);

                // Se estiver saindo do setor entregue, é necessário estornar o estoque do item.
                if (setor.Tipo == TipoSetor.Entregue || setor.Tipo == TipoSetor.ExpCarregamento)
                {
                    if ((idPedidoExpedicao > 0 && Liberacao.Estoque.SaidaEstoqueBoxLiberar) || trocaDevolucao)
                        return;

                    ProdutosPedidoEspelho prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null, ObtemIdProdPed(sessao, idProdPedProducao), true);

                    // Se o produto der saída ao liberar/confirmar o pedido, não volta para o estoque.
                    //if (!ProdutoSaiuEstoque(idLiberacao.GetValueOrDefault(0), idPedido, prodPedEsp.IdProdPed))
                    //return;

                    float m2Calc = Glass.Global.CalculosFluxo.ArredondaM2(sessao, prodPedEsp.Largura, (int)prodPedEsp.Altura, 1, 0, prodPedEsp.Redondo);
                    bool m2 = new List<int> { (int)Glass.Data.Model.TipoCalculoGrupoProd.M2, (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto }.Contains(Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)prodPedEsp.IdGrupoProd, (int)prodPedEsp.IdSubgrupoProd));

                    MovEstoqueDAO.Instance.CreditaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaConsiderar, idProdPedProducao, (decimal)(m2 ? m2Calc : 1),
                        !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)prodPedEsp.IdGrupoProd, (int)prodPedEsp.IdSubgrupoProd), true);

                    // Estorna saída desta peça no ProdutosPedido
                    var idProdPed = ProdutosPedidoDAO.Instance.ObterIdProdPed(sessao, (int)prodPedEsp.IdProdPed);
                    if (idProdPed.GetValueOrDefault() == 0)
                        throw new Exception(string.Format("Não foi possível recuperar o produto do pedido. Etiqueta: {0}.", ObtemEtiqueta(sessao, idProdPedProducao)));

                    ProdutosPedidoDAO.Instance.EstornoSaida(sessao, (uint)idProdPed, 1, System.Reflection.MethodBase.GetCurrentMethod().Name,ObtemEtiqueta(idProdPedProducao));

                    EstornarSaidaRevenda(sessao, idPedidoExpedicao, prodPedEsp, ObtemEtiqueta(idProdPedProducao));

                    //Se for tranferencia, credita o estoque da loja que esta recebendo a transferencia
                    var idPedTransferencia = idPedidoExpedicao > 0 ? idPedidoExpedicao : idPedido;
                    if (idCarregamento != null && (idCarregamento.GetValueOrDefault(0) > 0 && OrdemCargaDAO.Instance.TemTransferencia(sessao, idCarregamento.Value, idPedTransferencia)))
                    {
                        var idLojaTransferencia = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedTransferencia);

                        MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaTransferencia, idProdPedProducao,
                        (decimal)(m2 ? m2Calc : 1), 0, !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)prodPedEsp.IdGrupoProd, (int)prodPedEsp.IdSubgrupoProd), true, true);
                    }

                    var pedidoReservaLiberacao = idPedidoExpedicao > 0 && !trocaDevolucao ? idPedidoExpedicao : prodPedEsp.IdPedido;

                    // Executa o sql para retirar da liberação/reserva depois que marcar saída nos produtos, para que atualize corretamente a coluna
                    // reserva/liberação
                    if (!PedidoDAO.Instance.IsProducao(sessao, pedidoReservaLiberacao))
                    {
                        if (PedidoConfig.LiberarPedido)
                            ProdutoLojaDAO.Instance.ColocarLiberacao(sessao, (int)PedidoDAO.Instance.ObtemIdLoja(sessao, pedidoReservaLiberacao),
                                new Dictionary<int, float>() { { (int)prodPedEsp.IdProd, m2 ? m2Calc : 1 } }, null, null, null,
                                (int)idProdPedProducao, null, null, null, "ProdutoPedidoProducaoDAO - RetiraPecaSituacao");
                        else
                            ProdutoLojaDAO.Instance.ColocarReserva(sessao, (int)PedidoDAO.Instance.ObtemIdLoja(sessao, pedidoReservaLiberacao),
                                new Dictionary<int, float>() { { (int)prodPedEsp.IdProd, m2 ? m2Calc : 1 } }, null, null, null,
                                (int)idProdPedProducao, null, null, null, "ProdutoPedidoProducaoDAO - RetiraPecaSituacao");
                    }
                }

                //Se o setor for corte ou laminado remove o vinculo da chapa com a peça
                if (setor.Corte || setor.Laminado)
                {
                    var etq = ObtemEtiqueta(sessao, idProdPedProducao);
                    var idProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, etq, ProdutoImpressaoDAO.Instance.ObtemTipoEtiqueta(etq));
                    var idProdImpressaoChapa = ChapaCortePecaDAO.Instance.ObtemValorCampo<uint>(sessao, "IdProdImpressaoChapa", string.Format("IdProdImpressaoPeca={0}", idProdImpressao));
                    var quantidadeLeiturasChapa = ChapaCortePecaDAO.Instance.QtdeLeiturasChapa(sessao, idProdImpressao);
                    var idProd = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, idProdImpressaoChapa);

                    //Se não houver mais leituras para chapa volta o estoque da mesma
                    if (quantidadeLeiturasChapa == 1)
                    {
                        uint? idNf = ProdutoImpressaoDAO.Instance.ObtemIdNf(sessao, idProdImpressaoChapa);
                        uint? idLojaMovEstoque = (uint?)objPersistence.ExecuteScalar(sessao,
                            string.Format("SELECT idLoja FROM mov_estoque WHERE idNf={0} AND idProd={1} AND tipoMov={2} order by idmovestoque desc limit 1",
                                                idNf.GetValueOrDefault(0), idProd.GetValueOrDefault(), (int)MovEstoque.TipoMovEnum.Entrada));

                        var idLojaNf = NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNf.GetValueOrDefault());

                        var idLojaMovChapa = idLojaMovEstoque ?? (idLojaNf == 0 ? idLojaConsiderar : idLojaNf);

                        if (idProd > 0)
                            MovEstoqueDAO.Instance.CreditaEstoqueProducao(sessao, idProd.Value, idLojaMovChapa, idProdPedProducao, 1, false, false);
                    }
                    #region Ajusta o estoque e a Reserva da chapa no pedido de revenda

                    if (setor.Corte)
                    {
                        var idPedidoRevenda = PedidoDAO.Instance.ObterIdPedidoRevenda(sessao, (int)idPedido);

                        /* Chamado 54054.
                         * Caso a chapa esteja sendo desassociada da sua última etiqueta de produção, estorna a quantidade de saída do produto e soma a quantidade em Reserva. */
                        if (idPedidoRevenda > 0 && ChapaCortePecaDAO.Instance.QtdeLeituraChapaPedidoRevenda(sessao, idProdImpressaoChapa, (uint)idPedidoRevenda.Value) == 1)
                        {
                            #region Ajusta estoque do pedido de revenda que gerou o pedido produção de corte

                            var idProdChapa = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, idProdImpressaoChapa);
                            var numEtiquetaChapa = ProdutoImpressaoDAO.Instance.ObtemValorCampo<string>(sessao, "numEtiqueta", $"IdProdImpressao={idProdImpressaoChapa}");
                            var produtoPedidoRevenda = ProdutosPedidoDAO.Instance.GetByPedido(sessao, (uint)idPedidoRevenda)
                                .FirstOrDefault(f => f.IdProd == idProd.GetValueOrDefault() && f.Qtde > f.QtdSaida);
                            var idLojaPedidoRevenda = (int)PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedidoRevenda);
                            var idProdQtdeReserva = new Dictionary<int, float>();

                            if (produtoPedidoRevenda.IdProdPed == 0)
                                throw new Exception("Não foi possível baixar o estoque da chapa no pedido de revenda.");

                            // Atualiza o Qtd Saída dos produtos do pedido de revenda.
                            ProdutosPedidoDAO.Instance.MarcarSaida(sessao, produtoPedidoRevenda.IdProdPed, -1, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, numEtiquetaChapa);

                            // Verifica o tipo de cálculo do produto.
                            var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedidoRevenda.IdProd);

                            // Verifica o tipo de cálculo do produto.
                            var m2Calc = Global.CalculosFluxo.ArredondaM2(sessao, produtoPedidoRevenda.Largura, (int)produtoPedidoRevenda.Altura, produtoPedidoRevenda.Qtde, 0,
                                produtoPedidoRevenda.Redondo, 0, true);

                            // Ajusta o campo RESERVA do produto loja.
                            ProdutoLojaDAO.Instance.ColocarReserva(sessao, idLojaPedidoRevenda, new Dictionary<int, float>() { { (int)produtoPedidoRevenda.IdProd, 1F } }, null, null, null, null,
                                (int)idPedidoRevenda, null, null, "ProdutoPedidoProducaoDAO - AtualizaSituacao");

                            #endregion
                        }
                    }

                    #endregion

                    ChapaCortePecaDAO.Instance.DeleteByIdProdImpressaoPeca(sessao, idProdImpressao, idProdPedProducao);

                    //Marca a chapa novamente como disponivel
                    ChapaTrocadaDevolvidaDAO.Instance.MarcarChapaComoDisponivel(sessao, ProdutoImpressaoDAO.Instance.ObtemNumEtiqueta(idProdImpressaoChapa));

                    //Se a peça possui retalho e o mesmo não tiver sido associado na impressao da peça remove a associação.
                    var usoRetalho = UsoRetalhoProducaoDAO.Instance.ObtemAssociacao(sessao, idProdPedProducao);
                    if (usoRetalho != null && !usoRetalho.VinculadoImpressao)
                        UsoRetalhoProducaoDAO.Instance.RemoverAssociacao(sessao, usoRetalho.IdRetalhoProducao, usoRetalho.IdProdPedProducao);
                }

                PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, idPedido, null, DateTime.Now);

                if (idPedidoExpedicao > 0 && !trocaDevolucao)
                    PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, idPedidoExpedicao, null, DateTime.Now);
            }
        }

        /// <summary>
        /// Estorna o QtdSaida do produto_pedido do pedido de revenda
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedidoNovo"></param>
        /// <param name="prodPedEtiqueta"></param>
        private void EstornarSaidaRevenda(GDASession sessao, uint idPedidoNovo, ProdutosPedidoEspelho prodPedEtiqueta, string numEtiqueta)
        {
            if (idPedidoNovo == 0)
                return;

            // Carrega os produtos
            var prodPed = ProdutosPedidoDAO.Instance.GetByPedido(sessao, idPedidoNovo);

            // Soma a quantidade total do idProd passado nos produtos do pedido de revenda
            var qtdTotalProd = prodPed
                .Where(f => f.IdProd == prodPedEtiqueta.IdProd)
                .Sum(f => f.Qtde);

            // Procura o produto no pedido de revenda
            var produtoRevenda = prodPed.Where(f =>
                f.IdProd == prodPedEtiqueta.IdProd &&
                f.Altura == prodPedEtiqueta.Altura &&
                f.Largura == prodPedEtiqueta.Largura &&
                f.QtdSaida > 0);

            if (produtoRevenda.Count() > 0)
                ProdutosPedidoDAO.Instance.EstornoSaida(sessao, produtoRevenda.FirstOrDefault().IdProdPed, 1, System.Reflection.MethodBase.GetCurrentMethod().Name, numEtiqueta);
        }

        /// <summary>
        /// Volta uma peça reposta à situação anterior à reposição.
        /// </summary>
        public void VoltarPecaReposta(GDASession sessao, uint idProdPedProducao)
        {
            string dadosReposicaoPeca = GetDadosReposicaoPeca(sessao, idProdPedProducao);

            if (!IsPecaReposta(sessao, idProdPedProducao, false) || String.IsNullOrEmpty(dadosReposicaoPeca))
                return;

            ChapaCortePecaDAO.Instance.VoltarPecaRepostaChapaCortePeca(sessao, idProdPedProducao);

            var etiqueta = ObtemEtiqueta(sessao, idProdPedProducao);
            var item = GetElementByPrimaryKey(sessao, idProdPedProducao);

            // Recupera os dados da reposição
            List<string> dadosReposicao = new List<string>(dadosReposicaoPeca.Split('~'));
            if (dadosReposicao.Count <= 6 || dadosReposicao[6].Contains("!"))
                dadosReposicao.Insert(3, String.Empty);

            // Volta o produto à situação anterior
            objPersistence.ExecuteCommand(sessao, @"update produto_pedido_producao set idPedidoExpedicao=?idPedExp, pecaReposta=false,
                tipoPerdaRepos=null, idSubtipoPerdaRepos=null, obs=?obs, dataRepos=null, situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @",
                idSetor=?idSetor, idSetorRepos=null, idFuncRepos=null, dadosReposicaoPeca=null where idProdPedProducao=" + idProdPedProducao,
                new GDAParameter("?idPedExp", Glass.Conversoes.StrParaUintNullable(dadosReposicao[0])), new GDAParameter("?obs", dadosReposicao[2]),
                new GDAParameter("?idSetor", Glass.Conversoes.StrParaUint(dadosReposicao[1])));

            // Restaura a reposição anterior, se houver
            DadosReposicao anterior = DadosReposicaoDAO.Instance.Desempilha(sessao, idProdPedProducao);

            if (anterior != null)
            {
                // Volta os dados da reposição anterior ao produto
                objPersistence.ExecuteCommand(sessao, @"update produto_pedido_producao set pecaReposta=true, obs=?o,
                    tipoPerdaRepos=?tp, idSubtipoPerdaRepos=?sp, dataRepos=?d, idSetorRepos=?s, idFuncRepos=?f, 
                    " + (anterior.SituacaoProducao > 0 ? "situacaoProducao=?spr, " : String.Empty) +
                    "dadosReposicaoPeca=?dados where idProdPedProducao=" + idProdPedProducao,
                    new GDAParameter("?o", anterior.Obs), new GDAParameter("?tp", anterior.TipoPerdaRepos),
                    new GDAParameter("?sp", anterior.IdSubtipoPerdaRepos), new GDAParameter("?d", anterior.DataRepos),
                    new GDAParameter("?s", anterior.IdSetorRepos), new GDAParameter("?f", anterior.IdFuncRepos),
                    new GDAParameter("?spr", anterior.SituacaoProducao), new GDAParameter("?dados", anterior.DadosReposicaoPeca));
            }

            // Atualiza a tabela produto_impressao
            if (!String.IsNullOrEmpty(dadosReposicao[4]) || !String.IsNullOrEmpty(dadosReposicao[5]))
            {
                ProdutoImpressaoDAO.Instance.InsertOrUpdatePeca(sessao, etiqueta, dadosReposicao[5],
                    Glass.Conversoes.StrParaInt(dadosReposicao[6]), 0, ProdutoImpressaoDAO.TipoEtiqueta.Pedido, null);

                uint idProdImpressao = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, etiqueta,
                    ProdutoImpressaoDAO.TipoEtiqueta.Pedido);

                ProdutoImpressaoDAO.Instance.ExecuteScalar<int>(sessao, "update produto_impressao set idImpressao=" +
                    Glass.Conversoes.StrParaUintNullable(dadosReposicao[4]) + " where idProdImpressao=" + idProdImpressao);
            }

            // Apaga as leituras dessa peça
            objPersistence.ExecuteCommand(sessao, "Delete From leitura_producao Where idProdPedProducao=" + idProdPedProducao);

            var idsSetores = new List<uint>();

            // Restaura as leituras de produção
            for (int i = 7; i < dadosReposicao.Count; i++)
            {
                string[] leitura = dadosReposicao[i].Split('!');

                LeituraProducao lp = new LeituraProducao();
                lp.IdProdPedProducao = idProdPedProducao;
                lp.IdFuncLeitura = Glass.Conversoes.StrParaUint(leitura[0]);
                lp.IdSetor = Glass.Conversoes.StrParaUint(leitura[1]);
                lp.DataLeitura = Conversoes.ConverteData(leitura[2]);

                LeituraProducaoDAO.Instance.Insert(sessao, lp);

                idsSetores.Add(lp.IdSetor);
            }

            var temEntradaEstoque = false;

            foreach (var idSetor in idsSetores)
            {
                if (SetorDAO.Instance.IsEntradaEstoque(sessao, idSetor))
                {
                    temEntradaEstoque = true;
                    break;
                }
            }

            var idPedido = Glass.Conversoes.StrParaUint(etiqueta.Split('-')[0]);

            // Se for pedido de produção e o setor estiver marcado para dar entrada de estoque
            if (PedidoDAO.Instance.IsProducao(sessao, idPedido) && temEntradaEstoque)
            {
                var idLojaConsiderar = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0 ?
                    PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido) : UserInfo.GetUserInfo.IdLoja;

                // Busca o produto ao qual se refere a etiqueta
                var prodPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null, ObtemIdProdPed(sessao, idProdPedProducao), true);

                float m2Calc = Glass.Global.CalculosFluxo.ArredondaM2(sessao, prodPedEsp.Largura, (int)prodPedEsp.Altura, 1, 0, prodPedEsp.Redondo);
                bool m2 = new List<int>
                        {
                            (int)Glass.Data.Model.TipoCalculoGrupoProd.M2,
                            (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto
                        }
                .Contains(Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)prodPedEsp.IdGrupoProd, (int)prodPedEsp.IdSubgrupoProd));

                var areaMinimaProd = ProdutoDAO.Instance.ObtemAreaMinima(sessao, (int)prodPedEsp.IdProd);
                var idCliente = PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido);
                var m2CalcAreaMinima = Glass.Global.CalculosFluxo.CalcM2Calculo(sessao, idCliente, (int)prodPedEsp.Altura, prodPedEsp.Largura, 1,
                    (int)prodPedEsp.IdProd, prodPedEsp.Redondo, prodPedEsp.Beneficiamentos.CountAreaMinimaSession(sessao), areaMinimaProd, false, prodPedEsp.Espessura, true);

                MovEstoqueDAO.Instance.CreditaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaConsiderar, idProdPedProducao, 1, false, true);

                // Só baixa apenas se a peça possuir produto para baixa associado
                MovEstoqueDAO.Instance.BaixaEstoqueProducao(sessao, prodPedEsp.IdProd, idLojaConsiderar, idProdPedProducao,
                    (decimal)(m2Calc > 0 ? m2Calc : 1), (decimal)(m2 ? m2CalcAreaMinima : 0), true, false, true);

                // Marca que este produto entrou em estoque
                objPersistence.ExecuteCommand(sessao, "Update produto_pedido_producao Set entrouEstoque=true Where idProdPedProducao=" + idProdPedProducao);
            }

            AtualizaSituacaoPecaNaProducao(sessao, idProdPedProducao, null, true);
            LogAlteracaoDAO.Instance.LogProdPedProducao(sessao, item, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Retorna etiquetas a partir do produto pedido produção

        /// <summary>
        /// Retorna etiquetas a partir do produto pedido produção
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public string GetEtiquetasByIdProdPedProducao(uint idProdPedProducao)
        {
            string sql = "Select numEtiqueta From produto_pedido_producao Where idProdPedProducao=" + idProdPedProducao;

            object obj = objPersistence.ExecuteScalar(sql);

            return obj != null ? obj.ToString() : String.Empty;
        }

        #endregion

        #region Retorna cavaletes a partir do produto pedido espelho

        /// <summary>
        /// Retorna cavaletes a partir do produto pedido espelho
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string GetCavaletesByIdProdPed(uint idProdPed)
        {
            var sql = @"SELECT CodInterno
                        FROM produto_pedido_producao ppp
                            INNER JOIN cavalete c ON (ppp.IdCavalete = c.IdCavalete)
                        WHERE IdProdPed = " + idProdPed;

            return GetValoresCampo(sql, "CodInterno", ", ");
        }

        #endregion

        #region Retorna etiquetas a partir do produto pedido espelho

        /// <summary>
        /// Retorna etiquetas a partir do produto pedido espelho
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public string GetEtiquetasByIdProdPed(uint idProdPed)
        {
            return GetValoresCampo("select numEtiqueta from produto_pedido_producao where idProdPed=" + idProdPed,
                "numEtiqueta").Replace(",", ", ");
        }

        #endregion

        #region Retorna os ID's dos produtos de produção a partir do produto pedido espelho

        /// <summary>
        /// Retorna os ID's dos produtos de produção a partir do produto pedido espelho.
        /// </summary>
        public List<int> ObterIdsProdutoPedidoProducaoPeloIdProdPedEsp(GDASession session, int idProdPed)
        {
            var idsProdPedProducao = ExecuteMultipleScalar<int>(session, string.Format("SELECT IdProdPedProducao FROM produto_pedido_producao WHERE IdProdPed={0}", idProdPed));

            return idsProdPedProducao != null && idsProdPedProducao.Count > 0 ? idsProdPedProducao.Where(f => f > 0).ToList() : new List<int>();
        }

        #endregion

        #region Associa o produto de produção à etiqueta importada

        /// <summary>
        /// Associa o produto de produção à etiqueta importada.
        /// </summary>
        public void AssociarProdutoPedidoProducaoEtiquetaImportada(GDASession session, int idProdPedProducao, string numEtiquetaImportada)
        {
            objPersistence.ExecuteCommand(session, string.Format("UPDATE produto_pedido_producao SET NumEtiquetaCliente=?numEtiquetaCliente WHERE IdProdPedProducao={0}", idProdPedProducao),
                new GDAParameter("?numEtiquetaCliente", numEtiquetaImportada));
        }

        #endregion

        #region Retorna etiquetas a partir do produto pedido espelho de uma liberação

        /// <summary>
        /// Retorna etiquetas a partir do produto pedido espelho de uma liberação
        /// </summary>
        public string GetEtiquetasByIdProdPedLiberacao(uint idProdPed, uint idLiberarPedido)
        {
            string sql = "Select Cast(group_concat(numEtiqueta separator ', ') as char) From produto_pedido_producao Where idProdPed=" + idProdPed +
                " and idProdPedProducao in (select idProdPedProducao from produtos_liberar_pedido where idLiberarPedido=" + idLiberarPedido + ")";

            object obj = objPersistence.ExecuteScalar(sql);

            return obj != null ? obj.ToString() : String.Empty;
        }

        #endregion

        #region Verifica se a peça da etiqueta passada já está em produção

        /// <summary>
        /// Verifica se a peça já foi inserida na produção ou se existe
        /// </summary>
        public bool PecaEstaEmProducao(string codEtiqueta)
        {
            return PecaEstaEmProducao(null, codEtiqueta);
        }

        /// <summary>
        /// Verifica se a peça já foi inserida na produção ou se existe
        /// </summary>
        public bool PecaEstaEmProducao(GDASession session, string codEtiqueta)
        {
            var id = ObtemIdProdPedProducao(session, codEtiqueta);
            return id > 0;
        }

        /// <summary>
        /// Verifica se a peça está com a impressão cancelada.
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public bool PecaEstaCancelada(GDASession session, uint idProdPedProducao)
        {
            string etiqueta = ObtemValorCampo<string>(session, "coalesce(numEtiqueta, numEtiquetaCanc)",
                "idProdPedProducao=" + idProdPedProducao);

            return PecaEstaCancelada(session, etiqueta);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se a peça está com a impressão cancelada.
        /// </summary>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public bool PecaEstaCancelada(string numEtiqueta)
        {
            return PecaEstaCancelada(null, numEtiqueta);
        }

        /// <summary>
        /// Verifica se a peça está com a impressão cancelada.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public bool PecaEstaCancelada(GDASession sessao, string numEtiqueta)
        {
            return PecaEstaCancelada(sessao, numEtiqueta, false);
        }

        /// <summary>
        /// Verifica se a peça está com a impressão cancelada.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codEtiqueta"></param>
        /// <param name="reposicaoPeca">Na tela de reposição de peça as peças canceladas na tabela produto_impressao não podem ser repostas.</param>
        /// <returns></returns>
        public bool PecaEstaCancelada(GDASession sessao, string numEtiqueta, bool reposicaoPeca)
        {
            var situacao = ExecuteMultipleScalar<uint>(sessao,
                @"SELECT * FROM
                    (SELECT Situacao FROM produto_pedido_producao
                    WHERE (NumEtiqueta IS NOT NULL AND NumEtiqueta=?numEtiqueta) OR
                        (NumEtiquetaCanc IS NOT NULL AND NumEtiquetaCanc=?numEtiqueta))
                AS temp",
                new GDAParameter("?numEtiqueta", numEtiqueta));

            /* Chamado 16002.
             * O usuário marcou como reposta uma peça cancelada, ao cancelar uma peça existe uma verificação no método de cancelamento que não marca a
             * etiqueta como cancelada na produção, marca somente o produto de impressão. Por isso, inserimos esta verificação, para que, caso a peça esteja
             * cancelada na impressão, o usuário não consiga marcá-la como reposta, causando problemas no arquivo de otimização, por exemplo. */
            if (reposicaoPeca &&

                ExecuteScalar<bool>(sessao,
                    @"SELECT COUNT(*)>0 FROM produto_impressao pi
                    WHERE pi.NumEtiqueta=?numEtiqueta AND
                        (pi.Cancelado IS NOT NULL AND pi.Cancelado=1)",
                    new GDAParameter("?numEtiqueta", numEtiqueta)) &&

                // Chamado 16114 e 16112: Mesmo que a etiqueta tenha alguma peça cancelada, é possível que ela já tenha sido reimpressa, por isso é necessário
                // ter certeza que não existe nenhuma etiqueta impressa, mesmo que exista alguma cancelada
                ExecuteScalar<bool>(sessao,
                    @"SELECT COUNT(*)=0 FROM produto_impressao pi
                    WHERE pi.NumEtiqueta=?numEtiqueta AND
                        (pi.Cancelado IS NULL OR pi.Cancelado=0)",
                    new GDAParameter("?numEtiqueta", numEtiqueta))

                )
                return true;

            if (situacao.Contains((int)ProdutoPedidoProducao.SituacaoEnum.Producao))
                return false;
            else if (situacao.Contains((int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda) ||
                situacao.Contains((int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra))
                return true;

            return false;
        }

        #endregion

        #region Relatório de perdas

        private string SqlPerda(uint idFuncPerda, uint idPedido, uint idCliente, string nomeCliente, string dataIni,
            string dataFim, bool selecionar, string idsSetor, string idsDepartamento)
        {
            return SqlPerda(null, idFuncPerda, idPedido, idCliente, nomeCliente, dataIni, dataFim, selecionar, idsSetor, idsDepartamento);
        }

        private string SqlPerda(GDASession session, uint idFuncPerda, uint idPedido, uint idCliente, string nomeCliente, string dataIni, string dataFim,
            bool selecionar, string idsSetor, string idsDepartamento)
        {
            string criterio = "";
            string sql;
            string campos = selecionar ? @"ppp.*, pp.idPedido, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.altura, 
                if(pp.alturaReal > 0, pp.alturaReal, pp.altura)) as Altura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.largura, if(pp.Redondo, 0, if(pp.larguraReal > 0, pp.larguraReal, pp.largura))) as Largura, 
                if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.Ambiente, concat(p.Descricao, 
                if(pp.Redondo and " + (!BenefConfigDAO.Instance.CobrarRedondo()).ToString() + @", ' REDONDO', ''))) as DescrProduto, p.CodInterno, 
                p.custoCompra as valorCustoUnitario, ped.dataEntrega, ped.dataEntregaOriginal, cli.id_cli as IdCliente, cli.nome as nomeCliente, 
                apl.CodInterno as CodAplicacao, prc.CodInterno as CodProcesso, p.espessura, concat(cast(ped.IdPedido as char), 
                if(ped.IdPedidoAnterior is not null, concat(' (', concat(cast(ped.IdPedidoAnterior as char), 'R)')), ''), 
                if(ppp.idPedidoExpedicao is not null, concat(' (Exp. ', cast(ppp.idPedidoExpedicao as char), ')'), '')) as IdPedidoExibir, 
                s.descricao as descrSetor, d.descricao as DescrDepart, pp.ValorVendido as ValorUnit, ped.CodCliente, 
                Round(pp.TotM/(pp.Qtde*if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as TotM2, 
                (ped.situacao=" + (int)Pedido.SituacaoPedido.Cancelado + @") as PedidoCancelado, 
                ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @" as PedidoMaoObra, f.nome as nomeFuncPerda, 
                lp.dataLiberacao as DataLiberacaoPedido, ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @" as PedidoProducao, 
                '$$$' as Criterio" : "count(*)";

            /* Chamado 22928. */
            if (!selecionar &&
                idFuncPerda == 0 &&
                idCliente == 0 &&
                string.IsNullOrEmpty(nomeCliente) &&
                string.IsNullOrEmpty(idsSetor) &&
                string.IsNullOrEmpty(idsDepartamento))
                sql = @"
                    select " + campos + @"
                    from produto_pedido_producao ppp
                        INNER JOIN produtos_pedido_espelho pp ON(ppp.idProdPed = pp.idProdPed)
                        INNER JOIN pedido ped ON(pp.idPedido = ped.idPedido)
                    where ppp.dataPerda is not null and ppp.situacao<>" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;
            else
                sql = @"
                    select " + campos + @"
                    from produto_pedido_producao ppp
                        Left Join setor s On (ppp.idSetor=s.idSetor)
                        Inner Join produtos_pedido_espelho pp On (ppp.idProdPed=pp.idProdPed) 
                        Left Join ambiente_pedido_espelho a On (pp.idAmbientePedido=a.idAmbientePedido) 
                        Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                        Left Join liberarpedido lp On (lp.idLiberarPedido=ped.idLiberarPedido) 
                        Inner Join cliente cli On (ped.idCli=cli.id_Cli) 
                        Inner Join produto p On (pp.idProd=p.idProd)
                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                        Inner Join funcionario f On (ppp.idFuncPerda=f.idFunc)
                        Left Join func_departamento fd On (ppp.idFuncPerda=fd.idFunc) 
                        Left Join departamento d On (fd.idDepartamento=d.idDepartamento) 
                    where ppp.dataPerda is not null and ppp.situacao<>" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

            if (idFuncPerda > 0)
            {
                sql += " and ppp.idFuncPerda=" + idFuncPerda;
                criterio += "Funcionário Perda: " + FuncionarioDAO.Instance.GetNome(session, idFuncPerda) + "    ";
            }

            if (idPedido > 0)
            {
                sql += " And (ped.idPedido=" + idPedido;

                // Na vidrália não tem como filtrar pelo ped.idPedidoAnterior sem dar timeout, para utilizar o filtro desta maneira
                // teria que mudar totalmente a forma de fazer o count
                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido &&
                    PedidoDAO.Instance.IsPedidoReposto(session, idPedido))
                    sql += " Or ped.IdPedidoAnterior=" + idPedido;

                if (PedidoDAO.Instance.IsPedidoExpedicaoBox(session, idPedido))
                    sql += " Or ppp.idPedidoExpedicao=" + idPedido;

                sql += ")";

                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idCliente > 0)
            {
                sql += " and cli.id_Cli=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(session, idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(session, null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And cli.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and ppp.dataPerda>=?dataIni";
                criterio += "Data início Perda: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and ppp.dataPerda<=?dataFim";
                criterio += "Data fim Perda: " + dataFim + "    ";

            }

            if (!String.IsNullOrEmpty(idsSetor))
            {
                sql += " and ppp.idSetor in (" + idsSetor + ")";
            }

            if (!String.IsNullOrEmpty(idsDepartamento))
            {
                sql += " and fd.idDepartamento in (" + idsDepartamento + ")";
            }

            sql = sql.Replace("$$$", criterio);
            return sql;
        }

        public IList<ProdutoPedidoProducao> GetListPerda(uint idFuncPerda, uint idPedido, uint idCliente, string nomeCliente,
            string dataIni, string dataFim, string idsSetor, string idsDepartamento, string sortExpression, int startRow, int pageSize)
        {
            var sort = String.IsNullOrEmpty(sortExpression) ? "ppp.idProdPedProducao Desc " : sortExpression;
            return LoadDataWithSortExpression(SqlPerda(idFuncPerda, idPedido, idCliente, nomeCliente, dataIni, dataFim, true, idsSetor, idsDepartamento), sort, startRow,
                pageSize, GetParam(null, null, null, dataIni, dataFim, null, null, null, null, nomeCliente, null, null, null, 0));
        }

        public int GetCountPerda(uint idFuncPerda, uint idPedido, uint idCliente, string nomeCliente, string dataIni,
            string dataFim, string idsSetor, string idsDepartamento)
        {
            return GetCountPerda(null, idFuncPerda, idPedido, idCliente, nomeCliente, dataIni, dataFim, idsSetor, idsDepartamento);
        }

        public int GetCountPerda(GDASession session, uint idFuncPerda, uint idPedido, uint idCliente, string nomeCliente, string dataIni, string dataFim, string idsSetor, string idsDepartamento)
        {
            return objPersistence.ExecuteSqlQueryCount(session, SqlPerda(session, idFuncPerda, idPedido, idCliente, nomeCliente, dataIni, dataFim, false, idsSetor, idsDepartamento),
                GetParam(null, null, null, dataIni, dataFim, null, null, null, null, nomeCliente, null, null, null, 0));
        }

        public IList<ProdutoPedidoProducao> GetForRptPerda(uint idFuncPerda, uint idPedido, uint idCliente, string nomeCliente, string dataIni, string dataFim, string idsSetor)
        {
            var lst = objPersistence.LoadData(SqlPerda(idFuncPerda, idPedido, idCliente, nomeCliente, dataIni, dataFim, true, idsSetor, null),
                GetParam(null, null, null, dataIni, dataFim, null, null, null, null, nomeCliente, null, null, null, 0)).ToList();

            return lst;
        }

        #endregion

        #region Relatório de perdas (reposição de peças)

        private string SqlPerdaReposPeca(uint idFuncPerda, uint idPedido, uint idCliente, string nomeCliente, string codInterno,
            string descrProd, string dataIni, string dataFim, string idSetor, uint idTurno, string idTipoPerda, bool selecionar,
            int idCorVidro, float espessuraVidro, uint numeroNFe)
        {
            return SqlPerdaReposPeca(idFuncPerda, idPedido, idCliente, nomeCliente, codInterno, descrProd, dataIni, dataFim, idSetor,
                idTurno, idTipoPerda, selecionar, idCorVidro, espessuraVidro, numeroNFe);
        }

        private string SqlPerdaReposPeca(uint idFuncPerda, uint idPedido, uint idLoja, uint idCliente, string nomeCliente,
            string codInterno, string descrProd, string dataIni, string dataFim, string idSetor, uint idTurno, string idTipoPerda,
            bool selecionar, int idCorVidro, float espessuraVidro, uint numeroNFe)
        {
            string criterio = "";
            string campos1 = selecionar ? @"
                ppp.idProdPedProducao, null as numeroNFe, ppp.idProdPed, ppp.idSetor, ppp.idFuncPerda, ppp.idPedidoExpedicao, 
                ppp.idFuncRepos, ppp.idSetorRepos, ppp.situacao, ppp.dataPerda, ppp.numEtiqueta, ppp.planoCorte, ppp.tipoPerda, ppp.idSubtipoPerda, 
                ppp.obs, ppp.entrouEstoque, ppp.pecaReposta, ppp.tipoPerdaRepos, ppp.idSubtipoPerdaRepos, ppp.dataRepos, 
                ppp.numEtiquetaCanc, ppp.idImpressao, ppp.canceladoAdmin, ppp.dadosReposicaoPeca, pp.idPedido, 
                if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.altura, if(pp.alturaReal > 0, pp.alturaReal, pp.altura)) as Altura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.largura, if(pp.Redondo, 0, if (pp.larguraReal > 0, pp.larguraReal, pp.largura))) as Largura, cv.descricao As Cor, p.Espessura,
                if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.Ambiente, concat(p.Descricao, 
                if(pp.Redondo and " + (!BenefConfigDAO.Instance.CobrarRedondo()).ToString() + @", ' REDONDO', ''))) as DescrProduto, p.CodInterno, 
                ped.dataEntrega, ped.dataEntregaOriginal, cli.id_cli as IdCliente, cli.nome as nomeCliente, apl.CodInterno as CodAplicacao, 
                prc.CodInterno as CodProcesso, concat(cast(ped.IdPedido as char), if(ped.IdPedidoAnterior is not null, concat(' (', concat(cast(ped.IdPedidoAnterior as char), 
                'R)')), ''), if(ppp.idPedidoExpedicao is not null, concat(' (Exp. ', cast(ppp.idPedidoExpedicao as char), ')'), '')) as IdPedidoExibir, 
                s.descricao as descrSetor, sr.descricao as descrSetorRepos, p.CustoCompra AS ValorCustoUnitario, pp.ValorVendido as ValorUnit, ped.CodCliente, round(pp.TotM/(pp.qtde*if(ped.tipoPedido=" +
                (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as TotM2, (ped.situacao=" + (int)Pedido.SituacaoPedido.Cancelado + @") as PedidoCancelado, 
                (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @") as PedidoMaoObra, f.nome as nomeFuncPerda, lp.dataLiberacao as DataLiberacaoPedido, 
                (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + ") as PedidoProducao, 1 as qtde, '$$$' as Criterio" : "count(*)";

            string campos2 = selecionar ? @"
                ppp.idProdPedProducao, null as numeroNFe, ppp.idProdPed, ppp.idSetor, ppp.idFuncPerda, ppp.idPedidoExpedicao, 
                dr.idFuncRepos, dr.idSetorRepos, ppp.situacao, ppp.dataPerda, ppp.numEtiqueta, ppp.planoCorte, ppp.tipoPerda, ppp.idSubtipoPerda, 
                dr.obs, ppp.entrouEstoque, ppp.pecaReposta, dr.tipoPerdaRepos, dr.idSubtipoPerdaRepos, dr.dataRepos, 
                ppp.numEtiquetaCanc, ppp.idImpressao, ppp.canceladoAdmin, dr.dadosReposicaoPeca, pp.idPedido, 
                if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.altura, if(pp.alturaReal > 0, pp.alturaReal, pp.altura)) as Altura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.largura, if(pp.Redondo, 0, if (pp.larguraReal > 0, pp.larguraReal, pp.largura))) as Largura, cv.descricao As Cor, p.Espessura,
                if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.Ambiente, concat(p.Descricao, 
                if(pp.Redondo and " + (!BenefConfigDAO.Instance.CobrarRedondo()).ToString() + @", ' REDONDO', ''))) as DescrProduto, p.CodInterno, 
                ped.dataEntrega, ped.dataEntregaOriginal, cli.id_cli as IdCliente, cli.nome as nomeCliente, apl.CodInterno as CodAplicacao, 
                prc.CodInterno as CodProcesso, concat(cast(ped.IdPedido as char), if(ped.IdPedidoAnterior is not null, concat(' (', concat(cast(ped.IdPedidoAnterior as char), 
                'R)')), ''), if(ppp.idPedidoExpedicao is not null, concat(' (Exp. ', cast(ppp.idPedidoExpedicao as char), ')'), '')) as IdPedidoExibir, 
                s.descricao as descrSetor, sr.descricao as descrSetorRepos, p.CustoCompra AS ValorCustoUnitario, pp.ValorVendido as ValorUnit, ped.CodCliente, round(pp.TotM/(pp.qtde*if(ped.tipoPedido=" +
                (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1)), 4) as TotM2, (ped.situacao=" + (int)Pedido.SituacaoPedido.Cancelado + @") as PedidoCancelado, 
                (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @") as PedidoMaoObra, f.nome as nomeFuncPerda, lp.dataLiberacao as DataLiberacaoPedido, 
                (ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + ") as PedidoProducao, 1 as qtde, '$$$' as Criterio" : "count(*)";

            string campos3 = selecionar ? @"
                null, null, pt.idProdPed, null, td.idFunc, null, 
                td.idFunc, null, " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @", td.dataTroca, null, null, td.idTipoPerda, td.idSubtipoPerda, 
                td.descricao, null, true, td.idTipoPerda, td.idSubtipoPerda, td.dataTroca,  
                null, null, null, null, td.idPedido, 
                if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.altura, if(pt.alturaReal > 0, pt.alturaReal, pt.altura)) as Altura, if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", 
                a.largura, if(pt.Redondo, 0, pt.largura)) as Largura, cv.descricao As Cor, p.Espessura,
                if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.Ambiente, concat(p.Descricao, 
                if(pt.Redondo and " + (!BenefConfigDAO.Instance.CobrarRedondo()).ToString() + @", ' REDONDO', ''))) as DescrProduto, p.CodInterno, 
                ped.dataEntrega, ped.dataEntregaOriginal, cli.id_cli as IdCliente, cli.nome as nomeCliente, apl.CodInterno as CodAplicacao, 
                prc.CodInterno as CodProcesso, concat(cast(ped.IdPedido as char), 
                if(ped.IdPedidoAnterior is not null, concat(' (', concat(cast(ped.IdPedidoAnterior as char), 
                'R)')), '')) as IdPedidoExibir, if(td.tipo=" + (int)TrocaDevolucao.TipoTrocaDev.Troca + @", 'Troca', 'Devolução') as descrSetor, 
                if(td.tipo=" + (int)TrocaDevolucao.TipoTrocaDev.Troca + @", 'Troca', 'Devolução') as descrSetorRepos, pt.CustoProd AS ValorCustoUnitario, pt.ValorVendido as ValorUnit, 
                ped.CodCliente, pt.TotM as TotM2, (ped.situacao= " + (int)Pedido.SituacaoPedido.Cancelado + @") as PedidoCancelado, ped.tipoPedido=" +
                (int)Pedido.TipoPedidoEnum.MaoDeObra + @" as PedidoMaoObra, f.nome as nomeFuncPerda, lp.dataLiberacao as DataLiberacaoPedido, 
                ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @" as PedidoProducao, cast(pt.qtde as decimal(12,2)) as qtde, '$$$' as Criterio" : "count(*)";

            string campos4 = selecionar ? @"
                null, nf.numeroNFe, null, null, pcv.idFuncPerda, null, 
                null, null, " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @", pcv.dataPerda, pi.numEtiqueta, null, pcv.idTipoPerda, pcv.idSubtipoPerda, 
                pcv.obs, null, null, pcv.idTipoPerda, pcv.idSubtipoPerda, pcv.dataPerda,  
                null, pi.idImpressao, false, null, null, 
                pnf.altura, pnf.largura, cv.descricao as Cor, p.Espessura, p.Descricao as DescrProduto, p.CodInterno, 
                null, null, null as IdCliente, null as nomeCliente, null as CodAplicacao, 
                null as CodProcesso, null as IdPedidoExibir, 'Chapa de Vidro' as descrSetor, 'Chapa de Vidro' as descrSetorRepos, p.CustoCompra AS ValorCustoUnitario, pnf.ValorUnitario as ValorUnit, 
                null, round(pnf.TotM / pnf.qtde, 4) as TotM2, (nf.situacao= " + (int)NotaFiscal.SituacaoEnum.Cancelada + @") as PedidoCancelado, false as PedidoMaoObra, 
                f.nome as nomeFuncPerda, null as DataLiberacaoPedido, false as PedidoProducao, 1 as qtde, '$$$' as Criterio" : "count(*)";

            string campos5 = selecionar ? @"
                null, nf.numeroNFe, null, null, pcv.idFuncPerda, null, 
                null, null, " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @", pcv.dataPerda, pi.numEtiqueta, null, pcv.idTipoPerda, pcv.idSubtipoPerda, 
                pcv.obs, null, null, pcv.idTipoPerda, pcv.idSubtipoPerda, pcv.dataPerda,  
                null, pi.idImpressao, false, null, null, 
                p.altura, p.largura, cv.descricao as Cor, p.Espessura, p.Descricao as DescrProduto, p.CodInterno, 
                null, null, null as IdCliente, null as nomeCliente, null as CodAplicacao, 
                null as CodProcesso, null as IdPedidoExibir, 'Retalho de Produção' as descrSetor, 'Retalho de Producao' as descrSetorRepos, p.CustoCompra AS ValorCustoUnitario, pnf.ValorUnitario as ValorUnit, 
                null, round((p.Altura * p.Largura) / 1000000, 2) as TotM2, (nf.situacao= " + (int)NotaFiscal.SituacaoEnum.Cancelada + @") as PedidoCancelado, false as PedidoMaoObra, 
                f.nome as nomeFuncPerda, null as DataLiberacaoPedido, false as PedidoProducao, 1 as qtde, '$$$' as Criterio" : "count(*)";

            string sql =
                "(select " + campos1 + @"
                from produto_pedido_producao ppp
                    Left Join setor s On (ppp.idSetor=s.idSetor)
                    Left Join setor sr On (ppp.idSetorRepos=sr.idSetor)
                    Inner Join produtos_pedido_espelho pp On (ppp.idProdPed=pp.idProdPed) 
                    Left Join ambiente_pedido_espelho a On (pp.idAmbientePedido=a.idAmbientePedido) 
                    Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                    Left Join liberarpedido lp On (lp.idLiberarPedido=ped.idLiberarPedido) 
                    Inner Join cliente cli On (ped.idCli=cli.id_Cli) 
                    Inner Join produto p On (pp.idProd=p.idProd)
                    Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro)
                    Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                    Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                    Inner Join funcionario f on (ppp.idFuncRepos=f.idFunc)
                where ppp.pecaReposta=true{0})
                
                " + (selecionar ? "union all" : "+") + " (select " + campos2 + @"
                from produto_pedido_producao ppp
                    Inner Join dados_reposicao dr on (ppp.idProdPedProducao=dr.idProdPedProducao)
                    Left Join setor s On (ppp.idSetor=s.idSetor)
                    Left Join setor sr On (dr.idSetorRepos=sr.idSetor)
                    Inner Join produtos_pedido_espelho pp On (ppp.idProdPed=pp.idProdPed) 
                    Left Join ambiente_pedido_espelho a On (pp.idAmbientePedido=a.idAmbientePedido) 
                    Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
                    Left Join liberarpedido lp On (lp.idLiberarPedido=ped.idLiberarPedido) 
                    Inner Join cliente cli On (ped.idCli=cli.id_Cli) 
                    Inner Join produto p On (pp.idProd=p.idProd)
                    Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro)
                    Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
                    Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
                    Inner Join funcionario f on (dr.idFuncRepos=f.idFunc)
                where ppp.pecaReposta=true{1})
                
                " + (selecionar ? "union all" : "+") + " (select " + campos3 + @"
                from produto_trocado pt
                    inner join troca_devolucao td on (pt.idTrocaDevolucao=td.idTrocaDevolucao)
                    left join pedido ped On (td.idPedido=ped.idPedido) 
                    left join produtos_pedido pp on (pt.idProdPed=pp.idProdPed)
                    left Join ambiente_pedido a On (pp.idAmbientePedido=a.idAmbientePedido) 
                    left Join liberarpedido lp On (lp.idLiberarPedido=ped.idLiberarPedido) 
                    left Join cliente cli On (ped.idCli=cli.id_Cli) 
                    left Join produto p On (pt.idProd=p.idProd)
                    Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro)
                    left Join etiqueta_aplicacao apl On (pt.idAplicacao=apl.idAplicacao) 
                    left Join etiqueta_processo prc On (pt.idProcesso=prc.idProcesso) 
                    left Join funcionario f on (td.idFunc=f.idFunc)
                where td.situacao=" + (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada + @"{2})

                " + (selecionar ? "union all" : "+") + " (select " + campos4 + @"
                from perda_chapa_vidro pcv
                    inner join produto_impressao pi on (pcv.idProdImpressao=pi.idProdImpressao)
                    inner join produtos_nf pnf on (pi.idProdNf=pnf.idProdNf)
                    inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                    left Join produto p On (pnf.idProd=p.idProd)
                    left Join funcionario f on (pcv.idFuncPerda=f.idFunc)
                    Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro)
                where !pcv.Cancelado {3})"

                + (selecionar ? "union all" : "+") + " (select " + campos5 + @"
                from perda_chapa_vidro pcv
                    inner join produto_impressao pi on (pcv.idProdImpressao=pi.idProdImpressao)
                    inner join retalho_producao rp ON (pi.IdRetalhoProducao = rp.IdRetalhoProducao)
                    inner join produtos_nf pnf on (rp.idProdNf=pnf.idProdNf)
                    inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                    left Join produto p On (rp.idProd=p.idProd)
                    left Join funcionario f on (pcv.idFuncPerda=f.idFunc)
                    Left Join cor_vidro cv On (p.idCorVidro=cv.idCorVidro)
                where !pcv.Cancelado {3})";


            string where1 = "", where2 = "", where3 = "", where4 = "";

            if (idFuncPerda > 0)
            {
                where1 += " and ppp.idFuncRepos=" + idFuncPerda;
                where2 += " and dr.idFuncRepos=" + idFuncPerda;
                where3 += " and td.idFunc=" + idFuncPerda;
                where4 += " and pcv.idFuncPerda=" + idFuncPerda;
                criterio += "Funcionário Perda: " + FuncionarioDAO.Instance.GetNome(idFuncPerda) + "    ";
            }

            if (idLoja > 0)
            {
                where1 += " and ped.IdLoja=" + idLoja;
                where2 += " and ped.IdLoja=" + idLoja;
                where3 += " and ped.IdLoja=" + idLoja;
                where4 += " and nf.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (!String.IsNullOrEmpty(idTipoPerda))
            {
                if (!idTipoPerda.Split(',').Contains("0"))
                {

                    where1 += " and coalesce(ppp.tipoPerda, ppp.tipoPerdaRepos) In (" + idTipoPerda + ")";
                    where2 += " and dr.tipoPerdaRepos In (" + idTipoPerda + ")";
                    where3 += " and td.idTipoPerda In (" + idTipoPerda + ")";
                    where4 += " and pcv.idTipoPerda In (" + idTipoPerda + ")";

                    var nomeTipoPerda = String.Empty;

                    foreach (var id in idTipoPerda.Split(','))
                        nomeTipoPerda += TipoPerdaDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(id)) + ", ";

                    criterio += "Tipo(s) Perda: " + nomeTipoPerda.TrimEnd(' ').TrimEnd(',') + "    ";
                }
                else
                    criterio += "Tipo(s) Perda: Todos    ";
            }

            if (idPedido > 0)
            {
                where1 += " And (ped.idPedido=" + idPedido + " Or ped.IdPedidoAnterior=" + idPedido + " or ppp.idPedidoExpedicao=" + idPedido + ")";
                where2 += " And (ped.idPedido=" + idPedido + " Or ped.IdPedidoAnterior=" + idPedido + " or ppp.idPedidoExpedicao=" + idPedido + ")";
                where3 += " and td.idPedido=" + idPedido;
                where4 += " and false";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (numeroNFe > 0)
            {
                where1 += " And false";
                where2 += " And false";
                where3 += " and false";
                where4 += " and nf.numeroNFe=" + numeroNFe;
                criterio += "NF-e: " + numeroNFe + "    ";
            }

            if (idCliente > 0)
            {
                where1 += " and cli.id_Cli=" + idCliente;
                where2 += " and cli.id_Cli=" + idCliente;
                where3 += " and cli.id_Cli=" + idCliente;
                where4 += " and false";
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                where1 += " And cli.id_Cli in (" + ids + ")";
                where2 += " And cli.id_Cli in (" + ids + ")";
                where3 += " and cli.id_cli in (" + ids + ")";
                where4 += " and false";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (!String.IsNullOrEmpty(codInterno))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(codInterno, null);
                where1 += " And pp.idProd In (" + ids + ")";
                where2 += " And pp.idProd In (" + ids + ")";
                where3 += " And pt.idProd In (" + ids + ")";
                where4 += " And pnf.idProd In (" + ids + ")";
                criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(codInterno) + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProd);
                where1 += " And pp.idProd In (" + ids + ")";
                where2 += " And pp.idProd In (" + ids + ")";
                where3 += " And pt.idProd In (" + ids + ")";
                where4 += " And pnf.idProd In (" + ids + ")";
                criterio += "Produto: " + descrProd + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                where1 += " and ppp.dataRepos>=?dataIni";
                where2 += " and dr.dataRepos>=?dataIni";
                where3 += " and td.dataTroca>=?dataIni";
                where4 += " and pcv.dataPerda>=?dataIni";
                criterio += "Data início Perda: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                where1 += " and ppp.dataRepos<=?dataFim";
                where2 += " and dr.dataRepos<=?dataFim";
                where3 += " and td.dataTroca<=?dataFim";
                where4 += " and pcv.dataPerda<=?dataFim";
                criterio += "Data fim Perda: " + dataFim + "    ";
            }

            if (!String.IsNullOrEmpty(idSetor))
            {
                var setor = idSetor.Split(',').Select(x => Glass.Conversoes.StrParaInt(x));
                string crit, w1, w2, w3; bool w4 = false;
                crit = w1 = w2 = w3 = String.Empty;

                foreach (var s in setor)
                    if (s > 0)
                    {
                        w1 += s + ",";
                        w2 += s + ",";
                        crit += Utils.ObtemSetor((uint)s).Descricao + ", ";
                    }
                    else if (s == -1)
                    {
                        w3 += (int)TrocaDevolucao.TipoTrocaDev.Devolucao + ",";
                        crit += "Devolução, ";
                    }
                    else if (s == -2)
                    {
                        w3 += (int)TrocaDevolucao.TipoTrocaDev.Troca + ",";
                        crit += "Troca, ";
                    }
                    else if (s == -3)
                    {
                        w4 = true;
                        crit += "Chapa de Vidro, ";
                    }

                where1 += String.Format(" and ppp.idSetorRepos in ({0})", w1 != "" ? w1.TrimEnd(',') : "0");
                where2 += String.Format(" and dr.idSetorRepos in ({0})", w2 != "" ? w2.TrimEnd(',') : "0");
                where3 += String.Format(" and td.tipo in ({0})", w3 != "" ? w3.TrimEnd(',') : "0");
                where4 += !w4 ? " and false" : "";

                criterio += "Setor: " + crit.TrimEnd(' ', ',') + "    ";
            }
            else
            {
                where1 += " and false";
                where2 += " and false";
                where3 += " and false";
                where4 += " and false";
            }

            if (idTurno > 0)
            {
                string inicio = TurnoDAO.Instance.ObtemValorCampo<string>("inicio", "idTurno=" + idTurno);
                string termino = TurnoDAO.Instance.ObtemValorCampo<string>("termino", "idTurno=" + idTurno);
                string descricao = TurnoDAO.Instance.ObtemValorCampo<string>("descricao", "idTurno=" + idTurno);

                if (TimeSpan.Parse(inicio) <= TimeSpan.Parse(termino))
                {
                    where1 += " and ppp.DataRepos>=cast(concat(date_format(ppp.DataRepos, '%Y-%m-%d'), ' " + inicio + "') as datetime) and ppp.DataRepos<=cast(concat(date_format(ppp.DataRepos, '%Y-%m-%d'), ' " + termino + "') as datetime)";
                    where2 += " and dr.DataRepos>=cast(concat(date_format(dr.DataRepos, '%Y-%m-%d'), ' " + inicio + "') as datetime) and dr.DataRepos<=cast(concat(date_format(dr.DataRepos, '%Y-%m-%d'), ' " + termino + "') as datetime)";
                    where3 += " and td.DataTroca>=cast(concat(date_format(td.DataTroca, '%Y-%m-%d'), ' " + inicio + "') as datetime) and td.DataTroca<=cast(concat(date_format(td.DataTroca, '%Y-%m-%d'), ' " + termino + "') as datetime)";
                    where4 += " and pcv.dataPerda>=cast(concat(date_format(pcv.dataPerda, '%Y-%m-%d'), ' " + inicio + "') as datetime) and pcv.dataPerda<=cast(concat(date_format(pcv.dataPerda, '%Y-%m-%d'), ' " + termino + "') as datetime)";
                }
                else
                {
                    where1 += " and (ppp.DataRepos>=cast(concat(date_format(ppp.DataRepos, '%Y-%m-%d'), ' " + inicio + "') as datetime) or ppp.DataRepos<cast(concat(date_format(ppp.DataRepos, '%Y-%m-%d'), ' " + termino + "') as datetime))";
                    where2 += " and (dr.DataRepos>=cast(concat(date_format(dr.DataRepos, '%Y-%m-%d'), ' " + inicio + "') as datetime) or dr.DataRepos<cast(concat(date_format(dr.DataRepos, '%Y-%m-%d'), ' " + termino + "') as datetime))";
                    where3 += " and (td.DataTroca>=cast(concat(date_format(td.DataTroca, '%Y-%m-%d'), ' " + inicio + "') as datetime) or td.DataTroca<cast(concat(date_format(td.DataTroca, '%Y-%m-%d'), ' " + termino + "') as datetime))";
                    where4 += " and (pcv.dataPerda>=cast(concat(date_format(pcv.dataPerda, '%Y-%m-%d'), ' " + inicio + "') as datetime) or pcv.dataPerda<cast(concat(date_format(pcv.dataPerda, '%Y-%m-%d'), ' " + termino + "') as datetime))";
                }

                criterio += "Turno: " + descricao + "    ";
            }

            if (idCorVidro > 0)
            {
                where1 += " and p.idCorVidro = " + idCorVidro.ToString();
                where2 += " and p.idCorVidro = " + idCorVidro.ToString();
                where3 += " and p.idCorVidro = " + idCorVidro.ToString();
                where4 += " and p.idCorVidro = " + idCorVidro.ToString();
                criterio += " Cor do Vidro: " + CorVidroDAO.Instance.GetElementByPrimaryKey((uint)idCorVidro).Descricao + "    ";
            }

            if (espessuraVidro > 0)
            {
                where1 += " and p.espessura = ?esp";
                where2 += " and p.espessura = ?esp";
                where3 += " and p.espessura = ?esp";
                where4 += " and p.espessura = ?esp";
                criterio += " Espessura: " + espessuraVidro.ToString() + "mm" + "    ";
            }

            where1 += " and pp.qtde > 0 ";
            where2 += " and pp.qtde > 0 ";
            where3 += " and pp.qtde > 0 ";

            sql = String.Format(sql, where1, where2, where3, where4);
            sql = sql.Replace("$$$", criterio);
            sql = "Select " + (selecionar ? "* From (" + sql + ") As Temp" : sql);

            return sql;
        }

        public IList<ProdutoPedidoProducao> GetListPerdaReposPeca(uint idFuncPerda, uint idPedido, uint idLoja, uint idCliente, string nomeCliente,
            string codInterno, string descrProd, string dataIni, string dataFim, string idSetor, uint idTurno, string idTipoPerda,
            int idCorVidro, float espessuraVidro, uint numeroNFe, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "idProdPedProducao Desc " : sortExpression;
            return LoadDataWithSortExpression(SqlPerdaReposPeca(idFuncPerda, idPedido, idLoja, idCliente, nomeCliente, codInterno, descrProd, dataIni,
                dataFim, idSetor, idTurno, idTipoPerda, true, idCorVidro, espessuraVidro, numeroNFe), sortExpression, startRow, pageSize,
                GetParam(null, null, null, dataIni, dataFim, null, null, null, null, nomeCliente, null, null, null, espessuraVidro));
        }

        public int GetCountPerdaReposPeca(uint idFuncPerda, uint idPedido, uint idLoja, uint idCliente, string nomeCliente,
            string codInterno, string descrProd, string dataIni, string dataFim, string idSetor,
            uint idTurno, string idTipoPerda, int idCorVidro, float espessuraVidro, uint numeroNFe)
        {
            return GetCountWithInfoPaging(SqlPerdaReposPeca(idFuncPerda, idPedido, idLoja, idCliente, nomeCliente, codInterno, descrProd,
                dataIni, dataFim, idSetor, idTurno, idTipoPerda, true, idCorVidro, espessuraVidro, numeroNFe),
                GetParam(null, null, null, dataIni, dataFim, null, null, null, null, nomeCliente, null, null, null, espessuraVidro));
        }

        public IList<ProdutoPedidoProducao> GetForRptPerdaReposPeca(uint idFuncPerda, uint idPedido, uint idLoja, uint idCliente, string nomeCliente,
            string codInterno, string descrProd, string dataIni, string dataFim, string idSetor, uint idTurno,
            string idTipoPerda, int idCorVidro, float espessura, uint numeroNFe)
        {
            return objPersistence.LoadData(SqlPerdaReposPeca(idFuncPerda, idPedido, idLoja, idCliente, nomeCliente, codInterno, descrProd, dataIni,
                dataFim, idSetor, idTurno, idTipoPerda, true, idCorVidro, espessura, numeroNFe), GetParam(null, null, null, dataIni, dataFim, null,
                null, null, null, nomeCliente, null, null, null, espessura)).ToList();
        }

        #endregion

        #region Recupera um ProdutoPedidoProducao para a imagem da peça

        internal ProdutoPedidoProducao GetForImagemPeca(string codEtiqueta)
        {
            try
            {
                // Valida a etiqueta
                ValidaEtiquetaProducao(null, ref codEtiqueta);
            }
            catch
            {
                return null;
            }

            string where = "idProdPedProducao=" + ObtemValorCampo<uint>("idProdPedProducao",
                "numEtiqueta=?num", new GDAParameter("?num", codEtiqueta));

            ProdutoPedidoProducao item = new ProdutoPedidoProducao();
            item.IdProdPed = ObtemValorCampo<uint?>("idProdPed", where);
            item.NumEtiqueta = codEtiqueta;

            return item;
        }

        #endregion

        #region Verifica se a etiqueta foi reposta

        /// <summary>
        /// Verifica se a etiqueta foi reposta.
        /// </summary>
        /// <param name="codEtiqueta">Número da etiqueta da peça</param>
        /// <param name="semLeituras">A peça deve estar sem leituras na produção?</param>
        /// <returns></returns>
        public bool IsPecaReposta(GDASession sessao, string codEtiqueta, bool semLeituras)
        {
            uint idProdPedProducao = ObtemIdProdPedProducaoPorSituacao(sessao, codEtiqueta, (int)ProdutoPedidoProducao.SituacaoEnum.Producao).GetValueOrDefault(0);
            if (idProdPedProducao == 0)
                return false;

            return IsPecaReposta(sessao, idProdPedProducao, semLeituras);
        }

        /// <summary>
        /// Verifica se a etiqueta foi reposta.
        /// </summary>
        /// <param name="codEtiqueta">Número da etiqueta da peça</param>
        /// <param name="semLeituras">A peça deve estar sem leituras na produção?</param>
        /// <returns></returns>
        public bool IsPecaReposta(string codEtiqueta, bool semLeituras)
        {
            return IsPecaReposta(null, codEtiqueta, semLeituras);
        }

        /// <summary>
        /// Verifica se a etiqueta foi reposta.
        /// </summary>
        /// <param name="idProdPedProducao">Id da etiqueta da peça</param>
        /// <param name="semLeituras">A peça deve estar sem leituras na produção?</param>
        /// <returns></returns>
        public bool IsPecaReposta(GDASession sessao, uint idProdPedProducao, bool semLeituras)
        {
            if (!ObtemValorCampo<bool>(sessao, "pecaReposta", "idProdPedProducao=" + idProdPedProducao))
                return false;

            return !semLeituras || LeituraProducaoDAO.Instance.GetCountByProdPedProducao(sessao, idProdPedProducao) == 0;
        }

        /// <summary>
        /// Verifica se a etiqueta foi reposta.
        /// </summary>
        /// <param name="idProdPedProducao">Id da etiqueta da peça</param>
        /// <param name="semLeituras">A peça deve estar sem leituras na produção?</param>
        /// <returns></returns>
        public bool IsPecaReposta(uint idProdPedProducao, bool semLeituras)
        {
            return IsPecaReposta(null, idProdPedProducao, semLeituras);
        }

        public string GetDadosReposicaoPeca(GDASession sessao, uint idProdPedProducao)
        {
            return ObtemValorCampo<string>(sessao, "dadosReposicaoPeca", "idProdPedProducao=" + idProdPedProducao);
        }

        /// <summary>
        /// Recupera quantas etiquetas foram repostas de uma impressão
        /// </summary>
        /// <param name="idImpressao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int GetCountRepoPeca(uint idImpressao, uint idPedido)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM produto_impressao pi
                    INNER JOIN produto_pedido_producao ppp ON (pi.idImpressao = ppp.idImpressao AND pi.idProdPed = ppp.idProdPed AND
                        ppp.numEtiqueta = pi.numEtiqueta)
                WHERE ppp.pecaReposta = true AND pi.idImpressao = " + idImpressao;

            if (idPedido > 0)
                sql += " AND pi.idPedido = " + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        #endregion

        #region Sql para consulta do id da impressão dos dados da reposição

        internal string SqlIdImpressao(string campoDadosReposicao)
        {
            string locate = "locate('~', {1}, {0} + 1)";
            string inicio = locate;

            for (int i = 0; i < 4; i++)
                inicio = String.Format(inicio, locate, "{1}");

            string fim = String.Format(inicio, String.Format(locate, "-1", "{1}"), "{1}");
            inicio = String.Format(inicio, "-1", "{0}");
            fim = fim.Replace("{1}", "{0}");

            string campo = String.Format("substr({0}, " + inicio + " + 1, " + fim + " - " + inicio + " - 1)", campoDadosReposicao);

            return String.Format("if(instr((@campo := {0}), '!'), null, @campo)", campo);
        }

        #endregion

        #region Recupera a quantidade de produtos box que ja foi dada expedição

        /// <summary>
        /// Recupera a quantidade de produtos que ja foi vinculado expedição
        /// </summary>
        /// <param name="idPedidoExp"></param>
        /// <param name="idProduto"></param>
        /// <returns></returns>
        public int GetQtdePecaVinculadaExpedicao(uint idPedidoExp, uint idProduto)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM produto_pedido_producao ppp
                    INNER JOIN produtos_pedido pp ON (ppp.idprodped = pp.idprodpedEsp)
                WHERE ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                    AND ppp.idPedidoExpedicao = " + idPedidoExp + " AND pp.idProd = " + idProduto + @"
                    AND COALESCE(pp.invisivelFLuxo, FALSE)=FALSE";

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        #endregion

        #region Atualiza a situação da peça na produção

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <param name="impressaoEtiqueta"></param>
        /// <returns></returns>
        private bool PecaEstaProntaRoteiro(uint idProdPedProducao, bool impressaoEtiqueta)
        {
            return PecaEstaProntaRoteiro(null, idProdPedProducao, impressaoEtiqueta);
        }

        private bool PecaEstaProntaRoteiro(GDASession sessao, uint idProdPedProducao, bool impressaoEtiqueta)
        {
            var setores = SetorDAO.Instance.ObtemSetoresObrigatorios(sessao, idProdPedProducao);

            if (setores == null || setores.Count == 0)
                return !impressaoEtiqueta;

            var sql = string.Format(@"
                select sum(lp.dataLeitura is null)=0
                from setor s
                    left join leitura_producao lp on (lp.idProdPedProducao={0} and lp.idSetor=s.idSetor)
                    left join produto_pedido_producao ppp on (ppp.idProdPedProducao={0} and ppp.situacao={1})
                where s.idSetor in ({2}) and s.tipo in ({3})",

                idProdPedProducao,
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                string.Join(",", setores.Select(x => x.IdSetor.ToString()).ToArray()),
                (int)TipoSetor.PorRoteiro);

            return ExecuteScalar<int>(sessao, sql) == 1;
        }

        /// <summary>
        /// Atualiza a situação da peça na produção.
        /// </summary>
        public void AtualizaSituacaoPecaNaProducao(string etiqueta, DateTime? dataLeitura, bool atualizarSituacaoPedido)
        {
            uint idProdPedProducao = ObtemIdProdPedProducao(etiqueta).GetValueOrDefault();
            AtualizaSituacaoPecaNaProducao(idProdPedProducao, dataLeitura, atualizarSituacaoPedido);
        }

        /// <summary>
        /// Atualiza a situação da peça na produção.
        /// </summary>
        public void AtualizaSituacaoPecaNaProducao(GDASession sessao, uint idProdPedProducao, DateTime? dataLeitura,
            bool atualizarSituacaoPedido)
        {
            Dictionary<int, SituacaoProdutoProducao> idsPedidoSituacaoProducao = null;

            AtualizaSituacaoPecaNaProducao(sessao, idProdPedProducao, dataLeitura, atualizarSituacaoPedido, ref idsPedidoSituacaoProducao);
        }

        /// <summary>
        /// Atualiza a situação da peça na produção.
        /// </summary>
        public void AtualizaSituacaoPecaNaProducao(GDASession sessao, uint idProdPedProducao, DateTime? dataLeitura,
            bool atualizarSituacaoPedido, ref Dictionary<int, SituacaoProdutoProducao> idsPedidoSituacaoProducao)
        {
            if (idProdPedProducao == 0)
                return;

            uint idSetor = ObtemValorCampo<uint>(sessao, "idSetor", "idProdPedProducao=" + idProdPedProducao);
            SituacaoProdutoProducao situacaoProducao;
            var setor = Utils.ObtemSetor(idSetor);

            if (setor == null)
            {
                if (idSetor == 0)
                    throw new Exception("Não foi possível recuperar o setor.");

                setor = SetorDAO.Instance.GetElementByPrimaryKey(sessao, idSetor);

                if (setor.Situacao == Situacao.Inativo)
                    throw new Exception(string.Format("O setor {0} está inativo. É necessário ativá-lo para prosseguir.", setor.Descricao));

                /* Chamado 41576.
                 * É importante que o sistema não prossiga pois o setor não foi recuperado através do método Utils.ObtemSetor, o motivo deve ser averiguado. */
                throw new Exception("Não foi possível recuperar o setor.");
            }

            switch (setor.Tipo)
            {
                case TipoSetor.Pendente:
                    situacaoProducao = SituacaoProdutoProducao.Pendente;
                    break;

                case TipoSetor.Pronto:
                    situacaoProducao = SituacaoProdutoProducao.Pronto;
                    break;

                case TipoSetor.Entregue:
                case TipoSetor.ExpCarregamento:
                    situacaoProducao = SituacaoProdutoProducao.Entregue;
                    break;

                case TipoSetor.PorRoteiro:
                    situacaoProducao = PecaEstaProntaRoteiro(sessao, idProdPedProducao, Utils.ObtemSetor(idSetor).NumeroSequencia == 1) ?
                        SituacaoProdutoProducao.Pronto :
                        SituacaoProdutoProducao.Pendente;
                    break;

                default:
                    throw new Exception("Não foi identificado o tipo do setor.");
            }

            objPersistence.ExecuteCommand(sessao, String.Format(@"update produto_pedido_producao set situacaoProducao={1}
                where idProdPedProducao={0}", idProdPedProducao, (int)situacaoProducao));

            dataLeitura = dataLeitura ?? DateTime.Now;

            if (atualizarSituacaoPedido)
            {
                var idPedido = ObtemIdPedido(sessao, idProdPedProducao);

                if (idPedido > 0)
                    PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, idPedido, situacaoProducao, dataLeitura.Value);
            }
            // A chamada que informa este parâmetro como não nulo é onde ela deve ser retornada com as informações.
            else if (idsPedidoSituacaoProducao != null)
            {
                var idPedido = ObtemIdPedido(sessao, idProdPedProducao);

                if (idPedido > 0)
                {
                    // Salva no dicionário o ID do pedido com a situação de produção dele, para que, no estorno do carregamento,
                    // os pedidos tenham a situação de produção atualizada somente uma vez, ao invés de ser atualizada após o estorno de cada peça.
                    if (idsPedidoSituacaoProducao.Keys.Contains((int)idPedido))
                        idsPedidoSituacaoProducao[(int)idPedido] = situacaoProducao;
                    else
                        idsPedidoSituacaoProducao.Add((int)idPedido, situacaoProducao);
                }
            }

            var idPedidoNovo = ObtemIdPedidoExpedicao(sessao, idProdPedProducao);

            if (idPedidoNovo > 0)
                PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, idPedidoNovo.Value, situacaoProducao, dataLeitura.Value);
        }

        /// <summary>
        /// Atualiza a situação da peça na produção.
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public void AtualizaSituacaoPecaNaProducao(uint idProdPedProducao, DateTime? dataLeitura, bool atualizarSituacaoPedido)
        {
            AtualizaSituacaoPecaNaProducao(null, idProdPedProducao, dataLeitura, atualizarSituacaoPedido);
        }

        #endregion

        #region Parar / Retornar peça na produção

        /// <summary>
        /// Parar / Retornar peça na produção
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <param name="motivo"></param>
        public void PararRetornarPecaProducao(uint idProdPedProducao, string motivo)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuPCP.PararRetornarPecaProducao))
                throw new Exception("Você não tem permissão para Parar/Retornar peça na produção");

            if (idProdPedProducao == 0)
                throw new Exception("Peça não encontrada");

            if (string.IsNullOrEmpty(motivo))
                throw new Exception("Motivo não informado");

            if (motivo.Length > 500)
                throw new Exception("O motivo deve ter um tamanho máximo de 500 caracteres.");

            var prodPedProducao = GetElementByPrimaryKey(idProdPedProducao);

            if (prodPedProducao == null)
                throw new Exception("Peça não encontrada");

            prodPedProducao.PecaParadaProducao = !prodPedProducao.PecaParadaProducao;
            prodPedProducao.MotivoPecaParadaProducao = motivo;

            LogAlteracaoDAO.Instance.LogProdPedProducao(null, prodPedProducao, LogAlteracaoDAO.SequenciaObjeto.Novo);

            Update(prodPedProducao);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se uma peça esta com a produção parada
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public string VerificaPecaProducaoParada(string etiqueta)
        {
            return VerificaPecaProducaoParada(null, etiqueta);
        }

        /// <summary>
        /// Verifica se uma peça esta com a produção parada
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public string VerificaPecaProducaoParada(GDASession sessao, string etiqueta)
        {
            try
            {
                if (string.IsNullOrEmpty(etiqueta))
                    return "false;";

                // Este método é chamado duas vezes, se todas as peças do pedido estiverem sendo marcadas de uma só vez
                // na primeira chamada a etiqueta vem com a letra P, na segunda chamada as peças são passadas e a verificação é feita normalmente.
                if (etiqueta.ToUpper()[0] == 'P' || etiqueta.ToUpper()[0] == 'C' || etiqueta.ToUpper()[0] == 'V' ||
                    etiqueta.ToUpper()[0] == 'N' || etiqueta.ToUpper()[0] == 'R')
                    return "false;";

                var idProdPedProducao = ObtemIdProdPedProducao(sessao, etiqueta);

                if (idProdPedProducao.GetValueOrDefault() == 0)
                    throw new Exception("Peça não encontrada. Etiqueta: " + etiqueta);

                var parado = ObtemValorCampo<bool>(sessao, "pecaParadaProducao", "idProdPedProducao=" + idProdPedProducao);

                return parado ? "true;" + ObtemValorCampo<string>(sessao, "motivoPecaParadaProducao", "idProdPedProducao=" + idProdPedProducao) : "false;";
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("VerificaPecaProducaoParada - " + etiqueta, ex);
                throw ex;
            }
        }

        #endregion

        #region Verifica peça cancelada sem etiqueta impressa

        /// <summary>
        /// Verifica se o pedido possui alguma peça que foi cancela e que nã foi impressa novamente
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool VerificaPecaCanceladaSemNovaImpressao(GDASession sessao, uint idPedido)
        {
            var qtdMaoDeObra = "IF(ped.tipoPedido = " + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", ap.qtde * pp.qtde, pp.qtde)";
            var qtdImpressoMaoDeObra = "IF(ped.tipoPedido = " + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", ape.QtdeImpresso, ppe.QtdImpresso)";

            var sql = @"
                SELECT count(*)
                FROM produto_pedido_producao ppp
                    LEFT JOIN (SELECT DISTINCT ppp.numEtiqueta
                                FROM produto_pedido_producao ppp
                                    INNER JOIN produtos_pedido pp ON (pp.idprodpedesp = ppp.idprodped)
                                WHERE situacao = " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                                    AND idpedido = " + idPedido + @") AS tmp ON (tmp.numEtiqueta = COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta))
                    INNER JOIN produtos_pedido pp ON (pp.idprodpedesp = ppp.idprodped)
                    LEFT JOIN produtos_pedido_espelho ppe ON (pp.idprodpedesp = ppe.idprodped)
                    LEFT JOIN pedido ped ON (ped.IdPedido = pp.IdPedido)
                    LEFT JOIN ambiente_pedido ap ON (pp.IdAmbientePedido = ap.IdAmbientePedido)
                    LEFT JOIN ambiente_pedido_espelho ape ON (ppe.IdAmbientePedido = ape.IdAmbientePedido)
                WHERE ppp.situacao IN (" + (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda + @")
                    /* Chamado 12724.
                       Caso o produto tenha sido removido do pedido o mesmo não pode ser contabilizado como peça cancelada sem nova impressão. */
                    AND (" + qtdMaoDeObra + @" - Coalesce(pp.qtdeInvisivel, 0)) > 0
                    AND Coalesce(" + qtdMaoDeObra + @", 0) <> Coalesce(" + qtdImpressoMaoDeObra + @", 0)
                    AND pp.idpedido = " + idPedido + @"
                    AND tmp.numEtiqueta IS NULL
                    AND COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta) is not null";

            return ExecuteScalar<int>(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se a peça informada faz parte de uma liberação

        /// <summary>
        /// Verifica se a peça informada faz parte de uma liberação
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public bool FazParteLiberacao(string numEtiqueta, int idLiberarPedido)
        {
            var sql = @"
                    SELECT count(*)
                FROM produto_pedido_producao ppp
                	INNER JOIN produtos_pedido pp ON (ppp.IdProdPed = pp.IdProdPedEsp)
                	INNER JOIN produtos_liberar_pedido plp ON (pp.IdPedido = plp.IdPedido)
                WHERE plp.IdLiberarPedido = " + idLiberarPedido + " AND ppp.numEtiqueta = ?etq";

            return objPersistence.ExecuteSqlQueryCount(sql, new GDAParameter("?etq", numEtiqueta)) > 0;
        }

        #endregion

        #region Verifica se a etiqueta foi lida em uma OC de transferência

        /// <summary>
        /// Verifica se a etiqueta foi lida em uma OC de transferência
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        public bool FoiLidoOCTransferencia(GDASession sessao, uint idProdPedProducao)
        {
            var sql = @"
                SELECT count(*) 
                FROM produto_pedido_producao ppp
	                INNER JOIN pedido_ordem_carga poc ON (ppp.IdPedidoExpedicao = poc.IdPedido)
                    INNER JOIN ordem_carga oc ON (poc.IdOrdemCarga = oc.IdOrdemCarga)
                WHERE oc.TipoOrdemCarga = " + (int)OrdemCarga.TipoOCEnum.Transferencia + @" 
                    AND idProdPedProducao = " + idProdPedProducao;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Obtem o numero de Sequencia da Etiqueta informada

        /// <summary>
        /// Obtem o numero de Sequencia da Etiqueta informada
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public int ObtemNumSequencia(string numEtiqueta)
        {
            var idPedido = numEtiqueta.Split('-')[0];
            var item = numEtiqueta.Split('-')[1].Split('.')[0];
            var posicao = numEtiqueta.Split('.')[1].Split('/')[0];

            var sql = @"
                SELECT COUNT(*)
                FROM
                  ( SELECT COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta)
                   FROM produtos_pedido_espelho ppe
                    INNER JOIN produto_pedido_producao ppp ON (ppe.idprodPed = ppp.idProdPed)
                   WHERE ppe.idPedido = " + idPedido + @"
                     AND CAST(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta),'.',1),'-',-1) AS signed) <= " + item + @"
                     AND If(CAST(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta),'.',1),'-',-1) AS signed) = " + item + @",
                                CAST(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta),'/',1),'.',-1) AS signed) <= " + posicao + @", 1)
                   GROUP BY COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta)
                   ORDER BY CAST(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta),'.',1),'-',-1) AS signed),
                            CAST(SUBSTRING_INDEX(SUBSTRING_INDEX(COALESCE(ppp.numEtiquetaCanc, ppp.numEtiqueta),'/',1),'.',-1) AS signed) ) AS tmp";

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        #endregion

        #region Produtos de Composição

        /// <summary>
        /// Verifica se a peça passou setor Laminado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="codEtiqueta"></param>
        /// <returns></returns>
        public bool PecaPassouSetorLaminado(GDASession sessao, string codEtiqueta)
        {
            var sql = @"
            SELECT COUNT(*) 
            FROM produto_pedido_producao ppp
	            INNER JOIN leitura_producao lp ON (ppp.IdProdPedProducao = lp.IdProdPedProducao)
                INNER JOIN setor s ON (lp.IdSetor = s.IdSetor)
            WHERE s.Laminado 
                AND ppp.NumEtiqueta = ?etq 
                AND ppp.Situacao = " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                AND s.Situacao = " + (int)Situacao.Ativo;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?etq", codEtiqueta)) > 0;
        }

        /// <summary>
        /// Busca um dicionario com as etiquetas que estão com vinculo correto ou não na composição do laminado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPedProducao"></param>
        /// <param name="etiquetas"></param>
        /// <returns></returns>
        public Dictionary<uint, bool> EtiquetasVinculoProdComposicao(GDASession sessao, uint idProdPedProducao, List<string> etiquetas)
        {
            var dic = new Dictionary<uint, bool>();

            foreach (var e in etiquetas)
            {
                var idProdPedProducaoParent = ObterIdProdPedProducaoParentByEtiqueta(sessao, e).GetValueOrDefault(0);
                var idProdPedProducaoFilho = ObtemIdProdPedProducao(sessao, e).GetValueOrDefault(0);

                dic.Add(idProdPedProducaoFilho, idProdPedProducaoParent == idProdPedProducao);
            }

            return dic;
        }

        /// <summary>
        /// Obtem o IdProdPedProducao para reveincular um produto de composição
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPedProducaoParent"></param>
        /// <param name="idProdPed"></param>
        /// <param name="idsProdPedProducao"></param>
        /// <returns></returns>
        public uint? ObterIdProdPedProducaoReveinculoComposicao(GDASession sessao, uint idProdPedProducaoParent, uint idProdPed, List<uint> idsProdPedProducao)
        {
            var sql = @"
                SELECT IdProdPedProducao 
                FROM produto_pedido_producao 
                WHERE Situacao = {0}
                    AND IdProdPedProducaoParent = {1} 
                    AND IdProdPed = {2} 
                    AND IdProdPedProducao NOT IN ({3})
                LIMIT 1";

            sql = string.Format(sql, (int)ProdutoPedidoProducao.SituacaoEnum.Producao, idProdPedProducaoParent, idProdPed, idsProdPedProducao.Count == 0 ? "0" : string.Join(",", idsProdPedProducao));

            return ExecuteScalar<uint?>(sessao, sql);
        }

        #endregion

        #region Produtos Fornada

        private string SqlPecasFornada(int idFornada, bool selecionar)
        {
            var campos = selecionar ? "ppp.*, ppe.Altura, ppe.Largura, p.CodInterno, p.Descricao AS DescrProduto" : "COUNT(*)";

            var sql = @"SELECT " + campos + @"
                FROM produto_pedido_producao ppp 
                    INNER JOIN produtos_pedido_espelho ppe ON (ppp.IdProdPed = ppe.IdProdPed)
                    INNER JOIN produto p ON (ppe.IdProd = p.IdProd)
                WHERE ppp.IdFornada=" + idFornada;

            return sql;
        }

        public IList<ProdutoPedidoProducao> ObterPecasFornada(int idFornada, string sortExpression, int startRow, int pageSize)
        {
            if (idFornada == 0)
                return new List<ProdutoPedidoProducao>();

            var sql = SqlPecasFornada(idFornada, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Obtem as peças que foram passadas na fornada em questão
        /// </summary>
        /// <param name="idFornada"></param>
        /// <returns></returns>
        public IList<ProdutoPedidoProducao> ObterPecasFornada(int idFornada)
        {
            return ObterPecasFornada(idFornada, null, 0, 0);
        }

        public int ObterPecasFornadaCount(int idFornada)
        {
            var sql = SqlPecasFornada(idFornada, false);

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Retorna as peças passadas nas respectivas fornadas
        /// </summary>
        /// <param name="idsFornada"></param>
        /// <returns></returns>
        public IList<ProdutoPedidoProducao> ObterPecasFornadaRpt(string idsFornada)
        {
            var campos = "ppp.*, ppe.Altura, ppe.Largura, p.CodInterno, p.Descricao AS DescrProduto";

            var sql = @"SELECT " + campos + @"
                FROM produto_pedido_producao ppp 
                    INNER JOIN produtos_pedido_espelho ppe ON (ppp.IdProdPed = ppe.IdProdPed)
                    INNER JOIN produto p ON (ppe.IdProd = p.IdProd)
                WHERE ppp.IdFornada IN({0})";

            sql = string.Format(sql, idsFornada);

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna as peças passadas nas respectivas fornadas
        /// </summary>
        /// <param name="idsFornada"></param>
        /// <returns></returns>
        public IList<ProdutoPedidoProducao> GetProdutosPedidoProducaoByIdProdPed(GDASession session, uint idProdPed)
        {
            var sql = @"SELECT * FROM produto_pedido_producao WHERE IdProdPed=" + idProdPed;
            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion
    }
}