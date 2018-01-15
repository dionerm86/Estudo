using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class EntregaCte : CteBaseUserControl
    {
        #region Propriedades

        public WebGlass.Business.ConhecimentoTransporte.Entidade.EntregaCte ObjEntregaCte
        {
            get
            {
                var txtDataProg = ((TextBox)ctrlDataProg.FindControl("txtData"));
                var dataProg = txtDataProg.Text != "" ? txtDataProg.Text : "01/01/0001";
                var horaProg = txtHoraProg.Text != "" ? txtHoraProg.Text : "00:00:00";
    
                var txtDataIni = ((TextBox)ctrlDataIni.FindControl("txtData"));
                var dataIni = txtDataIni.Text != "" ? txtDataIni.Text : "01/01/0001";
                var horaIni = txtHoraInicio.Text != "" ? txtHoraInicio.Text : "00:00:00";
    
                var txtDataFim = ((TextBox)ctrlDataFim.FindControl("txtData"));
                var dataFim = txtDataFim.Text != "" ? txtDataFim.Text : "01/01/0001";
                var horaFim = txtHoraFim.Text != "" ? txtHoraFim.Text : "00:00:00";

                return new WebGlass.Business.ConhecimentoTransporte.Entidade.EntregaCte(
                    new Glass.Data.Model.Cte.EntregaCte
                    {
                        DataHoraFim = Convert.ToDateTime(dataFim + " " + horaFim),
                        DataHoraIni = Convert.ToDateTime(dataIni + " " + horaIni),
                        DataHoraProg = Convert.ToDateTime(dataProg + " " + horaProg),
                        TipoPeriodoData = Glass.Conversoes.StrParaInt(drpPeriodoData.SelectedValue),
                        TipoPeriodoHora = Glass.Conversoes.StrParaInt(drpPeriodoHora.SelectedValue)
                    });
            }
            set
            {
                string dataProg = string.Empty, horaProg = string.Empty, dataIni = string.Empty,
                    horaIni = string.Empty, dataFim = string.Empty, horaFim = string.Empty;
    
                drpPeriodoData.SelectedValue = value.TipoPeriodoData.ToString();
                drpPeriodoHora.SelectedValue = value.TipoPeriodoHora.ToString();
    
                if (value.TipoPeriodoData != 0)
                {
                    dataProg = value.DataHoraProg.ToString().Substring(0, 10) != "01/01/0001" ? value.DataHoraProg.ToString().Substring(0, 10) : "";
                    dataIni = value.DataHoraIni.ToString().Substring(0, 10) != "01/01/0001" ? value.DataHoraIni.ToString().Substring(0, 10) : "";
                    dataFim = value.DataHoraFim.ToString().Substring(0, 10) != "01/01/0001" ? value.DataHoraFim.ToString().Substring(0, 10) : "";
                }
    
                if (value.TipoPeriodoHora != 0)
                {
                    horaProg = value.DataHoraProg.ToString().Substring(11) != "00:00:00" ? value.DataHoraProg.ToString().Substring(11) : "";
                    horaIni = value.DataHoraIni.ToString().Substring(11) != "00:00:00" ? value.DataHoraIni.ToString().Substring(11) : "";
                    horaFim = value.DataHoraFim.ToString().Substring(11) != "00:00:00" ? value.DataHoraFim.ToString().Substring(11) : "";
                }
    
                if (value != null)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaEntregaEditar('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');",
                        dataProg, horaProg, dataIni, horaIni, dataFim, horaFim), true);
                }
            }
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return new BaseValidator[] { ctvDataProg, ctvDataIni, ctvDataFim, ctvHoraProg, ctvHoraIni, ctvHoraFim }; }
        }
    
        /*
         * Função carregaEntregaEditar é responsável pelo carregmento dos dados da entrega na tela de edição.
         * Os campos são exibidos conforme valores.
         */
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ObjEntregaCte.IdCte == 0 && TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
            {
                drpPeriodoData.SelectedValue = Configuracoes.FiscalConfig.TelaCadastroCTe.TipoPeriodoDataEntregaCtePadraoCteSaida;
                drpPeriodoHora.SelectedValue = Configuracoes.FiscalConfig.TelaCadastroCTe.TipoPeriodoHoraEntregaCtePadraoCteSaida;
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "iniciar_EntregaCte_script"))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "iniciar_EntregaCte_script", string.Format(@"
                    var divDataProg = FindControl('divDataProg', 'div');
                    var divDataIni = FindControl('divDataInicio', 'div');
                    var divDataFim = FindControl('divDataFim', 'div');

                    divDataProg.style.display = 'none';
                    divDataIni.style.display = 'none';
                    divDataFim.style.display = 'none';

                    var divHoraProg = FindControl('divHoraProg', 'div');
                    var divHoraIni = FindControl('divHoraInicio', 'div');
                    var divHoraFim = FindControl('divHoraFim', 'div');

                    divHoraProg.style.display = 'none';
                    divHoraIni.style.display = 'none';
                    divHoraFim.style.display = 'none';

                    {0}                        
                    {1}
                ",
                !IsPostBack && ObjEntregaCte.IdCte == 0 &&
                TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida &&
                !string.IsNullOrEmpty(Configuracoes.FiscalConfig.TelaCadastroCTe.TipoPeriodoDataEntregaCtePadraoCteSaida) ?
                    "ExibirControlesData(true);" : string.Empty,
                !IsPostBack && ObjEntregaCte.IdCte == 0 &&
                TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida &&
                !string.IsNullOrEmpty(Configuracoes.FiscalConfig.TelaCadastroCTe.TipoPeriodoHoraEntregaCtePadraoCteSaida) ?
                    "ExibirControlesHora(true);" : string.Empty), true);
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "EntregaCte_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "EntregaCte_script", @"
                    function carregaEntregaEditar(dataProg, horaProg, dataIni, horaIni, dataFim, horaFim)
                    {
                        var divDataProg = FindControl('divDataProg', 'div');
                        var divDataIni = FindControl('divDataInicio', 'div');
                        var divDataFim = FindControl('divDataFim', 'div');
    
                        var divHoraProg = FindControl('divHoraProg', 'div');
                        var divHoraIni = FindControl('divHoraInicio', 'div');
                        var divHoraFim = FindControl('divHoraFim', 'div');

                        var txtDataProg = FindControl('" + this.ctrlDataProg.ClientID + @"_txtData', 'input');
                        var txtDataIni = FindControl('" + this.ctrlDataIni.ClientID + @"_txtData', 'input');
                        var txtDataFim = FindControl('" + this.ctrlDataFim.ClientID + @"_txtData', 'input');

                        var txtHoraProg = FindControl('txtHoraProg', 'input');
                        var txtHoraIni = FindControl('txtHoraInicio', 'input');
                        var txtHoraFim = FindControl('txtHoraFim', 'input');

                        if(dataProg != '')
                        {
                            txtDataProg.value = dataProg;
                            divDataProg.style.display = 'block';
                        }
                        if(dataIni != '')
                        {
                            divDataIni.style.display = 'block';
                            txtDataIni.value = dataIni;
                        }
                        if(dataFim != '')
                        {
                            divDataFim.style.display = 'block';
                            txtDataFim.value = dataFim;
                        }
                        if(horaProg != '')
                        {
                            divHoraProg.style.display = 'block';
                            txtHoraProg.value = horaProg;
                        }
                        if(horaIni != '')
                        {
                            divHoraIni.style.display = 'block';
                            txtHoraIni.value = horaIni;
                        }
                        if(horaFim != '')
                        {
                            divHoraFim.style.display = 'block';
                            txtHoraFim.value = horaFim;
                        }

                        ExibirControlesData(true);
                        ExibirControlesHora(true);
                    }

                    //Função que exibe controles de data conforme tipo de data selecionado
                    function ExibirControlesData(manterValorCampos)
                    {
                        var dropSelecaoTipoData = FindControl('drpPeriodoData', 'select');
                        var txtDataProg = FindControl('" + this.ctrlDataProg.ClientID + @"_txtData', 'input');
                        var txtDataIni = FindControl('" + this.ctrlDataIni.ClientID + @"_txtData', 'input');
                        var txtDataFim = FindControl('" + this.ctrlDataFim.ClientID + @"_txtData', 'input');

                        if (!manterValorCampos) {
                            txtDataProg.value = '';
                            txtDataIni.value = '';
                            txtDataFim.value = '';
                        }
            
                        var divDataProg = FindControl('divDataProg', 'div');
                        var divDataIni = FindControl('divDataInicio', 'div');
                        var divDataFim = FindControl('divDataFim', 'div');

                        divDataProg.style.display = 'none';
                        divDataIni.style.display = 'none';
                        divDataFim.style.display = 'none';

                       if (dropSelecaoTipoData.value == '1')
                            divDataProg.style.display = 'block';
                       else if (dropSelecaoTipoData.value == '2')
                            divDataFim.style.display = 'block';
                       else if (dropSelecaoTipoData.value == '3')
                            divDataIni.style.display = 'block';
                       else if (dropSelecaoTipoData.value == '4')
                       {
                            divDataIni.style.display = 'block';
                            divDataFim.style.display = 'block';
                       }
                    }

                    //Função que exibe controles de hora conforme tipo de hora selecionada
                    function ExibirControlesHora(manterValorCampos)
                    {
                        var dropSelecaoTipoHora = FindControl('drpPeriodoHora', 'select');
                        var txtHoraProg = FindControl('txtHoraProg', 'input');
                        var txtHoraIni = FindControl('txtHoraInicio', 'input');
                        var txtHoraFim = FindControl('txtHoraFim', 'input');

                        if (!manterValorCampos) {
                            txtHoraProg.value = '';
                            txtHoraIni.value = '';
                            txtHoraFim.value = '';
                        }

                        var divHoraProg = FindControl('divHoraProg', 'div');
                        var divHoraIni = FindControl('divHoraInicio', 'div');
                        var divHoraFim = FindControl('divHoraFim', 'div');

                        divHoraProg.style.display = 'none';
                        divHoraIni.style.display = 'none';
                        divHoraFim.style.display = 'none';

                        if(dropSelecaoTipoHora.value == '1')
                            divHoraProg.style.display = 'block';
                        else if(dropSelecaoTipoHora.value == '2')
                            divHoraFim.style.display = 'block';
                        else if(dropSelecaoTipoHora.value == '3')
                            divHoraIni.style.display = 'block';
                        else if(dropSelecaoTipoHora.value == '4')
                        {
                            divHoraIni.style.display = 'block';
                            divHoraFim.style.display = 'block';
                        }
                    }", true);
            }
        }
    }
}