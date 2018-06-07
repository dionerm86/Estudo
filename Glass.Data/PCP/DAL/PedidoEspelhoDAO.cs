using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.IO;
using System.Web;
using System.Linq;
using Glass.Data.RelDAL;
using Glass.Configuracoes;
using System.Globalization;
using Ionic.Utils.Zip;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed class PedidoEspelhoDAO : BaseDAO<PedidoEspelho, PedidoEspelhoDAO>
    {
        private static readonly object _cancelarEspelhoLock = new object();
        private static readonly object _reabrirPedidoLock = new object();

        #region Listagem de Pedidos Padrão

        private string Sql(uint idPedido, string codCliente, uint idCli, string nomeCli, uint idLoja, uint idFunc,
            uint idFuncionarioConferente, int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt,
            string dataIniFab, string dataFimFab, string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis,
            string dataFimEmis, bool soFinalizados, bool apenasMaoDeObra, string idsPedidos, bool pedidosSemAnexos, bool pedidosAComprar,
            string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, string tipoPedido, string idsRotas, string dtCompraIni,
            string dtCompraFim, int idCompra, bool incluirQtdePecas, bool selecionar, out bool temFiltro)
        {
            return Sql(null, idPedido, codCliente, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao,
                situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin,
                dataIniConf, dataFimConf, dataIniEmis, dataFimEmis, soFinalizados, apenasMaoDeObra, idsPedidos,
                pedidosSemAnexos, pedidosAComprar, situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, tipoPedido,
                idsRotas, dtCompraIni, dtCompraFim, idCompra, incluirQtdePecas, 0, 0, selecionar, out temFiltro);
        }

        private string Sql(uint idPedido, string codCliente, uint idCli, string nomeCli, uint idLoja, uint idFunc,
            uint idFuncionarioConferente, int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt,
            string dataIniFab, string dataFimFab, string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis,
            string dataFimEmis, bool soFinalizados, bool apenasMaoDeObra, string idsPedidos, bool pedidosSemAnexos, bool pedidosAComprar,
            string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, string tipoPedido, string idsRotas, string dtCompraIni,
            string dtCompraFim, int idCompra, bool incluirQtdePecas, int origemPedido, int pedidosConferidos, bool selecionar, out bool temFiltro)
        {
            return Sql(null, idPedido, codCliente, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao,
                situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin,
                dataIniConf, dataFimConf, dataIniEmis, dataFimEmis, soFinalizados, apenasMaoDeObra, idsPedidos,
                pedidosSemAnexos, pedidosAComprar, situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, tipoPedido,
                idsRotas, dtCompraIni, dtCompraFim, idCompra, incluirQtdePecas, origemPedido, pedidosConferidos, selecionar, out temFiltro);
        }

        private string Sql(GDASession session, uint idPedido, string codCliente, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis, string dataFimEmis,
            bool soFinalizados, bool apenasMaoDeObra, string idsPedidos, bool pedidosSemAnexos, bool pedidosAComprar, string situacaoCnc,
            string dataIniSituacaoCnc, string dataFimSituacaoCnc, string tipoPedido, string idsRotas, string dtCompraIni, string dtCompraFim,
            int idCompra, bool incluirQtdePecas, bool selecionar, out bool temFiltro)
        {
            return Sql(session, idPedido, codCliente, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente,
             situacao, situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab,
             dataIniFin, dataFimFin, dataIniConf, dataFimConf, dataIniEmis, dataFimEmis,
             soFinalizados, apenasMaoDeObra, idsPedidos, pedidosSemAnexos, pedidosAComprar, situacaoCnc,
             dataIniSituacaoCnc, dataFimSituacaoCnc, tipoPedido, idsRotas, dtCompraIni, dtCompraFim,
             idCompra, incluirQtdePecas, 0, 0, selecionar, out temFiltro);
        }

        private string Sql(GDASession session, uint idPedido, string codCliente, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis, string dataFimEmis,
            bool soFinalizados, bool apenasMaoDeObra, string idsPedidos, bool pedidosSemAnexos, bool pedidosAComprar, string situacaoCnc,
            string dataIniSituacaoCnc, string dataFimSituacaoCnc, string tipoPedido, string idsRotas, string dtCompraIni, string dtCompraFim,
            int idCompra, bool incluirQtdePecas, int origemPedido, int pedidosConferidos, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            var temProdutosComprar = pedidosAComprar ? ProdutosPedidoEspelhoDAO.Instance.SqlCompra(session, "pe.idPedido", null, 0, null, null, 0, false) : "0";

            var campos = selecionar ? @"pe.*, p.idProjeto, p.tipoVenda, p.idFormaPagto, p.tipoEntrega, p.idSinal, c.Revenda as CliRevenda, 
                p.valorEntrada, p.Total as TotalPedido, p.DataEntrega, p.dataEntregaOriginal, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, 
                p.FastDelivery, p.idCli, p.TipoPedido, p.Importado " +

                (incluirQtdePecas ?
                @"/* QTDEPEÇAS */
                ,IF(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @",

                    /* AMBIENTE */
                    (SELECT CAST(SUM(COALESCE(ape.qtde, 0)) AS SIGNED INTEGER)
                    FROM ambiente_pedido_espelho ape
                        LEFT JOIN produto p ON (ape.idProd=p.idProd)
                    WHERE ape.idPedido=pe.idPedido AND p.idGrupoProd=" + (int)NomeGrupoProd.Vidro + @"),

                    /* PRODUTO */
                    (SELECT CAST(SUM(COALESCE(ppe.qtde, 0)) AS SIGNED INTEGER)
                    FROM produtos_pedido_espelho ppe
                        LEFT JOIN produto p ON (ppe.idProd=p.idProd)
                    WHERE !COALESCE(invisivelFluxo,FALSE) AND ppe.IdProdPedParent IS NULL AND ppe.idPedido=pe.idPedido AND p.idGrupoProd=" + (int)NomeGrupoProd.Vidro + "))" : ", 0") +

                    @" AS QtdePecas,

                f.Nome as NomeFunc, fconf.Nome as Conferente, ffin.Nome as ConferenteFin, Cast(l.IdLoja as unsigned) as idLoja, l.NomeFantasia as nomeLoja,
                fp.Descricao as FormaPagto, '$$$' as criterio, com.nome as nomeComissionado, (select cast(group_concat(distinct idItemProjeto) as char) as
                idItensProjeto from produtos_pedido_espelho where idPedido=pe.idPedido) as idItensProjeto, (" + temProdutosComprar + @") > 0 as
                temProdutosComprar, p.GeradoParceiro -- , Group_Concat(comp.idCompra Separator ', ') As CompraGerada" : "Count(*)";

            var criterio = string.Empty;

            var sql = @"
                Select " + campos + @" 
                From pedido_espelho pe 
                    Left Join pedido p On (pe.idPedido=p.idPedido) 
                    Left Join cliente c On (p.idCli=c.id_Cli) 
                    Left Join funcionario f On (p.IdFunc=f.IdFunc) 
                    Left Join funcionario fconf On (pe.IdFuncConf=fconf.IdFunc) 
                    Left Join funcionario ffin On (pe.IdFuncFin=ffin.IdFunc) 
                    Left Join loja l On (p.IdLoja = l.IdLoja) 
                    Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto)
                    Left Join comissionado com On (pe.idComissionado=com.idComissionado)
                  --  Left Join pedidos_compra pc ON (p.idPedido=pc.idPedido)
                  --  Left Join compra comp ON (pc.idCompra=comp.idCompra AND comp.situacao <> " + (int)Compra.SituacaoEnum.Cancelada + @")
                Where 1 ";

            if (idPedido > 0)
            {
                sql += " And p.IdPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }
            else if (!string.IsNullOrEmpty(idsPedidos))
            {
                sql += " And p.idPedido in (" + idsPedidos + ")";
                temFiltro = true;
            }
            else if (idCli > 0)
            {
                sql += " And p.IdCli=" + idCli;
                criterio += " Cliente: " + ClienteDAO.Instance.GetNome(session, idCli) + "    ";
                temFiltro = true;
            }
            else if (!string.IsNullOrEmpty(nomeCli))
            {
                var ids = ClienteDAO.Instance.GetIds(session, null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCli + "    ";
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                sql += " And l.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(session, idLoja);
                temFiltro = true;
            }

            if (idFunc > 0)
            {
                sql += " And p.idFunc=" + idFunc;
                criterio += "Vendedor (Assoc. Pedido): " + FuncionarioDAO.Instance.GetNome(session, idFunc) + "    ";
                temFiltro = true;
            }

            if (idFuncionarioConferente > 0)
            {
                sql += " And pe.idFuncConf=" + idFuncionarioConferente;
                criterio += "Conferente: " + FuncionarioDAO.Instance.GetNome(session, idFuncionarioConferente) + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(codCliente))
            {
                sql += " and p.codCliente like ?codCliente";
                criterio += "Cód. Cliente: " + codCliente + "    ";
                temFiltro = true;
            }

            if (situacao > 0)
            {
                sql += " And pe.situacao=" + situacao;
                criterio += "Situação PCP: " + (situacao == 1 ? "Aberto" : situacao == 2 ? "Finalizado" : situacao == 3 ? "Impresso" : situacao == 4 ? "Impresso Comum" : "N/A") + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(situacaoPedOri))
            {
                sql += " And p.situacao In (" + situacaoPedOri + ")";
                criterio += "Situação: " + PedidoDAO.Instance.GetSituacaoPedido(situacaoPedOri) + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(idsProcesso))
            {
                sql += " And p.idPedido In (Select idPedido From produtos_pedido_espelho Where idProcesso In (" + idsProcesso + "))";
                criterio += "Processo(s): " + EtiquetaProcessoDAO.Instance.GetCodInternoByIds(session, idsProcesso) + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniEnt))
            {
                sql += " And p.dataEntrega>=?dataIniEnt";
                criterio += "Data Entrega: " + dataIniEnt + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataFimEnt))
            {
                sql += " And p.dataEntrega<=?dataFimEnt";

                if (!string.IsNullOrEmpty(dataIniEnt))
                    criterio = criterio.TrimEnd() + " até " + dataFimEnt + "    ";
                else
                    criterio += "Data Entrega: até " + dataFimEnt + "    ";

                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniFab))
            {
                sql += " And pe.dataFabrica>=?dataIniFab";
                criterio += "Data Fábrica: " + dataIniFab + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataFimFab))
            {
                sql += " And pe.dataFabrica<=?dataFimFab";

                if (!string.IsNullOrEmpty(dataIniFab))
                    criterio = criterio.TrimEnd() + " até " + dataFimFab + "    ";
                else
                    criterio += "Data Fábrica: até " + dataFimFab + "    ";

                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniFin))
            {
                sql += " And pe.dataConf>=?dataIniFin";
                criterio += "Data Finalização: " + dataIniFin + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataFimFin))
            {
                sql += " And pe.dataConf<=?dataFimFin";

                if (!string.IsNullOrEmpty(dataIniFin))
                    criterio = criterio.TrimEnd() + " até " + dataFimFin + "    ";
                else
                    criterio += "Data Finalização: até " + dataFimFin + "    ";

                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniEmis))
            {
                sql += " And p.dataPedido>=?dataIniEmis";
                criterio += "Data Emissão Ped.: " + dataIniEmis + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataFimEmis))
            {
                sql += " And p.dataPedido<=?dataFimEmis";

                if (!string.IsNullOrEmpty(dataIniEmis))
                    criterio = criterio.TrimEnd() + " até " + dataFimEmis + "    ";
                else
                    criterio += "Data Emissão Ped. até " + dataFimEmis + "    ";

                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniConf))
            {
                sql += " And pe.dataEspelho>=?dataIniConf";
                criterio += "Data Conf.: " + dataIniConf + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataFimConf))
            {
                sql += " And pe.dataEspelho<=?dataFimConf";

                if (!string.IsNullOrEmpty(dataIniConf))
                    criterio = criterio.TrimEnd() + " até " + dataFimConf + "    ";
                else
                    criterio += "Data Conf.: até " + dataFimConf + "    ";

                temFiltro = true;
            }

            if (origemPedido > 0)
            {
                switch (origemPedido)
                {
                    case 1:
                        sql += " AND COALESCE(Importado, 0) = 0 AND COALESCE(GeradoParceiro, 0) = 0";
                        temFiltro = true;
                        break;

                    case 2:
                        sql += " AND COALESCE(GeradoParceiro, 0) = 1";
                        temFiltro = true;
                        break;

                    case 3:
                        sql += " AND COALESCE(Importado, 0) = 1";
                        temFiltro = true;
                        break;
                }
            }

            if (soFinalizados)
            {
                sql += " and (pe.Situacao=" + (int)PedidoEspelho.SituacaoPedido.Finalizado + " Or pe.Situacao=" +
                    (int)PedidoEspelho.SituacaoPedido.Impresso + ")";

                temFiltro = true;
            }

            if (apenasMaoDeObra)
            {
                sql += " And p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra;
                criterio += "Apenas pedidos de mão-de-obra    ";
                temFiltro = true;
            }

            if (pedidosSemAnexos)
            {
                sql += " and pe.idPedido not in (Select idPedido from fotos_pedido Where 1)";
                criterio += "Pedidos sem anexos    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(situacaoCnc))
            {
                sql += " AND situacaoCnc IN(" + situacaoCnc + ")";
                var criStr = "Situação CNC: ";
                foreach (var item in situacaoCnc.Split(','))
                {
                    var pedEsp = new PedidoEspelho()
                    {
                        SituacaoCnc = item.StrParaInt()
                    };
                    criStr += pedEsp.DescrSituacaoCnc + ", ";
                }
                criterio += criStr.Trim().Trim(',') + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniSituacaoCnc))
            {
                sql += " AND dataProjetoCnc >=?dataIniSituacaoCnc";
                criterio += "Período situação Proj. CNC: " + dataIniSituacaoCnc;
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataFimSituacaoCnc))
            {
                sql += " AND dataProjetoCnc <=?dataFimSituacaoCnc";

                if (!string.IsNullOrEmpty(dataFimSituacaoCnc))
                    criterio = criterio.TrimEnd() + " até " + dataFimSituacaoCnc + "    ";
                else
                    criterio += "Período situação Proj. CNC até: " + dataFimSituacaoCnc + "    ";

                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(tipoPedido))
            {
                sql += " and p.tipoPedido in (" + tipoPedido + ")";
                temFiltro = true;
                criterio += "Tipo Pedido: " + DataSources.Instance.GetDescrTipoPedido(tipoPedido) + "    ";
            }

            if (!string.IsNullOrEmpty(idsRotas))
            {
                sql += " AND EXISTS (SELECT * FROM rota_cliente WHERE idcliente=c.id_cli AND idRota IN (" + idsRotas + "))";
                temFiltro = true;

                var descRotas = string.Join(", ", RotaDAO.Instance.GetRptRota(session)
                    .Where(f => ("," + idsRotas + ",").Contains("," + f.IdRota.ToString() + ","))
                    .Select(f => f.CodInterno + " - " + f.Descricao).ToArray());

                criterio += "Rota: " + descRotas + "   ";
            }

            if (!string.IsNullOrEmpty(dtCompraIni))
            {
                sql += " AND pe.IdPedido IN (SELECT IdPedido FROM pedidos_compra WHERE IdCompra IN (SELECT IdCompra FROM compra WHERE DataCad>=?dtCompraIni))";
                temFiltro = true;
                criterio += "Data da Compra Inicial: " + dtCompraFim + "   ";
            }

            if (!string.IsNullOrEmpty(dtCompraFim))
            {
                sql += " AND pe.IdPedido IN (SELECT IdPedido FROM pedidos_compra WHERE IdCompra IN (SELECT IdCompra FROM compra WHERE DataCad<=?dtCompraFim))";
                temFiltro = true;
                criterio += "Data da Compra Final: " + dtCompraFim + "   ";
            }

            if (idCompra > 0)
            {
                sql += string.Format(" AND pe.IdPedido IN (SELECT IdPedido FROM pedidos_compra WHERE IdCompra={0})", idCompra);
                temFiltro = true;
                criterio += "Compra: " + idCompra + "   ";
            }

            if (pedidosConferidos > 0)
            {
                sql += " AND p.Importado = 1 AND pe.PedidoConferido="+ (pedidosConferidos == 1 ? "true" : "false") ;
                temFiltro = true;
                criterio += "Pedidos importados " + (pedidosConferidos == 1 ? "Conferidos" : "Não Conferidos") + "   ";
            }

            if (selecionar)
            {
                sql += " GROUP BY pe.IdPedido";

                if (pedidosAComprar)
                {
                    sql += " having temProdutosComprar=true";
                    criterio += "Pedidos com produtos a comprar    ";
                    temFiltro = true;
                }
            }
            else
            {
                if (pedidosAComprar)
                {
                    sql += " and (" + temProdutosComprar + ") > 0";

                    criterio += "Pedidos com produtos a comprar    ";
                    temFiltro = true;
                }
            }

            return sql.Replace("$$$", criterio);
        }

        public PedidoEspelho GetElement(uint idPedido)
        {
            return GetElement(null, idPedido);
        }

        public PedidoEspelho GetElement(GDASession sessao, uint idPedido)
        {
            bool temFiltro;
            var lstPedEsp = objPersistence.LoadData(sessao, Sql(sessao, idPedido, null, 0, string.Empty, 0, 0, 0, 0, null, null, null, null, null, null,
                null, null, null, null, null, null, false, false, null, false, false, null, null, null, null, null, null, null, 0, true, true, out temFiltro)).ToList();
            var pedEspelho = lstPedEsp.Count > 0 ? lstPedEsp[0] : new PedidoEspelho();

            // Se for à Prazo e tiver recebido sinal
            if (pedEspelho.TipoVenda == 2 && pedEspelho.RecebeuSinal)
            {
                CaixaDiario caixa = CaixaDiarioDAO.Instance.GetPedidoSinal(sessao, idPedido);

                pedEspelho.ConfirmouRecebeuSinal = "R$ " + pedEspelho.ValorEntrada.ToString("F2") + ", recebido por " + BibliotecaTexto.GetTwoFirstNames(caixa.DescrUsuCad) + " em " + caixa.DataCad.ToString("dd/MM/yy") + ". ";
            }

            return pedEspelho;
        }

        public IList<PedidoEspelho> GetForRpt(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis, string dataFimEmis, bool soFinalizados,
            string idsPedidos, bool pedidosSemAnexos, bool pedidosAComprar, string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc,
            string tipoPedido, string idsRotas, int origemPedido, int pedidosConferidos, LoginUsuario login)
        {
            string filtro = " Order By p.DataCad Desc";

            bool apenasMaoDeObra = !Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas) && Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra);

            bool temFiltro;
            return objPersistence.LoadData(Sql(idPedido, null, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao, situacaoPedOri,
                idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin, dataIniConf, dataFimConf, dataIniEmis, dataFimEmis,
                soFinalizados, apenasMaoDeObra, idsPedidos, pedidosSemAnexos, pedidosAComprar, situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc,
                tipoPedido, idsRotas, null, null, 0, false, origemPedido, pedidosConferidos,true, out temFiltro) + filtro,
                GetParam(null, nomeCli, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin, dataIniConf, dataFimConf, dataIniEmis,
                    dataFimEmis, dataIniSituacaoCnc, dataFimSituacaoCnc, null, null)).ToList();
        }

        public IList<uint> GetIdsForRpt(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente, int situacao,
         string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab, string dataIniFin,
         string dataFimFin, string dataIniConf, string dataFimConf, bool soFinalizados, bool pedidosSemAnexos, bool pedidosAComprar, string idsPedidos,
         string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, string tipoPedido, string idsRotas, string dtCompraIni,
         string dtCompraFim, int idCompra)
        {
            return GetIdsForRpt(idPedido, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao,
             situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin,
             dataFimFin, dataIniConf, dataFimConf, soFinalizados, pedidosSemAnexos, pedidosAComprar, idsPedidos,
             situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, tipoPedido, idsRotas, dtCompraIni,
             dtCompraFim, idCompra, 0,0);
        }

        public IList<uint> GetIdsForRpt(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente, int situacao,
            string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab, string dataIniFin,
            string dataFimFin, string dataIniConf, string dataFimConf, bool soFinalizados, bool pedidosSemAnexos, bool pedidosAComprar, string idsPedidos,
            string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, string tipoPedido, string idsRotas, string dtCompraIni,
            string dtCompraFim, int idCompra, int origemPedido, int pedidosConferidos)
        {
            if (!String.IsNullOrEmpty(idsPedidos))
                return Array.ConvertAll(idsPedidos.Split(','), delegate (string s)
                {
                    uint temp;
                    return uint.TryParse(s, out temp) ? temp : 0;
                });

            bool temFiltro;
            return objPersistence.LoadResult(Sql(idPedido, null, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao, situacaoPedOri,
                idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin, dataIniConf, dataFimConf, null, null, soFinalizados,
                false, null, pedidosSemAnexos, pedidosAComprar, situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, tipoPedido, idsRotas, dtCompraIni,
                dtCompraFim, idCompra, false, origemPedido, 0, true, out temFiltro), GetParam(null, nomeCli, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin,
                    dataFimFin, dataIniConf, dataFimConf, null, null, dataIniSituacaoCnc, dataFimSituacaoCnc, dtCompraIni, dtCompraFim
                )).Select(f => f.GetUInt32(0)).ToList();
        }
        public IList<PedidoEspelho> GetList(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis, string dataFimEmis, bool soFinalizados,
            bool pedidosSemAnexo, string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, bool pedidosAComprar, string tipoPedido,
            string idsRotas, string sortExpression, int startRow, int pageSize)
        {
            return GetList(idPedido, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente,
             situacao, situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab,
             dataIniFin, dataFimFin, dataIniConf, dataFimConf, dataIniEmis, dataFimEmis, soFinalizados,
             pedidosSemAnexo, situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, pedidosAComprar, tipoPedido,
             idsRotas, 0, 0, sortExpression, startRow, pageSize);
        }

        public IList<PedidoEspelho> GetList(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis, string dataFimEmis, bool soFinalizados,
            bool pedidosSemAnexo, string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, bool pedidosAComprar, string tipoPedido,
            string idsRotas, int origemPedido, int pedidosConferidos, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "pe.idPedido Desc" : sortExpression;

            bool apenasMaoDeObra = !Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas) &&
                Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra);

            bool temFiltro;
            string sql = Sql(idPedido, null, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao, situacaoPedOri, idsProcesso, dataIniEnt,
                dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin, dataIniConf, dataFimConf, dataIniEmis, dataFimEmis, soFinalizados,
                apenasMaoDeObra, null, pedidosSemAnexo, pedidosAComprar, situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, tipoPedido, idsRotas,
                null, null, 0, false, origemPedido, pedidosConferidos, true, out temFiltro);

            var pedidos = LoadDataWithSortExpression(sql, filtro, startRow, pageSize, temFiltro, GetParam(null, nomeCli, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab,
                dataIniFin, dataFimFin, dataIniConf, dataFimConf, dataIniEmis, dataFimEmis, dataIniSituacaoCnc, dataFimSituacaoCnc, null, null));

            foreach (var pedido in pedidos)
            {
                if (pedido.TipoPedido.HasValue && pedido.TipoPedido == Pedido.TipoPedidoEnum.MaoDeObra)
                    pedido.QtdePecas = ((long?)objPersistence.ExecuteScalar(
                        string.Format(@"
                        SELECT CAST(SUM(COALESCE(ape.qtde, 0)) AS SIGNED INTEGER)
                        FROM ambiente_pedido_espelho ape
                            INNER JOIN produto p ON(ape.idProd = p.idProd)
                        WHERE ape.idPedido = {0} AND p.idGrupoProd = {1}",
                            pedido.IdPedido, (int)NomeGrupoProd.Vidro))).GetValueOrDefault();
                else
                    pedido.QtdePecas = ((long?)objPersistence.ExecuteScalar(
                        string.Format(@"
                        SELECT CAST(SUM(COALESCE(ppe.qtde, 0)) AS SIGNED INTEGER)
                        FROM produtos_pedido_espelho ppe
                            INNER JOIN produto p ON (ppe.idProd = p.idProd)
                        WHERE !COALESCE(invisivelFluxo, FALSE) AND ppe.idPedido = {0} AND p.idGrupoProd = {1} AND ppe.IdProdPedParent IS NULL",
                            pedido.IdPedido, (int)NomeGrupoProd.Vidro))).GetValueOrDefault();
            }

            return pedidos;
        }

        public int GetCount(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente, int situacao,
            string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab, string dataIniFin,
            string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis, string dataFimEmis, bool soFinalizados, bool pedidosSemAnexo,
            string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, bool pedidosAComprar, string tipoPedido, string idsRotas)
        {
            return GetCount(idPedido, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao,
                 situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin,
                 dataFimFin, dataIniConf, dataFimConf, dataIniEmis, dataFimEmis, soFinalizados, pedidosSemAnexo,
                 situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, pedidosAComprar, tipoPedido, idsRotas, 0, 0);
        }

        public int GetCount(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente, int situacao,
            string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab, string dataIniFin,
            string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis, string dataFimEmis, bool soFinalizados, bool pedidosSemAnexo,
            string situacaoCnc, string dataIniSituacaoCnc, string dataFimSituacaoCnc, bool pedidosAComprar, string tipoPedido, string idsRotas, int origemPedido, int pedidosConferidos)
        {
            bool apenasMaoDeObra = !Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas) && Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra);

            bool temFiltro;
            string sql = Sql(idPedido, null, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao, situacaoPedOri, idsProcesso, dataIniEnt,
                dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin, dataIniConf, dataFimConf, dataIniEmis, dataFimEmis, soFinalizados,
                apenasMaoDeObra, null, pedidosSemAnexo, pedidosAComprar, situacaoCnc, dataIniSituacaoCnc, dataFimSituacaoCnc, tipoPedido, idsRotas,
                null, null, 0, false, origemPedido, pedidosConferidos, true, out temFiltro);

            return GetCountWithInfoPaging(sql, temFiltro, null, GetParam(null, nomeCli, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin,
                dataIniConf, dataFimConf, dataIniEmis, dataFimEmis, dataIniSituacaoCnc, dataFimSituacaoCnc, null, null));
        }

        public IList<PedidoEspelho> GetListSel(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, bool soFinalizados, bool pedidosSemAnexo, bool pedidosAComprar, string situacaoCnc,
            string dataIniSituacaoCnc, string dataFimSituacaoCnc, string idsRotas, string sortExpression, int startRow, int pageSize)
        {
            return GetListSel(idPedido, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente,
             situacao, situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt, dataIniFab, dataFimFab,
             dataIniFin, dataFimFin, soFinalizados, pedidosSemAnexo, pedidosAComprar, situacaoCnc,
             dataIniSituacaoCnc, dataFimSituacaoCnc, idsRotas, 0, 0, sortExpression, startRow, pageSize);
        }

        public IList<PedidoEspelho> GetListSel(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente,
            int situacao, string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab,
            string dataIniFin, string dataFimFin, bool soFinalizados, bool pedidosSemAnexo, bool pedidosAComprar, string situacaoCnc,
            string dataIniSituacaoCnc, string dataFimSituacaoCnc, string idsRotas, int origemPedido, int pedidosConferidos, string sortExpression, int startRow, int pageSize)
        {
            return GetList(idPedido, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao, situacaoPedOri, idsProcesso, dataIniEnt, dataFimEnt,
                dataIniFab, dataFimFab, dataIniFin, dataFimFin, null, null, null, null, soFinalizados, pedidosSemAnexo, situacaoCnc, dataIniSituacaoCnc,
                dataFimSituacaoCnc, pedidosAComprar, null, idsRotas, origemPedido, pedidosConferidos, sortExpression, startRow, pageSize);
        }

        public int GetCountSel(uint idPedido, uint idCli, string nomeCli, uint idLoja, uint idFunc, uint idFuncionarioConferente, int situacao,
            string situacaoPedOri, string idsProcesso, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab, string dataIniFin,
            string dataFimFin, bool soFinalizados, bool pedidosSemAnexo, bool pedidosAComprar, string situacaoCnc, string dataIniSituacaoCnc,
            string dataFimSituacaoCnc, string idsRotas)
        {
            return GetCount(idPedido, idCli, nomeCli, idLoja, idFunc, idFuncionarioConferente, situacao, situacaoPedOri, idsProcesso, dataIniEnt,
                dataFimEnt, dataIniFab, dataFimFab, dataIniFin, dataFimFin, null, null, null, null, soFinalizados, pedidosSemAnexo, situacaoCnc,
                dataIniSituacaoCnc, dataFimSituacaoCnc, pedidosAComprar, null, idsRotas);
        }

        public IList<PedidoEspelho> GetByIds(string idsPedidos)
        {
            bool temFiltro;
            return objPersistence.LoadData(Sql(0, null, 0, null, 0, 0, 0, 0, null, null, null, null, null, null, null, null, null, null, null, null, false,
                false, idsPedidos, false, false, null, null, null, null, null, null, null, 0, true, true, out temFiltro)).ToList();
        }

        private GDAParameter[] GetParam(string codCliente, string nomeCli, string dataIniEnt, string dataFimEnt, string dataIniFab, string dataFimFab, string dataIniFin,
            string dataFimFin, string dataIniConf, string dataFimConf, string dataIniEmis, string dataFimEmis, string dataIniSituacaoCnc, string dataFimSituacaoCnc,
            string dtCompraIni, string dtCompraFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codCliente))
                lstParam.Add(new GDAParameter("?codCliente", "%" + codCliente + "%"));

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(dataIniEnt))
                lstParam.Add(new GDAParameter("?dataIniEnt", DateTime.Parse(dataIniEnt + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimEnt))
                lstParam.Add(new GDAParameter("?dataFimEnt", DateTime.Parse(dataFimEnt + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniFab))
                lstParam.Add(new GDAParameter("?dataIniFab", DateTime.Parse(dataIniFab + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimFab))
                lstParam.Add(new GDAParameter("?dataFimFab", DateTime.Parse(dataFimFab + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniFin))
                lstParam.Add(new GDAParameter("?dataIniFin", DateTime.Parse(dataIniFin + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimFin))
                lstParam.Add(new GDAParameter("?dataFimFin", DateTime.Parse(dataFimFin + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniConf))
                lstParam.Add(new GDAParameter("?dataIniConf", DateTime.Parse(dataIniConf + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimConf))
                lstParam.Add(new GDAParameter("?dataFimConf", DateTime.Parse(dataFimConf + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniEmis))
                lstParam.Add(new GDAParameter("?dataIniEmis", DateTime.Parse(dataIniEmis + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimEmis))
                lstParam.Add(new GDAParameter("?dataFimEmis", DateTime.Parse(dataFimEmis + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniSituacaoCnc))
                lstParam.Add(new GDAParameter("?dataIniSituacaoCnc", DateTime.Parse(dataIniSituacaoCnc + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimSituacaoCnc))
                lstParam.Add(new GDAParameter("?dataFimSituacaoCnc", DateTime.Parse(dataFimSituacaoCnc + " 23:59")));

            if (!String.IsNullOrEmpty(dtCompraIni))
                lstParam.Add(new GDAParameter("?dtCompraIni", DateTime.Parse(dtCompraIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dtCompraFim))
                lstParam.Add(new GDAParameter("?dtCompraFim", DateTime.Parse(dtCompraFim + " 23:59:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca pedidos com produtos de beneficiamento

        private string SqlCompraProdBenef(uint idPedido, uint idCli, string nomeCli, uint idLoja, int situacao, string situacaoPedOri,
            string dataIniFin, string dataFimFin, string idsRota, int compraGerada, int idCompra, string dtCompraIni, string dtCompraFim, string dtEntregaPedIni, string dtEntregaPedFim,
            bool selecionar, out bool temFiltro, out string where)
        {
            temFiltro = false;

            var campos = selecionar ? @"pe.*, p.total as TotalPedido, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente,
                p.idCli, Cast(p.idLoja As Unsigned) As IdLoja, Coalesce(pc.idCompra, 0) > 0 As GerouCompra, p.DataEntrega As DataEntrega,
                (Select Group_Concat(pc1.idCompra) From pedidos_compra pc1 where pc1.idPedido=pe.idPedido) As CompraGerada,                
                '$$$' As criterio" :
                "Count(*) as Contador";

            var criterio = String.Empty;

            var sql = @"
                Select " + campos + @" From pedido_espelho pe
                    Left Join pedido p ON (pe.idPedido=p.idPedido)
                    Left Join cliente c ON (p.idCli=c.id_Cli)
                    Left Join pedidos_compra pc ON (p.idPedido=pc.idPedido)
                    Left Join compra comp ON (pc.idCompra=comp.idCompra)
                    Inner Join produtos_pedido_espelho ppe ON (pe.idPedido=ppe.idPedido)
                    Inner Join produto_pedido_espelho_benef ppeb ON (ppe.idProdPed=ppeb.idProdPed)
                    Inner Join benef_config bc ON (ppeb.idBenefConfig=bc.idBenefConfig)
                Where 1 {0}";

            where = " And Coalesce(bc.idProd, 0)>0";

            if (idPedido > 0)
            {
                where += " And p.IdPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }

            if (idCli > 0)
            {
                where += " And p.IdCli=" + idCli;
                criterio += " Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                where += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCli + "    ";
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                where += " And p.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja);
                temFiltro = true;
            }

            if (situacao > 0)
            {
                where += " And pe.situacao=" + situacao;
                criterio += "Situação PCP: " + (situacao == 1 ? "Aberto" : situacao == 2 ? "Finalizado" : situacao == 3 ? "Impresso" : situacao == 4 ? "Impresso Comum" : "N/A") + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(situacaoPedOri))
            {
                where += " And p.situacao In (" + situacaoPedOri + ")";
                criterio += "Situação: " + PedidoDAO.Instance.GetSituacaoPedido(situacaoPedOri) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIniFin))
            {
                where += " And pe.dataConf>=?dataIniFin";
                criterio += "Data Finalização: " + dataIniFin + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFimFin))
            {
                where += " And pe.dataConf<=?dataFimFin";

                if (!String.IsNullOrEmpty(dataIniFin))
                    criterio = criterio.TrimEnd() + " até " + dataFimFin + "    ";
                else
                    criterio += "Data Finalização: até " + dataFimFin + "    ";

                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(idsRota))
            {
                where += " AND EXISTS (SELECT * FROM rota_cliente WHERE idcliente=c.id_cli AND idRota IN (" + idsRota + "))";
                temFiltro = true;

                var descrRotas = string.Join(", ", RotaDAO.Instance.GetRptRota()
                    .Where(f => ("," + idsRota + ",").Contains("," + f.IdRota.ToString() + ","))
                    .Select(f => f.CodInterno + " - " + f.Descricao).ToArray());

                criterio += "Rota: " + descrRotas + "   ";
            }

            if (compraGerada > 0)
            {
                switch (compraGerada)
                {
                    case 1:
                        where += " And Coalesce(comp.idCompra, 0)>0 And Coalesce(comp.situacao, 0)<>" + (int)Compra.SituacaoEnum.Cancelada;
                        criterio += "Somente pedidos com compra gerada    ";
                        break;
                    case 2:
                        where += " And (Coalesce(comp.idCompra, 0)=0 Or (Coalesce(comp.idCompra, 0)>0 And comp.situacao=" + (int)Compra.SituacaoEnum.Cancelada + "))";
                        criterio += "Somente pedidos sem compra gerada    ";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(dtCompraIni))
            {
                where += " AND comp.dataCad >= ?dtCompraIni";
                criterio += "Data da Compra Inicial: " + dtCompraFim + "   ";
            }

            if (!string.IsNullOrEmpty(dtCompraFim))
            {
                where += " AND comp.dataCad <= ?dtCompraFim";
                criterio += "Data da Compra Final: " + dtCompraFim + "   ";
            }

            if (!string.IsNullOrEmpty(dtEntregaPedIni))
            {
                where += " AND p.DataEntrega >= ?dtEntregaPedIni";
                criterio += "Data da entrega inicial: " + dtEntregaPedIni + "   ";
            }

            if (!string.IsNullOrEmpty(dtEntregaPedFim))
            {
                where += " AND p.DataEntrega <= ?dtEntregaPedFim";
                criterio += "Data da entregada final: " + dtEntregaPedFim + "   ";
            }

            if (idCompra > 0)
            {
                where += " AND comp.IdCompra =" + idCompra;
                criterio += "Compra: " + idCompra + "   ";
            }

            sql = String.Format(sql.Replace("$$$", criterio), where) + " Group By pe.idPedido";

            if (!selecionar)
                sql = "Select Count(*) From (" + sql + ") as tbl";

            return sql;
        }

        public IList<PedidoEspelho> GetListCompraProdBenefSel(uint idPedido, uint idCli, string nomeCli, uint idLoja, int situacao, string situacaoPedOri,
            string dataIniFin, string dataFimFin, string idsRota, int compraGerada, int idCompra, string dtCompraIni, string dtCompraFim, string dtEntregaPedIni, string dtEntregaPedFim,
            int ordenarPor, string sortExpression, int startRow, int pageSize)
        {
            var temFiltro = false;
            var sort = ordenarPor == 1 ? "DataEntrega" : sortExpression;
            var where = String.Empty;
            var sql = SqlCompraProdBenef(idPedido, idCli, nomeCli, idLoja, situacao, situacaoPedOri, dataIniFin, dataFimFin, idsRota,
                compraGerada, idCompra, dtCompraIni, dtCompraFim, dtEntregaPedIni, dtEntregaPedFim, true, out temFiltro, out where);
            var lstPedidoEspelho = (IList<PedidoEspelho>)objPersistence.LoadDataWithSortExpression(sql, new InfoSortExpression(sort),
                new InfoPaging(startRow, pageSize), GetParamCompraProdBenef(nomeCli, dataIniFin, dataFimFin, dtCompraIni, dtCompraFim, dtEntregaPedIni, dtEntregaPedFim)).ToList();

            for (var i = 0; i < lstPedidoEspelho.Count; i++)
            {
                lstPedidoEspelho[i].Obs = (i + 1).ToString();
                lstPedidoEspelho[i].ProdutosBenefCompra = ProdutosPedidoEspelhoDAO.Instance.GetListCompraProdBenef(lstPedidoEspelho[i].IdPedido.ToString(),
                    0, null, 0, 0, null, null, null, null, ordenarPor == 1 ? "DataEntrega" : null, 0, 0).Where(f => f.IdPedido == lstPedidoEspelho[i].IdPedido).ToList();

                // Na Modelo é necessário exibir a profundidade da caixa de acordo com a descrição do beneficiamento.
                if (MenuConfig.ExibirCompraCaixa)
                {
                    // Salva a descricao do produto e a profundidade da caixa utilizada.
                    foreach (ProdutosPedidoEspelho ppe in lstPedidoEspelho[i].ProdutosBenefCompra)
                    {
                        var descrProdBenef = ppe.DescrProdutoBenef;

                        if (String.IsNullOrEmpty(descrProdBenef) || descrProdBenef.IndexOf("MM", StringComparison.Ordinal) < 3)
                            continue;

                        ppe.DescrProdutoBenef = descrProdBenef.Remove(descrProdBenef.IndexOf("MM", StringComparison.Ordinal) - 3);
                        ppe.ProfundidadeCaixa = descrProdBenef.Substring(descrProdBenef.IndexOf("MM", StringComparison.Ordinal) - 2);
                    }
                }
            }

            // Recupera todos os pedidos filtrados na tela.
            return lstPedidoEspelho;
        }

        public int GetCountCompraProdBenefSel(uint idPedido, uint idCli, string nomeCli, uint idLoja, int situacao,
            string situacaoPedOri, string dataIniFin, string dataFimFin, string idsRota, int compraGerada, int idCompra, string dtCompraIni, string dtCompraFim,
            string dtEntregaPedIni, string dtEntregaPedFim, int ordenarPor)
        {
            var temFiltro = false;
            var where = String.Empty;
            var sql = SqlCompraProdBenef(idPedido, idCli, nomeCli, idLoja, situacao, situacaoPedOri, dataIniFin, dataFimFin,
                idsRota, compraGerada, idCompra, dtCompraIni, dtCompraFim, dtEntregaPedIni, dtEntregaPedFim, false, out temFiltro, out where);

            return ExecuteScalar<int>(sql, GetParamCompraProdBenef(nomeCli, dataIniFin, dataFimFin, dtCompraIni, dtCompraFim, dtEntregaPedIni, dtEntregaPedFim));
        }

        private GDAParameter[] GetParamCompraProdBenef(string nomeCli, string dataIniFin, string dataFimFin, string dtCompraIni, string dtCompraFim, string dtEntregaPedIni, string dtEntregaPedFim)
        {
            var lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(dataIniFin))
                lstParam.Add(new GDAParameter("?dataIniFin", DateTime.Parse(dataIniFin + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFimFin))
                lstParam.Add(new GDAParameter("?dataFimFin", DateTime.Parse(dataFimFin + " 23:59:59")));

            if (!String.IsNullOrEmpty(dtCompraIni))
                lstParam.Add(new GDAParameter("?dtCompraIni", DateTime.Parse(dtCompraIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dtCompraFim))
                lstParam.Add(new GDAParameter("?dtCompraFim", DateTime.Parse(dtCompraFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(dtEntregaPedIni))
                lstParam.Add(new GDAParameter("?dtEntregaPedIni", DateTime.Parse(dtEntregaPedIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dtEntregaPedFim))
                lstParam.Add(new GDAParameter("?dtEntregaPedFim", DateTime.Parse(dtEntregaPedFim + " 23:59:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Gera Espelho

        /// <summary>
        /// Gera um espelho do pedido a partir de um pedido confirmado
        /// </summary>
        /// <param name="idPedido"></param>
        public void GeraEspelhoComTransacao(uint idPedido)
        {
            // Atualiza a reserva/liberação do produto somente se o web.config estiver configurado para alterar.
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    GeraEspelho(transaction, idPedido);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gera um espelho do pedido a partir de um pedido confirmado
        /// </summary>
        /// <param name="idPedido"></param>
        public void GeraEspelho(GDATransaction transaction, uint idPedido)
        {
            var lstImagensPcp = new List<string>();

            try
            {
                bool isMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(transaction, idPedido);

                // Verifica se o usuário possui permissão para gerar espelho de pedido
                if (UserInfo.GetUserInfo.IdCliente == null || UserInfo.GetUserInfo.IdCliente == 0)
                {
                    if (!Config.PossuiPermissao(Config.FuncaoMenuPCP.GerarConferenciaPedido))
                    {
                        if (Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra) && !isMaoDeObra)
                            throw new Exception("Você pode gerar conferência apenas de pedidos mão de obra.");
                        else
                            throw new Exception("Você não possui permissão para gerar Conferências de Pedidos.");
                    }
                }

                // Busca o pedido no qual será gerado o espelho
                Pedido ped = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, idPedido);

                if (ped == null)
                    throw new Exception("Este pedido não existe.");

                // Verifica se o pedido está liberado
                if (ped.Situacao == Pedido.SituacaoPedido.LiberadoParcialmente ||
                    (PedidoConfig.LiberarPedido && ped.Situacao == Pedido.SituacaoPedido.Confirmado && !PedidoDAO.Instance.IsPedidoReposicao(transaction, ped.IdPedido.ToString())))
                    throw new Exception("O pedido " + idPedido + " já foi liberado, cancele a liberação para gerar conferência.");

                // Verifica se o pedido está confirmado
                if (ped.Situacao != Pedido.SituacaoPedido.Confirmado && ped.Situacao != Pedido.SituacaoPedido.ConfirmadoLiberacao &&
                    ped.Situacao != Pedido.SituacaoPedido.Confirmado)
                    throw new Exception("O pedido " + idPedido + " ainda não foi confirmado.");

                // Verifica se o pedido já foi pago (se necessário)
                string mensagemErro;
                if (PedidoConfig.LiberarPedido && ped.TipoPedido != (int)Pedido.TipoPedidoEnum.Producao &&
                    ClienteDAO.Instance.IsPagamentoAntesProducao(transaction, ped.IdCli) &&
                        !PedidoDAO.Instance.VerificaSinalPagamentoReceber(transaction, ped, out mensagemErro))
                    throw new Exception("O pedido " + idPedido + " ainda não foi pago pelo cliente. " + mensagemErro);

                if (PedidoConfig.LiberarPedido && ped.TipoPedido == (int)Pedido.TipoPedidoEnum.Revenda && (PedidoConfig.DadosPedido.BloquearItensTipoPedido || !PCPConfig.PermitirGerarConferenciaPedidoRevenda))
                    throw new Exception("Não é permitido gerar conferência de pedidos de revenda.");

                if (ped.DataEntrega == null)
                    throw new Exception("A data de entrega do pedido deve ser preenchida.");

                // Verifica se já foi gerado um espelho para este pedido
                if (objPersistence.ExecuteScalar(transaction, "Select count(*) From pedido_espelho Where idPedido=" + idPedido).ToString().StrParaInt() > 0)
                    throw new Exception("Já foi gerada uma conferência para o pedido " + idPedido + ".");

                // Garante que não existe nenhum cálculo de projeto relacionado à este pedido (Estava ocorrendo de existir cálculos de projetos
                // com ids antigos "duplicados" nos pedidos espelho)
                string idsItemProjeto = ExecuteScalar<string>(transaction,
                    "Select Cast(group_concat(idItemProjeto) as char) From item_projeto Where idPedidoEspelho=" + idPedido);

                if (!String.IsNullOrEmpty(idsItemProjeto))
                {
                    objPersistence.ExecuteCommand(transaction, @"delete from medida_item_projeto where idItemProjeto in (?idsItemProjeto)", new GDAParameter("?idsItemProjeto", idsItemProjeto));
                    objPersistence.ExecuteCommand(transaction, @"delete from material_item_projeto where idItemProjeto in (?idsItemProjeto)", new GDAParameter("?idsItemProjeto", idsItemProjeto));
                    objPersistence.ExecuteCommand(transaction, @"delete from peca_item_projeto where idItemProjeto in (?idsItemProjeto)", new GDAParameter("?idsItemProjeto", idsItemProjeto));
                    objPersistence.ExecuteCommand(transaction, @"delete from item_projeto where idPedidoEspelho=" + idPedido);
                }

                // Pega a data de entrega da fábrica
                DateTime dataFabrica = CalculaDataFabrica(ped.DataEntrega.Value);

                // Insere o espelho
                PedidoEspelho pedEsp = new PedidoEspelho
                {
                    IdPedido = idPedido,
                    IdCli = ped.IdCli,
                    Situacao = (int)PedidoEspelho.SituacaoPedido.Aberto,
                    DataEspelho = DateTime.Now,
                    IdFuncConf = UserInfo.GetUserInfo.CodUser,
                    ValorIcms = ped.ValorIcms,
                    DataFabrica = dataFabrica,
                    IdProjeto = ped.IdProjeto,
                    IdComissionado = ped.IdComissionado,
                    PercComissao = ped.PercComissao,
                    ValorComissao = ped.ValorComissao,
                    ValorEntrega = ped.ValorEntrega,
                    PercentualRentabilidade = ped.PercentualRentabilidade,
                    RentabilidadeFinanceira = ped.RentabilidadeFinanceira
                };
                Insert(transaction, pedEsp);

                // Salva os registro da rentabilidade
                foreach (var registro in PedidoRentabilidadeDAO.Instance.ObterPorPedido(transaction, idPedido))
                    PedidoEspelhoRentabilidadeDAO.Instance.Insert(transaction, new PedidoEspelhoRentabilidade
                    {
                        IdPedido = (int)idPedido,
                        Tipo = registro.Tipo,
                        IdRegistro = registro.IdRegistro,
                        Valor = registro.Valor
                    });

                var lstProdPed = ProdutosPedidoDAO.Instance.GetByPedidoLite(transaction, idPedido).ToArray();

                // Variáveis que guardam os IDs dos novos ambientes e dos itens de projeto
                var ambientes = new Dictionary<uint?, uint>();
                var itensProjeto = new Dictionary<uint, uint>();
                var ambientesItemProjeto = new List<uint>();

                /* Chamado 50709. */
                var associacaoProdutosPedidoProdutosPedidoEspelho = new Dictionary<int, int>();

                // Gera os ambientes do pedido
                foreach (AmbientePedido a in (ped as IContainerCalculo).Ambientes.Obter().Cast<AmbientePedido>())
                {
                    // Se o produto for um cálculo de projeto, faz uma cópia para o pedido
                    if (a.IdItemProjeto.GetValueOrDefault() > 0 && !itensProjeto.ContainsKey(a.IdItemProjeto.GetValueOrDefault()))
                    {
                        uint idItemProjeto = ClonaItemProjeto(transaction, a.IdItemProjeto.Value, idPedido);
                        itensProjeto.Add(a.IdItemProjeto.Value, idItemProjeto);
                    }

                    var novo = new AmbientePedidoEspelho();
                    novo.Altura = a.Altura;
                    novo.Ambiente = a.Ambiente;
                    novo.Descricao = a.Descricao;
                    novo.IdAmbientePedidoOriginal = a.IdAmbientePedido;
                    novo.IdPedido = a.IdPedido;
                    novo.IdProd = a.IdProd;
                    novo.IdItemProjeto = a.IdItemProjeto != null && itensProjeto.ContainsKey(a.IdItemProjeto.Value) ? (uint?)itensProjeto[a.IdItemProjeto.Value] : null;
                    novo.Largura = a.Largura;
                    novo.Qtde = a.Qtde;
                    novo.QtdeImpresso = 0;
                    novo.Redondo = a.Redondo;
                    novo.TipoDesconto = a.TipoDesconto;
                    novo.Desconto = a.Desconto;
                    novo.TipoAcrescimo = a.TipoAcrescimo;
                    novo.Acrescimo = a.Acrescimo;
                    novo.IdAplicacao = a.IdAplicacao;
                    novo.IdProcesso = a.IdProcesso;
                    novo.PercentualRentabilidade = a.PercentualRentabilidade;
                    novo.RentabilidadeFinanceira = a.RentabilidadeFinanceira;

                    uint idNovo = AmbientePedidoEspelhoDAO.Instance.Insert(transaction, novo);
                    ambientes.Add(a.IdAmbientePedido, idNovo);

                    // Salva os registro da rentabilidade
                    foreach (var registro in AmbientePedidoRentabilidadeDAO.Instance.ObterPorAmbiente(transaction, a.IdAmbientePedido))
                        AmbientePedidoEspelhoRentabilidadeDAO.Instance.Insert(transaction, new AmbientePedidoEspelhoRentabilidade
                        {
                            IdAmbientePedido = (int)idNovo,
                            Tipo = registro.Tipo,
                            IdRegistro = registro.IdRegistro,
                            Valor = registro.Valor
                        });

                    if (novo.IdItemProjeto > 0)
                    {
                        // Atualiza os produtos do pedido original, indicando-os como invisíveis para o fluxo
                        objPersistence.ExecuteCommand(transaction, "update produtos_pedido set invisivelFluxo=true where idPedido=" + idPedido +
                            " and idAmbientePedido=" + a.IdAmbientePedido);

                        string where = "idAmbientePedido=" + a.IdAmbientePedido, whereBenef = "idProdPed in (select * from (select idProdPed from produtos_pedido " +
                            "where idAmbientePedido=" + a.IdAmbientePedido + ") as temp)";

                        decimal valorAcrescimoAplicado = ProdutosPedidoDAO.Instance.ObtemValorCampo<decimal>(transaction, "sum(valorAcrescimo)", where) +
                            ProdutoPedidoBenefDAO.Instance.ObtemValorCampo<decimal>(transaction, "sum(valorAcrescimo)", whereBenef);

                        decimal valorDescontoAplicado = ProdutosPedidoDAO.Instance.ObtemValorCampo<decimal>(transaction, "sum(valorDesconto)", where) +
                            ProdutoPedidoBenefDAO.Instance.ObtemValorCampo<decimal>(transaction, "sum(valorDesconto)", whereBenef);

                        ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElement(transaction, novo.IdItemProjeto.Value);
                        ProdutosPedidoEspelhoDAO.Instance.InsereAtualizaProdProj(transaction, pedEsp, idNovo, itemProj, valorAcrescimoAplicado,
                            valorDescontoAplicado, ped.PercComissao, false, true, associacaoProdutosPedidoProdutosPedidoEspelho);

                        ambientesItemProjeto.Add(a.IdAmbientePedido);
                    }
                }

                foreach (ProdutosPedido p in lstProdPed)
                {
                    // Verifica se o produto já foi inserido.
                    if (p.IdAmbientePedido > 0 && ambientesItemProjeto.Contains(p.IdAmbientePedido.Value))
                        continue;

                    // Recupera medidas reais do projeto (se houver)
                    int? pipAltura = null, pipLargura = null, mipLargura = null, pipQtde = null;
                    float? mipAlturaCalc = null, mipAltura = null, mipTotM = null, mipTotM2Calc = null;
                    uint? pipIdProd = null;
                    bool mipRedondo = false;
                    GenericBenefCollection mipBeneficiamentos = new GenericBenefCollection();

                    if (p.IdMaterItemProj > 0)
                    {
                        mipLargura = MaterialItemProjetoDAO.Instance.ObtemLargura(transaction, p.IdMaterItemProj.Value);
                        mipAlturaCalc = MaterialItemProjetoDAO.Instance.ObtemAlturaCalc(transaction, p.IdMaterItemProj.Value);
                        mipAltura = MaterialItemProjetoDAO.Instance.ObtemAltura(transaction, p.IdMaterItemProj.Value);
                        mipTotM = MaterialItemProjetoDAO.Instance.ObtemTotM(transaction, p.IdMaterItemProj.Value);
                        mipTotM2Calc = MaterialItemProjetoDAO.Instance.ObtemTotM2Calc(transaction, p.IdMaterItemProj.Value);
                        mipRedondo = MaterialItemProjetoDAO.Instance.ObtemRedondo(transaction, p.IdMaterItemProj.Value);
                        mipBeneficiamentos = MaterialProjetoBenefDAO.Instance.GetByMaterial(transaction, p.IdMaterItemProj.Value);

                        uint? mipIdPecaItemProj = MaterialItemProjetoDAO.Instance.ObtemIdPecaItemProj(transaction, p.IdMaterItemProj.Value);
                        if (mipIdPecaItemProj > 0)
                        {
                            pipAltura = PecaItemProjetoDAO.Instance.ObtemAltura(transaction, mipIdPecaItemProj.Value);
                            pipLargura = PecaItemProjetoDAO.Instance.ObtemLargura(transaction, mipIdPecaItemProj.Value);
                            pipIdProd = PecaItemProjetoDAO.Instance.ObtemIdProd(transaction, mipIdPecaItemProj.Value);
                            pipQtde = PecaItemProjetoDAO.Instance.ObtemQtde(transaction, mipIdPecaItemProj.Value);
                        }
                    }

                    var pe = new ProdutosPedidoEspelho();
                    pe.IdPedido = idPedido;
                    pe.IdProd = p.IdProd;
                    pe.IdAplicacao = p.IdAplicacao;
                    pe.IdProcesso = p.IdProcesso;
                    pe.IdNaturezaOperacao = p.IdNaturezaOperacao;
                    pe.IdItemProjeto = p.IdItemProjeto != null && itensProjeto.ContainsKey(p.IdItemProjeto.Value) ? itensProjeto[p.IdItemProjeto.Value] : p.IdItemProjeto;
                    pe.Qtde = p.Qtde;
                    pe.ValorVendido = p.ValorVendido;
                    pe.PercComissao = p.PercComissao;
                    pe.PercentualRentabilidade = p.PercentualRentabilidade;
                    pe.RentabilidadeFinanceira = p.RentabilidadeFinanceira;

                    // Busca medidas reais calculadas no projeto 
                    // (Apenas se tiver sido inserido projeto dentro do orçamento que gerou este pedido)
                    pe.Altura = pipAltura > 0 ? pipAltura.Value : mipAlturaCalc > 0 ? mipAlturaCalc.Value : p.Altura;
                    pe.AlturaReal = pipAltura > 0 ? pipAltura.Value : mipAltura > 0 ? mipAltura.Value : p.AlturaReal > 0 ? p.AlturaReal : p.Altura;
                    pe.Largura = pipLargura > 0 ? pipLargura.Value : mipLargura > 0 ? mipLargura.Value : p.Largura;

                    pe.TotM = pipAltura > 0 && pipLargura > 0
                        ? Glass.Global.CalculosFluxo.ArredondaM2(transaction, pipLargura.Value, pipAltura.Value, pe.Qtde, (int)pe.IdProd, pe.Redondo)
                        : mipTotM > 0
                            ? mipTotM.Value
                            : p.TotM;

                    pe.TotM2Calc = pipAltura > 0 && pipLargura > 0 && pipQtde > 0
                        ? Glass.Global.CalculosFluxo.CalcM2Calculo(transaction, ped.IdCli, pipAltura.Value, pipLargura.Value, pipQtde.Value, (int)pipIdProd.Value, mipRedondo, mipBeneficiamentos.CountAreaMinimaSession(transaction), ProdutoDAO.Instance.ObtemAreaMinima(transaction, (int)pipIdProd.Value), true, 0, true)
                        : mipTotM2Calc > 0
                            ? mipTotM2Calc.Value
                            : p.TotM2Calc;

                    pe.QtdImpresso = 0;
                    pe.Total = p.Total;
                    pe.Ambiente = p.Ambiente;
                    pe.IdAmbientePedido = p.IdAmbientePedido > 0 && (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra ||
                        PedidoConfig.DadosPedido.AmbientePedido || p.IdMaterItemProj > 0) && ambientes.ContainsKey(p.IdAmbientePedido) ? (uint?)ambientes[p.IdAmbientePedido] : null;
                    pe.PedCli = p.PedCli;
                    pe.AlturaBenef = p.AlturaBenef;
                    pe.LarguraBenef = p.LarguraBenef;
                    pe.EspessuraBenef = p.EspessuraBenef;
                    pe.Redondo = p.Redondo;
                    pe.Espessura = p.Espessura;
                    pe.ValorAcrescimo = p.ValorAcrescimo;
                    pe.ValorDesconto = p.ValorDesconto;
                    pe.ValorAcrescimoProd = p.ValorAcrescimoProd;
                    pe.ValorDescontoProd = p.ValorDescontoProd;
                    pe.AliqIcms = p.AliqIcms;
                    pe.ValorIcms = p.ValorIcms;
                    pe.AliqIpi = p.AliqIpi;
                    pe.ValorIpi = p.ValorIpi;
                    pe.PercDescontoQtde = p.PercDescontoQtde;
                    pe.ValorDescontoQtde = p.ValorDescontoQtde;
                    pe.Beneficiamentos = p.Beneficiamentos;
                    pe.ValorDescontoCliente = p.ValorDescontoCliente;
                    pe.ValorAcrescimoCliente = p.ValorAcrescimoCliente;
                    pe.ValorComissao = p.ValorComissao;
                    pe.ValorUnitarioBruto = p.ValorUnitarioBruto;
                    pe.TotalBruto = p.TotalBruto;
                    pe.IdProdBaixaEst = p.IdProdBaixaEst;
                    /* Chamado 24452. */
                    pe.Obs = string.Format("{0}{1}{2}", p.Obs, string.IsNullOrEmpty(p.ObsProjetoExterno) ? string.Empty : " ", p.ObsProjetoExterno);
                    pe.IdProdPedParent = ProdutosPedidoDAO.Instance.ObterIdProdPedEspParent(p.IdProdPedParent.GetValueOrDefault(0));

                    // Define se o pedido é mão de obra para que busque corretamente a QtdeAmbiente para que calcule coeretamente o valorUnitarioBruto
                    pe.PedidoMaoObra = isMaoDeObra;

                    uint idProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.InsertBase(transaction, pe, pedEsp);

                    // Salva os registro da rentabilidade
                    foreach (var registro in ProdutoPedidoRentabilidadeDAO.Instance.ObterPorProdutoPedido(transaction, p.IdProdPed))
                        ProdutoPedidoEspelhoRentabilidadeDAO.Instance.Insert(transaction, new ProdutoPedidoEspelhoRentabilidade
                        {
                            IdProdPed = (int)idProdPedEsp,
                            Tipo = registro.Tipo,
                            IdRegistro = registro.IdRegistro,
                            Valor = registro.Valor
                        });

                    /* Chamado 50709. */
                    associacaoProdutosPedidoProdutosPedidoEspelho.Add((int)p.IdProdPed, (int)idProdPedEsp);

                    // Atualiza o id do produto pedido espelho no produto pedido
                    objPersistence.ExecuteCommand(transaction, "update produtos_pedido set idProdPedEsp=" + idProdPedEsp + " where idProdPed=" + p.IdProdPed);

                    //Copia as imagens de vidros duplos ou laminados
                    var urlImagem = ProdutosPedidoDAO.Instance.ObterUrlImagemSalvar(p.IdProdPed);
                    if (File.Exists(urlImagem))
                    {
                        var caminhoImagemPCP = Utils.GetPecaProducaoPath + idProdPedEsp.ToString().PadLeft(10, '0') + "_0.jpg";
                        File.Copy(urlImagem, caminhoImagemPCP, true);
                    }

                }

                #region Valida quantidade item/material projeto

                /* Chamado 13840.
                 * O valor do pedido estava correto mas apenas dois materiais, de quatro, foram gerados, ao confirmar o projeto os materiais foram inclusos.*/

                // Recupera a quantidade de itens de projeto existentes no pedido original e no pedido espelho.
                var qtdItemProjPedOri = objPersistence.ExecuteSqlQueryCount(transaction, "SELECT COUNT(*) FROM item_projeto WHERE IdPedido=" + idPedido);
                var qtdItemProjPedEsp = objPersistence.ExecuteSqlQueryCount(transaction, "SELECT COUNT(*) FROM item_projeto WHERE IdPedidoEspelho=" + idPedido);

                // Caso a quantidade seja divergente então o pedido espelho foi gerado incorretamente e uma exceção deve ser lançada.
                if (qtdItemProjPedOri != qtdItemProjPedEsp)
                    throw new Exception("A quantidade de projetos do pedido espelho é diferente da quantidade de projetos do pedido original.");

                // Recupera a quantidade de materiais de itens de projeto existentes no pedido original e no pedido espelho.
                var qtdMaterItemProjPedOri = objPersistence.ExecuteSqlQueryCount(transaction, @"
                    SELECT COUNT(*) FROM material_item_projeto mip
                        INNER JOIN  item_projeto ip ON (mip.IdItemProjeto=ip.IdItemProjeto)
                    WHERE ip.IdPedido=" + idPedido);
                var qtdMaterItemProjPedEsp = objPersistence.ExecuteSqlQueryCount(transaction, @"
                    SELECT COUNT(*) FROM material_item_projeto mip
                        INNER JOIN  item_projeto ip ON (mip.IdItemProjeto=ip.IdItemProjeto)
                    WHERE ip.IdPedidoEspelho=" + idPedido);

                // Caso a quantidade seja divergente então o pedido espelho foi gerado incorretamente e uma exceção deve ser lançada.
                if (qtdMaterItemProjPedOri != qtdMaterItemProjPedEsp)
                {
                    // Pesquisa por materiais sem peças associadas
                    var ambiente = ExecuteMultipleScalar<string>(transaction, $@"
                        SELECT ip.Ambiente FROM material_item_projeto mip
	                        INNER JOIN item_projeto ip ON (mip.IdItemProjeto=ip.IdItemProjeto)
                        WHERE ip.IdPedido={ idPedido } 
                            AND idpecaitemproj not in (SELECT idpecaitemproj FROM peca_item_projeto)");

                    var msg = "A quantidade de materiais de projetos do pedido espelho é diferente da quantidade de projetos do pedido original. ";

                    if (ambiente.Count > 0)
                        msg += $"Projetos com problemas: { string.Join(", ", ambiente) }";

                    throw new Exception(msg);
                }

                #endregion

                #region Valida quantidade de produtos

                if (PCPConfig.CriarClone)
                {
                    /* Chamado 16328.
                     * O pedido espelho foi gerado com a quantidade de produtos correta, porém, nem todos os clones foram criados.*/

                    // Recupera a quantidade de produtos existentes no pedido original e no pedido espelho.
                    var qtdProdPedOri = objPersistence.ExecuteSqlQueryCount(transaction, "SELECT COUNT(*) FROM produtos_pedido WHERE IdPedido=" + idPedido +
                        " AND (InvisivelPedido IS NULL OR InvisivelPedido=FALSE)");
                    var qtdProdPedEsp = objPersistence.ExecuteSqlQueryCount(transaction, "SELECT COUNT(*) FROM produtos_pedido_espelho WHERE IdPedido=" + idPedido);

                    // Caso a quantidade seja divergente então o pedido espelho foi gerado incorretamente e uma exceção deve ser lançada.
                    if (qtdProdPedOri != qtdProdPedEsp)
                        throw new Exception("A quantidade de produtos do pedido espelho é diferente da quantidade de produtos do pedido original. Gere a conferência novamente.");
                }

                #endregion

                // No final deste método, já é feita a aplicação do acréscimo e desconto no pedido,
                // para que não seja feito por ambiente.
                objPersistence.ExecuteCommand(transaction, @"update pedido_espelho set tipoAcrescimo=?ta, acrescimo=?a, tipoDesconto=?td,
                    desconto=?d where idPedido=" + pedEsp.IdPedido, new GDAParameter("?ta", ped.TipoAcrescimo),
                    new GDAParameter("?a", ped.Acrescimo), new GDAParameter("?td", ped.TipoDesconto),
                    new GDAParameter("?d", ped.Desconto));

                // Aplica apenas o desconto, o acréscimo já está aplicado no valor do produto
                if (!PedidoConfig.RatearDescontoProdutos && ped.Desconto > 0)
                {
                    var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(transaction, pedEsp.IdPedido, false, false, true);
                    bool aplicado = AplicarDesconto(transaction, pedEsp, ped.TipoDesconto, ped.Desconto, produtosPedidoEspelho);
                    FinalizarAplicacaoComissaoAcrescimoDesconto(transaction, pedEsp, produtosPedidoEspelho, aplicado);
                }

                // Foi necessário marcar como true porque teve uma empresa que não estava atualizando o total, e o pedido espelho estava ficando
                // com valor 0 (zero) apesar de ter produto_pedido_espelho
                UpdateTotalPedido(transaction, pedEsp, true);

                #region Verifica se existe diferença de valor entre o pedido comercial e o pedido PCP

                // Coloca o pedido em aberto se o total do PCP coincidir com o total do pedido original (Margem de erro de R$0,50)
                // Teve que ser retirado para confirmação porque na vidrália aconteceu do pedido 162677 ter sido gerado PCP com um valor diferente
                // Teve que ser retirado da tempera de Vespasiano porque lá o pedido original tem dois valores, à vista e à prazo, porém na conferência
                // só vai o à vista (taxa à prazo).
                var totalOriginal = PedidoDAO.Instance.GetTotal(transaction, idPedido);
                var totalEspelho = ObtemValorCampo<decimal>(transaction, "total", "idPedido=" + idPedido);

                /* Chamado 49595. */
                // Tolerância de R$ 0,49 para mais ou menos de diferença entre o comercial e PCP.
                if (PedidoConfig.LiberarPedido && totalOriginal != totalEspelho &&
                    !(totalOriginal > totalEspelho - (decimal)0.5 && totalOriginal < totalEspelho + (decimal)0.5))
                {
                    // Recupera os produtos do pedido comercial.
                    var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(transaction, idPedido);
                    // Recupera os produtos do pedido PCP.
                    var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(transaction, idPedido, false);
                    // Variável criada para salvar os ambientes e produtos que estão com diferença de valor.
                    var mensagemAmbientesProdutos = new List<string>();

                    foreach (var associacao in associacaoProdutosPedidoProdutosPedidoEspelho)
                    {
                        /* Chamado 52547. */
                        if (produtosPedido.Where(f => f.IdProdPed == associacao.Key).ToList().Count == 0)
                            continue;

                        var produtoPedido = produtosPedido.Where(f => f.IdProdPed == associacao.Key).ToList()[0];
                        var produtoPedidoEspelho = produtosPedidoEspelho.Where(f => f.IdProdPed == associacao.Value).ToList()[0];

                        if (produtoPedido.Total != produtoPedidoEspelho.Total)
                        {
                            // Recupera a descrição do ambiente, caso ele exista.
                            var descricaoAmbientePedido = produtoPedido.IdAmbientePedido > 0 ?
                                    AmbientePedidoDAO.Instance.ObterDescricaoAmbiente(transaction, produtoPedido.IdAmbientePedido.Value) : string.Empty;

                            // Concatena o produto com diferença de valor e seu ambiente.
                            mensagemAmbientesProdutos.Add(string.Format("{0}Produto: {1}\nPed.: {2} - Ped. PCP: {3}",
                                string.IsNullOrEmpty(descricaoAmbientePedido) ? string.Empty : string.Format("Ambiente: {0} - ", descricaoAmbientePedido),
                                produtoPedido.CodInterno, produtoPedido.Total, produtoPedidoEspelho.Total));
                        }
                    }

                    // Informa na mensagem o valor do pedido comercial, o valor do pedido PCP, todos os produtos e ambientes com diferença de valor
                    // e informa o que deve ser feito para conseguirem gerar o PCP normalmente.
                    throw new Exception(string.Format("O valor total do pedido comercial ({0}) difere do valor total da conferência ({1}). {2}" +
                        "Reabra o pedido comercial e recalcule o(s) produto(s).",
                        totalOriginal.ToString("C"),
                        totalEspelho.ToString("C"),
                        mensagemAmbientesProdutos.Count > 0 ? string.Format("\n{0}\n", string.Join("\n", mensagemAmbientesProdutos)) : string.Empty));
                }

                #endregion

                // Copia as imagens de projeto que podem ter sido criadas no comercial, alterando o idProdPed do produtos_pedido para o
                // idProdPed de produtos_pedido_espelho, recém criado
                foreach (ItemProjeto ip in ItemProjetoDAO.Instance.GetByPedidoEspelho(transaction, idPedido))
                {
                    foreach (MaterialItemProjeto mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(transaction, ip.IdItemProjeto, false))
                    {
                        // Com base no campo IdMaterItemProjOrig, recupera o produtos_pedido associado ao mesmo, para verificar se 
                        // existe ou não alguma figura editada neste produto
                        uint idProdPed = ProdutosPedidoDAO.Instance.GetIdProdPedByMaterItemProj(transaction, idPedido, mip.IdMaterItemProjOrig.Value);

                        uint idProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetIdProdPedByMaterItemProj(transaction, idPedido, mip.IdMaterItemProj);

                        // Pega a peça associada à este produtos_pedido_espelho
                        PecaItemProjeto peca = PecaItemProjetoDAO.Instance.GetByProdPedEsp(transaction, idProdPedEsp);

                        if (peca == null)
                            continue;

                        foreach (string item in UtilsProjeto.GetItensFromPeca(peca.Item))
                        {
                            string caminhoImagemComercial = Utils.GetPecaComercialPath + idProdPed.ToString().PadLeft(10, '0') + "_" + item + ".jpg";
                            string caminhoImagemPCP = Utils.GetPecaProducaoPath + idProdPedEsp.ToString().PadLeft(10, '0') + "_" + item + ".jpg";

                            if (File.Exists(caminhoImagemComercial))
                            {
                                System.IO.File.Copy(caminhoImagemComercial, caminhoImagemPCP, true);
                                lstImagensPcp.Add(caminhoImagemPCP);
                            }
                        }

                        #region Copia o arquivo DXF do comercial para o pcp, se tiver sido editado

                        var caminhoComercial = PCPConfig.CaminhoSalvarCadProject(false) + idProdPed;
                        var caminhoPCP = PCPConfig.CaminhoSalvarCadProject(true) + idProdPedEsp;

                        //Copia o DXF
                        if (File.Exists(caminhoComercial + ".dxf"))
                            File.Copy(caminhoComercial + ".dxf", caminhoPCP + ".dxf");

                        //Copia o SVG
                        if (File.Exists(caminhoComercial + ".svg"))
                            File.Copy(caminhoComercial + ".svg", caminhoPCP + ".svg");

                        #endregion
                    }
                }

                VerificaImagensEditadas(transaction, idPedido);

                //Se o pedido for de produtos de composição finalizar ao gerar o espelho, pois a edição pode ser feita apenas no pedido
                if (PossuiProdutosComposicao(transaction, (int)idPedido))
                    FinalizarPedido(transaction, idPedido);
            }
            catch (Exception ex)
            {
                foreach (string img in lstImagensPcp)
                    File.Delete(img);

                // Após desfazer todas as alterações, o erro ocorrido ao gerar o pedido espelho deve ser salvo no banco de dados.
                ErroDAO.Instance.InserirFromException("Pedido: " + idPedido + " - CadPedidoEspelhoGerar.aspx Erro", ex);

                throw ex;
            }
        }

        // Calcula a data de fábrica do pedido
        public DateTime CalculaDataFabrica(DateTime dataEntrega)
        {
            // Pega a data de entrega da fábrica
            DateTime dataFabrica = dataEntrega;
            int diasUteis = PCPConfig.Etiqueta.DiasDataFabrica;

            for (int i = 0; i < diasUteis; i++)
            {
                dataFabrica = dataFabrica.AddDays(-1);

                while (!FuncoesData.DiaUtil(dataFabrica))
                    dataFabrica = dataFabrica.AddDays(-1);
            }
            
            /* Chamado 56812. */
            if (dataFabrica.Date < DateTime.Now.Date)
                do { dataFabrica = dataFabrica.AddDays(1); } while (!dataFabrica.DiaUtil() || dataFabrica.Date < DateTime.Now.Date);
            
            return dataFabrica;
        }

        /// <summary>
        /// Verifica se as imagens editadas no pedido original ainda existem no PCP
        /// </summary>
        /// <param name="idPedido"></param>
        private void VerificaImagensEditadas(GDASession sessao, uint idPedido)
        {
            // Garante que todas as imagens editadas no comercial foram inseridas
            int qtdImagensCOM = 0;
            int qtdImagensPCP = 0;
            if (ExecuteScalar<bool>(sessao, @"
                    Select Count(*) > 0 From peca_item_projeto 
                    Where imagemEditada
                        And idItemProjeto In (Select idItemProjeto From item_projeto Where idpedido=" + idPedido + ")"))
            {
                var idsProdPed = ProdutosPedidoDAO.Instance.ExecuteMultipleScalar<uint>(sessao, @"
                        Select idProdPed From produtos_pedido Where Coalesce(invisivelPedido, 0)=0 And idPedido=" + idPedido);

                var idsProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.ExecuteMultipleScalar<uint>(sessao, @"
                        Select idProdPed From produtos_pedido_espelho Where idPedido=" + idPedido);

                foreach (uint idProdPed in idsProdPed)
                    qtdImagensCOM += new DirectoryInfo(Utils.GetPecaComercialPath).GetFiles(
                        idProdPed.ToString().PadLeft(10, '0') + "_*.jpg").Length;

                foreach (uint idProdPedEsp in idsProdPedEsp)
                    qtdImagensPCP += new DirectoryInfo(Utils.GetPecaProducaoPath).GetFiles(
                        idProdPedEsp.ToString().PadLeft(10, '0') + "_*.jpg").Length;
            }

            // Se a quantidade de imagens editas no comercial for maior que a quantidade de imagens no PCP,
            // quer dizer que algumas podem não ter sido exportadas.
            if (qtdImagensCOM > qtdImagensPCP)
                throw new Exception("Falha ao copiar imagens alteradas dos projetos do comercial para o PCP, cancele a conferência e gere novamente.");
        }

        #endregion

        #region Comissão, Acréscimo e Desconto

        #region Comissão

        #region Aplica a comissão no valor dos produtos

        /// <summary>
        /// Aplica um percentual de comissão sobre o valor dos produtos do pedido.
        /// </summary>
        internal bool AplicarComissao(GDASession session, IContainerCalculo container, float percComissao,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            if (!PedidoConfig.Comissao.ComissaoAlteraValor)
                return false;

            return DescontoAcrescimo.Instance.AplicarComissao(session, container, percComissao, produtosPedidoEspelho);
        }

        #endregion

        #region Remove a comissão no valor dos produtos

        /// <summary>
        /// Remove comissão no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverComissao(GDASession session, IContainerCalculo container,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            return DescontoAcrescimo.Instance.RemoverComissao(session, container, produtosPedidoEspelho);
        }

        #endregion

        #region Recupera o valor da comissão

        public decimal GetComissaoPedido(uint idPedido)
        {
            string sql = "select coalesce(sum(coalesce(valorComissao,0)),0) from produtos_pedido_espelho where idPedido=" + idPedido;
            return decimal.Parse(objPersistence.ExecuteScalar(sql).ToString());
        }

        #endregion

        #region Métodos de suporte

        /// <summary>
        /// Recupera o percentual de comissão de um pedido.
        /// </summary>
        public float RecuperaPercComissao(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<float>(session, "percComissao", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém os dados de acréscimo, comissão e desconto do pedido espelho.
        /// </summary>
        public void ObterDadosComissaoDescontoAcrescimo(GDASession sessao, out decimal acrescimo, out decimal desconto, int idPedido, out float percComissao, out int tipoAcrescimo, out int tipoDesconto)
        {
            var filtro = string.Format("IdPedido={0}", idPedido);

            acrescimo = ObtemValorCampo<decimal>(sessao, "Acrescimo", filtro);
            desconto = ObtemValorCampo<decimal>(sessao, "Desconto", filtro);
            percComissao = RecuperaPercComissao(sessao, (uint)idPedido);
            tipoAcrescimo = ObtemValorCampo<int>(sessao, "TipoAcrescimo", filtro);
            tipoDesconto = ObtemValorCampo<int>(sessao, "TipoDesconto", filtro);
        }

        #endregion

        #endregion

        #region Acréscimo

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool AplicarAcrescimo(GDASession session, IContainerCalculo container, int tipoAcrescimo, decimal acrescimo,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            return DescontoAcrescimo.Instance.AplicarAcrescimo(
                session,
                container,
                tipoAcrescimo,
                acrescimo,
                produtosPedidoEspelho
            );
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverAcrescimo(GDASession session, IContainerCalculo container,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            return DescontoAcrescimo.Instance.RemoverAcrescimo(session, container, produtosPedidoEspelho);
        }

        #endregion

        #region Recupera o valor do acréscimo

        public decimal GetAcrescimoProdutos(uint idPedido)
        {
            string sql = @"select (
                    select coalesce(sum(coalesce(valorAcrescimoProd,0)+coalesce(valorAcrescimoCliente,0)),0)
                    from produtos_pedido_espelho where idPedido={0}
                )+(
                    select coalesce(sum(coalesce(valorAcrescimoProd,0)),0)
                    from produto_pedido_espelho_benef where idProdPed in (select * from (
                        select idProdPed from produtos_pedido_espelho where idPedido={0}
                    ) as temp)
                )";

            return ExecuteScalar<decimal>(String.Format(sql, idPedido));
        }

        public decimal GetAcrescimoPedido(uint idPedido)
        {
            string sql = @"select (
                    select coalesce(sum(coalesce(valorAcrescimo,0)),0)
                    from produtos_pedido_espelho where idPedido={0}
                )+(
                    select coalesce(sum(coalesce(valorAcrescimo,0)),0)
                    from produto_pedido_espelho_benef where idProdPed in (select * from (
                        select idProdPed from produtos_pedido_espelho where idPedido={0}
                    ) as temp)
                )";

            return ExecuteScalar<decimal>(String.Format(sql, idPedido));
        }

        #endregion

        #endregion

        #region Desconto

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool AplicarDesconto(GDASession sessao, IContainerCalculo container, int tipoDesconto, decimal desconto,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            var aplicado = DescontoAcrescimo.Instance.AplicarDesconto(sessao, container, tipoDesconto, desconto,
                produtosPedidoEspelho);

            objPersistence.ExecuteCommand(
                sessao,
                "update pedido_espelho set idFuncDesc=?f, dataDesc=?d where idPedido=" + (container.IdPedido() ?? 0),
                new GDAParameter("?f", UserInfo.GetUserInfo.CodUser),
                new GDAParameter("?d", DateTime.Now)
            );

            return aplicado;
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverDesconto(GDASession session, IContainerCalculo container,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            return DescontoAcrescimo.Instance.RemoverDesconto(session, container, produtosPedidoEspelho);
        }

        #endregion

        #region Recupera o valor do desconto

        public decimal GetDescontoProdutos(uint idPedido)
        {
            return GetDescontoProdutos(null, idPedido);
        }

        public decimal GetDescontoProdutos(GDASession sessao, uint idPedido)
        {
            string sql;

            if (PedidoConfig.RatearDescontoProdutos)
            {
                sql = @"select (
                        select coalesce(sum(coalesce(valorDescontoProd,0)+coalesce(valorDescontoQtde,0){1}),0)
                        from produtos_pedido_espelho where idPedido={0} and (InvisivelFluxo IS NULL OR InvisivelFluxo=false)
                    )+(
                        select coalesce(sum(coalesce(valorDescontoProd,0)),0)
                        from produto_pedido_espelho_benef where idProdPed in (select * from (
                            select idProdPed from produtos_pedido_espelho where idPedido={0} and (InvisivelFluxo IS NULL OR InvisivelFluxo=false)
                        ) as temp)
                    )";
            }
            else
            {
                sql = @"select (
                        select coalesce(sum(coalesce(ppe.total/a.totalProd*a.desconto,0)+coalesce(ppe.valorDescontoQtde,0){1}),0)
                        from produtos_pedido_espelho ppe
                            left join (
                                select a.idAmbientePedido, sum(ppe.total+coalesce(ppe.valorBenef,0)) as totalProd, 
                                    a.desconto*if(a.tipoDesconto=1, sum(ppe.total+coalesce(ppe.valorBenef,0))/100, 1) as desconto
                                from produtos_pedido_espelho ppe
                                    inner join ambiente_pedido_espelho a on (ppe.idAmbientePedido=a.idAmbientePedido)
                                where ppe.idPedido={0} and (InvisivelFluxo IS NULL OR InvisivelFluxo=false)
                                group by a.idAmbientePedido
                            ) as a on (ppe.idAmbientePedido=a.idAmbientePedido)
                        where ppe.idPedido={0} and (InvisivelFluxo IS NULL OR InvisivelFluxo=false)
                    )+(
                        select coalesce(sum(coalesce(ppeb.valor/a.totalProd*a.desconto,0)),0)
                        from produto_pedido_espelho_benef ppeb
                            inner join produtos_pedido_espelho ppe on (ppeb.idProdPed=ppe.idProdPed)
                            left join (
                                select a.idAmbientePedido, sum(ppe.total+coalesce(ppe.valorBenef,0)) as totalProd, 
                                    a.desconto*if(a.tipoDesconto=1, sum(ppe.total+coalesce(ppe.valorBenef,0))/100, 1) as desconto
                                from produtos_pedido_espelho ppe
                                    inner join ambiente_pedido_espelho a on (ppe.idAmbientePedido=a.idAmbientePedido)
                                where ppe.idPedido={0} and (InvisivelFluxo IS NULL OR InvisivelFluxo=false)
                                group by a.idAmbientePedido
                            ) as a on (ppe.idAmbientePedido=a.idAmbientePedido)
                        where ppe.idPedido={0} and (InvisivelFluxo IS NULL OR InvisivelFluxo=false)
                    )";
            }

            return ExecuteScalar<decimal>(sessao, String.Format(sql, idPedido, PedidoConfig.ConsiderarDescontoClienteDescontoTotalPedido ?
                "+coalesce(valorDescontoCliente,0)" : ""));
        }

        public decimal GetDescontoPedido(uint idPedido)
        {
            return GetDescontoPedido(null, idPedido);
        }

        public decimal GetDescontoPedido(GDASession sessao, uint idPedido)
        {
            string sql;

            if (PedidoConfig.RatearDescontoProdutos)
            {
                sql = @"select (
                        select coalesce(sum(coalesce(valorDesconto,0)),0)
                        from produtos_pedido_espelho where idPedido={0} and coalesce(invisivelFluxo,false)=false
                    )+(
                        select coalesce(sum(coalesce(valorDesconto,0)),0)
                        from produto_pedido_espelho_benef where idProdPed in (select * from (
                            select idProdPed from produtos_pedido_espelho where idPedido={0} and coalesce(invisivelFluxo,false)=false
                        ) as temp)
                    )";
            }
            else
            {
                decimal desconto;
                var descontoPedido = ObterDesconto(null, (int)idPedido);
                var tipoDescontoPedido = ObterTipoDesconto(null, (int)idPedido);
                var totalPedido = GetTotal(null, idPedido);
                var valorIcmsPedido = ObterValorIcms(null, (int)idPedido);
                var valorIpiPedido = ObterValorIpi(null, (int)idPedido);
                var valorEntrega = ObtemValorCampo<decimal>("ValorEntrega", "IdPedido=" + idPedido);

                var pedEsp = GetElementByPrimaryKey(idPedido);

                if (descontoPedido == 100 && tipoDescontoPedido == 1)
                    desconto = totalPedido > 0 ? totalPedido : ProdutosPedidoDAO.Instance.GetTotalByPedidoFluxo(idPedido);
                else
                {
                    if (tipoDescontoPedido == 2)
                        desconto = descontoPedido;
                    else
                    {
                        var taxaFastDelivery = PedidoDAO.Instance.ObtemTaxaFastDelivery(sessao, idPedido);

                        //Remove o IPI e ICMS
                        var total = totalPedido - valorIcmsPedido - valorIpiPedido - valorEntrega;

                        //Remove FastDelivery se houver
                        total = taxaFastDelivery > 0 ? total / (1 + ((decimal)taxaFastDelivery / 100)) : total;

                        //Calcula o desconto
                        desconto = total / (1 - (descontoPedido / 100)) * (descontoPedido / 100);
                    }
                }

                return desconto;
            }

            return ExecuteScalar<decimal>(sessao, String.Format(sql, idPedido));
        }

        #endregion

        #endregion

        #region Finalizar

        internal void FinalizarAplicacaoComissaoAcrescimoDesconto(GDASession sessao, IContainerCalculo container,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho, bool atualizar)
        {
            if (atualizar)
            {
                foreach (var produtoPedidoEspelho in produtosPedidoEspelho)
                {
                    ProdutosPedidoEspelhoDAO.Instance.Update(sessao, produtoPedidoEspelho, container);
                    ProdutosPedidoEspelhoDAO.Instance.AtualizaBenef(sessao, produtoPedidoEspelho.IdProdPed, produtoPedidoEspelho.Beneficiamentos, container);
                }
            }
        }

        #endregion

        #endregion

        #region Verifica se o desconto do pedido está dentro do permitido

        /// <summary>
        /// Verifica se o desconto do pedido está dentro do permitido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool DescontoPermitido(uint idPedido)
        {
            string somaDesconto = "(select sum(coalesce(valorDescontoQtde,0)" + (PedidoConfig.RatearDescontoProdutos ? "+coalesce(valorDesconto,0)+coalesce(valorDescontoProd,0)" :
                "") + ") from produtos_pedido_espelho where idPedido=p.idPedido)";

            uint idFunc = UserInfo.GetUserInfo.CodUser;
            if (Geral.ManterDescontoAdministrador)
                idFunc = ObtemIdFuncDesc(idPedido).GetValueOrDefault(idFunc);

            string sql = "Select Count(*) from pedido_espelho p Where idPedido=" + idPedido + @" And (
                (tipoDesconto=1 And desconto<=" + PedidoConfig.Desconto.GetDescontoMaximoPedido(idFunc, (int)PedidoDAO.Instance.GetTipoVenda(idPedido), (int)PedidoDAO.Instance.ObtemIdParcela(idPedido)).ToString().Replace(",", ".") + @") Or
                (tipoDesconto=2 And round(desconto/(total+" + somaDesconto + (!PedidoConfig.RatearDescontoProdutos ? "+desconto" : "") + "),2)<=(" +
                PedidoConfig.Desconto.GetDescontoMaximoPedido(idFunc, (int)PedidoDAO.Instance.GetTipoVenda(idPedido), (int)PedidoDAO.Instance.ObtemIdParcela(idPedido)).ToString().Replace(",", ".") + @"/100))
            )";

            return ExecuteScalar<int>(sql) > 0;
        }

        #endregion

        #region Atualiza o peso dos produtos e do pedido espelho

        /// <summary>
        /// Atualiza o peso dos produtos e do pedido espelho.
        /// </summary>
        public void AtualizaPeso(GDASession sessao, uint idPedido)
        { 
            string sql = @"
                UPDATE produtos_pedido_espelho ppe
                    LEFT JOIN 
                    (
                        " + Utils.SqlCalcPeso(Utils.TipoCalcPeso.ProdutoPedidoEspelho, idPedido, true, true, false) + @"
                    ) as peso on (ppe.idProdPed=peso.id)
                    INNER JOIN produto prod ON (ppe.idProd = prod.idProd)
                    LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd)
                    LEFT JOIN 
                    (
                        SELECT pp1.IdProdPedParent, sum(pp1.peso) as peso
                        FROM produtos_pedido_espelho pp1
                        GROUP BY pp1.IdProdPedParent
                    ) as pesoFilhos ON (ppe.IdProdPed = pesoFilhos.IdProdPedParent)
                SET ppe.peso = coalesce(IF(sgp.TipoSubgrupo IN (" + (int)TipoSubgrupoProd.VidroDuplo + "," + (int)TipoSubgrupoProd.VidroLaminado + @"), pesoFilhos.peso * ppe.Qtde, peso.peso), 0)
                WHERE ppe.idPedido={0};

                update produtos_pedido pp
                    inner join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                set pp.peso=ppe.peso
                where pp.idPedido={0};

                UPDATE pedido_espelho
                SET peso = coalesce((SELECT sum(peso) FROM produtos_pedido_espelho WHERE coalesce(IdProdPedParent, 0) = 0 AND idPedido={0} and !coalesce(invisivelFluxo, false)), 0) 
                WHERE idPedido = {0}";

            objPersistence.ExecuteCommand(sessao, String.Format(sql, idPedido));
        }

        #endregion

        #region Atualizar valor total do pedido

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        internal void UpdateTotalPedido(GDASession sessao, uint idPedido)
        {
            var pedidoEspelho = GetElement(sessao, idPedido);
            UpdateTotalPedido(sessao, pedidoEspelho);
        }

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        internal void UpdateTotalPedido(GDASession sessao, PedidoEspelho pedidoEspelho)
        {
            UpdateTotalPedido(sessao, pedidoEspelho, false);
        }

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        internal void UpdateTotalPedido(GDASession sessao, PedidoEspelho pedidoEspelho, bool forcarAtualizacao)
        {

            // Atualiza valor do pedido
            string sql = "update pedido_espelho p set Total=Round((Select Sum(Total + coalesce(valorBenef, 0)) From produtos_pedido_espelho " +
                "Where IdPedido=p.IdPedido and (invisivelFluxo=false or invisivelFluxo is null) AND IdProdPedParent IS NULL), 2) Where IdPedido=" + pedidoEspelho.IdPedido;

            objPersistence.ExecuteCommand(sessao, sql);

            if (!PedidoConfig.RatearDescontoProdutos)
            {
                // Atualiza total do pedido
                sql = @"
                Update pedido_espelho p Set 
                    Total=Round(Total-if(p.TipoDesconto=1, (p.Total * (p.Desconto / 100)), p.Desconto)-
                        Coalesce(
                            (
                                Select sum(if(tipoDesconto=1, (
                                    (
                                        Select sum(total + coalesce(valorBenef,0)) 
                                        From produtos_pedido_espelho 
                                        Where idAmbientePedido=a.idAmbientePedido
                                                and (invisivelFluxo=false or invisivelFluxo is null)
                                                AND IdProdPedParent IS NULL
                                    ) * (desconto / 100)
                            ), desconto)
                        ) 
                From ambiente_pedido_espelho a where idPedido=p.idPedido),0), 2) " +
                    "Where IdPedido=" + pedidoEspelho.IdPedido;

                objPersistence.ExecuteCommand(sessao, sql);
            }

            bool pedFastDelivery = PedidoDAO.Instance.IsFastDelivery(sessao, pedidoEspelho.IdPedido);
            float pedTaxaFastDelivery = PedidoDAO.Instance.ObtemTaxaFastDelivery(sessao, pedidoEspelho.IdPedido);
            uint pedIdLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, pedidoEspelho.IdPedido);
            uint pedIdCli = PedidoDAO.Instance.ObtemIdCliente(sessao, pedidoEspelho.IdPedido);

            float percFastDelivery = 1;

            // Verifica se há taxa de urgência para o pedido
            if (PedidoConfig.Pedido_FastDelivery.FastDelivery && pedFastDelivery)
            {
                percFastDelivery = 1 + (pedTaxaFastDelivery / 100);
                sql = "update pedido_espelho set Total=Round(Total * " + percFastDelivery.ToString().Replace(',', '.') + ", 2) where IdPedido=" + pedidoEspelho.IdPedido;

                objPersistence.ExecuteCommand(sessao, sql);
            }

            string descontoRateadoImpostos = "0";
            pedidoEspelho.Total = GetTotal(sessao, pedidoEspelho.IdPedido);

            if (!PedidoConfig.RatearDescontoProdutos)
            {
                var dadosAmbientes = (pedidoEspelho as IContainerCalculo).Ambientes.Obter(true)
                    .Cast<AmbientePedidoEspelho>()
                    .Select(x => new { x.IdAmbientePedido, x.TotalProdutos });

                var formata = new Func<decimal, string>(x => x.ToString().Replace(".", "").Replace(",", "."));

                decimal totalSemDesconto = GetTotalSemDesconto(sessao, pedidoEspelho.IdPedido, (pedidoEspelho.Total / (decimal)percFastDelivery));
                string selectAmbientes = !dadosAmbientes.Any() ? "select null as idAmbientePedido, 1 as total" :
                    string.Join(" union all ", dadosAmbientes.Select(x =>
                        string.Format("select {0} as idAmbientePedido, {1} as total", x.IdAmbientePedido, formata(x.TotalProdutos))).ToArray());

                descontoRateadoImpostos = @"(
                    if(coalesce(pe.desconto, 0)=0, 0, if(pe.tipoDesconto=1, pe.desconto / 100, pe.desconto / " + formata(totalSemDesconto) + @") * (ppe.total + coalesce(ppe.valorBenef, 0)))) - (
                    if(coalesce(ape.desconto, 0)=0, 0, if(ape.tipoDesconto=1, ape.desconto / 100, ape.desconto / (select total from (" + selectAmbientes + @") as amb 
                    where idAmbientePedido=ape.idAmbientePedido)) * (ppe.total + coalesce(ppe.valorBenef, 0))))";
            }

            // Calcula o valor do ICMS do pedido
            if (LojaDAO.Instance.ObtemCalculaIcmsPedido(sessao, pedIdLoja) && ClienteDAO.Instance.IsCobrarIcmsSt(sessao, pedIdCli))
            {
                var calcIcmsSt = CalculoIcmsStFactory.ObtemInstancia(sessao, (int)pedIdLoja, (int)pedIdCli, null, null, null, null);

                string idProd = "ppe.idProd";
                string total = "ppe.Total + Coalesce(ppe.ValorBenef, 0)";
                string aliquotaIcmsSt = "ppe.AliqIcms";

                sql = @"
                    Update produtos_pedido_espelho ppe
                        inner join pedido_espelho pe on (ppe.idPedido=pe.idPedido)
                        left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)
                    {0}
                    where ppe.idPedido=" + pedidoEspelho.IdPedido + " AND ppe.IdProdPedParent IS NULL";

                // Atualiza a Alíquota ICMSST somada ao FCPST com o ajuste do MVA e do IPI. Necessário porque na tela é recuperado e salvo o valor sem FCPST.
                objPersistence.ExecuteCommand(sessao, string.Format(sql,
                    "SET ppe.AliqIcms=(" + calcIcmsSt.ObtemSqlAliquotaInternaIcmsSt(sessao, idProd, total, descontoRateadoImpostos, aliquotaIcmsSt, percFastDelivery.ToString().Replace(',', '.')) + @")"));
                // Atualiza o valor do ICMSST calculado com a Alíquota recuperada anteriormente.
                objPersistence.ExecuteCommand(sessao, string.Format(sql,
                    "SET ppe.ValorIcms=(" + calcIcmsSt.ObtemSqlValorIcmsSt(total, descontoRateadoImpostos, aliquotaIcmsSt, percFastDelivery.ToString().Replace(',', '.')) + @")"));

                sql = @"
                    Update produtos_pedido pp
                        inner join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    set pp.AliqIcms=ppe.aliqIcms, 
                        pp.ValorIcms=ppe.valorIcms
                    where pp.idPedido=" + pedidoEspelho.IdPedido + " and pp.InvisivelPedido=true AND pp.IdProdPedParent IS NULL";

                objPersistence.ExecuteCommand(sessao, sql);

                sql = "update pedido_espelho set AliquotaIcms=Round((select sum(coalesce(AliqIcms, 0)) from produtos_pedido_espelho where idPedido=" + pedidoEspelho.IdPedido + " and (invisivelFluxo=false or invisivelFluxo is null) AND IdProdPedParent IS NULL) / (select Greatest(count(*), 1) from produtos_pedido_espelho where idPedido=" + pedidoEspelho.IdPedido + " and AliqIcms>0 and (invisivelFluxo=false or invisivelFluxo is null) AND IdProdPedParent IS NULL), 2) where idPedido=" + pedidoEspelho.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);

                sql = "update pedido_espelho set ValorIcms=Round((select sum(coalesce(ValorIcms, 0)) from produtos_pedido_espelho where IdPedido=" + pedidoEspelho.IdPedido + " and (invisivelFluxo=false or invisivelFluxo is null) AND IdProdPedParent IS NULL), 2), Total=(Total + ValorIcms) where idPedido=" + pedidoEspelho.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);
            }
            else
            {
                sql = "update produtos_pedido_espelho pp set AliqIcms=0, ValorIcms=0 where idPedido=" + pedidoEspelho.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);

                sql = "update produtos_pedido pp set AliqIcms=0, ValorIcms=0 where idPedido=" + pedidoEspelho.IdPedido + " and InvisivelPedido=true AND IdProdPedParent IS NULL";
                objPersistence.ExecuteCommand(sessao, sql);

                sql = "update pedido_espelho set AliquotaIcms=0, ValorIcms=0 where idPedido=" + pedidoEspelho.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);
            }

            // Calcula o valor do IPI do pedido
            if (LojaDAO.Instance.ObtemCalculaIpiPedido(sessao, pedIdLoja) && ClienteDAO.Instance.IsCobrarIpi(sessao, pedIdCli))
            {
                // A Alíquota e o valor do ipi devem ser calculados em dois comandos, 
                // em alguns casos o valor do ipi não estava considerando a alíquota do ipi
                sql = @"
                    Update produtos_pedido_espelho ppe
                        inner join pedido_espelho pe on (ppe.idPedido=pe.idPedido)
                        left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido) 
                    set ppe.AliquotaIpi=Round((select aliqIpi from produto where idProd=ppe.idProd), 2)
                    where ppe.IdProdPedParent IS NULL AND ppe.idPedido=" + pedidoEspelho.IdPedido;

                objPersistence.ExecuteCommand(sessao, sql);

                sql = @"
                    Update produtos_pedido_espelho ppe
                        inner join pedido_espelho pe on (ppe.idPedido=pe.idPedido)
                        left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido) 
                    set ppe.ValorIpi=(((ppe.Total + Coalesce(ppe.ValorBenef, 0) - " + descontoRateadoImpostos + @") * "
                        + percFastDelivery.ToString().Replace(',', '.') + @") * (Coalesce(ppe.AliquotaIpi, 0) / 100))
                    where ppe.idPedido=" + pedidoEspelho.IdPedido + " AND ppe.IdProdPedParent IS NULL";

                objPersistence.ExecuteCommand(sessao, sql);

                sql = @"
                    Update produtos_pedido pp
                        inner join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    set pp.AliquotaIpi=ppe.aliquotaIpi, 
                        pp.ValorIpi=ppe.valorIpi
                    where pp.idPedido=" + pedidoEspelho.IdPedido + " and pp.InvisivelPedido=true AND ppe.IdProdPedParent IS NULL";

                objPersistence.ExecuteCommand(sessao, sql);

                sql = "update pedido_espelho set AliquotaIpi=Round((select sum(coalesce(AliquotaIpi, 0)) from produtos_pedido_espelho where idPedido=" + pedidoEspelho.IdPedido + " and (invisivelFluxo=false or invisivelFluxo is null) AND IdProdPedParent IS NULL) / (select Greatest(count(*), 1) from produtos_pedido_espelho where idPedido=" + pedidoEspelho.IdPedido + " and AliquotaIpi>0 and (invisivelFluxo=false or invisivelFluxo is null) AND IdProdPedParent IS NULL), 2) where idPedido=" + pedidoEspelho.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);

                sql = "update pedido_espelho set ValorIpi=Round((select sum(coalesce(ValorIpi, 0)) from produtos_pedido_espelho where IdPedido=" + pedidoEspelho.IdPedido + " and (invisivelFluxo=false or invisivelFluxo is null) AND IdProdPedParent IS NULL), 2), Total=(Total + ValorIpi) where idPedido=" + pedidoEspelho.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);
            }
            else
            {
                sql = "update produtos_pedido_espelho pp set AliquotaIpi=0, ValorIpi=0 where idPedido=" + pedidoEspelho.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);

                sql = "update produtos_pedido pp set AliquotaIpi=0, ValorIpi=0 where idPedido=" + pedidoEspelho.IdPedido + " and InvisivelPedido=true AND IdProdPedParent IS NULL";
                objPersistence.ExecuteCommand(sessao, sql);

                sql = "update pedido_espelho set AliquotaIpi=0, ValorIpi=0 where idPedido=" + pedidoEspelho.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);
            }

            // Calcula os impostos dos produtos do pedido
            var impostos = CalculadoraImpostoHelper.ObterCalculadora<Model.PedidoEspelho>()
                .Calcular(sessao, pedidoEspelho);

            // Salva os dados dos impostos calculados
            impostos.Salvar(sessao);

            // Atualiza o campo ValorComissao
            sql = @"update pedido_espelho set valorComissao=total*coalesce(percComissao,0)/100 where idPedido=" + pedidoEspelho.IdPedido;
            objPersistence.ExecuteCommand(sessao, sql);

            //Aplica o frete no pedido
            objPersistence.ExecuteCommand(sessao, "UPDATE pedido_espelho SET Total = COALESCE(Total, 0) + COALESCE(ValorEntrega, 0) WHERE IdPedido=" + pedidoEspelho.IdPedido);

            // Atualiza peso e total de m²
            PedidoDAO.Instance.AtualizaTotM(sessao, pedidoEspelho.IdPedido, true);
            AtualizaPeso(sessao, pedidoEspelho.IdPedido);

            var rentabilidade = RentabilidadeHelper.ObterCalculadora<PedidoEspelho>().Calcular(sessao, pedidoEspelho);
            if (rentabilidade.Executado)
                rentabilidade.Salvar(sessao);
        }

        #endregion

        #region Finalizar pedido

        public IList<PedidoEspelho> GetForFinalizarMult(uint idPedido, string codCliente, uint idCliente,
            string nomeCliente, string dataIniConf, string dataFimConf)
        {
            bool temFiltro;
            string sql = Sql(idPedido, codCliente, idCliente, nomeCliente, 0, 0, 0, (int)PedidoEspelho.SituacaoPedido.Aberto, null, null, null, null, null, null, null,
                null, dataIniConf, dataFimConf, null, null, false, false, null, false, false, null, null, null, null, null, null, null, 0, false, true, out temFiltro);

            return objPersistence.LoadData(sql + " Order By pe.idPedido Desc", GetParam(codCliente, nomeCliente, null, null, null, null, null, null, dataIniConf, dataFimConf,
                null, null, null, null, null, null)).ToList();
        }

        /// <summary>
        /// Finaliza o pedido gerando uma conta a receber do valor excedente
        /// </summary>
        public uint FinalizarPedidoComTransacao(uint idPedido)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = FinalizarPedido(transaction, idPedido);

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
        /// Finaliza o pedido gerando uma conta a receber do valor excedente
        /// </summary>
        public uint FinalizarPedido(GDATransaction session, uint idPedido)
        {
            var pedAtual = GetElementByPrimaryKey(session, idPedido);

            if (ObtemSituacao(session, idPedido) == PedidoEspelho.SituacaoPedido.Finalizado)
                throw new Exception("Este pedido espelho já foi finalizado.");

            /* Chamado 53226.
             * PATCH MIGRAÇÃO: aplicar somente este bloqueio nas alterações deste arquivo. */
            var sqlProdutoQuantidadeZerada = string.Format("SELECT IdProd FROM produtos_pedido_espelho WHERE IdPedido={0} AND (Qtde IS NULL OR Qtde=0)", idPedido);

            if (objPersistence.ExecuteSqlQueryCount(session, sqlProdutoQuantidadeZerada) > 0)
                throw new Exception(string.Format("O(s) produto(s) {0} está(ão) com a quantidade zerada. Informe a quantidade dele(s) antes de finalizar o pedido.",
                    string.Join(", ", ExecuteMultipleScalar<string>(session, string.Format("SELECT CodInterno FROM produto WHERE IdProd IN ({0})", sqlProdutoQuantidadeZerada)))));

            /* Chamado 45775. */
            if (ExecuteScalar<bool>(session, string.Format("SELECT COUNT(*)>0 FROM produto_impressao WHERE IdPedido={0} AND IdImpressao IS NULL AND (Cancelado IS NULL OR Cancelado=0)", idPedido)))
                objPersistence.ExecuteCommand(session, string.Format("DELETE FROM produto_impressao WHERE IdPedido={0} AND IdImpressao IS NULL AND (Cancelado IS NULL OR Cancelado=0)", idPedido));

            /* Chamado 35652. */
            if (ExecuteScalar<bool>(session, string.Format("SELECT COUNT(*)>0 FROM produto_impressao WHERE IdPedido={0} AND IdImpressao IS NOT NULL AND (Cancelado IS NULL OR Cancelado=0)", idPedido)))
                throw new Exception("O pedido não pode ser finalizado porque existe(m) etiqueta(s) impressa(s). Cancele a(s) impressão(ões).");

            if (ObtemSituacao(session, idPedido) == PedidoEspelho.SituacaoPedido.Processando)
                throw new Exception("Não é possível finalizar pedidos que não terminaram de processar!");

            // Verifica se todas as peças da conferência possuem referência no produtos_pedido
            if (ExecuteScalar<bool>(session,
                String.Format("Select Count(*)>0 From produtos_pedido Where IdProdPedEsp is null and InvisivelFluxo=0 and IdPedido={0}", idPedido)))
                throw new Exception("Alguns produtos foram excluídos incorretamente da conferência, será necessário cancelá-la e gerá-la novamente.");

            // Verifica se o pedido possui produtos.
            if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From produtos_pedido_espelho Where idPedido=" + idPedido) == 0)
                throw new Exception("Inclua pelo menos um produto no pedido para finalizá-lo.");

            // Verifica se o pedido possui Itens de Projeto sem referência de ambiente.
            if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From item_projeto Where idPedidoEspelho=" + idPedido + @" And idItemProjeto Not In
                (Select idItemProjeto From ambiente_pedido_espelho Where idPedido=" + idPedido + ")") > 0)
                throw new Exception("Exclua os projetos que não possuem referência de ambiente.");

            /* Chamado 45515. */
            if (objPersistence.ExecuteSqlQueryCount(session, string.Format(@"SELECT COUNT(*) > 0 FROM item_projeto ip
	                INNER JOIN material_item_projeto mip ON (ip.IdItemProjeto=mip.IdItemProjeto)
                    INNER JOIN produtos_pedido_espelho ppe ON (mip.IdMaterItemProj=ppe.IdMaterItemProj)
                WHERE ppe.IdPedido={0} AND ip.IdPedido > 0;", idPedido)) > 0)
                throw new Exception("Alguns produtos foram gerados incorretamente na conferência, será necessário cancelá-la e gerá-la novamente.");
 
            /* Chamado 60766. */
            if (objPersistence.ExecuteSqlQueryCount(session, string.Format(@"SELECT COUNT(*) > 0 FROM produtos_pedido_espelho ppe
	                LEFT JOIN item_projeto ip ON (ppe.IdItemProjeto=ip.IdItemProjeto)
                WHERE ip.IdItemProjeto IS NULL AND ppe.IdItemProjeto IS NOT NULL AND ppe.IdItemProjeto>0 AND ppe.IdPedido={0};", idPedido)) > 0)
                throw new Exception("Alguns produtos foram gerados incorretamente na conferência, será necessário cancelá-la e gerá-la novamente.");

            // Verifica se algum Projeto Modelo utilizado no pedido está bloqueado para gerar novos pedidos.
            var modelosProjetoBloqueados = string.Empty;
            var idsProjetoModelo = ItemProjetoDAO.Instance.ObterIdsProjetoModeloPorPedido(session, idPedido);
            foreach (var id in idsProjetoModelo)
            {
                if (ProjetoModeloDAO.Instance.ObterSituacao(session, id) == ProjetoModelo.SituacaoEnum.Bloqueado)
                    modelosProjetoBloqueados += ProjetoModeloDAO.Instance.ObtemCodigo(session, id) + ", ";
            }
            if (!string.IsNullOrEmpty(modelosProjetoBloqueados))
                throw new Exception(string.Format("O(s) projeto(s) {0} esta(ão) bloqueado(s) para gerar novos pedidos. Selecione outro projeto para continuar.",
                    modelosProjetoBloqueados.Remove(modelosProjetoBloqueados.Length - 2, 2)));

            /* Chamado 62041. */
            if (objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) > 0 FROM produtos_pedido_espelho ppe WHERE ppe.IdPedido={0};", idPedido)) !=
                objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) > 0 FROM produtos_pedido pp WHERE pp.IdPedido={0} AND (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo=0);", idPedido)))
                throw new Exception("Alguns produtos foram gerados incorretamente na conferência, será necessário cancelá-la e gerá-la novamente.");

            var cliente = ClienteDAO.Instance.GetElementByPrimaryKey(session, PedidoDAO.Instance.ObtemIdCliente(session, idPedido));

            // Se o pedido tiver sido pago antecipadamente e o valor do espelho seja maior que o pedido original, 
            // obriga o usuário a cancelar a conferência e ajustar o que tiver que ajustar no pedido original
            if (!PCPConfig.TelaPedidoPcp.PermitirFinalizarComDiferencaEPagtoAntecip &&
                PedidoDAO.Instance.ObtemIdPagamentoAntecipado(session, idPedido) > 0 &&
                PedidoDAO.Instance.GetTotal(session, idPedido) < GetTotal(session, idPedido) &&
                (cliente.PercSinalMinimo.GetValueOrDefault() > 0 || cliente.PagamentoAntesProducao))
                throw new Exception("Não é possível finalizar esse pedido, o valor do pedido em conferência não pode ser maior que o valor do pedido comercial sempe que houver pagto. antecipado.");

            var obrigaInformarProcApl = PedidoConfig.DadosPedido.ObrigarProcAplVidros;
            var lstAmbientePedido = new List<uint>();
            var idsProdQtdeColocarReserva = new Dictionary<int, float>();
            var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, idPedido, false, false);

            /* Chamado 56301. */
            if (produtosPedidoEspelho.Any(f => f.IdSubgrupoProd == 0))
                throw new Exception(string.Format("Informe o subgrupo dos produtos {0} antes de finalizar o pedido.",
                    string.Join(", ", produtosPedidoEspelho.Where(f => f.IdSubgrupoProd == 0).Select(f => f.CodInterno).Distinct().ToList())));

            /* Chamado 33519. */
            ValidaTipoPedidoTipoProduto(session, PedidoDAO.Instance.GetElementByPrimaryKey(session, idPedido), produtosPedidoEspelho.ToArray());

            foreach (var prod in produtosPedidoEspelho)
            {
                /* Chamado 15834.
                    * Esta verificação irá obrigar o usuário a excluir o ambiente vazio, que por sua vez, faz com que
                    * a exportação de pedido gere vários produtos incorretos com quantidade "0,5". */
                if (prod.IdAmbientePedido.GetValueOrDefault() > 0 && !lstAmbientePedido.Contains(prod.IdAmbientePedido.Value))
                {
                    // Adiciona o id do ambiente para evitar que a verificação seja feita mais de uma vez para o mesmo ambiente.
                    lstAmbientePedido.Add(prod.IdAmbientePedido.Value);
                    // Recupera o id do item projeto associado ao ambiente.
                    var idItemProjeto = AmbientePedidoEspelhoDAO.Instance.ObtemItemProjeto(session, prod.IdAmbientePedido.Value);
                    // Verifica se o ambiente foi criado a partir de um projeto.
                    if (idItemProjeto > 0)
                        // Verifica se o ambiente está vazio.
                        if (objPersistence.ExecuteSqlQueryCount(session, "SELECT COUNT(*) FROM item_projeto WHERE IdItemProjeto=" +
                            idItemProjeto) == 0)
                            throw new Exception("O ambiente " + AmbientePedidoEspelhoDAO.Instance.ObtemValorCampo<string>(session, "Ambiente",
                                "IdAmbientePedido=" + prod.IdAmbientePedido.Value) + " está vazio, exclua-o.");
                }

                var tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)prod.IdProd);

                // Verifica se o processo e a aplicação foram informados
                if (obrigaInformarProcApl && Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)prod.IdGrupoProd) &&
                    (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto))
                {
                    if (prod.IdProcesso.GetValueOrDefault(0) == 0)
                        throw new Exception("Informe o processo do produto " + prod.DescrProduto + ".");

                    if (prod.IdAplicacao.GetValueOrDefault(0) == 0)
                        throw new Exception("Informe a aplicação do produto " + prod.DescrProduto + ".");
                }

                /* Chamado 25432. */
                if (PedidoConfig.LiberarPedido && !PedidoDAO.Instance.IsProducao(session, idPedido))
                {
                    var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;
                    var ml = tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6;

                    if (!idsProdQtdeColocarReserva.ContainsKey((int)prod.IdProd))
                        idsProdQtdeColocarReserva.Add((int)prod.IdProd,
                            m2 ? prod.TotM : ml ? prod.Qtde * prod.Altura : prod.Qtde);
                    else
                        idsProdQtdeColocarReserva[(int)prod.IdProd] +=
                            m2 ? prod.TotM : ml ? prod.Qtde * prod.Altura : prod.Qtde;
                }
            }

            if (idsProdQtdeColocarReserva.Count > 0 && !PedidoDAO.Instance.IsProducao(session, idPedido) && !FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar)
            {
                var idLoja = PedidoDAO.Instance.ObtemIdLoja(session, idPedido);
                ProdutoLojaDAO.Instance.ColocarReserva(session, (int)idLoja, idsProdQtdeColocarReserva, null, null, (int)idPedido, null,
                    null, null, null, "PedidoEspelhoDAO - FinalizarPedido");
                var idsProdQtdeTirarReserva = new Dictionary<int, float>();

                foreach (var produtoPedido in ProdutosPedidoDAO.Instance.GetByPedido(session, idPedido, false))
                {
                    var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session, (int)produtoPedido.IdProd);

                    var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;
                    var ml = tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6;

                    if (!idsProdQtdeTirarReserva.ContainsKey((int)produtoPedido.IdProd))
                        idsProdQtdeTirarReserva.Add((int)produtoPedido.IdProd,
                            m2 ? produtoPedido.TotM : ml ? produtoPedido.Qtde * produtoPedido.Altura : produtoPedido.Qtde);
                    else
                        idsProdQtdeTirarReserva[(int)produtoPedido.IdProd] +=
                            m2 ? produtoPedido.TotM : ml ? produtoPedido.Qtde * produtoPedido.Altura : produtoPedido.Qtde;
                }

                ProdutoLojaDAO.Instance.TirarReserva(session, (int)idLoja, idsProdQtdeTirarReserva, null, null, (int)idPedido, null,
                    null, null, null, "PedidoEspelhoDAO - FinalizarPedido");
            }

            // Obra
            var idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? PedidoDAO.Instance.GetIdObra(session, idPedido) : null;
            if (idObra > 0)
            {
                var lstProdSemComposicao = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, idPedido, false, true, true).ToArray();

                foreach (ProdutosPedidoEspelho p in lstProdSemComposicao)
                {
                    // Verifica se o produto está na obra do pedido e se tem m² suficiente para adiciona-lo
                    var dadosProduto = "'" + p.DescrProduto + "'" + (p.IdAmbientePedido > 0 ? " (ambiente '" + p.Ambiente + "')" : "");
                    var retorno = ProdutoObraDAO.Instance.IsProdutoObra(session, idObra.Value, p.CodInterno);

                    if (!retorno.ProdutoValido)
                        throw new Exception("Não é possível inserir o produto " + dadosProduto + " no pedido. " + retorno.MensagemErro);

                    // Se o pedido tiver forma de pagamento Obra, não permite inserir produto com tipo subgrupo VidroLaminado ou VidroDuplo sem produto de composição.
                    var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(session, (int)p.IdProd);
                    if (tipoSubgrupo == TipoSubgrupoProd.VidroLaminado || tipoSubgrupo == TipoSubgrupoProd.VidroDuplo)
                    {
                        if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa(p.IdProd))
                            throw new Exception("Não é possível inserir produtos do tipo de subgrupo vidro duplo ou laminado sem produto de composição em seu cadastro.");
                    }
                }

                var saldoObra = ObraDAO.Instance.ObtemSaldoComPedConf(session, idObra.Value);

                if (saldoObra < 0)
                    throw new Exception("O saldo da obra ultrapassou seu valor total. Saldo da obra: " + saldoObra.ToString("C"));
            }

            // Verifica se o total dos clones é igual aos seus produtos_pedido_espelho relacionados
            foreach (var prodPedEsp in ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, idPedido, false, false))
            {
                var prodPed = ProdutosPedidoDAO.Instance.GetByProdPedEsp(session, prodPedEsp.IdProdPed, false);

                if (prodPed != null && prodPed.IdProdPed > 0 && (prodPedEsp.Total != prodPed.Total || prodPedEsp.ValorBenef != prodPed.ValorBenef))
                    throw new Exception("É necessário cancelar a conferência e gerá-la novamente antes de finalizar. Alguns produtos da conferência estão divergentes do original.");
            }

            /* Chamado 56050. */
            if (!pedAtual.GeradoParceiro)
            {
                //Chamado 46533
                string msgDiasMinEntrega;
                var prodsDiasMinEntrega = produtosPedidoEspelho.Where(f => f.IdAplicacao.GetValueOrDefault() > 0).Select(f => new KeyValuePair<int, uint>((int)f.IdProd, f.IdAplicacao.Value)).ToList();
                if (!EtiquetaAplicacaoDAO.Instance.VerificaPrazoEntregaAplicacao(session, prodsDiasMinEntrega, PedidoDAO.Instance.ObtemDataEntrega(session, idPedido).GetValueOrDefault(), out msgDiasMinEntrega))
                    throw new Exception(msgDiasMinEntrega);
            }

            var idOrcamento = new uint();
            var tipoPedido = PedidoDAO.Instance.ObtemValorCampo<int>(session, "tipoPedido", "idPedido=" + idPedido);

            try
            {
                UpdateTotalPedido(session, pedAtual);
                AtualizaSituacaoImpressao(session, idPedido);

                //Verifica se deve gerar projeto para cnc
                AtualizaSituacaoCnc(session, idPedido);

                // Chamado 13003. Verifica se a situação do CNC do pedido foi atualizada corretamente.
                var situacaoCnc = ObtemValorCampo<int?>(session, "situacaoCnc", "idPedido=" + idPedido);
                if (situacaoCnc.GetValueOrDefault(0) == 0)
                {
                    ErroDAO.Instance.InserirFromException("AtualizaSituacaoCnc - idPedido: " + idPedido, new Exception(""));
                    throw new Exception("Falha ao atualizar a situação CNC do pedido, finalize-o novamente.");
                }

                #region Cria etiqueta produção

                // Chamado 12629. Movemos a geração de etiquetas para esta parte para que, caso ocorra algum erro, seja possível
                // somente apagar as etiquetas e lançar a exceção ocorrida.
                // Se for o caso, gera etiquetas na produção.

                // Cria as etiquetas na produção conforme os ambientes/produtos do pedido.
                ImpressaoEtiquetaDAO.Instance.GerarEtiquetasProducao(session, idPedido, false);

                // Variável criada para salvar a quantidade de peças na produção geradas para o pedido.
                var qtdeProdPedProducao = ProdutoPedidoProducaoDAO.Instance.GetCountByPedido(session, idPedido);

                // Verifica se todos os produtos do pedido geraram etiqueta, caso não tenham gerado uma exceção é lançada.
                if (tipoPedido != (int)Pedido.TipoPedidoEnum.MaoDeObra)
                {
                    var qtdeProdPed = ProdutosPedidoEspelhoDAO.Instance.ObterQtdePecasParaImpressaoFinalizacaoPCP(session, (int)idPedido);

                    // Compara a quantidade de peças na produção com a quantidade de produtos do pedido.
                    if (qtdeProdPedProducao != qtdeProdPed)
                        throw new Exception("A quantidade de peças na produção difere da quantidade de peças no pedido. Finalize a conferência novamente.");
                }
                else
                {
                    // Compara a quantidade de peças na produção com a quantidade de ambientes do pedido.
                    if (qtdeProdPedProducao !=
                        objPersistence.ExecuteSqlQueryCount(session, "SELECT SUM(ape.qtde) FROM ambiente_pedido_espelho ape WHERE ape.idPedido=" + idPedido))
                        throw new Exception("A quantidade de peças na produção difere da quantidade de peças no pedido. Finalize a conferência novamente.");
                }

                #endregion

                // Atualiza a situação da produção do pedido para Pendente se for pedido de revenda, possuir peças de box,
                // a empresa fizer separação de pedido de venda/revenda
                if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && tipoPedido == (int)Pedido.TipoPedidoEnum.Revenda &&
                    PedidoDAO.Instance.PossuiVidrosEstoque(session, idPedido))
                    objPersistence.ExecuteCommand(session, "Update pedido Set situacaoProducao=" + (int)Pedido.SituacaoProducaoEnum.Pendente + " Where idPedido=" + idPedido);

                var login = UserInfo.GetUserInfo;
                if (!login.IsAdminSync && login.Nome.ToString() != "admin")
                    objPersistence.ExecuteCommand(session, "Update pedido_espelho Set idFuncFin=" + login.CodUser + " Where idPedido=" + idPedido);

                if (PCPConfig.EmailSMS.EnviarEmailPedidoConfirmado && !HttpContext.Current.Request.Url.ToString().Contains("localhost"))
                    if (ProdutosPedidoEspelhoDAO.Instance.PossuiVidroCalcM2(session, idPedido))
                        Email.EnviaEmailPedidoPcp(session, idPedido);

                var idFunc = PedidoDAO.Instance.ObtemIdFunc(session, idPedido);
                var vendedor = FuncionarioDAO.Instance.GetElement(session, idFunc);

                if (vendedor != null && vendedor.EnviarEmail && !HttpContext.Current.Request.Url.ToString().Contains("localhost"))
                    Email.EnviaEmailPedidoPcpVendedor(session, idPedido, vendedor.Email, vendedor.Nome);

                // Verifica se irá gerar orçamento a partir deste pedido finalizado
                if (PCPConfig.GerarOrcamentoFerragensAluminiosPCP && ClienteDAO.Instance.GeraOrcamentoPCP(session, PedidoDAO.Instance.ObtemIdCliente(session, idPedido)) &&
                    tipoPedido == (int)Pedido.TipoPedidoEnum.Venda)
                {
                    // Se possuir orçamento gerado e o mesmo estiver em aberto, apaga os produtos do orçamento e insere novamente,
                    // mas se não tiver orçamento gerado, gera um novo
                    if (PossuiOrcamentoGerado(session, idPedido))
                    {
                        idOrcamento = ObtemIdOrcamentoGerado(session, idPedido);
                        if (OrcamentoDAO.Instance.ObtemSituacao(session, idOrcamento) == (int)Orcamento.SituacaoOrcamento.EmAberto)
                            GeraOrcamento(session, idPedido, idOrcamento);
                    }
                    else
                        idOrcamento = GeraOrcamento(session, idPedido, null);
                }

                // Se a empresa deve salvar o arquivo de marcação das peças então o método devido é chamado.
                if (PCPConfig.EmpresaGeraArquivoFml)
                    GerarArquivoFmlPeloPedido(session, produtosPedidoEspelho.ToArray(), true);
                else if (PCPConfig.EmpresaGeraArquivoDxf)
                    GerarArquivoDxfPeloPedido(session, produtosPedidoEspelho.ToArray());

                if (PCPConfig.EmpresaGeraArquivoSGlass)
                    GerarArquivoSglassPeloPedido(session, produtosPedidoEspelho.ToArray());

                if (PCPConfig.EmpresaGeraArquivoIntermac)
                    GerarArquivoIntermacPeloPedido(session, produtosPedidoEspelho.ToArray());

                LogAlteracaoDAO.Instance.LogPedidoEspelho(session, pedAtual);

                return idOrcamento;
            }
            catch (Exception ex)
            {
                // Chamado 9493. Caso o erro ocorra novamente, mesmo com o tratamento, será mais fácil identificar a causa do mesmo
                // sabendo qual foi a exceção lançada pelo sistema.
                ErroDAO.Instance.InserirFromException("Finalizar Pedido Espelho - " + ex.Message, ex, idPedido);

                throw ex;
            }
        }

        /// <summary>
        /// Gera um orçamento do pedido espelho finalizado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idOrcamento"></param>
        public uint GeraOrcamento(uint idPedido, uint? idOrcamento)
        {
            return GeraOrcamento(null, idPedido, idOrcamento);
        }

        /// <summary>
        /// Gera um orçamento do pedido espelho finalizado
        /// </summary>
        public uint GeraOrcamento(GDASession sessao, uint idPedido, uint? idOrcamento)
        {
            uint idCliente = PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido);
            int tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(sessao, idPedido);

            var lstItemProj = ItemProjetoDAO.Instance.GetForGerarOrcamento(sessao, idPedido).ToArray();
            Orcamento orcamento;

            // Se houver orçamento já gerado para este pedido, apaga os produtos, senão,
            // gera um novo orçamento com as ferragens e alumínios.
            if (idOrcamento > 0)
            {
                // Exclui os cálculos de projetos que possam estar no orçamento
                foreach (ItemProjeto ip in ItemProjetoDAO.Instance.GetByOrcamento(sessao, idOrcamento.Value))
                    ItemProjetoDAO.Instance.Delete(sessao, ip);

                // Exclui os produtos do orçamento remanescentes
                ProdutosOrcamentoDAO.Instance.DeleteByOrcamento(sessao, idOrcamento.Value);

                if (lstItemProj.Length == 0)
                    return 0;

                orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(sessao, idOrcamento.Value);
            }
            else
            {
                if (lstItemProj.Length == 0)
                    return 0;

                Cliente cliente = ClienteDAO.Instance.GetElementByPrimaryKey(sessao, idCliente);

                orcamento = new Orcamento
                {
                    IdPedidoEspelho = idPedido,
                    IdCliente = idCliente,
                    Situacao = (int)Orcamento.SituacaoOrcamento.EmAberto,
                    TelCliente = cliente.Telefone,
                    CelCliente = cliente.TelCel,
                    Email = cliente.Email != null ? cliente.Email.Split(';')[0] : null,
                    Endereco = cliente.Endereco + " nº " + cliente.Numero,
                    Bairro = cliente.Bairro,
                    Cidade = CidadeDAO.Instance.GetNome(sessao, (uint?)cliente.IdCidade),
                    Cep = cliente.Cep,
                    TipoOrcamento = (int)Orcamento.TipoOrcamentoEnum.Revenda,
                    TipoEntrega = tipoEntrega,
                    PrazoEntrega = OrcamentoConfig.DadosOrcamento.PrazoEntregaOrcamento,
                    Validade = OrcamentoConfig.DadosOrcamento.ValidadeOrcamento,
                    FormaPagto = OrcamentoConfig.DadosOrcamento.FormaPagtoOrcamento,
                    IdFuncionario = ClienteDAO.Instance.ObtemIdFunc(sessao, idCliente)
                };

                idOrcamento = OrcamentoDAO.Instance.Insert(sessao, orcamento);
            }

            // Insere os materiais calculado no orçamento
            foreach (ItemProjeto ip in lstItemProj)
            {
                ProdutosOrcamento po = new ProdutosOrcamento
                {
                    IdOrcamento = idOrcamento.Value,
                    Ambiente = ip.Ambiente,
                    Qtde = 1
                };

                var idProdParent = ProdutosOrcamentoDAO.Instance.Insert(sessao, po);

                foreach (var mip in ItemProjetoDAO.Instance.CalculaMateriais(sessao, ip, idCliente, tipoEntrega, true))
                {
                    // Não insere os alumínios
                    if (GrupoProdDAO.Instance.IsAluminio((int)mip.IdGrupoProd))
                        continue;

                    ProdutosOrcamento prodOrca = new ProdutosOrcamento
                    {
                        IdOrcamento = idOrcamento.Value,
                        IdProdParent = idProdParent,
                        IdProduto = mip.IdProd,
                        NumSeq =
                            ProdutosOrcamentoDAO.Instance.ObtemValorCampo<uint>(sessao, "numSeq",
                                "idProd=" + idProdParent),
                        Ambiente = ip.Ambiente,
                        Descricao = ProdutoDAO.Instance.GetDescrProduto(sessao, (int)mip.IdProd),
                        Total = mip.Total,
                        Qtde = mip.Qtde,
                        ValorProd = mip.Valor,
                        Altura = mip.Altura,
                        AlturaCalc = mip.AlturaCalc,
                        Largura = mip.Largura,
                        Redondo = mip.Redondo,
                        ValorTabela =
                            ProdutoDAO.Instance.GetValorTabela(sessao, (int)mip.IdProd, tipoEntrega, idCliente,
                                ClienteDAO.Instance.IsRevenda(sessao, idCliente), false, 0, (int?)idPedido, null, null)
                    };

                    if (prodOrca.IdProduto != null)
                    {
                        prodOrca.TipoCalculoUsado = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)prodOrca.IdProduto.Value);
                        prodOrca.Custo = mip.Custo;
                        prodOrca.Espessura = mip.Espessura;

                        if (prodOrca.Qtde != null && prodOrca.ValorProd != null)
                        {
                            ValorTotal.Instance.Calcular(
                                sessao,
                                orcamento,
                                prodOrca,
                                Helper.Calculos.Estrategia.ValorTotal.Enum.ArredondarAluminio.ArredondarApenasCalculo,
                                true,
                                prodOrca.Beneficiamentos.CountAreaMinima
                            );
                        }
                    }

                    ProdutosOrcamentoDAO.Instance.Insert(sessao, prodOrca);
                }
            }

            return idOrcamento.Value;
        }

        /// <summary>
        /// Atualiza a situação do pedido de acordo com as impressões.
        /// </summary>
        /// <param name="idPedido"></param>
        public void AtualizaSituacaoImpressao(uint idPedido)
        {
            AtualizaSituacaoImpressao(null, idPedido);
        }

        /// <summary>
        /// Atualiza a situação do pedido de acordo com as impressões.
        /// </summary>
        public void AtualizaSituacaoImpressao(GDASession session, uint idPedido)
        {
            PedidoEspelho.SituacaoPedido pedSituacao = ObtemValorCampo<PedidoEspelho.SituacaoPedido>(session, "situacao", "idPedido=" + idPedido);

            // Altera situação para finalizado, se a situação for diferente de impresso comum
            if (pedSituacao != PedidoEspelho.SituacaoPedido.ImpressoComum)
                AlteraSituacao(session, idPedido, !IsPedidoImpresso(session, idPedido) ? PedidoEspelho.SituacaoPedido.Finalizado : PedidoEspelho.SituacaoPedido.Impresso);
        }

        #endregion

        #region Reabrir Pedido finalizado

        /// <summary>
        /// Reabre pedido finalizado
        /// </summary>
        public void ReabrirPedidoComTransacao(uint idPedido)
        {
            lock (_reabrirPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        ReabrirPedido(transaction, idPedido, false);

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
        /// Reabre pedido finalizado
        /// </summary>
        public void ReabrirPedido(GDASession session, uint idPedido, bool cancelamento)
        {
            //Valida se o pedido ja tem OC se tiver não pode reabrir o espelho
            if (PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, idPedido))
                throw new Exception("Este pedido possui ordem de carga gerada, cancele-a antes de reabrí-lo.");

            // Verifica se o pedido possui alguma compra de produtos de beneficiamento gerada, caso tenha então não pode ser reaberto.
            if (objPersistence.ExecuteSqlQueryCount(session,
                @"Select Count(*) From pedidos_compra pc
                Left Join compra c ON (pc.idCompra=c.idCompra)
                Where pc.idPedido=" + idPedido + @" And pc.produtoBenef
                    And c.situacao<>" + (int)Compra.SituacaoEnum.Cancelada) > 0)
                throw new Exception("Este pedido possui compra gerada a partir de produtos associados a " +
                    "beneficiamentos, cancele-a antes de reabrí-lo.");

            //Valida se o espelho possui peças de composição, se tiver nao pode reabrir, tem que ser excluido e editado no pedido
            if (!cancelamento && PossuiProdutosComposicao(session, (int)idPedido))
                throw new Exception("Este pedido possui peças do tipo do subgrupo composição ou laminado e não pode ser editado, cancele e faça a edição no pedido comercial.");

            var pedido = GetElementByPrimaryKey(session, idPedido);

            if (pedido.Importado && pedido.PedidoConferido)
                throw new Exception("Cancele a conferência do pedido importado antes de reabri-lo");

            /* Chamado 46871. */
            if (!pedido.ExibirReabrir)
                throw new Exception("O pedido não pode ser reaberto, para reabrí-lo ele não pode estar liberado ou liberado parcialmente," +
                    " não pode estar associado à uma OC e sua situação deve ser Finalizado.");

            var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, idPedido, false);

            /* Chamado 57230. */
            if (PedidoDAO.Instance.TemVolume(session, idPedido))
                throw new Exception("O pedido PCP não pode ser reaberto, pois, possui volume gerado. Cancele o volume para reabrir o pedido PCP.");

            /* Chamado 33285. */
            if (objPersistence.ExecuteSqlQueryCount(session,
                string.Format(@"SELECT COUNT(*) FROM produtos_pedido_espelho WHERE QtdImpresso>0 AND IdPedido={0};",
                    idPedido)) > 0)
            {
                foreach (var produtoPedidoEspelho in produtosPedidoEspelho)
                {
                    if (ProdutoImpressaoDAO.Instance.QuantidadeImpressa(session, (int)produtoPedidoEspelho.IdProdPed) > 0)
                        throw new Exception("Uma ou mais peças do pedido estão impressas, não é possível reabri-lo.");
                    else
                        objPersistence.ExecuteCommand(session,
                            string.Format("UPDATE produtos_pedido_espelho SET QtdImpresso=0 WHERE IdProdPed={0}", produtoPedidoEspelho.IdProdPed));
                }
            }

            // Apaga peças que possam ter sido gerada na produção
            bool sql2Executado = false;
            foreach (uint id in ProdutosPedidoEspelhoDAO.Instance.ExecuteMultipleScalar<uint>(session,
                "Select idProdPed From produtos_pedido_espelho Where idPedido=" + idPedido))
            {
                // Apaga as etiquetas desse pedido
                ImpressaoEtiquetaDAO.Instance.ApagarEtiquetasOtimizacao(session, id, !sql2Executado, false);
                sql2Executado = true;
            }

            // Volta a situação do pedido para em aberto
            AlteraSituacao(session, idPedido, PedidoEspelho.SituacaoPedido.Aberto);

            LogAlteracaoDAO.Instance.LogPedidoEspelho(session, pedido);

            // Apaga arquivos de mesa de corte e imagens associadas
            foreach (var ppe in produtosPedidoEspelho)
            {
                ArquivoMesaCorteDAO.Instance.ApagarArquivoMesaCorte(session, ppe.IdProdPed);

                string imagem = ppe.ImagemUrl;

                if (!string.IsNullOrEmpty(imagem))
                {
                    try
                    {
                        if (Utils.ArquivoExiste(imagem))
                            File.Delete(imagem);
                    }
                    catch (Exception ex)
                    {
                        ErroDAO.Instance.InserirFromException(
                            string.Format("Reabrir Pedido Espelho. Excluir imagem associada ao produto pedido espelho de ID: {0}.",
                                ppe.IdProdPed), ex);
                    }
                }
            }

            var diretorioExiste = false;

            using (Glass.Seguranca.AutenticacaoRemota.Autenticar())
            {
                if (PCPConfig.EmpresaGeraArquivoFml)
                    diretorioExiste = Directory.Exists(PCPConfig.CaminhoSalvarFml);

                if (!diretorioExiste && PCPConfig.EmpresaGeraArquivoDxf)
                    diretorioExiste = Directory.Exists(PCPConfig.CaminhoSalvarDxf);

                if (!diretorioExiste && PCPConfig.EmpresaGeraArquivoSGlass)
                    diretorioExiste = Directory.Exists(PCPConfig.CaminhoSalvarProgramSGlass);

                // Apaga arquivos gerados pela intermac
                var dirCaminhoIntermac = new DirectoryInfo(PCPConfig.CaminhoSalvarIntermac);
                var arquivosIntermac = dirCaminhoIntermac.GetFiles(string.Format("{0}*.CNI", idPedido));
                foreach (var foundFile in arquivosIntermac)
                    File.Delete(foundFile.FullName);
            }

            if (diretorioExiste)
            {
                foreach (var prodPedEsp in ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, idPedido, false))
                {
                    //Busca as etiquetas da peça
                    var etiquetas = PecaItemProjetoDAO.Instance.ObtemEtiquetas(session, prodPedEsp.IdPedido,
                        prodPedEsp.IdProdPed, Glass.Conversoes.StrParaInt(prodPedEsp.Qtde.ToString()));

                    //Percorre as etiquetas da peça
                    foreach (var etiqueta in etiquetas.Split(','))
                    {
                        string forma;
                        var nomeArquivoDxf = ImpressaoEtiquetaDAO.Instance.ObterNomeArquivo(session, null, TipoArquivoMesaCorte.DXF, (int)prodPedEsp.IdProdPed, etiqueta, false, out forma, false);
                        var nomeArquivoFml = ImpressaoEtiquetaDAO.Instance.ObterNomeArquivo(session, null, TipoArquivoMesaCorte.FML, (int)prodPedEsp.IdProdPed, etiqueta, false, out forma, false);

                        using (Glass.Seguranca.AutenticacaoRemota.Autenticar())
                        {
                            if (File.Exists(PCPConfig.CaminhoSalvarFml + nomeArquivoFml))
                                File.Delete(PCPConfig.CaminhoSalvarFml + nomeArquivoFml);

                            if (File.Exists(PCPConfig.CaminhoSalvarDxf + nomeArquivoDxf))
                                File.Delete(PCPConfig.CaminhoSalvarDxf + nomeArquivoDxf);

                            var pathSglass = Path.Combine(PCPConfig.CaminhoSalvarProgramSGlass, Path.GetFileNameWithoutExtension(nomeArquivoDxf) + ".drawing");
                            if (File.Exists(pathSglass))
                                File.Delete(pathSglass);
                        }
                    }
                }
            }

            var idsProdQtde = new Dictionary<int, float>();

            // Se tipoPedido diferente de Produção e não realiza saída de estoque ao confirmar
            if (PedidoDAO.Instance.GetTipoPedido(session, idPedido) != Pedido.TipoPedidoEnum.Producao && !FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar)
            {
                // Insere na reserva peças do pedido original
                foreach (var ppe in ProdutosPedidoDAO.Instance.GetByPedido(session, idPedido, false))
                {
                    var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session, (int)ppe.IdProd);

                    var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;
                    var ml = tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6;

                    if (!idsProdQtde.ContainsKey((int)ppe.IdProd))
                        idsProdQtde.Add((int)ppe.IdProd,
                            m2 ? ppe.TotM : ml ? ppe.Qtde * ppe.Altura : ppe.Qtde);
                    else
                        idsProdQtde[(int)ppe.IdProd] +=
                            m2 ? ppe.TotM : ml ? ppe.Qtde * ppe.Altura : ppe.Qtde;
                }

                if (idsProdQtde.Count > 0)
                {
                    var idLoja = PedidoDAO.Instance.ObtemIdLoja(session, idPedido);
                    ProdutoLojaDAO.Instance.ColocarReserva(session, (int)idLoja, idsProdQtde, null, null, (int)idPedido, null, null, null,
                        null, "PedidoEspelhoDAO - ReabrirPedido");
                }

                idsProdQtde.Clear();

                // Remove da reserva peças da conferência
                foreach (var ppe in ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, idPedido, false))
                {
                    var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session, (int)ppe.IdProd);

                    var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;
                    var ml = tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6;

                    if (!idsProdQtde.ContainsKey((int)ppe.IdProd))
                        idsProdQtde.Add((int)ppe.IdProd,
                            m2 ? ppe.TotM : ml ? ppe.Qtde * ppe.Altura : ppe.Qtde);
                    else
                        idsProdQtde[(int)ppe.IdProd] +=
                            m2 ? ppe.TotM : ml ? ppe.Qtde * ppe.Altura : ppe.Qtde;
                }

                if (idsProdQtde.Count > 0)
                {
                    var idLoja = PedidoDAO.Instance.ObtemIdLoja(session, idPedido);
                    ProdutoLojaDAO.Instance.TirarReserva(session, (int)idLoja, idsProdQtde, null, null, (int)idPedido, null, null, null,
                        null, "PedidoEspelhoDAO - ReabrirPedido");
                }
            }
        }

        #endregion

        #region Gerar valor excedente

        /// <summary>
        /// Verifica se foi gerado crédito ao finalizar o pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="diferenca"></param>
        /// <returns></returns>
        public bool GerouCreditoAoFinalizar(uint idPedido, decimal diferenca)
        {
            return ExecuteScalar<bool>(@"
                Select Count(*) 
                From caixa_geral 
                Where idPedido=" + idPedido + @" 
                    And idConta=" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) + @"
                    And valorMov=?diferenca",
                new GDAParameter("?diferenca", diferenca));
        }

        /// <summary>
        /// Gera uma conta a receber do valor excedente se houver
        /// </summary>
        /// <param name="idPedido"></param>
        public void GerarExcedente(uint idPedido)
        {
            try
            {
                // Exclui conta a receber com o valor excedente do pedido caso já tenha sido gerada
                ContasReceberDAO.Instance.ExcluiExcedentePedido(null, idPedido);

                // Gera conta a receber caso o valor do pedido/projeto modificado seja maior que o valor do pedido inicial
                string sql = @"Select pe.total-p.total From pedido p 
                    Inner Join pedido_espelho pe On (p.idPedido=pe.idPedido) 
                    Where p.idPedido=" + idPedido;

                decimal diferenca = ExecuteScalar<decimal>(sql);

                if (diferenca > 0)
                {
                    ContasReceber conta = new ContasReceber
                    {
                        IdLoja = UserInfo.GetUserInfo.IdLoja,
                        IdCliente = PedidoDAO.Instance.ObtemIdCliente(idPedido),
                        IdPedido = idPedido,
                        IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorExcedente),
                        DataVec = DateTime.Now,
                        ValorVec = diferenca,
                        ValorExcedentePCP = true
                    };
                    ContasReceberDAO.Instance.Insert(conta);
                }
                else
                    throw new Exception(
                        string.Format("Não há valor excedente. O total do pedido após a conferência ficou {0} total do pedido original.",
                            diferenca == 0 ? "igual ao" : "menor que o"));
            }
            catch
            {
                ContasReceberDAO.Instance.ExcluiExcedentePedido(null, idPedido);

                throw;
            }
        }

        #endregion

        #region Altera situação do pedido

        public int AlteraSituacao(GDASession session, uint idPedido, PedidoEspelho.SituacaoPedido situacao)
        {
            string sql = "Update pedido_espelho Set Situacao=" + (int)situacao;

            if (situacao == PedidoEspelho.SituacaoPedido.Finalizado)
                sql += ", dataConf=now()";

            if (situacao == PedidoEspelho.SituacaoPedido.Aberto)
                sql += ", situacaoCnc=0, dataProjetoCnc=null, usuProjetoCnc=null";

            return objPersistence.ExecuteCommand(session, sql + " Where idPedido=" + idPedido);
        }

        public int AlteraSituacao(GDASession session, List<uint> lstIdPedido, PedidoEspelho.SituacaoPedido situacao)
        {
            var ids = lstIdPedido.Aggregate(string.Empty, (current, id) => current + (id + ","));

            return objPersistence.ExecuteCommand(session, "Update pedido_espelho Set Situacao=" + (int)situacao + " Where idPedido In (" + ids.TrimEnd(',') + ")");
        }

        #endregion

        #region Cancelar pedido espelho

        /// <summary>
        /// Cancela espelho do pedido
        /// </summary>
        public void CancelarEspelhoComTransacao(uint idPedido)
        {
            lock (_cancelarEspelhoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        CancelarEspelho(transaction, idPedido);

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
        /// Cancela espelho do pedido
        /// </summary>
        public void CancelarEspelho(GDASession session, uint idPedido)
        {
            PedidoEspelho pedEsp = GetElement(session, idPedido);

            var idsItemProjeto = String.Empty;
            var idsProdPedEsp = String.Empty;
            var idsProdPedClone = String.Empty;
            var possuiProdutosComposicao = PossuiProdutosComposicao(session, (int)idPedido);

            // Verifica se a conferência está em aberto
            if (!possuiProdutosComposicao &&
                pedEsp.Situacao != (int)PedidoEspelho.SituacaoPedido.Processando &&
                pedEsp.Situacao != (int)PedidoEspelho.SituacaoPedido.Aberto &&
                pedEsp.Situacao != (int)PedidoEspelho.SituacaoPedido.Finalizado)
                throw new Exception("Apenas conferência em aberto pode ser cancelada.");

            // Verifica se o pedido já foi liberado
            if (PedidoConfig.LiberarPedido &&
                objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From pedido Where idPedido=" + idPedido +
                    " And situacao=" + (int)Pedido.SituacaoPedido.Confirmado) > 0)
                throw new Exception(
                    "O pedido desta conferência já foi liberado, a conferência não pode ser cancelada.");
            
            /* Chamado 57230. */
            if (PedidoDAO.Instance.TemVolume(session, idPedido))
                throw new Exception("O pedido PCP não pode ser excluído, pois, possui volume gerado. Cancele o volume para excluir o pedido PCP.");

            // Se possuir peças de composição nao pode reabrir sendo assim antes de deletar tem que ser reaberto
            // para os procedimentos de reabertura sejam realizados.
            if (possuiProdutosComposicao)
                ReabrirPedido(session, idPedido, true);

            // Exclui conta a receber com o valor excedente do pedido caso já tenha sido gerada.
            ContasReceberDAO.Instance.ExcluiExcedentePedido(session, idPedido);

            // Apaga arquivos de mesa de corte e imagens associadas
            foreach (ProdutosPedidoEspelho ppe in ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, idPedido, false))
            {
                ArquivoMesaCorteDAO.Instance.ApagarArquivoMesaCorte(session, ppe.IdProdPed);

                try
                {
                    var arquivosImagem = Directory.GetFiles(Utils.GetPecaProducaoPath, ppe.IdProdPed.ToString().PadLeft(10, '0') + "_*");

                    foreach (var arquivoImagem in arquivosImagem)
                        File.Delete(arquivoImagem);

                    var caminhoDxf = PCPConfig.CaminhoSalvarCadProject(true) + ppe.IdProdPed + ".dxf";
                    if (File.Exists(caminhoDxf))
                        File.Delete(caminhoDxf);

                    var caminhoSvg = PCPConfig.CaminhoSalvarCadProject(true) + ppe.IdProdPed + ".svf";
                    if (File.Exists(caminhoSvg))
                        File.Delete(caminhoSvg);
                    
                    // Apaga arquivos gerados pela intermac
                    var dirCaminhoIntermac = new DirectoryInfo(PCPConfig.CaminhoSalvarIntermac);
                    var arquivosIntermac = dirCaminhoIntermac.GetFiles(string.Format("{0}'*.CNI", idPedido));
                    foreach (var foundFile in arquivosIntermac)
                        File.Delete(foundFile.FullName);
                }
                catch
                {
                }
            }

            idsItemProjeto = String.Join(",",
                ExecuteMultipleScalar<string>(session,
                    "Select Cast(idItemProjeto as char) from item_projeto where idPedidoEspelho=" + idPedido)
                    .ToArray());

            idsProdPedEsp = String.Join(",",
                ExecuteMultipleScalar<string>(session,
                    "Select Cast(idProdPed as char) from produtos_pedido_espelho where idPedido=" + idPedido)
                    .ToArray());

            if (String.IsNullOrEmpty(idsItemProjeto))
                idsItemProjeto = "0";

            // Chamado 12692. Ao recuperar os produtos do pedido (clone) pelos produtos do espelho, estava ocorrendo um erro, pois,
            // não existia produto espelho. Por isso, inserimos esta condição que verifica se existem produtos no pedido espelho e, caso
            // existam, recupera os produtos do pedido (clone).
            if (String.IsNullOrEmpty(idsProdPedEsp))
            {
                idsProdPedEsp = "0";
                idsProdPedClone = "0";
            }
            else
                idsProdPedClone = String.Join(",",
                    ExecuteMultipleScalar<string>(session,
                        "Select Cast(idProdPed as char) from produtos_pedido where invisivelPedido=true and idProdPedEsp in (" +
                        idsProdPedEsp + ")").ToArray());

            if (String.IsNullOrEmpty(idsProdPedClone))
                idsProdPedClone = "0";


            // Exclui dados relacionadas à conferência do pedido
            objPersistence.ExecuteCommand(session, "delete from produto_pedido_benef where idProdPed in  (" + idsProdPedClone + @")");
            objPersistence.ExecuteCommand(session, "delete from produto_pedido_rentabilidade WHERE IdProdPed IN (" + idsProdPedClone + @")");
            objPersistence.ExecuteCommand(session, "delete from produtos_pedido where invisivelPedido=true and idProdPedEsp in (" + idsProdPedEsp + @")");
            objPersistence.ExecuteCommand(session, "update produtos_pedido set invisivelFluxo=false, idProdPedEsp=null where idPedido=?id", new GDAParameter("?id", idPedido));
            objPersistence.ExecuteCommand(session, "delete from produto_pedido_espelho_benef where idProdPed in (" + idsProdPedEsp + @")");
            objPersistence.ExecuteCommand(session, "delete from pecas_excluidas_sistema where idProdPed in (" + idsProdPedEsp + @")");
            objPersistence.ExecuteCommand(session, "delete from produto_pedido_espelho_rentabilidade WHERE IdProdPed in (" + idsProdPedEsp + ")");
            objPersistence.ExecuteCommand(session, "delete from produtos_pedido_espelho where idPedido=?id", new GDAParameter("?id", idPedido));
            objPersistence.ExecuteCommand(session, 
                @"DELETE FROM ambiente_pedido_espelho_rentabilidade WHERE IdAmbientePedido IN 
                    (SELECT IdAmbientePedido FROM ambiente_pedido_espelho WHERE IdPedido=?id)", 
                new GDAParameter("?id", idPedido));
            objPersistence.ExecuteCommand(session, "delete from ambiente_pedido_espelho where idPedido=?id;", new GDAParameter("?id", idPedido));
            objPersistence.ExecuteCommand(session, "delete from pedido_espelho_rentabilidade where idPedido=?id;", new GDAParameter("?id", idPedido));

            // Exclui dados relacionados à cálculos de projeto
            if (!String.IsNullOrEmpty(idsItemProjeto))
                objPersistence.ExecuteCommand(session, "delete from medida_item_projeto where idItemProjeto in (" + idsItemProjeto + @")");
            objPersistence.ExecuteCommand(session, "delete from material_item_projeto where idItemProjeto in (" + idsItemProjeto + @")");
            objPersistence.ExecuteCommand(session, "delete from peca_item_projeto where idItemProjeto in (" + idsItemProjeto + @")");
            objPersistence.ExecuteCommand(session, "delete from item_projeto where idPedidoEspelho=?id;", new GDAParameter("?id", idPedido));

            // Exclui o pedido
            objPersistence.ExecuteCommand(session, @"
            delete from pedido_espelho where idPedido=?id;",
                new GDAParameter("?id", idPedido));

            // Reabre o pedido para edição
            if (PedidoConfig.LiberarPedido)
            {
                objPersistence.ExecuteCommand(session, "Update pedido set situacao=" +
                    (int)Pedido.SituacaoPedido.Conferido + " Where idPedido=" +
                    idPedido);

                // Atualiza o saldo da obra
                uint idObra = PedidoDAO.Instance.ObtemValorCampo<uint>(session, "idObra", "idPedido=" + idPedido);
                if (idObra > 0)
                    ObraDAO.Instance.AtualizaSaldo(session, idObra, false);
            }

            if (PedidoDAO.Instance.GetTipoPedido(session, idPedido) != Pedido.TipoPedidoEnum.Producao)
            {
                var idsProdQtde = new Dictionary<int, float>();

                foreach (var ppe in ProdutosPedidoDAO.Instance.GetByPedido(session, idPedido, false))
                {
                    var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session, (int)ppe.IdProd);

                    var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;
                    var ml = tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6;

                    if (!idsProdQtde.ContainsKey((int)ppe.IdProd))
                        idsProdQtde.Add((int)ppe.IdProd,
                            m2 ? ppe.TotM : ml ? ppe.Qtde * ppe.Altura : ppe.Qtde);
                    else
                        idsProdQtde[(int)ppe.IdProd] +=
                            m2 ? ppe.TotM : ml ? ppe.Qtde * ppe.Altura : ppe.Qtde;
                }

                if (idsProdQtde.Count > 0)
                {
                    var idLoja = PedidoDAO.Instance.ObtemIdLoja(session, idPedido);
                    ProdutoLojaDAO.Instance.TirarReserva(session, (int)idLoja, idsProdQtde, null, null, (int)idPedido, null, null, null,
                        null, "PedidoEspelhoDAO - CancelarEspelho");
                }

                //Salva cancelamento no log do pedido
                var logAlteracaoCancelamento = new LogAlteracao();
                logAlteracaoCancelamento.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                logAlteracaoCancelamento.IdRegistroAlt = (int)idPedido;
                logAlteracaoCancelamento.DataAlt = DateTime.Now;
                logAlteracaoCancelamento.IdFuncAlt = UserInfo.GetUserInfo.CodUser;
                logAlteracaoCancelamento.Referencia = LogAlteracao.GetReferencia(session, logAlteracaoCancelamento.Tabela,
                    (uint)logAlteracaoCancelamento.IdRegistroAlt);
                logAlteracaoCancelamento.Campo = "Situação";
                logAlteracaoCancelamento.ValorAnterior = PedidoDAO.Instance.GetSituacaoPedido((int)Pedido.SituacaoPedido.ConfirmadoLiberacao);
                logAlteracaoCancelamento.ValorAtual = PedidoDAO.Instance.GetSituacaoPedido((int)Pedido.SituacaoPedido.Conferido);
                LogAlteracaoDAO.Instance.Insert(session, logAlteracaoCancelamento);
            }
        }

        #endregion

        #region Verifica se há um espelho gerado para o pedido

        /// <summary>
        /// Verifica se há um espelho gerado para o pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExisteEspelho(uint idPedido)
        {
            return ExisteEspelho(null, idPedido);
        }

        /// <summary>
        /// Verifica se há um espelho gerado para o pedido.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExisteEspelho(GDASession session, uint idPedido)
        {
            string sql = "select count(*) from pedido_espelho where idPedido=" + idPedido;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Verifica se o pedido está finalizado

        public bool IsPedidoFinalizado(uint idPedido)
        {
            string sql = "select coalesce(situacao, 0) from pedido_espelho where idPedido=" + idPedido;
            return objPersistence.ExecuteScalar(sql).ToString().StrParaInt() == (int)PedidoEspelho.SituacaoPedido.Finalizado;
        }

        #endregion

        #region Verifica se o pedido está conferido

        public bool IsPedidoConferido(uint idPedido)
        {
            string sql = "select PedidoConferido from pedido_espelho where idPedido=" + idPedido;
            return objPersistence.ExecuteScalar(sql).ToString().ToLower() == "true";
        }

        #endregion

        #region Verifica se os pedidos está conferido

        /// <summary>
        ///  Verifica se os pedidos importado está conferido antes de imprimir
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        public string VerificarPedidoConferidos(string idsPedido)
        {
            var sql = ExecuteMultipleScalar<string>(string.Format(@"SELECT p.idPedido FROM pedido_espelho pe 
                                INNER JOIN  pedido p ON (p.idPedido = pe.idPedido) 
                                WHERE pe.idPedido IN ({0}) AND !pe.PedidoConferido AND p.importado;", idsPedido));

            return string.Join(", ", sql) ?? "";
        }

        #endregion

        #region Altera a situação conferido do pedido interno
        /// <summary>
        /// Altera a situação conferido do pedido interno
        /// </summary>
        /// <param name="idPedido"></param>
        public void AlteraSituacaoPedidoImportadoConferido(uint idPedido)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var pedEsp = GetElement(idPedido);
                    if (pedEsp == null)
                        throw new Exception("Pedido não encontrado.");

                    if (pedEsp.PedidoConferido && IsPedidoImpresso(transaction, idPedido))
                        throw new Exception("Existe pelo menos uma peça impressa para este pedido");

                    if (pedEsp.PedidoConferido)
                        pedEsp.PedidoConferido = false;
                    else
                        pedEsp.PedidoConferido = true;

                    LogAlteracaoDAO.Instance.LogPedidoEspelho(transaction, pedEsp, LogAlteracaoDAO.SequenciaObjeto.Novo);
                    Update(transaction, pedEsp);
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

        #endregion

        #region Verifica se o pedido está impresso

        /// <summary>
        /// Verifica se o pedido está impresso.
        /// </summary>
        public bool IsPedidoImpresso(GDASession session, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session, @"select count(*) from produto_impressao pi 
                inner join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                where ie.situacao=" + (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa +
                " and pi.idPedido=" + idPedido + " and !coalesce(pi.cancelado,false)") > 0;
        }

        #endregion

        #region Altera a data de entrega da fábrica

        public void VerificaCapacidadeProducaoSetor(uint idPedido, DateTime dataFabrica, float totM2Adicionar, uint idProcessoAdicionar)
        {
            VerificaCapacidadeProducaoSetor(null, idPedido, dataFabrica, totM2Adicionar, idProcessoAdicionar);
        }

        public void VerificaCapacidadeProducaoSetor(GDASession session, uint idPedido, DateTime dataFabrica, float totM2Adicionar, uint idProcessoAdicionar)
        {
            // Valida a capacidade de produção por setor através da data de fábrica do pedido:
            // só valida se a configuração estiver selecionada e se a data de fábrica tiver sido alterada
            CapacidadeProducaoDAO.Instance.ValidaDataFabricaPedido(session, idPedido, dataFabrica, ObtemDataFabrica(session, idPedido).GetValueOrDefault(),
                totM2Adicionar, idProcessoAdicionar);
        }

        /// <summary>
        /// Altera a data de fábrica do pedido.
        /// </summary>
        public void AlterarDataFabrica(GDASession session, string idsPedidos, DateTime novaData)
        {
            AlterarDataFabrica(session, idsPedidos, novaData, false);
        }

        /// <summary>
        /// Altera a data de fábrica do pedido.
        /// </summary>
        public void AlterarDataFabrica(string idsPedidos, DateTime novaData, bool alterarReposicao)
        {
            AlterarDataFabrica(null, idsPedidos, novaData, alterarReposicao);
        }

        /// <summary>
        /// Altera a data de fábrica do pedido.
        /// </summary>
        public void AlterarDataFabrica(GDASession session, string idsPedidos, DateTime novaData, bool alterarReposicao)
        {
            var aux = idsPedidos.Split(',');

            var idsPedido = new List<uint>();
            
            /* Chamado 56812. */
            if (novaData.Date < DateTime.Now.Date)
                do { novaData = novaData.AddDays(1); } while (!novaData.DiaUtil() || novaData.Date < DateTime.Now.Date);

            foreach (var a in aux)
            {
                uint idPedido = a.StrParaUint();
                if (idsPedido.Contains(idPedido))
                    continue;

                bool temPecaReposta = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<int>(session, "Count(*)",
                    "pecaReposta=true And idProdPed In (Select idProdPed From produtos_pedido Where idPedido=" + idPedido + ")") > 0;

                if ((PedidoDAO.Instance.IsPedidoReposicao(a) || temPecaReposta) && !alterarReposicao)
                    continue;

                var dataEntrega = PedidoDAO.Instance.ObtemDataEntrega(session, idPedido);
                
                // Altera a data de fábrica somente se a data de entrega for maior que a nova data
                if (dataEntrega == null || novaData <= dataEntrega)
                {
                    VerificaCapacidadeProducaoSetor(session, idPedido, novaData, 0, 0);

                    PedidoEspelho p = GetElementByPrimaryKey(session, idPedido);

                    objPersistence.ExecuteCommand(session, "update pedido_espelho set dataFabrica=?data where idPedido=" + idPedido, new GDAParameter("?data", novaData));

                    LogAlteracaoDAO.Instance.LogPedidoEspelho(session, p, LogAlteracaoDAO.SequenciaObjeto.Atual);
                }
            }
        }
        
        #endregion

        #region Atualiza os dados da tela

        public void UpdateDados(GDASession session, PedidoEspelho objUpdate)
        {
            PedidoEspelho atual = GetElementByPrimaryKey(session, objUpdate.IdPedido);
            RemoveComissaoDescontoAcrescimo(session, atual, objUpdate);
            AplicaComissaoDescontoAcrescimo(session, atual, objUpdate);

            if (objUpdate.DataFabrica != null)
            {
                VerificaCapacidadeProducaoSetor(session, objUpdate.IdPedido, objUpdate.DataFabrica.Value, 0, 0);

                // Atualiza a data de fábrica e a observação
                string sql = @"update pedido_espelho set dataFabrica=?df, obs=?obs, percComissao=?pc, valorComissao=(select sum(valorComissao) 
                    from produtos_pedido_espelho where idPedido=" + objUpdate.IdPedido + @"), idComissionado=?idCom,
                    tipoAcrescimo=?ta, acrescimo=?a, tipoDesconto=?td, desconto=?d where idPedido=" + objUpdate.IdPedido;

                objPersistence.ExecuteCommand(session, sql, new GDAParameter("?df", objUpdate.DataFabrica),
                    new GDAParameter("?obs", objUpdate.Obs), new GDAParameter("?ta", objUpdate.TipoAcrescimo),
                    new GDAParameter("?a", objUpdate.Acrescimo), new GDAParameter("?td", objUpdate.TipoDesconto),
                    new GDAParameter("?d", objUpdate.Desconto), new GDAParameter("?pc", objUpdate.PercComissao),
                    new GDAParameter("?idCom", objUpdate.IdComissionado));
            }

            UpdateTotalPedido(session, objUpdate, true);

            LogAlteracaoDAO.Instance.LogPedidoEspelho(atual, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Verifica se no pedido possui cálculos de projeto

        /// <summary>
        /// Verifica se no pedido possui cálculos de projeto
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiCalculoProjeto(uint idPedido)
        {
            string sql = "Select Count(*) From ambiente_pedido_espelho Where idItemProjeto>0 And idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se o pedido possui orçamento gerado

        /// <summary>
        /// Verifica se o pedido espelho possui orçamento gerado
        /// </summary>
        public bool PossuiOrcamentoGerado(uint idPedidoEspelho)
        {
            return PossuiOrcamentoGerado(null, idPedidoEspelho);
        }

        /// <summary>
        /// Verifica se o pedido espelho possui orçamento gerado
        /// </summary>
        public bool PossuiOrcamentoGerado(GDASession sessao, uint idPedidoEspelho)
        {
            string sql = "Select Count(*) From orcamento Where idPedidoEspelho=" + idPedidoEspelho;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Retorna campos do pedido

        /// <summary>
        /// Retorna situação do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public PedidoEspelho.SituacaoPedido ObtemSituacao(uint idPedido)
        {
            return ObtemSituacao(null, idPedido);
        }

        /// <summary>
        /// Retorna situação do pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public PedidoEspelho.SituacaoPedido ObtemSituacao(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<PedidoEspelho.SituacaoPedido>(sessao, "situacao", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Retorna a data de fábrica do pedido.
        /// </summary>
        public DateTime? ObtemDataFabrica(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<DateTime?>(session, "dataFabrica", "idPedido=" + idPedido);
        }

        public float ObtemPeso(uint idPedido)
        {
            return ObtemPeso(null, idPedido);
        }

        #region Comissão/Desconto/Acréscimo

        public float ObterPercentualComissao(GDASession session, int idPedido)
        {
            return ObtemValorCampo<float>(session, "PercComissao", string.Format("IdPedido={0}", idPedido));
        }

        public int ObterTipoAcrescimo(GDASession session, int idPedido)
        {
            return ObtemValorCampo<int>(session, "TipoAcrescimo", string.Format("IdPedido={0}", idPedido));
        }

        public decimal ObterAcrescimo(GDASession session, int idPedido)
        {
            return ObtemValorCampo<decimal>(session, "Acrescimo", string.Format("IdPedido={0}", idPedido));
        }

        public int ObterTipoDesconto(GDASession session, int idPedido)
        {
            return ObtemValorCampo<int>(session, "TipoDesconto", string.Format("IdPedido={0}", idPedido));
        }

        public decimal ObterDesconto(GDASession session, int idPedido)
        {
            return ObtemValorCampo<decimal>(session, "Desconto", string.Format("IdPedido={0}", idPedido));
        }

        #endregion

        public decimal ObterValorIpi(GDASession session, int idPedido)
        {
            return ObtemValorCampo<decimal>(session, "ValorIpi", string.Format("IdPedido={0}", idPedido));
        }

        public decimal ObterValorIcms(GDASession session, int idPedido)
        {
            return ObtemValorCampo<decimal>(session, "ValorIcms", string.Format("IdPedido={0}", idPedido));
        }

        public float ObtemPeso(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<float>(session, "peso", "idPedido=" + idPedido);
        }

        public float ObtemTotM2(uint idPedido)
        {
            return ObtemTotM2(null, idPedido);
        }

        public float ObtemTotM2(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<float>(session, "totM", "idPedido=" + idPedido);
        }

        public decimal ObtemTotal(uint idPedido)
        {
            return ObtemTotal(null, idPedido);
        }

        public decimal ObtemTotal(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<decimal>(sessao, "total", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtem o funcionário que colocou o desconto no pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? ObtemIdFuncDesc(uint idPedido)
        {
            return ObtemValorCampo<uint?>("idFuncDesc", "idPedido=" + idPedido);
        }
 
        /// <summary>
        /// Obtem o funcionário que colocou o desconto no pedido.
        /// </summary>
        public DateTime? ObtemDataConf(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<DateTime?>(session, "DataConf", string.Format("idPedido={0}", idPedido));
        }

        /// <summary>
        /// Obtém o idOrcamento gerado pelo pedido passado
        /// </summary>
        public uint ObtemIdOrcamentoGerado(uint idPedidoEspelho)
        {
            return  ObtemIdOrcamentoGerado(null, idPedidoEspelho);
        }

        /// <summary>
        /// Obtém o idOrcamento gerado pelo pedido passado
        /// </summary>
        public uint ObtemIdOrcamentoGerado(GDASession sessao, uint idPedidoEspelho)
        {
            string sql = "Select idOrcamento from orcamento Where idPedidoEspelho=" + idPedidoEspelho + " limit 1";

            return ExecuteScalar<uint>(sessao, sql);
        }

        public decimal ObterValorEntrega(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<decimal>("ValorEntrega", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Retorna o total do Pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetTotal(uint idPedido)
        {
            return GetTotal(null, idPedido);
        }

        /// <summary>
        /// Retorna o total do Pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetTotal(GDASession sessao, uint idPedido)
        {
            string sql = "Select Coalesce(total, 0) from pedido_espelho Where idPedido=" + idPedido;
            return ExecuteScalar<decimal>(sessao, sql);
        }

        internal decimal GetTotalSemDesconto(uint idPedido, decimal total)
        {
            return GetTotalSemDesconto(null, idPedido, total);
        }

        internal decimal GetTotalSemDesconto(GDASession sessao, uint idPedido, decimal total)
        {
            return total + GetDescontoPedido(sessao, idPedido) + GetDescontoProdutos(sessao, idPedido);
        }

        internal decimal GetTotalSemAcrescimo(uint idPedido, decimal total)
        {
            return total - GetAcrescimoPedido(idPedido) - GetAcrescimoProdutos(idPedido);
        }

        internal decimal GetTotalSemComissao(uint idPedido, decimal total)
        {
            return total - GetComissaoPedido(idPedido);
        }

        internal decimal GetTotalBruto(uint idPedido)
        {
            uint? idProjeto = PedidoDAO.Instance.ObtemIdProjeto(idPedido);
            float taxaPrazoPedido = PedidoDAO.Instance.ObtemValorCampo<float>("taxaPrazo", "idPedido=" + idPedido);
            decimal total = idProjeto == null ? ObtemValorCampo<decimal>("total", "idPedido=" + idPedido) : ProjetoDAO.Instance.GetTotal(null, idProjeto.Value);
            
            decimal acrescimo = total - GetTotalSemAcrescimo(idPedido, total);
            decimal desconto = GetTotalSemDesconto(idPedido, total) - total;
            decimal comissao = total - GetTotalSemComissao(idPedido, total);
            return total - acrescimo + desconto - comissao;
        }

        #endregion

        #region Clona item projeto para o pedido espelho passado

        /// <summary>
        /// Clona item projeto para o pedido passado
        /// </summary>
        public uint ClonaItemProjeto(GDASession sessao, uint idItemProjeto, uint idPedidoEsp)
        {
            var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, idPedidoEsp);

            // Clona item projeto
            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElement(sessao, idItemProjeto);
            itemProj.IdOrcamento = null;
            itemProj.IdProjeto = null;
            itemProj.IdPedido = null;
            itemProj.IdPedidoEspelho = idPedidoEsp;
            var idItemProjetoPedEsp = ItemProjetoDAO.Instance.Insert(sessao, itemProj);

            // Clona medidas
            MedidaItemProjetoDAO.Instance.DeleteByItemProjeto(sessao, idItemProjetoPedEsp);
            foreach (MedidaItemProjeto mip in MedidaItemProjetoDAO.Instance.GetListByItemProjeto(sessao, idItemProjeto))
            {
                mip.IdMedidaItemProjeto = 0;
                mip.IdItemProjeto = idItemProjetoPedEsp;
                MedidaItemProjetoDAO.Instance.Insert(sessao, mip);
            }

            // Apaga peças e materiais que possam existir
            PecaItemProjetoDAO.Instance.DeleteByItemProjeto(sessao, idItemProjetoPedEsp);
            MaterialItemProjetoDAO.Instance.DeleteByItemProjeto(sessao, idItemProjetoPedEsp);

            // Busca materiais
            var lstMateriais = MaterialItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, false);
            var lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(sessao, idItemProjeto, itemProj.IdProjetoModelo);

            // Chamado 13738
            if (lstPeca.Count == 0 && ProjetoModeloDAO.Instance.ObtemCodigo(sessao, itemProj.IdProjetoModelo) != "OTR01" &&
                /* Chamado 50109. */
                PecaProjetoModeloDAO.Instance.GetByModelo(sessao, itemProj.IdProjetoModelo).Count > 0)
                throw new Exception(string.Format("O projeto {0} não possui peças calculadas, portanto, " +
                    "calcule as medidas da(s) peça(s) e gere a conferência.", itemProj.Ambiente, idPedidoEsp));

            // Clona peças e materiais
            foreach (PecaItemProjeto pip in lstPeca)
            {
                // Clona as peças
                uint idPecaItemProjOld = pip.IdPecaItemProj;

                pip.Beneficiamentos = pip.Beneficiamentos;
                pip.IdPecaItemProj = 0;
                pip.IdItemProjeto = idItemProjetoPedEsp;
                uint idPecaItemProj = PecaItemProjetoDAO.Instance.Insert(sessao, pip);

                foreach (FiguraPecaItemProjeto fig in FiguraPecaItemProjetoDAO.Instance.GetForClone(sessao, idPecaItemProjOld))
                {
                    fig.IdPecaItemProj = idPecaItemProj;
                    FiguraPecaItemProjetoDAO.Instance.Insert(sessao, fig);
                }

                // Busca o material pela peça e clona ele também
                MaterialItemProjeto mip = lstMateriais.Find(f => f.IdPecaItemProj == idPecaItemProjOld);

                // O material pode ser nulo caso o usuário tenha inserido projeto de medidas exatas e tenha informado quantidade 0
                // ou uma quantidade menor que o padrão da peça
                if (mip != null)
                {
                    uint idMaterialOrig = mip.IdMaterItemProj;

                    mip.Beneficiamentos = mip.Beneficiamentos;
                    mip.IdMaterItemProj = 0;
                    mip.IdItemProjeto = idItemProjetoPedEsp;
                    mip.IdPecaItemProj = idPecaItemProj;
                    uint idMaterial = MaterialItemProjetoDAO.Instance.InsertBase(sessao, mip, pedido);

                    MaterialItemProjetoDAO.Instance.SetIdMaterItemProjOrig(sessao, idMaterialOrig, idMaterial);
                }
            }

            // Clona os materiais que não foram clonados acima (os que não possuem referência de peça)
            foreach (MaterialItemProjeto mip in lstMateriais.FindAll(f => f.IdPecaItemProj.GetValueOrDefault() == 0))
            {
                uint idMaterialOrig = mip.IdMaterItemProj;
                mip.Beneficiamentos = mip.Beneficiamentos;
                mip.IdMaterItemProj = 0;
                mip.IdItemProjeto = idItemProjetoPedEsp;

                uint idMaterial = MaterialItemProjetoDAO.Instance.InsertBase(sessao, mip, pedido);

                // Salva o id do material original no material clonado
                MaterialItemProjetoDAO.Instance.SetIdMaterItemProjOrig(sessao, idMaterialOrig, idMaterial);
            }

            #region Update Total Item Projeto

            ItemProjetoDAO.Instance.UpdateTotalItemProjeto(sessao, idItemProjetoPedEsp);

            PedidoEspelhoDAO.Instance.UpdateTotalPedido(sessao, idPedidoEsp);

            #endregion

            // Indica que o item do projeto foi conferido
            if (!PCPConfig.ExigirConferenciaPCP)
                ItemProjetoDAO.Instance.CalculoConferido(sessao, idItemProjetoPedEsp);

            return idItemProjetoPedEsp;
        }

        #endregion

        #region Remove e aplica comissão, desconto e acréscimo

        #region Remover

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        private void RemoveComissaoDescontoAcrescimo(GDASession sessao, PedidoEspelho pedidoEspelho,
            IEnumerable<ProdutosPedidoEspelho> produtosPedidoEspelho)
        {
            var ambientesPedido = (pedidoEspelho as IContainerCalculo).Ambientes.Obter()
                .Cast<AmbientePedidoEspelho>()
                .Where(f => f.Acrescimo > 0)
                .ToList();

            var removidos = new List<uint>();

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
            {
                var produtosAmbiente = ProdutosPedidoEspelhoDAO.Instance.GetByAmbiente(sessao, ambientePedido.IdAmbientePedido);
                if (AmbientePedidoEspelhoDAO.Instance.RemoverAcrescimo(sessao, pedidoEspelho, ambientePedido.IdAmbientePedido, produtosAmbiente))
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }

            if (RemoverComissao(sessao, pedidoEspelho, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            if (RemoverAcrescimo(sessao, pedidoEspelho, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            if (RemoverDesconto(sessao, pedidoEspelho, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            var produtosAtualizar = produtosPedidoEspelho
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(sessao, pedidoEspelho, produtosAtualizar, true);
            UpdateTotalPedido(sessao, pedidoEspelho);

            objPersistence.ExecuteCommand(sessao, @"update pedido_espelho set percComissao=0, desconto=0,
                acrescimo=0 where idPedido=" + pedidoEspelho.IdPedido);
        }
        
        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        internal void RemoveComissaoDescontoAcrescimo(GDASession session, PedidoEspelho antigo, PedidoEspelho novo)
        {
            var ambientesPedido = (novo as IContainerCalculo).Ambientes.Obter()
                .Cast<AmbientePedidoEspelho>()
                .Where(f => f.Acrescimo > 0)
                .ToList();

            var removidos = new List<uint>();

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
            {
                var produtosAmbiente = ProdutosPedidoEspelhoDAO.Instance.GetByAmbiente(session, ambientePedido.IdAmbientePedido);
                if (AmbientePedidoEspelhoDAO.Instance.RemoverAcrescimo(session, novo, ambientePedido.IdAmbientePedido, produtosAmbiente))
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }


            var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, novo.IdPedido, false, false, true);

            // Remove o valor da comissão nos produtos e no pedido
            if (RemoverComissao(session, novo, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            // Remove o acréscimo do pedido
            if (RemoverAcrescimo(session, novo, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            // Remove o desconto do pedido
            if (RemoverDesconto(session, novo, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            var produtosAtualizar = produtosPedidoEspelho
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(session, novo, produtosAtualizar, true);
            UpdateTotalPedido(session, novo);
        }

        #endregion

        #region Aplicar

        public void AplicaComissaoDescontoAcrescimo(GDASession session, PedidoEspelho pedidoEspelho)
        {
            var ambientesPedido = (pedidoEspelho as IContainerCalculo).Ambientes.Obter()
                .Cast<AmbientePedidoEspelho>()
                .Where(f => f.Acrescimo > 0)
                .ToList();

            var removidos = new List<uint>();

            var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, pedidoEspelho.IdPedido, false, false, true);

            if (AplicarAcrescimo(session, pedidoEspelho, pedidoEspelho.TipoAcrescimo, pedidoEspelho.Acrescimo, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            if (AplicarDesconto(session, pedidoEspelho, pedidoEspelho.TipoDesconto, pedidoEspelho.Desconto, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            if (AplicarComissao(session, pedidoEspelho, pedidoEspelho.PercComissao, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            objPersistence.ExecuteCommand(session, @"update pedido_espelho set percComissao=?pc, tipoDesconto=?td, desconto=?d,
                tipoAcrescimo=?ta, acrescimo=?a where idPedido=" + pedidoEspelho.IdPedido, new GDAParameter("?pc", pedidoEspelho.PercComissao),
                new GDAParameter("?td", pedidoEspelho.TipoDesconto), new GDAParameter("?d", pedidoEspelho.Desconto),
                new GDAParameter("?ta", pedidoEspelho.TipoAcrescimo), new GDAParameter("?a", pedidoEspelho.Acrescimo));

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
            {
                var produtosAmbiente = ProdutosPedidoEspelhoDAO.Instance.GetByAmbiente(session, ambientePedido.IdAmbientePedido);
                if (AmbientePedidoEspelhoDAO.Instance.AplicarAcrescimo(session, pedidoEspelho, ambientePedido.IdAmbientePedido, ambientePedido.TipoAcrescimo, ambientePedido.Acrescimo, produtosAmbiente))
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }

            var produtosAtualizar = produtosPedidoEspelho
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(session, pedidoEspelho, produtosAtualizar, true);
            UpdateTotalPedido(session, pedidoEspelho);
        }

        /// <summary>
        /// Aplica comissão, desconto e acréscimo em uma ordem pré-estabelecida.
        /// </summary>
        internal void AplicaComissaoDescontoAcrescimo(GDASession session, PedidoEspelho antigo, PedidoEspelho novo)
        {
            var ambientesPedido = (novo as IContainerCalculo).Ambientes.Obter()
                .Cast<AmbientePedidoEspelho>()
                .Where(f => f.Acrescimo > 0)
                .ToList();

            var removidos = new List<uint>();

            var alteraAcrescimo = antigo.Acrescimo != novo.Acrescimo || antigo.TipoAcrescimo != novo.TipoAcrescimo;
            var alteraDesconto = antigo.Desconto != novo.Desconto || antigo.TipoDesconto != novo.TipoDesconto;
            var alteraComissao = antigo.PercComissao != novo.PercComissao;

            var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, novo.IdPedido, false, false, true);

            // Remove o acréscimo do pedido
            if (alteraAcrescimo && AplicarAcrescimo(session, novo, novo.TipoAcrescimo, novo.Acrescimo, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            // Remove o desconto do pedido
            if (alteraDesconto && AplicarDesconto(session, novo, novo.TipoDesconto, novo.Desconto, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            // Remove o valor da comissão nos produtos e no pedido
            if (alteraComissao && AplicarComissao(session, novo, novo.PercComissao, produtosPedidoEspelho))
                removidos.AddRange(produtosPedidoEspelho.Select(p => p.IdProdPed));

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
            {
                var produtosAmbiente = ProdutosPedidoEspelhoDAO.Instance.GetByAmbiente(session, ambientePedido.IdAmbientePedido);
                if (AmbientePedidoEspelhoDAO.Instance.AplicarAcrescimo(session, novo, ambientePedido.IdAmbientePedido, ambientePedido.TipoAcrescimo, ambientePedido.Acrescimo, produtosAmbiente))
                    removidos.AddRange(produtosAmbiente.Select(p => p.IdProdPed));
            }

            var produtosAtualizar = produtosPedidoEspelho
                .Where(p => removidos.Contains(p.IdProdPed))
                .ToList();

            FinalizarAplicacaoComissaoAcrescimoDesconto(session, novo, produtosAtualizar, true);
            UpdateTotalPedido(session, novo);
        }

        #endregion

        #endregion

        #region Busca para tela de impressão de etiquetas

        private string SqlPedidoImpressaoEtiqueta(uint idCliente, uint idRota, string pedCliente,
            DateTime? dataEntregaIni, DateTime? dataEntregaFim, DateTime? dataFabricaIni, DateTime? dataFabricaFim,
            int tipoPedido, uint idLoja, uint idCorVidro, float espessura, uint idSubgrupoProd, float alturaMin,
            float alturaMax, int larguraMin, int larguraMax, bool selecionar, out string filtroAdicional, out bool temFiltro)
        {
            temFiltro = true;
            StringBuilder sql = new StringBuilder("select "),
                fa = new StringBuilder(), ppe = new StringBuilder(), ape = new StringBuilder();

            sql.AppendFormat(selecionar ? "pe.*, p.idProjeto, f.nome as nomeFunc, concat(p.idCli, ' - ', {0}) as nomeCliente" :
                "count(*)", ClienteDAO.Instance.GetNomeCliente("c"));

            #region Recupera os ids dos pedidos que serão utilizados como filtro dos produtos

            if (idCorVidro > 0)
            {
                ppe.AppendFormat(" and prod.idCorVidro={0}", idCorVidro);
                ape.AppendFormat(" and prod.idCorVidro={0}", idCorVidro);
            }

            if (espessura > 0)
            {
                ppe.AppendFormat(" and coalesce(ppe.espessura, prod.espessura)={0}", espessura.ToString().Replace(",", "."));
                ape.AppendFormat(" and prod.espessura={0}", espessura.ToString().Replace(",", "."));
            }

            if (idSubgrupoProd > 0)
            {
                ppe.AppendFormat(" and prod.idSubgrupoProd={0}", idSubgrupoProd);
                ape.AppendFormat(" and prod.idSubgrupoProd={0}", idSubgrupoProd);
            }

            if (alturaMin > 0)
            {
                ppe.AppendFormat(" and if(ppe.alturaReal>0, ppe.alturaReal, ppe.altura)>={0}", alturaMin.ToString().Replace(",", "."));
                ape.AppendFormat(" and ape.altura>={0}", alturaMin.ToString().Replace(",", "."));
            }

            if (alturaMax > 0)
            {
                ppe.AppendFormat(" and if(ppe.alturaReal>0, ppe.alturaReal, ppe.altura)<={0}", alturaMax.ToString().Replace(",", "."));
                ape.AppendFormat(" and ape.altura<={0}", alturaMax.ToString().Replace(",", "."));
            }

            if (larguraMin > 0)
            {
                ppe.AppendFormat(" and if(ppe.larguraReal>0, ppe.larguraReal, ppe.largura)>={0}", larguraMin);
                ape.AppendFormat(" and ape.largura>={0}", larguraMin);
            }

            if (larguraMax > 0)
            {
                ppe.AppendFormat(" and if(ppe.larguraReal>0, ppe.larguraReal, ppe.largura)<={0}", larguraMax);
                ape.AppendFormat(" and ape.largura<={0}", larguraMax);
            }

            var idsPedidos = GetValoresCampo(
                string.Format(@"
                select ppe.idPedido
                from produtos_pedido_espelho ppe
                    inner join produto prod on (ppe.idProd=prod.idProd)
                where prod.idGrupoProd={0} and ppe.qtdImpresso < ppe.qtde AND (ppe.InvisivelFluxo IS NULL OR ppe.InvisivelFluxo=0){1}", (int) Glass.Data.Model.NomeGrupoProd.Vidro, ppe), "idPedido");

            var idsPedidosMaoObra = GetValoresCampo(
                string.Format(@"
                select ape.idPedido
                from ambiente_pedido_espelho ape
                    inner join produto prod on (ape.idProd=prod.idProd)
                where prod.idGrupoProd={0} and ape.qtdeImpresso < ape.qtde{1}", (int)Glass.Data.Model.NomeGrupoProd.Vidro, ape), "idPedido");

            #endregion

            sql.AppendFormat(@"
                from pedido_espelho pe
                    inner join pedido p on (pe.idPedido=p.idPedido)
                    left join rota_cliente rc on (p.idCli=rc.idCliente)
                    left join funcionario f on (p.idFunc=f.idFunc)
                    left join cliente c on (p.idCli=c.id_Cli)
                where pe.situacao in ({0}) and p.situacao in ({1}) and 
                    if(p.tipoPedido<>{2}, pe.idPedido in ({3}), pe.idPedido in ({4})){5}",
                
                (int)PedidoEspelho.SituacaoPedido.Finalizado + "," + (int)PedidoEspelho.SituacaoPedido.Impresso + "," +
                    (int)PedidoEspelho.SituacaoPedido.ImpressoComum,
                (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + "," +
                    (int)Pedido.SituacaoPedido.Confirmado,
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                idsPedidos != String.Empty ? idsPedidos : "0",
                idsPedidosMaoObra != String.Empty ? idsPedidosMaoObra : "0",
                FILTRO_ADICIONAL);

            if (idCliente > 0)
            {
                sql.AppendFormat(" and p.idCli={0}", idCliente);
                temFiltro = true;
            }

            if (idRota > 0)
            {
                sql.AppendFormat(" and rc.idRota={0}", idRota);
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(pedCliente))
            {
                sql.Append(" and p.codCliente=?pedCli");
                temFiltro = true;
            }

            if (dataEntregaIni != null)
            {
                sql.AppendFormat(" and p.dataEntrega>=?dataEntregaIni");
                temFiltro = true;
            }

            if (dataEntregaFim != null)
            {
                sql.AppendFormat(" and p.dataEntrega<=?dataEntregaFim");
                temFiltro = true;
            }

            if (dataFabricaIni != null)
            {
                fa.AppendFormat(" and pe.dataFabrica>=?dataFabricaIni");
                temFiltro = true;
            }

            if (dataFabricaFim != null)
            {
                fa.AppendFormat(" and pe.dataFabrica<=?dataFabricaFim");
                temFiltro = true;
            }

            if (tipoPedido > 0)
            {
                sql.AppendFormat(" and p.tipoPedido={0}", tipoPedido);
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                sql.AppendFormat(" and p.idLoja={0}", idLoja);
                temFiltro = true;
            }

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        private GDAParameter[] GetParamsImpressaoEtiqueta(string pedCliente, DateTime? dataEntregaIni, 
            DateTime? dataEntregaFim, DateTime? dataFabricaIni, DateTime? dataFabricaFim)
        {
            var lista = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(pedCliente))
                lista.Add(new GDAParameter("?pedCli", pedCliente));

            if (dataEntregaIni != null)
                lista.Add(new GDAParameter("?dataEntregaIni", dataEntregaIni.Value.Date));

            if (dataEntregaFim != null)
                lista.Add(new GDAParameter("?dataEntregaFim", dataEntregaFim.Value.Date.AddDays(1).AddMilliseconds(-1)));

            if (dataFabricaIni != null)
                lista.Add(new GDAParameter("?dataFabricaIni", dataFabricaIni.Value.Date));

            if (dataFabricaFim != null)
                lista.Add(new GDAParameter("?dataFabricaFim", dataFabricaFim.Value.Date.AddDays(1).AddMilliseconds(-1)));

            return lista.ToArray();
        }

        public IList<PedidoEspelho> ObtemPedidosEspelhoImpressaoEtiqueta(uint idCliente, uint idRota, string pedCliente,
            DateTime? dataEntregaIni, DateTime? dataEntregaFim, DateTime? dataFabricaIni, DateTime? dataFabricaFim,
            int tipoPedido, uint idLoja, uint idCorVidro, float espessura, uint idSubgrupoProd, float alturaMin,
            float alturaMax, int larguraMin, int larguraMax, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional, sql = SqlPedidoImpressaoEtiqueta(idCliente, idRota, pedCliente, dataEntregaIni, dataEntregaFim,
                dataFabricaIni, dataFabricaFim, tipoPedido, idLoja, idCorVidro, espessura, idSubgrupoProd, alturaMin, alturaMax,
                larguraMin, larguraMax, true, out filtroAdicional, out temFiltro);

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "pe.idPedido desc";
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional, 
                GetParamsImpressaoEtiqueta(pedCliente, dataEntregaIni, dataEntregaFim, dataFabricaIni, dataFabricaFim));
        }

        public int ObtemNumeroPedidosEspelhoImpressaoEtiqueta(uint idCliente, uint idRota, string pedCliente,
            DateTime? dataEntregaIni, DateTime? dataEntregaFim, DateTime? dataFabricaIni, DateTime? dataFabricaFim,
            int tipoPedido, uint idLoja, uint idCorVidro, float espessura, uint idSubgrupoProd, float alturaMin,
            float alturaMax, int larguraMin, int larguraMax)
        {
            bool temFiltro;
            string filtroAdicional, sql = SqlPedidoImpressaoEtiqueta(idCliente, idRota, pedCliente, dataEntregaIni, dataEntregaFim,
                dataFabricaIni, dataFabricaFim, tipoPedido, idLoja, idCorVidro, espessura, idSubgrupoProd, alturaMin, alturaMax,
                larguraMin, larguraMax, true, out filtroAdicional, out temFiltro);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamsImpressaoEtiqueta(pedCliente, 
                dataEntregaIni, dataEntregaFim, dataFabricaIni, dataFabricaFim));
        }

        #endregion

        #region Valida o tipo do pedido com o tipo do produto (espessura dos produto se necessário)

        /// <summary>
        /// Verifica se os produtos são do mesmo tipo do pedido
        /// </summary>
        private void ValidaTipoPedidoTipoProduto(GDASession sessao, Pedido pedido, ProdutosPedidoEspelho[] produtosPedidoEspelho)
        {
            // Verifica se o pedido tem itens que não são permitidos pelo seu tipo
            if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && produtosPedidoEspelho != null)
            {
                var subGrupos = SubgrupoProdDAO.Instance.ObtemSubgrupos(sessao, produtosPedidoEspelho.Where(f => f.IdSubgrupoProd > 0).Select(f => (int)f.IdSubgrupoProd).ToList()).Distinct();

                foreach (var produtoPedidoEspelho in produtosPedidoEspelho.Where(f => f.IdProdPedParent.GetValueOrDefault() == 0))
                {
                    //Verifica se o produto é uma embalagem (Item de revenda que pode ser incluído em pedido de venda)
                    if (produtoPedidoEspelho.IdSubgrupoProd == 0 || subGrupos.Any(f => f.IdSubgrupoProd == produtoPedidoEspelho.IdSubgrupoProd && !f.PermitirItemRevendaNaVenda))
                    {
                        if ((Pedido.TipoPedidoEnum)pedido.TipoPedido == Pedido.TipoPedidoEnum.Venda &&
                        (produtoPedidoEspelho.IdGrupoProd != (uint)NomeGrupoProd.Vidro ||
                        (produtoPedidoEspelho.IdGrupoProd == (uint)NomeGrupoProd.Vidro &&
                        SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)produtoPedidoEspelho.IdGrupoProd, (int)produtoPedidoEspelho.IdSubgrupoProd))) &&
                        produtoPedidoEspelho.IdGrupoProd != (uint)NomeGrupoProd.MaoDeObra && produtoPedidoEspelho.IdProdPedParent.GetValueOrDefault(0) == 0)
                            throw new Exception("Não é possível incluir produtos de revenda em um pedido de venda.");

                        if ((Pedido.TipoPedidoEnum)pedido.TipoPedido == Pedido.TipoPedidoEnum.Revenda &&
                            ((produtoPedidoEspelho.IdGrupoProd == (uint)NomeGrupoProd.Vidro &&
                            !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)produtoPedidoEspelho.IdGrupoProd, (int)produtoPedidoEspelho.IdSubgrupoProd)) ||
                            produtoPedidoEspelho.IdGrupoProd == (uint)NomeGrupoProd.MaoDeObra))
                            throw new Exception("Não é possível incluir produtos de venda em um pedido de revenda.");

                        // Impede que o pedido seja finalizado caso tenham sido inseridos produtos de cor e espessura diferentes.
                        if ((PedidoConfig.DadosPedido.BloquearItensCorEspessura && !LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(null, pedido.IdLoja)) &&
                            ((Pedido.TipoPedidoEnum)pedido.TipoPedido == Pedido.TipoPedidoEnum.Venda ||
                            (Pedido.TipoPedidoEnum)pedido.TipoPedido == Pedido.TipoPedidoEnum.MaoDeObraEspecial))
                            if ((ProdutoDAO.Instance.ObtemIdCorVidro(sessao, (int)produtosPedidoEspelho.FirstOrDefault(f => f.IdProdPedParent.GetValueOrDefault(0) == 0)?.IdProd) != ProdutoDAO.Instance.ObtemIdCorVidro(sessao, (int)produtoPedidoEspelho.IdProd) ||
                                ProdutoDAO.Instance.ObtemEspessura(sessao, (int)produtosPedidoEspelho.FirstOrDefault(f => f.IdProdPedParent.GetValueOrDefault(0) == 0)?.IdProd) != ProdutoDAO.Instance.ObtemEspessura(sessao, (int)produtoPedidoEspelho.IdProd)) &&
                                produtoPedidoEspelho.IdProdPedParent.GetValueOrDefault() == 0)
                                throw new Exception("Todos produtos devem ter a mesma cor e espessura.");
                    }
                }
            }
        }

        #endregion

        #region Verifica se deve gerar projeto para CNC

        /// <summary>
        /// Atualiza a situação do projeto cnc do pedido espelho
        /// </summary>
        /// <param name="idPedido"></param>
        public void AtualizaSituacaoCnc(uint idPedido)
        {
            AtualizaSituacaoCnc(null, idPedido);
        }

        /// <summary>
        /// Atualiza a situação do projeto cnc do pedido espelho
        /// </summary>
        public void AtualizaSituacaoCnc(GDASession sessao, uint idPedido)
        {
            try
            {
                var projetar = false;

                //Percorre os produtos do pedido espelho
                foreach (var prod in ProdutosPedidoEspelhoDAO.Instance.GetByPedido(sessao, idPedido, false, false))
                {
                    if (ProdutosPedidoEspelhoDAO.Instance.DeveGerarProjCNC(sessao, prod))
                    {
                        projetar = true;
                        break;
                    }
                }

                //Atualiza a situação cnc
                objPersistence.ExecuteCommand(sessao, "UPDATE pedido_espelho SET situacaoCnc=" +
                    (projetar ? (int)PedidoEspelho.SituacaoCncEnum.NaoProjetado : (int)PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido) +
                    " WHERE idPedido=" + idPedido);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("AtualizaSituacaoCnc - idPedido: " + idPedido, ex);
                throw;
            }
        }

        #endregion

        #region Gerar arquivo FML

        public PacoteArquivosMesa GerarArquivoFmlPeloPedido(ProdutosPedidoEspelho[] lstProdPedEsp, bool salvarArquivoSeparado)
        {
            return GerarArquivoFmlPeloPedido(null, lstProdPedEsp, salvarArquivoSeparado);
        }

        public PacoteArquivosMesa GerarArquivoFmlPeloPedido(GDASession session, ProdutosPedidoEspelho[] lstProdPedEsp, bool salvarArquivoSeparado)
        {
            var lstArqMesa = new List<byte[]>();
            var lstCodArq = new List<string>();
            var lstErrosArq = new List<KeyValuePair<string, Exception>>();

            var lstEtiquetas = "";
            var lstEtiqueta = new List<Glass.Data.RelModel.Etiqueta>();

            // Percorre os produtos
            foreach (var prodPedEsp in lstProdPedEsp)
            {
                var gerarFml = false;

                if (prodPedEsp.IdMaterItemProj.GetValueOrDefault() == 0 || MaterialItemProjetoDAO.Instance.ObtemidPecaItemProj(session, prodPedEsp.IdMaterItemProj.Value) == 0)
                {
                    if (prodPedEsp.IdProcesso.GetValueOrDefault() == 0)
                        continue;

                    var tipoProcesso = EtiquetaProcessoDAO.Instance.ObtemTipoProcesso(session, prodPedEsp.IdProcesso.Value);

                    // Verifica se a peça avulsa é de instalação. Se não for instalação passa para a próxima.
                    if (tipoProcesso != (int)EtiquetaTipoProcesso.Caixilho && tipoProcesso != (int)EtiquetaTipoProcesso.Instalacao)
                        continue;

                    //Se o produto avulso for de instalação verifica se o mesmo tem arquivo FML.
                    var idArquivoMesaCorte = ProdutoDAO.Instance.ObtemIdArquivoMesaCorte(session, prodPedEsp.IdProd);

                    if (idArquivoMesaCorte.GetValueOrDefault() == 0)
                        continue;

                    var tipoArquivo = ProdutoDAO.Instance.ObterTipoArquivoMesaCorte(session, (int)prodPedEsp.IdProd);

                    gerarFml = !salvarArquivoSeparado || PCPConfig.SalvarArquivoBasicoAoFinalizarPCP ?
                        tipoArquivo == TipoArquivoMesaCorte.FMLBasico || tipoArquivo == TipoArquivoMesaCorte.FML :
                        tipoArquivo == TipoArquivoMesaCorte.FML;
                }
                else // Se o item for de projeto
                {
                    var idPecaItemProj = MaterialItemProjetoDAO.Instance.ObtemidPecaItemProj(session, prodPedEsp.IdMaterItemProj.Value);

                    var pecaItemProjeto = PecaItemProjetoDAO.Instance.GetElement(session, idPecaItemProj);

                    // Se a peça não for instalação passa para a próxima.
                    if (pecaItemProjeto == null)
                        continue;

                    // Se a peça não tiver arquivo FML
                    var idArquivoMesaCorte = pecaItemProjeto.IdArquivoMesaCorte.GetValueOrDefault();

                    if (idArquivoMesaCorte == 0)
                        continue;

                    var tipoArquivoMesaCorte = PecaProjetoModeloDAO.Instance.ObtemValorCampo<TipoArquivoMesaCorte?>(session, "TipoArquivo", "IdPecaProjMod=" + pecaItemProjeto.IdPecaProjMod);

                    if (tipoArquivoMesaCorte == null)
                        continue;

                    gerarFml = !salvarArquivoSeparado || PCPConfig.SalvarArquivoBasicoAoFinalizarPCP ?
                        tipoArquivoMesaCorte == TipoArquivoMesaCorte.FMLBasico || tipoArquivoMesaCorte == TipoArquivoMesaCorte.FML :
                        tipoArquivoMesaCorte == TipoArquivoMesaCorte.FML;
                }

                if (gerarFml)
                {
                    // Busca as etiquetas da peça
                    var etiquetas = PecaItemProjetoDAO.Instance.ObtemEtiquetas(session, prodPedEsp.IdPedido, prodPedEsp.IdProdPed, prodPedEsp.Qtde.ToString().StrParaInt());

                    // Percorre as etiquetas da peça
                    foreach (var etiqueta in etiquetas.Split(','))
                    {
                        var etiq = new Glass.Data.RelModel.Etiqueta();
                        etiq.NumEtiqueta = etiqueta;
                        etiq.IdPedido = prodPedEsp.IdPedido.ToString();
                        etiq.IdProdPedEsp = prodPedEsp.IdProdPed;
                        etiq.Espessura = prodPedEsp.Espessura;
                        etiq.Altura = prodPedEsp.AlturaProducao.ToString();
                        etiq.Largura = prodPedEsp.LarguraProducao.ToString();

                        lstEtiqueta.Add(etiq);
                        lstEtiquetas += etiq.NumEtiqueta + "|";
                    }
                }

                gerarFml = false;
            }

            var resultado = new PacoteArquivosMesa(lstArqMesa, lstCodArq, lstErrosArq);

            if (!String.IsNullOrEmpty(lstEtiquetas))
            {
                ImpressaoEtiquetaDAO.Instance.MontaArquivoMesaOptyway(session, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, 0, false, false);

                if (salvarArquivoSeparado)
                {
                    var caminhoSalvarFml = PCPConfig.CaminhoSalvarFml;

                    using (Glass.Seguranca.AutenticacaoRemota.Autenticar())
                        try
                        {
                            if (System.IO.Directory.Exists(caminhoSalvarFml))
                                resultado.SalvarArquivos(caminhoSalvarFml);
                        }
                        catch { /* Ignora */ }
                }
            }

            return resultado;
        }

        #endregion

        #region Gerar arquivo DXF

        public void GerarArquivoDxfPeloPedido(GDASession session, ProdutosPedidoEspelho[] lstProdPedEsp)
        {
            var lstArqMesa = new List<byte[]>();
            var lstCodArq = new List<string>();
            var lstErrosArq = new List<KeyValuePair<string, Exception>>();

            var lstEtiqueta = EtiquetaDAO.Instance.EtiquetasGerarDxf(session, lstProdPedEsp);

            if (lstEtiqueta.Count > 0)
            {
                ImpressaoEtiquetaDAO.Instance.MontaArquivoMesaOptyway(session, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, 0, false, false);

                var caminhoSalvarDxf = PCPConfig.CaminhoSalvarDxf;

                if (Directory.Exists(caminhoSalvarDxf))
                {
                    for (var i = 0; i < lstArqMesa.Count; i++)
                    {
                        var nomeArquivoDxf = caminhoSalvarDxf + lstCodArq[i];

                        if (!File.Exists(nomeArquivoDxf))
                        {
                            using (var fs = File.Create(nomeArquivoDxf))
                            {
                                fs.Write(lstArqMesa[i], 0, lstArqMesa[i].Length);
                            }

                            /* Chamado 16982. */
                            if (!File.Exists(nomeArquivoDxf))
                                throw new Exception("Algumas marcações não foram salvas no servidor. Verifique se a pasta, " +
                                    "onde as marcações são salvas, está disponível no servidor. Caso esteja, finalize o pedido novamente. " +
                                    "Caminho onde as marcações são salvas no servidor: " + caminhoSalvarDxf);
                        }
                    }
                }
            }
        }

        #endregion

        #region Gerar arquivo SGLASS

        public void GerarArquivoSglassPeloPedido(GDASession session, ProdutosPedidoEspelho[] lstProdPedEsp)
        {
            var lstArqMesa = new List<byte[]>();
            var lstCodArq = new List<string>();
            var lstErrosArq = new List<KeyValuePair<string, Exception>>();

            var lstEtiqueta = EtiquetaDAO.Instance.EtiquetasGerarDxf(session, lstProdPedEsp);

            if (lstEtiqueta.Count > 0)
            {
                ImpressaoEtiquetaDAO.Instance.MontaArquivoMesaOptyway(session, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, 0, false, 0, true, false, false);

                var tempPath = Path.GetTempPath();
                var programsDirectory = PCPConfig.CaminhoSalvarProgramSGlass;
                var hardwaresDirectory = PCPConfig.CaminhoSalvarSGlassHardware;

                for (int i = 0; i < lstArqMesa.Count; i++)
                {
                    SalvarArquivoSglass(lstCodArq[i], lstArqMesa[i], tempPath, programsDirectory, hardwaresDirectory);
                }
            }
        }

        /// <summary>
        /// Recebe o arquivo de mesa referênte ao SGlass, o nome do arquivo e o caminho das pastas e salva o arquivo.
        /// </summary>
        /// <param name="nomeArquivo"></param>
        /// <param name="arqMesa"></param>
        /// <param name="tempPath"></param>
        /// <param name="programsDirectory"></param>
        /// <param name="hardwaresDirectory"></param>
        public void SalvarArquivoSglass(string nomeArquivo, byte[] arqMesa, string tempPath, string programsDirectory, string hardwaresDirectory)
        {
            var fileName = Path.GetFileNameWithoutExtension(nomeArquivo);
            var zipPath = Path.Combine(tempPath, fileName);

            try
            {
                using (var zip = ZipFile.Read(arqMesa))
                {
                    zip.ExtractAll(zipPath, true);
                }
            }
            catch
            {
                return;
            }

            var programsPath = Path.Combine(zipPath, "Programs");
            var hardwaresPath = Path.Combine(zipPath, "hardwares");
            var files = Directory.GetFiles(programsPath);
            File.Move(files[0], Path.Combine(programsPath, (fileName + Path.GetExtension(files[0]))));

            if (Directory.Exists(programsPath))
                foreach (var file in Directory.GetFiles(programsPath))
                {
                    var path = Path.Combine(programsDirectory, Path.GetFileName(file));
                    if (File.Exists(path))
                        File.Delete(path);

                    File.Move(file, path);
                }

            if (Directory.Exists(hardwaresPath))
                foreach (var file in Directory.GetFiles(hardwaresPath))
                {
                    var path = Path.Combine(hardwaresDirectory, Path.GetFileName(file));
                    if (File.Exists(path))
                        File.Delete(path);

                    File.Move(file, Path.Combine(hardwaresDirectory, Path.GetFileName(file)));
                }

            Directory.Delete(zipPath, true);
        }

        #endregion

        #region Gerar arquivo Intermac

        public void GerarArquivoIntermacPeloPedido(GDASession session, ProdutosPedidoEspelho[] lstProdPedEsp)
        {
            var lstArqMesa = new List<byte[]>();
            var lstCodArq = new List<string>();
            var lstErrosArq = new List<KeyValuePair<string, Exception>>();

            var lstEtiqueta = EtiquetaDAO.Instance.EtiquetasGerarDxf(session, lstProdPedEsp);

            if (lstEtiqueta.Count > 0)
            {
                ImpressaoEtiquetaDAO.Instance.MontaArquivoMesaOptyway(session, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, 0, false, 0, false, true, false);

                var caminhoSalvarIntermac = PCPConfig.CaminhoSalvarIntermac;

                if (Directory.Exists(caminhoSalvarIntermac))
                {
                    for (var i = 0; i < lstArqMesa.Count; i++)
                    {
                        var nomeArquivoDxf = caminhoSalvarIntermac + lstCodArq[i];

                        if (!File.Exists(nomeArquivoDxf))
                        {
                            using (var fs = File.Create(nomeArquivoDxf))
                            {
                                fs.Write(lstArqMesa[i], 0, lstArqMesa[i].Length);
                            }

                            /* Chamado 16982. */
                            if (!File.Exists(nomeArquivoDxf))
                                throw new Exception("Algumas marcações não foram salvas no servidor. Verifique se a pasta, " +
                                    "onde as marcações são salvas, está disponível no servidor. Caso esteja, finalize o pedido novamente. " +
                                    "Caminho onde as marcações são salvas no servidor: " + caminhoSalvarIntermac);
                        }
                    }
                }
            }
        }

        #endregion

        #region Produtos de Composição

        /// <summary>
        /// Verifica se o pedido possui produto de composição
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiProdutosComposicao(GDASession sessao, int idPedido)
        {
            var tipoComposicao = (int)TipoSubgrupoProd.VidroDuplo + ", " + (int)TipoSubgrupoProd.VidroLaminado;
            var sql = @"
                SELECT COUNT(*) 
                FROM produtos_pedido pp
	                INNER JOIN produto p ON (pp.IdProd = p.IdProd)
                    INNER JOIN grupo_prod gp ON (p.IdGrupoProd = gp.IdGrupoProd)
                    INNER JOIN subgrupo_prod sgp ON (p.IdSubgrupoProd = sgp.IdSubgrupoProd)
                WHERE sgp.TipoSubgrupo IN (" + tipoComposicao + ") AND pp.IdPedido = " + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Atualização do valor de tabela dos produtos do pedido espelho

        /// <summary>
        /// Atualiza o valor de tabela dos produtos do pedido espelho.
        /// </summary>
        internal void AtualizarValorTabelaProdutosPedidoEspelho(GDASession session, Pedido antigo, Pedido novo)
        {
            #region Declaração de variáveis

            var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, novo.IdPedido, false);
            var itensProjeto = ItemProjetoDAO.Instance.GetByPedidoEspelho(session, novo.IdPedido);
            
            #endregion

            #region Remoção do acréscimo, comissão e desconto

            var pedidoEspelho = GetElement(session, novo.IdPedido);
            AtualizaPedidoEspelhoComDadosPedido(pedidoEspelho, novo);

            RemoveComissaoDescontoAcrescimo(session, pedidoEspelho, produtosPedidoEspelho);

            #endregion

            #region Atualização dos itens de projeto

            foreach (var itemProjeto in itensProjeto)
            {
                var idAmbientePedidoEspelho = AmbientePedidoEspelhoDAO.Instance.GetIdByItemProjeto(session, itemProjeto.IdItemProjeto);
                var itemProjetoConferido = itemProjeto.Conferido;

                ProdutosPedidoEspelhoDAO.Instance.InsereAtualizaProdProj(session, pedidoEspelho, idAmbientePedidoEspelho, itemProjeto, false);

                // Este método é chamado através da atualização do pedido pela notinha verde. Dentro do método InsereAtualizaProdProj, o item de projeto é marcado como não conferido,
                // porém ele deve-se manter como conferido, pois não foi feita alteração no projeto, diretamente.
                if (itemProjetoConferido)
                {
                    ItemProjetoDAO.Instance.CalculoConferido(session, itemProjeto.IdItemProjeto);
                }
            }

            #endregion

            #region Atualização dos totais dos produtos do pedido espelho

            // Percorre cada produto, do pedido espelho, e recalcula seu valor unitário, com base no valor de tabela e no desconto/acréscimo do cliente.
            foreach (var produtoPedidoEspelho in produtosPedidoEspelho)
            {
                if (ProdutoDAO.Instance.VerificarAtualizarValorTabelaProduto(session, antigo, novo, produtoPedidoEspelho))
                {
                    ProdutosPedidoEspelhoDAO.Instance.RecalcularValores(session, produtoPedidoEspelho, pedidoEspelho, false);
                    ProdutosPedidoEspelhoDAO.Instance.UpdateBase(session, produtoPedidoEspelho, novo);
                }
            }

            #endregion

            #region Atualização dos totais do pedido espelho

            AplicaComissaoDescontoAcrescimo(session, pedidoEspelho);
            UpdateTotalPedido(session, pedidoEspelho);

            #endregion
        }

        private void AtualizaPedidoEspelhoComDadosPedido(PedidoEspelho pedidoEspelho, Pedido pedido)
        {
            pedidoEspelho.TipoVenda = pedido.TipoVenda ?? 0;
            pedidoEspelho.IdFormaPagto = pedido.IdFormaPagto ?? 0;
            pedidoEspelho.ValorEntrada = pedido.ValorEntrada;
            pedidoEspelho.IdComissionado = pedido.IdComissionado;
            pedidoEspelho.PercComissao = pedido.PercComissao;
            pedidoEspelho.TipoDesconto = pedido.TipoDesconto;
            pedidoEspelho.Desconto = pedido.Desconto;
            pedidoEspelho.TipoAcrescimo = pedido.TipoAcrescimo;
            pedidoEspelho.Acrescimo = pedido.Acrescimo;
            pedidoEspelho.Obs = pedido.Obs;
            pedidoEspelho.DataEntrega = pedido.DataEntrega;
            pedidoEspelho.TipoEntrega = pedido.TipoEntrega ?? 0;
            pedidoEspelho.FastDelivery = pedido.FastDelivery;
            pedidoEspelho.IdLoja = pedido.IdLoja;
        }

        #endregion

        #region Rentabilidade

        /// <summary>
        /// Atualiza a rentabilidade do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="percentualRentabilidade">Percentual da rentabilidade.</param>
        /// <param name="rentabilidadeFinanceira">Rentabilidade financeira.</param>
        public void AtualizarRentabilidade(GDA.GDASession sessao,
            uint idPedido, decimal percentualRentabilidade, decimal rentabilidadeFinanceira)
        {
            objPersistence.ExecuteCommand(sessao, "UPDATE pedido_espelho SET PercentualRentabilidade=?percentual, RentabilidadeFinanceira=?rentabilidade WHERE IdPedido=?idPedido",
                new GDA.GDAParameter("?percentual", percentualRentabilidade),
                new GDA.GDAParameter("?rentabilidade", rentabilidadeFinanceira),
                new GDA.GDAParameter("?idPedido", idPedido));
        }

        #endregion

        #region Impostos

        /// <summary>
        /// Atualiza os valores de impostos associados com a instancia informada.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="produtoPedido"></param>
        public void AtualizarImpostos(GDASession sessao, PedidoEspelho pedido)
        {
            // Relação das propriedades que devem ser atualizadas
            var propriedades = new[]
            {
                nameof(ProdutosPedidoEspelho.ValorIpi),
                nameof(ProdutosPedidoEspelho.ValorIcms)
            };

            objPersistence.Update(sessao, pedido, string.Join(",", propriedades), DirectionPropertiesName.Inclusion);
        }

        #endregion
    }
}