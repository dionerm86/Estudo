using Glass.Data.Model;
using System.IO;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class FotosTrocaDevolucaoDAO : BaseDAO<FotosTrocaDevolucao, FotosTrocaDevolucaoDAO>
    {
        //private FotosTrocaDevolucaoDAO() { }

        /// <summary>
        /// Retorna todas as fotos da troca / devolução passada
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public FotosTrocaDevolucao[] GetByTrocaDevolucao(uint idTrocaDevolucao)
        {
            string sql = "Select * From fotos_troca_devolucao Where idTrocaDevolucao=" + idTrocaDevolucao;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosTrocaDevolucao objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return GDAOperations.Delete(objDelete);
        }

        /// <summary>
        /// Verifica se a troca / devolução possui anexo.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiAnexo(uint idTrocaDevolucao)
        {
            string sql = "Select Count(*) From fotos_troca_devolucao Where idTrocaDevolucao=" + idTrocaDevolucao;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
