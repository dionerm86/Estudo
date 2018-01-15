using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstLiberarPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            grdLiberarPedido.Columns[6].Visible = PedidoConfig.Impostos.CalcularIcmsPedido;
            grdLiberarPedido.Columns[7].Visible = PedidoConfig.Impostos.CalcularIcmsPedido;

            if (SomenteConsulta())
            {
                lnkAnexos.Visible = false;
                lnkImprimir.Visible = false;
                lnkImprimirTotais.Visible = false;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdLiberarPedido.PageIndex = 0;
        }
    
        protected bool ExibirRelatorioCompleto()
        {
            return Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaExpAlmPedidosEntrega ||
                Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaExpAlmPedidosBalcao;
        }

        protected bool ExibirRelatorioCliente()
        {
            return Liberacao.TelaLiberacao.ExibirRelatorioCliente;
        }

        /// <summary>
        /// Verifica se a tela foi aberta através da tela de pedidos por um usuário que não tem acesso ao financeiro,
        /// neste caso o acesso deve ser restrito.
        /// </summary>
        protected bool SomenteConsulta()
        {
            return !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
        }
    
        protected void grdLiberarPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ReenviarEmail")
            {
                var idLiberarPedido = Convert.ToUInt32(e.CommandArgument);
    
                // Envia o e-mail
                Email.EnviaEmailLiberacao(null, idLiberarPedido);
    
                LogAlteracaoDAO.Instance.LogReenvioEmailLiberacao(idLiberarPedido);
    
                Glass.MensagemAlerta.ShowMsg("O e-mail foi adicionado na fila para ser enviado.", Page);
            }
        }
    }
}
