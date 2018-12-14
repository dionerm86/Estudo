using System;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;
using System.Collections.Generic;

namespace WebGlass.Business.Cliente.Ajax
{
    public interface IBuscarEValidar
    {
        string GetCliPedido(string idCli);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetCliPedido(string idCli)
        {
            try
            {
                var cli = ClienteDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCli));

                if (cli == null || cli.IdCli == 0)
                    return "Erro;Cliente não encontrado.";

                var limite = Glass.Data.CalculadoraLimiteCredito.Calculadora.ObterLimite(null, cli);
                var limiteDisponivel = (limite - ContasReceberDAO.Instance.GetDebitos((uint)cli.IdCli, null));
                var temLimite = limite == 0 || limiteDisponivel > 0;
                IEnumerable<string> motivos;

                if (cli.Situacao == (int)Glass.Data.Model.SituacaoCliente.Inativo)
                    return "Erro;Cliente inativo. Motivo: " + cli.Obs;

                else if (cli.Situacao == (int)Glass.Data.Model.SituacaoCliente.Cancelado)
                    return "Erro;Cliente cancelado. Motivo: " + cli.Obs;

                else if (cli.Situacao == (int)Glass.Data.Model.SituacaoCliente.Bloqueado)
                    return "Erro;Cliente bloqueado. Motivo: " + cli.Obs;

                else if (Glass.Data.GerenciadorSituacaoCliente.Gerenciador.VerificarBloqueio(null, cli, out motivos))
                {
                    return $"Erro;Cliente bloqueado. Motivo: {string.Join(";", motivos)}";
                }
                else if (FinanceiroConfig.BloquearEmissaoPedidoLimiteExcedido && !temLimite && FinanceiroConfig.PerguntarVendedorFinalizacaoFinanceiro)
                    return "Erro;Cliente não possui limite suficiente para emitir pedidos. Limite Disponível: " + limiteDisponivel;

                else
                {
                    string[] obs = MetodosAjax.GetObsCli(idCli).Split(';');
                    if (obs[0] == "Erro")
                        return String.Join(";", obs);

                    string empresaEntregaRota = (Glass.Configuracoes.PedidoConfig.TelaCadastro.MarcarPedidosRotaComoEntrega &&
                        RotaClienteDAO.Instance.IsClienteAssociado((uint)cli.IdCli)).ToString().ToLower();

                    DateTime? dataRota = RotaDAO.Instance.GetDataRota((uint)cli.IdCli, DateTime.Now, null);
                    string dataRotaStr = dataRota != null ? dataRota.Value.ToString("dd/MM/yyyy") : "";

                    string dadosComissionado = cli.IdComissionado > 0 ? ";" + cli.IdComissionado.Value + ";" +
                        ComissionadoDAO.Instance.GetNome((uint)cli.IdComissionado.Value) + ";" +
                        ComissionadoDAO.Instance.ObtemPercentual((uint)cli.IdComissionado.Value).ToString() : ";;;";

                    /* Chamado 67492. */
                    if (!FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro && cli.BloquearPedidoContaVencida)
                    {
                        if (ContasReceberDAO.Instance.ClientePossuiContasVencidas((uint)cli.IdCli))
                            obs[1] += " <br/>Cliente bloqueado. Motivo: Contas a receber em atraso.";
                    }

                    if (PedidoConfig.TelaCadastro.ExibirCreditoClienteAoBuscar)
                        obs[1] += " <br/>Crédito: " + cli.Credito;

                    var idLoja = string.Empty;

                    if (cli.IdLoja > 0 &&
                        ((PedidoConfig.ExibirOpcaoDeveTransferir && cli.IdFunc > 0 && FuncionarioDAO.Instance.ObtemIdLoja((uint)cli.IdFunc.Value) != cli.IdLoja) ||
                        Geral.ConsiderarLojaClientePedidoFluxoSistema))
                        idLoja = cli.IdLoja.ToString();

                    var idVendedor =
                        PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido && cli.IdFunc > 0 ? cli.IdFunc.Value : (int)UserInfo.GetUserInfo.CodUser;

                    if (!FuncionarioDAO.Instance.GetVendedores().Any(f => f.IdFunc == idVendedor))
                    {
                        idVendedor = (int)UserInfo.GetUserInfo.CodUser;
                    }


                    return "Ok;" + cli.Nome + ";" + cli.Revenda.ToString().ToLower() + ";" + obs[1] + ";" + dataRotaStr + ";" +
                        empresaEntregaRota + ";" + cli.PagamentoAntesProducao.ToString().ToLower() + ";" + cli.PercSinalMinimo + ";" +
                        idVendedor + dadosComissionado + ";" + cli.PercentualComissao + ";" + idLoja + ";" + cli.IdTransportador;
                }
            }
            catch
            {
                return "Erro;Cliente não encontrado.";
            }
        }
    }
}
