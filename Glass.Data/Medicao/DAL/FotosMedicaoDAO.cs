using Glass.Data.Model;
using System.IO;

namespace Glass.Data.DAL
{
    public sealed class FotosMedicaoDAO : BaseDAO<FotosMedicao, FotosMedicaoDAO>
    {
        //private FotosMedicaoDAO() { }

        /// <summary>
        /// Retorna todas as fotos da medição passada
        /// </summary>
        /// <param name="idMedicao"></param>
        /// <returns></returns>
        public FotosMedicao[] GetByMedicao(string idsMedicao)
        {
            string sql = string.Format("Select * From fotos_medicao Where idMedicao in ({0})", idsMedicao);

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public override int Delete(FotosMedicao objDelete)
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
