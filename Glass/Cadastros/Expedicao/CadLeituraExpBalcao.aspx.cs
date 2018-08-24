using Glass.Data.DAL;
using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Text;

namespace Glass.UI.Web.Cadastros.Expedicao
{
    public partial class CadLeituraExpBalcao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Expedicao.CadLeituraExpBalcao));

            if (!IsPostBack)
            {
                // Obtém os setores que o funcionário possui acesso
                var funcSetor = FuncionarioSetorDAO.Instance.GetSetores(UserInfo.GetUserInfo.CodUser);

                if (funcSetor.Count == 0)
                    Response.Redirect("../../WebGlass/Main.aspx");

                var setor = Data.Helper.Utils.ObtemSetor((uint)funcSetor[0].IdSetor);

                // Se não for exp. balcão, sai desta tela
                if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.MarcadorProducao || 
                    !FuncionarioSetorDAO.Instance.PossuiSetorEntregue(UserInfo.GetUserInfo.CodUser) ||
                    !Glass.Configuracoes.PCPConfig.UsarNovoControleExpBalcao)
                    Response.Redirect("../../WebGlass/Main.aspx");

                hdfTempoLogin.Value = setor.TempoLogin.ToString();
                hdfCorTela.Value = setor.DescrCorTela;

                UserInfo.SetActivity();

                hdfFunc.Value = UserInfo.GetUserInfo.CodUser.ToString();

                #region Mensagens
                
                var mensagemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Glass.Global.Negocios.IMensagemFluxo>();

                // Verifica se há novas mensagens e quantas são
                var msgNova = mensagemFluxo.ExistemNovasMensagens((int)UserInfo.GetUserInfo.CodUser);
                lnkMensagens.Visible = !msgNova;
                lnkMensagensNaoLidas.Visible = msgNova;

                #endregion
            }
        }

        protected void lnkLgout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            dtvExpBalcao.DataBind();
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            dtvExpBalcao.DataBind();
        }

        protected void btnEstorno_Click(object sender, EventArgs e)
        {
            var grvProdutos = (GridView)dtvExpBalcao.FindControl("grvProdutos");
            var gvrVolumes = (GridView)dtvExpBalcao.FindControl("gvrVolumes");

            if (!bool.Parse(hdfEstornar.Value))
            {
                grvProdutos.Columns[0].Visible = true;
                gvrVolumes.Columns[0].Visible = true;
                hdfEstornar.Value = "true";
                return;
            }

            grvProdutos.Columns[0].Visible = false;
            gvrVolumes.Columns[0].Visible = false;
            hdfEstornar.Value = "false";

            var idsItens = new Dictionary<int, int>();

            foreach (GridViewRow row in grvProdutos.Rows)
            {
                if (((CheckBox)row.FindControl("chkSelEstorno")).Checked)
                {
                    var idProdPedProducao = ((HiddenField)row.FindControl("hdfIdProdPedProducao")).Value;
                    var idProdImpressaoChapa = ((HiddenField)row.FindControl("hdfIdProdImpressaoChapa")).Value;

                    if (!string.IsNullOrEmpty(idProdPedProducao))
                        idsItens.Add(Glass.Conversoes.StrParaInt(idProdPedProducao), 1);
                    else if (!string.IsNullOrEmpty(idProdImpressaoChapa))
                        idsItens.Add(Glass.Conversoes.StrParaInt(idProdImpressaoChapa), 2);
                }
            }

            foreach (GridViewRow row in gvrVolumes.Rows)
                if (((CheckBox)row.FindControl("chkSelEstorno")).Checked)
                    idsItens.Add(Glass.Conversoes.StrParaInt(((HiddenField)row.FindControl("hdfIdVolume")).Value), 3);

            if (idsItens.Count == 0)
                return;

            var result = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IExpedicaoFluxo>().EstornaLiberacao(idsItens);

            if (!result)
                MensagemAlerta.ErrorMsg("Falha ao estornar: ", result);

            imgPesq_Click(null, null);
        }

        protected void btnEstornoTodos_Click(object sender, EventArgs e)
        {
            var idLiberacao = txtCodLiberacao.Text.ToString().StrParaInt();
            var idPedido = txtIdPedido.Text.ToString().StrParaInt();
            var visualizar = string.Join(",", drpVisualizar.SelectedValues);

            var itens = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IExpedicaoFluxo>().BuscaParaExpBalcao(idLiberacao, idPedido, visualizar);

            var idsItens = new Dictionary<int, int>();

            foreach (var peca in itens.Pecas.Where(f=> f.Expedido))
            {
                if (peca.IdProdPedProducao.GetValueOrDefault() > 0)
                    idsItens.Add(peca.IdProdPedProducao.Value, 1);

                else if (peca.IdProdImpressaoChapa.GetValueOrDefault() > 0)
                    idsItens.Add(peca.IdProdImpressaoChapa.Value, 2);

                else if (peca.IdVolume.GetValueOrDefault() > 0)
                {
                    idsItens.Add(peca.IdVolume.Value, 3);
                }
            }

            if (idsItens.Count == 0)
                return;

            var result = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IExpedicaoFluxo>().EstornaLiberacao(idsItens);

            if (!result)
                MensagemAlerta.ErrorMsg("Falha ao estornar: ", result);

            imgPesq_Click(null, null);
        }

        protected void dtvExpBalcao_DataBound(object sender, EventArgs e)
        {
            var dtv = (DetailsView)sender;
            var dataItem = ((Glass.PCP.Negocios.Entidades.ExpBalcao)dtv.DataItem);
            var clienteVisivel = dataItem.IdLiberacao > 0;

            ((Label)dtv.FindControl("lblCliente")).Visible = clienteVisivel;
            ((Label)dtv.FindControl("lblNomeCliente")).Visible = clienteVisivel;

            ((Button)dtv.FindControl("btnEstorno")).Visible = dataItem.EstornarVisivel;
            ((Button)dtv.FindControl("btnEstornoTodos")).Visible = dataItem.EstornarVisivel;
        }

        protected void gvrVolumes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var Expedido = ((Glass.PCP.Negocios.Entidades.ItemExpBalcao)e.Row.DataItem).Expedido;
                foreach (TableCell cell in e.Row.Cells)
                    cell.ForeColor = Expedido ? Color.Green : Color.Red;
            }
        }

        protected void gvrVolumes_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;

            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void grvProdutos_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;

            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void grvProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = ((Glass.PCP.Negocios.Entidades.ItemExpBalcao)e.Row.DataItem);
                foreach (TableCell cell in e.Row.Cells)
                    cell.ForeColor = item.Expedido || item.ExpedidoManualmente ? Color.Green : (item.TrocadoDevolvido ? Color.Blue : Color.Red);
            }
        }

        #region Metodos AJAX

        /// <summary>
        /// Verifica se a liberação esta pronta para ser expedida
        /// </summary>
        /// <param name="idLiberacao"></param>
        [Ajax.AjaxMethod()]
        public void ValidaLiberacao(string idLiberacao)
        {
            LiberarPedidoDAO.Instance.ValidaLiberacaoParaExpedicaoBalcao(Glass.Conversoes.StrParaUint(idLiberacao));
        }

        /// <summary>
        /// Realiza a leitura da expedição
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idLiberacao"></param>
        /// <param name="etiqueta"></param>
        /// <param name="idPedidoExp"></param>
        [Ajax.AjaxMethod()]
        public void EfetuaLeitura(string idFunc, string idLiberacao, string etiqueta, string idPedidoExp)
        {
            ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IExpedicaoFluxo>()
                .EfetuaLeitura(Glass.Conversoes.StrParaInt(idFunc), Glass.Conversoes.StrParaInt(idLiberacao), etiqueta, Glass.Conversoes.StrParaIntNullable(idPedidoExp));
        }

        /// <summary>
        /// Verifica se a etiqueta é de revenda para fazer o vinculo com o pedido
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string IsEtiquetaRevenda(string etiqueta)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(etiqueta.Split('-')[0]);
            return (PedidoDAO.Instance.IsProducao(null, idPedido) || etiqueta.ToUpper().Substring(0, 1).Equals("N") || etiqueta.ToUpper().Substring(0, 1).Equals("R")).ToString();
        }

        [Ajax.AjaxMethod()]
        public int ObterIdPedidoRevenda(string etiqueta)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.ObterIdPedidoRevenda(etiqueta);
        }

        #endregion

        protected string OpcoesDeLeitura()
        {
            var retorno = new StringBuilder();

            retorno.AppendLine("Para leitura de faixas de etiqueta usar a seguinte sintaxe 1111-1.1/1=4\n(As etiquetas no exemplo serão lidas do item 1 até o item 4 nas etiquetas referentes à posição 1)");
            return retorno.ToString();
        }
    }
}