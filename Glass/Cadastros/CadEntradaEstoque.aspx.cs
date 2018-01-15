using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadEntradaEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtNum.Focus();
        }

        protected void imbPesq_Click(object sender, ImageClickEventArgs e)
        {
            uint id = Glass.Conversoes.StrParaUint(txtNum.Text);
            int tipo = drpTipo.SelectedIndex;

            tbEntradaCompra.Visible = false;
            tbEntradaNFe.Visible = false;

            if (tipo == 0) // Compra
            {
                if (!CompraDAO.Instance.CompraExists(id))
                    Glass.MensagemAlerta.ShowMsg("Não existe nenhuma compra com o número passado.", Page);
                else if (CompraDAO.Instance.ObtemEstoqueBaixado(id))
                    Glass.MensagemAlerta.ShowMsg("Esta compra já teve o estoque creditado.", Page);
                else
                {
                    tbEntradaCompra.Visible = true;
                    drpLojaCompra.SelectedValue = CompraDAO.Instance.ObtemIdLoja(id).ToString();
                }
            }
            else if (tipo == 1) // NF
            {
                var notaFiscalEntradaTerceiros = NotaFiscalDAO.Instance.GetByNumeroNFe(id, (int)NotaFiscal.TipoDoc.EntradaTerceiros);
                var notaFiscalEntrada = NotaFiscalDAO.Instance.GetByNumeroNFe(id, (int)NotaFiscal.TipoDoc.Entrada);

                if ((notaFiscalEntradaTerceiros == null || notaFiscalEntradaTerceiros.Length == 0) && (notaFiscalEntrada == null || notaFiscalEntrada.Length == 0))
                    Glass.MensagemAlerta.ShowMsg("Não existe nenhuma nota fiscal de terceiros com o número passado.", Page);
                else
                    tbEntradaNFe.Visible = true;
            }
        }
    
        protected void lnkTodosCompra_Click(object sender, EventArgs e)
        {
            // Marca todos os campos de qtd de saída com o total de saída que falta ser lançado
            foreach (GridViewRow row in grdProdCompra.Rows)
                ((TextBox)row.FindControl("txtQtdEntrada")).Text = (Glass.Conversoes.StrParaInt(((HiddenField)row.FindControl("hdfQtde")).Value) -
                    Glass.Conversoes.StrParaInt(((HiddenField)row.FindControl("hdfQtdEntrada")).Value)).ToString();
        }
    
        protected void lnkTodosNf_Click(object sender, EventArgs e)
        {
            // Marca todos os campos de qtd de saída com o total de saída que falta ser lançado
            foreach (GridViewRow row in grdProdNf.Rows)
                ((TextBox)row.FindControl("txtQtdEntrada")).Text = (Glass.Conversoes.StrParaInt(((HiddenField)row.FindControl("hdfQtde")).Value) -
                    Glass.Conversoes.StrParaInt(((HiddenField)row.FindControl("hdfQtdEntrada")).Value)).ToString();
        }
    
        protected void btnMarcarEntradaCompra_Click(object sender, EventArgs e)
        {
            if (txtNum.Text == String.Empty || grdProdCompra.Rows.Count <= 0)
                return;
    
            try
            {
                if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.AuxAlmoxarifado &&
                    !Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque))
                {
                    throw new Exception("Você não tem permissão para marcar entrada de produtos.");
                }
    
                var lstProdCompra = new List<WebGlass.Business.Compra.Entidade.DadosAumentarEstoque>();
    
                foreach (GridViewRow r in grdProdCompra.Rows)
                {
                    string qtdEntradaInformadaString = ((TextBox)r.FindControl("txtQtdEntrada")).Text;
    
                    var dadosProduto = new WebGlass.Business.Compra.Entidade.DadosAumentarEstoque()
                    {
                        IdProdCompra = Glass.Conversoes.StrParaUint(((HiddenField)r.FindControl("hdfIdProdCompra")).Value),
                        QtdeAumentar = String.IsNullOrEmpty(qtdEntradaInformadaString) ? 0 : float.Parse(qtdEntradaInformadaString),
                        DescricaoProduto = ((HiddenField)r.FindControl("hdfDescr")).Value.Replace("'", "").Replace("\"", "")
                    };
    
                    // Se a quantidade de saída for 0, continua no próximo item
                    if (dadosProduto.QtdeAumentar == 0)
                        continue;
    
                    lstProdCompra.Add(dadosProduto);
                }
    
                WebGlass.Business.Compra.Fluxo.AlterarEstoque.Instance.AumentarEstoque(
                    Glass.Conversoes.StrParaUint(drpTipo.SelectedValue == "0" ? drpLojaCompra.SelectedValue : drpLojaNf.SelectedValue), lstProdCompra, true);
    
                Glass.MensagemAlerta.ShowMsg("Entrada de produtos efetuada com sucesso.", Page);
    
                grdProdCompra.DataBind();
                txtNum.Focus();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao marcar entrada de estoque.", ex, Page);
            }
        }
    
        protected void btnMarcarEntradaNF_Click(object sender, EventArgs e)
        {
            if (txtNum.Text == String.Empty || grdProdNf.Rows.Count <= 0)
                return;
    
            try
            {
                if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.AuxAlmoxarifado &&
                    !Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque))
                {
                    throw new Exception("Você não tem permissão para marcar entrada de produtos.");
                }
    
                var lstProdNf = new List<WebGlass.Business.NotaFiscal.Entidade.DadosAumentarEstoque>();
    
                foreach (GridViewRow r in grdProdNf.Rows)
                {
                    string qtdEntradaInformadaString = ((TextBox)r.FindControl("txtQtdEntrada")).Text;
    
                    var dadosProduto = new WebGlass.Business.NotaFiscal.Entidade.DadosAumentarEstoque()
                    {
                        IdProdNf = Glass.Conversoes.StrParaUint(((HiddenField)r.FindControl("hdfIdProdNf")).Value),
                        QtdeAumentar = String.IsNullOrEmpty(qtdEntradaInformadaString) ? 0 : float.Parse(qtdEntradaInformadaString),
                        DescricaoProduto = ((HiddenField)r.FindControl("hdfDescr")).Value.Replace("'", "").Replace("\"", "")
                    };
    
                    // Se a quantidade de saída for 0, continua no próximo item
                    if (dadosProduto.QtdeAumentar == 0)
                        continue;
    
                    lstProdNf.Add(dadosProduto);
                }
    
                WebGlass.Business.NotaFiscal.Fluxo.AlterarEstoque.Instance.AumentarEstoque(
                    Glass.Conversoes.StrParaUint(drpLojaNf.SelectedValue), lstProdNf, true);
    
                Glass.MensagemAlerta.ShowMsg("Entrada de produtos efetuada com sucesso.", Page);
    
                grdProdNf.DataBind();
                txtNum.Focus();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao marcar entrada de estoque.", ex, Page);
            }
        }
    }
}
