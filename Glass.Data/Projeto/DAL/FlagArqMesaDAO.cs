using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using GDA;

namespace Glass.Data.DAL
{
    public class FlagArqMesaDAO : BaseDAO<FlagArqMesa, FlagArqMesaDAO>
    {
        #region Busca padrão

        public IList<FlagArqMesa> GetList(string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression("SELECT * FROM flag_arq_mesa", sort, startRow, pageSize);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM flag_arq_mesa");
        }

        public IList<string> ObtemDescicao(int[] idsFlagArqMesa)
        {
            return ExecuteMultipleScalar<string>(string.Format("SELECT descricao from flag_arq_mesa where idFlagArqMesa IN ({0})", string.Join(",", idsFlagArqMesa.Select(f => f.ToString()).ToArray())));
        }

        #endregion

        public List<FlagArqMesa> ObtemPorPecaProjMod(int idPecaProjMod, bool buscarPadrao)
        {
            return ObtemPorPecaProjMod(null, idPecaProjMod, buscarPadrao);
        }

        public List<FlagArqMesa> ObtemPorPecaProjMod(GDASession session, int idPecaProjMod, bool buscarPadrao)
        {
            var sql = @"
            SELECT fam.*
            FROM flag_arq_mesa fam
            WHERE IdFlagArqMesa IN
                (
                    SELECT IdFlagArqMesa
                    FROM flag_arq_mesa_peca_projeto_modelo
                    WHERE IdPecaProjMod = " + idPecaProjMod + @"
                )";

            if (buscarPadrao)
                sql += @"
                    OR IdFlagArqMesa IN
                        (
                            SELECT IdFlagArqMesa
                            FROM flag_arq_mesa
                            WHERE Padrao = 1
                        )";

            return objPersistence.LoadData(session, sql).ToList();
        }

        public int? FindByDescricao(int idFlagArqMesa, string descricao)
        {
            string trataDescr = @"
                Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(descricao, ' ', ''), 
                '.', ''), 'ã', 'a'), 'á', 'a'), 'â', 'a'), 'é', 'e'), 'ê', 'e'), 'í', 'i'), 'ç', 'c')";

            GDAParameter p = new GDAParameter("?descricao", FlagArqMesaPecaProjMod.TrataDescricao(descricao));
            string sql = "select count(*) from flag_arq_mesa where idFlagArqMesa=" + idFlagArqMesa + " and " + trataDescr + "=?descricao";
            if (objPersistence.ExecuteSqlQueryCount(sql, p) > 0)
                return idFlagArqMesa;

            sql = "select {0} from flag_arq_mesa where " + trataDescr + "=?descricao";
            if (objPersistence.ExecuteSqlQueryCount(string.Format(sql, "count(*)"), p) > 0)
                return ExecuteScalar<int?>(string.Format(sql, "idFlagArqMesa"), p);

            return null;
        }

        /// <summary>
        /// Busca as flags pelo produto
        /// </summary>
        /// <param name="idProduto"></param>
        /// <param name="buscarPadrao"></param>
        /// <returns></returns>
        public List<FlagArqMesa> ObtemPorProduto(int idProduto, bool buscarPadrao)
        {
            var sql = @"
            SELECT fam.*
            FROM flag_arq_mesa fam
            WHERE IdFlagArqMesa IN
                (
                    SELECT IdFlagArqMesa
                    FROM flag_arq_mesa_produto
                    WHERE idProduto = " + idProduto + @"
                )";

            if (buscarPadrao)
                sql += @"
                    OR IdFlagArqMesa IN
                        (
                            SELECT IdFlagArqMesa
                            FROM flag_arq_mesa
                            WHERE Padrao = 1
                        )";

            return objPersistence.LoadData(sql);
        }

    }
}
