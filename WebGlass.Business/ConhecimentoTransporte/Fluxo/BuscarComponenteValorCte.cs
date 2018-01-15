using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarComponenteValorCte : BaseFluxo<BuscarComponenteValorCte>
    {
        private BuscarComponenteValorCte() { }

        /// <summary>
        /// Busca componentes do valor da prestação pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.ComponenteValorCte GetComponenteValorCte(uint idCte)
        {
            using (Glass.Data.DAL.CTe.ComponenteValorCteDAO dao = Glass.Data.DAL.CTe.ComponenteValorCteDAO.Instance)
            {
                return new Entidade.ComponenteValorCte(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// BUsca lista de complementos de valor da prestação pelo idCte
        /// </summary>
        public List<Entidade.ComponenteValorCte> GetComponentesCte(uint idCte)
        {
            return GetComponentesCte(null, idCte);
        }

        /// <summary>
        /// BUsca lista de complementos de valor da prestação pelo idCte
        /// </summary>
        public List<Entidade.ComponenteValorCte> GetComponentesCte(GDASession session, uint idCte)
        {
            var componentes = new List<Entidade.ComponenteValorCte>();
            using (Glass.Data.DAL.CTe.ComponenteValorCteDAO dao = Glass.Data.DAL.CTe.ComponenteValorCteDAO.Instance)
            {
                foreach (var i in dao.GetComponentesByIdCte(session, idCte))
                    componentes.Add(new Entidade.ComponenteValorCte(i));

                return componentes;
            }
        }
    }
}
