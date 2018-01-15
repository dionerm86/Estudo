using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarProtocoloCte : BaseFluxo<CadastrarProtocoloCte>
    {
        private CadastrarProtocoloCte() { }
        
        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="protocoloCte"></param>
        /// <returns></returns>
        public uint Insert(Entidade.ProtocoloCte protocoloCte)
        {
            return Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance.Insert(Convert(protocoloCte));
        }

        /// <summary>
        /// insere lista de protocolos
        /// </summary>
        /// <param name="listaProtocolosCte"></param>
        /// <returns></returns>
        public List<uint> Insert(List<Entidade.ProtocoloCte> listaProtocolosCte)
        {
            var lista = new List<uint>();
            foreach (var i in listaProtocolosCte)
                lista.Add(Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance.Insert(Convert(i)));

            return lista;
        }

        /// <summary>
        /// atualiza lista de protocolos
        /// </summary>
        /// <param name="listaProtocolosCte"></param>
        /// <returns></returns>
        public List<uint> Update(List<Entidade.ProtocoloCte> listaProtocolosCte)
        {
            List<uint> lista = new List<uint>();
            foreach (var i in listaProtocolosCte)
            {
                Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance.Delete(i.IdCte, i.TipoProtocolo);
                lista.Add(Insert(i));
            }

            return lista;
        }

        /// <summary>
        /// atualiza protocolo
        /// </summary>
        /// <param name="protocolosCte"></param>
        /// <returns></returns>
        public int Update(Entidade.ProtocoloCte protocolosCte)
        {
            return Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance.Update(Convert(protocolosCte));
        }

        /// <summary>
        /// atualiza dados 
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="numProtocolo"></param>
        public void Update(uint idCte, string numProtocolo)
        {
            Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance.Update(idCte, numProtocolo);
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="protocoloCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ProtocoloCte Convert(WebGlass.Business.ConhecimentoTransporte.Entidade.ProtocoloCte protocoloCte)
        {
            return new Glass.Data.Model.Cte.ProtocoloCte
            {
                IdCte = protocoloCte.IdCte,
                TipoProtocolo = protocoloCte.TipoProtocolo,
                NumProtocolo = protocoloCte.NumProtocolo,
                DataCad = protocoloCte.DataCad
            };
        }
    }
}
