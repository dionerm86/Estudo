using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glass.Data.Model
{
    /// <summary>
    /// Classe para armazenar os dados retornados ao receber
    /// </summary>
    public class CapptaRetornoRecebimento
    {
        /// <summary>
        /// Indfica se a transação ocorreu com sucesso
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Indentifica se a transação era de recebimento ou estorno
        /// </summary>
        public bool Estorno { get; set; }

        /// <summary>
        /// Mensagem em caso de falha
        /// </summary>
        public string MensagemErro { get; set; }

        /// <summary>
        /// Mensagem de retorno da finalização
        /// </summary>
        public string MensagemRetorno { get; set; }

        /// <summary>
        /// Referência
        /// </summary>
        public int IdReferencia { get; set; }

        /// <summary>
        /// Tipo do recebimento
        /// </summary>
        public Glass.Data.Helper.UtilsFinanceiro.TipoReceb TipoRecebimento { get; set; }

        /// <summary>
        /// Checkou Guid
        /// </summary>
        public string CheckoutGuid { get; set; }

        /// <summary>
        /// Recebimentos da transação
        /// </summary>
        [JsonProperty("responses")]
        public List<CapptaRecebimentos> Recebimentos { get; set; }

        /// <summary>
        /// Informacoes do recebimento
        /// </summary>
        public class CapptaRecebimentos
        {
            /// <summary>
            /// Código administrativo
            /// </summary>
            [JsonProperty("AdministrativeCode")]
            public string CodigoAdministrativo { get; set; }

            /// <summary>
            /// Posição do recebimento
            /// </summary>
            public int PagtoIndex { get; set; }

            /// <summary>
            /// Comprovantes
            /// </summary>
            [JsonProperty("Receipt")]
            public CapptaComprovantes Comprovantes { get; set; }
        }

        /// <summary>
        /// Comprovantes de recebimento da CAPPTA
        /// </summary>
        public class CapptaComprovantes
        {
            /// <summary>
            /// Comprovante do cliente
            /// </summary>
            [JsonProperty("CustomerReceipt")]
            public string ComprovanteCliente { get; set; }

            /// <summary>
            /// Comprovante da loja
            /// </summary>
            [JsonProperty("MerchantReceipt")]
            public string ComprovanteLoja { get; set; }

            /// <summary>
            /// Comprovante reduzido
            /// </summary>
            [JsonProperty("ReducedReceipt")]
            public string ComprovanteReduzido { get; set; }
        }
    }
}
