using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarLacreCteRod : BaseFluxo<BuscarLacreCteRod>
    {
        private BuscarLacreCteRod() { }

        /// <summary>
        /// Busca lacre pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.LacreCteRod GetLacreCteRod(uint idCte)
        {
            using (Glass.Data.DAL.CTe.LacreCteRodDAO dao = Glass.Data.DAL.CTe.LacreCteRodDAO.Instance)
            {
                return new Entidade.LacreCteRod(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// Busca lista de lacres do cte
        /// </summary>
        public List<Entidade.LacreCteRod> GetLacresCteRod(uint idCte)
        {
            return GetLacresCteRod(null, idCte);
        }

        /// <summary>
        /// Busca lista de lacres do cte
        /// </summary>
        public List<Entidade.LacreCteRod> GetLacresCteRod(GDASession session, uint idCte)
        {
            var lacres = new List<Entidade.LacreCteRod>();
            using (Glass.Data.DAL.CTe.LacreCteRodDAO dao = Glass.Data.DAL.CTe.LacreCteRodDAO.Instance)
            {
                foreach (var i in dao.GetLacresByIdCte(session, idCte))
                    lacres.Add(new Entidade.LacreCteRod(i));

                return lacres;
            }
        }
    }
}
