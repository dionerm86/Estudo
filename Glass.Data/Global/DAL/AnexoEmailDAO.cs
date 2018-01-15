using Glass.Data.Model;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class AnexoEmailDAO : BaseDAO<AnexoEmail, AnexoEmailDAO>
    {
        //private AnexoEmailDAO() { }

        public AnexoEmail[] GetByEmail(uint idEmail)
        {
            string sql = "select * from anexo_email where idEmail=" + idEmail;
            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public void DeleteByEmail(uint idEmail)
        {
            objPersistence.ExecuteCommand("delete from anexo_email where idEmail=" + idEmail);
        }

        /// <summary>
        /// Apaga o boleto da pasta
        /// </summary>
        /// <param name="idsAnexos"></param>
        public void ApagarBoletosAnexos(int[] idsAnexos)
        {
            if (idsAnexos.Count() > 0 && idsAnexos.FirstOrDefault() > 0)
            {
                foreach (var idAnexo in idsAnexos)
                {
                    //Verifica se existe o anexo na pasta referente ao anexo email, se existir apaga.
                    if (System.IO.File.Exists(Armazenamento.ArmazenamentoIsolado.DiretorioBoletos + string.Format("\\anexo{0}.pdf", idAnexo)))
                        System.IO.File.Delete(Armazenamento.ArmazenamentoIsolado.DiretorioBoletos + string.Format("\\anexo{0}.pdf", idAnexo));
                }
            }
        }
    }
}
