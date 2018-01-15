using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ValidacaoPecaModeloDAO : BaseCadastroDAO<ValidacaoPecaModelo, ValidacaoPecaModeloDAO>
    {
        #region Busca padrão

        /// <summary>
        /// Sql para retornar as validações.
        /// </summary>
        public string Sql(int idValidacaoPecaModelo, int idPecaProjMod, bool selecionar)
        {
            var campos = selecionar ? "vpp.*" : "Count(*)";
            var filtro = string.Empty;

            var sql = @"
                SELECT " + campos + @"
                FROM validacao_peca_modelo vpp
                WHERE 1 {0}
                ORDER BY vpp.IdValidacaoPecaModelo ASC";

            if (idValidacaoPecaModelo > 0)
                filtro += " AND vpp.IdValidacaoPecaModelo=" + idValidacaoPecaModelo;

            if (idPecaProjMod > 0)
                filtro += " AND vpp.IdPecaProjMod=" + idPecaProjMod;

            return string.Format(sql, filtro);
        }

        /// <summary>
        /// Obtém validações de acordo com a sua chave primária.
        /// </summary>
        public ValidacaoPecaModelo ObtemValidacao(int idValidacaoPecaModelo)
        {
            return GetElementByPrimaryKey((uint)idValidacaoPecaModelo);
        }

        /// <summary>
        /// Obtém validações de acordo com a peça do projeto modelo.
        /// </summary>
        public List<ValidacaoPecaModelo> ObtemValidacoes(int idPecaProjMod)
        {
            return ObtemValidacoes(null, idPecaProjMod);
        }

        /// <summary>
        /// Obtém validações de acordo com a peça do projeto modelo.
        /// </summary>
        public List<ValidacaoPecaModelo> ObtemValidacoes(GDASession session, int idPecaProjMod)
        {
            /* Usado para exibir o footer da grid. */
            if (ObtemQuantidadeValidacoes(session, idPecaProjMod) == 0)
            {
                var listaVazia = new List<ValidacaoPecaModelo>();
                listaVazia.Add(new ValidacaoPecaModelo());
                return listaVazia;
            }

            return objPersistence.LoadData(session, Sql(0, idPecaProjMod, true));
        }

        /// <summary>
        /// Obtém a quantidade de validações.
        /// </summary>
        public int ObtemQuantidadeValidacoes(int idPecaProjMod)
        {
            return ObtemQuantidadeValidacoes(null, idPecaProjMod);
        }

        /// <summary>
        /// Obtém a quantidade de validações.
        /// </summary>
        public int ObtemQuantidadeValidacoes(GDASession session, int idPecaProjMod)
        {
            return objPersistence.ExecuteSqlQueryCount(session, Sql(0, idPecaProjMod, false));
        }

        #endregion

        #region Apaga as validações pela peça do projeto

        public void DeleteByPecaProjMod(GDASession session, int idPecaProjMod)
        {
            objPersistence.ExecuteCommand(session, "DELETE FROM flag_arq_mesa_peca_projeto_modelo WHERE IdPecaProjMod=" + idPecaProjMod);
        }

        #endregion

        #region Métodos sobescritos

        /// <summary>
        /// Atualiza os dados da validação da peça do projeto modelo.
        /// </summary>
        public override int Update(ValidacaoPecaModelo objUpdate)
        {
            LogAlteracaoDAO.Instance.LogValidacaoPecaModelo(objUpdate);
            return base.Update(objUpdate);
        }

        #endregion
    }
}
