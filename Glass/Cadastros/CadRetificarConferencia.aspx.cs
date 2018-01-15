using System;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Text;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetificarConferencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadRetificarConferencia));
        }
    
        [Ajax.AjaxMethod()]
        public string Retificar(string idConferente, string idsPedido, string dataEfetuar, string idsRetificar)
        {
            try
            {
                PedidoConferenciaDAO.Instance.RetificarConferencia(Glass.Conversoes.StrParaUint(idConferente), idsPedido.TrimEnd(','), DateTime.Parse(dataEfetuar), idsRetificar.TrimEnd(','));
                return "ok\tConferências retificadas.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao retificar conferências.", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string GetPedidos(string idConferente, string dataEfetuar)
        {
            try
            {
                var lstPedidos = PedidoConferenciaDAO.Instance.GetForRpt(0, Glass.Conversoes.StrParaUint(idConferente), 0, null,
                    (int)PedidoConferencia.SituacaoConferencia.EmAndamento, 0, dataEfetuar, null, null);
    
                StringBuilder str = new StringBuilder();
    
                foreach (PedidoConferencia p in lstPedidos)
                {
                    str.Append(p.IdPedido + ";");
                    str.Append(p.NomeLoja.Replace("'", "").Replace("|", "").Replace(";", "") + ";");
                    str.Append(p.NomeInicialCli.Replace("|", "").Replace(";", "") + ";");
                    str.Append(p.TelCli.Replace("|", "").Replace(";", "") + ";");
                    str.Append(p.LocalObra.Replace("|", "").Replace(";", "") + ";");
                    str.Append(p.DataEntrega != null ? p.DataEntrega.Value.ToString("dd/MM/yyyy") : String.Empty);
                    str.Append("|");
                }
    
                return lstPedidos.Count == 0 ? String.Empty : "ok\t" + str.ToString().TrimEnd('|');
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar conferências.", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string GetPedido(string idPedido)
        {
            try
            {
                Glass.Data.Model.Pedido pedido = PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idPedido));
                Cliente cliente = ClienteDAO.Instance.GetElementByPrimaryKey(pedido.IdCli);
    
                if (pedido.IdPedido == 0)
                    return "Erro\tPedido não encontrado.";
    
                if (pedido.Situacao != Glass.Data.Model.Pedido.SituacaoPedido.EmConferencia &&
                    pedido.Situacao != Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia)
                    return "Erro\tEste pedido não está em conferência.";
    
                PedidoConferencia pedConf = PedidoConferenciaDAO.Instance.GetElementByPrimaryKey(pedido.IdPedido);
    
                if (pedConf.Situacao == (int)PedidoConferencia.SituacaoConferencia.EmAndamento)
                    return "Erro\tA conferência deste pedido já está em andamento.";
    
                if (pedConf.Situacao == (int)PedidoConferencia.SituacaoConferencia.Finalizada)
                    return "Erro\tA conferência deste pedido já está finalizada.";
    
                StringBuilder str = new StringBuilder();
                str.Append("ok\t" + pedido.IdPedido + "\t");
                str.Append(LojaDAO.Instance.GetElementByPrimaryKey(pedido.IdLoja).NomeFantasia.Replace("'", "").Replace("|", "").Replace(";", "") + "\t");
                str.Append(cliente.Nome.Replace("|", "").Replace(";", "") + "\t");
                str.Append(cliente.Telefone.Replace("|", "").Replace(";", "") + "\t");
                str.Append(pedido.LocalizacaoObra.Replace("|", "").Replace(";", "") + "\t");
                str.Append(pedido.DataEntrega != null ? pedido.DataEntrega.Value.ToString("dd/MM/yyyy") : String.Empty);
    
                return str.ToString();
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao carregar pedidos.", ex);
            }
        }
    }
}
