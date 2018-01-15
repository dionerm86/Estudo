using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaVariosPedidosPcp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.ListaVariosPedidosPcp));
            grupos.Visible = PedidoConfig.DadosPedido.AmbientePedido;
    
            cblGrupoProd.DataBind();
        }
    
        [Ajax.AjaxMethod()]
        public string VerificaPedido(string idPedido)
        {
            try
            {
                PedidoEspelho.SituacaoPedido situacao = PedidoEspelhoDAO.Instance.ObtemSituacao(Glass.Conversoes.StrParaUint(idPedido));
    
                if (situacao == PedidoEspelho.SituacaoPedido.Processando ||
                    situacao == PedidoEspelho.SituacaoPedido.Aberto)
                    return "Erro\tA conferência deste pedido ainda não foi finalizada.";
                
                return "Ok\t";
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Registro não encontrado.") > -1)
                    return "Erro\tErro: Pedido não encontrado. Verifique se foi gerada conferência desse pedido.";
                else
                    return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    
        protected void cblGrupoProd_DataBound(object sender, EventArgs e)
        {
            chkMarcarDesmarcar.Checked = true;
    
            for (int i = 0; i < cblGrupoProd.Items.Count; i++)
            {
                cblGrupoProd.Items[i].Attributes.Add("Valor", cblGrupoProd.Items[i].Value);
                cblGrupoProd.Items[i].Selected = chkMarcarDesmarcar.Checked;
            }
        }
    }
}
