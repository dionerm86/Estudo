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
                        IdCliente = ctrlParticipanteEmitente.IdCliente,
                        IdFornec = ctrlParticipanteEmitente.IdFornec,
                        IdLoja = ctrlParticipanteEmitente.IdLoja,
                        IdTransportador = ctrlParticipanteEmitente.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Emitente,
                        Tomador = drpTipoTomador.SelectedValue == "emitente"
                    });
                }

                if (ctrlParticipanteDestinatario.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = ctrlParticipanteDestinatario.IdCliente,
                        IdFornec = ctrlParticipanteDestinatario.IdFornec,
                        IdLoja = ctrlParticipanteDestinatario.IdLoja,
                        IdTransportador = ctrlParticipanteDestinatario.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Destinatario,
                        Tomador = drpTipoTomador.SelectedValue == "destinatario"
                    });
                }

                if (ctrlParticipanteRemetente.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = ctrlParticipanteRemetente.IdCliente,
                        IdFornec = ctrlParticipanteRemetente.IdFornec,
                        IdLoja = ctrlParticipanteRemetente.IdLoja,
                        IdTransportador = ctrlParticipanteRemetente.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Remetente,
                        Tomador = drpTipoTomador.SelectedValue == "remetente"
                    });
                }

                if (ctrlParticipanteExpedidor.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = ctrlParticipanteExpedidor.IdCliente,
                        IdFornec = ctrlParticipanteExpedidor.IdFornec,
                        IdLoja = ctrlParticipanteExpedidor.IdLoja,
                        IdTransportador = ctrlParticipanteExpedidor.IdTransportador,
                        NumSeq = numSeq++,
                        TipoParticipante = Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Expedidor,
                        Tomador = drpTipoTomador.SelectedValue == "expedidor"
                    });
                }

                if (ctrlParticipanteRecebedor.TemParticipanteSelecionado)
                {
                    participantes.Add(new WebGlass.Business.ConhecimentoTransporte.Entidade.ParticipanteCte()
                    {
                        IdCliente = ctrlParticipanteRecebedor.IdCliente,
                        IdFornec = ctrlParticipanteRecebedor.IdFornec,
                        IdLoja = ctrlParticipanteRecebedor.IdLoja,
                        IdTransportador = ctrlParticipanteRecebedor.IdTransportador,
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
                            ctrlParticipanteEmitente.IdLoja = i.IdLoja;
                            ctrlParticipanteEmitente.IdCliente = i.IdCliente;
                            ctrlParticipanteEmitente.IdFornec = i.IdFornec;
                            ctrlParticipanteEmitente.IdTransportador = i.IdTransportador;
                            break;

                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Destinatario:
                            ctrlParticipanteDestinatario.IdLoja = i.IdLoja;
                            ctrlParticipanteDestinatario.IdCliente = i.IdCliente;
                            ctrlParticipanteDestinatario.IdFornec = i.IdFornec;
                            ctrlParticipanteDestinatario.IdTransportador = i.IdTransportador;
                            break;

                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Remetente:
                            ctrlParticipanteRemetente.IdLoja = i.IdLoja;
                            ctrlParticipanteRemetente.IdCliente = i.IdCliente;
                            ctrlParticipanteRemetente.IdFornec = i.IdFornec;
                            ctrlParticipanteRemetente.IdTransportador = i.IdTransportador;
                            break;

                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Expedidor:
                            ctrlParticipanteExpedidor.IdLoja = i.IdLoja;
                            ctrlParticipanteExpedidor.IdCliente = i.IdCliente;
                            ctrlParticipanteExpedidor.IdFornec = i.IdFornec;
                            ctrlParticipanteExpedidor.IdTransportador = i.IdTransportador;
                            break;

                        case Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Recebedor:
                            ctrlParticipanteRecebedor.IdLoja = i.IdLoja;
                            ctrlParticipanteRecebedor.IdCliente = i.IdCliente;
                            ctrlParticipanteRecebedor.IdFornec = i.IdFornec;
                            ctrlParticipanteRecebedor.IdTransportador = i.IdTransportador;
                            break;
                    }

                    // se participante for o tomador, verifica qual o tipo dele 
                    //(loja, fornecedor, transportador ou cliente) e seta os valores pertinentes ao tipo
                    if (i.Tomador)
                    {
                        drpTipoTomador.SelectedValue = i.TipoParticipante.ToString().ToLower();

                        if (i.IdLoja > 0)
                        {
                            ctrlParticipanteTomador.IdLoja = i.IdLoja;
                            hdfTomadorSelecionado.Value = i.IdLoja.ToString();
                        }
                        else if (i.IdCliente > 0)
                        {
                            ctrlParticipanteTomador.IdCliente = i.IdCliente;
                            hdfTomadorSelecionado.Value = i.IdCliente.ToString();
                        }
                        else if (i.IdFornec > 0)
                        {
                            ctrlParticipanteTomador.IdFornec = i.IdFornec;
                            hdfTomadorSelecionado.Value = i.IdFornec.ToString();
                        }
                        else if (i.IdTransportador > 0)
                        {
                            ctrlParticipanteTomador.IdTransportador = i.IdTransportador;
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
