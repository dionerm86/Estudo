using Glass.Data.Model;
using System.IO;

namespace Glass.Data.DAL
{
    public sealed class FotosOrcamentoDAO : BaseDAO<FotosOrcamento, FotosOrcamentoDAO>
    {
        //private FotosOrcamentoDAO() { }

        /// <summary>
        /// Retorna todas as fotos do pedido passado
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public FotosOrcamento[] GetByOrcamento(uint idOrcamento)
        {
            string sql = "Select * From fotos_orcamento Where idOrcamento=" + idOrcamento;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosOrcamento objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return base.Delete(objDelete);
        }

        /// <summary>
        /// Verifica se o pedido possui anexo.
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        public bool PossuiAnexo(uint idOrcamento)
        {
            string sql = "Select Count(*) From fotos_orcamento where idOrcamento=" + idOrcamento;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
