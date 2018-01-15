using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarSeguroCte : BaseFluxo<BuscarSeguroCte>
    {
        private BuscarSeguroCte() { }

        /// <summary>
        /// Buscar seguro do cte
        /// </summary>
        public Entidade.SeguroCte GetSeguroCte(uint idCte)
        {
            return GetSeguroCte(null, idCte);
        }

        /// <summary>
        /// Buscar seguro do cte
        /// </summary>
        public Entidade.SeguroCte GetSeguroCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.SeguroCteDAO dao = Glass.Data.DAL.CTe.SeguroCteDAO.Instance)
            {
                var seguroCte = new Entidade.SeguroCte(dao.GetElement(session, idCte));
                seguroCte.ObjSeguradora = BuscarSeguradora.Instance.GetSeguradora(session, seguroCte.IdSeguradora);
                return seguroCte;
            }
        }

        /// <summary>
        /// Buscar lista de seguros cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public List<Entidade.SeguroCte> GetSegurosCte(uint idCte)
        {
            var seguros = new List<Entidade.SeguroCte>();
            using (Glass.Data.DAL.CTe.SeguroCteDAO dao = Glass.Data.DAL.CTe.SeguroCteDAO.Instance)
            {
                foreach (var i in dao.GetSegurosByIdCte(idCte))
                    seguros.Add(new Entidade.SeguroCte(i));

                return seguros;
            }
        }
    }
}
