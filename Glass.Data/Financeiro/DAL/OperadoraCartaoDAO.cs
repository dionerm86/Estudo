using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class OperadoraCartaoDAO : BaseDAO<OperadoraCartao, OperadoraCartaoDAO>
    {
        #region Busca Padrão

        public string Sql(bool selecionar)
        {
            var sql = "SELECT " + (selecionar ? "*" : "Count(*)") + " FROM operadora_cartao WHERE 1";
            return sql;
        }

        public IList<OperadoraCartao> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(true), sortExpression, startRow, pageSize);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(false));
        }

        #endregion

        public bool OperadoraCartaoEmUso(uint idOperadoraCartao)
        {
            return objPersistence.ExecuteSqlQueryCount(string.Format(@"
                SELECT COUNT(IDTIPOCARTAO) FROM tipo_cartao_credito WHERE OPERADORA={0}", idOperadoraCartao)) > 0;
        }

        /// <summary>
        /// Retorna a descrição da operadora de cartão
        /// </summary>
        /// <param name="idOperadoraCartao"></param>
        /// <returns></returns>
        public string ObterDescricaoOperadora(uint idOperadoraCartao)
        {
            return ObtemValorCampo<string>("Descricao", "IdOperadoraCartao=" + idOperadoraCartao);
        }

        public uint ObterIdOperadoraPelaDescricao(string descricao)
        {
            return ObtemValorCampo<uint>("IdOperadoraCartao", "Descricao=?descricao", new GDA.GDAParameter("?descricao", descricao));
        }

        #region Métodos Sobrescritos

        public override int Update(OperadoraCartao objUpdate)
        {
            LogAlteracaoDAO.Instance.LogOperadoraCartao(GetElementByPrimaryKey(objUpdate.IdOperadoraCartao), objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(OperadoraCartao objDelete)
        {
            if (OperadoraCartaoEmUso(objDelete.IdOperadoraCartao))
                throw new Exception("A operadora de cartão não pode ser deletada pois está em uso!");

            return base.Delete(objDelete);
        }

        #endregion
    }
}
