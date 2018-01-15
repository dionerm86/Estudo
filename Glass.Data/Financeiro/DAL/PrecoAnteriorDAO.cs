using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class PrecoAnteriorDAO : BaseDAO<PrecoAnterior, PrecoAnteriorDAO>
    {
        //private PrecoAnteriorDAO() { }

        public PrecoAnterior GetElement(uint idProd)
        {
            string sql = "Select p.Descricao, pa.* From produto p " +
                "Left Join preco_anterior pa On (pa.idProd=pa.idProd) Where p.idProd=" + idProd;

            return objPersistence.LoadOneData(sql);
        }
    }
}
