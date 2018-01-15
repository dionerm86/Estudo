using System.Collections.Generic;
using System.Linq;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarCobrancaDuplCte : BaseFluxo<BuscarCobrancaDuplCte>
    {
        private BuscarCobrancaDuplCte() { }

        /// <summary>
        /// Busca dados de cobrança de duplicatas pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.CobrancaDuplCte GetCobrancaDuplCte(uint idCte)
        {
            using (Glass.Data.DAL.CTe.CobrancaDuplCteDAO dao = Glass.Data.DAL.CTe.CobrancaDuplCteDAO.Instance)
            {
                return new Entidade.CobrancaDuplCte(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// Busca lista de dobranças de duplicatas pelo idCte
        /// </summary>
        public List<Entidade.CobrancaDuplCte> GetList(uint idCte)
        {
            return GetList(null, idCte);
        }

        /// <summary>
        /// Busca lista de dobranças de duplicatas pelo idCte
        /// </summary>
        public List<Entidade.CobrancaDuplCte> GetList(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.CobrancaDuplCteDAO dao = Glass.Data.DAL.CTe.CobrancaDuplCteDAO.Instance)
            {
                var listaRetorno = new List<Entidade.CobrancaDuplCte>();
                foreach (var i in dao.GetList(session, idCte).ToList())
                {
                    listaRetorno.Add(new Entidade.CobrancaDuplCte(i));
                }
                return listaRetorno;
            }
        }
    }
}
