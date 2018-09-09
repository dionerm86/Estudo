// <copyright file="ItemCadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Models.Genericas.CadastroAtualizacao
{
    /// <summary>
    /// Classe que possui os dados de um item enviado para cadastro/atualização de um modelo.
    /// </summary>
    /// <typeparam name="T">O tipo do valor que o item representa.</typeparam>
    internal class ItemCadastroAtualizacaoDto<T>
    {
        private readonly bool temValor;
        private readonly T valor;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemCadastroAtualizacaoDto{T}"/>.
        /// </summary>
        /// <param name="temValor">Indica se o item foi enviado no JSON convertido.</param>
        /// <param name="valor">O valor enviado no JSON.</param>
        internal ItemCadastroAtualizacaoDto(bool temValor, T valor)
        {
            this.temValor = temValor;
            this.valor = valor;
        }

        /// <summary>
        /// Recupera o valor normalizado para o preenchimento do modelo.
        /// </summary>
        /// <param name="valorModelo">O valor atual do modelo, que será mantido se necessário.</param>
        /// <returns>O valor normalizado.</returns>
        public T ObterValor(T valorModelo)
        {
            return this.temValor ? this.valor : valorModelo;
        }
    }
}
