using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Implementação base do registro da rentabilidade.
    /// </summary>
    class RegistroRentabilidade : IRegistroRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Identificador do registro associado.
        /// </summary>
        public int IdRegistro { get; }

        /// <summary>
        /// Descritor do registro.
        /// </summary>
        public DescritorRegistroRentabilidade Descritor { get; }

        /// <summary>
        /// Tipo do registro.
        /// </summary>
        public TipoRegistroRentabilidade Tipo { get; }

        /// <summary>
        /// Valor do registro.
        /// </summary>
        public decimal Valor { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="idRegistro">Identificador do registro.</param>
        /// <param name="descritor"></param>
        /// <param name="tipo"></param>
        /// <param name="valor"></param>
        public RegistroRentabilidade(int idRegistro, DescritorRegistroRentabilidade descritor, TipoRegistroRentabilidade tipo, decimal valor)
        {
            IdRegistro = idRegistro;
            Descritor = descritor;
            Tipo = tipo;
            Valor = valor;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera o texto que representa a instancia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Tipo} => Id: {IdRegistro}; Valor: {Valor}";
        }

        #endregion
    }
}
