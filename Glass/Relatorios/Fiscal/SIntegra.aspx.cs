using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Relatorios.Fiscal
{
    public partial class SIntegra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = FuncoesData.ObtemDataPrimeiroDiaUltimoMes();
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = FuncoesData.ObtemDataUltimoDiaUltimoMes();
            }
        }
    
        protected void ddlLoja_DataBound(object sender, EventArgs e)
        {
            uint idLoja = Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue);
    
            if (idLoja.ToString() != hdfIdLoja.Value || String.IsNullOrEmpty(hdfIdLoja.Value))
            {
                cblRegistros.Items.Clear();
    
                // Monta a CheckBoxList com os registros a serem gerados do Sintegra
                cblRegistros.Items.Add(new ListItem("Registro 50 (Notas Fiscais quanto ao ICMS)", "50"));
                cblRegistros.Items.Add(new ListItem("Registro 51 (Notas Fiscais quanto ao IPI)", "51"));
                cblRegistros.Items.Add(new ListItem("Registro 53 (Substituição tributária)", "53"));
                cblRegistros.Items.Add(new ListItem("Registro 54 (Itens das Notas Fiscais)", "54"));
                cblRegistros.Items.Add(new ListItem("Registro 70 (Nota fiscal de serviço de transporte)", "70"));
                cblRegistros.Items.Add(new ListItem("Registro 74 (Registro de Inventário)", "74"));
                cblRegistros.Items.Add(new ListItem("Registro 75 (Código dos produtos)", "75"));
    
                ConfiguracaoLoja sintegra50 = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.SIntegraRegistro50, idLoja);
                ConfiguracaoLoja sintegra51 = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.SIntegraRegistro51, idLoja);
                ConfiguracaoLoja sintegra53 = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.SIntegraRegistro53, idLoja);
                ConfiguracaoLoja sintegra54 = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.SIntegraRegistro54, idLoja);
                ConfiguracaoLoja sintegra70 = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.SIntegraRegistro70, idLoja);
                ConfiguracaoLoja sintegra74 = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.SIntegraRegistro74, idLoja);
                ConfiguracaoLoja sintegra75 = ConfiguracaoLojaDAO.Instance.GetItem(Config.ConfigEnum.SIntegraRegistro75, idLoja);
    
                // Monta a CheckBoxList com os registros a serem gerados do Sintegra
                cblRegistros.Items.FindByValue("50").Selected = sintegra50 != null ? sintegra50.ValorBooleano : true;
                cblRegistros.Items.FindByValue("51").Selected = sintegra51 != null ? sintegra51.ValorBooleano : true;
                cblRegistros.Items.FindByValue("53").Selected = sintegra53 != null ? sintegra53.ValorBooleano : true;
                cblRegistros.Items.FindByValue("54").Selected = sintegra54 != null ? sintegra54.ValorBooleano : true;
                cblRegistros.Items.FindByValue("70").Selected = sintegra70 != null ? sintegra70.ValorBooleano : true;
                cblRegistros.Items.FindByValue("74").Selected = sintegra74 != null ? sintegra74.ValorBooleano : true;
                cblRegistros.Items.FindByValue("75").Selected = sintegra75 != null ? sintegra75.ValorBooleano : true;
            }
    
            hdfIdLoja.Value = idLoja.ToString();
        }
    
        protected void btnBaixar_Click(object sender, EventArgs e)
        {
            uint idLoja = Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue);            
            DateTime dataIni = DateTime.Parse(((TextBox)ctrlDataIni.FindControl("txtData")).Text);
            DateTime dataFim = DateTime.Parse(((TextBox)ctrlDataFim.FindControl("txtData")).Text);
    
            try
            {
                Response.Redirect(this.ResolveClientUrl("~/Handlers/Fiscal.ashx") + "?tipo=SIntegra&loja=" + idLoja + "&inicio=" +
                    dataIni.ToString("dd/MM/yyyy") + "&fim=" + dataFim.ToString("dd/MM/yyyy") +
                    "&reg50=" + cblRegistros.Items.FindByValue("50").Selected +
                    "&reg51=" + cblRegistros.Items.FindByValue("51").Selected +
                    "&reg53=" + cblRegistros.Items.FindByValue("53").Selected +
                    "&reg54=" + cblRegistros.Items.FindByValue("54").Selected +
                    "&reg70=" + cblRegistros.Items.FindByValue("70").Selected +
                    "&reg74=" + cblRegistros.Items.FindByValue("74").Selected +
                    "&reg75=" + cblRegistros.Items.FindByValue("75").Selected);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar Sintegra.", ex, Page);
            }
        }
    }
}
