using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public sealed class BuscarProtocoloCte : BaseFluxo<BuscarProtocoloCte>
    {
        private BuscarProtocoloCte() { }

        /// <summary>
        /// Busca protocolo pelo idCte e pelo tipo de protocolo
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="tipoProtocolo"></param>
        /// <returns></returns>
        public Entidade.ProtocoloCte GetProtocoloCte(uint idCte, int tipoProtocolo)
        {
            using (Glass.Data.DAL.CTe.ProtocoloCteDAO dao = Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance)
            {
                return new Entidade.ProtocoloCte(dao.GetElement(idCte, tipoProtocolo));
            }
        }

        /// <summary>
        /// Busca protocolo pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.ProtocoloCte GetProtocoloCte(uint idCte)
        {
            using (Glass.Data.DAL.CTe.ProtocoloCteDAO dao = Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance)
            {
                return new Entidade.ProtocoloCte(dao.GetProtocolosByIdCte(idCte).First());
            }
        }

        /// <summary>
        /// Busca lista de protocolos pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public List<Entidade.ProtocoloCte> GetProtocolosCte(uint idCte)
        {
            var protocolos = new List<Entidade.ProtocoloCte>();
            using (Glass.Data.DAL.CTe.ProtocoloCteDAO dao = Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance)
            {
                foreach (var i in dao.GetProtocolosByIdCte(idCte))
                    protocolos.Add(new Entidade.ProtocoloCte(i));

                return protocolos;
            }
        }
    }
}
