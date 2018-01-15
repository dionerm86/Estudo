using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadBenef : System.Web.UI.Page
    {
        #region Métodos Protegidos

        /// <summary>
        /// Método acionado quando a página for iniciada.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!string.IsNullOrEmpty(Request["IdBenefConfig"]))
                dtvBenef.ChangeMode(DetailsViewMode.Edit);

            dtvBenef.Register("~/Listas/LstBenef.aspx");
            odsBenefConfig.Register();
        }

        /// <summary>
        /// Método acionado quando a página for carregada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadBenef));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }

        #endregion

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstBenef.aspx");
        }
    
        [Ajax.AjaxMethod]
        public string PodeRemover(string idBenefConfig, string filho)
        {
            try
            {
                var idBenef = Glass.Conversoes.StrParaUint(idBenefConfig);
                var item = BenefConfigDAO.Instance.GetByNomeParent(filho, idBenef);
    
                if (item == null)
                    return "true";

                return (!BenefConfigDAO.Instance.BenefConfigUsado((uint)item.IdBenefConfig)).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }
    
        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno)
        {
            var dadosProduto = WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetDadosProduto(codInterno);
            return dadosProduto;
        }
    
        protected void txtNome_Load(object sender, EventArgs e)
        {
            var id = Glass.Conversoes.StrParaUintNullable(Request["idBenefConfig"]);
    
            if (id > 0 && BenefConfigDAO.Instance.Exists(id.Value))
            {
                if (BenefConfigDAO.Instance.TemBenefRedondo(id.Value.ToString()))
                    (sender as TextBox).Enabled = false;
            }
        }
    }
}
