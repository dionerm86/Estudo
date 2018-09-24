// <copyright file="ConverterCadastroAtualizacaoParaPedido.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Pedidos.CadastroAtualizacao;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
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
            destino.IdCli = (uint)this.cadastro.ObterValorNormalizado(c => c.IdCliente, (int)destino.IdCli);
            destino.IdLoja = (uint)this.cadastro.ObterValorNormalizado(c => c.IdLoja, (int)destino.IdLoja);
            destino.IdObra = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdObra, (int?)destino.IdObra);
            destino.DataPedido = this.cadastro.ObterValorNormalizado(c => c.DataPedido, destino.DataPedido);
            destino.FastDelivery = this.cadastro.ObterValorNormalizado(c => c.FastDelivery, destino.FastDelivery);
            destino.CodCliente = this.cadastro.ObterValorNormalizado(c => c.CodigoPedidoCliente, destino.CodCliente);
            destino.DeveTransferir = this.cadastro.ObterValorNormalizado(c => c.DeveTransferir, destino.DeveTransferir);
            destino.TipoPedido = (int)this.cadastro.ObterValorNormalizado(c => c.Tipo, (Data.Model.Pedido.TipoPedidoEnum)destino.TipoPedido);
            destino.TipoVenda = (int)this.cadastro.ObterValorNormalizado(c => c.TipoVenda, (Data.Model.Pedido.TipoVendaPedido?)destino.TipoVenda);
            destino.IdFunc = (uint)this.cadastro.ObterValorNormalizado(c => c.IdVendedor, (int)destino.IdFunc);
            destino.IdMedidor = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdMedidor, (int?)destino.IdMedidor);
            destino.IdFuncVenda = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdFuncionarioComprador, (int?)destino.IdFuncVenda);
            destino.IdTransportador = this.cadastro.ObterValorNormalizado(c => c.IdTransportador, destino.IdTransportador);
            destino.IdPedidoRevenda = this.cadastro.ObterValorNormalizado(c => c.IdPedidoRevenda, destino.IdPedidoRevenda);
            destino.GerarPedidoProducaoCorte = this.cadastro.ObterValorNormalizado(c => c.GerarPedidoCorte, destino.GerarPedidoProducaoCorte);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);
            destino.ObsLiberacao = this.cadastro.ObterValorNormalizado(c => c.ObservacaoLiberacao, destino.ObsLiberacao);

            this.ConverterDadosEntrega(destino);
            this.ConverterDadosFormaPagamento(destino);
            this.ConverterDadosDesconto(destino);
            this.ConverterDadosAcrescimo(destino);
            this.ConverterDadosComissionado(destino);
            this.ConverterDadosEnderecoObra(destino);
            this.ConverterDadosSinal(destino);
        }

        private void ConverterDadosEntrega(Data.Model.Pedido destino)
        {
            if (this.cadastro.Entrega != null)
            {
                destino.TipoEntrega = this.cadastro.Entrega.Tipo ?? destino.TipoEntrega;
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
                destino.TipoDesconto = (int)(this.cadastro.Desconto.Tipo ?? TipoValor.Valor);
                destino.Desconto = this.cadastro.Desconto.Valor.GetValueOrDefault();
            }
        }

        private void ConverterDadosAcrescimo(Data.Model.Pedido destino)
        {
            if (this.cadastro.Acrescimo != null)
            {
                destino.TipoAcrescimo = (int)(this.cadastro.Acrescimo.Tipo ?? TipoValor.Valor);
                destino.Acrescimo = this.cadastro.Acrescimo.Valor.GetValueOrDefault();
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

        private void ConverterDadosSinal(Data.Model.Pedido destino)
        {
            if (this.cadastro.Sinal != null)
            {
                destino.ValorEntrada = this.cadastro.Sinal.Valor.GetValueOrDefault();
            }
        }
    }
}
