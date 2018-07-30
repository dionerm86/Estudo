using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class OperadoraCartaoDAO : BaseDAO<OperadoraCartao, OperadoraCartaoDAO>
    {
        #region Busca Padrão

        public string Sql(Situacao? situacao, bool selecionar)
        {
            var sql = "SELECT " + (selecionar ? "*" : "Count(*)") + " FROM operadora_cartao WHERE 1";

            if (situacao != null)
                sql += string.Format(" AND Situacao={0} ", (int)situacao);

            return sql;
        }

        public IList<OperadoraCartao> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(null, true), sortExpression, startRow, pageSize);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(null, false));
        }

        public IList<OperadoraCartao> PesquisarOperadoraCartaoPelaSituacao(Situacao situacao)
        {
            var sql = Sql(situacao, true);
            return objPersistence.LoadData(sql).ToList();
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
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    var oldObject = GetElementByPrimaryKey(transaction, objUpdate.IdOperadoraCartao);

                    var possuiCartoesAssociadosAtivos = objPersistence.ExecuteSqlQueryCount(string.Format(@"
                        SELECT COUNT(IDTIPOCARTAO) FROM tipo_cartao_credito WHERE OPERADORA={0} AND SITUACAO={1}", objUpdate.IdOperadoraCartao, (int)Situacao.Ativo)) > 0;

                    if (objUpdate.Situacao != oldObject.Situacao && objUpdate.Situacao == Situacao.Inativo && possuiCartoesAssociadosAtivos)
                        throw new Exception("A operadora de cartão não pode ser alterada pois está em uso em cartões ativos.");

                    var retorno = base.Update(transaction, objUpdate);
                    LogAlteracaoDAO.Instance.LogOperadoraCartao(transaction, oldObject, objUpdate);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("Alterar Tipo Cartão", ex);
                    throw ex;
                }
            }
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
