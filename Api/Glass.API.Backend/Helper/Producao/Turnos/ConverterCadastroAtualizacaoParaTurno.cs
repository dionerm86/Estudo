// <copyright file="ConverterCadastroAtualizacaoParaTurno.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Producao.V1.Turnos.CadastroAtualizacao;
using Glass.Global.Negocios.Entidades;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Producao.Turnos
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de turnos.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaTurno
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Turno> turno;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaTurno"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O turno atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaTurno(
            CadastroAtualizacaoDto cadastro,
            Turno atual = null)
        {
            this.cadastro = cadastro;
            this.turno = new Lazy<Turno>(() =>
            {
                var destino = atual ?? new Turno();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de turno preenchida.</returns>
        public Turno ConverterParaTurno()
        {
            return this.turno.Value;
        }

        private void ConverterDtoParaModelo(Turno destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.NumSeq = this.cadastro.ObterValorNormalizado(c => c.Sequencia, destino.NumSeq);
            destino.Inicio = this.cadastro.ObterValorNormalizado(c => c.Inicio, destino.Inicio);
            destino.Termino = this.cadastro.ObterValorNormalizado(c => c.Termino, destino.Termino);
        }
    }
}
