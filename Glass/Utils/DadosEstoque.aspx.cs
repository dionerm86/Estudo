using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelModel;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class DadosEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            uint idProd = 0;
            uint.TryParse(Request["idProd"], out idProd);
    
            Produto prod = ProdutoDAO.Instance.GetElementIfExist(idProd);
            nomeProduto.Visible = prod != null;
            separador.Visible = !nomeProduto.Visible;
            buscaProduto.Visible = !nomeProduto.Visible;
    
            if (nomeProduto.Visible)
            {
                hdfIdProd.Value = prod.IdProd.ToString();
                lblProduto.Text = prod.CodInterno + " - " + prod.Descricao;
            }
            else
            {
                prod = String.IsNullOrEmpty(txtCodProd.Text) ? null :
                    ProdutoDAO.Instance.GetByCodInterno(txtCodProd.Text);
    
                if (prod != null)
                {
                    hdfIdProd.Value = prod.IdProd.ToString();
                    lblDescrProd.Text = prod.Descricao;
                }
                else
                {
                    hdfIdProd.Value = "";
                    lblDescrProd.Text = "";
                }
            }
    
            if (!IsPostBack)
            {
                bool isCompra = Request["compra"] == "1";
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                uint idOrcamento = Glass.Conversoes.StrParaUint(Request["idOrcamento"]);
    
                lblLiberacao.Visible = PedidoConfig.LiberarPedido;
                grdEstoqueProdutos.Columns[3].Visible = PedidoConfig.LiberarPedido && !isCompra;
                grdProdutosPedido.Columns[1].Visible = PedidoConfig.DadosPedido.AmbientePedido && !isCompra;
    
                dadosProducao.Visible = prod != null && Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd) && !isCompra;
                dadosReserva.Visible = !isCompra;
                dadosCompra.Visible = isCompra || idOrcamento >= 0 || (idPedido > 0 ? PedidoDAO.Instance.GetTipoPedido(null, idPedido) == Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda : false);
            }
        }
    
        protected void imgCancelar_DataBinding(object sender, EventArgs e)
        {
            ImageButton i = (ImageButton)sender;
            i.Visible = false;
    
            GridViewRow linha = i.Parent.Parent as GridViewRow;
            if (linha == null || linha.DataItem == null)
                return;
    
            // O botão cancelar só ficará visível se o pedido que estiver indisponibilizando o estoque 
            // estiver com a data de entrega vencida há mais de 2 dias
            EstoqueProdutos p = linha.DataItem as EstoqueProdutos;
            i.Visible = p.DataEntrega != null && (DateTime.Now - p.DataEntrega.Value).Days > 2;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    }
}
