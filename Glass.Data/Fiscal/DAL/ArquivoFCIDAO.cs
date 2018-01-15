using System;
using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ArquivoFCIDAO: BaseDAO<ArquivoFCI, ArquivoFCIDAO>
    {
        private string Sql(uint idArquivoFCI, bool selecionar)
        {
            string campos = selecionar ? @"afci.*, f.Nome as NomeUsuCad, f1.nome as NomeUsuImportacao" : "Count(*)";

            string sql = @"
                SELECT " + campos + @"
                FROM arquivo_fci afci 
                    LEFT JOIN funcionario f ON (afci.usuCad = f.idFunc) 
                    LEFT JOIN funcionario f1 ON (afci.usuImportacao = f1.idFunc) 
                Where 1";

            if (idArquivoFCI > 0)
                sql += " AND afci.idArquivoFCI=" + idArquivoFCI;

            return sql;
        }

        /// <summary>
        /// Retorna a lista de arquivos FCI gerados pelo sistema
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<ArquivoFCI> ObterLista(string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "afci.idArquivoFci desc";
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Retorna o numero de arquivos FCI gerados pelo sistema
        /// </summary>
        /// <returns></returns>
        public int ObterListaCount()
        {
            return GetCountWithInfoPaging(Sql(0, true), false);
        }

        /// <summary>
        /// Busca um arquivo da fci pelo id
        /// </summary>
        /// <param name="IdArquivoFCI"></param>
        /// <returns></returns>
        public ArquivoFCI GetElement(uint IdArquivoFCI)
        {
            return objPersistence.LoadOneData(Sql(IdArquivoFCI, true));
        }
    }
}
