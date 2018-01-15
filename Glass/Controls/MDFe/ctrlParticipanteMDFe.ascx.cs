using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlParticipanteMDFe : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdManifestoEletronico { get; set; }

        public List<Data.Model.ParticipanteMDFe> Participantes
        {
            get
            {
                int numSeq = 0;
                var participantes = new List<Data.Model.ParticipanteMDFe>();

                if (ctrlParticipanteEmitente.TemParticipanteSelecionado)
                {
                    participantes.Add(new Data.Model.ParticipanteMDFe
                    {
                        IdManifestoEletronico = IdManifestoEletronico,
                        NumSeq = numSeq++,
                        TipoParticipante = Data.Model.TipoParticipanteEnum.Emitente,
                        IdLoja = (int?)ctrlParticipanteEmitente.IdLoja,
                        IdFornecedor = (int?)ctrlParticipanteEmitente.IdFornec,
                        IdCliente = (int?)ctrlParticipanteEmitente.IdCliente,
                        IdTransportador = (int?)ctrlParticipanteEmitente.IdTransportador
                    });
                }

                if (ctrlParticipanteContratante.TemParticipanteSelecionado)
                {
                    participantes.Add(new Data.Model.ParticipanteMDFe
                    {
                        IdManifestoEletronico = IdManifestoEletronico,
                        NumSeq = numSeq++,
                        TipoParticipante = Data.Model.TipoParticipanteEnum.Contratante,
                        IdLoja = (int?)ctrlParticipanteContratante.IdLoja,
                        IdFornecedor = (int?)ctrlParticipanteContratante.IdFornec,
                        IdCliente = (int?)ctrlParticipanteContratante.IdCliente,
                        IdTransportador = (int?)ctrlParticipanteContratante.IdTransportador
                    });
                }

                return participantes;
            }
            set
            {
                foreach (var i in value.Where(f => f.IdCliente > 0 || f.IdFornecedor > 0 || f.IdLoja > 0 || f.IdTransportador > 0))
                {
                    //Verifica qual o tipo de participante para setar os valores pertinentes ao mesmo
                    switch (i.TipoParticipante)
                    {
                        case Glass.Data.Model.TipoParticipanteEnum.Emitente:
                            ctrlParticipanteEmitente.IdLoja = (uint?)i.IdLoja;
                            ctrlParticipanteEmitente.IdCliente = (uint?)i.IdCliente;
                            ctrlParticipanteEmitente.IdFornec = (uint?)i.IdFornecedor;
                            ctrlParticipanteEmitente.IdTransportador = (uint?)i.IdTransportador;
                            break;

                        case Glass.Data.Model.TipoParticipanteEnum.Contratante:
                            ctrlParticipanteContratante.IdLoja = (uint?)i.IdLoja;
                            ctrlParticipanteContratante.IdCliente = (uint?)i.IdCliente;
                            ctrlParticipanteContratante.IdFornec = (uint?)i.IdFornecedor;
                            ctrlParticipanteContratante.IdTransportador = (uint?)i.IdTransportador;
                            break;
                    }
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}