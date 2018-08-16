using Colosoft.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SelCentroCusto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (CentroCustoDAO.Instance.GetCountReal() > 0 &&
                    CentroCustoAssociadoDAO.Instance.ObtemDadosCentroCustoCount(Glass.Conversoes.StrParaInt(Request["idCompra"]), Glass.Conversoes.StrParaInt(Request["idImpostoServ"]),
                    Glass.Conversoes.StrParaInt(Request["idNf"]), Glass.Conversoes.StrParaInt(Request["idContaPg"]), Glass.Conversoes.StrParaInt(Request["IdCte"])) == 0)
                    foreach (TableCell c in grdCentroCustoAssociado.Rows[0].Cells)
                        c.Text = String.Empty;
            }
        }
        
        protected void lnkInsCentroCusto_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var centroCustoCompra = new CentroCustoAssociado();
                centroCustoCompra.IdCompra = Glass.Conversoes.StrParaIntNullable(Request["idCompra"]);
                centroCustoCompra.IdImpostoServ = Glass.Conversoes.StrParaIntNullable(Request["idImpostoServ"]);
                centroCustoCompra.IdNf = Glass.Conversoes.StrParaIntNullable(Request["idNf"]);
                centroCustoCompra.IdContaPg = Glass.Conversoes.StrParaIntNullable(Request["idContaPg"]);
                centroCustoCompra.IdCentroCusto = Glass.Conversoes.StrParaInt(((DropDownList)grdCentroCustoAssociado.FooterRow.FindControl("ddlCentroCusto")).SelectedValue);
                centroCustoCompra.Valor = Glass.Conversoes.StrParaDecimal(((TextBox)grdCentroCustoAssociado.FooterRow.FindControl("txtValor")).Text);
                centroCustoCompra.IdCte = Glass.Conversoes.StrParaIntNullable(Request["IdCte"]);

                CentroCustoAssociadoDAO.Instance.Insert(centroCustoCompra);
                grdCentroCustoAssociado.DataBind();
                grvDetalhesCentroCusto.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir centro de custo da compra.", ex, Page);
                return;
            }
        }

        protected void grdCentroCustoAssociado_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdCentroCustoAssociado.ShowFooter = e.CommandName != "Edit";
        }

        protected void odsCentroCustoAssociado_Deleted(object sender, VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }

            grdCentroCustoAssociado.DataBind();
            grvDetalhesCentroCusto.DataBind();
        }

        protected void odsCentroCustoAssociado_Updated(object sender, VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }

            grdCentroCustoAssociado.DataBind();
            grvDetalhesCentroCusto.DataBind();
        }

    }
}