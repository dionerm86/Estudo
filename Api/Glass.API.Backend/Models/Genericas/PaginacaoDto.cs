// <copyright file="PaginacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper;
using System;

namespace Glass.API.Backend.Models.Genericas
{
    /// <summary>
    /// Classe base para métodos que retornam listas paginadas.
    /// </summary>
    public abstract class PaginacaoDto
    {
        private readonly Lazy<ITraducaoOrdenacao> tradutorOrdenacao;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="PaginacaoDto"/>.
        /// </summary>
        /// <param name="tradutorOrdenacao">O tradutor de ordenação que será usado pelo objeto.</param>
        protected PaginacaoDto(Func<PaginacaoDto, ITraducaoOrdenacao> tradutorOrdenacao)
        {
            this.Pagina = 1;
            this.NumeroRegistros = 10;
            this.Ordenacao = null;
            this.tradutorOrdenacao = new Lazy<ITraducaoOrdenacao>(() => tradutorOrdenacao(this));
        }

        /// <summary>
        /// Obtém ou define o número da página que está sendo buscada.
        /// </summary>
        public int Pagina { get; set; }

        /// <summary>
        /// Obtém ou define o número de registros por página a serem retornados.
        /// </summary>
        public int NumeroRegistros { get; set; }

        /// <summary>
        /// Obtém ou define o campo para o qual será feita a ordenação.
        /// </summary>
        public string Ordenacao { get; set; }

        /// <summary>
        /// Retorna o número do primeiro registro para a consulta.
        /// </summary>
        /// <returns>O número do primeiro registro para a consulta ao banco de dados.</returns>
        internal int ObterPrimeiroRegistroRetornar()
        {
            return (this.Pagina - 1) * this.NumeroRegistros;
        }

        /// <summary>
        /// Obtém o campo traduzido para ordenação para a consulta.
        /// </summary>
        /// <returns>O campo que será usado na consulta ao banco de dados.</returns>
        internal string ObterTraducaoOrdenacao()
        {
            return this.tradutorOrdenacao.Value != null
                ? this.tradutorOrdenacao.Value.ObterTraducaoOrdenacao()
                : this.Ordenacao;
        }
    }
}
