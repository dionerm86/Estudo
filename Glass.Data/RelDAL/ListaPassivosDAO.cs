using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class ListaPassivosDAO : BaseDAO<ListaPassivos, ListaPassivosDAO>
    {
        //private ListaPassivosDAO() { }

        private string Sql(uint idLoja, string dtIni, string dtFim, uint idGrupoConta, string planoConta, bool agrupar, bool selecionar)
        {
            string camposCP = selecionar ? @"c.idConta, if(paga=true, dataPagto, dataVenc) as dataMov, 
                if(paga=true, 0, valorVenc) as valorPagar, if(paga=true, valorPago, 0) as valorPago" : "c.idConta";
            
            string camposCG = selecionar ? @"c.idConta, dataMov, if(tipoMov=2, valorMov, 0) as valorPagar, 
                if(tipoMov=1, valorMov, 0) as valorPago" : "c.idConta";
            
            string camposMB = selecionar ? @"m.idConta, dataMov, if(tipoMov=2, valorMov, 0) as valorPagar, 
                if(tipoMov=1, valorMov, 0) as valorPago" : "m.idConta";

            string campos = selecionar ? @"lista_passivos.idConta, {0}(lista_passivos.valorPagar) as valorPagar, 
                {0}(lista_passivos.valorPago) as valorPago, lista_passivos.dataMov, 
                concat(gc.descricao, ' - ', pc.descricao) as descrPlanoConta, '$$$' as criterio" :
                agrupar ? "count(distinct lista_passivos.idConta)" : "count(*)";

            string whereCP = "", whereCG = "", whereMB = "";
            string criterio = "";

            #region Define o filtro

            if (idLoja > 0)
            {
                whereCP += " and idLoja=" + idLoja;
                whereCG += " and idLoja=" + idLoja;
                whereMB += " and f.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (!String.IsNullOrEmpty(dtIni))
            {
                whereCP += " and if(paga=true, dataPagto, dataVenc)>=?dtIni";
                whereCG += " and dataMov>=?dtIni";
                whereMB += " and dataMov>=?dtIni";
                criterio += "Data início: " + dtIni + "    ";
            }

            if (!String.IsNullOrEmpty(dtFim))
            {
                whereCP += " and if(paga=true, dataPagto, dataVenc)<=?dtFim";
                whereCG += " and dataMov<=?dtFim";
                whereMB += " and dataMov<=?dtFim";
                criterio += "Data fim: " + dtFim + "    ";
            }

            if (idGrupoConta > 0)
            {
                whereCP += " and p.idGrupo=" + idGrupoConta;
                whereCG += " and p.idGrupo=" + idGrupoConta;
                whereMB += " and p.idGrupo=" + idGrupoConta;
                criterio += "Grupo Conta: " + GrupoContaDAO.Instance.ObtemValorCampo<string>("Descricao", "idGrupo=" + idGrupoConta);
            }

            if (!String.IsNullOrEmpty(planoConta))
            {
                string idsPlano = Glass.Data.DAL.PlanoContasDAO.Instance.GetIds(planoConta);

                if (!String.IsNullOrEmpty(idsPlano))
                {
                    whereCP += " and c.idConta in (" + idsPlano + ")";
                    whereCG += " and c.idConta in (" + idsPlano + ")";
                    whereMB += " and m.idConta in (" + idsPlano + ")";

                    criterio += "Plano de conta: " + planoConta + "    ";
                }
                else // Se não houverem ids para o filtro não busca nada
                {
                    whereCP += " and 0";
                    whereCG += " and 0";
                    whereMB += " and 0";
                }
            }

            #endregion

            string sql = "select " + campos + @"
                from (
                    select " + camposCP + @"
                    from contas_pagar c
                    Inner Join plano_contas p On (c.idConta=p.idConta)
                    where coalesce(renegociada,false)=false" + whereCP + @"
                    
                    union all select " + camposCG + @"
                    from caixa_geral c
                    Inner Join plano_contas p On (c.idConta=p.idConta)
                    where lancManual=true" + whereCG + @"
                    
                    union all select " + camposMB + @"
                    from mov_banco m
                    Inner Join plano_contas p On (m.idConta=p.idConta)
                    inner join funcionario f On (m.usuCad=f.idFunc)
                    where lancManual=true" + whereMB + @"
                ) as lista_passivos
                    inner join plano_contas pc on (lista_passivos.idConta=pc.idConta)
                    inner join grupo_conta gc on (pc.idGrupo=gc.idGrupo) " +
                (agrupar && selecionar ? "group by lista_passivos.idConta" : "");

            sql = String.Format(sql, agrupar ? "sum" : "");
            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string dtIni, string dtFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dtIni))
                lst.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtFim))
                lst.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59")));

            return lst.ToArray();
        }

        public IList<ListaPassivos> GetList(uint idLoja, string dtIni, string dtFim, uint idGrupoConta, string planoConta, 
            string sortExpression, int startRow, int pageSize)
        {
            string sql = Sql(idLoja, dtIni, dtFim, idGrupoConta, planoConta, true, true);
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "descrPlanoConta asc";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParams(dtIni, dtFim));
        }

        public int GetCount(uint idLoja, string dtIni, string dtFim, uint idGrupoConta, string planoConta)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idLoja, dtIni, dtFim, idGrupoConta, planoConta, true, false), GetParams(dtIni, dtFim));
        }

        public IList<ListaPassivos> GetForRpt(uint idLoja, string dtIni, string dtFim, uint idGrupoConta, string planoConta)
        {
            string sql = Sql(idLoja, dtIni, dtFim, idGrupoConta, planoConta, false, true) + " order by descrPlanoConta, lista_passivos.dataMov asc";
            return objPersistence.LoadData(sql, GetParams(dtIni, dtFim)).ToList();
        }
    }
}
