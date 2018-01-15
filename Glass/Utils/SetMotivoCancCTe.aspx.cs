using System;
using Glass.Data.CTeUtils;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancCTe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var idCte = Request["IdCte"];

            var idCidade = Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetElement(idCte.StrParaUint()).IdCidadeCte;

            var estado = CidadeDAO.Instance.GetElementByPrimaryKey(idCidade).NomeUf;

            switch (estado.ToUpper())
            {
                case "MT":
                    lblMsgRestricaoCancelamento.Text = @"Só é possível cancelar uma conhecimento de transporte que tenha sido autorizada no período 02 horas após a emissão
                                                        Obs: Não é possível cancelar CTE vinculado a um MDF - e. ";
                    lblMsgRestricaoCancelamento.ForeColor = System.Drawing.Color.Red;
                    lblMsgRestricaoCancelamento.Font.Bold = true;
                    break;
                default:
                    lblMsgRestricaoCancelamento.Text = @"O prazo de cancelamento do CTE é de 168 horas. 
                                                        Obs: Não é possível cancelar CTE vinculado a um MDF - e. ";
                    lblMsgRestricaoCancelamento.ForeColor = System.Drawing.Color.Red;
                    lblMsgRestricaoCancelamento.Font.Bold = true;
                    break;
            }
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = EnviaXML.EnviaCancelamento(Glass.Conversoes.StrParaUint(Request["idCte"]), txtMotivo.Text);
    
                Glass.MensagemAlerta.ShowMsg(msg, Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar CTe.", ex, Page);
                return;
            }
        }
    }
}
