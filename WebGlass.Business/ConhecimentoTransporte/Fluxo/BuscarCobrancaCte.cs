using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarCobrancaCte : BaseFluxo<BuscarCobrancaCte>
    {
        private BuscarCobrancaCte() { }

        /// <summary>
        /// Pesquisa Cobrança pelo idCte
        /// </summary>
        public Entidade.CobrancaCte GetCobrancaCte(uint idCte)
        {
            return GetCobrancaCte(null, idCte);
        }

        /// <summary>
        /// Pesquisa Cobrança pelo idCte
        /// </summary>
        public Entidade.CobrancaCte GetCobrancaCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.CobrancaCteDAO dao = Glass.Data.DAL.CTe.CobrancaCteDAO.Instance)
            {
                var cobrancaCte = new Entidade.CobrancaCte(dao.GetElement(session, idCte));
                
                cobrancaCte.ObjCobrancaDuplCte = BuscarCobrancaDuplCte.Instance.GetList(session, idCte);

                return cobrancaCte;
            }
        }
    }
}
