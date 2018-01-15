using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa do fornecedor.
    /// </summary>
    public class FornecedorPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do fornecedor.
        /// </summary>
        public int IdFornec { get; set; }

        /// <summary>
        /// Tipo de pessoa.
        /// </summary>
        public string TipoPessoa { get; set; }

        /// <summary>
        /// Nome fantasia.
        /// </summary>
        public string Nomefantasia { get; set; }

        /// <summary>
        /// Razão social.
        /// </summary>
        public string Razaosocial { get; set; }

        /// <summary>
        /// Nome do fornecedor.
        /// </summary>
        public string Nome
        {
            get
            {
                return !string.IsNullOrEmpty(Nomefantasia) ? Nomefantasia :
                       !string.IsNullOrEmpty(Razaosocial) ? Razaosocial : "";
            }
        }

        /// <summary>
        /// CPF/CNPJ.
        /// </summary>
        public string CpfCnpj { get; set; }

        /// <summary>
        /// RG/Inscrição Estadual.
        /// </summary>
        public string RgInscEst { get; set; }

        /// <summary>
        /// Suframa.
        /// </summary>
        public string Suframa { get; set; }

        /// <summary>
        /// CRT.
        /// </summary>
        public Data.Model.RegimeFornecedor Crt { get; set; }

        /// <summary>
        /// Data da última compra.
        /// </summary>
        public DateTime? Dtultcompra { get; set; }

        /// <summary>
        /// Telefone de contato.
        /// </summary>
        public string Telcont { get; set; }

        /// <summary>
        /// Fax.
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Situação do fornecedor.
        /// </summary>
        public Data.Model.SituacaoFornecedor Situacao { get; set; }

        /// <summary>
        /// Identifica se é para bloquear o pagamento.
        /// </summary>
        public bool BloquearPagto { get; set; }

        /// <summary>
        /// Nome do vendedor.
        /// </summary>
        public string Vendedor { get; set; }

        /// <summary>
        /// Telefone celular do vendedor.
        /// </summary>
        public string Telcelvend { get; set; }

        /// <summary>
        /// Crédito.
        /// </summary>
        public decimal Credito { get; set; }

        /// <summary>
        /// Observação.
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Nome da cidade associada.
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// Nome do estado associado.
        /// </summary>
        public string Uf { get; set; }

        /// <summary>
        /// Endereço.
        /// </summary>
        public string Endereco { get; set; }

        /// <summary>
        /// Número do endereço.
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// Complemento.
        /// </summary>
        public string Compl { get; set; }

        /// <summary>
        /// Bairro.
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// CEP.
        /// </summary>
        public string Cep { get; set; }

        /// <summary>
        /// Endereço completo.
        /// </summary>
        public string EnderecoCompleto
        {
            get
            {
                string endereco = Endereco + ", " + Numero + " - " + Bairro;
                if (Cidade != null && Uf != null)
                    endereco += " - " + Cidade + "/" + Uf;

                endereco += " CEP: " + Cep;
                return endereco;
            }
        }

        /// <summary>
        /// Nome do pais.
        /// </summary>
        public string Pais { get; set; }

        /// <summary>
        /// Nome do plano de contas associado.
        /// </summary>
        public string PlanoContas { get; set; }

        /// <summary>
        /// Parcela.
        /// </summary>
        public string Parcela { get; set; }

        #endregion
    }
}
