using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Text;

namespace Glass.UI.Web.Otimizacao.Views
{
    public partial class SelecionaPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(SelecionaPedido));
        }
    
        [Ajax.AjaxMethod()]
        public string ObterDadosPedido(string id)
        {
            try
            {
                StringBuilder str = new StringBuilder();
    
                uint idPedido = Glass.Conversoes.StrParaUint(id);

                Glass.Data.Model.Pedido pedido = PedidoDAO.Instance.GetElement(idPedido);
    
                pedido.QtdePecas = PedidoDAO.Instance.ObtemQuantidadePecas(idPedido);
    
                str.Append(pedido.IdPedido + ";");
                str.Append(pedido.NomeCliente + ";");
                str.Append(pedido.NomeFunc + ";");
                str.Append(pedido.DescrTipoVenda + ";");
                str.Append("R$" + pedido.Total + ";");
                str.Append(pedido.TotM + ";");
                str.Append(pedido.QtdePecas + ";");
                str.Append(pedido.DataPedido.ToString("dd/MM/yyyy") + ";");
                str.Append(pedido.DescrSituacaoPedido + ";");
    
                return "ok\t" + str.ToString().Replace('\t', ' ').TrimEnd('|');
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string ObterDadosOrcamento(string id)
        {
            try
            {
                StringBuilder str = new StringBuilder();
    
                uint idOrca = Glass.Conversoes.StrParaUint(id);
    
                Orcamento orcamento = OrcamentoDAO.Instance.GetElement(idOrca);
    
                orcamento.QtdePecas = OrcamentoDAO.Instance.ObtemQuantidadePecas(idOrca);
    
                str.Append(orcamento.IdOrcamento + ";");
                str.Append(orcamento.NomeCliente + ";");
                str.Append(orcamento.NomeFuncionario + ";");
                str.Append(orcamento.DescrTipoOrcamento + ";");
                str.Append("R$" + orcamento.Total + ";");
                str.Append("" + ";");
                str.Append(orcamento.QtdePecas + ";");
                str.Append(orcamento.DataOrcamento + ";");
                str.Append(orcamento.DescrSituacao + ";");
    
                return "ok\t" + str.ToString().Replace('\t', ' ').TrimEnd('|');
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    
    }
}
