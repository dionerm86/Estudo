using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class ChartOrcaVendasDAO : BaseDAO<ChartOrcaVendas, ChartOrcaVendasDAO>
    {
        //private ChartOrcaVendasDAO() { }

        public ChartOrcaVendas[] GetOrcaVendas(uint idLoja, uint idVendedor, IEnumerable<int> situacao, int tipoFunc, string dataIni, string dataFim)
        {
            var periodoIni = DateTime.Parse(dataIni);
            var periodoFim = DateTime.Parse(dataFim).AddDays(1);

            var listOrcaVendas = new List<ChartOrcaVendas>();

            while (periodoIni < periodoFim)
            {
                var serie = new ChartOrcaVendas();
                serie.Periodo = periodoIni.ToString("MMM-yy");

                var login = UserInfo.GetUserInfo;
                var administrador = login.IsAdministrador;
                var cliente = login.IsCliente;
                var emitirGarantia = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantia);
                var emitirReposicao = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoReposicao);
                var emitirPedidoFuncionario = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoFuncionario);

                var orcamento = GraficoOrcamentosDAO.Instance.GetOrcamentos(idLoja, idVendedor, situacao, periodoIni.ToString("dd/MM/yyyy"), periodoIni.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy"), 0, false);
                var venda = ChartVendasDAO.Instance.GetVendas((int?)idLoja, tipoFunc, (int?)idVendedor, 0, 0, null,
                    periodoIni.ToString("dd/MM/yyyy"), periodoIni.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy"), null, 0,
                    cliente, administrador, emitirGarantia, emitirReposicao, emitirPedidoFuncionario);

                if (orcamento.Count > 0 && venda.Length > 0)
                {
                    serie.Orcamento = orcamento[0].TotalVenda.ToString();
                    serie.Venda = venda[0].TotalVenda.ToString();
                }

                if (string.IsNullOrEmpty(serie.Orcamento))
                    serie.Orcamento = "0";

                if (string.IsNullOrEmpty(serie.Venda))
                    serie.Venda = "0";

                listOrcaVendas.Add(serie);

                periodoIni = periodoIni.AddMonths(+1);
            }

            return listOrcaVendas.ToArray();
        }

        public ChartOrcaVendasImagem[] ObterChartOrcaVendasImagem()
        {
            return new ChartOrcaVendasImagem[0];
        }
    }
}
