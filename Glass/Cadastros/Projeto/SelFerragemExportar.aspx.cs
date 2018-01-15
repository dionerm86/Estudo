using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Linq;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class SelFerragemExportar : Page
    {
        #region Carregamento da tela

        protected void Page_Load(object sender, EventArgs e) { }

        /// <summary>
        /// Método utilizado para atualizar a tela.
        /// </summary>
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdFerragens.PageIndex = 0;
        }

        #endregion

        #region Adicionar ferragem

        /// <summary>
        /// Adiciona todas as ferragems, filtradas, à tela de exportação de ferragens.
        /// </summary>
        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            // Recupera o fluxo de ferragens.
            var ferragemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Projeto.Negocios.IFerragemFluxo>();
            // Recupera as ferragens com base nos parâmetros da tela, através do fluxo de ferragem.
            var ferragens = ferragemFluxo.PesquisarFerragem(txtNomeFerragem.Text, drpFabricanteFerragem.SelectedValue.StrParaIntNullable().GetValueOrDefault(), null);

            // Verifica se alguma ferragem foi recuperada.
            if (ferragens != null && ferragens.Count > 0)
            {
                // Script que será utilizado para adicionar cada ferragem à tela de exportação.
                var scriptBase = "window.opener.setarFerragem({0}, '{1}', '{2}', '{3}', '{4}')";
                // Monta uma lista de scripts para setar cada ferragem na tela de exportação.
                var script = ferragens.Select(f => string.Format(scriptBase, f.IdFerragem, f.Nome, f.NomeFabricante, f.Situacao, f.DataAlteracao)).ToList();
                // Registra o script acima, na tela.
                ClientScript.RegisterClientScriptBlock(typeof(string), "addAll", string.Format("{0}; closeWindow();", string.Join(";", script)), true);
            }
        }

        #endregion
    }
}
