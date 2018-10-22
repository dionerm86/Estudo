// <copyright file="ConverterCadastroAtualizacaoParaTipoPerda.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Producao.V1.TiposPerda.CadastroAtualizacao;
using Glass.PCP.Negocios.Entidades;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Producao.TiposPerda
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de tipos de perda.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaTipoPerda
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<TipoPerda> tipoPerda;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaTipoPerda"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O tipo de perda atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaTipoPerda(
            CadastroAtualizacaoDto cadastro,
            TipoPerda atual = null)
        {
            this.cadastro = cadastro;
            this.tipoPerda = new Lazy<TipoPerda>(() =>
            {
                var destino = atual ?? new TipoPerda();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de tipo de perda preenchida.</returns>
        public TipoPerda ConverterParaTipoPerda()
        {
            return this.tipoPerda.Value;
        }

        private void ConverterDtoParaModelo(TipoPerda destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.IdSetor = this.cadastro.ObterValorNormalizado(c => c.IdSetor, destino.IdSetor);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
            destino.ExibirPainelProducao = this.cadastro.ObterValorNormalizado(c => c.ExibirNoPainelProducao, destino.ExibirPainelProducao);
        }
    }
}
