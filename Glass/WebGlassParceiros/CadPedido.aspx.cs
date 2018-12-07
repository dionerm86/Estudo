using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class CadPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserInfo.GetUserInfo.IdCliente == null || UserInfo.GetUserInfo.IdCliente == 0 ||
                (Request["idPedido"] != null && UserInfo.GetUserInfo.IdCliente != PedidoDAO.Instance.GetIdCliente(null, Glass.Conversoes.StrParaUint(Request["idPedido"]))))
            {
                Response.Redirect("~/LstPedidos.aspx");
                return;
            }

            Ajax.Utility.RegisterTypeForAjax(typeof(WebGlassParceiros.CadPedido));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            bool isMaoDeObra = IsPedidoMaoDeObra();
            bool isProducao = IsPedidoProducao();
            bool isRevenda = IsPedidoRevenda();

            // Indica se o pedido é um pedido de mão de obra ou de produção
            if (!String.IsNullOrEmpty(Request["idPedido"]))
            {
                hdfPedidoMaoDeObra.Value = isMaoDeObra.ToString().ToLower();
                hdfPedidoProducao.Value = isProducao.ToString().ToLower();
            }
            else
            {
                hdfPedidoMaoDeObra.Value = (Request["maoObra"] == "1").ToString().ToLower();
                hdfPedidoProducao.Value = (Request["producao"] == "1").ToString().ToLower();
            }

            if (isMaoDeObra)
            {
                grdAmbiente.Columns[1].HeaderText = "Peça de vidro";
                grdAmbiente.Columns[2].Visible = false;
                grdAmbiente.Columns[3].Visible = true;
                grdAmbiente.Columns[4].Visible = true;
                grdAmbiente.Columns[5].Visible = true;
                grdAmbiente.Columns[6].Visible = true;
                grdAmbiente.Columns[7].Visible = true;
                grdAmbiente.Columns[8].Visible = true;

                grdProdutos.Columns[9].Visible = false;
                grdProdutos.Columns[10].Visible = false;

                inserirMaoObra.Visible = true;
                lbkInserirMaoObra.OnClientClick = "openWindow(500, 700, \"../Utils/SetProdMaoObra.aspx?idPedido=" + Request["idPedido"] + "\"); return false";
            }

            if (!OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento)
            {
                grdAmbiente.Columns[9].Visible = isMaoDeObra;
                grdAmbiente.Columns[10].Visible = false;
            }

            // Indica se os produtos devem ser bloqueados de acordo com o tipo de pedido
            hdfBloquearMaoDeObra.Value = PedidoConfig.DadosPedido.BloqueioPedidoMaoDeObra.ToString().ToLower();

            // Se a empresa não possuir acesso ao módulo PCP, esconde colunas Apl e Proc
            if (!Geral.ControlePCP)
            {
                grdProdutos.Columns[9].Visible = false;
                grdProdutos.Columns[10].Visible = false;
            }

            if (!IsPostBack && Request["idPedido"] != null)
            {
                // Se este pedido não puder ser editado, volta para lista de pedidos
                if (!(PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idPedido"])).EditVisible))
                {
                    Response.Redirect("../Listas/LstPedidos.aspx");
                    return;
                }
            }

            if (dtvPedido.CurrentMode == DetailsViewMode.Insert)
            {
                if (Request["idPedido"] == null)
                {
                    string dataPedido = DateTime.Now.ToString("dd/MM/yyyy");
                    if (dtvPedido.FindControl("txtDataPed") != null)
                        ((TextBox)dtvPedido.FindControl("txtDataPed")).Text = dataPedido;

                    if (dtvPedido.FindControl("hdfDataPedido") != null)
                        ((HiddenField)dtvPedido.FindControl("hdfDataPedido")).Value = dataPedido;

                    LoginUsuario login = UserInfo.GetUserInfo;
                    ((DropDownList)dtvPedido.FindControl("drpVendedorIns")).SelectedValue = login.CodUser.ToString();
                }
                else
                {
                    hdfIdPedido.Value = Request["idPedido"];
                    dtvPedido.ChangeMode(DetailsViewMode.ReadOnly);
                }
            }

            hdfComissaoVisible.Value = PedidoConfig.Comissao.ComissaoPedido.ToString().ToLower();
            hdfMedidorVisible.Value = Geral.ControleMedicao.ToString().ToLower();
            divProduto.Visible = dtvPedido.CurrentMode == DetailsViewMode.ReadOnly;

            if (Geral.NaoVendeVidro())
            {
                grdProdutos.Columns[grdProdutos.Columns.Count - 2].Visible = false;
                grdProdutos.Columns[grdProdutos.Columns.Count - 3].Visible = false;
            }

            if (!IsPostBack)
                hdfNaoVendeVidro.Value = Glass.Configuracoes.Geral.NaoVendeVidro().ToString().ToLower();

            // Mostra a opção de inserir projeto apenas se for pedido pedido de venda, se a empresa tiver opção de usar projeto
            // e se o pedido tiver com opção readonly
            if (!IsPostBack)
            {
                uint? idObra = Request["idPedido"] != null ? PedidoDAO.Instance.GetIdObra(null, Glass.Conversoes.StrParaUint(Request["idPedido"])) : null;
                lnkProjeto.Visible = !isMaoDeObra && !isRevenda &&
                    (idObra == null || idObra == 0) && !isProducao && dtvPedido.CurrentMode == DetailsViewMode.ReadOnly;
            }

            grdAmbiente.ShowFooter = false;
        }

        protected void grdProdutos_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            dtvPedido.DataBind();
            grdAmbiente.DataBind();
        }

        protected void grdProdutos_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            dtvPedido.DataBind();
            grdAmbiente.DataBind();
        }

        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutos.ShowFooter = e.CommandName != "Edit";
        }

        protected void grdProdutos_PreRender(object sender, EventArgs e)
        {
            string ambiente = hdfIdAmbiente.Value;

            // Se não houver nenhum produto cadastrado no pedido (e no ambiente passado)
            if (grdProdutos.Rows.Count > 0 && ProdutosPedidoDAO.Instance.CountInPedidoAmbiente(Glass.Conversoes.StrParaUint(Request["idPedido"]), !String.IsNullOrEmpty(ambiente) ? Glass.Conversoes.StrParaUint(ambiente) : 0) == 0)
                grdProdutos.Rows[0].Visible = false;
        }

        protected void ambMaoObra_Load(object sender, EventArgs e)
        {
            ((HtmlControl)sender).Visible = IsPedidoMaoDeObra();
        }

        protected void txtAmbiente_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Visible = !IsPedidoMaoDeObra();
        }

        #region Eventos DataSource

        protected void odsPedido_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Pedido.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                hdfIdPedido.Value = e.ReturnValue.ToString();
                Response.Redirect("CadPedido.aspx?IdPedido=" + hdfIdPedido.Value);
            }
        }

        protected void odsPedido_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                //dtvPedido.ChangeMode(DetailsViewMode.ReadOnly);
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do Pedido.", e.Exception, Page);
            }
            else
            {
                bool alterarProjeto = bool.Parse(hdfAlterarProjeto.Value);
                if (alterarProjeto)
                {
                    uint idPedido = Glass.Conversoes.StrParaUint(hdfIdPedido.Value);
                    ProjetoDAO.Instance.AtualizarClienteByPedido(idPedido);
                }

                Response.Redirect("CadPedido.aspx?IdPedido=" + hdfIdPedido.Value);
            }
        }

        protected void odsProdXPed_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }

            dtvPedido.DataBind();
        }

        protected void odsProdXPed_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }

            dtvPedido.DataBind();
        }

        #endregion

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string IsObraCliente(string idObra, string idCliente)
        {
            return (ObraDAO.Instance.GetNomeCliente(Glass.Conversoes.StrParaUint(idObra), false) == ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCliente))).ToString();
        }

        [Ajax.AjaxMethod]
        public string IsProdutoObra(string idPedido, string codInterno, bool isComposicao)
        {
            uint? idObra = PedidoDAO.Instance.GetIdObra(null, Glass.Conversoes.StrParaUint(idPedido));
            if (idObra > 0 && !isComposicao)
            {
                ProdutoObra prod = ProdutoObraDAO.Instance.GetByCodInterno(idObra.Value, codInterno);
                if (prod == null)
                    return "Erro;Esse produto não está cadastrado no pagamento antecipado.";

                float tamanhoProdutos = ProdutosPedidoDAO.Instance.TotalMedidasObra(idObra.Value, codInterno, null);
                if (prod.TamanhoMaximo <= tamanhoProdutos)
                    return "Erro;Esse produto já foi utilizado totalmente para a obra.";

                return "Ok;" + prod.ValorUnitario + ";" + (prod.TamanhoMaximo - tamanhoProdutos) + ";" + PedidoConfig.DadosPedido.AlterarValorUnitarioProduto.ToString().ToLower();
            }

            return "Ok;0;0;" + PedidoConfig.DadosPedido.AlterarValorUnitarioProduto.ToString().ToLower();
        }

        [Ajax.AjaxMethod]
        public string GetTamanhoMaximoProduto(string idPedido, string codInterno, string totM2Produto)
        {
            uint? idObra = PedidoDAO.Instance.GetIdObra(null, Glass.Conversoes.StrParaUint(idPedido));
            if (idObra > 0)
            {
                ProdutoObra prod = ProdutoObraDAO.Instance.GetByCodInterno(idObra.Value, codInterno);
                if (prod == null)
                    return "Erro;Esse produto não está cadastrado no pagamento antecipado.";

                float tamanhoProdutos = ProdutosPedidoDAO.Instance.TotalMedidasObra(idObra.Value, codInterno, null);
                float tamanhoProduto = float.Parse(totM2Produto.Replace(".", ","));

                return "Ok;" + (prod.TamanhoMaximo - tamanhoProdutos + tamanhoProduto);
            }

            return "Ok;0";
        }

        /// <summary>
        /// Retorna o código que reprensenta a forma de pagamento "Cartao"
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCartaoCod()
        {
            return ((int)Glass.Data.Model.Pagto.FormaPagto.Cartao).ToString();
        }

        /// <summary>
        /// Retorna o código que reprensenta a forma de pagamento "Cheque"
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetChequeCod()
        {
            return ((int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio).ToString();
        }

        /// <summary>
        /// Retorna o código que reprensenta a forma de pagamento "Dinheiro"
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetDinheiroCod()
        {
            return ((int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro).ToString();
        }

        [Ajax.AjaxMethod]
        public string GetValorMinimo(string codInterno, string tipoEntrega, string idCliente, string revenda,
            string reposicao, string idProdPedStr, string percDescontoQtdeStr, string alturaStr)
        {
            float percDescontoQtde = !string.IsNullOrWhiteSpace(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
            float altura = !string.IsNullOrWhiteSpace(alturaStr) ? float.Parse(alturaStr.Replace(".", ",")) : 0;
            uint idProdPed;

            if (uint.TryParse(idProdPedStr, out idProdPed))
            {
                return ProdutoDAO.Instance.GetValorMinimo(idProdPed, ProdutoDAO.TipoBuscaValorMinimo.ProdutoPedido,
                    revenda.ToLower() == "true", percDescontoQtde, Conversoes.StrParaInt(Request["idPedido"]), null, null, altura).ToString();
            }
            else
            {
                Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

                // Recupera o valor mínimo do produto
                int? tipoEntr = !string.IsNullOrWhiteSpace(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !string.IsNullOrWhiteSpace(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                return ProdutoDAO.Instance.GetValorMinimo(prod.IdProd, tipoEntr, idCli, revenda.ToLower() == "true",
                    reposicao.ToLower() == "true", percDescontoQtde, Conversoes.StrParaInt(Request["idPedido"]), null, null, altura).ToString();
            }
        }

        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno, string tipoEntrega, string revenda, string reposicao, string idCliente, string percComissao,
            string pedidoMaoObra, string pedidoProducao, string ambienteMaoObra, string percDescontoQtdeStr, string idLoja)
        {
            Produto prod = null;

            try
            {
                bool isPedidoMaoObra = pedidoMaoObra.ToLower() == "true";
                bool isPedidoProducao = pedidoProducao.ToLower() == "true";
                bool isAmbienteMaoObra = ambienteMaoObra.ToLower() == "true";

                prod = ProdutoDAO.Instance.GetByCodInterno(codInterno, null, Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaUintNullable(idCliente), null, true);

                if (prod == null)
                    return "Erro;Não existe produto com o código informado.";
                else if (prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
                else if (prod.Compra)
                    return "Erro;Produto utilizado apenas na compra.";

                if (isPedidoMaoObra)
                {
                    if (!isAmbienteMaoObra)
                    {
                        if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                            return "Erro;Apenas produtos do grupo 'Mão de Obra Beneficiamento' podem ser incluídos nesse pedido.";
                    }
                    else if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro)
                        return "Erro;Apenas produtos do grupo 'Vidro' podem ser usados como peça de vidro.";
                }
                else if (isPedidoProducao)
                {
                    if (prod.IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Vidro || !SubgrupoProdDAO.Instance.IsSubgrupoProducao(prod.IdGrupoProd, prod.IdSubgrupoProd))
                        return "Erro;Apenas produtos do grupo 'Vidro' marcados como 'Produtos para Estoque' podem ser incluídos nesse pedido.";
                }
                else if (PedidoConfig.DadosPedido.BloqueioPedidoMaoDeObra && prod.IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra)
                    return "Erro;Produtos do grupo 'Mão de Obra Beneficiamento' estão bloqueados para pedidos comuns.";

                if (SubgrupoProdDAO.Instance.ObterBloquearEcommerce(null, prod.IdSubgrupoProd.Value))
                    return "Erro;Este produto não pode ser selecionado na plataforma e-commerce. Entre em contato com a empresa para mais informações";

                string retorno = "Prod;" + prod.IdProd + ";" + prod.Descricao;
                decimal valorProduto = 0;

                // Recupera o valor de tabela do produto
                int? tipoEntr = !String.IsNullOrEmpty(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !String.IsNullOrEmpty(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                float percDescontoQtde = !String.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
                valorProduto = ProdutoDAO.Instance.GetValorTabela(prod.IdProd, tipoEntr, idCli, revenda.ToLower() == "true",
                    reposicao.ToLower() == "true", percDescontoQtde, Conversoes.StrParaInt(Request["idPedido"]), null, null);

                if (PedidoConfig.Comissao.ComissaoPedido)
                    valorProduto = valorProduto / ((100 - decimal.Parse(percComissao)) / 100);

                retorno += ";" + valorProduto.ToString("F2");

                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd).ToString().ToLower() + ";" +
                    (prod.AtivarAreaMinima ? prod.AreaMinima.ToString().Replace(',', '.') : "0");

                bool bloquearEstoque = GrupoProdDAO.Instance.BloquearEstoque(prod.IdGrupoProd, prod.IdSubgrupoProd) && !isPedidoProducao;
                retorno += ";" + (bloquearEstoque ? ProdutoLojaDAO.Instance.GetEstoque(UserInfo.GetUserInfo.IdLoja, (uint)prod.IdProd, isPedidoProducao).ToString() : "100000");

                // Verifica como deve ser feito o cálculo do produto
                retorno += ";" + Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false);

                // Retorna a espessura do produto
                retorno += ";" + prod.Espessura;

                // Retorna a alíquota ICMS do produto
                retorno += ";" + prod.AliqICMSInterna.ToString().Replace(',', '.');

                //if (isPedidoProducao)
                retorno += ";" + (prod.Altura != null ? prod.Altura.Value.ToString() : "") + ";" + (prod.Largura != null ? prod.Largura.Value.ToString() : "");

                retorno += ";" + prod.IdCorVidro + ";" + prod.Forma + ";" + prod.CustoCompra;

                return retorno;
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro;Falha ao buscar produto.", ex);
            }
        }

        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetCli(string idCli)
        {
            try
            {
                Cliente cli = ClienteDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCli));

                if (cli == null || cli.IdCli == 0)
                    return "Erro;Cliente não encontrado.";
                else if (cli.Situacao == (int)SituacaoCliente.Inativo)
                    return "Erro;Cliente inativo. Motivo: " + cli.Obs.Replace("\n", "<br />");
                else if (cli.Situacao == (int)SituacaoCliente.Cancelado)
                    return "Erro;Cliente cancelado. Motivo: " + cli.Obs.Replace("\n", "<br />");
                else if (cli.Situacao == (int)SituacaoCliente.Bloqueado)
                    return "Erro;Cliente bloqueado. Motivo: " + cli.Obs.Replace("\n", "<br />");
                else
                    return "Ok;" + cli.Nome + ";" + cli.Revenda.ToString().ToLower() + ";" + cli.Obs.Replace("\n", "<br />");
            }
            catch
            {
                return "Erro;Cliente não encontrado.";
            }
        }

        /// <summary>
        /// Retorna o total dos produtos do pedido
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string TotalProdPed(string idPedido)
        {
            string total = ProdutosPedidoDAO.Instance.GetTotalByPedido(Glass.Conversoes.StrParaUint(idPedido));

            return total;
        }

        [Ajax.AjaxMethod]
        public string UsarDiferencaM2Prod(string codInternoProd)
        {
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInternoProd);
            if (prod == null)
                return "false";

            return (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd) && prod.IdSubgrupoProd != (int)Data.Helper.Utils.SubgrupoProduto.LevesDefeitos &&
                Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, prod.IdGrupoProd, prod.IdSubgrupoProd, false) != (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd).ToString().ToLower();
        }

        #region Fast Delivery

        [Ajax.AjaxMethod]
        public string CheckFastDelivery(string idPedidoStr, string dataEntrega, string diferencaM2)
        {
            // Se a data de entrega não tiver sido informada, não realiza verificação de metragem quadrada
            if (String.IsNullOrEmpty(dataEntrega))
                return "Ok|true";

            uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);

            try
            {
                DateTime dataEntregaAtual = DateTime.Parse(dataEntrega);
                float totalM2 = ProdutosPedidoDAO.Instance.TotalM2FastDelivery(dataEntregaAtual, idPedido);
                float m2Pedido = ProdutosPedidoDAO.Instance.GetTotalM2ByPedido(idPedido) + float.Parse(diferencaM2.Replace('.', ','));
                if (m2Pedido == 0)
                    return "Ok|true";

                DateTime? novaDataEntrega = ProdutosPedidoDAO.Instance.GetFastDeliveryDay(idPedido, dataEntregaAtual, m2Pedido);

                if (novaDataEntrega == null)
                    throw new Exception("Não foi possível encontrar uma data para agendar o Fast Delivery.");

                return "Ok|" + (novaDataEntrega.Value == dataEntregaAtual).ToString().ToLower() + "|" + totalM2 + "|" + m2Pedido + "|" + novaDataEntrega.Value.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        [Ajax.AjaxMethod]
        public string AtualizarFastDelivery(string idPedido, string dataEntrega)
        {
            Glass.Data.Model.Pedido ped = PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idPedido));

            try
            {
                ped.DataEntregaString = dataEntrega;

                PedidoDAO.Instance.Update(ped);
                return "Ok|";
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        #endregion

        #endregion

        #region Ambiente

        protected void grdAmbiente_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAmbiente.ShowFooter = e.CommandName != "Edit";

            if (e.CommandName == "ViewProd")
            {
                // Mostra os produtos relacionado ao ambiente selecionado
                hdfIdAmbiente.Value = e.CommandArgument.ToString();
                grdProdutos.Visible = true;
                grdProdutos.DataBind();

                // Mostra no label qual ambiente está sendo incluido produtos
                bool maoDeObra = IsPedidoMaoDeObra();
                AmbientePedido ambiente = AmbientePedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(hdfIdAmbiente.Value));
                lblAmbiente.Text = "<br />" + ambiente.Ambiente;
                hdfAlturaAmbiente.Value = !maoDeObra ? "" : ambiente.Altura.Value.ToString();
                hdfLarguraAmbiente.Value = !maoDeObra ? "" : ambiente.Largura.Value.ToString();
                hdfQtdeAmbiente.Value = !maoDeObra ? "1" : ambiente.Qtde.Value.ToString();
                hdfRedondoAmbiente.Value = !maoDeObra ? "" : ambiente.Redondo.ToString().ToLower();
            }
            else if (e.CommandName == "Update")
                Glass.Validacoes.DisableRequiredFieldValidator(Page);
        }

        protected void grdAmbiente_PreRender(object sender, EventArgs e)
        {
            // Se não houver nenhum ambiente cadastrado para este pedido, esconde a primeira linha
            if (AmbientePedidoDAO.Instance.CountInPedido(Glass.Conversoes.StrParaUint(Request["idPedido"])) == 0)
                grdAmbiente.Rows[0].Visible = false;
        }

        protected void lnkInsAmbiente_Click(object sender, EventArgs e)
        {
            string ambiente = ((HiddenField)grdAmbiente.FooterRow.FindControl("hdfDescrAmbiente")).Value;
            string descricao = ((TextBox)grdAmbiente.FooterRow.FindControl("txtDescricao")).Text;

            if (ambiente == String.Empty)
            {
                Glass.MensagemAlerta.ShowMsg("Informe o ambiente.", Page);
                return;
            }

            AmbientePedido ambPed = new AmbientePedido();
            ambPed.IdPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
            ambPed.Ambiente = ambiente;
            ambPed.Descricao = descricao;

            if (IsPedidoMaoDeObra())
            {
                string qtde = ((TextBox)grdAmbiente.FooterRow.FindControl("txtQtdeAmbiente")).Text;
                string altura = ((TextBox)grdAmbiente.FooterRow.FindControl("txtAlturaAmbiente")).Text;
                string largura = ((TextBox)grdAmbiente.FooterRow.FindControl("txtLarguraAmbiente")).Text;
                string idProd = ((HiddenField)grdAmbiente.FooterRow.FindControl("hdfAmbIdProd")).Value;
                bool redondo = ((CheckBox)grdAmbiente.FooterRow.FindControl("chkRedondoAmbiente")).Checked;
                string idAplicacao = ((HiddenField)grdAmbiente.FooterRow.FindControl("hdfAmbIdAplicacao")).Value;
                string idProcesso = ((HiddenField)grdAmbiente.FooterRow.FindControl("hdfAmbIdProcesso")).Value;

                ambPed.Qtde = !String.IsNullOrEmpty(qtde) ? (int?)Glass.Conversoes.StrParaInt(qtde) : null;
                ambPed.Altura = !String.IsNullOrEmpty(altura) ? (int?)Glass.Conversoes.StrParaInt(altura) : null;
                ambPed.Largura = !String.IsNullOrEmpty(largura) ? (int?)Glass.Conversoes.StrParaInt(largura) : null;
                ambPed.IdProd = !String.IsNullOrEmpty(idProd) ? (uint?)Glass.Conversoes.StrParaUint(idProd) : null;
                ambPed.Redondo = !redondo ? ProdutoDAO.Instance.IsRedondo(Conversoes.StrParaUint(idProd)) : redondo;
                ambPed.IdAplicacao = !String.IsNullOrEmpty(idAplicacao) ? (uint?)Glass.Conversoes.StrParaUint(idAplicacao) : null;
                ambPed.IdProcesso = !String.IsNullOrEmpty(idProcesso) ? (uint?)Glass.Conversoes.StrParaUint(idProcesso) : null;

                if (ambPed.Altura != ambPed.Largura && redondo)
                    throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");
            }

            try
            {
                // Cadastra um novo ambiente para o pedido
                hdfIdAmbiente.Value = AmbientePedidoDAO.Instance.Insert(ambPed).ToString();
                lblAmbiente.Text = "<br />" + ambiente;

                hdfAlturaAmbiente.Value = ambPed.Altura != null ? ambPed.Altura.Value.ToString() : "";
                hdfLarguraAmbiente.Value = ambPed.Largura != null ? ambPed.Largura.Value.ToString() : "";

                grdProdutos.Visible = true;
                grdAmbiente.DataBind();
                grdProdutos.DataBind();

                // Esconde a 1ª linha da grdProduto, por não haver produtos cadastrados no ambiente
                grdProdutos.Rows[0].Visible = false;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir ambiente.", ex, Page);
            }
        }

        protected void odsAmbiente_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                hdfIdAmbiente.Value = "";
                lblAmbiente.Text = "";
                grdProdutos.Visible = false;
                dtvPedido.DataBind();
            }
        }

        protected void odsAmbiente_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                dtvPedido.DataBind();
        }

        #endregion

        #region "Finalizar" Pedido

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPedidoMaoDeObra())
                {
                    var ambientes = AmbientePedidoDAO.Instance.GetByPedido(Glass.Conversoes.StrParaUint(Request["idPedido"]), false);
                    foreach (AmbientePedido a in ambientes)
                        if (!AmbientePedidoDAO.Instance.PossuiProdutos(a.IdAmbientePedido))
                            throw new Exception("O vidro " + a.PecaVidro + " não possui mão-de-obra cadastrada. Cadastre alguma mão-de-obra ou remova o vidro para continuar.");
                }

                PedidoDAO.Instance.FinalizarPedidoComTransacao(Glass.Conversoes.StrParaUint(Request["idPedido"]), false);

                AbreImpressaoPedido();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
        }

        protected void AbreImpressaoPedido()
        {
            string script = @"
                openWindow(600, 800, '../Relatorios/RelPedido.aspx?idPedido=" + Request["idPedido"] + @"');
                redirectUrl('LstPedidos.aspx" + (Request["ByVend"] == "1" ? "?ByVend=1" : "") + "');";

            ClientScript.RegisterClientScriptBlock(typeof(string), "showRpt", script, true);
        }

        #endregion

        #region Em Conferência "Pedido"

        protected void btnEmConferencia_Click(object sender, EventArgs e)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);

            // Verifica se o Pedido possui produtos
            if (ProdutosPedidoDAO.Instance.CountInPedido(idPedido) == 0)
            {
                Glass.MensagemAlerta.ShowMsg("Inclua pelo menos um produto no pedido para finalizá-lo.", Page);
                return;
            }

            try
            {
                // Cria um registro na tabela em conferencia para este pedido
                PedidoConferenciaDAO.Instance.NovaConferencia(idPedido, PedidoDAO.Instance.ObtemIdSinal(null, idPedido) > 0);
                Voltar();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
            }
        }

        #endregion

        #region Insere ProdutoPedido

        protected void lnkInsProd_Click(object sender, EventArgs e)
        {
            if (grdProdutos.PageCount > 1)
                grdProdutos.PageIndex = grdProdutos.PageCount - 1;

            Controls.ctrlBenef benef = (Controls.ctrlBenef)grdProdutos.FooterRow.FindControl("ctrlBenefInserir");
            bool isPedidoMaoDeObra = IsPedidoMaoDeObra();
            bool isPedidoProducao = IsPedidoProducao();

            uint idPedido = Glass.Conversoes.StrParaUint(Request["IdPedido"]);
            int idProd = !String.IsNullOrEmpty(hdfIdProd.Value) ? Glass.Conversoes.StrParaInt(hdfIdProd.Value) : 0;
            string idAmbiente = hdfIdAmbiente.Value;
            string alturaString = ((TextBox)grdProdutos.FooterRow.FindControl("txtAlturaIns")).Text;
            string alturaRealString = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfAlturaRealIns")).Value;
            string larguraString = ((TextBox)grdProdutos.FooterRow.FindControl("txtLarguraIns")).Text;
            Single altura = !String.IsNullOrEmpty(alturaString) ? Single.Parse(alturaString, System.Globalization.NumberStyles.AllowDecimalPoint) : 0;
            Single alturaReal = !String.IsNullOrEmpty(alturaRealString) ? Single.Parse(alturaRealString, System.Globalization.NumberStyles.AllowDecimalPoint) : 0;
            int largura = !String.IsNullOrEmpty(larguraString) ? Glass.Conversoes.StrParaInt(larguraString) : 0;
            string idProcessoStr = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdProcesso")).Value;
            string idAplicacaoStr = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdAplicacao")).Value;
            string espessuraString = ((TextBox)grdProdutos.FooterRow.FindControl("txtEspessura")).Text;
            float espessura = !String.IsNullOrEmpty(espessuraString) ? Glass.Conversoes.StrParaFloat(espessuraString) : 0;
            bool redondo = ((CheckBox)benef.FindControl("Redondo_chkSelecao")) != null ? ((CheckBox)benef.FindControl("Redondo_chkSelecao")).Checked : false;
            float aliquotaIcms = float.Parse(((HiddenField)grdProdutos.FooterRow.FindControl("hdfAliquotaIcmsProd")).Value.Replace('.', ','));
            decimal valorIcms = Glass.Conversoes.StrParaDecimal(((HiddenField)grdProdutos.FooterRow.FindControl("hdfValorIcmsProd")).Value.Replace('.', ','));
            string alturaBenefString = ((DropDownList)grdProdutos.FooterRow.FindControl("drpAltBenef")).SelectedValue;
            string larguraBenefString = ((DropDownList)grdProdutos.FooterRow.FindControl("drpLargBenef")).SelectedValue;
            string espBenefString = isPedidoMaoDeObra ? ((TextBox)grdProdutos.FooterRow.FindControl("txtEspBenef")).Text : "";
            int? alturaBenef = isPedidoMaoDeObra && !String.IsNullOrEmpty(alturaBenefString) ? (int?)Glass.Conversoes.StrParaInt(alturaBenefString) : null;
            int? larguraBenef = isPedidoMaoDeObra && !String.IsNullOrEmpty(larguraBenefString) ? (int?)Glass.Conversoes.StrParaInt(larguraBenefString) : null;

            int tipoEntrega = Glass.Conversoes.StrParaInt(((HiddenField)dtvPedido.FindControl("hdfTipoEntrega")).Value);
            uint idCliente = Glass.Conversoes.StrParaUint(((HiddenField)dtvPedido.FindControl("hdfIdCliente")).Value);
            bool reposicao = bool.Parse(((HiddenField)dtvPedido.FindControl("hdfIsReposicao")).Value);

            // Cria uma instância do ProdutosPedido
            ProdutosPedido prodPed = new ProdutosPedido();
            prodPed.IdPedido = idPedido;
            prodPed.Qtde = float.Parse(((TextBox)grdProdutos.FooterRow.FindControl("txtQtdeIns")).Text.Replace('.', ','));
            prodPed.ValorVendido = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorIns")).Text); ;
            prodPed.PercDescontoQtde = ((Controls.ctrlDescontoQtde)grdProdutos.FooterRow.FindControl("ctrlDescontoQtde")).PercDescontoQtde;
            prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(idProd, tipoEntrega, idCliente, false, reposicao, prodPed.PercDescontoQtde, Conversoes.StrParaInt(Request["idPedido"]), null, null, altura);
            prodPed.Altura = altura;
            prodPed.AlturaReal = alturaReal;
            prodPed.Largura = largura;
            prodPed.IdProd = (uint)idProd;
            prodPed.Espessura = espessura;
            prodPed.Redondo = !redondo ? ProdutoDAO.Instance.IsRedondo((uint)idProd) : redondo;
            if (!String.IsNullOrEmpty(idAmbiente)) prodPed.IdAmbientePedido = Glass.Conversoes.StrParaUint(idAmbiente);
            if (!String.IsNullOrEmpty(idAplicacaoStr)) prodPed.IdAplicacao = Glass.Conversoes.StrParaUint(idAplicacaoStr);
            if (!String.IsNullOrEmpty(idProcessoStr)) prodPed.IdProcesso = Glass.Conversoes.StrParaUint(idProcessoStr);
            prodPed.AliqIcms = aliquotaIcms;
            prodPed.ValorIcms = valorIcms;
            prodPed.AlturaBenef = alturaBenef;
            prodPed.LarguraBenef = larguraBenef;
            prodPed.EspessuraBenef = Glass.Conversoes.StrParaFloatNullable(espBenefString);
            prodPed.Beneficiamentos = benef.Beneficiamentos;
            prodPed.PedCli = ((TextBox)grdProdutos.FooterRow.FindControl("txtPedCli")).Text;

            if (altura != largura && redondo)
                throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");

            uint idProdPed = 0;

            try
            {
                // Insere o produto_pedido
                idProdPed = ProdutosPedidoDAO.Instance.InsertEAtualizaDataEntrega(prodPed);

                grdProdutos.DataBind();
                dtvPedido.DataBind();
                grdAmbiente.DataBind();
            }
            catch (Exception ex)
            {
                //if (idProdPed > 0)
                //    ProdutosPedidoDAO.Instance.DeleteByPrimaryKey(idProdPed);

                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir produto no Pedido.", ex, Page);
                return;
            }
        }

        #endregion

        #region Cancelar edição de pedido

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            hdfIdPedido.Value = Request["idPedido"];

            dtvPedido.ChangeMode(DetailsViewMode.ReadOnly);

            divProduto.Visible = dtvPedido.CurrentMode == DetailsViewMode.ReadOnly;
            //grdProdutos.Visible = divProduto.Visible;

            grdProdutos.Visible = (PedidoConfig.DadosPedido.AmbientePedido && !String.IsNullOrEmpty(hdfIdAmbiente.Value) &&
                hdfIdAmbiente.Value != "0") || !PedidoConfig.DadosPedido.AmbientePedido;
        }

        #endregion

        #region Editar Pedido

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            divProduto.Visible = false;
            grdProdutos.Visible = false;
            lnkProjeto.Visible = false;
        }

        #endregion

        #region Voltar

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Voltar();
        }

        private void Voltar()
        {
            if (Request["ByVend"] == "1")
                Response.Redirect("../Listas/LstPedidos.aspx?ByVend=1");
            else
                Response.Redirect("../Listas/LstPedidos.aspx");
        }

        #endregion

        #region Fast Delivery

        protected void FastDelivery_Load(object sender, EventArgs e)
        {
            sender.GetType().GetProperty("Visible").SetValue(sender, PedidoConfig.Pedido_FastDelivery.FastDelivery, null);

            if (sender is CheckBox && ((CheckBox)sender).ID == "chkFastDelivery")
            {
                bool exibir = PedidoConfig.Pedido_FastDelivery.FastDelivery &&
                    Config.PossuiPermissao(Config.FuncaoMenuPedido.PermitirMarcarFastDelivery);
                ((CheckBox)sender).Style.Value = exibir ? "" : "display: none";
            }
        }

        #endregion

        #region Têmpera Fora

        protected void TemperaFora_Load(object sender, EventArgs e)
        {
            sender.GetType().GetProperty("Visible").SetValue(sender, PedidoConfig.TamanhoVidro.UsarTamanhoMaximoVidro, null);
        }

        #endregion

        #region Métodos usados para iniciar valores na página

        protected string GetPosValor()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);

                int tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(idPedido);
                bool isRevenda = ClienteDAO.Instance.IsRevenda(PedidoDAO.Instance.ObtemIdCliente(null, idPedido));

                // Verifica qual valor será utilizado
                if (isRevenda) // Se for cliente revenda, valor de atacado
                    return "1";
                else if (tipoEntrega == 1 || tipoEntrega == 4) // Balcão ou Entrega
                    return "2";
                else if (tipoEntrega == 2 || tipoEntrega == 3 || tipoEntrega == 5 || tipoEntrega == 6) // Colocação Comum e Temperado
                    return "3";
                else
                    return "1";
            }
            else
                return "1";
        }

        protected string GetTotalM2Pedido()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
            {
                var prodPed = ProdutosPedidoDAO.Instance.GetByPedido(Glass.Conversoes.StrParaUint(Request["idPedido"]));
                float m2 = 0f;

                foreach (ProdutosPedido p in prodPed)
                    if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)p.IdGrupoProd) && p.TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd)
                        m2 += p.TotM;

                return m2.ToString().Replace(',', '.');
            }
            else
                return "0";
        }

        protected string GetDataEntrega()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
            {
                DateTime? dataEntrega = PedidoDAO.Instance.ObtemDataEntrega(null, Glass.Conversoes.StrParaUint(Request["idPedido"]));

                return dataEntrega != null ? dataEntrega.Value.ToString("dd/MM/yyyy") : "";
            }
            else
                return "";
        }

        protected string GetDataPedido()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.ObtemDataPedido(null, Glass.Conversoes.StrParaUint(Request["idPedido"])).ToString("dd/MM/yyyy");
            else
                return "";
        }

        protected string GetPrazoEntregaFastDelivery()
        {
            return PedidoConfig.Pedido_FastDelivery.PrazoEntregaFastDelivery.ToString();
        }

        protected string IsFastDelivery()
        {
            return PedidoConfig.Pedido_FastDelivery.FastDelivery.ToString().ToLower();
        }

        protected bool IsPedidoMaoDeObra()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.IsMaoDeObra(null, Glass.Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }

        protected bool IsPedidoProducao()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.IsProducao(null, Glass.Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }

        protected bool IsPedidoRevenda()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.IsRevenda(null ,Glass.Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }

        #endregion

        #region ICMS

        protected void Icms_Load(object sender, EventArgs e)
        {
            var idPedido = Request["idPedido"];
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, Conversoes.StrParaUint(idPedido));
            sender.GetType().GetProperty("Visible").SetValue(sender, LojaDAO.Instance.ObtemCalculaIcmsStPedido(null, idLoja), null);
        }

        #endregion

        #region Beneficiamentos

        protected void txtEspessura_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            ProdutosPedido prodPed = linhaControle.DataItem as ProdutosPedido;
            txt.Enabled = prodPed.Espessura <= 0;
        }

        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Controls.ctrlBenef benef = (Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            Control codProd = null;
            if (linhaControle.FindControl("lblCodProdIns") != null)
                codProd = linhaControle.FindControl("lblCodProdIns");
            else
                codProd = linhaControle.FindControl("txtCodProdIns");
            TextBox txtAltura = (TextBox)linhaControle.FindControl("txtAlturaIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtEspessura");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtLarguraIns");
            HiddenField hdfPercComissao = (HiddenField)dtvPedido.FindControl("hdfPercComissao");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtQtdeIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvPedido.FindControl("hdfTipoEntrega");
            Label lblTotalM2 = Beneficiamentos.UsarM2CalcBeneficiamentos ?
                (Label)linhaControle.FindControl("lblTotM2Calc") :
                (Label)linhaControle.FindControl("lblTotM2Ins");
            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtValorIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvPedido.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvPedido.FindControl("hdfIdCliente");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfCustoProd");

            benef.CampoAltura = txtAltura;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoPercComissao = hdfPercComissao;
            benef.CampoQuantidade = txtQuantidade;
            benef.CampoTipoEntrega = hdfTipoEntrega;
            benef.CampoTotalM2 = lblTotalM2;
            benef.CampoValorUnitario = txtValorIns;
            benef.CampoCusto = hdfCustoProd;
            benef.CampoProdutoID = codProd;
            benef.CampoRevenda = hdfCliRevenda;
            benef.CampoClienteID = hdfIdCliente;
            benef.CampoAplicacaoID = linhaControle.FindControl("hdfIdAplicacao");
            benef.CampoProcessoID = linhaControle.FindControl("hdfIdProcesso");
            benef.CampoAplicacao = linhaControle.FindControl("txtAplIns");
            benef.CampoProcesso = linhaControle.FindControl("txtProcIns");
        }

        #endregion

        #region Parcelas

        protected void ctrlParcelas1_DataBinding(object sender, EventArgs e)
        {
            Glass.Data.Model.Pedido ped = dtvPedido.DataItem as Glass.Data.Model.Pedido;

            Controls.ctrlParcelas ctrlParcelas = (Controls.ctrlParcelas)sender;
            HiddenField hdfCalcularParcela = (HiddenField)dtvPedido.FindControl("hdfCalcularParcela");
            HiddenField hdfExibirParcela = (HiddenField)dtvPedido.FindControl("hdfExibirParcela");
            DropDownList drpNumParc = (DropDownList)dtvPedido.FindControl("drpNumParc");
            Controls.ctrlTextBoxFloat ctrValEntrada = (Controls.ctrlTextBoxFloat)dtvPedido.FindControl("ctrValEntrada");
            TextBox txtEntrada = (TextBox)ctrValEntrada.FindControl("txtNumber");
            HiddenField hdfEntrada = (HiddenField)dtvPedido.FindControl("hdfValorEntrada");
            TextBox txtTotal = (TextBox)dtvPedido.FindControl("txtTotal");
            TextBox txtDesconto = (TextBox)dtvPedido.FindControl("txtDesconto");
            DropDownList drpTipoDesconto = (DropDownList)dtvPedido.FindControl("drpTipoDesconto");
            HiddenField hdfDesconto = (HiddenField)dtvPedido.FindControl("hdfDesconto");
            HiddenField hdfTipoDesconto = (HiddenField)dtvPedido.FindControl("hdfTipoDesconto");
            TextBox txtAcrescimo = (TextBox)dtvPedido.FindControl("txtAcrescimo");
            DropDownList drpTipoAcrescimo = (DropDownList)dtvPedido.FindControl("drpTipoAcrescimo");
            HiddenField hdfAcrescimo = (HiddenField)dtvPedido.FindControl("hdfAcrescimo");
            HiddenField hdfTipoAcrescimo = (HiddenField)dtvPedido.FindControl("hdfTipoAcrescimo");

            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcela;
            ctrlParcelas.CampoExibirParcelas = hdfExibirParcela;
            ctrlParcelas.CampoParcelasVisiveis = drpNumParc;
            ctrlParcelas.CampoValorEntrada = ped.RecebeuSinal ? (Control)hdfEntrada : (Control)txtEntrada;
            ctrlParcelas.CampoValorTotal = txtTotal;
            ctrlParcelas.CampoValorDescontoAtual = txtDesconto;
            ctrlParcelas.CampoTipoDescontoAtual = drpTipoDesconto;
            ctrlParcelas.CampoValorDescontoAnterior = hdfDesconto;
            ctrlParcelas.CampoTipoDescontoAnterior = hdfTipoDesconto;
            ctrlParcelas.CampoValorAcrescimoAtual = txtAcrescimo;
            ctrlParcelas.CampoTipoAcrescimoAtual = drpTipoAcrescimo;
            ctrlParcelas.CampoValorAcrescimoAnterior = hdfAcrescimo;
            ctrlParcelas.CampoTipoAcrescimoAnterior = hdfTipoAcrescimo;
        }

        #endregion

        protected void hdfMaoDeObra_Load(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = (Request["maoObra"] == "1").ToString().ToLower();
        }

        protected void hdfProducao_Load(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = (Request["producao"] == "1").ToString().ToLower();
        }

        protected void txtDataEntrega_Load(object sender, EventArgs e)
        {
            uint? idPedido = Request["idPedido"] != null ? (uint?)Glass.Conversoes.StrParaUint(Request["idPedido"]) : null;
            uint idCli = idPedido > 0 ? PedidoDAO.Instance.GetIdCliente(null, idPedido.Value) : 0;
            DateTime dataMinima, dataFastDelivery;

            if ((!IsPostBack || dtvPedido.CurrentMode == DetailsViewMode.Edit) &&
                PedidoDAO.Instance.GetDataEntregaMinima(null, idCli, idPedido, null, null, out dataMinima, out dataFastDelivery))
            {
                ((HiddenField)((TextBox)sender).Parent.FindControl("hdfDataEntregaFD")).Value = dataFastDelivery.ToString("dd/MM/yyyy");
                ((HiddenField)((TextBox)sender).Parent.FindControl("hdfDataEntregaNormal")).Value = dataMinima.ToString("dd/MM/yyyy");

                if (dtvPedido.CurrentMode == DetailsViewMode.Insert)
                    ((TextBox)sender).Text = dataMinima.ToString("dd/MM/yyyy");
            }
        }

        protected bool GetBloquearDataEntrega()
        {
            uint? idPedido = Request["idPedido"] != null ? (uint?)Glass.Conversoes.StrParaUint(Request["idPedido"]) : null;
            return PedidoDAO.Instance.BloquearDataEntregaMinima(null, idPedido);
        }

        protected void ddlTipoEntrega_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Configuracoes.ProjetoConfig.TelaCadastroParceiros.BloquearTipoEntregaEntrega)
                {
                    ((DropDownList)sender).Enabled = false;
                    ((DropDownList)sender).SelectedValue = DataSources.Instance.GetTipoEntregaEntrega().ToString();
                }
                else
                {
                    Glass.Data.Model.Pedido.TipoEntregaPedido? tipoEntrega = PedidoConfig.TipoEntregaPadraoPedido;
                    if (tipoEntrega != null)
                        ((DropDownList)sender).SelectedValue = ((int)tipoEntrega.Value).ToString();
                }
            }
        }

        protected void grdAmbiente_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        protected void txtPercentual_Load(object sender, EventArgs e)
        {
            if (PedidoConfig.Comissao.UsarComissionadoCliente)
                ((TextBox)sender).Style.Add("display", "none");
        }

        protected string GetDescontoProdutos()
        {
            try
            {
                if (!String.IsNullOrEmpty(Request["idPedido"]))
                    return Glass.Data.DAL.PedidoDAO.Instance.GetDescontoProdutos(null, Glass.Conversoes.StrParaUint(Request["idPedido"])).ToString().Replace(",", ".");
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
        }

        protected string GetDescontoPedido()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Request["idPedido"]))
                {
                    decimal descontoProdutos, descontoPedido;
                    var idPedido = Request["idPedido"].StrParaUint();

                    descontoProdutos = PedidoDAO.Instance.GetDescontoProdutos(null, idPedido);
                    descontoPedido = PedidoDAO.Instance.GetDescontoPedido(null, idPedido, descontoProdutos);

                    return (descontoProdutos + descontoPedido).ToString().Replace(",", ".");
                }
                else
                {
                    return "0";
                }
            }
            catch
            {
                return "0";
            }
        }

        protected void txtValorIns_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }

        protected void lblQtdeAmbiente_PreRender(object sender, EventArgs e)
        {
            ((Label)sender).Text = IsPedidoMaoDeObra() ? " x " + hdfQtdeAmbiente.Value + " peça(s) de vidro" : "";
        }

        protected string NomeControleBenef()
        {
            return grdProdutos.EditIndex == -1 ? "ctrlBenefInserir" : "ctrlBenefEditar";
        }

        protected void ctrlDescontoQtde_Load(object sender, EventArgs e)
        {
            Controls.ctrlDescontoQtde desc = (Controls.ctrlDescontoQtde)sender;
            GridViewRow linha = desc.Parent.Parent as GridViewRow;

            desc.CampoQtde = linha.FindControl("txtQtdeIns");
            desc.CampoProdutoID = linha.FindControl("hdfIdProd");
            desc.CampoClienteID = dtvPedido.FindControl("hdfIdCliente");
            desc.CampoTipoEntrega = dtvPedido.FindControl("hdfTipoEntrega");
            desc.CampoRevenda = dtvPedido.FindControl("hdfCliRevenda");
            desc.CampoValorUnit = linha.FindControl("txtValorIns");

            if (desc.CampoProdutoID == null)
                desc.CampoProdutoID = hdfIdProd;
        }

        /// <summary>
        /// Mostra/Esconde campos do total bruto e líquido
        /// </summary>
        protected void lblTotalBrutoLiquido_Load(object sender, EventArgs e)
        {
            if (!Geral.NaoVendeVidro())
                ((WebControl)sender).Visible = false;
        }

        /// <summary>
        /// Mostra/Esconde campos do total geral
        /// </summary>
        protected void lblTotalGeral_Load(object sender, EventArgs e)
        {
            if (Geral.NaoVendeVidro())
                ((WebControl)sender).Visible = false;
        }

        protected int GetNumeroProdutosPedido()
        {
            uint idPedido = Request["idPedido"] != null ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
            return idPedido > 0 ? ProdutosPedidoDAO.Instance.CountInPedido(idPedido) : 0;
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                string codPedCli = ((TextBox)dtvPedido.FindControl("txtCodPedCli")).Text;
                string obs = ((TextBox)dtvPedido.FindControl("txtObs")).Text;
                string obsLib = ((TextBox)dtvPedido.FindControl("txtObsLib")).Text;
                var idTransportador = ((DropDownList)dtvPedido.FindControl("drpTransportador")).SelectedValue;

                PedidoDAO.Instance.UpdateParceiro(null, idPedido, codPedCli, null, obs, obsLib, Glass.Conversoes.StrParaIntNullable(idTransportador));
                Glass.MensagemAlerta.ShowMsg("Pedido atualizado!", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados.", ex, Page);
            }
        }

        protected bool IsExportacaoOptyWay()
        {
            return EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.OptyWay;
        }

        protected bool IsReposicao(object tipoVenda)
        {
            return (int)tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição;
        }
    }
}
