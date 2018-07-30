using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class BandeiraCartaoDAO : BaseDAO<BandeiraCartao, BandeiraCartaoDAO>
    {
        #region Busca Padrão

        public string Sql(Situacao? situacao, bool selecionar)
        {
            var sql = "SELECT " + (selecionar ? "*" : "Count(*)") + " FROM bandeira_cartao WHERE 1";

            if (situacao != null)
                sql += string.Format(" AND Situacao={0} ", (int)situacao);

            return sql;
        }

        public IList<BandeiraCartao> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(null, true), sortExpression, startRow, pageSize);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(null, false));
        }

        public IList<BandeiraCartao> PesquisarBandeiraCartaoPelaSituacao(Situacao situacao)
        {
            var sql = Sql(situacao, true);
            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        public bool BandeiraCartaoEmUso(uint idBandeiraCartao)
        {
            return objPersistence.ExecuteSqlQueryCount(string.Format(@"
                SELECT COUNT(IDTIPOCARTAO) FROM tipo_cartao_credito WHERE BANDEIRA={0}", idBandeiraCartao)) > 0;
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

        #region Métodos Sobrescritos

        public override int Update(BandeiraCartao objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    var oldObject = GetElementByPrimaryKey(transaction, objUpdate.IdBandeiraCartao);

                    var posssuiCartoesAssociadosAtivos = objPersistence.ExecuteSqlQueryCount(transaction, string.Format(@"
                            SELECT COUNT(IDTIPOCARTAO) FROM tipo_cartao_credito WHERE BANDEIRA={0} AND SITUACAO={1}", objUpdate.IdBandeiraCartao, (int)Situacao.Ativo)) > 0;

                    if (oldObject.Situacao != objUpdate.Situacao && objUpdate.Situacao == Situacao.Inativo && posssuiCartoesAssociadosAtivos)
                        throw new Exception("A bandeira de cartão não pode ser Inativada pois está em uso em cartões ativos.");

                    var retorno = base.Update(transaction, objUpdate);
                    LogAlteracaoDAO.Instance.LogBandeiraCartao(transaction, oldObject, objUpdate);

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

        public override int Delete(BandeiraCartao objDelete)
        {
            if (BandeiraCartaoEmUso(objDelete.IdBandeiraCartao))
                throw new Exception("A bandeira de cartão não pode ser deletada pois está em uso!");

            return base.Delete(objDelete);
        }

        #endregion
    }
}
