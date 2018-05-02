using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Representa o descritor do registro de rentabilidade.
    /// </summary>
    public class DescritorRegistroRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Nome do registro.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Descrição do registro.
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Posição do registro.
        /// </summary>
        public int Posicao { get; set; }

        /// <summary>
        /// Identifica se é para exibir o descritor no relatório.
        /// </summary>
        public bool ExibirRelatorio { get; }

        #endregion

        #region Constructores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="nome">Nome do registro.</param>
        /// <param name="descricao">Descrição do registro.</param>
        /// <param name="posicao">Posição do registro.</param>
        /// <param name="exibirRelatorio">Identifica se é para exibir o descritor no relatório.</param>
        public DescritorRegistroRentabilidade(string nome, string descricao, int posicao, bool exibirRelatorio)
        {
            Nome = nome;
            Descricao = descricao;
            Posicao = posicao;
            ExibirRelatorio = exibirRelatorio;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Formata o valor do registro.
        /// </summary>
        /// <param name="registro">Instancia do registro que será formatado.</param>
        /// <param name="cultura">Cultura que será usada</param>
        /// <returns></returns>
        public virtual string FormatarValor(IRegistroRentabilidade registro, System.Globalization.CultureInfo cultura)
        {
            registro.Require(nameof(registro)).NotNull();
            return registro.Valor.ToString(cultura);
        }

        #endregion
    }
}
