using System;
using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class PagtoAdministradoraCartaoDAO : BaseDAO<PagtoAdministradoraCartao, PagtoAdministradoraCartaoDAO>
    {
        //private PagtoAdministradoraCartaoDAO() { }

        #region Busca padrão

        private string Sql(uint idAdminCartao, string idsLojas, int mesInicio, int anoInicio, int mesFim, int anoFim, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string campos = selecionar ? "pac.*, ac.nome as nomeAdminCartao, coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja" : "count(*)";

            string sql = "select " + campos + @"
                from pagto_administradora_cartao pac
                    left join administradora_cartao ac on (pac.idAdminCartao=ac.idAdminCartao)
                    left join loja l on (pac.idLoja=l.idLoja)
                where 1";

            if (idAdminCartao > 0)
            {
                sql += " and pac.idAdminCartao=" + idAdminCartao;
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(idsLojas) && idsLojas != "0")
            {
                sql += " and pac.idLoja in (" + idsLojas + ")";
                temFiltro = true;
            }

            if (mesInicio > 0)
            {
                sql += " and pac.mes>=" + mesInicio;
                temFiltro = true;
            }

            if (anoInicio > 0)
            {
                sql += " and pac.ano>=" + anoInicio;
                temFiltro = true;
            }

            if (mesFim > 0)
            {
                sql += " and pac.mes<=" + mesFim;
                temFiltro = true;
            }

            if (anoFim > 0)
            {
                sql += " and pac.ano<=" + anoFim;
                temFiltro = true;
            }

            return sql;
        }

        public IList<PagtoAdministradoraCartao> GetList(uint idAdminCartao, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idAdminCartao) == 0)
                return new PagtoAdministradoraCartao[1] { new PagtoAdministradoraCartao() };

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "pac.ano desc, pac.mes desc, pac.idLoja asc";
            
            bool temFiltro;
            return LoadDataWithSortExpression(Sql(idAdminCartao, null, 0, 0, 0, 0, true, out temFiltro), sortExpression, startRow, pageSize, temFiltro);
        }

        public int GetCount(uint idAdminCartao)
        {
            int real = GetCountReal(idAdminCartao);
            return real > 0 ? real : 1;
        }

        public int GetCountReal(uint idAdminCartao)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(Sql(idAdminCartao, null, 0, 0, 0, 0, false, out temFiltro), temFiltro);
        }

        #endregion

        #region Busca para EFD

        public IList<PagtoAdministradoraCartao> GetForEFD(string idsLojas, DateTime inicio, DateTime fim)
        {
            bool temFiltro;
            return objPersistence.LoadData(Sql(0, idsLojas, inicio.Month, inicio.Year, fim.Month, fim.Year, true, out temFiltro)).ToList();
        }

        #endregion

        #region Métodos sobrescritos

        private void ValidarPagto(PagtoAdministradoraCartao pac, bool inserir)
        {
            if (inserir)
            {
                string sql = "select count(*) from pagto_administradora_cartao where idAdminCartao=" + pac.IdAdminCartao +
                    " and idLoja=" + pac.IdLoja + " and mes=" + pac.Mes + " and ano=" + pac.Ano;

                if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                    throw new Exception("Já existe um pagamento associado a essa loja e período.");
            }
        }

        public override uint Insert(PagtoAdministradoraCartao objInsert)
        {
            ValidarPagto(objInsert, true);
            return base.Insert(objInsert);
        }

        public override int Update(PagtoAdministradoraCartao objUpdate)
        {
            ValidarPagto(objUpdate, false);
            return base.Update(objUpdate);
        }

        #endregion
    }
}
