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
        /// <summary>
        /// Identifica se pode exibir o preço anterior.
        /// </summary>
        public bool ExibirPrecoAnterior
        {
            get { return UserInfo.GetUserInfo.TipoUsuario == (int)Glass.Data.Helper.Utils.TipoFuncionario.Administrador; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkImprimir.Visible = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarProduto);

            grdProduto.Columns[13].Visible = PedidoConfig.LiberarPedido;
        }

        protected void drpGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Insere o tipo de ordenação por subgrupo de produto caso algum grupo seja selecionado e exclui esta ordenação caso todos os grupos estejam sendo buscados.
            if (drpGrupo.SelectedIndex > 0 && drpOrdenar.Items.FindByText("SubGrupo") == null)
                drpOrdenar.Items.Insert(3, new ListItem("SubGrupo", "IdSubgrupoProd"));
            else if (drpGrupo.SelectedIndex == 0 && drpOrdenar.Items.FindByValue("3") != null)
                drpOrdenar.Items.RemoveAt(3);
        }
    }
}
