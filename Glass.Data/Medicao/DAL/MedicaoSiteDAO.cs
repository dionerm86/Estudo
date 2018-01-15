using System;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class MedicaoSiteDAO : BaseDAO<MedicaoSite, MedicaoSiteDAO>
    {
        //private MedicaoSiteDAO() { }

        public override uint Insert(MedicaoSite objInsert)
        {
            objInsert.DataPedido = DateTime.Now;
            objInsert.Emitido = false;

            return base.Insert(objInsert);
        }

        public void SetAsEmitido(uint codMedicao)
        {
            string sql;

            if (!ObtemValorCampo<bool>("emitido", "codMedicao=" + codMedicao)) 
                sql = "Update medicao_site Set Emitido=1 Where codMedicao=" + codMedicao;
            else
                sql = "Update medicao_site Set Emitido=0 Where codMedicao=" + codMedicao;

            objPersistence.ExecuteCommand(sql);
        }
    }
}
