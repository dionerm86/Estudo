using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarVeiculoCte : BaseFluxo<BuscarVeiculoCte>
    {
        private BuscarVeiculoCte() { }

        /// <summary>
        /// Buscar veículo do cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.VeiculoCte GetVeiculoCte(uint idCte)
        {
            using (Glass.Data.DAL.CTe.VeiculoCteDAO dao = Glass.Data.DAL.CTe.VeiculoCteDAO.Instance)
            {
                return new Entidade.VeiculoCte(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// Buscar lista de veículos do cte
        /// </summary>
        public List<Entidade.VeiculoCte> GetVeiculosCte(uint idCte)
        {
            return GetVeiculosCte(null, idCte);
        }

        /// <summary>
        /// Buscar lista de veículos do cte
        /// </summary>
        public List<Entidade.VeiculoCte> GetVeiculosCte(GDASession session, uint idCte)
        {
            var veiculos = new List<Entidade.VeiculoCte>();
            using (Glass.Data.DAL.CTe.VeiculoCteDAO dao = Glass.Data.DAL.CTe.VeiculoCteDAO.Instance)
            {
                foreach (var i in dao.GetVeiculosCteByIdCte(session, idCte))
                    veiculos.Add(new Entidade.VeiculoCte(i));

                return veiculos;
            }
        }
    }
}
