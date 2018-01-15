using System;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoEstornoItemCarregamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SetMotivoEstornoItemCarregamento));
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Efetua o estorno de itens do carregamento
        /// </summary>
        /// <param name="idsItensCarregamento"></param>
        /// <param name="idCarregamento"></param>
        /// <param name="motivo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string EstornoCarregamento(string idsItensCarregamento, string idCarregamento, string motivo)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.EstornoCarregamento(idsItensCarregamento, Conversoes.StrParaUintNullable(idCarregamento), motivo);
        }

        #endregion
    }
}
