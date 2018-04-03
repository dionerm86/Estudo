using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using System.Collections.Generic;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadConfirmarPedidoInterno : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            confirmar.Visible = false;
    
            if (selPedidoInterno.Valor == "")
                return;
    
            try
            {
                uint idPedidoInterno = Glass.Conversoes.StrParaUint(selPedidoInterno.Valor);
                if (PedidoInternoDAO.Instance.GetCount(idPedidoInterno, 0, 0, null, null, null) == 0)
                    throw new Exception("Esse pedido interno n�o existe.");
    
                if (!PedidoInternoDAO.Instance.PodeConfirmar(null, idPedidoInterno))
                    throw new Exception("Esse pedido interno n�o pode ser confirmado. Ele pode estar cancelado, confirmado ou aguardando autoriza��o.");
    
                confirmar.Visible = true;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao buscar o pedido interno.", ex, Page);
            }
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            bool erro = false;
            try
            {
                uint idPedidoInterno = Glass.Conversoes.StrParaUint(selPedidoInterno.Valor);
    
                Glass.Data.Model.PedidoInterno pedido = PedidoInternoDAO.Instance.GetElement(idPedidoInterno);
    
                // Cria o dicion�rio para o par�metro do m�todo
                Dictionary<uint, float> qtdeProd = new Dictionary<uint, float>();
    
                for (int i = 0; i < grdProdutos.Rows.Count; i++) 
                {
                    uint idProdPedInterno = Glass.Conversoes.StrParaUint(((HiddenField)grdProdutos.Rows[i].FindControl("hdfIdProdPedInterno")).Value);
                    float qtde = float.Parse(((TextBox)grdProdutos.Rows[i].FindControl("txtQtde")).Text);
                    float qtdePedido = float.Parse(((Label)grdProdutos.Rows[i].FindControl("lblQtdePedido")).Text);
                    float qtdeM2 = float.Parse(((Label)grdProdutos.Rows[i].FindControl("Label2")).Text);
                    RangeValidator v = (RangeValidator)grdProdutos.Rows[i].FindControl("RangeValidator1");
                    
                    Glass.Data.Model.ProdutoPedidoInterno produto = ProdutoPedidoInternoDAO.Instance.GetElement(idProdPedInterno);

                    bool m2 = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, (int)produto.IdGrupoProd, (int?)produto.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                               Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, (int)produto.IdGrupoProd, (int?)produto.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    ProdutoLojaDAO.Instance.NewProd((int)produto.IdProd, (int)pedido.IdLoja);
                    var quantidadeEstoque = ProdutoLojaDAO.Instance.GetEstoque(null, pedido.IdLoja, produto.IdProd, null, false, false, false);
                    
                    if (qtde < 0)
                    {
                        erro = true;
                        v.IsValid = false;
                        v.ErrorMessage = "Produto " + produto.DescrProduto + ": A quantidade confirmada n�o pode ser menor que zero.";
                        v.ToolTip = v.ErrorMessage;
                    }

                    if(m2 && qtde > qtdeM2)
                    {
                        erro = true;
                        v.IsValid = false;
                        v.ErrorMessage = "Produto " + produto.DescrProduto + ": A quantidade confirmada n�o pode ser superior � quantidade pedida!";
                        v.ToolTip = v.ErrorMessage;
                    }
    
                    if (!m2 && qtde > qtdePedido)
                    {
                        erro = true;
                        v.IsValid = false;
                        v.ErrorMessage = "Produto " + produto.DescrProduto + ": A quantidade confirmada n�o pode ser superior � quantidade pedida!";
                        v.ToolTip = v.ErrorMessage;
                    }
                    else if (quantidadeEstoque < qtde)
                    {
                        erro = true;
                        v.IsValid = false;
                        v.ErrorMessage = "Produto " + produto.DescrProduto + ": Verifique a disponibilidade em estoque!";
                        v.ToolTip = v.ErrorMessage;
                    }
                    else
                    {
                        qtdeProd.Add(idProdPedInterno, qtde);
                    }
                }
    
                if (erro)
                {
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Falha", "alert('A confirma��o do pedido falhou. Verifique os erros!'); return false;", true);
                    return;
                }
                else
                {
                    PedidoInternoDAO.Instance.Confirmar(idPedidoInterno, qtdeProd);
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok", "alert('Pedido interno confirmado!'); redirectUrl(window.location.href);", true);
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao confirmar o pedido interno.", ex, Page);
            }
        }
    }
}
