using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class CadRoteiroProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] != null && dtvRoteiroProducao.CurrentMode != DetailsViewMode.Edit)
                dtvRoteiroProducao.ChangeMode(DetailsViewMode.Edit);
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/Producao/LstRoteiroProducao.aspx");
        }
    
        protected void cblSetores_DataBound(object sender, EventArgs e)
        {
            var itens = odsSetor.Select().Cast<WebGlass.Business.Setor.Entidade.SetorParaRoteiro>();
    
            var codigos = (sender as WebControl).Parent.FindControl("hdfCodigosSetores") as HiddenField;
            List<string> setores = new List<string>(codigos.Value.Split(','));
    
            foreach (ListItem item in (sender as CheckBoxList).Items)
            {
                item.Attributes.Add("onclick", "alteraSetores()");
                item.Attributes.Add("valor", item.Value);
                item.Selected = setores.Contains(item.Value);
    
                /* var i = itens.First(x => x.Codigo.ToString() == item.Value);
                if (i.Beneficiamento)
                    item.Text += " *"; */
            }
        }
    
        protected void odsRoteiroProducao_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir roteiro de produção.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                btnCancelar_Click(sender, EventArgs.Empty);
        }
    
        protected void odsRoteiroProducao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar roteiro de produção.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                btnCancelar_Click(sender, EventArgs.Empty);
        }
    }
}
