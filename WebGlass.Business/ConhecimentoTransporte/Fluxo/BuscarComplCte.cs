using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarComplCte : BaseFluxo<BuscarComplCte>
    {
        private BuscarComplCte() { }

        /// <summary>
        /// Busca dados de complemento do cte pelo idCte
        /// </summary>
        public Entidade.ComplCte GetComplCte(uint idCte)
        {
            return GetComplCte(null, idCte);
        }

        /// <summary>
        /// Busca dados de complemento do cte pelo idCte
        /// </summary>
        public Entidade.ComplCte GetComplCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.ComplCteDAO dao = Glass.Data.DAL.CTe.ComplCteDAO.Instance)
            {
                var complCte = new Entidade.ComplCte(dao.GetElement(session, idCte));
                complCte.ObjComplPassagemCte = Fluxo.BuscarComplPassagemCte.Instance.GetComplPassagemCte(session, idCte);

                return complCte;
            }
        }
    }
}