using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarComplPassagemCte : BaseFluxo<BuscarComplPassagemCte>
    {
        private BuscarComplPassagemCte() { }

        /// <summary>
        /// Busca dados complementares do cte para fins operacionais ou comerciais pelo idCte
        /// </summary>
        public Entidade.ComplPassagemCte GetComplPassagemCte(uint idCte)
        {
            return GetComplPassagemCte(null, idCte);
        }

        /// <summary>
        /// Busca dados complementares do cte para fins operacionais ou comerciais pelo idCte
        /// </summary>
        public Entidade.ComplPassagemCte GetComplPassagemCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.ComplPassagemCteDAO dao = Glass.Data.DAL.CTe.ComplPassagemCteDAO.Instance)
            {
                return new Entidade.ComplPassagemCte(dao.GetElement(session, idCte));
            }
        }
    }
}