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
            destino.Descricao = this.cadastro.Descricao ?? destino.Descricao;
            destino.Ambiente = this.cadastro.Nome ?? destino.Ambiente;

            this.ConverterDadosProdutoMaoDeObra(destino);
        }

        private void ConverterDadosProdutoMaoDeObra(AmbientePedido destino)
        {
            if (this.cadastro.ProdutoMaoDeObra == null)
            {
                return;
            }

            destino.Altura = (int?)this.cadastro.ProdutoMaoDeObra.Altura ?? destino.Altura;
            destino.IdAplicacao = (uint?)this.cadastro.ProdutoMaoDeObra.Aplicacao?.Id ?? destino.IdAplicacao;
            destino.CodAplicacao = this.cadastro.ProdutoMaoDeObra.Aplicacao?.Codigo ?? destino.CodAplicacao;
            destino.CodInterno = this.cadastro.ProdutoMaoDeObra.CodigoInterno ?? destino.CodInterno;
            destino.IdProd = (uint?)this.cadastro.ProdutoMaoDeObra.Id ?? destino.IdProd;
            destino.Largura = this.cadastro.ProdutoMaoDeObra.Largura ?? destino.Largura;
            destino.IdProcesso = (uint?)this.cadastro.ProdutoMaoDeObra.Processo?.Id ?? destino.IdProcesso;
            destino.CodProcesso = this.cadastro.ProdutoMaoDeObra.Processo?.Codigo ?? destino.CodProcesso;
            destino.Qtde = (int?)this.cadastro.ProdutoMaoDeObra.Quantidade ?? destino.Qtde;
            destino.Redondo = this.cadastro.ProdutoMaoDeObra.Redondo ?? destino.Redondo;
        }
    }
}
