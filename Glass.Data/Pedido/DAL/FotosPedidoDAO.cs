using Glass.Data.Model;
using System.IO;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class FotosPedidoDAO : BaseDAO<FotosPedido, FotosPedidoDAO>
    {
        //private FotosPedidoDAO() { }

        /// <summary>
        /// Retorna todas as fotos do pedido passado
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public FotosPedido[] GetByPedido(uint idPedido)
        {
            string sql = "Select * From fotos_pedido Where idPedido=" + idPedido;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosPedido objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return GDAOperations.Delete(objDelete);
        }

        /// <summary>
        /// Verifica se o pedido possui anexo.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiAnexo(uint idPedido)
        {
            string sql = "Select Count(*) From fotos_pedido where idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
