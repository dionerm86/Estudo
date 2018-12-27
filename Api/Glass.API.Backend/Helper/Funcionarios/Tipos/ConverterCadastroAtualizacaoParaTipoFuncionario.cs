// <copyright file="ConverterCadastroAtualizacaoParaTipoFuncionario.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Funcionarios.V1.Tipos.CadastroAtualizacao;
using Glass.Global.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.Funcionarios.Tipos
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de tipos de funcionário.
    /// </summary>
    public class ConverterCadastroAtualizacaoParaTipoFuncionario
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<TipoFuncionario> tipoFuncionario;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaTipoFuncionario"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O tipo de funcionário atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaTipoFuncionario(
            CadastroAtualizacaoDto cadastro,
            TipoFuncionario atual = null)
        {
            this.cadastro = cadastro;
            this.tipoFuncionario = new Lazy<TipoFuncionario>(() =>
            {
                var destino = atual ?? new TipoFuncionario();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model do tipo de funcionário preenchida.</returns>
        public TipoFuncionario ConverterParaTipoFuncionario()
        {
            return this.tipoFuncionario.Value;
        }

        private void ConverterDtoParaModelo(TipoFuncionario destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
        }
    }
}