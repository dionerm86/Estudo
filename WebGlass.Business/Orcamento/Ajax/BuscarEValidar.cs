using System;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.Orcamento.Ajax
{
    public interface IBuscarEValidar
    {
        string GetDadosOrcamento(string idOrcamentoStr);
        string GetCli(string idClienteStr);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetDadosOrcamento(string idOrcamentoStr)
        {
            try
            {
                uint idOrcamento = Glass.Conversoes.StrParaUint(idOrcamentoStr);
                uint? idClienteOrca = OrcamentoDAO.Instance.ObtemIdCliente(null, idOrcamento);
                int? tipoEntregaOrca = OrcamentoDAO.Instance.ObtemTipoEntrega(idOrcamento);

                string idCliente = idClienteOrca > 0 ? idClienteOrca.ToString() : "";
                bool revenda = idClienteOrca > 0 ? ClienteDAO.Instance.IsRevenda(null, idClienteOrca.Value) : false;
                string tipoEntrega = tipoEntregaOrca > 0 ? tipoEntregaOrca.ToString() : ((int)Glass.Data.Model.Orcamento.TipoEntregaOrcamento.Balcao).ToString();

                return "Ok;" + idCliente + ";" + revenda + ";" + OrcamentoDAO.Instance.RecuperaPercComissao(idOrcamento).ToString().Replace(",", ".") + ";" + tipoEntrega;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recuperar dados do orçamento.", ex);
            }
        }

        /// <summary>
        /// Retorna o dados do cliente, para Orçamento, separados por |
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public string GetCli(string idCli)
        {
            try
            {
                var cli = ClienteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idCli));

                if (cli == null || cli.IdCli == 0)
                    return "Erro|Cliente não encontrado.";

                else if (cli.Situacao == (int)Glass.Data.Model.SituacaoCliente.Inativo && !OrcamentoConfig.TelaCadastro.PermitirInserirClienteInativoBloqueado)
                    return "Erro|Cliente inativo. Motivo: " + cli.Obs;

                else if (cli.Situacao == (int)Glass.Data.Model.SituacaoCliente.Cancelado)
                    return "Erro|Cliente cancelado. Motivo: " + cli.Obs;

                else if (cli.Situacao == (int)Glass.Data.Model.SituacaoCliente.Bloqueado && !OrcamentoConfig.TelaCadastro.PermitirInserirClienteInativoBloqueado)
                    return "Erro|Cliente bloqueado. Motivo: " + cli.Obs;

                string[] obs = Glass.Data.Helper.MetodosAjax.GetObsCli(idCli).Split(';');
                if (obs[0] == "Erro")
                    return String.Join(";", obs);

                if (cli.BloquearPedidoContaVencida)
                {
                    if (ContasReceberDAO.Instance.ClientePossuiContasVencidas((uint)cli.IdCli))
                        obs[1] += " <br/>Cliente bloqueado. Motivo: Contas a receber em atraso.";
                }

                if (PedidoConfig.TelaCadastro.ExibirCreditoClienteAoBuscar)
                    obs[1] += " <br/>Crédito: " + cli.Credito;

                var local = cli.Nome + "|" + cli.Telefone + "|" + cli.TelCel + "|" + (cli.Email != null ? cli.Email.Split(',')[0] : null) + "|" + cli.Endereco + " n.º " +
                    cli.Numero + "|" + cli.Bairro + "|" + CidadeDAO.Instance.GetNome((uint?)cli.IdCidade) + "/" + CidadeDAO.Instance.GetNomeUf(null, (uint?)cli.IdCidade) + "|" + cli.Cep + "|" + cli.Compl + "|" +
                    (Glass.Configuracoes.PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido ? cli.IdFunc.GetValueOrDefault(0) : 0) + "|" + cli.CpfCnpj + "|" + cli.ObsNfe +
                    "|" + obs[1];

                return local;
            }
            catch
            {
                return "";
            }
        }
    }
}
