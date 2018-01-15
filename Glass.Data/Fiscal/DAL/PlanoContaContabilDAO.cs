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
            return DeleteByPrimaryKey((uint)objDelete.IdContaContabil);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            #region Verifica se existe associação

            /* Chamado 57311.
             * Impede que o plano de conta contábil seja removido do sistema caso esteja sendo utilizado. */

            var mensagemBloqueio = "Não é possível apagar esse plano de conta contábil, pois, ele está associado a pelo menos {0}.";
            var associacoesBloqueio = new List<string>();

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM bem_ativo_imobilizado WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("um bem ativo imobilizado");

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM cliente WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("um cliente");

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM efd_cte WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("um EFD CTe");

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM fornecedor WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("um fornecedor");

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM info_adicional_nf WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("uma informação adicional de nota fiscal");

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM plano_contas WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("um plano de contas");

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM produto WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("um produto");

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM produtos_nf WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("um produto de nota fiscal");

            if (objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM receitas_diversas WHERE IdContaContabil={0}", Key)) > 0)
                associacoesBloqueio.Add("uma receita diversa");

            if (associacoesBloqueio.Count > 0)
                throw new Exception(string.Format(mensagemBloqueio, string.Join(", ", associacoesBloqueio)));
            
            #endregion

            return GDAOperations.Delete( new PlanoContaContabil { IdContaContabil = (int)Key } );
        }

        #endregion
    }
}
