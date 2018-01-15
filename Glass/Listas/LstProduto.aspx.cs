using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstProduto : System.Web.UI.Page
    {
        #region Propriedades

        /// <summary>
        /// Identifica se pode exibir o preço anterior.
        /// </summary>
        public bool ExibirPrecoAnterior
        {
            get { return UserInfo.GetUserInfo.TipoUsuario == (int)Glass.Data.Helper.Utils.TipoFuncionario.Administrador; }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            odsProduto.Register();
            grdProduto.Register(false, false);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            lnkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarProduto);
            lnkImprimir.Visible = lnkInserir.Visible;
    
            grdProduto.Columns[13].Visible = PedidoConfig.LiberarPedido;
    
            if (!IsPostBack)
            {
                // Mantém filtros selecionados
                string idGrupo = Request["gr"];
                string idSubgrupo = Request["sb"];
    
                if (!String.IsNullOrEmpty(idGrupo) && idGrupo != "0")
                {
                    drpGrupo.SelectedValue = idGrupo;
                    drpGrupo.DataBind();
                    drpSubgrupo.DataBind();
                }
    
                if (!String.IsNullOrEmpty(idSubgrupo) && idSubgrupo != "0")
                    drpSubgrupo.SelectedValue = idSubgrupo;
    
                // Carrega o DropDownList de colunas
                if (!ProdutoConfig.TelaListagem.UsarRelatorioProdutosDiferente)
                {
                    cbdColunas.Items.Add(new ListItem("Balcão", "3"));
                    cbdColunas.Items.Add(new ListItem("Obra", "4"));
                    cbdColunas.Items.Add(new ListItem(new Produto().DescrAtacadoRepos, "5"));
                    cbdColunas.Items.Add(new ListItem("Disp. Estoque", "6"));
                    cbdColunas.Items.Add(new ListItem("Reserva", "7"));
                    cbdColunas.Items.Add(new ListItem("Estoque", "8"));
                }
                else
                {
                    cbdColunas.Items.Add(new ListItem("Custo Forn.", "1"));
                    cbdColunas.Items.Add(new ListItem("Custo Imp.", "2"));
                    cbdColunas.Items.Add(new ListItem("Balcão", "3"));
                    cbdColunas.Items.Add(new ListItem("Obra", "4"));
                    cbdColunas.Items.Add(new ListItem(new Produto().DescrAtacadoRepos, "5"));
                    cbdColunas.Items.Add(new ListItem("Disp. Estoque", "6"));
                }
            }
    
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarProduto))
            {
                grdProduto.Columns[0].Visible = false;
                lnkInserir.Visible = false;
            }
    
            if (Session["pgIndProd"] != null)
                grdProduto.PageIndex = Glass.Conversoes.StrParaInt(Session["pgIndProd"].ToString());
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadProduto.aspx");
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void drpGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
    
            // Insere o tipo de ordenação por subgrupo de produto caso algum grupo seja selecionado e exclui esta ordenação caso todos os grupos estejam sendo buscados.
            if (drpGrupo.SelectedIndex > 0 && drpOrdenar.Items.FindByText("SubGrupo") == null)
                drpOrdenar.Items.Insert(3, new ListItem("SubGrupo", "IdSubgrupoProd"));
            else if (drpGrupo.SelectedIndex == 0 && drpOrdenar.Items.FindByValue("3") != null)
                drpOrdenar.Items.RemoveAt(3);
        }
    
        protected void drpSubgrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void grdProduto_PageIndexChanged(object sender, EventArgs e)
        {
            if (grdProduto.PageIndex > 0)
            {
                if (Session["pgIndProd"] == null)
                    Session.Add("pgIndProd", grdProduto.PageIndex.ToString());
                else
                    Session["pgIndProd"] = grdProduto.PageIndex.ToString();
            }
        }
    }
}
