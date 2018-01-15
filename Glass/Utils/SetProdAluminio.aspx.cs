using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetProdAluminio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(Request["idProd"]);
            lblProd.Text = prod.CodInterno + " - " + prod.Descricao;
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            List<ProdutosPedidoEspelho> lstProdEsp = new List<ProdutosPedidoEspelho>();
            ProdutosPedidoEspelho prodEsp;
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(Request["idProd"]);
            uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
            decimal valor = Glass.Conversoes.StrParaDecimal(Request["val"].Replace('.', ','));
    
            try
            {
                // Para cada qtd e ml
                for (int i = 1; i <= 10; i++)
                {
                    string qtd = ((TextBox)Page.FindControl("txtQtd" + i)).Text;
                    string ml = ((TextBox)Page.FindControl("txtMl" + i)).Text;
    
                    // Se a qtd e o ml tiverem sido informados, insere produto numa lista para ser inserido
                    if (!String.IsNullOrEmpty(qtd) && !String.IsNullOrEmpty(ml))
                    {
                        float alturaReal = Single.Parse(ml, System.Globalization.NumberStyles.AllowDecimalPoint);
                        float altura = alturaReal, totM2 = 0;
                        decimal custo = 0, total = 0;
                        Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(0, prod.IdProd, 0, Glass.Conversoes.StrParaInt(qtd), 1, valor, 0, false, 1, false, ref custo, ref altura, ref totM2, ref total, false, 0);
    
                        prodEsp = new ProdutosPedidoEspelho();
                        prodEsp.IdPedido = idPedido;
                        prodEsp.IdProd = (uint)prod.IdProd;
                        prodEsp.Qtde = Glass.Conversoes.StrParaInt(qtd);
                        prodEsp.AlturaReal = alturaReal;
                        prodEsp.ValorVendido = valor;
                        prodEsp.Altura = altura;
                        prodEsp.IdAmbientePedido = !String.IsNullOrEmpty(Request["idAmbiente"]) ? (uint?)Glass.Conversoes.StrParaUint(Request["idAmbiente"]) : null;
    
                        lstProdEsp.Add(prodEsp);
                    }
                }
    
                // Insere os produtos no pedido
                foreach (ProdutosPedidoEspelho p in lstProdEsp)
                {
                    // Verifica se o produto com a largura e altura especificados já foi adicionado ao pedido
                    if (ProdutosPedidoEspelhoDAO.Instance.ExistsInPedido(p.IdPedido, p.IdProd, p.AlturaReal, p.Largura, p.IdProcesso, p.IdAplicacao))
                        continue;
    
                    ProdutosPedidoEspelhoDAO.Instance.Insert(p);
                }
    
                ClientScript.RegisterClientScriptBlock(typeof(string), "ok", "alert('Produtos inseridos com sucesso.'); window.opener.recarregar(); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir produtos no pedido.", ex, Page);
            }
        }
    }
}
