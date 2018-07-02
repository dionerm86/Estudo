using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Collections.Generic;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTrocaDev : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadTrocaDev));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!Config.PossuiPermissao(Config.FuncaoMenuEstoque.EfetuarTrocaDevolucao))
            {
                Response.Redirect("~/WebGlass/Main.aspx");
                return;
            }

            uint idTrocaDevolucao = 0;

            if (Request["popup"] == "1")
                Page.ClientScript.RegisterStartupScript(GetType(), "troca", "hidePopup();", true);

            if (!IsPostBack && Request["idTrocaDev"] == null)
            {
                dtvTroca.ChangeMode(DetailsViewMode.Insert);
                ((Label)dtvTroca.FindControl("lblDataTroca")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                produtos.Visible = false;
            }
            else if (Request["idTrocaDev"] != null)
            {
                idTrocaDevolucao = Glass.Conversoes.StrParaUint(Request["idTrocaDev"]);

                if (!TrocaDevolucaoDAO.Instance.PodeEditar(idTrocaDevolucao))
                {
                    Response.Redirect("../Listas/LstTrocaDev.aspx" +
                        (!String.IsNullOrEmpty(Request["popup"]) ? "?popup=1" : ""));

                    return;
                }

                bool isTroca = TrocaDevolucaoDAO.Instance.IsTroca(idTrocaDevolucao);
                separadorProd.Visible = isTroca;
                produtosNovos.Visible = isTroca;

                if (produtos.Visible)
                {
                    lkbInserir.Visible = TrocaDevolucaoDAO.Instance.TemPedido(idTrocaDevolucao);
                    lkbInserirNovo.Visible = lkbInserir.Visible && TrocaDevolucaoDAO.Instance.ObtemUsarPedidoReposicao(idTrocaDevolucao);

                    grdProdutosTrocados.ShowFooter = !lkbInserir.Visible;
                    grdProdutosNovos.ShowFooter = !lkbInserirNovo.Visible;

                    if (grdProdutosNovos.Rows.Count == 1)
                        grdProdutosNovos.Rows[0].Visible = ProdutoTrocaDevolucaoDAO.Instance.GetCountReal(idTrocaDevolucao) != 0;

                    if (grdProdutosTrocados.Rows.Count == 1)
                        grdProdutosTrocados.Rows[0].Visible = ProdutoTrocadoDAO.Instance.GetCountReal(idTrocaDevolucao) != 0;

                    grdProdutosTrocados.Columns[13].Visible = !TrocaDevolucaoDAO.Instance.ObtemUsarPedidoReposicao(idTrocaDevolucao);
                }
            }
            
            hdfUrlRetorno.Value = "../Listas/LstTrocaDev.aspx" + (Request["popup"] == "1" ? "?popup=1" : "");
        }

        protected string GetSubtitulo()
        {
            if (!String.IsNullOrEmpty(Request["idTrocaDev"]))
            {
                uint idTrocaDevolucao = Glass.Conversoes.StrParaUint(Request["idTrocaDev"]);
                int tipo = TrocaDevolucaoDAO.Instance.ObtemTipo(idTrocaDevolucao); // Sempre considera o tipo da troca/devolução
                return tipo == 1 ? "Trocados" : "Devolvidos";
            }

            return String.Empty;
        }

        protected void odsTroca_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                var msgErro = MensagemAlerta.FormatErrorMsg("Falha ao inserir troca/devolução.", e.Exception);
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "err", string.Format("alert('{0}'); window.location.href = window.location.href;", msgErro), true);
            }
            else
                Response.Redirect("~/Cadastros/CadTrocaDev.aspx?idTrocaDev=" + e.ReturnValue.ToString() +
                    (!String.IsNullOrEmpty(Request["popup"]) ? "&popup=1" : ""));
        }

        protected void odsTroca_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar troca/devolução.", e.Exception, Page);
            }
            else
                Response.Redirect("~/Cadastros/CadTrocaDev.aspx?idTrocaDev=" + Request["idTrocaDev"] +
                    (!String.IsNullOrEmpty(Request["popup"]) ? "&popup=1" : ""));
        }

        protected void dtvTroca_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Finalizar")
            {
                uint idTrocaDevolucao = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                if (ProdutoTrocadoDAO.Instance.GetCountReal(idTrocaDevolucao) == 0)
                {
                    Glass.MensagemAlerta.ShowMsg("Cadastre pelo menos um produto para ser trocado/devolvido antes de finalizar.", Page);
                    return;
                }
                else if (TrocaDevolucaoDAO.Instance.IsTroca(idTrocaDevolucao) && ProdutoTrocaDevolucaoDAO.Instance.GetCountReal(idTrocaDevolucao) == 0)
                {
                    Glass.MensagemAlerta.ShowMsg("Cadastre pelo menos um produto para troca/devolução antes de finalizar.", Page);
                    return;
                }

                try
                {
                    string mensagem = "";

                    // Se o funcionário for Caixa Diário, ou tiver permissão de caixa diário
                    bool isCaixaDiario = UserInfo.GetUserInfo.IsCaixaDiario;

                    // Se o funcionário for Financeiro
                    bool isCaixaGeral = UserInfo.GetUserInfo.IsFinanceiroReceb;

                    // Se o funcionário não tiver permissão de financeiro, gera conta a receber direto
                    if (!TrocaDevolucaoDAO.Instance.TemValorExcedente(idTrocaDevolucao) || (!isCaixaDiario && !isCaixaGeral))
                        mensagem = TrocaDevolucaoDAO.Instance.Finalizar(idTrocaDevolucao, false);

                    // Se o funcionário tiver permissão de financeiro, permite que o mesmo receba o valor da troca ou gere conta a receber
                    else
                    {
                        dtvTroca.Fields[1].Visible = false;
                        produtos.Visible = false;
                        pagamento.Visible = true;
                    }

                    if (mensagem.Length > 0)
                    {
                        mensagem = @"
                            alert('" + mensagem + @"'); 
                            openWindow(600, 800, '../Relatorios/RelBase.aspx?rel=TrocaDevolucao&idTrocaDevolucao=" + idTrocaDevolucao + @"');
                            redirectUrl('../Listas/LstTrocaDev.aspx');";

                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "Finalizar", mensagem, true);
                    }
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar troca/devolução.", ex, Page);
                }
            }
            else
            {
                produtos.Visible = e.CommandName != "Edit";
            }
        }

        protected void btnGerarContaRec_Click(object sender, EventArgs e)
        {
            try
            {
                uint idTrocaDevolucao = Glass.Conversoes.StrParaUint(Request["idTrocaDev"]);

                string mensagem = "";

                if (!TrocaDevolucaoDAO.Instance.TemValorExcedente(idTrocaDevolucao))
                {
                    Glass.MensagemAlerta.ShowMsg("Esta troca não possui valor excedente.", Page);
                    return;
                }

                mensagem = TrocaDevolucaoDAO.Instance.Finalizar(idTrocaDevolucao, false);

                if (mensagem.Length > 0)
                {
                    mensagem = @"
                        alert('" + mensagem + @"'); 
                        openWindow(600, 800, '../Relatorios/RelBase.aspx?rel=TrocaDevolucao&idTrocaDevolucao=" + idTrocaDevolucao + @"'); 
                        redirectUrl('../Listas/LstTrocaDev.aspx" +
                        (!String.IsNullOrEmpty(Request["popup"]) ? "?popup=1" : "") + "');";

                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Finalizar", mensagem, true);
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar troca/devolução.", ex, Page);
            }
        }

        protected void odsProdutosTrocados_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            AtualizarItens();
        }

        protected void odsProdutosTrocados_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            AtualizarItens();
        }

        protected void odsProdutosTrocados_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar produto trocado.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                AtualizarItens();
        }

        protected void odsProdTroca_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            AtualizarItens();
        }

        protected void odsProdTroca_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            AtualizarItens();
        }

        protected void odsProdTroca_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            AtualizarItens();
        }

        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutosNovos.ShowFooter = !lkbInserirNovo.Visible && e.CommandName != "Edit";
        }

        protected void grdProdutosTrocados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutosTrocados.ShowFooter = !lkbInserir.Visible && e.CommandName != "Edit";
        }

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string GetValorMinimo(string codInterno, string tipoEntrega, string idCliente, string revenda, string idStr, string percDescontoQtdeStr, string tipo, string idPedido)
        {
            float percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
            uint id;

            if (uint.TryParse(idStr, out id))
            {
                ProdutoDAO.TipoBuscaValorMinimo tipoBusca = tipo != "Troca_" ? ProdutoDAO.TipoBuscaValorMinimo.ProdutoTrocaDevolucao : ProdutoDAO.TipoBuscaValorMinimo.ProdutoTrocado;
                return ProdutoDAO.Instance.GetValorMinimo(id, tipoBusca, revenda.ToLower() == "true", percDescontoQtde, idPedido.StrParaIntNullable(), null, null).ToString();
            }
            else
            {
                Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

                // Recupera o valor mínimo do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                return ProdutoDAO.Instance.GetValorMinimo(prod.IdProd, tipoEntr, idCli, revenda == "true", false, percDescontoQtde, idPedido.StrParaIntNullable(), null, null).ToString();
            }
        }

        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno, string tipoEntrega, string revenda, string idCliente, string percDescontoQtdeStr, string idLoja, string idPedido)
        {
            try
            {
                Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno, Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaUintNullable(idCliente), null, true);

                if (prod == null)
                    return "Erro;Não existe produto com o código informado.";
                else if (prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
                else if (prod.Compra)
                    return "Erro;Produto utilizado apenas na compra.";

                if (PedidoConfig.DadosPedido.BloqueioPedidoMaoDeObra && prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                    return "Erro;Produtos do grupo 'Mão de Obra Beneficiamento' estão bloqueados para pedidos comuns.";

                string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;
                decimal valorProduto = 0;

                // Recupera o valor de tabela do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                float percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
                valorProduto = ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, bool.Parse(revenda), false, percDescontoQtde, idPedido.StrParaIntNullable(), null, null);

                if (PedidoConfig.Comissao.ComissaoPedido && PedidoConfig.Comissao.ComissaoAlteraValor)
                {
                    var percentualComissao = PedidoDAO.Instance.ObterPercentualComissao(null, idPedido.StrParaInt());

                    valorProduto = percentualComissao > 0 ? valorProduto / (decimal)((100 - percentualComissao) / 100) : valorProduto;
                }

                retorno += ";" + valorProduto.ToString("F2");

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0");

                bool bloquearEstoque = GrupoProdDAO.Instance.BloquearEstoque(prod.IdGrupoProd, prod.IdSubgrupoProd);
                retorno += ";" + (bloquearEstoque ? ProdutoLojaDAO.Instance.GetEstoque(null, UserInfo.GetUserInfo.IdLoja, (uint)prod.IdProd).ToString() : "100000");

                // Verifica como deve ser feito o cálculo do produto
                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(prod.IdGrupoProd, prod.IdSubgrupoProd);

                // Retorna a espessura do produto
                retorno += ";" + prod.Espessura;

                // Retorna a alíquota ICMS do produto
                retorno += ";" + prod.AliqICMSInterna.ToString().Replace(',', '.');

                //if (isPedidoProducao)
                retorno += ";" + (prod.Altura != null ? prod.Altura.Value.ToString() : "") + ";" + (prod.Largura != null ? prod.Largura.Value.ToString() : "");

                retorno += ";" + prod.IdCorVidro + ";" + prod.CustoCompra;

                return retorno;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao inserir produto para troca.", ex);
            }

        }

        [Ajax.AjaxMethod]
        public string GetDadosPedido(string idPedido, string idTrocaDev)
        {
            try
            {
                Glass.Data.Model.Pedido.SituacaoPedido situacao = PedidoDAO.Instance.ObtemSituacao(Glass.Conversoes.StrParaUint(idPedido));

                if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Confirmado && situacao != Glass.Data.Model.Pedido.SituacaoPedido.LiberadoParcialmente)
                    return "Erro;A troca/devolução só pode ser feita se o pedido estiver " + (PedidoConfig.LiberarPedido ? "liberado" : "confirmado") + ".";

                bool pedidosReposicao = PedidoDAO.Instance.IsPedidoReposto(Glass.Conversoes.StrParaUint(idPedido)) &&
                    PedidoReposicaoDAO.Instance.PedidoParaTroca(PedidoDAO.Instance.IdReposicao(Glass.Conversoes.StrParaUint(idPedido)).GetValueOrDefault());

                string trocasString = TrocaDevolucaoDAO.Instance.ObtemIdTrocaDevPorPedido(Glass.Conversoes.StrParaUint(idPedido));
                var trocas = new List<string>((trocasString ?? String.Empty).Split(','));

                for (int i = trocas.Count - 1; i >= 0; i--)
                    if (String.IsNullOrEmpty(trocas[i]) || trocas[i] == idTrocaDev)
                        trocas.RemoveAt(i);

                trocasString = String.Join(", ", trocas.ToArray());

                return "Ok;" + PedidoDAO.Instance.ObtemIdCliente(Glass.Conversoes.StrParaUint(idPedido)) + ";" +
                    trocasString + ";" +
                    pedidosReposicao.ToString().ToLower();
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar cliente do pedido.", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string AddProdutoTroca(string idTrocaDevStr, string idProdPedStr, string qtdeStr, string etiquetas)
        {
            try
            {
                var idTrocaDev = idTrocaDevStr.StrParaInt();
                var idProdPed = idProdPedStr.StrParaInt();
                var qtde = qtdeStr.StrParaDecimal();

                ProdutoTrocadoDAO.Instance.InsertFromPedidoComTransacao(idTrocaDev, idProdPed, qtde, etiquetas);

                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao inserir produto para troca.", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string AddProdutoNovo(string idTrocaDevStr, string idProdPedStr, string qtdeStr)
        {
            try
            {
                var idTrocaDev = idTrocaDevStr.StrParaInt();
                var idProdPed = idProdPedStr.StrParaInt();
                var qtde = qtdeStr.StrParaDecimal();

                ProdutoTrocaDevolucaoDAO.Instance.InsertFromPedido(idTrocaDev, idProdPed, qtde);

                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao inserir produto para troca.", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string Finalizar(string idTrocaDevStr, string valoresRecebStr, string formasPagtoStr, string contasBancoStr, string depositoNaoIdentificado, 
            string cartaoNaoIdentificado, string tiposCartaoStr, string tiposBoletoStr, string txAntecipStr, string jurosStr, string recebParcial, 
            string gerarCredito, string creditoUtilizado, string numAutConstrucard, string numParcCartoesStr, string chequesPagto, string numAutCartao)
        {
            try
            {
                uint idTrocaDevolucao = Glass.Conversoes.StrParaUint(idTrocaDevStr);

                string[] valores = valoresRecebStr.Split(';');
                string[] fPagtos = formasPagtoStr.Split(';');
                string[] cBancos = contasBancoStr.Split(';');
                string[] tCartoes = tiposCartaoStr.Split(';');
                string[] tBoletos = tiposBoletoStr.Split(';');
                string[] tAntecip = txAntecipStr.Split(';');
                string[] parcCartoes = numParcCartoesStr.Split(';');
                string[] sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
                string[] sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(';');
                string[] sNumAutCartao = numAutCartao.Split(';');

                decimal[] valoresReceb = new decimal[valores.Length];
                uint[] formasPagto = new uint[fPagtos.Length];
                uint[] contasBanco = new uint[cBancos.Length];
                uint[] tiposCartao = new uint[tCartoes.Length];
                uint[] tiposBoleto = new uint[tBoletos.Length];
                decimal[] txAntecip = new decimal[tAntecip.Length];
                uint[] numParcCartoes = new uint[parcCartoes.Length];
                uint[] depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
                uint[] cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];

                for (int i = 0; i < valores.Length; i++)
                {
                    valoresReceb[i] = !string.IsNullOrEmpty(valores[i]) ? decimal.Parse(valores[i].Replace(".", ",")) : 0;
                    formasPagto[i] = !string.IsNullOrEmpty(fPagtos[i]) ? Conversoes.StrParaUint(fPagtos[i]) : 0;
                    contasBanco[i] = !string.IsNullOrEmpty(cBancos[i]) ? Conversoes.StrParaUint(cBancos[i]) : 0;
                    tiposCartao[i] = !string.IsNullOrEmpty(tCartoes[i]) ? Conversoes.StrParaUint(tCartoes[i]) : 0;
                    tiposBoleto[i] = !string.IsNullOrEmpty(tBoletos[i]) ? Conversoes.StrParaUint(tBoletos[i]) : 0;
                    txAntecip[i] = !string.IsNullOrEmpty(tAntecip[i]) ? decimal.Parse(tAntecip[i].Replace(".", ",")) : 0;
                    numParcCartoes[i] = !string.IsNullOrEmpty(parcCartoes[i]) ? Conversoes.StrParaUint(parcCartoes[i]) : 0;
                    depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i]) ? Convert.ToUInt32(sDepositoNaoIdentificado[i]) : 0;
                }

                for (int i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? Convert.ToUInt32(sCartaoNaoIdentificado[i]) : 0;
                }

                decimal juros = Conversoes.StrParaDecimal(jurosStr);
                decimal creditoUtil = Conversoes.StrParaDecimal(creditoUtilizado);

                var retorno = TrocaDevolucaoDAO.Instance.Finalizar(idTrocaDevolucao, valoresReceb, formasPagto, contasBanco, depNaoIdentificado, cartNaoIdentificado, tiposCartao, tiposBoleto, txAntecip,
                    juros, recebParcial == "true", gerarCredito == "true", creditoUtil, numAutConstrucard, false, numParcCartoes, chequesPagto, sNumAutCartao);

                var urlRedirect = "../Listas/LstTrocaDev.aspx";

                return "Ok;" + retorno + ";" + urlRedirect;
            }
            catch (Exception ex)
            {
                return "Erro;" + MensagemAlerta.FormatErrorMsg("Falha ao finalizar troca/devolução.", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string BloquearValorMin(string tipo)
        {
            return tipo != "Novo_" ? "true" : "false";
        }

        #endregion

        #region Beneficiamentos

        protected void txtEspessura_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            string tipo = txt.ID.IndexOf("Troca_") > -1 ? "Troca_" : "Novo_";

            if (tipo != "Troca_")
            {
                ProdutoTrocaDevolucao prodTroca = linhaControle.DataItem as ProdutoTrocaDevolucao;
                txt.Enabled = prodTroca.Espessura <= 0;
            }
            else
            {
                ProdutoTrocado prodTroca = linhaControle.DataItem as ProdutoTrocado;
                txt.Enabled = prodTroca.Espessura <= 0;
            }
        }

        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            string tipo = benef.ID.Contains("Troca_") ? "Troca_" : "Novo_";
            string tipoCampo = tipo == "Troca_" && lkbInserir.Visible ? "lbl" : "txt";
            benef.ValidarEspessura = tipoCampo != "lbl";

            Control codProd = linhaControle.FindControl(tipo + tipoCampo + "CodProdIns");
            Control txtAltura = linhaControle.FindControl(tipo + tipoCampo + "AlturaIns");
            Control txtEspessura = linhaControle.FindControl(tipo + "txtEspessura");
            Control txtLargura = linhaControle.FindControl(tipo + tipoCampo + "LarguraIns");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl(tipo + "txtQtdeIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvTroca.FindControl("hdfTipoEntrega");
            Label lblTotalM2 = Beneficiamentos.UsarM2CalcBeneficiamentos ?
                (Label)linhaControle.FindControl(tipo + "lblTotM2CalcIns") :
                (Label)linhaControle.FindControl(tipo + "lblTotM2Ins");
            Control txtValorIns = linhaControle.FindControl(tipo + tipoCampo + "ValorIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvTroca.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvTroca.FindControl("hdfIdCliente");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl(tipo + "hdfCustoProd");

            benef.CampoAltura = txtAltura;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoQuantidade = txtQuantidade;
            benef.CampoTipoEntrega = hdfTipoEntrega;
            benef.CampoTotalM2 = lblTotalM2;
            benef.CampoCusto = hdfCustoProd;
            benef.CampoValorUnitario = txtValorIns;
            benef.CampoProdutoID = codProd;
            benef.CampoRevenda = hdfCliRevenda;
            benef.CampoClienteID = hdfIdCliente;
            benef.CampoAplicacaoID = linhaControle.FindControl(tipo + "hdfIdAplicacao");
            benef.CampoProcessoID = linhaControle.FindControl(tipo + "hdfIdProcesso");
            benef.CampoAplicacao = linhaControle.FindControl(tipo + "txtAplIns");
            benef.CampoProcesso = linhaControle.FindControl(tipo + "txtProcIns");
        }

        #endregion

        private void AtualizarItens()
        {
            uint idTrocaDevolucao;
            if (!uint.TryParse(Request["idTrocaDev"], out idTrocaDevolucao))
                return;

            TrocaDevolucaoDAO.Instance.UpdateTotaisTrocaDev(idTrocaDevolucao);

            Response.Redirect("CadTrocaDev.aspx?idTrocaDev=" + idTrocaDevolucao +
                (!String.IsNullOrEmpty(Request["popup"]) ? "&popup=1" : ""), true);
        }

        protected void ctrlFormaPagto_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlFormaPagto fp = (Glass.UI.Web.Controls.ctrlFormaPagto)sender;
            fp.CampoCredito = (HiddenField)dtvTroca.FindControl("hdfCreditoCliente");
            fp.CampoValorConta = (HiddenField)dtvTroca.FindControl("hdfValorExcedente");
            fp.CampoClienteID = dtvTroca.FindControl("hdfIdCliente");
        }

        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            string tipo = ((WebControl)sender).ID.IndexOf("Troca_") > -1 ? "Troca_" : "Novo_";

            uint idTrocaDevolucao = Glass.Conversoes.StrParaUint(Request["idTrocaDev"]);
            GridViewRow linha = tipo != "Troca_" ? grdProdutosNovos.FooterRow : grdProdutosTrocados.FooterRow;
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)linha.FindControl(tipo + "ctrlBenefInserir");
            Glass.UI.Web.Controls.ctrlDescontoQtde desc = (Glass.UI.Web.Controls.ctrlDescontoQtde)linha.FindControl(tipo + "ctrlDescontoQtde");

            string idProd = ((HiddenField)linha.FindControl(tipo + "hdfIdProd")).Value;
            string idProcesso = ((HiddenField)linha.FindControl(tipo + "hdfIdProcesso")).Value;
            string idAplicacao = ((HiddenField)linha.FindControl(tipo + "hdfIdAplicacao")).Value;
            string qtde = ((TextBox)linha.FindControl(tipo + "txtQtdeIns")).Text;
            string valorVendido = ((TextBox)linha.FindControl(tipo + "txtValorIns")).Text;
            string altura = ((TextBox)linha.FindControl(tipo + "txtAlturaIns")).Text;
            string alturaReal = ((HiddenField)linha.FindControl(tipo + "hdfAlturaRealIns")).Value;
            string largura = ((TextBox)linha.FindControl(tipo + "txtLarguraIns")).Text;
            string totM = ((Label)linha.FindControl(tipo + "lblTotM2Ins")).Text;
            string totM2Calc = ((Label)linha.FindControl(tipo + "lblTotM2CalcIns")).Text;
            string espessura = ((TextBox)linha.FindControl(tipo + "txtEspessura")).Text;
            bool alterarEstoque = ((CheckBox)linha.FindControl(tipo + "chkAlterarEstoque")).Checked;
            bool comDefeito = tipo == "Troca_" ? ((CheckBox)linha.FindControl(tipo + "chkComDefeito")).Checked : false;

            if (tipo != "Troca_")
            {
                ProdutoTrocaDevolucao novo = new ProdutoTrocaDevolucao();
                novo.IdTrocaDevolucao = idTrocaDevolucao;
                novo.IdProd = !String.IsNullOrEmpty(idProd) ? Glass.Conversoes.StrParaUint(idProd) : 0;
                novo.IdProcesso = !String.IsNullOrEmpty(idProcesso) ? (uint?)Glass.Conversoes.StrParaUint(idProcesso) : null;
                novo.IdAplicacao = !String.IsNullOrEmpty(idAplicacao) ? (uint?)Glass.Conversoes.StrParaUint(idAplicacao) : null;
                novo.Qtde = !String.IsNullOrEmpty(qtde) ? float.Parse(qtde.Replace('.', ',')) : 0;
                novo.ValorVendido = Glass.Conversoes.StrParaDecimal(valorVendido);
                novo.Altura = !String.IsNullOrEmpty(altura) ? float.Parse(altura) : 0;
                novo.AlturaReal = !String.IsNullOrEmpty(alturaReal) ? float.Parse(alturaReal) : 0;
                novo.Largura = !String.IsNullOrEmpty(largura) ? Glass.Conversoes.StrParaInt(largura) : 0;
                novo.TotM = !String.IsNullOrEmpty(totM) ? float.Parse(totM) : 0;
                novo.TotM2Calc = !String.IsNullOrEmpty(totM2Calc) ? float.Parse(totM2Calc) : 0;
                novo.Espessura = !String.IsNullOrEmpty(espessura) ? Glass.Conversoes.StrParaFloat(espessura) : 0;
                novo.Redondo = benef.Redondo;
                novo.AlterarEstoque = alterarEstoque;
                novo.Beneficiamentos = benef.Beneficiamentos;
                novo.PercDescontoQtde = desc.PercDescontoQtde;

                ProdutoTrocaDevolucaoDAO.Instance.Insert(novo);
            }
            else
            {
                ProdutoTrocado novo = new ProdutoTrocado();
                novo.IdTrocaDevolucao = idTrocaDevolucao;
                novo.IdProd = !String.IsNullOrEmpty(idProd) ? Glass.Conversoes.StrParaUint(idProd) : 0;
                novo.IdProcesso = !String.IsNullOrEmpty(idProcesso) ? (uint?)Glass.Conversoes.StrParaUint(idProcesso) : null;
                novo.IdAplicacao = !String.IsNullOrEmpty(idAplicacao) ? (uint?)Glass.Conversoes.StrParaUint(idAplicacao) : null;
                novo.Qtde = !String.IsNullOrEmpty(qtde) ? float.Parse(qtde.Replace('.', ',')) : 0;
                novo.ValorVendido = Glass.Conversoes.StrParaDecimal(valorVendido);
                novo.Altura = !String.IsNullOrEmpty(altura) ? float.Parse(altura) : 0;
                novo.AlturaReal = !String.IsNullOrEmpty(alturaReal) ? float.Parse(alturaReal) : 0;
                novo.Largura = !String.IsNullOrEmpty(largura) ? Glass.Conversoes.StrParaInt(largura) : 0;
                novo.TotM = !String.IsNullOrEmpty(totM) ? float.Parse(totM) : 0;
                novo.TotM2Calc = !String.IsNullOrEmpty(totM2Calc) ? float.Parse(totM2Calc) : 0;
                novo.Espessura = !String.IsNullOrEmpty(espessura) ? Glass.Conversoes.StrParaFloat(espessura) : 0;
                novo.Redondo = benef.Redondo;
                novo.AlterarEstoque = alterarEstoque;
                novo.ComDefeito = comDefeito;
                novo.Beneficiamentos = benef.Beneficiamentos;
                novo.PercDescontoQtde = desc.PercDescontoQtde;

                ProdutoTrocadoDAO.Instance.Insert(novo);
            }

            AtualizarItens();
        }

        protected void ctrlDescontoQtde_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlDescontoQtde desc = (Glass.UI.Web.Controls.ctrlDescontoQtde)sender;
            GridViewRow linha = desc.Parent.Parent as GridViewRow;

            string tipo = desc.ID.IndexOf("Troca_") > -1 ? "Troca_" : "Novo_";

            desc.CampoQtde = linha.FindControl(tipo + "txtQtdeIns");
            desc.CampoProdutoID = linha.FindControl(tipo + "hdfIdProd");
            desc.CampoClienteID = dtvTroca.FindControl("hdfIdCliente");
            desc.CampoTipoEntrega = dtvTroca.FindControl("hdfTipoEntrega");
            desc.CampoRevenda = dtvTroca.FindControl("hdfCliRevenda");
            desc.CampoValorUnit = tipo == "Troca_" ? linha.FindControl(tipo + "lblValorIns") : linha.FindControl(tipo + "txtValorIns");
        }

        protected void drpFunc_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack && Request["idTrocaDev"] == null)
                ((DropDownList)sender).SelectedValue = UserInfo.GetUserInfo.CodUser.ToString();

            ((DropDownList)sender).Enabled = EstoqueConfig.PermitirAlterarFuncionarioTrocaDevolucao ||
                UserInfo.GetUserInfo.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Administrador;
        }

        protected string NomeControleBenefTrocado()
        {
            return grdProdutosTrocados.EditIndex == -1 ? "Troca_ctrlBenefInserir" : "Troca_ctrlBenefEditar";
        }

        protected string NomeControleBenefNovo()
        {
            return grdProdutosNovos.EditIndex == -1 ? "Novo_ctrlBenefInserir" : "Novo_ctrlBenefEditar";
        }

        protected void ddlOrigem_Load(object sender, EventArgs e)
        {
            ((WebControl)sender).Visible = OrigemTrocaDescontoDAO.Instance.GetList().Length > 0;
        }
    }
}
