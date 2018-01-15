using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class SelNotaFiscalReferenciada : System.Web.UI.Page
    {
        static string numerosNota;
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Utils.SelNotaFiscalReferenciada));

            if (!IsPostBack)
            {
                numerosNota = string.Empty;
                if (Request["numeros"] != null)
                {
                    if (!Request["numeros"].Contains(","))
                    {
                        var num = Glass.Data.DAL.NotaFiscalDAO.Instance.ObtemNumeroNf(null, Conversoes.StrParaUint(Request["numeros"]));
                        numerosNota = Request["numeros"] + ";" + num;
                    }
                    else
                    {
                        var idsNf = Request["numeros"].Split(',');
                        foreach (var i in idsNf)
                        {
                            if (i != "0" && !string.IsNullOrEmpty(i))
                            {
                                var num = Glass.Data.DAL.NotaFiscalDAO.Instance.ObtemNumeroNf(null, Conversoes.StrParaUint(i));
                                numerosNota += i + ";" + num + ",";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retorna o Código/Descrição do produto/kit
        /// </summary>
        [Ajax.AjaxMethod()]
        public string ObtemNumeroNF(string numeroNF, string idNfe)
        {
            if (numerosNota == null)
                numerosNota = string.Empty;
            var dados = new List<string>();

            if (numerosNota.Contains(","))
            {
                foreach (var i in numerosNota.Split(','))
                    if (!string.IsNullOrEmpty(i))
                        dados.Add(i);
            }
            else if (!string.IsNullOrEmpty(numerosNota))
                dados.Add(numerosNota);

            if (dados.Any(f => f == idNfe + ";" + numeroNF) && dados.Count == 1)
                dados = new List<string>();
            else if (dados.Any(f => f == idNfe + ";" + numeroNF))
                dados.Remove(idNfe + ";" + numeroNF);
            else
                dados.Add(idNfe + ";" + numeroNF);

            numerosNota = dados.Count > 1 ? string.Join(",", dados) : (dados.Count == 0 ? null : (dados[0]));

            return dados.Count > 1 ? string.Join(",", dados) : (dados.Count == 0 ? null : (dados[0]));
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
        protected void drpLoja_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    
        protected void grdNf_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }
    
        protected void grdNf_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var idNf = ((HiddenField)e.Row.Cells[0].FindControl("hdfIdNf")).Value;
                if (!string.IsNullOrEmpty(numerosNota) && numerosNota.Contains(idNf + ";" + e.Row.Cells[2].Text))
                {
                    CheckBox chk = (CheckBox)e.Row.Cells[0].FindControl("chkItem");
                    chk.Checked = true;
                }
            }
        }

        protected void grdNf_RowCreated(object sender, GridViewRowEventArgs e)
        {
            
        }

        protected void OnPaging(object sender, GridViewPageEventArgs e)
        {

            grdNf.PageIndex = e.NewPageIndex;
            grdNf.DataBind();
            if (ViewState["CheckBoxArray"] != null)
            {
                ArrayList CheckBoxArray = (ArrayList)ViewState["CheckBoxArray"];

                for (int i = 0; i < grdNf.Rows.Count; i++)
                {
                    if (grdNf.Rows[i].RowType == DataControlRowType.DataRow)
                    {
                        int CheckBoxIndex = grdNf.PageSize * (grdNf.PageIndex) + (i + 1);
                        if (CheckBoxArray.IndexOf(CheckBoxIndex) != -1)
                        {
                            CheckBox chk = (CheckBox)grdNf.Rows[i].Cells[0].FindControl("chkItem");
                            chk.Checked = true;
                        }
                    }
                }
            }
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", "ObtemNumeroNFGrid();", true);
        }

        protected void chkItem_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void grdNf_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void grdNf_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {

        }
    }
}
