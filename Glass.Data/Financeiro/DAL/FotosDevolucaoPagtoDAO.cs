using Glass.Data.Model;
using System.IO;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class FotosDevolucaoPagtoDAO : BaseDAO<FotosDevolucaoPagto, FotosDevolucaoPagtoDAO>
    {
        //private FotosDevolucaoPagtoDAO() { }

        /// <summary>
        /// Retorna todas as fotos da devolução de pagamento passada
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public FotosDevolucaoPagto[] GetByDevolucaoPagto(uint idDevolucaoPagto)
        {
            string sql = "Select * From fotos_devolucao_pagto Where idDevolucaoPagto=" + idDevolucaoPagto;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosDevolucaoPagto objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return GDAOperations.Delete(objDelete);
        }

        /// <summary>
        /// Verifica se a devolução de pagamento possui anexo.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiAnexo(uint idDevolucaoPagto)
        {
            string sql = "Select Count(*) From fotos_devolucao_pagto Where idDevolucaoPagto=" + idDevolucaoPagto;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return GDAOperations.Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
