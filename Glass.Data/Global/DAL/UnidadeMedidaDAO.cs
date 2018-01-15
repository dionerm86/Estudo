using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class UnidadeMedidaDAO : BaseCadastroDAO<UnidadeMedida, UnidadeMedidaDAO>
    {
        //private UnidadeMedidaDAO() { }

        #region Busca Padrão

        private string SqlList(string codigo, string descricao, bool selecionar)
        {
            string sql = @"Select " + (selecionar ? "u.*" : "Count(*)") + @" From unidade_medida u Where 1 ";

            if (!String.IsNullOrEmpty(codigo))
                sql += " And u.codigo Like ?codigo";

            if (!String.IsNullOrEmpty(descricao))
                sql += " And u.descricao Like ?descricao";

            return sql;
        }

        public IList<UnidadeMedida> GetSortedByCodInterno()
        {
            return objPersistence.LoadData(SqlList(null, null, true) + " order by Codigo asc").ToList();
        }

        public IList<UnidadeMedida> GetList(string codigo, string descricao, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(codigo, descricao) == 0)
            {
                List<UnidadeMedida> lst = new List<UnidadeMedida>();
                lst.Add(new UnidadeMedida());
                return lst.ToArray();
            }

            string filtro = String.IsNullOrEmpty(sortExpression) ? "Codigo" : sortExpression;

            return LoadDataWithSortExpression(SqlList(codigo, descricao, true), filtro, startRow, pageSize, GetParam(codigo, descricao));
        }

        public int GetCountReal(string codigo, string descricao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(codigo, descricao, false), GetParam(codigo, descricao));
        }

        public int GetCount(string codigo, string descricao)
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(codigo, descricao, false), GetParam(codigo, descricao));

            return count == 0 ? 1 : count;
        }

        private GDAParameter[] GetParam(string codigo, string descricao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codigo))
                lstParam.Add(new GDAParameter("?codigo", "%" + codigo + "%"));

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Obtém dados das unidades de medida

        public string ObtemCodigo(uint idUnidadeMedida)
        {
            return ObtemValorCampo<string>("codigo", "idUnidadeMedida=" + idUnidadeMedida);
        }

        #endregion

        #region Métodos Sobrescritos

        public override uint Insert(UnidadeMedida objInsert)
        {
            /*// Verifica se já existe uma unidade de medida cadastrada com o código passado.
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From unidade_medida Where codigo=?codigo",
                new GDAParameter("?codigo", objInsert.Codigo)) > 0)
                throw new Exception("Já existe uma unidade de medida cadastrada com este código.");

            if (objInsert.Codigo == objInsert.Descricao)
                throw new Exception("O código e a descrição da unidade de medida não podem ser os mesmos.");

            return base.Insert(objInsert);*/
            throw new NotSupportedException();
        }

        public override int Update(UnidadeMedida objUpdate)
        {
            /*// Verifica se já existe uma unidade de medida cadastrada com o código passado.
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From unidade_medida Where codigo=?codigo And idUnidadeMedida<>" +
                objUpdate.IdUnidadeMedida, new GDAParameter("?codigo", objUpdate.Codigo)) > 0)
                throw new Exception("Já existe uma unidade de medida cadastrada com este código.");

            if (objUpdate.Codigo == objUpdate.Descricao)
                throw new Exception("O código e a descrição da unidade de medida não podem ser os mesmos.");

            LogAlteracaoDAO.Instance.LogUnidadeMedida(objUpdate);
            return base.Update(objUpdate);*/
            throw new NotSupportedException();
        }

        public override int Delete(UnidadeMedida objDelete)
        {
            //return DeleteByPrimaryKey(objDelete.IdUnidadeMedida);
            throw new NotSupportedException();
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            /*// Verifica se esta unidade de medida está sendo usada em algum produto
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From produto Where idUnidadeMedida=" + Key + " Or idUnidadeMedidaTrib=" + Key) > 0)
                throw new Exception("Esta unidade de medida está sendo usada, portanto não pode ser excluída.");

            return GDAOperations.Delete(new UnidadeMedida { IdUnidadeMedida = Key });*/
            throw new NotSupportedException();
        }

        #endregion
    }
}
