using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstCustoFixoGerados : System.Web.UI.Page
    {
        #region Variaveis Locais

        bool exibirCentroCusto = false;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            exibirCentroCusto = Glass.Configuracoes.FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal() > 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void drpLoja_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Verifica se o icone do centro de custo deve ser exibido
        /// </summary>
        /// <returns></returns>
        protected bool ExibirCentroCusto()
        {
            return exibirCentroCusto;
        }
    }
}
