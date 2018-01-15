using Glass.Data.Model;
using System.IO;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class FotosCompraDAO : BaseDAO<FotosCompra, FotosCompraDAO>
    {
        //private FotosCompraDAO() { }

        /// <summary>
        /// Retorna todas as fotos da compra passada
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public FotosCompra[] GetByCompra(uint idCompra)
        {
            var sql = "Select * From fotos_compra Where idCompra=" + idCompra;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosCompra objDelete)
        {
            var path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return base.Delete(objDelete);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return GDAOperations.Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
