// <copyright file="ConverterCadastroAtualizacaoParaFeriado.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Datas.Feriados.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Datas.Feriados
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de feriado.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaFeriado
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.Feriado> feriado;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaFeriado"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O feriado atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaFeriado(
            CadastroAtualizacaoDto cadastro,
            Data.Model.Feriado atual = null)
        {
            this.cadastro = cadastro;
            this.feriado = new Lazy<Data.Model.Feriado>(() =>
            {
                var destino = atual ?? new Data.Model.Feriado();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de feriado preenchida.</returns>
        public Data.Model.Feriado ConverterParaFeriado()
        {
            return this.feriado.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.Feriado destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
            destino.Dia = this.cadastro.ObterValorNormalizado(c => c.Dia, destino.Dia);
            destino.Mes = this.cadastro.ObterValorNormalizado(c => c.Mes, destino.Mes);
        }
    }
}
