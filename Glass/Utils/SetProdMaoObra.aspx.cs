using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SetProdMaoObra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SetProdMaoObra));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
            uint idCliente = PedidoDAO.Instance.ObtemIdCliente(null, idPedido);
    
            hdfTipoEntrega.Value = PedidoDAO.Instance.ObtemTipoEntrega(idPedido).ToString();
            hdfIdCliente.Value = idCliente.ToString();
            hdfCliRevenda.Value = ClienteDAO.Instance.IsRevenda(idCliente).ToString();
            hdfPercComissao.Value = PedidoDAO.Instance.ObterPercentualComissao(null, (int)idPedido).ToString();
            hdfIsReposicao.Value = PedidoDAO.Instance.IsPedidoReposicao(null, idPedido.ToString()).ToString();
    
            if (!IsPostBack)
                txtQtdeMaoObra.Text = "1";
        }
    
        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno, string tipoEntrega, string revenda, string reposicao, 
            string idCliente, string percComissao, string ambienteMaoObra)
        {
            bool isAmbienteMaoObra = ambienteMaoObra.ToLower() == "true";
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);
    
            if (prod == null)
                return "Erro;Não existe produto com o código informado.";
            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo.";
            else if (prod.Compra)
                return "Erro;Produto utilizado apenas na compra.";
    
            if (!isAmbienteMaoObra)
            {
                if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                    return "Erro;Apenas produtos do grupo 'Mão de Obra Beneficiamento' podem ser incluídos nesse pedido.";
            }
            else if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                return "Erro;Apenas produtos do grupo 'Vidro' podem ser usados como peça de vidro.";
    
            string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;
            decimal valorProduto = 0;
    
            // Recupera o valor de tabela do produto
            int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
            uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
            valorProduto = ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, revenda.ToLower() == "true",
                reposicao.ToLower() == "true", 0, null, null, null);
    
            if (PedidoConfig.Comissao.ComissaoPedido)
                valorProduto = valorProduto / ((100 - decimal.Parse(percComissao)) / 100);
    
            retorno += ";" + valorProduto.ToString("F2");
    
            // Verifica como deve ser feito o cálculo do produto
            retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(prod.IdGrupoProd, prod.IdSubgrupoProd);

            return retorno;
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                var idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                var pcp = !string.IsNullOrEmpty(Request["pcp"]) ? Request["pcp"] == "true" : false;
    
                // Recupera os dados da mão de obra
                var idProdMaoObra = Glass.Conversoes.StrParaUint(hdfIdProdMaoObra.Value);
                int qtdeMaoObra = Glass.Conversoes.StrParaInt(txtQtdeMaoObra.Text);
                var valorMaoObra = Glass.Conversoes.StrParaDecimal(txtValorUnitMaoObra.Text);
                var tipoCalcMaoObra = Glass.Conversoes.StrParaInt(hdfTipoCalcMaoObra.Value);
                var alturaBenef = !string.IsNullOrEmpty(drpAltBenef.SelectedValue) ? Glass.Conversoes.StrParaInt(drpAltBenef.SelectedValue) : 0;
                var larguraBenef = !string.IsNullOrEmpty(drpLargBenef.SelectedValue) ? Glass.Conversoes.StrParaInt(drpLargBenef.SelectedValue) : 0;
                var espBenef = !string.IsNullOrEmpty(txtEspBenef.Text) ? (int?)Glass.Conversoes.StrParaInt(txtEspBenef.Text) : null;
    
                for (var i = 1; i <= 10; i++)
                {
                    var idProdAmbiente = (HiddenField)Master.FindControl("pagina").FindControl("hdfAmbIdProd" + i) != null ?
                        Glass.Conversoes.StrParaUint(((HiddenField)Master.FindControl("pagina").FindControl("hdfAmbIdProd" + i)).Value) : 0;
    
                    if (idProdAmbiente == 0)
                    {
                        continue;
                    }

                    var codAmbiente = (TextBox)Master.FindControl("pagina").FindControl("txtCodAmb" + i) != null ?
                        ((TextBox)Master.FindControl("pagina").FindControl("txtCodAmb" + i)).Text : "";
    
                    var descrAmbiente = (HiddenField)Master.FindControl("pagina").FindControl("hdfDescrAmb" + i) != null ?
                        ((HiddenField)Master.FindControl("pagina").FindControl("hdfDescrAmb" + i)).Value : "";
    
                    var qtdeAmbiente = (TextBox)Master.FindControl("pagina").FindControl("txtQtde" + i) != null ?
                        Glass.Conversoes.StrParaInt(((TextBox)Master.FindControl("pagina").FindControl("txtQtde" + i)).Text) : 0;
    
                    var alturaAmbiente = (TextBox)Master.FindControl("pagina").FindControl("txtAltura" + i) != null ?
                        Glass.Conversoes.StrParaInt(((TextBox)Master.FindControl("pagina").FindControl("txtAltura" + i)).Text) : 0;
    
                    var larguraAmbiente = (TextBox)Master.FindControl("pagina").FindControl("txtLargura" + i) != null ?
                        Glass.Conversoes.StrParaInt(((TextBox)Master.FindControl("pagina").FindControl("txtLargura" + i)).Text) : 0;
    
                    var idAplicacao = (HiddenField)Master.FindControl("pagina").FindControl("hdfIdAplicacao" + i) != null ?
                        Glass.Conversoes.StrParaUintNullable(((HiddenField)Master.FindControl("pagina").FindControl("hdfIdAplicacao" + i)).Value) : 0;
    
                    var idProcesso = (HiddenField)Master.FindControl("pagina").FindControl("hdfIdProcesso" + i) != null ?
                        Glass.Conversoes.StrParaUintNullable(((HiddenField)Master.FindControl("pagina").FindControl("hdfIdProcesso" + i)).Value) : 0;
    
                    var redondoAmbiente = (CheckBox)Master.FindControl("pagina").FindControl("chkRedondo" + i) != null ?
                        ((CheckBox)Master.FindControl("pagina").FindControl("chkRedondo" + i)).Checked : false;
    
                    var idAmbiente = new uint();
                    
                    // Insere o ambiente
                    if (!pcp)
                    {
                        var novo = new AmbientePedido();
                        novo.IdPedido = idPedido;
                        novo.IdProd = idProdAmbiente;
                        novo.Ambiente = descrAmbiente;
                        novo.Altura = alturaAmbiente;
                        novo.Largura = larguraAmbiente;
                        novo.Qtde = qtdeAmbiente;
                        novo.IdAplicacao = idAplicacao;
                        novo.IdProcesso = idProcesso;
                        novo.Redondo = redondoAmbiente;

                        if (novo.Altura != novo.Largura && redondoAmbiente)
                        {
                            throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");
                        }
    
                        idAmbiente = AmbientePedidoDAO.Instance.InsertComTransacao(novo);
                    }
                    else
                    {
                        var novo = new AmbientePedidoEspelho();
                        novo.IdPedido = idPedido;
                        novo.IdProd = idProdAmbiente;
                        novo.Ambiente = descrAmbiente;
                        novo.Altura = alturaAmbiente;
                        novo.Largura = larguraAmbiente;
                        novo.Qtde = qtdeAmbiente;
                        novo.Redondo = redondoAmbiente;

                        if (novo.Altura != novo.Largura && redondoAmbiente)
                        {
                            throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");
                        }

                        idAmbiente = AmbientePedidoEspelhoDAO.Instance.InsertComTransacao(novo);
                    }
    
                    if (idAmbiente <= 0)
                    {
                        throw new Exception("Ambiente não cadastrado.");
                    }
    
                    // Insere a mão de obra no ambiente
                    if (!pcp)
                    {
                        ProdutosPedido prod = new ProdutosPedido();
                        prod.IdPedido = idPedido;
                        prod.IdAmbientePedido = idAmbiente;
                        prod.IdProd = idProdMaoObra;
                        prod.Qtde = qtdeMaoObra;
                        prod.ValorVendido = valorMaoObra;
    
                        if (tipoCalcMaoObra == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalcMaoObra == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto || 
                            tipoCalcMaoObra == (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro)
                        {
                            prod.Altura = alturaAmbiente;
                            prod.Largura = larguraAmbiente;
                        }

                        prod.Espessura = ProdutoDAO.Instance.ObtemEspessura((int)prod.IdProd);
                        prod.AlturaBenef = alturaBenef;
                        prod.LarguraBenef = larguraBenef;
                        prod.EspessuraBenef = espBenef;
                        prod.IdGrupoProd = (uint)ProdutoDAO.Instance.ObtemIdGrupoProd((int)prod.IdProd);
                        prod.IdSubgrupoProd = (uint)ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)prod.IdProd).GetValueOrDefault(0);

                        ProdutosPedidoDAO.Instance.Insert(prod);
                    }
                    else
                    {
                        ProdutosPedidoEspelho prod = new ProdutosPedidoEspelho();
                        prod.IdPedido = idPedido;
                        prod.IdAmbientePedido = idAmbiente;
                        prod.IdProd = idProdMaoObra;
                        prod.Qtde = qtdeMaoObra * qtdeAmbiente;
                        prod.ValorVendido = valorMaoObra;
    
                        if (tipoCalcMaoObra == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalcMaoObra == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto || 
                            tipoCalcMaoObra == (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro)
                        {
                            prod.Altura = alturaAmbiente;
                            prod.Largura = larguraAmbiente;
                        }

                        prod.Espessura = ProdutoDAO.Instance.ObtemEspessura((int)prod.IdProd);
                        prod.AlturaBenef = alturaBenef;
                        prod.LarguraBenef = larguraBenef;
                        prod.EspessuraBenef = espBenef;
                        prod.IdGrupoProd = (uint)ProdutoDAO.Instance.ObtemIdGrupoProd((int)prod.IdProd);
                        prod.IdSubgrupoProd = (uint)ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)prod.IdProd).GetValueOrDefault(0);

                        ProdutosPedidoEspelhoDAO.Instance.InsertComTransacao(prod);
                    }
                }
    
                // Limpa os controles dos ambientes
                for (var i = 1; i <= 10; i++)
                {
                    if ((TextBox)Master.FindControl("pagina").FindControl("txtCodAmb" + i) != null)
                        ((TextBox)Master.FindControl("pagina").FindControl("txtCodAmb" + i)).Text = "";
    
                    if ((HiddenField)Master.FindControl("pagina").FindControl("hdfAmbIdProd" + i) != null)
                        ((HiddenField)Master.FindControl("pagina").FindControl("hdfAmbIdProd" + i)).Value = "";
    
                    if ((Label)Master.FindControl("pagina").FindControl("lblDescrAmb" + i) != null)
                        ((Label)Master.FindControl("pagina").FindControl("lblDescrAmb" + i)).Text = "";
    
                    if ((HiddenField)Master.FindControl("pagina").FindControl("hdfDescrAmb" + i) != null)
                        ((HiddenField)Master.FindControl("pagina").FindControl("hdfDescrAmb" + i)).Value = "";
    
                    if ((TextBox)Master.FindControl("pagina").FindControl("txtQtde" + i) != null)
                        ((TextBox)Master.FindControl("pagina").FindControl("txtQtde" + i)).Text = "";
    
                    if ((TextBox)Master.FindControl("pagina").FindControl("txtAltura" + i) != null)
                        ((TextBox)Master.FindControl("pagina").FindControl("txtAltura" + i)).Text = "";
    
                    if ((TextBox)Master.FindControl("pagina").FindControl("txtLargura" + i) != null)
                        ((TextBox)Master.FindControl("pagina").FindControl("txtLargura" + i)).Text = "";
    
                    if ((TextBox)Master.FindControl("pagina").FindControl("txtProcIns" + i) != null)
                        ((TextBox)Master.FindControl("pagina").FindControl("txtProcIns" + i)).Text = "";
    
                    if ((HiddenField)Master.FindControl("pagina").FindControl("hdfIdProcesso" + i) != null)
                        ((HiddenField)Master.FindControl("pagina").FindControl("hdfIdProcesso" + i)).Value = "";
    
                    if ((TextBox)Master.FindControl("pagina").FindControl("txtAplIns" + i) != null)
                        ((TextBox)Master.FindControl("pagina").FindControl("txtAplIns" + i)).Text = "";
    
                    if ((HiddenField)Master.FindControl("pagina").FindControl("hdfIdAplicacao" + i) != null)
                        ((HiddenField)Master.FindControl("pagina").FindControl("hdfIdAplicacao" + i)).Value = "";
    
                    if ((CheckBox)Master.FindControl("pagina").FindControl("chkRedondo" + i) != null)
                        ((CheckBox)Master.FindControl("pagina").FindControl("chkRedondo" + i)).Checked = false;
                }
    
                // Atualiza a tela do pedido e exibe a mensagem de sucesso na tela
                ClientScript.RegisterClientScriptBlock(GetType(), "atualizar", "window.opener.redirectUrl(window.opener.location.href);\n", true);
                ClientScript.RegisterStartupScript(GetType(), "foco", "FindControl('txtCodAmb1', 'input').focus();\n", true);
                Glass.MensagemAlerta.ShowMsg("Produtos inseridos com sucesso!", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir produtos no pedido.", ex, Page);
            }
        }
    }
}
