using System;
using System.Linq;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.OrdemCarga.Ajax
{
    public interface IOrdemCarga
    {
        string GerarOCs(string idsRotas, string idCli, string nomeCli, string dtEntPedIni, string dtEntPedFin, string idLoja, string tipoOC,
            string idsCliIgnorarBloqueio, string pedidosObs, string idPedido,
            string codRotasExternas, string idCliExterno, string nomeCliExterno, string fastDelivery, string obsLiberacao);
        string GetIdsPedidosByOCForLiberacao(string idOC);
        string BuscarDadosOC(string idOC);
        string PodeAdicionarPedidoOC(string idOC);
    }

    internal class OrdemCarga : IOrdemCarga
    {

        #region IOrdemCarga Members

        /// <summary>
        /// Gera as ocs automatticamente
        /// </summary>
        /// <param name="idsRotas"></param>
        /// <param name="idCli"></param>
        /// <param name="nomeCli"></param>
        /// <param name="dtEntPedIni"></param>
        /// <param name="dtEntPedFin"></param>
        /// <param name="idLoja"></param>
        /// <param name="tipoOC"></param>
        /// <param name="idsCliIgnorarBloqueio"></param>
        /// <param name="pedidosObs"></param>
        /// <returns></returns>
        public string GerarOCs(string idsRotas, string idCli, string nomeCli, string dtEntPedIni, string dtEntPedFin, string idLoja, string tipoOC,
            string idsCliIgnorarBloqueio, string pedidosObs, string idPedido, string codRotasExternas, string idCliExterno, string nomeCliExterno, string fastDelivery, string obsLiberacao)
        {
            try
            {
                dtEntPedIni = dtEntPedIni.Replace("-", "/");
                dtEntPedFin = dtEntPedFin.Replace("-", "/");
                uint loja = Glass.Conversoes.StrParaUint(idLoja);
                Glass.Data.Model.OrdemCarga.TipoOCEnum tpOC = (Glass.Data.Model.OrdemCarga.TipoOCEnum)Glass.Conversoes.StrParaInt(tipoOC);
                uint idCliente = Glass.Conversoes.StrParaUint(idCli);
                uint idPed = Glass.Conversoes.StrParaUint(idPedido);

                var idsCliIgnBloq = idsCliIgnorarBloqueio.Split(';').Select(f => Glass.Conversoes.StrParaUint(f)).ToList();

                WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.GerarOCs(idsRotas, idCliente, nomeCli, dtEntPedIni, dtEntPedFin,
                    loja, tpOC, idsCliIgnBloq, pedidosObs.ToLower() == "true", idPed, codRotasExternas, idCliExterno.StrParaUint(), nomeCliExterno, fastDelivery.ToLower() == "true", obsLiberacao);

                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }

        /// <summary>
        /// Recupera os ids dos pedidos de uma OC
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public string GetIdsPedidosByOCForLiberacao(string idOC)
        {
            try
            {
                if (string.IsNullOrEmpty(idOC))
                    throw new Exception("Nenhuma OC foi informada.");

                var ids = Fluxo.OrdemCargaFluxo.Instance.GetIdsPedidosByOCForLiberacao(Glass.Conversoes.StrParaUint(idOC));
                return "Ok;" + ids;
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }

        /// <summary>
        /// Buscar dados da OC
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public string BuscarDadosOC(string idOC)
        {
            var oc = OrdemCargaDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idOC));

            if (oc == null)
                throw new Exception("OC não foi encontrada.");

            return oc.IdCliente + ";" + oc.IdRota + ";" + oc.DataRotaIni.ToShortDateString() + ";" + oc.DataRotaFin.ToShortDateString() + ";" +
                oc.IdLoja + ";" + (int)oc.TipoOrdemCarga + ";" + (int)oc.Situacao;
        }

        /// <summary>
        /// Verifica se pode adicionar pedido 
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public string PodeAdicionarPedidoOC(string idOC)
        {
            return Fluxo.OrdemCargaFluxo.Instance.PodeAdicionarPedido(Glass.Conversoes.StrParaUint(idOC)).ToString();
        }

        #endregion
    }
}
