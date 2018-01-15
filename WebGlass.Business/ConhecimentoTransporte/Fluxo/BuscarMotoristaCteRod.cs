using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarMotoristaCteRod : BaseFluxo<BuscarMotoristaCteRod>
    {
        private BuscarMotoristaCteRod() { }

        /// <summary>
        /// Buscar motorista pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.MotoristaCteRod GetMotoristaCteRod(uint idCte)
        {
            using (Glass.Data.DAL.CTe.MotoristaCteRodDAO dao = Glass.Data.DAL.CTe.MotoristaCteRodDAO.Instance)
            {
                return new Entidade.MotoristaCteRod(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// Busca lista de motorista pelo idCte
        /// </summary>
        public List<Entidade.MotoristaCteRod> GetMotoristasCteRod(uint idCte)
        {
            return GetMotoristasCteRod(null, idCte);
        }

        /// <summary>
        /// Busca lista de motorista pelo idCte
        /// </summary>
        public List<Entidade.MotoristaCteRod> GetMotoristasCteRod(GDASession session, uint idCte)
        {
            var motoristas = new List<Entidade.MotoristaCteRod>();
            using (Glass.Data.DAL.CTe.MotoristaCteRodDAO dao = Glass.Data.DAL.CTe.MotoristaCteRodDAO.Instance)
            {
                foreach (var i in dao.GetMotoristasIdCte(session, idCte))
                    motoristas.Add(new Entidade.MotoristaCteRod(i));

                return motoristas;
            }
        }
    }
}
