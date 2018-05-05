using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPedidoReposicao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.GerarReposicao))
            {
                Response.Redirect("~/Listas/LstPedidos.aspx");
                Response.End();
                return;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadPedidoReposicao));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            // Se a empresa não possuir acesso ao módulo PCP, esconde colunas Apl e Proc
            if (!Geral.ControlePCP)
            {
                grdProdutos.Columns[9].Visible = false;
                grdProdutos.Columns[10].Visible = false;
            }
    
            if (Request["idPedido"] != null)
            {
                panFiltro.Visible = false;
                dtvPedido.Visible = true;
                lkbAdicionarProduto.Visible = true;
                dtvPedidoRepos.Visible = true;
    
                if (!IsPostBack)
                {
                    // Mostra os produtos relacionado ao ambiente selecionado
                    AmbientePedido ambiente = AmbientePedidoDAO.Instance.GetForReposicaoPedido(Glass.Conversoes.StrParaUint(Request["idPedido"]));
                    hdfIdAmbiente.Value = ambiente != null ? ambiente.IdAmbientePedido.ToString() : "";
    
                    grdProdutos.Visible = true;
                    grdProdutos.DataBind();
                }
            }
    
            hdfComissaoVisible.Value = PedidoConfig.Comissao.ComissaoPedido.ToString().ToLower();
            divProduto.Visible = dtvPedido.CurrentMode == DetailsViewMode.ReadOnly;
            grdProdutos.Visible = divProduto.Visible;
    
            grdProdutos.Columns[3].Visible = Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos;
            grdProdutos.Columns[4].Visible = Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos;
            grdProdutos.Columns[5].Visible = !Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos;
    
            if (!Geral.NaoVendeVidro())
            {
                // Esconde colunas Apl, Proc e V. Benef.
                grdProdutos.Columns[10].Visible = false;
                grdProdutos.Columns[11].Visible = false;
            }
            else
            {
                grdProdutos.Columns[grdProdutos.Columns.Count - 2].Visible = false;
                grdProdutos.Columns[grdProdutos.Columns.Count - 3].Visible = false;
            }
    
            // Se a empresa trabalha com ambiente de pedido e não houver nenhum ambiente cadastrado, esconde grid de produtos
            grdProdutos.Visible = ((PedidoConfig.DadosPedido.AmbientePedido && !String.IsNullOrEmpty(hdfIdAmbiente.Value) &&
                hdfIdAmbiente.Value != "0") || !PedidoConfig.DadosPedido.AmbientePedido) && (Request["idPedido"] != null);
        }
    
        protected void grdProdutos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ProdutoPedidoBenefDAO.Instance.DeleteByProdPed(Glass.Conversoes.StrParaUint(e.Keys["IdProdPed"].ToString()));
        }
    
        protected void grdProdutos_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            dtvPedido.DataBind();
            dtvPedidoRepos.DataBind();
        }
    
        protected void grdProdutos_PreRender(object sender, EventArgs e)
        {
            string ambiente = hdfIdAmbiente.Value;
    
            // Se não houver nenhum produto cadastrado no pedido (e no ambiente passado)
            if (ProdutosPedidoDAO.Instance.CountInPedidoAmbiente(Glass.Conversoes.StrParaUint(Request["idPedido"]), !String.IsNullOrEmpty(ambiente) ? Glass.Conversoes.StrParaUint(ambiente) : 0) == 0)
                grdProdutos.Rows[0].Visible = false;
        }
    
        protected void grdProdutos_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            // Calcula novamente o valor dos beneficiamentos
            ProdutosPedidoDAO.Instance.UpdateValorBenef(null, Glass.Conversoes.StrParaUint(e.Keys["IdProdPed"].ToString()));
    
            // Calcula novamente o valor do total do pedido
            PedidoDAO.Instance.UpdateTotalPedido(Glass.Conversoes.StrParaUint(Request["IdPedido"]));

            // Atualiza o DetailsView do pedido
            dtvPedido.DataBind();
            dtvPedidoRepos.DataBind();
        }

        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string AddProduto(string idProduto, string pedido, string ambiente, string etiqueta)
        {
            return WebGlass.Business.Pedido.Fluxo.PedidoReposicao.Ajax.AddProduto(idProduto, pedido, ambiente, etiqueta);
        }
    
        #endregion
    
        #region "Finalizar" Pedido
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                string scriptExecutar;
                WebGlass.Business.Pedido.Fluxo.PedidoReposicao.Instance.Finalizar(Glass.Conversoes.StrParaUint(Request["idPedido"]),
                    Request["ByVend"] == "1", out scriptExecutar);
    
                ClientScript.RegisterClientScriptBlock(typeof(string), "showRpt", scriptExecutar, true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar pedido reposição.", ex, Page);
                return;
            }
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
    
        #region Repor pedido
    
        protected void btnRepor_Click(object sender, EventArgs e)
        {
            try
            {
                IEnumerator select = odsPedido.Select().GetEnumerator();
                select.Reset();
                select.MoveNext();
    
                uint idPedido = WebGlass.Business.Pedido.Fluxo.PedidoReposicao.Instance.Repor((Glass.Data.Model.Pedido)select.Current);
    
                // Redireciona para a parte de alteração do pedido
                Response.Redirect("~/Cadastros/CadPedidoReposicao.aspx?IdPedido=" + idPedido);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao repor pedido.", ex, Page);
            }
        }
    
        #endregion
    
        #region Filtro de pedido
    
        protected void imbPesq_Click(object sender, ImageClickEventArgs e)
        {
            odsPedido.SelectParameters["idPedido"].DefaultValue = txtNumPedido.Text;
            dtvPedido.DataBind();
            try
            {
                dtvPedido.Visible = ((object[])odsPedido.Select()).Length > 0;
            }
            catch
            {
                dtvPedido.Visible = true;
            }
        }
    
        #endregion
    
        protected void dtvPedido_DataBound(object sender, EventArgs e)
        {
            txtDataCliente_Load(dtvPedido.FindControl("txtDataCliente"), e);
        }
    
        protected void ReposicaoButtons_DataBinding(object sender, EventArgs e)
        {
            if (((Button)sender).ID == "btnFinalizar" || ((Button)sender).ID == "btnSalvar")
            {
                ((Button)sender).Visible = (Request["idPedido"] != null);
                ((Label)dtvPedido.FindControl("lblErro")).Visible = !((Button)sender).Visible;
            }
            else if (((Button)sender).ID == "btnRepor")
            {
                ((Button)sender).Visible = (Request["idPedido"] == null);
                IEnumerator select = odsPedido.Select().GetEnumerator();
                select.Reset();
                select.MoveNext();
                Glass.Data.Model.Pedido atual = (Glass.Data.Model.Pedido)select.Current;
                uint? idReposicao = PedidoDAO.Instance.ObterIdPedidoAnterior(atual.IdPedido);
    
                if (atual.TipoVenda == 3 || idReposicao != null)
                {
                    ((Label)dtvPedido.FindControl("lblErro")).Text = atual.TipoVenda == 3 ?
                        "Esse pedido já é um pedido de reposição." :
                        "Já existe um pedido de reposição para este pedido (num. reposição: " + idReposicao.Value + ")";
    
                    ((Button)sender).Enabled = true;
                }
                else
                    ((Label)dtvPedido.FindControl("lblErro")).Text = "";
            }
        }
    
        protected void odsPedidoRepos_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao atualizar o pedido.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                dtvPedido.DataBind();
        }
    
        protected void odsPedidoRepos_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao inserir o pedido.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                dtvPedido.DataBind();
        }
    
        protected void txtDataCliente_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                DateTime? data = PedidoReposicaoDAO.Instance.GetByPedido(idPedido).DataClienteInformado;
                ((TextBox)sender).Text = data != null ? data.Value.ToString("dd/MM/yyyy") : String.Empty;
            }
        }
    
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                DateTime dataEntrega = DateTime.Parse(((TextBox)dtvPedido.FindControl("txtDataEntrega")).Text);
                DateTime? dataCliente = null;
                try
                {
                    dataCliente = DateTime.Parse(((TextBox)dtvPedido.FindControl("txtDataCliente")).Text);
                }
                catch { }
    
                PedidoReposicaoDAO.Instance.SalvarDatas(idPedido, dataEntrega, dataCliente);
    
                dtvPedidoRepos.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao salvar datas.", ex, Page);
            }
        }
    
        protected void rgvQtde_DataBinding(object sender, EventArgs e)
        {
            RangeValidator r = (RangeValidator)sender;
            GridViewRow linha = r.Parent.Parent as GridViewRow;
            if (linha == null)
                return;
    
            ProdutosPedido pp = linha.DataItem as ProdutosPedido;
            if (pp == null || pp.IdProdPedAnterior == null)
                return;
    
            r.MaximumValue = ProdutosPedidoDAO.Instance.ObtemQtde(pp.IdProdPedAnterior.Value).ToString();
            r.ErrorMessage = "Valor entre 1 e " + r.MaximumValue;
        }
    
        protected string ExibirTroca()
        {
            return PedidoConfig.PermitirTrocaPorPedido ? "" : "display: none";
        }
    
        protected bool HabilitarPodeTrocar()
        {
            return grdProdutos.Rows.Count == 1 && !grdProdutos.Rows[0].Visible;
        }

        protected void odsPedidoRepos_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var pedido = ((PedidoReposicao)e.InputParameters[0]);
            var idLoja = Conversoes.StrParaUint(((DropDownList)dtvPedidoRepos.FindControl("drpLoja")).SelectedValue);

            PedidoDAO.Instance.AtualizaLoja(pedido.IdPedido, idLoja);
        }
    }
}
