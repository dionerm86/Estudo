using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class CategoriaContaDAO : BaseDAO<CategoriaConta, CategoriaContaDAO>
    {
        //private CategoriaContaDAO() { }

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From categoria_conta";

            return sql;
        }

        public IList<CategoriaConta> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                List<CategoriaConta> lst = new List<CategoriaConta>();
                lst.Add(new CategoriaConta());
                return lst.ToArray();
            }

            string sort = String.IsNullOrEmpty(sortExpression) ? " Order By numSeq" : String.Empty;

            return objPersistence.LoadDataWithSortExpression(SqlList(true) + sort, new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize), null).ToList();
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(false), null);

            return count == 0 ? 1 : count;
        }

        public IList<CategoriaConta> GetOrdered()
        {
            string sql = string.Format("SELECT * FROM categoria_conta WHERE tipo<>{0} AND Situacao={1} ORDER BY descricao ASC", 
                (int)TipoCategoriaConta.Subtotal,
                (int)Glass.Situacao.Ativo);

            return objPersistence.LoadData(sql).ToList();
        }

        public string ObtemDescricao(uint idCategoriaConta)
        {
            return ObtemValorCampo<string>("descricao", "idCategoriaConta=" + idCategoriaConta);
        }

        #region Busca Ids das categorias de conta

        /// <summary>
        /// Busca Ids das categorias de conta
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        public string GetIds(string descricao)
        {
            string sql = "select idCategoriaConta from categoria_conta where descricao like ?descricao";
            return GetValoresCampo(sql, "idCategoriaConta", new GDAParameter("?descricao", "%" + descricao + "%"));
        }

        #endregion

        #region Troca posição da categoria de contas

        /// <summary>
        /// Troca posição da categoria de contas
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <param name="acima"></param>
        public void ChangePosition(uint idCategoriaConta, bool acima)
        {
            int numSeq = ObtemValorCampo<int>("numSeq", "idCategoriaConta=" + idCategoriaConta);

            // Altera a posição da categoria adjacente à esta
            objPersistence.ExecuteCommand("Update categoria_conta Set numSeq=numSeq" + (acima ? "+1" : "-1") +
                " Where numSeq=" + (numSeq + (acima ? -1 : 1)));

            // Altera a posição deste beneficiamento
            objPersistence.ExecuteCommand("Update categoria_conta Set numSeq=numSeq" + (acima ? "-1" : "+1") +
                " Where idCategoriaConta=" + idCategoriaConta);
        }

        #endregion

        #region Métodos Sobrescritos

        public override uint Insert(CategoriaConta objInsert)
        {
            objInsert.NumeroSequencia = ExecuteScalar<int>("Select Max(Coalesce(numSeq, 0)) + 1 From categoria_conta");

            return base.Insert(objInsert);
        }

        public override int Update(CategoriaConta objUpdate)
        {
            // Retira planos de contas de fornecedores que não sejam da categoria de débito
            string sql = @"
                Update fornecedor set idconta=null 
                Where idconta in (
                    Select idconta From plano_contas p 
                        Inner Join grupo_conta g On (p.IdGrupo=g.IdGrupo) 
                        Left Join categoria_conta c On (g.idcategoriaconta=c.idcategoriaconta) 
                    Where p.situacao=" + (int)PlanoContas.SituacaoEnum.Inativo + " or g.situacao=" + (int)Situacao.Inativo +
                    " or c.tipo not in(" + (int)TipoCategoriaConta.DespesaVariavel + ", " + (int)TipoCategoriaConta.DespesaFixa + @") or g.idCategoriaConta is null
                );";

            objPersistence.ExecuteCommand(sql);

            return base.Update(objUpdate);
        }

        public override int Delete(CategoriaConta objDelete)
        {
            return DeleteByPrimaryKey((uint)objDelete.IdCategoriaConta);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se esta categoria está sendo usada em algum grupo de conta
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From grupo_conta Where idCategoriaConta=" + Key) > 0)
                throw new Exception("Esta categoria não pode ser excluída por haver grupos de conta relacionadas à mesma.");

            return base.DeleteByPrimaryKey(Key);
        }

        #endregion
    }
}
