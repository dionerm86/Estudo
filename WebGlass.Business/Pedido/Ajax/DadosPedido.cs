using System;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass;

namespace WebGlass.Business.Pedido.Ajax
{
    public interface IDadosPedido
    {
        string LoadAjax(string tipo, string idClienteStr);
        string GetDataEntrega(string idCli, string idPedido, string tipoPedido, string tipoEntrega,
            string dataBase, string fastDelivery);
        string PodeInserir(string idClienteStr);
        string CheckFastDelivery(string idPedido, string dataEntrega, string diferencaM2);
        string AtualizarFastDelivery(string idPedido, string dataEntrega);
        string CheckMaximoVendas(string idPedido, string dataEntrega, string diferencaM2);
        string AtualizarMaximoVendas(string idPedido, string dataEntrega);
    }

    internal class DadosPedido : IDadosPedido
    {
        private string TipoVendaPedido(uint idCliente, uint? tipoPagto)
        {
            string retorno = "";
            string formato = "<option value='{0}'{2}>{1}</option>";
            var isTipoPagtoAVista = true;

            if (tipoPagto > 0)
            {
                var parcela = ParcelasDAO.Instance.GetElementByPrimaryKey(tipoPagto.Value);
                isTipoPagtoAVista = parcela != null ? parcela.NumParcelas == 0 : true;
            }

            foreach (var g in DataSources.Instance.GetTipoVenda())
            {
                bool adicionar = g.Id != (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo && g.Id != (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista;
                adicionar = adicionar || g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo && ParcelasDAO.Instance.GetCountByCliente(idCliente, ParcelasDAO.TipoConsulta.Prazo) > 0;
                adicionar = adicionar || g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista && ParcelasDAO.Instance.GetCountByCliente(idCliente, ParcelasDAO.TipoConsulta.Vista) > 0;

                if (adicionar)
                {
                    retorno += String.Format(formato, g.Id, g.Descr, tipoPagto > 0 && ((isTipoPagtoAVista && g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista) ||
                        (!isTipoPagtoAVista && g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo)) ? " selected='selected'" : "");
                }
            }

            return retorno;
        }

        private string FormaPagtoPedido(uint idCliente, uint? idFormaPagto)
        {
            string retorno = "";
            string formato = "<option value='{0}'{2}>{1}</option>";

            retorno = String.Format(formato, "", "", "");
            foreach (var f in FormaPagtoDAO.Instance.GetForPedido(idCliente))
            {
                retorno += String.Format(formato, f.IdFormaPagto, f.Descricao, idFormaPagto > 0 && f.IdFormaPagto == idFormaPagto ?
                    " selected='selected'" : "");
            }

            return retorno;
        }

        public string LoadAjax(string tipo, string idClienteStr)
        {
            uint idCliente = Glass.Conversoes.StrParaUint(idClienteStr);
            string retorno = "";

            switch (tipo)
            {
                case "tipoVenda":
                    retorno = TipoVendaPedido(idCliente, ClienteDAO.Instance.ObtemTipoPagto(idCliente));
                    break;

                case "formaPagto":
                    retorno = FormaPagtoPedido(idCliente, ClienteDAO.Instance.ObtemIdFormaPagto(idCliente));
                    break;
            }

            return retorno;
        }

        public string GetDataEntrega(string idCli, string idPedido, string tipoPedido, string tipoEntrega,
            string dataBase, string fastDelivery)
        {
            bool isFastDelivery = fastDelivery.ToLower() == "true";

            DateTime dataEntrega, dataFastDelivery;
            bool desabilitarCampo;
            string data = string.Empty;

            if (PedidoDAO.Instance.GetDataEntregaMinima(Glass.Conversoes.StrParaUint(idCli), Glass.Conversoes.StrParaUintNullable(idPedido),
                Glass.Conversoes.StrParaInt(tipoPedido), Glass.Conversoes.StrParaInt(tipoEntrega), Conversoes.ConverteData(dataBase), out dataEntrega,
                out dataFastDelivery, out desabilitarCampo))
            {
                data = (!isFastDelivery ? dataEntrega : dataFastDelivery).ToString("dd/MM/yyyy") + ";" +
                    desabilitarCampo.ToString().ToLower();
            }
            else
                data = "";

            if (string.IsNullOrEmpty(data) && Glass.Configuracoes.PedidoConfig.TelaCadastro.BuscarDataEntregaDeHojeSeDataVazia)
                data = DateTime.Now.ToString("dd/MM/yyyy");

            return data;
        }

        public string PodeInserir(string idClienteStr)
        {
            uint idCliente = Glass.Conversoes.StrParaUint(idClienteStr);
            return PedidoDAO.Instance.GetCountBloqueioEmissao(idCliente) + ";" +
                PedidoDAO.Instance.GetIdsBloqueioEmissao(idCliente);
        }

        public string CheckFastDelivery(string idPedido, string dataEntrega, string diferencaM2)
        {
            // Se a data de entrega não tiver sido informada, não realiza verificação de metragem quadrada
            if (String.IsNullOrEmpty(dataEntrega))
                return "Ok|true";

            try
            {
                DateTime dataEntregaAtual = DateTime.Parse(dataEntrega);
                float totalM2 = ProdutosPedidoDAO.Instance.TotalM2FastDelivery(dataEntregaAtual, Glass.Conversoes.StrParaUint(idPedido));
                float m2Pedido = ProdutosPedidoDAO.Instance.GetTotalM2ByPedido(Glass.Conversoes.StrParaUint(idPedido)) + float.Parse(diferencaM2.Replace('.', ','));
                if (m2Pedido == 0)
                    return "Ok|true";

                DateTime? novaDataEntrega = ProdutosPedidoDAO.Instance.GetFastDeliveryDay(Glass.Conversoes.StrParaUint(idPedido), dataEntregaAtual, m2Pedido);

                if (novaDataEntrega == null)
                    throw new Exception("Não foi possível encontrar uma data para agendar o Fast Delivery.");

                return "Ok|" + (novaDataEntrega.Value.Date == dataEntregaAtual.Date).ToString().ToLower() + "|" + totalM2.ToString().Replace(',', '.') +
                    "|" + m2Pedido.ToString().Replace(',', '.') + "|" + novaDataEntrega.Value.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        public string AtualizarFastDelivery(string idPedido, string dataEntrega)
        {
            var ped = PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idPedido));

            try
            {
                ped.DataEntregaString = dataEntrega;

                PedidoDAO.Instance.Update(ped);
                return "Ok|";
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        public string CheckMaximoVendas(string idPedido, string dataEntrega, string diferencaM2)
        {
            // Se a data de entrega não tiver sido informada, não realiza verificação de metragem quadrada
            if (String.IsNullOrEmpty(dataEntrega))
                return "Ok|true";

            try
            {
                DateTime dataEntregaAtual = DateTime.ParseExact(dataEntrega, "dd/MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR"));
                float totalM2 = ProdutosPedidoDAO.Instance.TotalM2MaximoVendas(dataEntregaAtual, Glass.Conversoes.StrParaUint(idPedido));
                float m2Pedido = ProdutosPedidoDAO.Instance.GetTotalM2ByPedido(Glass.Conversoes.StrParaUint(idPedido)) + float.Parse(diferencaM2.Replace('.', ','));

                if (m2Pedido == 0 || Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega))
                    return "Ok|true";

                DateTime? novaDataEntrega = ProdutosPedidoDAO.Instance.GetMaximoVendasDay(Glass.Conversoes.StrParaUint(idPedido), dataEntregaAtual, m2Pedido);

                if (novaDataEntrega == null)
                    throw new Exception("Não foi possível encontrar uma data para agendar a entrega.");

                return "Ok|" + (novaDataEntrega.Value.Date == dataEntregaAtual.Date).ToString().ToLower() + "|" + totalM2.ToString().Replace(',', '.') +
                    "|" + m2Pedido.ToString().Replace(',', '.') + "|" + novaDataEntrega.Value.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                return "Erro|" + " " + ex.Message;
            }
        }

        public string AtualizarMaximoVendas(string idPedido, string dataEntrega)
        {
            var ped = PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idPedido));

            try
            {
                ped.DataEntregaString = dataEntrega;
                
                PedidoDAO.Instance.Update(ped);
                return "Ok|";
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }
    }
}