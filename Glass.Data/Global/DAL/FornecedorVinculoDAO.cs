using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using System.Linq;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class FornecedorVinculoDAO : BaseDAO<FornecedorVinculo, FornecedorVinculoDAO>
    {
        #region Criar vínculo de fornecedor

        /// <summary>
        /// Cria o vinculo
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="idFornecVinculo"></param>
        public void CriarVinculo(int idFornec, int idFornecVinculo)
        {
            var FornecVinc = new FornecedorVinculo()
            {
                IdFornec = idFornec,
                IdFornecVinculo = idFornecVinculo
            };

            var logFornec = new LogAlteracao()
            {
                Tabela = (int)LogAlteracao.TabelaAlteracao.Fornecedor,
                DataAlt = DateTime.Now,
                Campo = "Criado Vínculo com Fornecedor",
                IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                IdRegistroAlt = idFornec,
                ValorAtual = idFornecVinculo + " - " + FornecedorDAO.Instance.GetNome((uint)idFornecVinculo)
            };

            LogAlteracaoDAO.Instance.Insert(logFornec);

            Insert(FornecVinc);
        }

        #endregion

        #region Remover fornecedor vinculado

        /// <summary>
        /// Remover fornecedor vinculado
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="idFornecVinculo"></param>
        public void RemoverVinculo(int idFornec, int idFornecVinculo)
        {
            var fornecVinc = new FornecedorVinculo()
            {
                IdFornec = idFornec,
                IdFornecVinculo = idFornecVinculo
            };

            var logFornec = new LogAlteracao()
            {
                Tabela = (int)LogAlteracao.TabelaAlteracao.Fornecedor,
                DataAlt = DateTime.Now,
                Campo = "Removido Ví­nculo com Fornecedor",
                IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                IdRegistroAlt = idFornec,
                ValorAnterior = idFornecVinculo + " - " + FornecedorDAO.Instance.GetNome((uint)idFornecVinculo)
            };

            LogAlteracaoDAO.Instance.Insert(logFornec);

            Delete(fornecVinc);
        }

        #endregion

        #region Pesquisar vinculo

        /// <summary>
        /// Recupera a lista de ids de vinculos de um fornecedor.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public string GetIdsVinculados(GDASession session, int idFornec)
        {
            var sql = "SELECT IdFornecVinculo FROM fornecedor_vinculo where IdFornec = ?IdFornec";

            var ids = ExecuteMultipleScalar<int>(session, sql, new GDAParameter("?IdFornec", idFornec));

            return string.Join(",", ids.Select(f => f.ToString()));
        }

        /// <summary>
        /// Obtem o id e nome do fornecedor vinculado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public string ObterVinculo(GDASession session, int idFornec)
        {
            var nomeFornecBD = Glass.Configuracoes.FinanceiroConfig.FinanceiroPagto.ExibirRazaoSocialContasPagarPagas ? 
                "f.RazaoSocial, f.NomeFantasia" :
                "f.NomeFantasia, f.RazaoSocial";

            var sql = @"
                SELECT CONCAT(COALESCE(" + nomeFornecBD + @"), ';', f.IdFornec) 
                FROM fornecedor_vinculo fv
                    INNER JOIN fornecedor f ON (fv.IdFornec = f.IdFornec)
                WHERE IdFornecVinculo = ?IdFornec";

            var result = ExecuteScalar<string>(session, sql, new GDAParameter("?IdFornec", idFornec));

            return result;
        }

        #endregion
    }
}
