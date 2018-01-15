using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PaisDAO : BaseDAO<Pais, PaisDAO>
    {
        //private PaisDAO() { }

        /// <summary>
        /// Obtém a listagem de países do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Pais> GetOrdered()
        {
            string sql = "Select * From pais Order By Case nomePais When 'BRASIL' then 1 else 99 end, nomePais Asc";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna o id do pais pelo seu nome
        /// </summary>
        /// <param name="nomePais"></param>
        /// <returns></returns>
        public uint GetPaisByNome(string nomePais)
        {
            string sql = "Select idPais From pais p Where p.NomePais Like ?pais limit 1";

            object obj = objPersistence.ExecuteScalar(sql, new GDAParameter("?pais", nomePais));

            if (obj == null || obj.ToString() == String.Empty)
                return 0;

            return Glass.Conversoes.StrParaUint(obj.ToString());
        }
    }
}
