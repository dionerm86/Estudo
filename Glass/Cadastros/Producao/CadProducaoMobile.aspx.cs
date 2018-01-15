using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Text;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class CadProducaoMobile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Altera a cor do label de descrição da etiqueta
            if (IsPostBack)
                lblDescrEtiqueta.ForeColor = lblDescrEtiqueta.ForeColor == System.Drawing.Color.Black ? System.Drawing.Color.Red : System.Drawing.Color.Black;
            
			hdfFunc.Value = UserInfo.GetUserInfo.CodUser.ToString();
    
            UserInfo.SetActivity();
    
            // Obtém os setores que o funcionário possui acesso
            var funcSetor = FuncionarioSetorDAO.Instance.GetSetores(UserInfo.GetUserInfo.CodUser);

            if (funcSetor == null || funcSetor.Count == 0)
                throw new Exception("Este funcionário não tem permissão de leitura em nenhum setor.");

            Setor setorPrincipal = Data.Helper.Utils.ObtemSetor((uint)funcSetor[0].IdSetor);
    
            if (!IsPostBack)
            {
                if (setorPrincipal.Tipo == TipoSetor.Entregue)
                    pedidoNovo.Visible = true;
    
                if ((Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != DataSources.TipoReposicaoEnum.Peca && !Config.PossuiPermissao(Config.FuncaoMenuPedido.ReposicaoDePeca)) ||
                    (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca && !Config.PossuiPermissao(Config.FuncaoMenuPedido.GerarReposicao)))
                    chkPerda.Attributes.Add("style", "display: none");
    
                codChapa.Visible = setorPrincipal.Corte && PCPConfig.Etiqueta.UsarControleChapaCorte;
                tdCodCavalete.Visible = setorPrincipal.InformarCavalete;
                tbRota.Style.Value = "display: " + (setorPrincipal.InformarRota ? "" : "none");
                lblTitulo.Text = setorPrincipal.Descricao;
                hdfTitulo.Value = setorPrincipal.Descricao;
                hdfSetor.Value = setorPrincipal.IdSetor.ToString();
                hdfTempoLogin.Value = setorPrincipal.TempoLogin.ToString();
                hdfCorTela.Value = setorPrincipal.DescrCorTela;
                hdfInformarRota.Value = setorPrincipal.InformarRota.ToString().ToLower();
                hdfSituacao.Value = ((int)ProdutoPedidoProducao.SituacaoEnum.Producao).ToString();
            }
    
            perdaDefinitiva.Visible = false;// Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca;
    
            #region Monta menu da produção
    
            TableRow tr = new TableRow();
            tbMenu.Controls.Add(tr);
    
            foreach (FuncionarioSetor fs in funcSetor)
            {
                if (fs.IdSetor == 1)
                    continue;
    
                TableCell td = new TableCell();
                HyperLink lnkButton = new HyperLink();
                lnkButton.Text = fs.DescrSetor;
                lnkButton.NavigateUrl = "CadProducaoMobile.aspx?idSetor=" + fs.IdSetor;
                td.Controls.Add(lnkButton);
                tr.Controls.Add(td);
                tr.Controls.Add(new TableCell());
            }
    
            #endregion
    
            if (Request["idSetor"] != null)
                lnkButton_Click(sender, e);
        }
    
        void LimpaDadosPeca()
        {
            chkPerda.Checked = false;
            chkPerda_CheckedChanged(chkPerda, EventArgs.Empty);
        }
    
        void lnkButton_Click(object sender, EventArgs e)
        {
            uint idSetor = Glass.Conversoes.StrParaUint(Request["idSetor"]);

            Setor setor = Data.Helper.Utils.ObtemSetor(idSetor);
    
            // Habilita campos se o tipo do setor selecionado for "Entregue"
            bool setorEntregue = setor.Tipo == TipoSetor.Entregue;
            pedidoNovo.Visible = setorEntregue;
    
            // Desmarca checkBoxes
            codChapa.Visible = setor.Corte && PCPConfig.Etiqueta.UsarControleChapaCorte;
            tdCodCavalete.Visible = setor.InformarCavalete;
            tbRota.Style.Value = "display: " + (setor.InformarRota ? "" : "none");
    
            if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ReposicaoDePeca))
                chkPerda.Attributes.Add("style", "display: none");
    
            // Altera campos para que fique com os dados do setor selecionado
            lblTitulo.Text = setor.Descricao;
            hdfTitulo.Value = setor.Descricao;
            hdfFunc.Value = UserInfo.GetUserInfo.CodUser.ToString();
            hdfSetor.Value = setor.IdSetor.ToString();
            hdfTempoLogin.Value = setor.TempoLogin.ToString();
            hdfCorTela.Value = string.IsNullOrWhiteSpace(setor.DescrCorTela) ? "Azul" : setor.DescrCorTela;
            hdfInformarRota.Value = setor.InformarRota.ToString().ToLower();
            hdfSituacao.Value = ((int)ProdutoPedidoProducao.SituacaoEnum.Producao).ToString();
        }
    
        #region Métodos Ajax
    
        public bool IsProducao(string numEtiqueta)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(numEtiqueta.Split('-')[0]);
                return PedidoDAO.Instance.IsProducao(idPedido);
            }
            catch
            {
                return false;
            }
        }
    
        public bool VerificaPedidoExiste(string pedido)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(pedido);
                return PedidoDAO.Instance.PedidoExists(idPedido);
            }
            catch
            {
                return false;
            }
        }
    
        public string GetProdutosPedido(string idPedido)
        {
            try
            {
                uint id;
                if (!uint.TryParse(idPedido, out id))
                    throw new Exception("Número do pedido inválido.");
    
                var produtosTemp = id > 0 ? ProdutosPedidoDAO.Instance.GetByPedido(id, true) : new List<ProdutosPedido>();
                if (produtosTemp.Count == 0)
                    throw new Exception("Não foram encontrados produtos para esse pedido.");
    
                StringBuilder retorno = new StringBuilder(@"<table>
                    <tr><th style='font-size: small'>Produto</th>
                    <th style='padding-left: 8px; font-size: small'>Qtde.</th>
                    <th style='padding-left: 8px; font-size: small'>Qtde. Expedir</th>
                    <th style='padding-left: 8px; font-size: small'>Altura</th>
                    <th style='padding-left: 8px; font-size: small'>Largura</th></tr>");
    
                Dictionary<uint, ProdutosPedido> agrupa = new Dictionary<uint, ProdutosPedido>();
                foreach (ProdutosPedido p in produtosTemp)
                {
                    if (!agrupa.ContainsKey(p.IdProd))
                        agrupa.Add(p.IdProd, p);
                    else
                        agrupa[p.IdProd].Qtde += p.Qtde;
                }
    
                foreach (ProdutosPedido p in agrupa.Values)
                {
                    int qtde = (int)p.Qtde;
                    qtde -= ProdutoPedidoProducaoDAO.Instance.GetQtdeLiberadaByPedProd(p.IdPedido, null, p.IdProd);
    
                    if (qtde <= 0)
                        continue;
    
                    retorno.Append("<tr><td style='font-size: small'>");
                    retorno.Append(p.DescrProduto);
                    retorno.Append("</td><td style='padding-left: 8px; font-size: small'>");
                    retorno.Append(p.Qtde);
                    retorno.Append("</td><td style='padding-left: 8px; font-size: small'>");
                    retorno.Append(qtde);
                    retorno.Append("</td><td style='padding-left: 8px; font-size: small'>");
                    retorno.Append(p.AlturaLista);
                    retorno.Append("</td><td style='padding-left: 8px; font-size: small'>");
                    retorno.Append(p.Largura);
                    retorno.Append("</td></tr>");
                }
    
                retorno.Append("</table>");
                return retorno.ToString();
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }
    
        #endregion
    
        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }
    
        #region Eventos dos botões de exibição de controles adicionais
    
        protected void chkPerda_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == chkPerda)
            {
                chkPedidoNovo.Checked = false;
                chkPedidoNovo_CheckedChanged(sender, e);
    
                AlteraValidationGroup(chkPerda.Checked ? "perda" : "");
            }
    
            dadosPerda.Visible = chkPerda.Checked;
    
            if (!chkPerda.Checked)
            {
                drpTipoPerda.SelectedValue = "";
                txtObs.Text = "";
            }
    
            Color corFundo = !chkPerda.Checked ? Color.White : ColorTranslator.FromHtml("#FF5050");
            Color corTexto = !chkPerda.Checked ? Color.Black : Color.White;
    
            txtCodEtiqueta.BackColor = corFundo;
            txtCodEtiqueta.ForeColor = corTexto;
        }
    
        protected void chkPedidoNovo_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == chkPedidoNovo)
            {
                chkPerda.Checked = false;
                chkPerda_CheckedChanged(sender, e);
    
                AlteraValidationGroup("");
            }
    
            dadosPedidoNovo.Visible = chkPedidoNovo.Checked;
    
            if (!chkPedidoNovo.Checked)
            {
                txtPedidoNovo.Text = "";
                lblProdutosPedido.Text = "";
            }
        }
    
        #endregion
    
        #region Validação
    
        private void CorrigeCodEtiqueta(TextBox campo)
        {
            // Corrige o código da etiqueta
            if (campo.Text.Length > 0 && campo.Text[0] == ']')
                campo.Text = campo.Text.Substring(3);
    
            campo.Text = campo.Text.Replace(";", "/");
        }
    
        private void AlteraValidationGroup(string validationGroup)
        {
            rfvChapa.ValidationGroup = validationGroup;
            rfvEtiqueta.ValidationGroup = validationGroup;
            ctvRota.ValidationGroup = validationGroup;
        }
    
        private bool Validar()
        {
            CorrigeCodEtiqueta(txtCodChapa);
            CorrigeCodEtiqueta(txtCodEtiqueta);
    
            Page.Validate(rfvEtiqueta.ValidationGroup);
            return Page.IsValid;
        }
    
        protected void ctvTipoPerda_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value != "";
        }
    
        protected void ctvObs_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !Glass.Configuracoes.ProducaoConfig.ObrigarMotivoPerda ||
                drpTipoPerda.SelectedValue != TipoPerdaDAO.Instance.GetIDByNomeExato("Outros").ToString() || args.Value != "";
        }
    
        protected void ctvRota_ServerValidate(object source, ServerValidateEventArgs args)
        {
            uint idSetor = Glass.Conversoes.StrParaUint(hdfSetor.Value);
            args.IsValid = !Data.Helper.Utils.ObtemSetor(idSetor).InformarRota || args.Value != "";
        }
    
        #endregion
    
        protected void btnMarcarPeca_Click(object sender, EventArgs e)
        {
            if (!Validar())
                return;
    
            try
            {
                string codChapa = txtCodChapa.Text, codEtiqueta = txtCodEtiqueta.Text, descrProd = null;
                uint? pedidoNovo = Glass.Conversoes.StrParaUintNullable(txtPedidoNovo.Text);
                uint idFunc = Glass.Conversoes.StrParaUint(hdfFunc.Value);
    
                if (!chkPerda.Checked)
                    descrProd = ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(idFunc, codChapa, codEtiqueta, Glass.Conversoes.StrParaUint(hdfSetor.Value),
                        false, false, null, null, null, pedidoNovo, Glass.Conversoes.StrParaUint(drpRota.SelectedValue), null, null, false, txtCodCavalete.Text, 0);
                else if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao != DataSources.TipoReposicaoEnum.Peca)
                    descrProd = ProdutoPedidoProducaoDAO.Instance.AtualizaSituacaoComTransacao(idFunc, codChapa, codEtiqueta, Glass.Conversoes.StrParaUint(hdfSetor.Value),
                        true, false, Glass.Conversoes.StrParaUint(drpTipoPerda.SelectedValue), Glass.Conversoes.StrParaUintNullable(drpSubtipoPerda.SelectedValue),
                        txtObs.Text, null, 0, null, null, false, null, 0);
                else
                    descrProd = ProdutoPedidoProducaoDAO.Instance.MarcarPecaReposta(codChapa, codEtiqueta, Glass.Conversoes.StrParaUint(hdfSetor.Value),
                        UserInfo.GetUserInfo.CodUser, DateTime.Now, Glass.Conversoes.StrParaUint(drpTipoPerda.SelectedValue),
                        Glass.Conversoes.StrParaUintNullable(drpSubtipoPerda.SelectedValue), txtObs.Text, chkPerdaDefinitiva.Checked);
    
                lblDescrEtiqueta.Text = descrProd;
                txtCodEtiqueta.Text = string.Empty;
                txtCodCavalete.Text = string.Empty;
                
                LimpaDadosPeca();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar situação da etiqueta.", ex, Page);
            }
        }
    
        protected void drpTipoPerda_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpSubtipoPerda.Items.Clear();
            drpSubtipoPerda.Items.Add(new ListItem());
            drpSubtipoPerda.DataBind();
    
            drpSubtipoPerda.SelectedIndex = 0;
            drpSubtipoPerda.Visible = drpSubtipoPerda.Items.Count > 1;
            lblSubtipo.Visible = drpSubtipoPerda.Visible;
        }
    }
}
