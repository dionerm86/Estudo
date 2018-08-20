using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa da sugestão do cliente.
    /// </summary>
    public class SugestaoClientePesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da sugestão.
        /// </summary>
        public int IdSugestao { get; set; }

        /// <summary>
        /// Identificador do cliente associado.
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Data de cadastro.
        /// </summary>
        public DateTime DataCad { get; set; }

        /// <summary>
        /// Nome do funcionário que cadastrou.
        /// </summary>
        public string Funcionario { get; set; }

        /// <summary>
        /// Nome do cliente associado.
        /// </summary>
        public string Cliente { get; set; }

        /// <summary>
        /// Tipo de sugestão.
        /// </summary>
        public int TipoSugestao { get; set; }

        /// <summary>
        /// Descrição da sugestão.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Identifica se a sugestão foi cancelada.
        /// </summary>
        public bool Cancelada { get; set; }

        /// <summary>
        /// Identifica a descrição da rota do cliente
        /// </summary>
        public string DescricaoRota { get; set; }

        /// <summary>
        /// Id do funcionário de cadastro.
        /// </summary>
        public uint IdFunc { get; set; }
        /// <summary>
        /// Id do pedido 
        /// </summary>
        public uint? IdPedido { get; set; }

        #endregion
    }
}
