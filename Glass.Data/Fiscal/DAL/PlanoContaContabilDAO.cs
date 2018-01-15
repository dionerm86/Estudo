using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PlanoContaContabilDAO : BaseDAO<PlanoContaContabil, PlanoContaContabilDAO>
    {
        //private PlanoContaContabilDAO() { }

        #region Busca padrão

        private string Sql(bool selecionar)
        {
            string campos = selecionar ? "pcc.*" : "count(*)";
            string sql = "select " + campos + @"
                from plano_conta_contabil pcc
                where 1";

            return sql;
        }

        public IList<PlanoContaContabil> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new PlanoContaContabil[] { new PlanoContaContabil() };

            return LoadDataWithSortExpression(Sql(true), sortExpression, startRow, pageSize);
        }

        public int GetCount()
        {
            int retorno = GetCountReal();
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(false));
        }

        #endregion

        #region Busca para filtros

        public IList<PlanoContaContabil> GetSorted(int natureza)
        {
            string sql = "select * from plano_conta_contabil";

            if (natureza > 0)
                sql += " where natureza=" + natureza;

            sql += " order by descricao";
            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Natureza do plano de contas

        public GenericModel[] GetNaturezas()
        {
            List<GenericModel> retorno = new List<GenericModel>();

            PlanoContaContabil temp = new PlanoContaContabil();
            foreach (int n in Enum.GetValues(typeof(PlanoContaContabil.NaturezaEnum)))
            {
                temp.Natureza = n;
                retorno.Add(new GenericModel((uint)n, temp.DescrNatureza));
            }

            retorno.Sort(new Comparison<GenericModel>(
                delegate(GenericModel x, GenericModel y)
                {
                    return x.Descr.CompareTo(y.Descr);
                }
            ));

            return retorno.ToArray();
        }

        #endregion

        #region Obtém descrição

        /// <summary>
        /// Retorna a descrição do plano de contas contábil.
        /// </summary>
        /// <param name="idContaContabil"></param>
        /// <returns></returns>
        public string ObtemDescricao(uint idContaContabil)
        {
            return ObtemValorCampo<string>("descricao", "idContaContabil=" + idContaContabil);
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(PlanoContaContabil objInsert)
        {
            objInsert.DataCad = DateTime.Now;
            objInsert.Usucad = UserInfo.GetUserInfo.CodUser;

            PlanoContaContabil vazio = new PlanoContaContabil();
            vazio.IdContaContabil = (int)base.Insert(objInsert);
            LogAlteracaoDAO.Instance.LogPlanoContaContabil(vazio, LogAlteracaoDAO.SequenciaObjeto.Atual);

            return (uint)vazio.IdContaContabil;
        }

        public override int Update(PlanoContaContabil objUpdate)
        {
            LogAlteracaoDAO.Instance.LogPlanoContaContabil(objUpdate, LogAlteracaoDAO.SequenciaObjeto.Novo);
            return base.Update(objUpdate);
        }

        public override int Delete(PlanoContaContabil objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdContaContabil);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (CurrentPersistenceObject.ExecuteSqlQueryCount(@"select count(*) from produto where IDCONTACONTABIL=" + Key) > 0)
                throw new Exception("Não é possível apagar esse plano de conta contábil porque ele está associado a um produto.");

            return GDAOperations.Delete( new PlanoContaContabil { IdContaContabil = (int)Key } );
        }

        #endregion
    }
}
