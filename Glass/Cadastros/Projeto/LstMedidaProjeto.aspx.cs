using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class LstMedidaProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (MedidaProjetoDAO.Instance.GetCountReal(txtDescricao.Text) == 0)
                foreach (TableCell c in grdMedidaProjeto.Rows[0].Cells)
                    c.Text = String.Empty;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                MedidaProjeto medida = new MedidaProjeto();
                medida.Descricao = ((TextBox)grdMedidaProjeto.FooterRow.FindControl("txtDescricaoIns")).Text;
                medida.ValorPadrao = Glass.Conversoes.StrParaInt(((TextBox)grdMedidaProjeto.FooterRow.FindControl("txtValorPadrao")).Text);
                medida.ExibirMedidaExata = ((CheckBox)grdMedidaProjeto.FooterRow.FindControl("chkMedidaExata")).Checked;
                medida.IdGrupoMedProj = Glass.Conversoes.StrParaUintNullable(((DropDownList)grdMedidaProjeto.FooterRow.FindControl("drpGrupoMedProj")).SelectedValue);
    
                MedidaProjetoDAO.Instance.Insert(medida);
    
                grdMedidaProjeto.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir medida.", ex, Page);
            }
        }
    
        protected void odsMedidaProjeto_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsMedidaProjeto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdMedidaProjeto.PageIndex = 0;
        }
    }
}
