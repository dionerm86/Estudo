using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstRotas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstRotas));
    
            txtDataFim.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDataInicio.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    
        /// <summary>
        /// Verifica se existe algum ponto dados os filtros passados
        /// </summary>
        /// <param name="idEquipe"></param>
        /// <param name="dtInicio"></param>
        /// <param name="dtFim"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string ExistePontos(string idEquipe, string dtInicio, string dtFim)
        {
            //return PontosRotaDAO.Instance.CheckPontosByEquipe(Glass.Conversoes.StrParaUint(idEquipe), dtInicio, dtFim).ToString();
            return null;
        }
    }
}
