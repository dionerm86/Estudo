using System;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.OrdemCarga.Ajax
{
    public interface ICarregamento
    {
        string GetIdsOCsParaCarregar(string idCliente, string nomeCliente, string idLoja, string idRota, string idsOCs);
        string ValidaOCForCarregamento(string idOC);
        string FinalizaCarregamento(string veiculo, string idMotorista, string dtPrevSaida, string idLoja, string idsOCs, string enviarEmail);
        string ValidaCarregamentoAcimaCapacidadeVeiculo(string veiculo, string idsOCs);
        string CarregamentoExiste(string idCarregamento);
        void EfetuaLeitura(string idFunc, string idCarregamento, string etiqueta, string idPedidoExp, string numCli, string nomeCli,
            string idOc, string idPedido, string altura, string largura, string numEtqFiltro, uint idClienteExterno, string nomeClienteExterno, uint idPedidoExterno);
        string IsEtiquetaRevenda(string etiqueta);
        int ObterIdPedidoRevenda(string etiqueta);
        string EstornoCarregamento(string idsItensCarregamento, uint? idCarregamento, string motivo);
        string GetIdsPedidosByCarregamento(string idCarregamento);
    }

    public class Carregamento : ICarregamento
    {
        public int ObterIdPedidoRevenda(string etiqueta)
        {
            return Fluxo.CarregamentoFluxo.Instance.ObterIdPedidoRevenda(etiqueta);
        }

        /// <summary>
        /// Recupera as OCs de um cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="idRota"></param>
        /// <param name="idsOCs"></param>
        /// <returns></returns>
        public string GetIdsOCsParaCarregar(string idCliente, string nomeCliente, string idLoja, string idRota, string idsOCs)
        {
            uint idCli = Glass.Conversoes.StrParaUint(idCliente);
            return Fluxo.CarregamentoFluxo.Instance.GetIdsOCsParaCarregar(idCli, nomeCliente, idLoja, idRota, idsOCs);

        }

        /// <summary>
        /// Valida uma OC para inclusão no carregamento
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public string ValidaOCForCarregamento(string idOC)
        {
            try
            {
                Fluxo.CarregamentoFluxo.Instance.ValidaOCForCarregamento(null, Glass.Conversoes.StrParaUint(idOC));

                return "ok;";
            }
            catch (Exception ex)
            {
                return "erro;" + ex.Message;
            }
        }

        /// <summary>
        /// Finaliza um carregamento
        /// </summary>
        /// <param name="veiculo"></param>
        /// <param name="idMotorista"></param>
        /// <param name="dtPrevSaida"></param>
        /// <param name="idLoja"></param>
        /// <param name="idsOCs"></param>
        /// <param name="enviarEmail"></param>
        /// <returns></returns>
        public string FinalizaCarregamento(string veiculo, string idMotorista, string dtPrevSaida, string idLoja, string idsOCs, string enviarEmail)
        {
            try
            {
                return Fluxo.CarregamentoFluxo.Instance.FinalizaCarregamento(veiculo, Glass.Conversoes.StrParaUint(idMotorista), DateTime.Parse(dtPrevSaida),
                    Glass.Conversoes.StrParaUint(idLoja), idsOCs, enviarEmail.ToLower() == "true").ToString();

            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao finalizar carregamento. " + ex.Message);
            }
        }

        public string ValidaCarregamentoAcimaCapacidadeVeiculo(string veiculo, string idsOCs)
        {
            try
            {
                if (string.IsNullOrEmpty(veiculo))
                    throw new Exception("O veiculo não foi informado.");

                if (string.IsNullOrEmpty(idsOCs))
                    throw new Exception("Nenhuma OC foi informada.");

                var capacidadeKG = VeiculoDAO.Instance.ObtemCapacidadeKgVeiculo(veiculo);
                double pesoCarregamento = Fluxo.CarregamentoFluxo.Instance.CalcPesoCarregamento(idsOCs);

                if (pesoCarregamento > capacidadeKG)
                    return "Excedeu;O peso do carregamento excede a capacidade do veículo, deseja continuar?";
                else
                    return "OK;";
            }
            catch (Exception ex)
            {
                return "Erro;Falha ao validar carregamento. "+ ex.Message;
            }
        }

        /// <summary>
        /// Verifica se um carregamento existe.
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public string CarregamentoExiste(string idCarregamento)
        {
            try
            {
                if (!CarregamentoDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCarregamento)))
                    return "Erro;Carregamento não encontrado.";

                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;Falha ao buscar carregamento. " + ex.Message;
            }
        }

        /// <summary>
        /// Efetua a leitura de um item do carregamento
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idCarregamento"></param>
        /// <param name="etiqueta"></param>
        /// <param name="idPedidoExp"></param>
        /// <param name="numCli"></param>
        /// <param name="nomeCli"></param>
        /// <param name="idOc"></param>
        /// <param name="idPedido"></param>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="numEtqFiltro"></param>
        public void EfetuaLeitura(string idFunc, string idCarregamento, string etiqueta, string idPedidoExp, string numCli, string nomeCli,
            string idOc, string idPedidoFiltro, string altura, string largura, string numEtqFiltro, uint idClienteExterno, string nomeClienteExterno, uint idPedidoExterno)
        {
            uint? idPedExp = !string.IsNullOrEmpty(idPedidoExp) ? Glass.Conversoes.StrParaUintNullable(idPedidoExp) : (uint?)null;
            uint? idCliente = !string.IsNullOrEmpty(numCli) ? Glass.Conversoes.StrParaUintNullable(numCli) : (uint?)null;

            Fluxo.CarregamentoFluxo.Instance.EfetuaLeitura(Glass.Conversoes.StrParaUint(idFunc), Glass.Conversoes.StrParaUint(idCarregamento), 
                etiqueta, idPedExp, idCliente, nomeCli, idOc.StrParaIntNullable(), idPedidoFiltro.StrParaIntNullable(), 
                altura.StrParaDecimalNullable(), largura.StrParaDecimalNullable(), numEtqFiltro, idClienteExterno, nomeClienteExterno, idPedidoExterno);
        }

        /// <summary>
        /// Verifica se a etiqueta é de revenda para fazer o vinculo com o pedido
        /// </summary>
        /// <param name="etiqueta"></param>
        /// <returns></returns>
        public string IsEtiquetaRevenda(string etiqueta)
        {
            return Fluxo.CarregamentoFluxo.Instance.IsEtiquetaRevenda(etiqueta).ToString();
        }

        /// <summary>
        /// Efetua o estorno de itens do carregamento
        /// </summary>
        /// <param name="idsItensCarregamento"></param>
        /// <param name="idCarregamento"></param>
        /// <param name="motivo"></param>
        /// <returns></returns>
        public string EstornoCarregamento(string idsItensCarregamento, uint? idCarregamento, string motivo)
        {
            try
            {
                Fluxo.CarregamentoFluxo.Instance.EstornoCarregamento(idsItensCarregamento, idCarregamento, motivo);

                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }

        /// <summary>
        /// Recupera os ids dos pedidos de um carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public string GetIdsPedidosByCarregamento(string idCarregamento)
        {
            try
            {
                if (string.IsNullOrEmpty(idCarregamento))
                    throw new Exception("Nenhum carregamento foi informado");

                string ids = Fluxo.CarregamentoFluxo.Instance.GetIdsPedidosByCarregamento(Glass.Conversoes.StrParaUint(idCarregamento));

                return "Ok;" + ids;
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }
    }
}
