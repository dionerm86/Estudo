using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class EstoqueIFDDAO : BaseDAO<EstoqueIFD, EstoqueIFDDAO>
    {
        //private EstoqueIFDDAO() { }

        private string Sql(string data)
        {
            string gruposBase = "select idGrupoProd from grupo_prod where tipoGrupo=";
            string gruposMercadoria = gruposBase + (int)TipoGrupoProd.Mercadoria;
            string gruposConsumo = gruposBase + (int)TipoGrupoProd.UsoConsumo;
            string gruposDiversos = gruposBase + (int)TipoGrupoProd.Diversos;

            string campoSomar = "abs(valor)", filtroAdicional;
            bool temFiltro;

            string sqlSaldoAnterior = MovimentacaoEstoqueDAO.Instance.SqlEstoqueInicial(0, 0, null, null, null, null, data, data, 0, 0, 0, "", null,
                0, 0, 0, false, false, false, null, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional);

            string sqlMovimentacoes = MovimentacaoEstoqueDAO.Instance.Sql(0, 0, null, null, null, null, data, data, 0, 0, 0, "", null, 0, 0, 0,
                false, false, false, null, true, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional);

            string sql = @"
                select tipo, cast(sum(valorMercadoria) as decimal(12,2)) as valorMercadoria, cast(sum(valorConsumo) as decimal(12,2)) as valorConsumo, 
                    cast(sum(valorDiversos) as decimal(12,2)) as valorDiversos
                from (
                    select if(estoqueInicial, 0, tipo) as tipo, sum(if(p.idGrupoProd in (" + gruposMercadoria + @"), " + campoSomar + @", 0)) as valorMercadoria,
                        sum(if(p.idGrupoProd in (" + gruposConsumo + @"), " + campoSomar + @", 0)) as valorConsumo, sum(if(p.idGrupoProd in (" + 
                        gruposDiversos + @"), " + campoSomar + @", 0)) as valorDiversos
                    from (
                        " + sqlSaldoAnterior + @"
                        union all " + sqlMovimentacoes + @"
                    ) as temp
                        left join produto p on (temp.idProd=p.idProd)
                    group by tipo
                    
                    union all select 0 as tipo, 0 as valorMercadoria, 0 as valorConsumo, 0 as valorDiversos
                    union all select 1 as tipo, 0 as valorMercadoria, 0 as valorConsumo, 0 as valorDiversos
                    union all select 2 as tipo, 0 as valorMercadoria, 0 as valorConsumo, 0 as valorDiversos
                ) as estoque_ifd
                group by tipo";

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

        public IList<EstoqueIFD> GetForRpt(string data)
        {
            return objPersistence.LoadData(Sql(data), GetParams(data)).ToList();
        }
    }
}
