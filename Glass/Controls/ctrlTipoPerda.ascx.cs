using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlTipoPerda : BaseUserControl
    {
        public bool PermitirVazio
        {
            get { return !rfvTipoPerda.Enabled; }
            set { rfvTipoPerda.Enabled = !value; }
        }
    
        public bool ExibirItemVazio
        {
            get { return drpTipoPerda.AppendDataBoundItems; }
            set { drpTipoPerda.AppendDataBoundItems = value; }
        }
    
        public uint? IdTipoPerda
        {
            get { return Glass.Conversoes.StrParaUintNullable(drpTipoPerda.SelectedValue); }
            set
            {
                if (value != null)
                {
                    if (drpTipoPerda.Items.Count == 0)
                        drpTipoPerda.DataBind();
    
                    drpTipoPerda.SelectedValue = value.ToString();
                }
                else if (ExibirItemVazio)
                    drpTipoPerda.SelectedIndex = 0;
            }
        }

        public uint? IdSubtipoPerda
        {
            get { return Glass.Conversoes.StrParaUintNullable(hdfIdSubtipoPerda.Value); }
            set { hdfIdSubtipoPerda.Value = value != null ? value.ToString() : ""; }
        }

        public int? IdSetor
        {
            get { return hdfIdSetor.Value.StrParaIntNullable(); }
            set { hdfIdSetor.Value = value != null ? value.ToString() : string.Empty; }
        }

        [Ajax.AjaxMethod]
        public string GetSubtipos(string idTipoPerdaStr)
        {
            try
            {
                string baseOption = "<option value='{0}'>{1}</option>";
                string option = String.Format(baseOption, "", "");
    
                uint idTipoPerda = Glass.Conversoes.StrParaUint(idTipoPerdaStr);
                foreach (SubtipoPerda s in SubtipoPerdaDAO.Instance.GetByTipoPerda(idTipoPerda))
                    option += String.Format(baseOption, s.IdSubtipoPerda, s.Descricao);
    
                return option;
            }
            catch
            {
                return "";
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlTipoPerda));
    
            drpTipoPerda.Attributes.Add("OnChange", "getSubtipos('" + this.ClientID + "', this.value)");
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlTipoPerda_script"))
            {
                string script = @"function getSubtipos(nomeControle, tipoPerda)
                {
                    var resposta = ctrlTipoPerda.GetSubtipos(tipoPerda).value;
                    var drpSubtipoPerda = document.getElementById(nomeControle + '_drpSubtipoPerda');
                    drpSubtipoPerda.innerHTML = resposta;
                    
                    var celula = drpSubtipoPerda;
                    while (celula.nodeName.toLowerCase() != 'td')
                        celula = celula.parentNode;
                    
                    var hdfIdSubtipoPerda = document.getElementById(nomeControle + '_hdfIdSubtipoPerda');
                    
                    if (drpSubtipoPerda.options.length > 1)
                    {
                        celula.style.display = '';
                        drpSubtipoPerda.selectedIndex = 0;
                        hdfIdSubtipoPerda.value = drpSubtipoPerda.value;
                    }
                    else
                    {
                        celula.style.display = 'none';
                        hdfIdSubtipoPerda.value = '';
                    }
                }";
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlTipoPerda_script", script, true);
            }
        }

        protected new string Init()
        {
            uint? idSubtipoPerda = IdSubtipoPerda;

            if (idSubtipoPerda > 0)
            {
                return hdfIdSubtipoPerda.ClientID + ".value = " + idSubtipoPerda + "; " +
                    this.ClientID + "_drpSubtipoPerda.value = " + idSubtipoPerda;
            }
            else
                return "";
        }
    }
}
