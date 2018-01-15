using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados do registro da pesquisa de funcionários.
    /// </summary>
    public class FuncionarioPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do funcionário.
        /// </summary>
        public int IdFunc { get; set; }

        /// <summary>
        /// Identificador da loja associada.
        /// </summary>
        public int IdLoja { get; set; }

        /// <summary>
        /// Identificador do tipo de funcionário.
        /// </summary>
        public int IdTipoFunc { get; set; }

        /// <summary>
        /// Tipo do Funcionário.
        /// </summary>
        public string TipoFuncionario { get; set; }

        /// <summary>
        /// Loja.
        /// </summary>
        public string Loja { get; set; }

        /// <summary>
        /// Nome do funcionário.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// CPF.
        /// </summary>
        public string Cpf { get; set; }

        /// <summary>
        /// RG.
        /// </summary>
        public string Rg { get; set; }

        /// <summary>
        /// Telefone residencial 
        /// </summary>
        public string TelRes { get; set; }

        /// <summary>
        /// Telefone celular.
        /// </summary>
        public string TelCel { get; set; }

        /// <summary>
        /// Função.
        /// </summary>
        public string Funcao { get; set; }

         /// <summary>
        /// Valor do salário do funcionário.
        /// </summary>
        public decimal Salario { get; set; }

        /// <summary>
        /// Data de entrada na empresa do funcionário.
        /// </summary>
        public DateTime? DataEnt { get; set; }

        /// <summary>
        /// Data de nascimento do funcionário.
        /// </summary>
        public DateTime? DataNasc { get; set; }

        /// <summary>
        /// Usuário Administrador da Sync.
        /// </summary>
        public bool AdminSync { get; set; }

        #endregion
    }
}
