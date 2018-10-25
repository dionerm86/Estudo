using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Genericos
{
    public partial class Recibo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.Genericos.Recibo));
    
            if (PedidoConfig.LiberarPedido)
            {
                drpReferente.Items[1].Text = "Total da Liberação";
                drpReferente.Items[3].Enabled = false;
            }
        }
    
        [Ajax.AjaxMethod()]
        public bool IsLiberacao()
        {
            return PedidoConfig.LiberarPedido;
        }
    
        [Ajax.AjaxMethod()]
        public string GetOrcaProd(string idOrcamento)
        {
            try
            {
                // Busca produtos do orçamento
                var lstProd = ProdutosOrcamentoDAO.Instance.GetForRecibo(Glass.Conversoes.StrParaInt(idOrcamento));
    
                if (lstProd.Count == 0)
                    return "Erro\tO orçamento informado não possui nenhum produto ou não existe.";
    
                string prods = String.Empty;
    
                foreach (ProdutosOrcamento p in lstProd)
                    prods += p.Qtde + " " + p.Descricao + "|";
    
                return "Ok\t" + prods.TrimEnd('|');
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string GetPedidoProd(string idPedido)
        {
            try
            {
                // Busca ambientes do pedido
                var lstAmbiente = AmbientePedidoDAO.Instance.GetForRecibo(Glass.Conversoes.StrParaUint(idPedido));
    
                if (lstAmbiente.Count == 0)
                    return "Erro\tO pedido informado não possui nenhum ambiente ou não existe.";
    
                string prods = String.Empty;
    
                foreach (AmbientePedido ap in lstAmbiente)
                    prods += ap.Qtde + " " + ap.Descricao + "|";
    
                return "Ok\t" + prods.TrimEnd('|');
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetParcelas(string idStr, string isPedidoStr)
        {
            try
            {
                uint id = Glass.Conversoes.StrParaUint(idStr);
                bool isPedido = bool.Parse(isPedidoStr);
                string parcelas = String.Empty;
    
                if (isPedido)
                {
                    var lstContasR = ContasReceberDAO.Instance.GetByPedido(null, id, false, true);
                    if (lstContasR.Count == 0)
                        return "Erro;O pedido informado não possui parcelas ou não existe.";

                    int numParc = 1;
                    foreach (ContasReceber c in lstContasR)
                        parcelas += c.IdContaR + ":" + (numParc++) + "/" + lstContasR.Count + " - " + c.ValorVec.ToString("C") + " com vencimento em " + c.DataVecString + "|";
                }
                else
                {
                    var lstContasR = ContasReceberDAO.Instance.GetByLiberacaoPedido(id, true);
                    if (lstContasR.Count == 0)
                        return "Erro;A liberação informada não possui parcelas ou não existe.";
    
                    int numParc = 1;
                    foreach (ContasReceber c in lstContasR)
                        parcelas += c.IdContaR + ":" + (numParc++) + "/" + lstContasR.Count + " - " + c.ValorVec.ToString("C") + " com vencimento em " + c.DataVecString + "|";
                }
    
                return "Ok;" + parcelas.TrimEnd('|');
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }

        [Ajax.AjaxMethod()]
        public string ValidaDados(string idOrca, string idPed, string idLib)
        {
            var idOrcamento = idOrca != "0" && idOrca != "" ? idOrca.StrParaUint() : 0;
            var idPedido = idPed != "0" && idPed != "" ? idPed.StrParaUint() : 0;
            var idLiberacao = idLib != "0" && idLib != "" ? idLib.StrParaUint() : 0;

            if (idOrcamento > 0 && !OrcamentoDAO.Instance.Exists(idOrcamento))
                return "O orçamento informado não existe.";
            else if (idLiberacao > 0 && !LiberarPedidoDAO.Instance.LiberacaoExists(idLiberacao))
                return "A liberação informada não existe.";
            else if (idPedido > 0 && !PedidoDAO.Instance.PedidoExists(idPedido))
                return "O pedido informado não existe.";

            return string.Empty;
        }

        [Ajax.AjaxMethod()]
        public string ContaEstaPaga(string idContaPagar)
        {
            var idContaP = idContaPagar == "" ? 0 : uint.Parse(idContaPagar);

            var resultado = ContasPagarDAO.Instance.EstaPaga(idContaP);

            return resultado ? "true" : "false";
        }
    }
}
