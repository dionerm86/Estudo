using Glass.Data.Model;
using System.IO;

namespace Glass.Data.DAL
{
    public class FotosFornecedorDAO : BaseDAO<FotosFornecedor, FotosFornecedorDAO>
    {
        //private FotosClienteDAO() { }

        /// <summary>
        /// Retorna todas as fotos da compra passada
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public FotosFornecedor[] GetByFornecedor(uint idFornecedor)
        {
            string sql = "Select * From fotos_fornecedor Where idFornecedor=" + idFornecedor;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosFornecedor objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return base.Delete(objDelete);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
