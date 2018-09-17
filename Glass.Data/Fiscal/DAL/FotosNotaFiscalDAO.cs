using Glass.Data.Model;
using System.IO;

namespace Glass.Data.DAL
{
    public class FotosNotaFiscalDAO : BaseDAO<FotosNotaFiscal, FotosNotaFiscalDAO>
    {
        /// <summary>
        /// Retorna todas as fotos do pedido passado
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public FotosNotaFiscal[] GetByNotaFiscal(uint idNf)
        {
            string sql = "Select * From fotos_nota_fiscal Where idNf=" + idNf;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosNotaFiscal objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return base.Delete(objDelete);
        }

        /// <summary>
        /// Verifica se a nota fiscal possui anexo.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiAnexo(uint idNf)
        {
            string sql = "Select Count(*) From fotos_nota_fiscal where idNf=" + idNf;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
