using System;
using System.Text;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass;

namespace WebGlass.Business.Medicao.Ajax
{
    public interface IBuscarEValidar
    {
        string GetMedicao(string idMedicao);
        string VerificarLimite(string idMedicao, string data);
        string NumMedicoesDia(string data);
        string GetCli(string idCliente);
        string VerificarPodeAssociarOrcamento(string idOrcamento);
        string VerificarPodeAssociarPedido(string idPedido);
        string VerificarMedicaoDefinitivaOrcamento(string idMedicao);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetMedicao(string idMedicao)
        {
            try
            {
                var medicao = MedicaoDAO.Instance.GetMedicao(Glass.Conversoes.StrParaUint(idMedicao));

                if (medicao.IdMedicao == 0)
                    return "Erro\tMedição não encontrada.";

                if (medicao.Situacao == 2)
                    return "Erro\tEsta Medição já está em andamento.";

                if (medicao.Situacao == 3)
                    return "Erro\tMedição já está finalizada.";

                if (medicao.Situacao == 5)
                    return "Erro\tMedição está finalizada.";

                StringBuilder str = new StringBuilder();
                str.Append("ok\t" + medicao.IdMedicao + "\t");
                str.Append(medicao.NomeCliente.Replace("'", "") + "\t");
                str.Append(medicao.DescrTurno + "\t");
                str.Append(medicao.DescrSituacao + "\t");
                str.Append(medicao.DataMedicao != null ? medicao.DataMedicao.Value.ToString("dd/MM/yyyy") : String.Empty);

                return str.ToString();
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao atribuir medições ao medidor.", ex);
            }
        }

        public string VerificarLimite(string idMedicao, string data)
        {
            try
            {
                if (OrcamentoConfig.LimiteDiarioMedicoes == 0)
                    return "Ok;true;" + data;

                uint? id = !String.IsNullOrEmpty(idMedicao) ? (uint?)Glass.Conversoes.StrParaUint(idMedicao) : null;
                DateTime dataMedicao = DateTime.Parse(data);

                int numMedicoesDia = MedicaoDAO.Instance.GetNumMedicoes(dataMedicao, id);
                if (id == null)
                    numMedicoesDia++;

                if (numMedicoesDia <= OrcamentoConfig.LimiteDiarioMedicoes)
                    return "Ok;true;" + data;
                else
                {
                    DateTime? proximaData = MedicaoDAO.Instance.GetMedicaoDay(dataMedicao, id);
                    if (proximaData != null)
                        return "Ok;false;" + proximaData.Value.ToString("dd/MM/yyyy");
                }

                throw new ApplicationException("Número de medições nessa data ultrapassa o limite diário.");
            }
            catch (Exception ex)
            {
                string erro = ex is ApplicationException ? ex.Message : Glass.MensagemAlerta.FormatErrorMsg("Erro ao verificar limite de medições.", ex);
                return "Erro;" + erro;
            }
        }

        public string NumMedicoesDia(string data)
        {
            if (OrcamentoConfig.LimiteDiarioMedicoes == 0)
                return "";

            DateTime dataMedicao = DateTime.Parse(data);
            return MedicaoDAO.Instance.GetNumMedicoes(dataMedicao).ToString();
        }

        /// <summary>
        /// Retorna o dados do cliente, para a Medição, separados por |
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public string GetCli(string idCli)
        {
            try
            {
                var cli = ClienteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idCli));
                if (cli == null || cli.IdCli == 0)
                    return "";

                var enderecoBuscar = "";
                var numeroBuscar = "";
                var bairroBuscar = "";
                var idCidadeBuscar = new uint();
                var cepBuscar = "";
                var complBuscar = "";

                if (!string.IsNullOrEmpty(cli.EnderecoEntrega))
                {
                    enderecoBuscar = cli.EnderecoEntrega;
                    numeroBuscar = cli.NumeroEntrega;
                    bairroBuscar = cli.BairroEntrega;
                    idCidadeBuscar = (uint)cli.IdCidadeEntrega.GetValueOrDefault();
                    cepBuscar = cli.CepEntrega;
                    complBuscar = cli.ComplEntrega;
                }
                else
                {
                    enderecoBuscar = cli.Endereco;
                    numeroBuscar = cli.Numero;
                    bairroBuscar = cli.Bairro;
                    idCidadeBuscar = (uint)cli.IdCidade.GetValueOrDefault();
                    cepBuscar = cli.Cep;
                    complBuscar = cli.Compl;
                }

                var local = cli.Nome + "|" + cli.Telefone + "|" + cli.TelCel + "|" + (cli.Email != null ? cli.Email.Split(';')[0] : null) + "|" + enderecoBuscar + " n.º " +
                    numeroBuscar + "|" + bairroBuscar + "|" + (idCidadeBuscar > 0 ? CidadeDAO.Instance.GetNome(idCidadeBuscar) : "") +
                    "|" + cepBuscar + "|" + complBuscar;

                return local;
            }
            catch
            {
                return "";
            }
        }

        public string VerificarPodeAssociarOrcamento(string idOrcamento)
        {
            if (string.IsNullOrEmpty(idOrcamento))
                return "Ok;";

            if (Glass.Conversoes.StrParaUintNullable(idOrcamento).GetValueOrDefault() == 0 ||
                !OrcamentoDAO.Instance.Exists(Glass.Conversoes.StrParaUintNullable(idOrcamento).GetValueOrDefault()))
                return "Erro;Este orçamento não existe.";

            var possuiMedicaoDefinitiva = MedicaoDAO.Instance.ObterMedicaoDefinitivaPeloIdOrcamento(Glass.Conversoes.StrParaInt(idOrcamento)) > 0;

            var orcamento = OrcamentoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idOrcamento));

            if (possuiMedicaoDefinitiva &&(orcamento.Situacao == (int)Glass.Data.Model.Orcamento.SituacaoOrcamento.Negociado ||
                orcamento.Situacao == (int)Glass.Data.Model.Orcamento.SituacaoOrcamento.EmNegociacao))
                return "Ok;possuiMedicaoDefinitiva";

            if (orcamento.Situacao == (int)Glass.Data.Model.Orcamento.SituacaoOrcamento.Negociado ||
                orcamento.Situacao == (int)Glass.Data.Model.Orcamento.SituacaoOrcamento.EmNegociacao)
                return "Ok;";
            else
                return "Erro;O orçamento deve estar nas situações Negociado ou Em Negociação para que seja associado à medição através desta tela.";
        }

        public string VerificarPodeAssociarPedido(string idPedido)
        {
            if (string.IsNullOrEmpty(idPedido))
                return "Ok;";

            if (idPedido.StrParaUintNullable().GetValueOrDefault() == 0 ||
                !PedidoDAO.Instance.Exists(idPedido.StrParaUintNullable().GetValueOrDefault()))
                return "Erro;Este pedido não existe.";

            var situacaoPedido = PedidoDAO.Instance.ObtemSituacao(idPedido.StrParaUint());

            if (situacaoPedido == Glass.Data.Model.Pedido.SituacaoPedido.Ativo ||
                situacaoPedido == Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia ||
                situacaoPedido == Glass.Data.Model.Pedido.SituacaoPedido.Conferido ||
                situacaoPedido == Glass.Data.Model.Pedido.SituacaoPedido.ConfirmadoLiberacao)
                return "Ok;";
            else
                return string.Format("Erro;O pedido deve estar nas situações {0} para que seja associado à medição através desta tela.",
                    PedidoConfig.LiberarPedido ? "Ativo, Conferido COM ou Confirmado PCP" : "Ativo ou Conferido COM");
        }

        /// <summary>
        /// Retorna "true" caso a medição informada seja a medição definitiva de algum orçamento e "false" caso não seja.
        /// </summary>
        public string VerificarMedicaoDefinitivaOrcamento(string idMedicao)
        {
            try
            {
                if (idMedicao.StrParaIntNullable().GetValueOrDefault() == 0)
                    return "Ok;false";

                var medicao = MedicaoDAO.Instance.GetMedicao(idMedicao.StrParaUint());

                return (medicao.MedicaoDefinitiva && medicao.IdOrcamento > 0) ? "Ok;true" : "Ok;false";
            }
            catch (Exception ex)
            {
                string erro = ex is ApplicationException ? ex.Message : MensagemAlerta.FormatErrorMsg("Erro ao verificar medição definitiva do orçamento.", ex);
                return "Erro;" + erro;
            }
        }
    }
}