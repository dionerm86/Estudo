// <copyright file="ConverterCadastroAtualizacaoParaPedido.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Pedidos.CadastroAtualizacao;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Pedidos
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de pedido.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaPedido
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.Pedido> pedido;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaPedido"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O pedido atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaPedido(CadastroAtualizacaoDto cadastro, Data.Model.Pedido atual = null)
        {
            this.cadastro = cadastro;
            this.pedido = new Lazy<Data.Model.Pedido>(() =>
            {
                var destino = atual ?? new Data.Model.Pedido();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de pedido preenchida.</returns>
        public Data.Model.Pedido ConverterParaPedido()
        {
            return this.pedido.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.Pedido destino)
        {
            destino.IdCli = (uint?)this.cadastro.IdCliente ?? destino.IdCli;
            destino.IdLoja = (uint?)this.cadastro.IdLoja ?? destino.IdLoja;
            destino.IdObra = (uint?)this.cadastro.IdObra ?? destino.IdObra;
            destino.DataPedido = this.cadastro.DataPedido ?? destino.DataPedido;
            destino.FastDelivery = this.cadastro.FastDelivery ?? destino.FastDelivery;
            destino.CodCliente = this.cadastro.CodigoPedidoCliente ?? destino.CodCliente;
            destino.DeveTransferir = this.cadastro.DeveTransferir ?? destino.DeveTransferir;
            destino.TipoPedido = (int?)this.cadastro.Tipo ?? destino.TipoPedido;
            destino.TipoVenda = (int?)this.cadastro.TipoVenda ?? destino.TipoVenda;
            destino.IdFunc = (uint?)this.cadastro.IdVendedor ?? destino.IdFunc;
            destino.IdMedidor = (uint?)this.cadastro.IdMedidor ?? destino.IdMedidor;
            destino.IdFuncVenda = (uint?)this.cadastro.IdFuncionarioComprador ?? destino.IdFuncVenda;
            destino.IdTransportador = this.cadastro.IdTransportador ?? destino.IdTransportador;
            destino.IdPedidoRevenda = this.cadastro.IdPedidoRevenda ?? destino.IdPedidoRevenda;
            destino.GerarPedidoProducaoCorte = this.cadastro.GerarPedidoCorte ?? destino.GerarPedidoProducaoCorte;
            destino.ValorEntrada = this.cadastro.Sinal.Valor ?? destino.ValorEntrada;
            destino.Obs = this.cadastro.Observacao ?? destino.Obs;
            destino.ObsLiberacao = this.cadastro.ObservacaoLiberacao ?? destino.ObsLiberacao;

            this.ConverterDadosEntrega(destino);
            this.ConverterDadosFormaPagamento(destino);
            this.ConverterDadosDesconto(destino);
            this.ConverterDadosAcrescimo(destino);
            this.ConverterDadosComissionado(destino);
            this.ConverterDadosEnderecoObra(destino);
        }

        private void ConverterDadosEntrega(Data.Model.Pedido destino)
        {
            if (this.cadastro.Entrega != null)
            {
                destino.TipoEntrega = this.cadastro.Entrega.Tipo?.Id ?? destino.TipoEntrega;
                destino.DataEntrega = this.cadastro.Entrega.Data ?? destino.DataEntrega;
                destino.ValorEntrega = this.cadastro.Entrega.Valor ?? destino.ValorEntrega;
            }
        }

        private void ConverterDadosFormaPagamento(Data.Model.Pedido destino)
        {
            if (this.cadastro.FormaPagamento != null)
            {
                destino.IdFormaPagto = (uint?)this.cadastro.FormaPagamento.Id ?? destino.IdFormaPagto;
                destino.IdTipoCartao = (uint?)this.cadastro.FormaPagamento.IdTipoCartao ?? destino.IdTipoCartao;

                if (this.cadastro.FormaPagamento.Parcelas != null)
                {
                    destino.IdParcela = (uint?)this.cadastro.FormaPagamento.Parcelas.Id ?? destino.IdParcela;
                    destino.NumParc = this.cadastro.FormaPagamento.Parcelas.NumeroParcelas ?? destino.NumParc;

                    if (this.cadastro.FormaPagamento.Parcelas.Detalhes != null)
                    {
                        destino.DatasParcelas = this.cadastro.FormaPagamento.Parcelas.Detalhes
                            .OrderBy(d => d.Data)
                            .Select(d => d.Data)
                            .ToArray();

                        destino.ValoresParcelas = this.cadastro.FormaPagamento.Parcelas.Detalhes
                            .OrderBy(d => d.Data)
                            .Select(d => d.Valor)
                            .ToArray();
                    }
                }
            }
        }

        private void ConverterDadosDesconto(Data.Model.Pedido destino)
        {
            if (this.cadastro.Desconto != null)
            {
                destino.TipoDesconto = this.cadastro.Desconto.Tipo;
                destino.Desconto = this.cadastro.Desconto.Valor;
            }
        }

        private void ConverterDadosAcrescimo(Data.Model.Pedido destino)
        {
            if (this.cadastro.Acrescimo != null)
            {
                destino.TipoAcrescimo = this.cadastro.Acrescimo.Tipo;
                destino.Acrescimo = this.cadastro.Acrescimo.Valor;
            }
        }

        private void ConverterDadosComissionado(Data.Model.Pedido destino)
        {
            if (this.cadastro.Comissionado != null)
            {
                destino.IdComissionado = (uint?)this.cadastro.Comissionado.Id;
                destino.PercComissao = this.cadastro.Comissionado.PercentualComissao;
            }
        }

        private void ConverterDadosEnderecoObra(Data.Model.Pedido destino)
        {
            if (this.cadastro.EnderecoObra != null)
            {
                destino.EnderecoObra = this.cadastro.EnderecoObra.Logradouro;
                destino.BairroObra = this.cadastro.EnderecoObra.Bairro;
                destino.CidadeObra = this.cadastro.EnderecoObra.Cidade.Nome;
                destino.CepObra = this.cadastro.EnderecoObra.Cep;
            }
        }
    }
}
