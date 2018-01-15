using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstTransacaoCapptaTef : System.Web.UI.Page
    {
        private bool corAlternada = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(LstTransacaoCapptaTef));
        }

        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }

        #region Métodos AJAX

        /// <summary>
        /// Atualiza os pagamentos feitos com o cappta tef
        /// </summary>
        /// <param name="tipoPagto"></param>
        /// <param name="id"></param>
        /// <param name="checkoutGuid"></param>
        /// <param name="codControle"></param>
        [Ajax.AjaxMethod]
        public void SalvarCancelamento(string tipoPagto, string id, string checkoutGuid, string codControle, string customerReceipt, string merchantReceipt)
        {
            TransacaoCapptaTefDAO.Instance.SalvaCancelamento((Data.Helper.UtilsFinanceiro.TipoReceb)tipoPagto.StrParaInt(), id.StrParaInt(), checkoutGuid, codControle, customerReceipt, merchantReceipt);
        }

        #endregion
    }
}