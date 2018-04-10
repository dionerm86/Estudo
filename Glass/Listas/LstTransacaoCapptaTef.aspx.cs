using Glass.Data.DAL;
using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstTransacaoCapptaTef : System.Web.UI.Page
    {
        private bool corAlternada = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(LstTransacaoCapptaTef));
        }

        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }

        /// <summary>
        /// Finaliza uma transação que ficou na situação processando
        /// </summary>
        /// <param name="tipoRecebimento"></param>
        /// <param name="idReferencia"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public void FinalizarTransacaoProcessando(int tipoRecebimento, int idReferencia)
        {
            TransacaoCapptaTefDAO.Instance.FinalizarTransacaoProcessando((Data.Helper.UtilsFinanceiro.TipoReceb)tipoRecebimento, idReferencia);
        }
    }
}