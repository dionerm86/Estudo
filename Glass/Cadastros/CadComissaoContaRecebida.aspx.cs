using Glass.Data.DAL;
using System;
using System.Text;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadComissaoContaRecebida : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadComissaoContaRecebida));

            if (!IsPostBack)
            {
                // Coloca como período o mês anterior inteiro
                DateTime final = DateTime.Now.AddDays(-DateTime.Now.Day);
                DateTime inicial = final.AddDays(1 - final.Day);

                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = inicial.ToString("dd/MM/yyyy");

                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = final.ToString("dd/MM/yyyy");

                // Coloca como data de vencimento para a parcela da comissão a
                // data atual acrescida de 1 mês
                ((TextBox)ctrlDataComissao.FindControl("txtData")).Text = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");
            }
        }

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string GetContas(string idFunc, string idLoja, string tipoContaContabil, string dataIni, string dataFim, string dataRecIni, string dataRecFim)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var c in ContasReceberDAO.Instance.ObtemContasRecebidasParaComissao(null, idFunc.StrParaUint(), idLoja.StrParaInt(), tipoContaContabil, dataIni, dataFim, dataRecIni, dataRecFim))
            {
                sb.Append(c.IdContaR + "\t");
                sb.Append(c.IdNomeCli + "\t");
                sb.Append(c.ValorVec.ToString("C") + "\t");
                sb.Append(c.DataVec.ToShortDateString() + "\t");
                sb.Append(c.ValorRec.ToString("C") + "\t");
                sb.Append(c.DataRec.Value.ToShortDateString() + "\t");
                sb.Append(c.DescricaoContaContabil + "\t");
                sb.Append(c.ValorImpostos.ToString("C") + "\t");
                sb.Append(c.ValorBaseCalcComissao.ToString("C") + "\t");
                sb.Append(c.ValorComissao.ToString("C") + "\t");
                sb.Append(c.Referencia + "\t");
                sb.Append(c.NumParcString + "\t");
                sb.Append("\n");
            }

            return sb.ToString().TrimEnd('\n');
        }

        [Ajax.AjaxMethod]
        public void GerarComissao(string idsContas, string idFunc, string dataContaPg, string dataCadIni, string dataCadFim, string dataRecIni, string dataRecFim)
        {
            ComissaoDAO.Instance.GerarComissaoContasRecebidas(idFunc.StrParaUint(), idsContas, dataContaPg, dataCadIni, dataCadFim, dataRecIni, dataRecFim);
        }

        #endregion
    }
}