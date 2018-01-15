using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarValePedagioCteRod : BaseFluxo<BuscarValePedagioCteRod>
    {
        private BuscarValePedagioCteRod() { }

        /// <summary>
        /// Buscar vale pedágio pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.ValePedagioCteRod GetValePedagioCteRod(uint idCte)
        {
            using (Glass.Data.DAL.CTe.ValePedagioCteRodDAO dao = Glass.Data.DAL.CTe.ValePedagioCteRodDAO.Instance)
            {
                return new Entidade.ValePedagioCteRod(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// Buscar lista de vales pedágios pelo idCte
        /// </summary>
        public List<Entidade.ValePedagioCteRod> GetValesPedagioCteRod(uint idCte)
        {
            return GetValesPedagioCteRod(null, idCte);
        }

        /// <summary>
        /// Buscar lista de vales pedágios pelo idCte
        /// </summary>
        public List<Entidade.ValePedagioCteRod> GetValesPedagioCteRod(GDASession session, uint idCte)
        {
            var valesPedagio = new List<Entidade.ValePedagioCteRod>();
            using (Glass.Data.DAL.CTe.ValePedagioCteRodDAO dao = Glass.Data.DAL.CTe.ValePedagioCteRodDAO.Instance)
            {
                foreach (var i in dao.GetValesPedagioCte(session, idCte))
                    valesPedagio.Add(new Entidade.ValePedagioCteRod(i));

                return valesPedagio;
            }
        }
    }
}
