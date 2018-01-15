using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstOrigemTrocaDesconto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (OrigemTrocaDescontoDAO.Instance.GetListCountReal() == 0)
                foreach (TableCell c in grdOrigem.Rows[0].Cells)
                    c.Text = String.Empty;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                OrigemTrocaDescontoDAO.Instance.Insert(new OrigemTrocaDesconto()
                {
                    Descricao = ((TextBox)grdOrigem.FooterRow.FindControl("txtDescricaoIns")).Text
                });
    
                grdOrigem.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Origem Troca/Desconto.", ex, Page);
            }
        }
    
        protected void odsOrigem_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
