using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarParticipanteCte : BaseFluxo<BuscarParticipanteCte>
    {
        private BuscarParticipanteCte() { }

        /// <summary>
        /// Busca participante do cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public Entidade.ParticipanteCte GetParticipanteCte(uint idCte)
        {
            using (Glass.Data.DAL.CTe.ParticipanteCteDAO dao = Glass.Data.DAL.CTe.ParticipanteCteDAO.Instance)
            {
                return new Entidade.ParticipanteCte(dao.GetElement(idCte));
            }
        }

        /// <summary>
        /// Busca lista de participantes do cte
        /// </summary>
        public List<Entidade.ParticipanteCte> GetParticipantesCte(uint idCte)
        {
            return GetParticipantesCte(null, idCte);
        }

        /// <summary>
        /// Busca lista de participantes do cte
        /// </summary>
        public List<Entidade.ParticipanteCte> GetParticipantesCte(GDASession session, uint idCte)
        {
            var participantes = new List<Entidade.ParticipanteCte>();
            using (Glass.Data.DAL.CTe.ParticipanteCteDAO dao = Glass.Data.DAL.CTe.ParticipanteCteDAO.Instance)
            {
                foreach (var i in dao.GetParticipanteByIdCte(session, idCte))
                    participantes.Add(new Entidade.ParticipanteCte(i));

                return participantes;
            }
        }

        /// <summary>
        /// Busca participante do cte pelo tipo
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="tipoParticipante"></param>
        /// <returns></returns>
        public Entidade.ParticipanteCte GetParticipanteByIdCteTipo(uint idCte, int tipoParticipante)
        {
            using (Glass.Data.DAL.CTe.ParticipanteCteDAO dao = Glass.Data.DAL.CTe.ParticipanteCteDAO.Instance)
            {
                return new Entidade.ParticipanteCte(dao.GetParticipanteByIdCteTipo(idCte, tipoParticipante));
            }
        }
    }
}
