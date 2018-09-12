// <copyright file="ConverterCadastroAtualizacaoParaAmbientePedido.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Pedidos.AmbientesPedido.CadastroAtualizacao;
using Glass.Data.Model;
using System;

namespace Glass.API.Backend.Helper.Pedidos.AmbientesPedido
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de ambiente de pedido.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaAmbientePedido
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<AmbientePedido> ambientePedido;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaAmbientePedido"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O ambiente de pedido atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaAmbientePedido(CadastroAtualizacaoDto cadastro, AmbientePedido atual = null)
        {
            this.cadastro = cadastro;
            this.ambientePedido = new Lazy<AmbientePedido>(() =>
            {
                var destino = atual ?? new AmbientePedido();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de ambiente de pedido preenchida.</returns>
        public AmbientePedido ConverterParaAmbientePedido()
        {
            return this.ambientePedido.Value;
        }

        private void ConverterDtoParaModelo(AmbientePedido destino)
        {
            destino.Acrescimo = this.cadastro.Acrescimo?.Valor ?? destino.Acrescimo;
            destino.TipoAcrescimo = this.cadastro.Acrescimo?.Tipo ?? destino.TipoAcrescimo;
            destino.Desconto = this.cadastro.Desconto?.Valor ?? destino.Desconto;
            destino.TipoDesconto = this.cadastro.Desconto?.Tipo ?? destino.TipoDesconto;
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
            destino.Ambiente = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Ambiente);

            this.ConverterDadosProdutoMaoDeObra(destino);
        }

        private void ConverterDadosProdutoMaoDeObra(AmbientePedido destino)
        {
            if (this.cadastro.ProdutoMaoDeObra == null)
            {
                return;
            }

            destino.Altura = (int?)this.cadastro.ProdutoMaoDeObra.ObterValorNormalizado(c => c.Altura, destino.Altura);
            destino.IdAplicacao = (uint?)this.cadastro.ProdutoMaoDeObra.ObterValorNormalizado(c => c.IdAplicacao, (int?)destino.IdAplicacao);
            destino.CodInterno = this.cadastro.ProdutoMaoDeObra.ObterValorNormalizado(c => c.CodigoInterno, destino.CodInterno);
            destino.IdProd = (uint?)this.cadastro.ProdutoMaoDeObra.ObterValorNormalizado(c => c.Id, (int?)destino.IdProd);
            destino.Largura = this.cadastro.ProdutoMaoDeObra.ObterValorNormalizado(c => c.Largura, destino.Largura);
            destino.IdProcesso = (uint?)this.cadastro.ProdutoMaoDeObra.ObterValorNormalizado(c => c.IdProcesso, (int?)destino.IdProcesso);
            destino.Qtde = (int?)this.cadastro.ProdutoMaoDeObra.ObterValorNormalizado(c => c.Quantidade, destino.Qtde);
            destino.Redondo = this.cadastro.ProdutoMaoDeObra.ObterValorNormalizado(c => c.Redondo, destino.Redondo);
        }
    }
}
