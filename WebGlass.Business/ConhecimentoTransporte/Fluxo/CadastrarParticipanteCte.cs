using GDA;
using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarParticipanteCte : BaseFluxo<CadastrarParticipanteCte>
    {
        private CadastrarParticipanteCte() { }

        public List<uint> Insert(List<Entidade.ParticipanteCte> listaParticipantesCte)
        {
            return Insert(null, listaParticipantesCte);
        }

        public uint Insert(Entidade.ParticipanteCte participanteCte)
        {
            return Insert(null, participanteCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="participanteCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao,Entidade.ParticipanteCte participanteCte)
        {
            return Glass.Data.DAL.CTe.ParticipanteCteDAO.Instance.Insert(sessao, Convert(participanteCte));
        }

        /// <summary>
        /// insere lista de participantes
        /// </summary>
        /// <param name="listaParticipantesCte"></param>
        /// <returns></returns>
        public List<uint> Insert(GDASession sessao, List<Entidade.ParticipanteCte> listaParticipantesCte)
        {
            var lista = new List<Glass.Data.Model.Cte.ParticipanteCte>();
            foreach (var i in listaParticipantesCte)
                lista.Add(Convert(i));

            return Glass.Data.DAL.CTe.ParticipanteCteDAO.Instance.Insert(sessao, lista);
        }

        public List<uint> Update(List<Entidade.ParticipanteCte> listaParticipantesCte)
        {
            return Update(null, listaParticipantesCte);
        }

        /// <summary>
        /// atualiza lista de participantes
        /// </summary>
        /// <param name="listaParticipantesCte"></param>
        /// <returns></returns>
        public List<uint> Update(GDASession sessao, List<Entidade.ParticipanteCte> listaParticipantesCte)
        {
            using (Glass.Data.DAL.CTe.ParticipanteCteDAO dao = Glass.Data.DAL.CTe.ParticipanteCteDAO.Instance)
            {
                dao.Delete(sessao, listaParticipantesCte.Where(f => f.IdCte > 0).First().IdCte);
                var lista = new List<uint>();
                foreach (var i in listaParticipantesCte)
                    lista.Add(Insert(sessao, i));

                return lista;
            }
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="participanteCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ParticipanteCte Convert(WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte participanteCte)
        {
            return new Glass.Data.Model.Cte.ParticipanteCte
            {
                IdCliente = participanteCte.IdCliente,
                IdCte = participanteCte.IdCte,
                IdFornec = participanteCte.IdFornec,
                IdLoja = participanteCte.IdLoja,
                IdTransportador = participanteCte.IdTransportador,
                NumSeq = participanteCte.NumSeq,
                TipoParticipante = participanteCte.TipoParticipante,
                Tomador = participanteCte.Tomador
            };
        }
    }
}
