using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarSeguradora : BaseFluxo<BuscarSeguradora>
    {
        private BuscarSeguradora() { }

        /// <summary>
        /// Buscar seguradora
        /// </summary>
        public Entidade.Seguradora GetSeguradora(uint idSeguradora)
        {
            return GetSeguradora(null, idSeguradora);
        }

        /// <summary>
        /// Buscar seguradora
        /// </summary>
        public Entidade.Seguradora GetSeguradora(GDASession session, uint idSeguradora)
        {
            using (Glass.Data.DAL.CTe.SeguradoraDAO dao = Glass.Data.DAL.CTe.SeguradoraDAO.Instance)
            {
                return new Entidade.Seguradora(dao.GetElement(session, idSeguradora));
            }
        }
    }
}
