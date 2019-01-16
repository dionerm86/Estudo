using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using System.Web.Security;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros.Expedicao
{
    public partial class CadLeituraCarregamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Expedicao.CadLeituraCarregamento));

            if (!IsPostBack)
            {
                // Obtém os setores que o funcionário possui acesso
                var funcSetor = FuncionarioSetorDAO.Instance.GetSetores(UserInfo.GetUserInfo.CodUser);

                if (funcSetor.Count == 0)
                    Response.Redirect("../../WebGlass/Main.aspx");

                var setor = Data.Helper.Utils.ObtemSetor((uint)funcSetor[0].IdSetor);

                // Se não for expedição de carregamento ou se não usar o controle de carregamento, sai desta tela
                if (!OrdemCargaConfig.UsarControleOrdemCarga ||
                    UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.MarcadorProducao ||
                     setor.Tipo != TipoSetor.ExpCarregamento)
                    Response.Redirect("../../WebGlass/Main.aspx");

                hdfTempoLogin.Value = setor.TempoLogin.ToString();
                hdfCorTela.Value = setor.DescrCorTela;

                UserInfo.SetActivity();

                hdfFunc.Value = UserInfo.GetUserInfo.CodUser.ToString();

                if (OrdemCargaConfig.OpcaoAtualizarAutomaticamenteMarcada)
                    chkAtuAutomaticamente.Checked = true;

                // Apenas quem tiver permissão de voltar peças na produção poderá estornar itens do carregamento
                if (!Config.PossuiPermissao(Config.FuncaoMenuPCP.VoltarSetorPecaProducao))
                {
                    btnEstornarTodos.Style.Add("display", "none");
                    btnEstorno.Style.Add("display", "none");
                }

                #region Mensagens

                var mensagemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Glass.Global.Negocios.IMensagemFluxo>();

                // Verifica se há novas mensagens e quantas são
                var msgNova = mensagemFluxo.ExistemNovasMensagens((int)UserInfo.GetUserInfo.CodUser);
                lnkMensagens.Visible = !msgNova;
                lnkMensagensNaoLidas.Visible = msgNova;
                #endregion


                if (!OrdemCargaConfig.ControlarPedidosImportados)
                    tbClienteExterno.Style.Add("Display", "none");
            }

            LinkBalcao.Visible = FuncionarioSetorDAO.Instance.PossuiSetorEntregue(UserInfo.GetUserInfo.CodUser);
        }

        protected void divChat_Load(object sender, EventArgs e)
        {
            divChat.Visible = UserInfo.GetUserInfo.IsCliente ? false : FuncionarioDAO.Instance.ObtemHabilitarChat(UserInfo.GetUserInfo.CodUser);
        }

        public string ObterNomeUsuario()
        {
            return string.Format("{0} ({1})", UserInfo.GetUserInfo.Nome, System.Configuration.ConfigurationManager.AppSettings["sistema"]);
        }

        public string ObterEmailUsuario()
        {
            return FuncionarioDAO.Instance.GetEmail(UserInfo.GetUserInfo.CodUser);
        }

        protected void lnkLgout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void grvProdutos_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;

            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void gvrVolumes_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;

            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void grvProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var corLinha = ((Glass.Data.Model.ItemCarregamento)e.Row.DataItem).CorLinha;
                foreach (TableCell cell in e.Row.Cells)
                    cell.ForeColor = corLinha;
            }
        }

        protected void gvrVolumes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var corLinha = ((Glass.Data.Model.ItemCarregamento)e.Row.DataItem).CorLinha;
                foreach (TableCell cell in e.Row.Cells)
                    cell.ForeColor = corLinha;
            }
        }

        #region Métodos AJAX

        /// <summary>
        /// Efetua a leitura de uma peça ou volume
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="etiqueta"></param>
        /// <param name="idPedidoExp"></param>
        /// <param name="numCli"></param>
        /// <param name="nomeCli"></param>
        [Ajax.AjaxMethod()]
        public void EfetuaLeitura(string idFunc, string idCarregamento, string etiqueta, string idPedidoExp, string numCli, string nomeCli,
            string idOc, string idPedido, string altura, string largura, string numEtqFiltro, string idClienteExterno, string nomeClienteExterno, string idPedidoExterno)
        {
            if (UserInfo.GetUserInfo.CodUser.ToString() != idFunc)
            {
                throw new Exception("Erro ao recuperar o usuário logado. Faça login novamente!");
            }
            else
            {
                WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.EfetuaLeitura(idFunc, idCarregamento, etiqueta, idPedidoExp, numCli, nomeCli,
                    idOc, idPedido, altura, largura, numEtqFiltro, idClienteExterno.StrParaUint(), nomeClienteExterno, idPedidoExterno.StrParaUint());
            }
        }

        /// <summary>
        /// Verifica se um carregamento existe
        /// </summary>
        /// <param name="idCarregameto"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string CarregamentoExiste(string idCarregameto)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.CarregamentoExiste(idCarregameto);
        }

        /// <summary>
        /// Verifica se a etiqueta é de revenda para fazer o vinculo com o pedido
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string IsEtiquetaRevenda(string etiqueta)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.IsEtiquetaRevenda(etiqueta);
        }

        [Ajax.AjaxMethod()]
        public int ObterIdPedidoRevenda(string etiqueta)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.ObterIdPedidoRevenda(etiqueta);
        }

        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (String.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }

        #endregion

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            PageBind();
        }

        private void PageBind()
        {
            dtvItensCarregamento.DataBind();
            grvProdutos.DataBind();
            gvrVolumes.DataBind();
        }

        protected void btnEstorno_Click(object sender, EventArgs e)
        {
            if (!grvProdutos.Columns[0].Visible || !gvrVolumes.Columns[0].Visible)
            {
                grvProdutos.Columns[0].Visible = true;
                gvrVolumes.Columns[0].Visible = true;
                return;
            }

            grvProdutos.Columns[0].Visible = false;
            gvrVolumes.Columns[0].Visible = false;

            var idsItens = new List<uint>();

            foreach (GridViewRow row in grvProdutos.Rows)
                if (((CheckBox)row.FindControl("chkSelEstorno")).Checked)
                    idsItens.Add(Glass.Conversoes.StrParaUint(((HiddenField)row.FindControl("hdfIdItemCarregamento")).Value));

            foreach (GridViewRow row in gvrVolumes.Rows)
                if (((CheckBox)row.FindControl("chkSelEstorno")).Checked)
                    idsItens.Add(Glass.Conversoes.StrParaUint(((HiddenField)row.FindControl("hdfIdItemCarregamento")).Value));

            if (idsItens.Count == 0)
                return;

            string ids = string.Join(",", idsItens.Select(i => i.ToString()).ToArray());

            string script = "openWindow(100, 260, '../../Utils/SetMotivoEstornoItemCarregamento.aspx?popup=true&ids=" + ids + "', null, true, true);";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "abrirEstorno", script, true);
        }

        protected void dtvItensCarregamento_DataBound(object sender, EventArgs e)
        {
            btnEstorno.Visible = btnEstornarTodos.Visible = ((WebGlass.Business.OrdemCarga.Entidade.InfoCarregamento)((DetailsView)sender).DataItem).EstornarVisivel;
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            PageBind();
        }

        protected void btnEstornarTodos_Click(object sender, EventArgs e)
        {
            string script = @"openWindow(100, 260, '../../Utils/SetMotivoEstornoItemCarregamento.aspx?popup=true&idCarregamento=" + txtCodCarregamento.Text +"&idCliente=" + txtNumCli.Text +"&idOc=" + txtIdOc.Text +"&idPedido=" + txtIdPedido.Text +"&etiqueta=" + txtEtqFiltro.Text +"&altura=" + txtAltura.Text +"&largura=" + txtLargura.Text +"', null, true, true);";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "abrirEstorno", script, true);
        }

        protected void LinkBalcao_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadLeituraExpBalcao.aspx");
        }
    }
}
