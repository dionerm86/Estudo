// <copyright file="ConverterCadastroAtualizacaoParaTipo.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Clientes.V1.Tipos.CadastroAtualizacao;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Clientes.Tipos
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de tipo de cliente.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaTipo
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.TipoCliente> tipoCliente;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaTipo"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O tipo de cliente atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaTipo(
            CadastroAtualizacaoDto cadastro,
            Data.Model.TipoCliente atual = null)
        {
            this.cadastro = cadastro;
            this.tipoCliente = new Lazy<Data.Model.TipoCliente>(() =>
            {
                var destino = atual ?? new Data.Model.TipoCliente();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de tipo de cliente preenchida.</returns>
        public Data.Model.TipoCliente ConverterParaTipo()
        {
            return this.tipoCliente.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.TipoCliente destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
            destino.CobrarAreaMinima = this.cadastro.ObterValorNormalizado(c => c.CobrarAreaMinima, destino.CobrarAreaMinima);
        }
    }
}
