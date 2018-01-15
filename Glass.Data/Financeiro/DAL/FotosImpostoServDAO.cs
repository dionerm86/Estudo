using Glass.Data.Model;
using System.IO;

namespace Glass.Data.DAL
{
    public sealed class FotosImpostoServDAO : BaseDAO<FotosImpostoServ, FotosImpostoServDAO>
    {
        //private FotosImpostoServDAO() { }

        /// <summary>
        /// Retorna todas as fotos do imposto / Serviço passado
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public FotosImpostoServ[]GetByImpostoServ(uint idImpostoServ)
        {
            string sql = "Select * From fotos_imposto_serv Where idImpostoServ=" + idImpostoServ;

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosImpostoServ objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return base.Delete(objDelete);
        }

        /// <summary>
        /// Verifica se o imposto / serviço possui anexo.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiAnexo(uint idImpostoServ)
        {
            string sql = "Select Count(*) From fotos_imposto_serv Where idImpostoServ=" + idImpostoServ;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        public int DeleteInstanceByPrimaryKey(uint Key)
        {
            return Delete(GetElementByPrimaryKey((uint)Key));
        }
    }
}
