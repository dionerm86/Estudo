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
        public FotosImpostoServ[] GetByImpostoServ(uint idImpostoServ)
        {
            return GetByImpostoServ(null, idImpostoServ);
        }

        public FotosImpostoServ[] GetByImpostoServ(GDA.GDASession session, uint idImpostoServ)
        {
            string sql = "Select * From fotos_imposto_serv Where idImpostoServ=" + idImpostoServ;

            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }

        public override int Delete(FotosImpostoServ objDelete)
        {
            return Delete(null, objDelete);
        }

        public override int Delete(GDA.GDASession session, FotosImpostoServ objDelete)
        {
            string path = objDelete.FilePath;

            if (File.Exists(path))
                File.Delete(path);

            return base.Delete(session, objDelete);
        }

        /// <summary>
        /// Verifica se o imposto / serviço possui anexo.
        /// </summary>
        public bool PossuiAnexo(GDA.GDASession session, uint idImpostoServ)
        {
            string sql = "Select Count(*) From fotos_imposto_serv Where idImpostoServ=" + idImpostoServ;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        public int DeleteInstanceByPrimaryKey(uint Key)
        {
            return DeleteInstanceByPrimaryKey(null, Key);
        }

        public int DeleteInstanceByPrimaryKey(GDA.GDASession session, uint Key)
        {
            return Delete(session, GetElementByPrimaryKey(session, Key));
        }
    }
}
