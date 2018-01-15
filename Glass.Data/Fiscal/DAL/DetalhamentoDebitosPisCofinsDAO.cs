using GDA;
using Glass.Data.EFD;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class DetalhamentoDebitosPisCofinsDAO : BaseDAO<Model.DetalhamentoDebitosPisCofins, DetalhamentoDebitosPisCofinsDAO>
    {
        public List<Model.DetalhamentoDebitosPisCofins> ObtemParaEfd(string inicio, string fim, DataSourcesEFD.TipoImpostoEnum? tipoImposto)
        {
            var lstParam = new List<GDAParameter>();
            string sql = "select * from detalhamento_debitos_piscofins where 1";

            if (!String.IsNullOrEmpty(inicio)) {
                sql += " and dataPagamento>=?dataIni";
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(inicio + " 00:00:00")));
            }

            if (!String.IsNullOrEmpty(fim)) {
                sql += " and dataPagamento<=?dataFim";
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(fim + " 23:59:59")));
            }

            if (tipoImposto != null)
                sql += " and tipoImposto=" + (int)tipoImposto;

            return objPersistence.LoadData(sql, lstParam.ToArray());
        }
    }
}
