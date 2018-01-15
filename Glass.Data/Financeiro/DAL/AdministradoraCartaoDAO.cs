using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class AdministradoraCartaoDAO : BaseDAO<AdministradoraCartao, AdministradoraCartaoDAO>
    {
        //private AdministradoraCartaoDAO() { }

        #region Busca padrão

        private string Sql(uint idAdminCartao, bool selecionar)
        {
            string campos = selecionar ? "ac.*, c.nomeCidade, c.nomeUf, c.codIbgeCidade, c.codIbgeUf" : "count(*)";
            string sql = "select " + campos + @"
                from administradora_cartao ac
                    left join cidade c on (ac.idCidade=c.idCidade)
                where 1";

            return sql;
        }

        public IList<AdministradoraCartao> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false));
        }

        public AdministradoraCartao GetElement(uint idAdminCartao)
        {
            List<AdministradoraCartao> item = objPersistence.LoadData(Sql(idAdminCartao, true));
            return item.Count > 0 ? item[0] : null;
        }

        #endregion

        #region Busca o nome da administradora de cartão

        /// <summary>
        /// Busca o nome da administradora de cartão.
        /// </summary>
        /// <param name="idAdminCartao"></param>
        /// <returns></returns>
        public string GetNome(uint idAdminCartao)
        {
            string sql = "select nome from administradora_cartao where idAdminCartao=" + idAdminCartao;
            return ExecuteScalar<string>(sql);
        }

        #endregion

        #region Busca para EFD

        public IList<AdministradoraCartao> GetForEFD(uint idLoja, DateTime inicio, DateTime fim)
        {
            string sql = @"select *
                from administradora_cartao
                where idAdminCartao in (select * from (
                    select idAdminCartao
                    from pagto_administradora_cartao
                    where idLoja=" + idLoja + @"
                        and mes>=" + inicio.Month + @"
                        and ano>=" + inicio.Year + @"
                        and mes<=" + fim.Month + @"
                        and ano<=" + fim.Year + @"
                ) as temp)
                or idAdminCartao in (select * from (
                    select idAdminCartao
                    from documento_fiscal
                    where idNf in (select * from (
                        select idNf
                        from nota_fiscal
                        where idLoja=" + idLoja + @"
                            and dataEmissao>=?dataIni
                            and dataEmissao<=?dataFim
                    ) as temp2)
                ) as temp1)";

            return objPersistence.LoadData(sql, new GDAParameter("?dataIni", inicio), new GDAParameter("?dataFim", fim)).ToList();
        }

        #endregion

        #region Métodos sobrescritos

        public override int Update(AdministradoraCartao objUpdate)
        {
            LogAlteracaoDAO.Instance.LogAdministradoraCartao(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(AdministradoraCartao objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdAdminCartao);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (objPersistence.ExecuteSqlQueryCount("select count(*) from pagto_administradora_cartao where idAdminCartao=" + Key) > 0)
                throw new Exception("Não é possível excluir essa administradora de cartão pois ela já tem pagamentos associados.");

            return base.DeleteByPrimaryKey(Key);
        }

        #endregion
    }
}
