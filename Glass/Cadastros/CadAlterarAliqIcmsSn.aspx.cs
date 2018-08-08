using System;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadAlterarAliqIcmsSn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ConfiguracaoLoja configLoja = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.AliquotaICMSSimplesNacional, (UserInfo.GetUserInfo?.IdLoja).GetValueOrDefault());
                txtAliquotaIcmsSn.Text = configLoja.ValorDecimal.ToString().Replace(".", ",");
            }
        }
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                ConfiguracaoLoja configLoja = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.AliquotaICMSSimplesNacional, (UserInfo.GetUserInfo?.IdLoja).GetValueOrDefault());
                configLoja.ValorDecimal = Glass.Conversoes.StrParaDecimalNullable(txtAliquotaIcmsSn.Text);
                ConfiguracaoLojaDAO.Instance.Update(configLoja);
                
                ClientScript.RegisterClientScriptBlock(typeof(string), "alert", "alert('Alíquota salva com sucesso.'); closeWindow();", true);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao salvar Alíquota de ICMS (Simples Nacional). " + ex.Message);
            }
        }
    }
}
