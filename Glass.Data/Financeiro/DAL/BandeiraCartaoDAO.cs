using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class BandeiraCartaoDAO : BaseDAO<BandeiraCartao, BandeiraCartaoDAO>
    {
        #region Busca Padrão

        public string Sql(bool selecionar)
        {
            var sql = "SELECT " + (selecionar ? "*" : "Count(*)") + " FROM bandeira_cartao WHERE 1";
            return sql;
        }

        public IList<BandeiraCartao> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(true), sortExpression, startRow, pageSize);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(false));
        }

        #endregion

        public bool BandeiraCartaoEmUso(uint idBandeiraCartao)
        {
            return objPersistence.ExecuteSqlQueryCount(string.Format(@"
                SELECT COUNT(IDTIPOCARTAO) FROM tipo_cartao_credito WHERE BANDEIRA={0}", idBandeiraCartao)) > 0;
        }

        public bool BandeiraCartaoEmUsoAtivos(uint idBandeiraCartao)
        {
            return objPersistence.ExecuteSqlQueryCount(string.Format(@"
                SELECT COUNT(IDTIPOCARTAO) FROM tipo_cartao_credito WHERE BANDEIRA={0} AND SITUACAO=1", idBandeiraCartao)) > 0;
        }

        /// <summary>
        /// Retorna a descrição da bandeira de cartão
        /// </summary>
        /// <param name="idBandeiraCartao"></param>
        /// <returns></returns>
        public string ObterDescricaoBandeira(uint idBandeiraCartao)
        {
            return ObtemValorCampo<string>("Descricao", "IdBandeiraCartao=" + idBandeiraCartao);
        }

        public uint ObterIdBandeiraPelaDescricao(string descricao)
        {
            return ObtemValorCampo<uint>("IdBandeiraCartao", "Descricao=?descricao", new GDA.GDAParameter("?descricao", descricao));
        }

        public IList<BandeiraCartao> GetAtivas()
        {
            var sql = Sql(true) + " AND Situacao=1 ";
            return objPersistence.LoadData(sql).ToList();
        }

        #region Métodos Sobrescritos

        public override int Update(BandeiraCartao objUpdate)
        {
            var oldObject = GetElementByPrimaryKey(objUpdate.IdBandeiraCartao);

            LogAlteracaoDAO.Instance.LogBandeiraCartao(oldObject, objUpdate);

            if (oldObject.Situacao != objUpdate.Situacao && objUpdate.Situacao == Situacao.Inativo && BandeiraCartaoEmUsoAtivos(objUpdate.IdBandeiraCartao))
                throw new Exception("A bandeira de cartão não pode ser Inativada pois está em uso em cartões ativos.");

            return base.Update(objUpdate);
        }

        public override int Delete(BandeiraCartao objDelete)
        {
            if (BandeiraCartaoEmUso(objDelete.IdBandeiraCartao))
                throw new Exception("A bandeira de cartão não pode ser deletada pois está em uso!");

            return base.Delete(objDelete);
        }

        #endregion
    }
}
