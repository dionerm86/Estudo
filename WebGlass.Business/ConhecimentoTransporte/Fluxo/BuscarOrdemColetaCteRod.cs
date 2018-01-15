using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarOrdemColetaCteRod : BaseFluxo<BuscarOrdemColetaCteRod>
    {
        private BuscarOrdemColetaCteRod() { }

        /// <summary>
        /// Busca dados de ordem coleta do cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.OrdemColetaCteRod GetOrdemColetaCteRod(uint idCte)
        {
            using (Glass.Data.DAL.CTe.OrdemColetaCteRodDAO dao = Glass.Data.DAL.CTe.OrdemColetaCteRodDAO.Instance)
            {
                return new Entidade.OrdemColetaCteRod(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// Busca lista de ordens de coleta do cte
        /// </summary>
        public List<Entidade.OrdemColetaCteRod> GetOrdensColetaCteRod(uint idCte)
        {
            return GetOrdensColetaCteRod(null, idCte);
        }

        /// <summary>
        /// Busca lista de ordens de coleta do cte
        /// </summary>
        public List<Entidade.OrdemColetaCteRod> GetOrdensColetaCteRod(GDASession session, uint idCte)
        {
            var ordensColeta = new List<Entidade.OrdemColetaCteRod>();
            using (Glass.Data.DAL.CTe.OrdemColetaCteRodDAO dao = Glass.Data.DAL.CTe.OrdemColetaCteRodDAO.Instance)
            {
                foreach (var i in dao.GetOrdensColetaCte(session, idCte))
                    ordensColeta.Add(new Entidade.OrdemColetaCteRod(i));

                return ordensColeta;
            }
        }
    }
}
