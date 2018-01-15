using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaPedidoPcp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.ListaPedidoPcp));
    
            btnBuscar.Visible = PedidoConfig.DadosPedido.AmbientePedido;
            btnVisualizar.Visible = !PedidoConfig.DadosPedido.AmbientePedido;
    
            cblGrupoProd.DataBind();
        }
    
        [Ajax.AjaxMethod()]
        public string VerificaPedido(string idPedido)
        {
            try
            {
                PedidoEspelho.SituacaoPedido situacao = PedidoEspelhoDAO.Instance.ObtemSituacao(Glass.Conversoes.StrParaUint(idPedido));
    
                if (situacao == PedidoEspelho.SituacaoPedido.Processando ||
                    situacao == PedidoEspelho.SituacaoPedido.Aberto)
                    return "Erro\tA conferência deste pedido ainda não foi finalizada.";
    
                return "Ok\t";
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Registro não encontrado.") > -1)
                    return "Erro\tErro: Pedido não encontrado. Verifique se foi gerada conferência desse pedido.";
                else
                    return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string[] verificar = VerificaPedido(txtNumPedido.Text).Split('\t');
            bool exibir = false;
    
            if (verificar[0] != "Ok")
                Glass.MensagemAlerta.ShowMsg(verificar[1], Page);
            else
            {
                exibir = true;
                hdfNumPedido.Value = txtNumPedido.Text;
            }
    
            linhaTipos.Visible = exibir;
            grupos.Visible = exibir;
            produtos.Visible = exibir;
            separador.Visible = exibir;
            observacoes.Visible = exibir;
            visualizar.Visible = exibir;
    
            if (!IsPostBack)
                grdAmbientes.DataBind();
        }
    
        protected void cblGrupoProd_DataBound(object sender, EventArgs e)
        {
            chkMarcarDesmarcar.Checked = true;
    
            for (int i = 0; i < cblGrupoProd.Items.Count; i++)
            {
                cblGrupoProd.Items[i].Attributes.Add("Valor", cblGrupoProd.Items[i].Value);
                cblGrupoProd.Items[i].Selected = chkMarcarDesmarcar.Checked;
            }
        }
    
        protected void chkAmbiente_DataBinding(object sender, EventArgs e)
        {
            CheckBox chkAmbiente = (CheckBox)sender;
            chkAmbiente.Attributes.Add("ambiente", "true");
    
            string script = "document.getElementById('{0}').checked = true;\n" +
                "exibirAmbiente(document.getElementById('{0}'), 'ambiente_' + document.getElementById('{1}').value);\n";
    
            script = String.Format(script, chkAmbiente.ClientID, chkAmbiente.ClientID.Replace("chkAmbiente", "hdfIdAmbientePedido"));
            Page.ClientScript.RegisterStartupScript(GetType(), "exibirAmbiente_" + chkAmbiente.ClientID, script, true);
        }
    
        protected void chkMarcarProduto_DataBinding(object sender, EventArgs e)
        {
            CheckBox chkMarcarProduto = (CheckBox)sender;
            ProdutosPedidoEspelho prodPed = ((GridViewRow)chkMarcarProduto.Parent.Parent).DataItem as ProdutosPedidoEspelho;
            chkMarcarProduto.Attributes.Add("valor", prodPed.IdProdPed.ToString());
        }
    
        protected void grdProdutosPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HiddenField hdfComprado = (HiddenField)e.Row.FindControl("hdfComprado");
            if (hdfComprado == null)
                return;
    
            if (hdfComprado.Value.ToLower() == "true")
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = System.Drawing.Color.Blue;
        }
    }
}
