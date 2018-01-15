using System;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCreditarCxDiario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCreditarCxDiario));
        }
    
        [Ajax.AjaxMethod()]
        public string Creditar(string idLoja, string idConta, string valor, string formaEntrada, string obs)
        {
            uint idCxDiario = 0;
    
            try
            {
                uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;
    
                if ((!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                    tipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.Administrador) ||
                    !Geral.ControleCaixaDiario)
                    return "Erro\tApenas funcion�rio com permiss�o de Caixa Di�rio pode efetuar cr�ditos no Caixa Di�rio.";
    
                idCxDiario = CaixaDiarioDAO.Instance.MovCxCredito(Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaUint(idConta), 1, Glass.Conversoes.StrParaInt(formaEntrada), Glass.Conversoes.StrParaDecimal(valor), obs);
    
                return "Ok\tCr�dito efetuado com sucesso.";
            }
            catch (Exception ex)
            {
                if (idCxDiario > 0)
                    CaixaDiarioDAO.Instance.DeleteByPrimaryKey(idCxDiario);
    
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao efetuar cr�dito.", ex);
            }
        }
    }
}
