using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa das rotas do cliente.
    /// </summary>
    public class RotaClientePesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da rota do cliente.
        /// </summary>
        public int IdRotaCliente { get; set; }

        /// <summary>
        /// Identificador da rota.
        /// </summary>
        public int IdRota { get; set; }

        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Número de sequencia do cliente na rota.
        /// </summary>
        public int NumSeq { get; set; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// CPF/CNPJ
        /// </summary>
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Número do endereço do cliente.
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// COmplemento do endereço do cliente.
        /// </summary>
        public string Compl { get; set; }

        /// <summary>
        /// Bairro do cliente.
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// Cidade do cliente.
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// Estado do cliente.
        /// </summary>
        public string Uf { get; set; }

        /// <summary>
        /// Endereço do cliente.
        /// </summary>
        public string Endereco { get; set; }

        /// <summary>
        /// Endereço completo do cliente.
        /// </summary>
        public string EnderecoCompleto
        {
            get
            {
                string compl = string.IsNullOrEmpty(Compl) ? " " : " (" + Compl + ") ";
                return Endereco + (!string.IsNullOrEmpty(Numero) ? ", " + Numero : string.Empty) + compl + Bairro + " - " + Cidade + "/" + Uf;
            }
        }
        
        /// <summary>
        /// Situação do cliente.
        /// </summary>
        public Glass.Data.Model.SituacaoCliente Situacao { get; set; }

        /// <summary>
        /// Telefone de contato do cliente.
        /// </summary>
        public string TelCont { get; set; }

        /// <summary>
        /// Email do cliente.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Data da última compra feita pelo cliente.
        /// </summary>
        public DateTime? DtUltCompra { get; set; }

        /// <summary>
        /// Total já comprado pelo cliente.
        /// </summary>
        public decimal TotalComprado { get; set; }

        #endregion
    }
}
