using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public class FlagArqMesaArqCalcEngineDAO : BaseDAO<FlagArqMesaArqCalcEngine, FlagArqMesaArqCalcEngineDAO>
    {
        public IList<FlagArqMesaArqCalcEngine> ObtemPorArqCalcEngine(int idArquivoCalcEngine)
        {
            return objPersistence.LoadData("SELECT * FROM flag_arq_mesa_arq_calcengine where idArquivoCalcEngine=" + idArquivoCalcEngine).ToList();
        }

        public void DeletePorArqCalcEngine(int idArquivoCalcEngine)
        {
            objPersistence.ExecuteCommand("DELETE FROM flag_arq_mesa_arq_calcengine WHERE idArquivoCalcEngine=" + idArquivoCalcEngine);
        }

        public bool FlagArqMesaArqCalcEngineExiste (FlagArqMesaArqCalcEngine famace)
        {
            var tmp = objPersistence.LoadData("SELECT * FROM flag_arq_mesa_arq_calcengine where IdArquivoCalcEngine=" + famace.IdArquivoCalcEngine + " AND IdFlagArqMesa=" + famace.IdFlagArqMesa);
            return tmp.ToList().Count > 0;
        }

        public void InsereSeNaoExistir(FlagArqMesaArqCalcEngine famace)
        {
            if (!FlagArqMesaArqCalcEngineExiste(famace))
                Insert(famace);
        }
    }
}
