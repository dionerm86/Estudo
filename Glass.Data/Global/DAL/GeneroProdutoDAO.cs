using System;
using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class GeneroProdutoDAO : BaseDAO<GeneroProduto, GeneroProdutoDAO>
    {
        //private GeneroProdutoDAO() { }

        public IList<GeneroProduto> GetOrdered()
        {
            string sql = "Select * From genero_produto Order By Codigo";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna o código do gênero de produto passado
        /// </summary>
        /// <param name="idGeneroProduto"></param>
        /// <returns></returns>
        public string GetCodigo(uint? idGeneroProduto)
        {
            if (idGeneroProduto == null || idGeneroProduto == 0)
                return String.Empty;

            string sql = "Select Coalesce(codigo,'') From genero_produto Where idGeneroProduto=" + idGeneroProduto;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return String.Empty;

            return obj.ToString();
        }
    }
}
