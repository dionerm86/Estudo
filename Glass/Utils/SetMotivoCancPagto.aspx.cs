using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancPagto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ctrlDataEstorno.Data = DateTime.Now;
                estornoBanco.Visible = ExibirEstornoBanco();
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            FilaOperacoes.Pagamento.AguardarVez();

            try
            {
                DateTime? dataEstorno = chkEstornar.Checked ? ctrlDataEstorno.DataNullable : null;

                if (!string.IsNullOrEmpty(Request["idPagto"]))
                    PagtoDAO.Instance.CancelarPagtoComTransacao(Request["IdPagto"].StrParaUint(), txtMotivo.Text,
                        chkEstornar.Checked, dataEstorno);
                else if (!string.IsNullOrEmpty(Request["idAntecipFornec"]))
                    AntecipacaoFornecedorDAO.Instance.CancelaAntecipFornec(
                        Glass.Conversoes.StrParaUint(Request["idAntecipFornec"]), txtMotivo.Text,
                        dataEstorno.GetValueOrDefault());
                else if (!string.IsNullOrEmpty(Request["idContaRParcCartao"]))
                    ContasReceberDAO.Instance.CancelarRecebimentoParcCartao(Request["idContaRParcCartao"].StrParaUint(),
                        chkEstornar.Checked, dataEstorno, txtMotivo.Text);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
            finally
            {
                FilaOperacoes.Pagamento.ProximoFila();
            }

            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok",
                "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }

        protected bool ExibirEstornoBanco()
        {    
            try
            {
                uint id = 0;
                string nomeCampo = "";
    
                if(!string.IsNullOrEmpty(Request["idPagto"]))
                {
                    id = Glass.Conversoes.StrParaUint(Request["idPagto"]);
                    nomeCampo = "idPagto";
                }
                else if (!string.IsNullOrEmpty(Request["idAntecipFornec"]))
                {
                    id = Glass.Conversoes.StrParaUint(Request["idAntecipFornec"]);
                    nomeCampo = "idAntecipFornec";
                }
                else if (!string.IsNullOrEmpty(Request["idContaRParcCartao"]))
                {
                    id = Glass.Conversoes.StrParaUint(Request["idContaRParcCartao"]);
                    nomeCampo = "idContaR";
                }

                return !String.IsNullOrEmpty(nomeCampo) ? MovBancoDAO.Instance.ExistsByCampo(nomeCampo, id) : false;
            }
            catch
            {
                return false;
            }
        }
    }
}
