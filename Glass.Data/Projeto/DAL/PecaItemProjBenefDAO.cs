using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PecaItemProjBenefDAO : BaseDAO<PecaItemProjBenef, PecaItemProjBenefDAO>
    {
        //private PecaItemProjBenefDAO() { }

        public PecaItemProjBenef[] GetByPecaItemProj(uint idPecaItemProj)
        {
            string sql = "select * from peca_item_proj_benef where idPecaItemProj=" + idPecaItemProj;
            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public void DeleteByPecaItemProj(GDASession sessao, uint idPecaItemProj)
        {
            string sql = "delete from peca_item_proj_benef where idPecaItemProj=" + idPecaItemProj;
            objPersistence.ExecuteCommand(sessao, sql);
        }

        public void DeleteByItemProjeto(GDASession sessao, uint idItemProjeto)
        {
            string sql = "delete from peca_item_proj_benef where idPecaItemProj in (select idPecaItemProj from peca_item_projeto where idItemProjeto=" + idItemProjeto + ")";
            objPersistence.ExecuteCommand(sessao, sql);
        }
    }
}
