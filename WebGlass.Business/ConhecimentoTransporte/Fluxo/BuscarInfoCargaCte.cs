using System.Collections.Generic;
using System.Linq;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarInfoCargaCte : BaseFluxo<BuscarInfoCargaCte>
    {
        private BuscarInfoCargaCte() { }

        /// <summary>
        /// Busca informações sobre a carga pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.InfoCargaCte GetInfoCargaCte(uint idCte)
        {
            using (Glass.Data.DAL.CTe.InfoCargaCteDAO dao = Glass.Data.DAL.CTe.InfoCargaCteDAO.Instance)
            {
                return new Entidade.InfoCargaCte(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// Busc lista de informações sobre as cargas do cte
        /// </summary>
        public List<Entidade.InfoCargaCte> GetList(uint idCte)
        {
            return GetList(null, idCte);
        }

        /// <summary>
        /// Busc lista de informações sobre as cargas do cte
        /// </summary>
        public List<Entidade.InfoCargaCte> GetList(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.InfoCargaCteDAO dao = Glass.Data.DAL.CTe.InfoCargaCteDAO.Instance)
            {
                var listaRetorno = new List<Entidade.InfoCargaCte>();
                foreach (var i in dao.GetList(session, idCte).ToList())
                {
                    listaRetorno.Add(new Entidade.InfoCargaCte(i));
                }
                return listaRetorno;
            }
        }
    }
}
