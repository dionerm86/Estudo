using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.NFeUtils;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlConsultaCadCliSintegra : BaseUserControl
    {
        #region Variaveis Locais
    
        private uint? _idCliente;
    
        private Control _ctrlCliente;
    
        #endregion
    
        #region Propiedades
    
        /// <summary>
        /// Identificador do cliente a ser pesquisado
        /// </summary>
        public uint? IdCliente
        {
            get { return _idCliente; }
            set { _idCliente = value; }
        }
    
        /// <summary>
        /// Controle que possui o identificador do cliente
        /// </summary>
        public Control CtrlCliente
        {
            get { return _ctrlCliente; }
            set { _ctrlCliente = value; }
        }
    
        #endregion
    
        #region Metodos AJAX
    
        /// <summary>
        /// Realiza a consulta da situação do cadastro do contribuinte
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string ConsultaSitCadContribuinte(string idCliente)
        {
            try
            {
                uint idCli = Glass.Conversoes.StrParaUint(idCliente);
    
                Cliente cli = ClienteDAO.Instance.GetElement(idCli);
    
                if (cli == null)
                    return "Cliente não encontrado.";
    
                string retorno =  ConsultaSituacao.ConsultaSitCadastroContribuinte(cli.Uf, cli.CpfCnpj);
    
                if (cli.Situacao == 2 && 
                    cli.Obs.Contains("Última pesquisa ao cadastro do sintegra há mais de") && 
                    retorno.Contains("Situação: Habilitado."))
                    return "confirm&&" + retorno;
                else
                    return "alert&&" + retorno;
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService.", ex);
            }
    
        }
    
        /// <summary>
        /// Ativa o cliente
        /// </summary>
        /// <param name="idCliente"></param>
        [Ajax.AjaxMethod()]
        public void AtivarCliente(string idCliente)
        {
            uint idCli = Glass.Conversoes.StrParaUint(idCliente);
    
            ClienteDAO.Instance.AlteraSituacao(idCli);
    
            Cliente cli = ClienteDAO.Instance.GetElement(idCli);
            if (cli != null && cli.Situacao == 1)
            {
                string strToRemove = "Última pesquisa ao cadastro do sintegra há mais de " +
                    FinanceiroConfig.PeriodoInativarClienteUltimaConsultaSintegra + " dias.";
    
                if (cli.Obs.Contains(strToRemove))
                {
                    cli.Obs = cli.Obs.Remove(cli.Obs.IndexOf(strToRemove), strToRemove.Length);

                    cli.IdRota = (int)ClienteDAO.Instance.ObtemIdRota((uint)idCli);
                    ClienteDAO.Instance.Update(cli);
                }
            }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlConsultaCadCliSintegra));
        }
    
        protected void imgConSit_PreRender(object sender, EventArgs e)
        {
            if (_idCliente.HasValue)
                imgConSit.OnClientClick = "ConsSitCadContr('" + _idCliente.Value + "'); return false;";
            else if (_ctrlCliente != null)
                imgConSit.OnClientClick = "ConsSitCadContr(document.getElementById('" + _ctrlCliente.ClientID + "').value); return false;";
            else
                imgConSit.Visible = false;
        }
    }
}
