using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPedidoEspelhoGerarMult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                chkFinalizarEspelho.Checked = Configuracoes.PedidoConfig.TelaConfirmaPedidoLiberacao.GerarPedidoMarcado;
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    
        protected void btnGerarEspelho_Click(object sender, EventArgs e)
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
    
                DateTime? dataEntrega = !String.IsNullOrEmpty(((TextBox)ctrlDataEntrega.FindControl("txtData")).Text) ? 
                    (DateTime?)DateTime.Parse(((TextBox)ctrlDataEntrega.FindControl("txtData")).Text) : null;
    
                if (idsPedidos.Count == 0)
                    Glass.MensagemAlerta.ShowMsg("Informe os pedidos que serão geradas as conferências.", Page);
                else
                {
                    Glass.MensagemAlerta.ShowMsg(WebGlass.Business.PedidoEspelho.Fluxo.Gerar.Instance.GerarVarios(idsPedidos,
                        chkAlterarDataEntrega.Checked, dataEntrega, chkFinalizarEspelho.Checked), Page);
    
                    grdPedido.DataBind();
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar Conferência do Pedido.", ex, Page);
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    }
}
