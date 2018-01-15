using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class ListaTotalLiberacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var exibirTotalEspelho = PCPConfig.ExibirDadosPcpListaAposConferencia;
    
            var liberacoes = LiberarPedidoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]), Glass.Conversoes.StrParaUint(Request["idPedido"]),
                        Glass.Conversoes.StrParaIntNullable(Request["numeroNfe"]), Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaUint(Request["idCliente"]),
                        Request["nomeCliente"], Glass.Conversoes.StrParaInt(Request["liberacaoNf"]), Request["dataIni"], Request["dataFim"],
                        Glass.Conversoes.StrParaInt(Request["situacao"]), Glass.Conversoes.StrParaUint(Request["idLoja"]),null, null);
    
    
            var prodLibPed = new List<ProdutosLiberarPedido>();
    
            var total = new decimal();
            var totm = new float();
            var peso = new double();
            
            foreach(var lib in liberacoes)
            {
                var plp = ProdutosLiberarPedidoDAO.Instance.GetByLiberarPedido(lib.IdLiberarPedido);
                prodLibPed.AddRange(plp);
                total += lib.Total;
            }
    
            foreach (var plp in prodLibPed)
            {
                //total += (plp.Total / (decimal)plp.QtdeProd) * (decimal)plp.Qtde;
                totm += plp.TotM2Calc;
                peso += (plp.Peso / (double)plp.QtdeProd) * (double)plp.Qtde;
            }
    
            Label1.Text = "R$ " + Math.Round(total, 2).ToString();
            Label2.Text = Math.Round(totm, 2).ToString() + " m²";
            Label3.Text = Math.Round(peso, 2).ToString() + " kg";
        }
    }
}
