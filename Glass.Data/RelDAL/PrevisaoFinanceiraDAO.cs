using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class PrevisaoFinanceiraDAO : BaseDAO<PrevisaoFinanceira, PrevisaoFinanceiraDAO>
    {
        //private PrevisaoFinanceiraDAO() { }

        private string SqlReceber(uint idLoja, string data, bool previsaoPedidos)
        {
            string sqlBase = @"
                select sum(valor) 
                from (
                    select sum(valorVec) as Valor
                    from contas_receber 
                    where recebida=false
                        and datediff(dataVec, ?data){0}
                        and (isParcelaCartao=false or isParcelaCartao is null)
                        and idAntecipContaRec Is Null
                        " + (idLoja > 0 ? "and idLoja=" + idLoja : "") + @"
                ) as receber";

            string sqlBaseCheques = @"
                select sum(valor)
                from (
                    select sum(valor) as Valor
                    from cheques
                    where tipo=2
                        and situacao in (" + (int)Cheques.SituacaoCheque.EmAberto + "," + (int)Cheques.SituacaoCheque.Devolvido + @")
                        and datediff(dataVenc, ?data){0}
                        " + (idLoja > 0 ? "and idLoja=" + idLoja : "") + @"
                ) as cheques";

            string situacoesPedido = (int)Pedido.SituacaoPedido.Conferido + "," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao;
            string tiposVenda = (int)Pedido.TipoVendaPedido.APrazo + "," + (int)Pedido.TipoVendaPedido.AVista + "," + (int)Pedido.TipoVendaPedido.Funcionario;
            string tiposPedido = (int)Pedido.TipoPedidoEnum.MaoDeObra + "," + (int)Pedido.TipoPedidoEnum.Revenda + "," + (int)Pedido.TipoPedidoEnum.Venda;

            string sqlBasePedidos = @"
                SELECT SUM(Valor)
                FROM (
                    SELECT (COALESCE(pe.total, p.total) - IF(p.idSinal > 0 And p.valorEntrada > 0, COALESCE(p.valorEntrada, 0), 0) -
                        IF(p.idPagamentoAntecipado > 0 And p.valorPagamentoAntecipado > 0, COALESCE(p.valorPagamentoAntecipado, 0), 0)) AS valor
                    FROM pedido p
                        LEFT JOIN pedido_espelho pe ON (pe.idPedido = p.idPedido)
                    WHERE 1 
                        " + (idLoja > 0 ? "AND p.idLoja = " + idLoja : "") + @"
                        AND p.situacao IN (" + situacoesPedido + @")
                        AND p.tipoVenda IN (" + tiposVenda + @")
                        AND p.tipoPedido IN (" + tiposPedido + @")
                        AND DATEDIFF(p.dataEntrega,?data){0}
                        group by p.IdPedido having valor > 0
                ) as valorPedidos";

            string vencidasMais90Dias = String.Format(sqlBase, "<-90");
            string vencidas90Dias = String.Format(sqlBase, "<-60 and datediff(dataVec, ?data)>=-90");
            string vencidas60Dias = String.Format(sqlBase, "<-30 and datediff(dataVec, ?data)>=-60");
            string vencidas30Dias = String.Format(sqlBase, "<0 and datediff(dataVec, ?data)>=-30");
            string vencimentoHoje = String.Format(sqlBase, "=0");
            string vencer30Dias = String.Format(sqlBase, ">0 and datediff(dataVec, ?data)<=30");
            string vencer60Dias = String.Format(sqlBase, ">30 and datediff(dataVec, ?data)<=60");
            string vencer90Dias = String.Format(sqlBase, ">60 and datediff(dataVec, ?data)<=90");
            string vencerMais90Dias = String.Format(sqlBase, ">90");

            string chequesVencidosMais90Dias = String.Format(sqlBaseCheques, "<-90");
            string chequesVencidos90Dias = String.Format(sqlBaseCheques, "<-60 and datediff(dataVenc, ?data)>=-90");
            string chequesVencidos60Dias = String.Format(sqlBaseCheques, "<-30 and datediff(dataVenc, ?data)>=-60");
            string chequesVencidos30Dias = String.Format(sqlBaseCheques, "<0 and datediff(dataVenc, ?data)>=-30");
            string chequesVencimentoHoje = String.Format(sqlBaseCheques, "=0");
            string chequesVencer30Dias = String.Format(sqlBaseCheques, ">0 and datediff(dataVenc, ?data)<=30");
            string chequesVencer60Dias = String.Format(sqlBaseCheques, ">30 and datediff(dataVenc, ?data)<=60");
            string chequesVencer90Dias = String.Format(sqlBaseCheques, ">60 and datediff(dataVenc, ?data)<=90");
            string chequesVencerMais90Dias = String.Format(sqlBaseCheques, ">90");

            string pedidosVencidosMais90Dias = String.Format(sqlBasePedidos, "<-90");
            string pedidosVencidos90Dias = String.Format(sqlBasePedidos, "<-60 and DATEDIFF(p.dataEntrega,?data)>=-90");
            string pedidosVencidos60Dias = String.Format(sqlBasePedidos, "<-30 and DATEDIFF(p.dataEntrega,?data)>=-60");
            string pedidosVencidos30Dias = String.Format(sqlBasePedidos, "<0 and DATEDIFF(p.dataEntrega,?data)>=-30");
            string pedidosVencimentoHoje = String.Format(sqlBasePedidos, "=0");
            string pedidosVencer30Dias = String.Format(sqlBasePedidos, ">0 and DATEDIFF(p.dataEntrega,?data)<=30");
            string pedidosVencer60Dias = String.Format(sqlBasePedidos, ">30 and DATEDIFF(p.dataEntrega,?data)<=60");
            string pedidosVencer90Dias = String.Format(sqlBasePedidos, ">60 and DATEDIFF(p.dataEntrega,?data)<=90");
            string pedidosVencerMais90Dias = String.Format(sqlBasePedidos, ">90");

            string sql = "select cast((" + vencidasMais90Dias + ") as decimal(12,2)) as VencidasMais90Dias, " +
                "cast((" + vencidas90Dias + ") as decimal(12,2)) as Vencidas90Dias, " +
                "cast((" + vencidas60Dias + ") as decimal(12,2)) as Vencidas60Dias, " +
                "cast((" + vencidas30Dias + ") as decimal(12,2)) as Vencidas30Dias, " +
                "cast((" + vencimentoHoje + ") as decimal(12,2)) as VencimentoHoje, " +
                "cast((" + vencer30Dias + ") as decimal(12,2)) as Vencer30Dias, " +
                "cast((" + vencer60Dias + ") as decimal(12,2)) as Vencer60Dias, " +
                "cast((" + vencer90Dias + ") as decimal(12,2)) as Vencer90Dias, " +
                "cast((" + vencerMais90Dias + ") as decimal(12,2)) as VencerMais90Dias, " +
                "cast((" + chequesVencidosMais90Dias + ") as decimal(12,2)) as ChequesVencidosMais90Dias, " +
                "cast((" + chequesVencidos90Dias + ") as decimal(12,2)) as ChequesVencidos90Dias, " +
                "cast((" + chequesVencidos60Dias + ") as decimal(12,2)) as ChequesVencidos60Dias, " +
                "cast((" + chequesVencidos30Dias + ") as decimal(12,2)) as ChequesVencidos30Dias, " +
                "cast((" + chequesVencimentoHoje + ") as decimal(12,2)) as ChequesVencimentoHoje, " +
                "cast((" + chequesVencer30Dias + ") as decimal(12,2)) as ChequesVencer30Dias, " +
                "cast((" + chequesVencer60Dias + ") as decimal(12,2)) as ChequesVencer60Dias, " +
                "cast((" + chequesVencer90Dias + ") as decimal(12,2)) as ChequesVencer90Dias, " +
                "cast((" + chequesVencerMais90Dias + ") as decimal(12,2)) as ChequesVencerMais90Dias";

            if (previsaoPedidos)
            {
                sql += ", cast((" + pedidosVencidosMais90Dias + ") as decimal(12,2)) as PedidosVencidosMais90Dias, " +
                    "cast((" + pedidosVencidos90Dias + ") as decimal(12,2)) as PedidosVencidos90Dias, " +
                    "cast((" + pedidosVencidos60Dias + ") as decimal(12,2)) as PedidosVencidos60Dias, " +
                    "cast((" + pedidosVencidos30Dias + ") as decimal(12,2)) as PedidosVencidos30Dias, " +
                    "cast((" + pedidosVencimentoHoje + ") as decimal(12,2)) as PedidosVencimentoHoje, " +
                    "cast((" + pedidosVencer30Dias + ") as decimal(12,2)) as PedidosVencer30Dias, " +
                    "cast((" + pedidosVencer60Dias + ") as decimal(12,2)) as PedidosVencer60Dias, " +
                    "cast((" + pedidosVencer90Dias + ") as decimal(12,2)) as PedidosVencer90Dias, " +
                    "cast((" + pedidosVencerMais90Dias + ") as decimal(12,2)) as PedidosVencerMais90Dias";
            }

            return sql;
        }

        private string SqlPagar(uint idLoja, string data, bool previsaoCustoFixo)
        {
            string sqlBase = @"
                select sum(valor) 
                from (
                    select sum(valorVenc) as valor
                    from contas_pagar 
                    where paga=false 
                        and datediff(dataVenc, ?data){0}
                        " + (idLoja > 0 ? "and idLoja=" + idLoja : "") + @"
                ) as pagar";

            string sqlBaseCheques = @"
                select sum(valor)
                from (
                    select sum(valor) as Valor
                    from cheques
                    where tipo=1
                        and situacao in (" + (int)Cheques.SituacaoCheque.EmAberto + "," + (int)Cheques.SituacaoCheque.Devolvido + @")
                        and datediff(dataVenc, ?data){0}
                        " + (idLoja > 0 ? "and idLoja=" + idLoja : "") + @"
                ) as cheques";

            string sqlLoja = "";

            if (idLoja > 0)
                sqlLoja = " and idloja = " + idLoja;

            string sqlPrevisaoCustoFixo = @"
                SELECT sum(valorvenc) AS custoFixo
                FROM
                    (SELECT c.valorvenc, c.situacao, c.idcustofixo, c.diavenc, c.idLoja
                     FROM custo_fixo c where 1 " + sqlLoja + @") 
                AS dados,
                    (SELECT DISTINCT cast(date_sub(dataVenc, interval day(dataVenc)-1 DAY) AS date) AS DATA
                     FROM contas_pagar c
                     WHERE (paga=FALSE OR paga IS NULL) 
                        " + sqlLoja + @"
                        AND DATEDIFF(DataVenc, ?data){0}) 
                AS DATA
                WHERE situacao=" + (int)CustoFixo.SituacaoEnum.Ativo + sqlLoja +
                      @" AND cast(concat(idCustoFixo, ',', month(DATA), ',', year(DATA)) AS char) NOT IN
                            (SELECT *
                             FROM (SELECT DISTINCT cast(concat(idCustoFixo, ',', month(dataVenc), ',', year(dataVenc)) AS char)
                                    FROM contas_pagar c
                                    WHERE idCustoFixo IS NOT NULL AND DATEDIFF(DataVenc, ?data){0} " + sqlLoja + @") 
                            AS tbl )
                         AND DATEDIFF(((DATA.DATA - interval DAY(DATA.DATA) DAY)+interval diaVenc DAY),?data){1}";

            string vencidasMais90Dias = String.Format(sqlBase, "<-90");
            string vencidas90Dias = String.Format(sqlBase, "<-60 and datediff(dataVenc, ?data)>=-90");
            string vencidas60Dias = String.Format(sqlBase, "<-30 and datediff(dataVenc, ?data)>=-60");
            string vencidas30Dias = String.Format(sqlBase, "<0 and datediff(dataVenc, ?data)>=-30");
            string vencimentoHoje = String.Format(sqlBase, "=0");
            string vencer30Dias = String.Format(sqlBase, ">0 and datediff(dataVenc, ?data)<=30");
            string vencer60Dias = String.Format(sqlBase, ">30 and datediff(dataVenc, ?data)<=60");
            string vencer90Dias = String.Format(sqlBase, ">60 and datediff(dataVenc, ?data)<=90");
            string vencerMais90Dias = String.Format(sqlBase, ">90");

            string chequesVencidosMais90Dias = String.Format(sqlBaseCheques, "<-90");
            string chequesVencidos90Dias = String.Format(sqlBaseCheques, "<-60 and datediff(dataVenc, ?data)>=-90");
            string chequesVencidos60Dias = String.Format(sqlBaseCheques, "<-30 and datediff(dataVenc, ?data)>=-60");
            string chequesVencidos30Dias = String.Format(sqlBaseCheques, "<0 and datediff(dataVenc, ?data)>=-30");
            string chequesVencimentoHoje = String.Format(sqlBaseCheques, "=0");
            string chequesVencer30Dias = String.Format(sqlBaseCheques, ">0 and datediff(dataVenc, ?data)<=30");
            string chequesVencer60Dias = String.Format(sqlBaseCheques, ">30 and datediff(dataVenc, ?data)<=60");
            string chequesVencer90Dias = String.Format(sqlBaseCheques, ">60 and datediff(dataVenc, ?data)<=90");
            string chequesVencerMais90Dias = String.Format(sqlBaseCheques, ">60");

            string previsaoCustoFixoVencer30Dias = String.Format(sqlPrevisaoCustoFixo, ">0 and datediff(dataVenc, ?data)<=30",
                ">0 AND DATEDIFF(((DATA.DATA - interval DAY(DATA.DATA) DAY)+interval diaVenc DAY),?data)<=30");
            string previsaoCustoFixoVencer60Dias = String.Format(sqlPrevisaoCustoFixo, ">30 and datediff(dataVenc, ?data)<=60",
                ">30 AND DATEDIFF(((DATA.DATA - interval DAY(DATA.DATA) DAY)+interval diaVenc DAY),?data)<=60");
            string previsaoCustoFixoVencer90Dias = String.Format(sqlPrevisaoCustoFixo, ">60 and datediff(dataVenc, ?data)<=90",
                ">60 AND DATEDIFF(((DATA.DATA - interval DAY(DATA.DATA) DAY)+interval diaVenc DAY),?data)<=90");

            string sql = "select cast((" + vencidasMais90Dias + ") as decimal(12,2)) as VencidasMais90Dias, " +
                "cast((" + vencidas90Dias + ") as decimal(12,2)) as Vencidas90Dias, " +
                "cast((" + vencidas60Dias + ") as decimal(12,2)) as Vencidas60Dias, " +
                "cast((" + vencidas30Dias + ") as decimal(12,2)) as Vencidas30Dias, " +
                "cast((" + vencimentoHoje + ") as decimal(12,2)) as VencimentoHoje, " +
                "cast((" + vencer30Dias + ") as decimal(12,2)) as Vencer30Dias, " +
                "cast((" + vencer60Dias + ") as decimal(12,2)) as Vencer60Dias, " +
                "cast((" + vencer90Dias + ") as decimal(12,2)) as Vencer90Dias, " +
                "cast((" + vencerMais90Dias + ") as decimal(12,2)) as VencerMais90Dias, " +
                "cast((" + chequesVencidosMais90Dias + ") as decimal(12,2)) as ChequesVencidosMais90Dias, " +
                "cast((" + chequesVencidos90Dias + ") as decimal(12,2)) as ChequesVencidos90Dias, " +
                "cast((" + chequesVencidos60Dias + ") as decimal(12,2)) as ChequesVencidos60Dias, " +
                "cast((" + chequesVencidos30Dias + ") as decimal(12,2)) as ChequesVencidos30Dias, " +
                "cast((" + chequesVencimentoHoje + ") as decimal(12,2)) as ChequesVencimentoHoje, " +
                "cast((" + chequesVencer30Dias + ") as decimal(12,2)) as ChequesVencer30Dias, " +
                "cast((" + chequesVencer60Dias + ") as decimal(12,2)) as ChequesVencer60Dias, " +
                "cast((" + chequesVencer90Dias + ") as decimal(12,2)) as ChequesVencer90Dias, " +
                "cast((" + chequesVencerMais90Dias + ") as decimal(12,2)) as ChequesVencerMais90Dias";

            if (previsaoCustoFixo)
            {
                sql += ", cast((" + previsaoCustoFixoVencer30Dias + ") as decimal(12,2)) as PrevisaoCustoFixoVencer30Dias, " +
                    "cast((" + previsaoCustoFixoVencer60Dias + ") as decimal(12,2)) as PrevisaoCustoFixoVencer60Dias, " +
                    "cast((" + previsaoCustoFixoVencer90Dias + ") as decimal(12,2)) as PrevisaoCustoFixoVencer90Dias";
            }

            return sql;
        }

        private GDAParameter[] GetParams(string data)
        {
            List<GDAParameter> lista = new List<GDAParameter>();

            if (String.IsNullOrEmpty(data))
                data = DateTime.Now.ToShortDateString();
            
            lista.Add(new GDAParameter("?data", DateTime.Parse(data)));

            return lista.ToArray();
        }

        public PrevisaoFinanceira GetReceber(uint idLoja, string data, bool previsaoPedidos)
        {
            return objPersistence.LoadOneData(SqlReceber(idLoja, data, previsaoPedidos), GetParams(data));
        }

        /// <summary>
        /// Recupera a previsão financeira
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="data"></param>
        /// <param name="previsaoCustoFixo"></param>
        /// <returns></returns>
        public PrevisaoFinanceira GetPagar(uint idLoja, string data, bool previsaoCustoFixo)
        {
            return objPersistence.LoadOneData(SqlPagar(idLoja, data, previsaoCustoFixo), GetParams(data));
        }
    }
}
