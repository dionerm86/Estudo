using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarEfdCte : BaseFluxo<BuscarEfdCte>
    {
        private BuscarEfdCte() { }

        /// <summary>
        /// Pesquisa dados do EFD pelo idCte
        /// </summary>
        public Entidade.EfdCte GetEfdCte(uint idCte)
        {
            return GetEfdCte(null, idCte);
        }

        /// <summary>
        /// Pesquisa dados do EFD pelo idCte
        /// </summary>
        public Entidade.EfdCte GetEfdCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.EfdCteDAO dao = Glass.Data.DAL.CTe.EfdCteDAO.Instance)
            {
                if (dao.Exists(session, idCte))
                    return new Entidade.EfdCte(dao.GetElementByPrimaryKey(session, idCte));
                else
                    return new Entidade.EfdCte();
            }
        }
    }
}
