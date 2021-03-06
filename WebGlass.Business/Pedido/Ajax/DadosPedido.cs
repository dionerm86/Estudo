﻿using Glass;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.Pedido.Ajax
{
    public interface IDadosPedido
    {
        string LoadAjax(string tipo, string idClienteStr, string tipoVendaStr);

        string GetDataEntrega(string idCli, string idPedido, string tipoPedido, string tipoEntrega, string dataBase, string fastDelivery);

        string PodeInserir(string idClienteStr);

        string CheckFastDelivery(string idPedido, string dataEntrega, string diferencaM2);

        string AtualizarFastDelivery(string idPedido, string dataEntrega);

        List<GenericModel> ObterTiposVendaCliente(uint? idCliente, bool incluirVazio);
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

        /// <summary>
        /// Recupera as formas de pagamento disponíveis para o pedido, com base no cliente e tipo de venda do pedido.
        /// </summary>
        private string FormaPagtoPedido(int idCliente, int? idFormaPagto, int tipoVenda)
        {
            var retorno = string.Empty;
            var formato = "<option value='{0}'{2}>{1}</option>";

            retorno = string.Format(formato, string.Empty, string.Empty, string.Empty);
            var formasPagto = FormaPagtoDAO.Instance.GetForPedido(null, idCliente, tipoVenda);

            foreach (var f in formasPagto)
            {
                /*Caso a config UsarControleDescontoFormaPagamentoDadosProduto estiver habilitada, não deve recuperar a forma de pagamento Prazo para o tipo de venda À Vista
                e não deve recuperar a forma de pagamento Dinheiro para o tipo de venda À Prazo.*/
                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                {
                    if ((tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista && f.IdFormaPagto != (uint)Pagto.FormaPagto.Prazo)
                        || (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo && f.IdFormaPagto != (uint)Pagto.FormaPagto.Dinheiro))
                    {
                        retorno += string.Format(formato, f.IdFormaPagto, f.Descricao, idFormaPagto > 0 && f.IdFormaPagto == idFormaPagto ? " selected='selected'" : string.Empty);
                    }
                }
                else
                {
                    retorno += string.Format(formato, f.IdFormaPagto, f.Descricao, idFormaPagto > 0 && f.IdFormaPagto == idFormaPagto ? " selected='selected'" : string.Empty);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Recupera o tipo de venda ou a forma de pagamento do pedido, com base no tipo, cliente e tipo de venda informados.
        /// </summary>
        public string LoadAjax(string tipo, string idClienteStr, string tipoVendaStr)
        {
            var idCliente = idClienteStr.StrParaIntNullable().GetValueOrDefault();
            var tipoVenda = tipoVendaStr.StrParaIntNullable().GetValueOrDefault();
            var retorno = string.Empty;

            switch (tipo)
            {
                case "tipoVenda":
                    retorno = TipoVendaPedido((uint)idCliente, ClienteDAO.Instance.ObtemTipoPagto((uint)idCliente));
                    break;

                case "formaPagto":
                    retorno = FormaPagtoPedido(idCliente, (int?)ClienteDAO.Instance.ObtemIdFormaPagto((uint)idCliente), tipoVenda);
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

            if (PedidoDAO.Instance.GetDataEntregaMinima(null, Glass.Conversoes.StrParaUint(idCli), Glass.Conversoes.StrParaUintNullable(idPedido),
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

        public List<GenericModel> ObterTiposVendaCliente(uint? idCliente, bool incluirVazio)
        {
            if (idCliente.GetValueOrDefault() == 0 || !PedidoConfig.DadosPedido.BloquearDadosClientePedido)
            {
                return DataSources.Instance.GetTipoVenda(incluirVazio).ToList();
            }

            var tiposVenda = new List<GenericModel>();

            foreach (var g in DataSources.Instance.GetTipoVenda(incluirVazio))
            {
                bool adicionar = g.Id != (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo && g.Id != (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista;
                adicionar = adicionar || g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo && ParcelasDAO.Instance.GetCountByCliente(idCliente.GetValueOrDefault(), ParcelasDAO.TipoConsulta.Prazo) > 0;
                adicionar = adicionar || g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista && ParcelasDAO.Instance.GetCountByCliente(idCliente.GetValueOrDefault(), ParcelasDAO.TipoConsulta.Vista) > 0;

                if (adicionar)
                {
                    tiposVenda.Add(g);
                }
            }

            return tiposVenda;
        }
    }
}
