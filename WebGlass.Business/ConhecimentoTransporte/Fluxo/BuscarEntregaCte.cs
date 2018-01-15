using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarEntregaCte : BaseFluxo<BuscarEntregaCte>
    {
        private BuscarEntregaCte() { }

        /// <summary>
        /// Busca dados da entrega
        /// </summary>
        public Entidade.EntregaCte GetEntregaCte(uint idCte)
        {
            return GetEntregaCte(null, idCte);
        }

        /// <summary>
        /// Busca dados da entrega
        /// </summary>
        public Entidade.EntregaCte GetEntregaCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.EntregaCteDAO dao = Glass.Data.DAL.CTe.EntregaCteDAO.Instance)
            {
                return new Entidade.EntregaCte(dao.GetElement(session, idCte));
            }
        }
    }
}
