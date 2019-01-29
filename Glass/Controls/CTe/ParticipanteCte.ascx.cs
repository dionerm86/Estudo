using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using WebGlass.Business.ConhecimentoTransporte.Entidade;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class ParticipanteCte : CteBaseUserControl
    {
        #region Properties

        protected bool CteSaida
        {
            get { return TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida; }
        }

        protected bool CteEntradaTerceiros
        {
            get { return TipoDocumentoCte == Cte.TipoDocumentoCteEnum.EntradaTerceiros; }
        }

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte> ObjParticipanteCte
        {
            get
            {
                int numSeq = 0;
                var participantes = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte>();

                if (ctrlParticipanteEmitente.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = (uint?)ctrlParticipanteEmitente.IdCliente,
                        IdFornec = (uint?)ctrlParticipanteEmitente.IdFornec,
                        IdLoja = (uint?)ctrlParticipanteEmitente.IdLoja,
                        IdTransportador = (uint?)ctrlParticipanteEmitente.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Emitente,
                        Tomador = drpTipoTomador.SelectedValue == "emitente"
                    });
                }

                if (ctrlParticipanteDestinatario.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = (uint?)ctrlParticipanteDestinatario.IdCliente,
                        IdFornec = (uint?)ctrlParticipanteDestinatario.IdFornec,
                        IdLoja = (uint?)ctrlParticipanteDestinatario.IdLoja,
                        IdTransportador = (uint?)ctrlParticipanteDestinatario.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Destinatario,
                        Tomador = drpTipoTomador.SelectedValue == "destinatario"
                    });
                }

                if (ctrlParticipanteRemetente.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = (uint?)ctrlParticipanteRemetente.IdCliente,
                        IdFornec = (uint?)ctrlParticipanteRemetente.IdFornec,
                        IdLoja = (uint?)ctrlParticipanteRemetente.IdLoja,
                        IdTransportador = (uint?)ctrlParticipanteRemetente.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Remetente,
                        Tomador = drpTipoTomador.SelectedValue == "remetente"
                    });
                }

                if (ctrlParticipanteExpedidor.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = (uint?)ctrlParticipanteExpedidor.IdCliente,
                        IdFornec = (uint?)ctrlParticipanteExpedidor.IdFornec,
                        IdLoja = (uint?)ctrlParticipanteExpedidor.IdLoja,
                        IdTransportador = (uint?)ctrlParticipanteExpedidor.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Expedidor,
                        Tomador = drpTipoTomador.SelectedValue == "expedidor"
                    });
                }

                if (ctrlParticipanteRecebedor.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = (uint?)ctrlParticipanteRecebedor.IdCliente,
                        IdFornec = (uint?)ctrlParticipanteRecebedor.IdFornec,
                        IdLoja = (uint?)ctrlParticipanteRecebedor.IdLoja,
                        IdTransportador = (uint?)ctrlParticipanteRecebedor.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Recebedor,
                        Tomador = drpTipoTomador.SelectedValue == "recebedor"
                    });
                }

                return participantes;
            }
            set
            {
                foreach (var i in value.Where(f => f.IdCliente > 0 || f.IdFornec > 0 || f.IdLoja > 0 || f.IdTransportador > 0))
                {
                    //Verifica qual o tipo de participante para setar os valores pertinentes ao mesmo
                    switch (i.TipoParticipante)
                    {
                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Emitente:
                            ctrlParticipanteEmitente.IdLoja = (int?)i.IdLoja;
                            ctrlParticipanteEmitente.IdCliente = (int?)i.IdCliente;
                            ctrlParticipanteEmitente.IdFornec = (int?)i.IdFornec;
                            ctrlParticipanteEmitente.IdTransportador = (int?)i.IdTransportador;
                            break;

                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Destinatario:
                            ctrlParticipanteDestinatario.IdLoja = (int?)i.IdLoja;
                            ctrlParticipanteDestinatario.IdCliente = (int?)i.IdCliente;
                            ctrlParticipanteDestinatario.IdFornec = (int?)i.IdFornec;
                            ctrlParticipanteDestinatario.IdTransportador = (int?)i.IdTransportador;
                            break;

                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Remetente:
                            ctrlParticipanteRemetente.IdLoja = (int?)i.IdLoja;
                            ctrlParticipanteRemetente.IdCliente = (int?)i.IdCliente;
                            ctrlParticipanteRemetente.IdFornec = (int?)i.IdFornec;
                            ctrlParticipanteRemetente.IdTransportador = (int?)i.IdTransportador;
                            break;

                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Expedidor:
                            ctrlParticipanteExpedidor.IdLoja = (int?)i.IdLoja;
                            ctrlParticipanteExpedidor.IdCliente = (int?)i.IdCliente;
                            ctrlParticipanteExpedidor.IdFornec = (int?)i.IdFornec;
                            ctrlParticipanteExpedidor.IdTransportador = (int?)i.IdTransportador;
                            break;

                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Recebedor:
                            ctrlParticipanteRecebedor.IdLoja = (int?)i.IdLoja;
                            ctrlParticipanteRecebedor.IdCliente = (int?)i.IdCliente;
                            ctrlParticipanteRecebedor.IdFornec = (int?)i.IdFornec;
                            ctrlParticipanteRecebedor.IdTransportador = (int?)i.IdTransportador;
                            break;
                    }

                    // se participante for o tomador, verifica qual o tipo dele 
                    //(loja, fornecedor, transportador ou cliente) e seta os valores pertinentes ao tipo
                    if (i.Tomador)
                    {
                        drpTipoTomador.SelectedValue = i.TipoParticipante.ToString().ToLower();

                        if (i.IdLoja > 0)
                        {
                            ctrlParticipanteTomador.IdLoja = (int?)i.IdLoja;
                            hdfTomadorSelecionado.Value = i.IdLoja.ToString();
                        }
                        else if (i.IdCliente > 0)
                        {
                            ctrlParticipanteTomador.IdCliente = (int?)i.IdCliente;
                            hdfTomadorSelecionado.Value = i.IdCliente.ToString();
                        }
                        else if (i.IdFornec > 0)
                        {
                            ctrlParticipanteTomador.IdFornec = (int?)i.IdFornec;
                            hdfTomadorSelecionado.Value = i.IdFornec.ToString();
                        }
                        else if (i.IdTransportador > 0)
                        {
                            ctrlParticipanteTomador.IdTransportador = (int?)i.IdTransportador;
                            hdfTomadorSelecionado.Value = i.IdTransportador.ToString();
                        }
                    }
                }
            }
        }

        #endregion

        public ParticipanteCte()
        {
            this.AlterouTipoDocumentoCte += new EventHandler(ParticipanteCte_AlterouTipoDocumentoCte);
        }

        private void ParticipanteCte_AlterouTipoDocumentoCte(object sender, EventArgs e)
        {
            ctrlParticipanteTomador_Load(null, EventArgs.Empty);
            ctrlParticipanteEmitente_Load(null, EventArgs.Empty);
            ctrlParticipanteDestinatario_Load(null, EventArgs.Empty);
        }

        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get
            {
                return new BaseValidator[] { ctrlParticipanteTomador.Validador, cvDrpTomador,
                    ctrlParticipanteDestinatario.Validador};
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var errorMessage = "{0} - Campo obrigatório";
            ctrlParticipanteTomador.Validador.ErrorMessage = String.Format(errorMessage, "Tomador");
            ctrlParticipanteEmitente.Validador.ErrorMessage = String.Format(errorMessage, "Emitente");
            ctrlParticipanteRemetente.Validador.ErrorMessage = String.Format(errorMessage, "Remetente");
            ctrlParticipanteDestinatario.Validador.ErrorMessage = String.Format(errorMessage, "Destinatário");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            drpTipoTomador.Items[1].Enabled = !CteSaida;
            drpTipoTomador.Items[5].Enabled = !CteEntradaTerceiros;

            drpTipoTomador.Attributes.Add("onclick", string.Format("VerificaSelecaoTomador('{0}');",
                ctrlParticipanteTomador.ClientID));

            drpTipoTomador.Attributes.Add("onblur", string.Format("CarregaTomador('{0}','{1}','{2}','{3}','{4}','{5}', {6}, {7});",
                ctrlParticipanteTomador.ClientID, ctrlParticipanteEmitente.ClientID, ctrlParticipanteRemetente.ClientID,
                ctrlParticipanteDestinatario.ClientID, ctrlParticipanteExpedidor.ClientID, ctrlParticipanteRecebedor.ClientID,
                CteSaida.ToString().ToLower(), CteEntradaTerceiros.ToString().ToLower()));

            imgEditarTomador.Attributes.Add("onclick", string.Format("EditarTomador('{0}','{1}','{2}','{3}','{4}','{5}', {6}, {7});",
                ctrlParticipanteTomador.ClientID, ctrlParticipanteEmitente.ClientID, ctrlParticipanteRemetente.ClientID,
                ctrlParticipanteDestinatario.ClientID, ctrlParticipanteExpedidor.ClientID, ctrlParticipanteRecebedor.ClientID,
                CteSaida.ToString().ToLower(), CteEntradaTerceiros.ToString().ToLower()));

            ((DropDownList)ctrlParticipanteEmitente.FindControl("drpPart")).Attributes.Add("onclick", string.Format("VerificaSelecaoTomador('{0}');",
                ctrlParticipanteTomador.ClientID));
            //((ImageButton)ctrlParticipanteEmitente.FindControl("imgPart")).Attributes.Add("style", "visibility:hidden;");

            if (!ctrlParticipanteEmitente.TemParticipanteSelecionado)
                ((DropDownList)ctrlParticipanteEmitente.FindControl("drpPart")).Attributes.Add("disabled", "true");

            ((DropDownList)ctrlParticipanteRemetente.FindControl("drpPart")).Attributes.Add("onclick", string.Format("VerificaSelecaoTomador('{0}');",
                ctrlParticipanteTomador.ClientID));
            //((ImageButton)ctrlParticipanteRemetente.FindControl("imgPart")).Attributes.Add("style", "visibility:hidden;");

            if (!ctrlParticipanteRemetente.TemParticipanteSelecionado)
                ((DropDownList)ctrlParticipanteRemetente.FindControl("drpPart")).Attributes.Add("disabled", "true");

            ((DropDownList)ctrlParticipanteDestinatario.FindControl("drpPart")).Attributes.Add("onclick", string.Format("VerificaSelecaoTomador('{0}');",
            ctrlParticipanteTomador.ClientID));
            //((ImageButton)ctrlParticipanteDestinatario.FindControl("imgPart")).Attributes.Add("style", "visibility:hidden;");

            //if (!ctrlParticipanteDestinatario.TemParticipanteSelecionado)
            //    ((DropDownList)ctrlParticipanteDestinatario.FindControl("drpPart")).Attributes.Add("disabled", "true");

            ((DropDownList)ctrlParticipanteRecebedor.FindControl("drpPart")).Attributes.Add("onclick", string.Format("VerificaSelecaoTomador('{0}');",
                ctrlParticipanteTomador.ClientID));
            //((ImageButton)ctrlParticipanteRecebedor.FindControl("imgPart")).Attributes.Add("style", "visibility:hidden;");

            if (!ctrlParticipanteRecebedor.TemParticipanteSelecionado)
                ((DropDownList)ctrlParticipanteRecebedor.FindControl("drpPart")).Attributes.Add("disabled", "true");

            ((DropDownList)ctrlParticipanteExpedidor.FindControl("drpPart")).Attributes.Add("onclick", string.Format("VerificaSelecaoTomador('{0}');",
                ctrlParticipanteTomador.ClientID));
            //((ImageButton)ctrlParticipanteExpedidor.FindControl("imgPart")).Attributes.Add("style", "visibility:hidden");

            if (!ctrlParticipanteExpedidor.TemParticipanteSelecionado)
                ((DropDownList)ctrlParticipanteExpedidor.FindControl("drpPart")).Attributes.Add("disabled", "true");
        }

        protected void ctrlParticipanteTomador_Load(object sender, EventArgs e)
        {
            var drop = (DropDownList)ctrlParticipanteTomador.FindControl("drpPart");

            if (drop.Items.Count == 0)
                drop.DataBind();

            drop.Items[0].Enabled = CteSaida;
        }

        protected void ctrlParticipanteEmitente_Load(object sender, EventArgs e)
        {
            var drop = (DropDownList)ctrlParticipanteEmitente.FindControl("drpPart");

            if (CteSaida)
            {
                drop.SelectedValue = "3";
                drop.Enabled = false;
            }
            else
                drop.Enabled = true;
        }

        protected void ctrlParticipanteDestinatario_Load(object sender, EventArgs e)
        {
            var drop = (DropDownList)ctrlParticipanteDestinatario.FindControl("drpPart");

            if (CteEntradaTerceiros && ObjParticipanteCte.Count(f =>
                f.TipoParticipante == Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Destinatario) == 0)
                drop.SelectedValue = "3";
        }
    }
}
