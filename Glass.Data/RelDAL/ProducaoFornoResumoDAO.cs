using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoFornoResumoDAO : BaseDAO<ProducaoFornoResumo, ProducaoFornoResumoDAO>
    {
        //private ProducaoFornoResumoDAO() { }

        private string Sql(int setor, DateTime dataIni, DateTime dataFim, bool relatorio, int idTurno, bool selecionar)
        {
            List<Turno> turnos = TurnoDAO.Instance.GetList();

            if (idTurno > 0)
                turnos = turnos.Where(f => f.IdTurno == (uint)idTurno).ToList();

            string campoTurno = "case";
            string m2QtdeTurno = "";
            string campoTotM2 = (PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? "pp.TotM2Calc" : "pp.TotM") + 
                "/(pp.qtde*if(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", a.qtde, 1))";

            string campoQtde = "(pp.qtde*if(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + ", a.qtde, 1))";

            foreach (Turno t in turnos)
            {
                TimeSpan inicio = new TimeSpan(Glass.Conversoes.StrParaInt(t.Inicio.Substring(0, 2)), Glass.Conversoes.StrParaInt(t.Inicio.Substring(3)), 0);
                TimeSpan termino = new TimeSpan(Glass.Conversoes.StrParaInt(t.Termino.Substring(0, 2)), Glass.Conversoes.StrParaInt(t.Termino.Substring(3)), 0);

                if (inicio.Ticks <= termino.Ticks)
                    campoTurno += " when dataLeitura>=cast(concat(date_format(dataLeitura, '%Y-%m-%d'), ' " + t.Inicio + "') as datetime) and dataLeitura<cast(concat(date_format(dataLeitura, '%Y-%m-%d'), ' " + t.Termino + "') as datetime) then " + t.IdTurno + Environment.NewLine;
                else
                    campoTurno += " when dataLeitura>=cast(concat(date_format(dataLeitura, '%Y-%m-%d'), ' " + t.Inicio + "') as datetime) or dataLeitura<cast(concat(date_format(dataLeitura, '%Y-%m-%d'), ' " + t.Termino + "') as datetime) then " + t.IdTurno + Environment.NewLine;

                m2QtdeTurno += "sum(if(lp.turno=" + t.IdTurno + ", " + campoTotM2 + ", 0)) as TotM2" + t.NumSeq + ", " + "sum(if(lp.turno=" + t.IdTurno + ", " + campoQtde + ", 0)) AS Qtde" + t.NumSeq + ", " + Environment.NewLine;
            }

            campoTurno += " end";

            string sqlBase = @"
                select lp.DataLeitura as Data, prod.idCorVidro, cv.Descricao as DescrCorVidro, prod.espessura, 
                    (p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @") as Producao,
                    " + m2QtdeTurno + @"t.descricao as Turno, t.numSeq as numSeqTurno, '$$$' as Criterio {0}
                from (
                    select * from (
                        select idProdPedProducao, dataLeitura, " + campoTurno + @" as turno
                        from leitura_producao
                        where dataLeitura>=?dtIni
                            and dataLeitura<=?dtFim
                            " + (setor > 0 ? "and idSetor=" + setor : "") + @"
                        order by dataLeitura asc
                    ) as temp
                    group by idProdPedProducao
                ) lp
                    inner join produto_pedido_producao ppp on (lp.idProdPedProducao=ppp.idProdPedProducao)
                    inner join produtos_pedido_espelho pp on (ppp.idProdPed=pp.idProdPed)
                    left join ambiente_pedido_espelho a On (pp.idAmbientePedido=a.idAmbientePedido) 
                    inner join pedido p on (pp.idPedido=p.idPedido)
                    inner join produto prod on (pp.idProd=prod.idProd)
                    left join cor_vidro cv on (prod.idCorVidro=cv.idCorVidro)
                    inner join turnos t on (lp.turno=t.idTurno)
                where ppp.situacao in (" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + "," + (int)ProdutoPedidoProducao.SituacaoEnum.Perda + @")
                    and p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                group by day(lp.DataLeitura), {1}t.numSeq";

            string criterio = "Período: " + dataIni.ToShortDateString() + " à " + dataFim.ToShortDateString();

            if (setor > 0)
                criterio += "    Setor " + Utils.ObtemSetor((uint)setor).Descricao;

            string sql;

            if (!relatorio)
                sql = String.Format(sqlBase, ", 0 as Ordenacao, 0 as Ordenacao1", @"cv.idCorVidro, prod.Espessura, 
                    (p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @"), ");
            else
            {
                string camposAdicionais;
                string groupBy;

                camposAdicionais = ", 'Cores' as TituloGrupo, cv.Descricao as Titulo, 1 as Ordenacao, 0 as Ordenacao1";
                groupBy = "cv.idCorVidro, ";
                sql = String.Format(sqlBase, camposAdicionais, groupBy);

                camposAdicionais = ", 'Espessuras' as TituloGrupo, concat(cast(prod.Espessura as char), 'mm') as Titulo, 2 as Ordenacao, prod.Espessura as Ordenacao1";
                groupBy = "prod.Espessura, ";
                sql += " union all " + String.Format(sqlBase, camposAdicionais, groupBy);

                camposAdicionais = ", 'Tipo' as TituloGrupo, if(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @", 'Box', 'Eng.') as Titulo, 3 as Ordenacao, 0 as Ordenacao1";
                groupBy = "p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @", ";
                sql += " union all " + String.Format(sqlBase, camposAdicionais, groupBy);

                camposAdicionais = ", 'Total' as TituloGrupo, '' as Titulo, 4 as Ordenacao, 0 as Ordenacao1";
                groupBy = "";
                sql += " union all " + String.Format(sqlBase, camposAdicionais, groupBy);
            }

            sql = "select " + (selecionar ? "*" : "count(*)") + " from (" + sql + ") as tabela";
            if (selecionar)
                sql += " order by Ordenacao, Ordenacao1, Data";

            var a = sql.Replace("$$$", criterio); 

            return sql.Replace("$$$", criterio);
        }

        public IList<ProducaoFornoResumo> GetList(int setor, DateTime dataIni, DateTime dataFim, int idTurno, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(setor, dataIni, dataFim, false, idTurno, true), sortExpression, startRow, pageSize, GetParams(dataIni.ToShortDateString(), dataFim.ToShortDateString()));
        }

        public int GetCount(int setor, DateTime dataIni, DateTime dataFim, int idTurno)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(setor, dataIni, dataFim, false, idTurno, false), GetParams(dataIni.ToShortDateString(), dataFim.ToShortDateString()));
        }

        private class ChaveRpt
        {
            public string Titulo, TituloGrupo;
            public uint NumSeqTurno;

            public override bool Equals(object obj)
            {
                if (!(obj is ChaveRpt))
                    return false;

                ChaveRpt comp = obj as ChaveRpt;
                return comp.Titulo == Titulo && comp.TituloGrupo == TituloGrupo && comp.NumSeqTurno == NumSeqTurno;
            }

            public override int GetHashCode()
            {
                return 1564463218;
            }
        }

        public IList<ProducaoFornoResumo> GetForRpt(int setor, DateTime dataIni, DateTime dataFim, int idTurno)
        {
            var retorno = new List<ProducaoFornoResumo>(objPersistence.LoadData(Sql(setor, dataIni, dataFim, true, idTurno, true), GetParams(dataIni.ToShortDateString(), dataFim.ToShortDateString())).ToList());
            var total = new List<ProducaoFornoResumo>();

            if (retorno.Count > 0)
            {
                Dictionary<ChaveRpt, int> posicoes = new Dictionary<ChaveRpt, int>();
                for (int i = 0; i < retorno.Count; i++)
                {
                    ChaveRpt chave = new ChaveRpt
                    {
                        NumSeqTurno = retorno[i].NumSeqTurno,
                        Titulo = retorno[i].Titulo,
                        TituloGrupo = retorno[i].TituloGrupo
                    };

                    if (!posicoes.ContainsKey(chave))
                    {
                        ProducaoFornoResumo novo = new ProducaoFornoResumo();
                        novo.Titulo = retorno[i].Titulo;
                        novo.TituloGrupo = retorno[i].TituloGrupo;
                        novo.Turno = retorno[i].Turno;
                        novo.NumSeqTurno = retorno[i].NumSeqTurno;

                        total.Add(novo);
                        posicoes.Add(chave, total.Count - 1);
                    }

                    int index = posicoes[chave];
                    total[index].TotM2Turno1 += retorno[i].TotM2Turno1;
                    total[index].TotM2Turno2 += retorno[i].TotM2Turno2;
                    total[index].TotM2Turno3 += retorno[i].TotM2Turno3;
                    total[index].TotM2Turno4 += retorno[i].TotM2Turno4;
                    total[index].TotM2Turno5 += retorno[i].TotM2Turno5;
                }
            }

            retorno.AddRange(total.ToArray());
            return retorno;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dtIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dtFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }
    }
}
