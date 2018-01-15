using System;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class ListaTotalPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var exibirTotalEspelho = PCPConfig.ExibirDadosPcpListaAposConferencia;
    
            var pedidos = PedidoDAO.Instance.GetList(Glass.Conversoes.StrParaUint(Request["idPedido"]), Glass.Conversoes.StrParaUint(Request["idLoja"]), Glass.Conversoes.StrParaUint(Request["idCli"]),
                Request["nomeCli"], Request["codCliente"], Glass.Conversoes.StrParaUint(Request["idCidade"]), Request["endereco"], Request["bairro"], Request["complemento"], Request["situacao"],
                Request["situacaoProd"], Request["byVend"], Request["byConf"], Request["maoObra"], Request["maoObraEspecial"], Request["producao"], 
                Glass.Conversoes.StrParaUint(Request["idOrcamento"]), Glass.Conversoes.StrParaFloat(Request["altura"]), Glass.Conversoes.StrParaInt(Request["largura"]), 
                Glass.Conversoes.StrParaInt(Request["diasProntoLib"]), Glass.Conversoes.StrParaFloat(Request["valorDe"]), Glass.Conversoes.StrParaFloat(Request["valorAte"]), Request["dataCadIni"],
                Request["dataCadFim"], Request["dataFinIni"], Request["dataFinFim"], Request["funcFinalizacao"], Request["tipo"], Glass.Conversoes.StrParaInt(Request["fastDelivery"]),
                Glass.Conversoes.StrParaInt(Request["tipoVenda"]), Glass.Conversoes.StrParaInt(Request["origemPedido"]), Request["obs"], null, 0, 0);
    
            decimal total = 0, totm = 0, peso = 0;
            var quantidadePedidos = 0;
    
            foreach (var p in pedidos)
            {
                if (PCPConfig.ExibirDadosPcpListaAposConferencia && p.TemEspelho)
                {
                    total += p.TotalEspelho;
                    totm += (decimal)PedidoEspelhoDAO.Instance.ObtemTotM2(p.IdPedido);
                    peso += (decimal)PedidoEspelhoDAO.Instance.ObtemPeso(p.IdPedido);
                }
                else
                {
                    total += (decimal)p.Total;
                    totm += (decimal)p.TotM;
                    peso += (decimal)p.Peso;
                }
    
                quantidadePedidos++;
            }
    
            lblQuantidadePedidos.Text = quantidadePedidos.ToString();
            lblTotalPedidos.Text = "R$ " + Math.Round(total, 2).ToString();
            lblM2Pedidos.Text = Math.Round(totm, 2).ToString() + " m²";
            lblPesoPedidos.Text = Math.Round(peso, 2).ToString() + " kg";
        }
    }
}
