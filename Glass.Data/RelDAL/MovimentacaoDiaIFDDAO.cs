using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class MovimentacaoDiaIFDDAO : BaseDAO<MovimentacaoDiaIFD, MovimentacaoDiaIFDDAO>
    {
        //private MovimentacaoDiaIFDDAO() { }

        #region SQL das tabelas

        private string SqlVendas()
        {
            return SqlVendas("?dataIni", "?dataFim", null);
        }

        private string SqlVendas(string paramDataIni, string paramDataFim, int? tipo)
        {
            int tipoPrazo = tipo == null ? 0 : tipo.Value;
            int tipoVista = tipo == null ? 1 : tipo.Value;

            string sql = @"
                select tabela, tipo, idConta, cast(sum(valor) as decimal(12,2)) as valor
                from (
                    select " + (int)MovimentacaoDiaIFD.TabelaMovimentacao.Vendas + @" as tabela, 
                        " + tipoVista + @" as tipo, null as idConta, sum(valor) as valor
                    from caixa_diario
                    where idConta in (" + UtilsPlanoConta.ContasAVista() + "," + UtilsPlanoConta.ContasSinalPedido() + @")
                        {0}
                    group by idConta
                    
                    union select " + (int)MovimentacaoDiaIFD.TabelaMovimentacao.Vendas + @" as tabela, 
                        " + tipoVista + @" as tipo, null as idConta, sum(valorMov) as valor
                    from caixa_geral
                    where idConta in (" + UtilsPlanoConta.ContasAVista() + "," + UtilsPlanoConta.ContasSinalPedido() + @")
                        {0}
                    group by idConta
                    
                    union select " + (int)MovimentacaoDiaIFD.TabelaMovimentacao.Vendas + @" as tabela, 
                        " + tipoPrazo + @" as tipo, idConta, sum(valorVec) as valor
                    from contas_receber
                    where idConta in (" + UtilsPlanoConta.ContasAPrazoContasReceber() + "," + UtilsPlanoConta.ContasAPrazo() + @")
                        {0}
                    group by idConta
                ) as movimentacao_dia_ifd
                group by tabela, tipo, idConta";

            string filtro = "";
            if (!PedidoConfig.LiberarPedido)
                filtro = "and idPedido in (select idPedido from pedido where situacao=" + (int)Pedido.SituacaoPedido.Confirmado + " and dataConf>={0} and dataConf<={1})";
            else
                filtro = "and idLiberarPedido in (select idLiberarPedido from liberarpedido where situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado + " and dataLiberacao>={0} and dataLiberacao<={1})";

            sql = String.Format(sql, filtro);
            sql = String.Format(sql, paramDataIni, paramDataFim);
            return sql;
        }

        private string SqlObrigacoes()
        {
            return SqlObrigacoes("?dataIni", "?dataFim", null);
        }

        private string SqlObrigacoes(string paramDataIni, string paramDataFim, int? tipo)
        {
            int tipoPrazo = tipo == null ? 0 : tipo.Value;

            string sql = @"
                select " + (int)MovimentacaoDiaIFD.TabelaMovimentacao.Obrigacoes + @" as tabela,
                    " + tipoPrazo + @" as tipo, gc.idCategoriaConta as idConta, sum(cp.valorVenc) as valor
                from contas_pagar cp
                    left join plano_contas pc on (cp.idConta=pc.idConta)
                    left join grupo_conta gc on (pc.idGrupo=gc.idGrupo)
                where cp.dataCad>={0}
                    and cp.dataCad<={1}
                group by gc.idGrupo";

            sql = String.Format(sql, paramDataIni, paramDataFim);
            return sql;
        }

        private string SqlAcumulado()
        {
            string dataInicioMes = "cast(concat(year(?dataIni), '-', month(?dataIni), '-01') as datetime)";
            string dataFimMes = "date_sub(date_add(" + dataInicioMes + ", interval 1 month), interval 1 second)";

            string vendas = SqlVendas(dataInicioMes, dataFimMes, 0);
            string obrigacoes = SqlObrigacoes(dataInicioMes, dataFimMes, 1);

            string sql = @"
                select " + (int)MovimentacaoDiaIFD.TabelaMovimentacao.Acumulado + @" as tabela,
                    tipo, null as idConta, sum(if(tipo=0, valor, -valor)) as valor
                from (
                    " + vendas + @"
                    union " + obrigacoes + @"
                ) as acumulado
                group by tipo";

            return sql;
        }

        #endregion

        private string Sql()
        {
            string sql = @"
                select tabela, tipo, idConta, sum(valor) as valor
                from (
                    " + SqlVendas() + @"
                    union " + SqlObrigacoes() + @"
                    union " + SqlAcumulado() + @"
                ) as movimentacao_dia_ifd
                group by tabela, tipo, idConta";

            return sql;
        }

        private GDAParameter[] GetParams(string data)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(data))
            {
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(data + " 00:00:00")));
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(data + " 23:59:59")));
            }

            return lstParam.ToArray();
        }

        public IList<MovimentacaoDiaIFD> GetForRpt(string data)
        {
            return objPersistence.LoadData(Sql(), GetParams(data)).ToList();
        }
    }
}
