using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstFormulaExpressaoCalculo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imbInserir_Click(object sender, ImageClickEventArgs e)
        {
            var descricao = ((TextBox)grdFormulaExpressaoCalculo.FooterRow.FindControl("txtDescricao")).Text.ToUpper();
            try
            {
                if (descricao != null && descricao != "")
                {
                    FormulaExpressaoCalculo fec = new FormulaExpressaoCalculo { Descricao = descricao };
                    FormulaExpressaoCalculoDAO.Instance.Insert(fec);
                    Glass.MensagemAlerta.ShowMsg("Fórmula inserida com sucesso.", Page);
                }
                else
                {
                    Glass.MensagemAlerta.ShowMsg("Informe a descrição da fórmula.", Page);
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Não foi possível inserir a fórmula.", ex, Page);
            }

            grdFormulaExpressaoCalculo.DataBind();
        }

        protected void grdFormulaExpressaoCalculo_DataBound(object sender, EventArgs e)
        {
            if (grdFormulaExpressaoCalculo.Rows.Count == 1)
                grdFormulaExpressaoCalculo.Rows[0].Visible = FormulaExpressaoCalculoDAO.Instance.GetCountReal() > 0;
            else if (grdFormulaExpressaoCalculo.Rows.Count > 0)
                grdFormulaExpressaoCalculo.Rows[0].Visible = true;
        }

        protected void odsFormulaExpressaoCalculo_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg("Falha ao apagar a fórmula. ", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}