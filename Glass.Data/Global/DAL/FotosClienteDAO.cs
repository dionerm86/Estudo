using Glass.Data.Model;
using System.IO;

namespace Glass.Data.DAL
{
    public sealed class FotosClienteDAO : BaseDAO<FotosCliente, FotosClienteDAO>
    {
        //private FotosClienteDAO() { }

        /// <summary>
        /// Retorna todas as fotos da compra passada
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public FotosCliente[] GetByCliente(uint idCliente)
        {
            string sql = "Select * From fotos_cliente Where idCliente=" + idCliente;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosCliente objDelete)
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
