using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using GDA;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class GraficoOrcamentosDAO : BaseDAO<GraficoOrcamentos, GraficoOrcamentosDAO>
    {
        //private GraficoOrcamentosDAO() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idVendedor"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="agrupar"></param>
        /// <param name="series">Se verdadeiro, busca apenas séries (do gráfico), senão busca todos os dados</param>
        /// <returns></returns>
        public IList<GraficoOrcamentos> GetOrcamentos(uint idLoja, uint idVendedor, IEnumerable<int> situacao, string dataIni, string dataFim, int agrupar, bool series)
        {
            string data = agrupar != 3 ? "DataCad" : "DataSituacao";

            string campos = @"o.idLoja, o.idFunc, cast(Sum(o.Total) as decimal(12,2)) as TotalVenda, f.Nome as NomeVendedor, coalesce(l.nomeFantasia, l.razaoSocial) as NomeLoja, 
                (Right(Concat('0', Cast(Month(o." + data + ") as char), '/', Cast(Year(o." + data + @") as char)), 7)) as DataVenda, 
                o.situacao, '$$$' as Criterio";

            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @" 
                From orcamento o
                    left join funcionario f On (o.idFunc=f.idFunc)
                    left join loja l on (o.idLoja=l.idLoja)
                Where 1";

            if (idLoja > 0)
            {
                sql += " And o.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idVendedor > 0)
            {
                sql += " And o.idFunc=" + idVendedor;
                criterio += "Vendedor: " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor)) + "    ";
            }

            if (situacao != null && situacao.Count() > 0 && situacao.ToList()[0] != 0)
            {
                sql+=string.Format(" And o.Situacao IN ({0})", string.Join(",", situacao));

                criterio += "Situação: " + string.Join(",", situacao.Select(f => Orcamento.GetDescrSituacao(f)).ToList());
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And o." + data + ">=?dataIni";
                criterio += "Data Início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And o." + data + "<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
            }

            // Agrupar por loja
            if (agrupar == 1)
            {
                sql += " and o.idLoja is not null Group By o.idLoja";

                if (!series)
                    sql += ", (Right(Concat('0', Cast(Month(o." + data + ") as char), '/', Cast(Year(o." + data + ") as char)), 7))";

                sql += " Order By coalesce(l.Nomefantasia, l.razaoSocial)";
            }
            // Agrupar por vendedor
            else if (agrupar == 2)
            {
                sql += " and o.idFunc is not null Group By o.idFunc";

                if (!series)
                    sql += ", (Right(Concat('0', Cast(Month(o." + data + ") as char), '/', Cast(Year(o." + data + ") as char)), 7))";

                sql += " Having sum(o.Total) > 500";

                sql += " Order By f.Nome";
            }
            // Agrupar por situação
            else if (agrupar == 3)
            {
                sql += " and o.situacao is not null Group By o.situacao";

                if (!series)
                    sql += ", (Right(Concat('0', Cast(Month(o." + data + ") as char), '/', Cast(Year(o." + data + ") as char)), 7))";

                sql += " Order By o.situacao";
            }
            else
            // Nenhum
            {
                if (!series)
                    sql += " Group By (Right(Concat('0', Cast(Month(o." + data + ") as char), '/', Cast(Year(o." + data + ") as char)), 7))";
            }
            
            return objPersistence.LoadData(sql.Replace("$$$", criterio), GetParams(dataIni, dataFim)).ToList();
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }

        public GraficoOrcamentosImagem[] ObterGraficoOrcamentosImagem()
        {
            return new GraficoOrcamentosImagem[0];
        }
    }
}
