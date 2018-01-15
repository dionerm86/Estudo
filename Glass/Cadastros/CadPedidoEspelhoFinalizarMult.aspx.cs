using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPedidoEspelhoFinalizarMult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkAlterarDataEntrega.Checked && ((TextBox)ctrlDataEntrega.FindControl("txtData")).Text == "")
                    throw new Exception("Informe a data de entrega dos pedidos.");
    
                List<string> idsPedidos = new List<string>();
    
                for (int i = 0; i < grdPedido.Rows.Count; i++)
                {
                    CheckBox chkMarcar = grdPedido.Rows[i].Cells[0].FindControl("chkMarcar") as CheckBox;
                    HiddenField hdfIdPedido = grdPedido.Rows[i].Cells[0].FindControl("hdfIdPedido") as HiddenField;
    
                    if (!chkMarcar.Checked)
                        continue;
    
                    idsPedidos.Add(hdfIdPedido.Value);
                }
    
                if (idsPedidos.Count == 0)
                    throw new Exception("Informe os pedidos que serão finalizados.");
    
                else
                {
                    var dataEntrega = ((TextBox)ctrlDataEntrega.FindControl("txtData")).Text;
    
                    Glass.MensagemAlerta.ShowMsg(WebGlass.Business.PedidoEspelho.Fluxo.Finalizar.Instance.FinalizarVarios(idsPedidos,
                        chkAlterarDataEntrega.Checked, !String.IsNullOrEmpty(dataEntrega) ? (DateTime?)DateTime.Parse(dataEntrega) : null), Page);
    
                    grdPedido.DataBind();
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar Conferência do Pedido.", ex, Page);
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    }
}
