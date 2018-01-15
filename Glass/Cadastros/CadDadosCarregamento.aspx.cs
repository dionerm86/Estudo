using System;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDadosCarregamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadDadosCarregamento));
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Finaliza uma ordem de carga
        /// </summary>
        /// <param name="veiculo"></param>
        /// <param name="idMotorista"></param>
        /// <param name="dtPrevSaida"></param>
        /// <param name="idLoja"></param>
        /// <param name="idsOCs"></param>
        /// <param name="enviarEmail"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string FinalizaCarregamento(string veiculo, string idMotorista, string dtPrevSaida, string idLoja, string idsOCs, string enviarEmail)
        {
            idLoja = idLoja.Split(',')[0];

            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax
                .FinalizaCarregamento(veiculo, idMotorista, dtPrevSaida, idLoja, idsOCs, enviarEmail);
        }

        [Ajax.AjaxMethod()]
        public string ValidaCarregamentoAcimaCapacidadeVeiculo(string veiculo, string idsOCs)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax
                .ValidaCarregamentoAcimaCapacidadeVeiculo(veiculo, idsOCs);
        }

        #endregion

        public bool EnviarEmailAoFinalizar()
        {
            return Configuracoes.OrdemCargaConfig.PerguntarSeEnviaraEmailAoFinalizar;
        }
    }
}
