using Glass.Data.Model;
using System.IO;

namespace Glass.Data.DAL
{
    public sealed class FotosLiberacaoDAO : BaseDAO<FotosLiberacao, FotosLiberacaoDAO>
    {
        //private FotosLiberacaoDAO() { }

        /// <summary>
        /// Retorna todas as fotos do pedido passado
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public FotosLiberacao[] GetByLiberacao(uint idLiberarPedido)
        {
            string sql = "Select * From fotos_liberacao Where idLiberarPedido=" + idLiberarPedido;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosLiberacao objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return base.Delete(objDelete);
        }

        /// <summary>
        /// Verifica se a liberacao possui anexo.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiAnexo(uint idLiberarPedido)
        {
            string sql = "Select Count(*) From fotos_liberacao where idLiberarPedido=" + idLiberarPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
