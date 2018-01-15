using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;

namespace Glass.Data.DAL
{
    public class ParticipanteMDFeDAO : BaseDAO<ParticipanteMDFe, ParticipanteMDFeDAO>
    {
        public List<ParticipanteMDFe> ObterParticipanteMDFe(int idManifestoEletronico)
        {
            return objPersistence.LoadData(string.Format("SELECT * FROM participante_mdfe WHERE IdManifestoEletronico={0}", idManifestoEletronico)).ToList();
        }

        public int ObterNumSeqMax(GDASession sessao, int idManifestoEletronico)
        {
            return ObtemValorCampo<int>("MAX(NumSeq)", "IdManifestoEletronico=" + idManifestoEletronico);
        }

        public ParticipanteMDFe ObterParticipantePeloIdMDFeTipo(int idManifestoEletronico, TipoParticipanteEnum tipoParticipante)
        {
            return objPersistence.LoadOneData(string.Format("SELECT * FROM participante_mdfe WHERE IdManifestoEletronico={0} AND TipoParticipante={1}",
                idManifestoEletronico, (int)tipoParticipante));
        }

        public override uint Insert(GDASession session, ParticipanteMDFe objInsert)
        {
            objInsert.NumSeq = (ObterNumSeqMax(session, objInsert.IdManifestoEletronico) + 1);
            return base.Insert(session, objInsert);
        }

        public void DeletarPorIdManifestoEletronico(GDASession sessao, int idManifestoEletronico)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM participante_mdfe WHERE IdManifestoEletronico=" + idManifestoEletronico, null);
        }
    }
}
