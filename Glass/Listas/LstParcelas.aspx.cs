using System;
namespace Glass.UI.Web.Listas
{
    public partial class LstParcelas : System.Web.UI.Page
    {
        public string ObtemDescricaoNumeroParcelas(int numeroParcelas)
        {
            if (numeroParcelas == 0)
                return "À vista";
            else if (numeroParcelas == 1)
                return "1 parcela";
            else
                return numeroParcelas + " parcelas";
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdParcelas.Register();
            odsParcelas.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void grdParcelas_Load(object sender, EventArgs e)
        {
            grdParcelas.Columns[4].Visible = Glass.Configuracoes.FinanceiroConfig.UsarDescontoEmParcela;
            grdParcelas.Columns[6].Visible = Configuracoes.PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista;
        }
    }
}
