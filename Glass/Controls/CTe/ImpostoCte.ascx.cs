using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;
using Glass.Data.EFD;
using System.Collections.Generic;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class ImpostoCte : CteBaseUserControl
    {
        private DataSourcesEFD.TipoImpostoEnum _tipoImposto;
        private string[] _cstNormal, _cstComRed, _cstSt, _cstComCred;

        #region Propriedades
    
        public DataSourcesEFD.TipoImpostoEnum TipoImposto
        {
            get { return _tipoImposto; }
            set
            {
                _tipoImposto = value;
                cvdrpCst.ErrorMessage = string.Format("Imposto - {0}: Campo CST deve ser selecionado.",
                    DataSourcesEFD.Instance.GetDescrTipoImposto((int)value));
                rftxtAliquota.ErrorMessage = string.Format("Imposto - {0}: Campo Alíquota deve ser informado.",
                    DataSourcesEFD.Instance.GetDescrTipoImposto((int)value));
                rftxtAliquotaStRetido.ErrorMessage = string.Format("Imposto - {0}: Campo Alíquota ST Retido deve ser informado.",
                    DataSourcesEFD.Instance.GetDescrTipoImposto((int)value));
                rftxtBC.ErrorMessage = string.Format("Imposto - {0}: Campo Valor BC deve ser informado.",
                    DataSourcesEFD.Instance.GetDescrTipoImposto((int)value));
                rftxtBCSTRetido.ErrorMessage = string.Format("Imposto - {0}: Campo BC ST Retido deve ser informado.",
                    DataSourcesEFD.Instance.GetDescrTipoImposto((int)value));
                rftxtPercRedBC.ErrorMessage = string.Format("Imposto - {0}: Campo Percentual Red. BC deve ser informado.",
                    DataSourcesEFD.Instance.GetDescrTipoImposto((int)value));
                rftxtValorImposto.ErrorMessage = string.Format("Imposto - {0}: Campo Valor deve ser informado.",
                    DataSourcesEFD.Instance.GetDescrTipoImposto((int)value));
                rftxtValorSTRetido.ErrorMessage = string.Format("Imposto - {0}: Campo Valor ST Retido deve ser informado.",
                    DataSourcesEFD.Instance.GetDescrTipoImposto((int)value));
            }
        }
    
        public bool AtivarValidadores { get; set; }

        public WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte ObjImpostoCte
        {
            get
            {
                return new WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte()
                {
                    TipoImposto = (int)this.TipoImposto,
                    Aliquota = Glass.Conversoes.StrParaFloat(txtAliquota.Text),
                    AliquotaStRetido = Glass.Conversoes.StrParaFloat(txtAliquotaStRetido.Text),
                    BaseCalc = Glass.Conversoes.StrParaDecimal(txtBC.Text),
                    BaseCalcStRetido = Glass.Conversoes.StrParaDecimal(txtBCSTRetido.Text),
                    Cst = drpCst.SelectedValue,
                    PercRedBaseCalc = Glass.Conversoes.StrParaFloat(txtPercRedBC.Text),
                    Valor = Glass.Conversoes.StrParaDecimal(txtValorImposto.Text),
                    ValorCred = Glass.Conversoes.StrParaDecimal(txtValorCredito.Text),
                    ValorStRetido = Glass.Conversoes.StrParaDecimal(txtValorSTRetido.Text)
                };
            }
            set
            {
                var cst = value.Cst;
                var aliquota = value.Aliquota > 0 ? value.Aliquota.ToString() : string.Empty;
                var bcRetido = value.BaseCalcStRetido > 0 ? value.BaseCalcStRetido.ToString() : string.Empty;
                var valorStRetido = value.ValorStRetido > 0 ? value.ValorStRetido.ToString() : string.Empty;
                var bc = value.BaseCalc > 0 ? value.BaseCalc.ToString() : string.Empty;
                var percRedBC = value.PercRedBaseCalc > 0 ? value.PercRedBaseCalc.ToString() : string.Empty;
                var valorImposto = value.Valor > 0 ? value.Valor.ToString() : string.Empty;
                var aliquotaStRetido = value.AliquotaStRetido > 0 ? value.AliquotaStRetido.ToString() : string.Empty;
                var valorCredito = value.ValorCred > 0 ? value.ValorCred.ToString() : string.Empty;
                
                if (TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                    switch (TipoImposto)
                    {
                        case DataSourcesEFD.TipoImpostoEnum.Icms:
                            aliquota = value.Aliquota > 0 ? value.Aliquota.ToString() : string.Empty;
                            bcRetido = value.BaseCalcStRetido > 0 ? value.BaseCalcStRetido.ToString() : string.Empty;
                            valorStRetido = value.ValorStRetido > 0 ? value.ValorStRetido.ToString() : string.Empty;
                            break;
                        case DataSourcesEFD.TipoImpostoEnum.Pis:
                            aliquota = value.Aliquota > 0 ? value.Aliquota.ToString() : FiscalConfig.TelaCadastroCTe.AliquotaPISImpostoCtePadraoCteSaida;
                            bcRetido = value.BaseCalcStRetido > 0 ? value.BaseCalcStRetido.ToString() : FiscalConfig.TelaCadastroCTe.BCSTRetidoPISImpostoCtePadraoCteSaida;
                            valorStRetido = value.ValorStRetido > 0 ? value.ValorStRetido.ToString() : FiscalConfig.TelaCadastroCTe.ValorSTRetidoPISImpostoCtePadraoCteSaida;
                            break;
                        case DataSourcesEFD.TipoImpostoEnum.Cofins:
                            aliquota = value.Aliquota > 0 ? value.Aliquota.ToString() : FiscalConfig.TelaCadastroCTe.AliquotaCOFINSImpostoCtePadraoCteSaida;
                            bcRetido = value.BaseCalcStRetido > 0 ? value.BaseCalcStRetido.ToString() : FiscalConfig.TelaCadastroCTe.BCSTRetidoCOFINSImpostoCtePadraoCteSaida;
                            valorStRetido = value.ValorStRetido > 0 ? value.ValorStRetido.ToString() : FiscalConfig.TelaCadastroCTe.ValorSTRetidoCOFINSImpostoCtePadraoCteSaida;
                            break;
                    }

                Page.ClientScript.RegisterStartupScript(GetType(), "iniciarImposto_" + this.ID,
                    string.Format("carregaImpostoInicial('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
                    this.ClientID, cst, bc, percRedBC, aliquota, valorImposto, bcRetido, aliquotaStRetido, valorStRetido, valorCredito), true);                        
            }
        }
    
        protected string[] CstNormal
        {
            get { return _cstNormal; }
            set { _cstNormal = value; }
        }
    
        protected string[] CstComRed
        {
            get { return _cstComRed; }
            set { _cstComRed = value; }
        }
    
        protected string[] CstSt
        {
            get { return _cstSt; }
            set { _cstSt = value; }
        }
    
        protected string[] CstComCred
        {
            get { return _cstComCred; }
            set { _cstComCred = value; }
        }
    
        public void ItensCst(IEnumerable<GenericModel> cst, string[] cstNormal, string[] cstComRed, string[] cstSt, string[] cstComCred)
        {
            // Remove os itens além do primeiro ("selecione")
            while (drpCst.Items.Count > 1)
                drpCst.Items.RemoveAt(1);
    
            // Adiciona os itens à lista
            foreach (var c in cst)
                drpCst.Items.Add(new ListItem(c.Descr, c.Id != null ? c.Id.Value.ToString("00") : String.Empty));
    
            CstNormal = cstNormal;
            CstComRed = cstComRed;
            CstSt = cstSt;
            CstComCred = cstComCred;
        }    
    
        #endregion
    
        public override IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return new[] { cvdrpCst }; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ObjImpostoCte.IdCte == 0 && TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
            {
                switch (TipoImposto)
                {
                    case DataSourcesEFD.TipoImpostoEnum.Icms:
                        drpCst.SelectedValue = FiscalConfig.TelaCadastroCTe.CSTICMSImpostoCtePadraoCteSaida;
                        break;
                    case DataSourcesEFD.TipoImpostoEnum.Pis:
                        drpCst.SelectedValue = FiscalConfig.TelaCadastroCTe.CSTPISImpostoCtePadraoCteSaida;
                        txtAliquota.Text = FiscalConfig.TelaCadastroCTe.AliquotaPISImpostoCtePadraoCteSaida;
                        txtValorSTRetido.Text = FiscalConfig.TelaCadastroCTe.ValorSTRetidoPISImpostoCtePadraoCteSaida;
                        txtBCSTRetido.Text = FiscalConfig.TelaCadastroCTe.BCSTRetidoPISImpostoCtePadraoCteSaida;
                        break;
                    case DataSourcesEFD.TipoImpostoEnum.Cofins:
                        drpCst.SelectedValue = FiscalConfig.TelaCadastroCTe.CSTCOFINSImpostoCtePadraoCteSaida;
                        txtAliquota.Text = FiscalConfig.TelaCadastroCTe.AliquotaCOFINSImpostoCtePadraoCteSaida;
                        txtValorSTRetido.Text = FiscalConfig.TelaCadastroCTe.ValorSTRetidoCOFINSImpostoCtePadraoCteSaida;
                        txtBCSTRetido.Text = FiscalConfig.TelaCadastroCTe.BCSTRetidoCOFINSImpostoCtePadraoCteSaida;
                        break;
                }
            }
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ImpostoCte_script"))
            {
                var impostoCteScript =
                    @"
                    function inArray(array, valor) {
                        for (var i in array)
                            if (array[i] == valor)
                                return true;

                        return false;
                    }
        
                    function exibirControlesImposto(nomeControle, cstNormal, cstComRed, cstSt, cstComCred, inicio) {
                        inicio = inicio || false;
                        var parent = document.getElementById(nomeControle);
                        var valorBC = FindControl('valor-bc', 'div', parent);
                        var perRedBC = FindControl('perc-red-bc', 'div', parent);
                        var aliquota = FindControl('aliquota', 'div', parent);
                        var valorImposto = FindControl('valor-imposto', 'div', parent);
                        var bcStRetido = FindControl('bc-st-retido', 'div', parent);
                        var aliquotaSTRetido = FindControl('aliquota-st-retido', 'div', parent);
                        var valorStRetido = FindControl('valor-st-retido', 'div', parent);
                        var valorCredito = FindControl('valor-credito', 'div', parent);

                        valorBC.hidden = perRedBC.hidden = aliquota.hidden = valorImposto.hidden = 
                            bcStRetido.hidden = aliquotaSTRetido.hidden = valorStRetido.hidden = valorCredito.hidden = true;

                        var rftxtBC = FindControl('rftxtBC', 'span', parent);
                        var rftxtPercRedBC = FindControl('rftxtPercRedBC', 'span', parent);
                        var rftxtAliquota = FindControl('rftxtAliquota', 'span', parent);
                        var rftxtValorImposto = FindControl('rftxtValorImposto', 'span', parent);
                        var rftxtBCSTRetido = FindControl('rftxtBCSTRetido', 'span', parent);
                        var rftxtAliquotaStRetido = FindControl('rftxtAliquotaStRetido', 'span', parent);
                        var rftxtValorSTRetido = FindControl('rftxtValorSTRetido', 'span', parent);

                        ?retiraValidationGroup

                        if (!inicio) {
                            FindControl('txtBC', 'input', parent).value = FindControl('txtPercRedBC', 'input', parent).value = 
                                FindControl('txtAliquota', 'input', parent).value = FindControl('txtValorImposto', 'input', parent).value = 
                                FindControl('txtBCSTRetido', 'input', parent).value = FindControl('txtAliquotaStRetido', 'input', parent).value = 
                                FindControl('txtValorSTRetido', 'input', parent).value = FindControl('txtValorCredito', 'input', parent).value = '';
                        }

                        var valorCst = FindControl('drpCst', 'select', parent).value;

                        if (inArray(cstNormal, valorCst))
                        {
                            valorBC.hidden = false;
                            aliquota.hidden = false;
                            valorImposto.hidden = false;

                            ?rftxtBCValidationGroup
                            ?rftxtAliquotaValidationGroup
                            ?rftxtValorImpostoValidationGroup
                        }
                        else if (inArray(cstComRed, valorCst) && inArray(cstSt, valorCst))
                        {
                            perRedBC.hidden = false;
                            valorBC.hidden = false;
                            aliquota.hidden = false;
                            valorImposto.hidden = false;
                            bcStRetido.hidden = false;
                            valorStRetido.hidden = false;
                            valorCredito.hidden = false;

                            ?rftxtPercRedBCValidationGroup
                            ?rftxtBCValidationGroup
                            ?rftxtAliquotaValidationGroup
                            ?rftxtValorImpostoValidationGroup
                            ?rftxtBCSTRetidoValidationGroup
                            ?rftxtValorSTRetidoValidationGroup
                        }
                        else if (inArray(cstComRed, valorCst))
                        {
                            perRedBC.hidden = false;
                            valorBC.hidden = false;
                            aliquota.hidden = false;
                            valorImposto.hidden = false;

                            ?rftxtPercRedBCValidationGroup
                            ?rftxtBCValidationGroup
                            ?rftxtAliquotaValidationGroup
                            ?rftxtValorImpostoValidationGroup
                        }
                        else if(inArray(cstSt, valorCst))
                        {
                            bcStRetido.hidden = false;
                            valorStRetido.hidden = false;
                            aliquota.hidden = false;
                            valorCredito.hidden = false;

                            ?rftxtBCSTRetidoValidationGroup
                            ?rftxtValorSTRetidoValidationGroup
                            ?rftxtAliquotaValidationGroup
                        }
                        else if(inArray(cstComCred, valorCst))
                        {
                            perRedBC.hidden = false;
                            valorBC.hidden = false;
                            aliquota.hidden = false;
                            valorImposto.hidden = false;
                            valorCredito.hidden = false;

                            ?rftxtBCSTRetidoValidationGroup
                            ?rftxtBCValidationGroup
                            ?rftxtAliquotaValidationGroup
                            ?rftxtValorImpostoValidationGroup
                        }
                    }

                    function carregaImpostoInicial(nomeControle, cst, bc, percRedBc, aliquota, valorImposto, bcRetido, 
                        aliquotaStRetido, valorStRetido, valorCredito)
                    {
                        var parent = document.getElementById(nomeControle);
                        var drpCst = FindControl(nomeControle + '_drpCst', 'select', parent);
                        var txtBc = FindControl(nomeControle + '_txtBC', 'input', parent);
                        var txtPercRedBC = FindControl(nomeControle + '_txtPercRedBC', 'input', parent);
                        var txtaliquota = FindControl(nomeControle + '_txtAliquota', 'input', parent);
                        var txtvalorImposto = FindControl(nomeControle + '_txtValorImposto', 'input', parent);
                        var txtbcRetido = FindControl(nomeControle + '_txtBCSTRetido', 'input', parent);
                        var txtAliquotaStRetido = FindControl(nomeControle + '_txtAliquotaStRetido', 'input', parent);
                        var txtValorStRetido = FindControl(nomeControle + '_txtValorSTRetido', 'input', parent);
                        var txtValorCredito = FindControl(nomeControle + '_txtValorCredito', 'input', parent);

                        var drpCst = FindControl(nomeControle + '_drpCst', 'select', parent);
                        var divBc = FindControl('valor-bc', 'div', parent);
                        var divPercRedBC = FindControl('perc-red-bc', 'div', parent);
                        var divAliquota = FindControl('aliquota', 'div', parent);
                        var divValorImposto = FindControl('valor-imposto', 'div', parent);
                        var divBcRetido = FindControl('bc-st-retido', 'div', parent);
                        var divAliquotaStRetido = FindControl('aliquota-st-retido', 'div', parent);
                        var divValorStRetido = FindControl('valor-st-retido', 'div', parent);
                        var divValorCredito = FindControl('valor-credito', 'div', parent);

                        drpCst.value = cst;

                        txtBc.value = bc;
                        divBc.hidden = bc.length == 0;

                        txtPercRedBC.value = percRedBc;
                        divPercRedBC.hidden = percRedBc.length == 0;

                        txtaliquota.value = aliquota;
                        divAliquota.hidden = aliquota.length == 0;

                        txtvalorImposto.value = valorImposto;
                        divValorImposto.hidden = valorImposto.length == 0;

                        txtbcRetido.value = bcRetido;
                        divBcRetido.hidden = bcRetido.length == 0;

                        txtAliquotaStRetido.value = aliquotaStRetido;
                        divAliquotaStRetido.hidden = aliquotaStRetido.length == 0;

                        txtValorStRetido.value = valorStRetido;
                        divValorStRetido.hidden = valorStRetido.length == 0;

                        txtValorCredito.value = valorCredito;
                        divValorCredito.hidden = valorCredito.length == 0;
                    }";

                impostoCteScript = impostoCteScript
                    .Replace("?retiraValidationGroup", @"
                            if (rftxtBC != null)
                                rftxtBC.validationGroup = '';

                            if (rftxtPercRedBC != null)
                                rftxtPercRedBC.validationGroup = '';

                            if (rftxtAliquota != null)
                                rftxtAliquota.validationGroup = '';

                            if (rftxtValorImposto != null)
                                rftxtValorImposto.validationGroup = '';
                            
                            if (rftxtBCSTRetido != null)
                                rftxtBCSTRetido.validationGroup = '';

                            if (rftxtAliquotaStRetido != null)
                                rftxtAliquotaStRetido.validationGroup = '';

                            if (rftxtValorSTRetido != null)
                                rftxtValorSTRetido.validationGroup = '';");

                if (TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                    impostoCteScript = impostoCteScript
                        .Replace("?rftxtAliquotaValidationGroup", "if (rftxtAliquota != null ) { rftxtAliquota.validationGroup = 'c'; }")
                        .Replace("?rftxtBCValidationGroup", "if (rftxtBC != null ) { rftxtBC.validationGroup = 'c'; }")
                        .Replace("?rftxtBCSTRetidoValidationGroup", "if (rftxtBCSTRetido != null ) { rftxtBCSTRetido.validationGroup = 'c'; }")
                        .Replace("?rftxtPercRedBCValidationGroup", "if (rftxtPercRedBC != null ) { rftxtPercRedBC.validationGroup = 'c'; }")
                        .Replace("?rftxtValorImpostoValidationGroup", "if (rftxtValorImposto != null ) { rftxtValorImposto.validationGroup = 'c'; }")
                        .Replace("?rftxtValorSTRetidoValidationGroup", "if (rftxtValorSTRetido != null ) { rftxtValorSTRetido.validationGroup = 'c'; }");
                else
                    impostoCteScript = impostoCteScript
                        .Replace("?rftxtAliquotaValidationGroup", string.Empty)
                        .Replace("?rftxtBCValidationGroup", string.Empty)
                        .Replace("?rftxtBCSTRetidoValidationGroup", string.Empty)
                        .Replace("?rftxtPercRedBCValidationGroup", string.Empty)
                        .Replace("?rftxtValorImpostoValidationGroup", string.Empty)
                        .Replace("?rftxtValorSTRetidoValidationGroup", string.Empty);

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ImpostoCte_script", impostoCteScript, true);
            }
        }
    
        protected void drpCst_Load(object sender, EventArgs e)
        {
            (sender as DropDownList).Attributes.Add("onchange", ExibirControlesImposto(false));
        }
    
        protected string ExibirControlesImposto(bool inicio)
        {
            var toArray = new Func<string[], string>(x =>
            {
                return x != null && x.Length > 0 ? "['" + String.Join("','", x) + "']" : "[]";
            });
    
            return "exibirControlesImposto('" + this.ClientID +
                String.Format("', {0}, {1}, {2}, {3}, {4})", toArray(CstNormal), toArray(CstComRed),
                toArray(CstSt), toArray(CstComCred), inicio.ToString().ToLower());
        }
    }
}
