using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstLoja : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdLoja.Register();
            odsLoja.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkInserir.Visible = Configuracoes.Geral.PermitirInserirNovaLoja;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadLoja.aspx");
        }
    
        protected void grdLoja_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Inativar")
            {
                var negLoja = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.ILojaFluxo>();
                var res = negLoja.AlterarSituacaoLoja(e.CommandArgument.ToString().StrParaInt());

                if (res)
                    grdLoja.DataBind();
                else
                    Glass.MensagemAlerta.ShowMsg(String.Format(
                        "Falha ao ativar/inativar loja. {0}", res.Message), 
                        Page);
            }
        }

        protected string ExibeSituacao(object dataItem)
        {
            var loja = dataItem as Glass.Global.Negocios.Entidades.LojaPesquisa;
            return Colosoft.Translator.Translate(loja.Situacao, "fem").Format();
        }
    }
}
