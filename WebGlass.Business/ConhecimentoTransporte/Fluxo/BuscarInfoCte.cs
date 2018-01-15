using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarInfoCte : BaseFluxo<BuscarInfoCte>
    {
        private BuscarInfoCte() { }

        /// <summary>
        /// Busca informações da quantidade da carga do cte
        /// </summary>
        public Entidade.InfoCte GetInfoCte(uint idCte)
        {
            return GetInfoCte(null, idCte);
        }

        /// <summary>
        /// Busca informações da quantidade da carga do cte
        /// </summary>
        public Entidade.InfoCte GetInfoCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.InfoCteDAO dao = Glass.Data.DAL.CTe.InfoCteDAO.Instance)
            {
                var infoCte = new Entidade.InfoCte(dao.GetElement(session, idCte));
                infoCte.ObjInfoCargaCte = BuscarInfoCargaCte.Instance.GetList(session, idCte);
                return infoCte;
            }
        }
    }
}
