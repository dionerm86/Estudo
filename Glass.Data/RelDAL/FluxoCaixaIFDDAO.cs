using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class FluxoCaixaIFDDAO : BaseDAO<FluxoCaixaIFD, FluxoCaixaIFDDAO>
    {
        //private FluxoCaixaIFDDAO() { }

        private string Sql()
        {
            string dataInicioSemana = "date_sub(?dataIni, interval (dayofweek(?dataIni) - 1) day)";
            string dataFimSemana = "date_add(?dataFim, interval (7 - dayofweek(?dataFim)) day)";
            string dataInicioProxSemana = "date_add(" + dataInicioSemana + ", interval 1 week)";
            string dataFimProxSemana = "date_add(" + dataFimSemana + ", interval 1 week)";

            string sql = @"
                select fluxo_caixa_ifd.tipoMov as tipo, cast(sum(valorAtual) as decimal(12,2)) as valorAtual, cast(sum(valorSemana) as decimal(12,2)) as valorSemana,
                    cast(sum(valorProxSemana) as decimal(12,2)) as valorProxSemana, if(pc.idConta is null, '', if(fluxo_caixa_ifd.tipoMov=1, gc.descricao, cc.descricao)) as descrGrupoConta,
                    if(pc.idConta is null, fluxo_caixa_ifd.descricao, if(fluxo_caixa_ifd.tipoMov=1, pc.descricao, gc.descricao)) as descrPlanoConta
                from (
                    select idConta, Descricao, Valor as valorAtual, 0 as valorSemana, 0 as valorProxSemana, TipoMov
                    from (
                        " + FluxoCaixaDAO.Instance.Sql("?dataIni", "?dataFim", "0,1") + @"
                    ) as atual
                    union all select idConta, Descricao, 0 as valorAtual, Valor as valorSemana, 0 as valorProxSemana, TipoMov
                    from (
                        " + FluxoCaixaDAO.Instance.Sql(dataInicioSemana, dataFimSemana, "0,1") + @"
                    ) as semana
                    union all select idConta, Descricao, 0 as valorAtual, 0 as valorSemana, Valor as valorProxSemana, TipoMov
                    from (
                        " + FluxoCaixaDAO.Instance.Sql(dataInicioProxSemana, dataFimProxSemana, "0,1") + @"
                    ) as proxSemana
                ) as fluxo_caixa_ifd
                    left join plano_contas pc on (fluxo_caixa_ifd.idConta=pc.idConta)
                    left join grupo_conta gc on (pc.idGrupo=gc.idGrupo)
                    left join categoria_conta cc on (gc.idCategoriaConta=cc.idCategoriaConta)
                group by tipo, descrGrupoConta, descrPlanoConta";

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

        public IList<FluxoCaixaIFD> GetForRpt(string data)
        {
            return objPersistence.LoadData(Sql(), GetParams(data)).ToList();
        }
    }
}
