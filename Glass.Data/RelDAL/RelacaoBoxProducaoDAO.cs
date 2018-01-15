using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class RelacaoBoxProducaoDAO : BaseDAO<RelacaoBoxProducao, RelacaoBoxProducaoDAO>
    {
        //private RelacaoBoxProducaoDAO() { }

        private string Sql(string data, bool selecionar)
        {
            string criterio = "Data: " + data;
            string campos = selecionar ? "*, '$$$' as Criterio" : "count(*)";
            string campoQtde = "sum(if(lp.idSetor in (select idSetor from setor where forno=true) and lp.dataLeitura>={0} and lp.dataLeitura<={1}, 1, 0))";

            string sql = @"
                select " + campos + @"
                from (
                    select if(ppe.altura=1880, 'Móvel', 'Fixo') as modelo, ppe.altura, ppe.largura, cv.descricao as descrCorVidro, 
                        cast(" + String.Format(campoQtde, "?dataIniAnt", "?dataFimAnt") + @" as signed) as qtdeAnterior,
                        cast(" + String.Format(campoQtde, "?dataIni", "?dataFim") + @" as signed) as qtde,
                        cast(greatest((select sum(coalesce(estMinimo,0)-(coalesce(qtdEstoque,0)-coalesce(reserva,0)-coalesce(liberacao,0))) from produto_loja where idProd=ppe.idProd),0) as signed) as qtdeProduzir
                    from leitura_producao lp
                        left join produto_pedido_producao ppp on (lp.idProdPedProducao=ppp.idProdPedProducao)
                        left join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                        left join produto p on (ppe.idProd=p.idProd)
                        left join cor_vidro cv on (p.idCorVidro=cv.idCorVidro)
                        left join pedido ped on (ppe.idPedido=ped.idPedido)
                    where ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @"
                        and ppe.altura in (1845,1850,1880)
                        and ppp.situacao in (" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @")
                    group by ppe.altura, ppe.largura, p.idCorVidro
                    order by ppe.altura, ppe.largura, cv.descricao
                ) as relacao_box";

            sql = sql.Replace("$$$", criterio);
            return sql;
        }

        private GDAParameter[] GetParams(string data)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(data))
            {
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(data + " 00:00")));
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(data + " 23:59")));
                lst.Add(new GDAParameter("?dataIniAnt", DateTime.Parse(data + " 00:00").AddDays(-1)));
                lst.Add(new GDAParameter("?dataFimAnt", DateTime.Parse(data + " 23:59").AddDays(-1)));
            }

            return lst.ToArray();
        }

        public IList<RelacaoBoxProducao> GetForRpt(string data)
        {
            return objPersistence.LoadData(Sql(data, true), GetParams(data)).ToList();
        }

        public IList<RelacaoBoxProducao> GetList(string data, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(data, true), sortExpression, startRow, pageSize, true, GetParams(data));
        }

        public int GetCount(string data)
        {
            return GetCountWithInfoPaging(Sql(data, true), true, null, GetParams(data));
        }
    }
}
