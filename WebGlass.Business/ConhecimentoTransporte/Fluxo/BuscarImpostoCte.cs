using System.Collections.Generic;
using System.Linq;
using Glass.Data.EFD;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarImpostoCte : BaseFluxo<BuscarImpostoCte>
    {
        private BuscarImpostoCte() { }

        /// <summary>
        /// Buscar dados de icms do cte
        /// </summary>
        public List<Entidade.ImpostoCte> GetImpostosCte(uint idCte)
        {
            return GetImpostosCte(null, idCte);
        }

        /// <summary>
        /// Buscar dados de icms do cte
        /// </summary>
        public List<Entidade.ImpostoCte> GetImpostosCte(GDASession session, uint idCte)
        {
            using (Glass.Data.DAL.CTe.ImpostoCteDAO dao = Glass.Data.DAL.CTe.ImpostoCteDAO.Instance)
            {
                return dao.GetList(session, idCte).Select(x => new Entidade.ImpostoCte(x)).ToList();
            }
        }

        /// <summary>
        /// Buscar dados de icms do cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.ImpostoCte GetImpostoCte(uint idCte, DataSourcesEFD.TipoImpostoEnum tipoImposto)
        {
            using (Glass.Data.DAL.CTe.ImpostoCteDAO dao = Glass.Data.DAL.CTe.ImpostoCteDAO.Instance)
            {
                return new Entidade.ImpostoCte(dao.GetElement(idCte, tipoImposto));
            }
        }
    }
}
