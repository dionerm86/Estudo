using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CartaCorrecaoDAO))]
    [PersistenceClass("carta_correcao")]
    public class CartaCorrecao
    {
        #region Construtores

        public CartaCorrecao()
        {
        }

        #endregion

        #region Enumeradores

        public enum SituacaoEnum
        {
            Ativa = 1,
            Registrada,
            Recusada
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IdCarta", PersistenceParameterType.IdentityKey)]
        public uint IdCarta { get; set; }

        [PersistenceProperty("IdNf", DirectionParameter.OutputOnlyInsert)]
        public uint IdNf { get; set; }

        /// <summary>
        /// Identificador de controle do Lote de envio do Evento. 
        /// Número seqüencial autoincremental único para 
        /// identificação do Lote. A responsabilidade de gerar e 
        /// controlar é exclusiva do autor do evento. O Web Service 
        /// não faz qualquer uso deste identificador. 
        /// </summary>
        //[PersistenceProperty("IdLote")]
        //public int IdLote { get; set; }

        /// <summary>
        /// Código do órgão de recepção do Evento. Utilizar a Tabela 
        /// do IBGE, utilizar 90 para identificar o Ambiente Nacional. 
        /// </summary>
        [PersistenceProperty("Orgao", DirectionParameter.OutputOnlyInsert)]
        public uint Orgao { get; set; }

        /// <summary>
        /// Identificação do Ambiente: 
        /// 1 - Produção 
        /// 2 – Homologação 
        /// </summary>
        [PersistenceProperty("TipoAmbiente", DirectionParameter.OutputOnlyInsert)]
        public int TipoAmbiente { get; set; }

        /// <summary>
        /// Informar o CNPJ do autor do Evento se houver
        /// </summary>
        [PersistenceProperty("CNPJ", DirectionParameter.OutputOnlyInsert)]
        public string CNPJ { get; set; }

        /// <summary>
        /// Informar o CNPJ ou o CPF do autor do Evento se houver
        /// </summary>
        [PersistenceProperty("CPF", DirectionParameter.OutputOnlyInsert)]
        public string CPF { get; set; }

        /// <summary>
        /// Chave de Acesso da NF-e vinculada ao Evento
        /// </summary>
        [PersistenceProperty("ChaveNFe", DirectionParameter.OutputOnlyInsert)]
        public string ChaveNFe { get; set; }

        private DateTime _dataCadastro = DateTime.Now;

        [PersistenceProperty("DataCadastro")]
        public DateTime DataCadastro { get { return _dataCadastro; } set { _dataCadastro = value; } }

        private uint _tipoEvento = 110110;
        /// <summary>
        /// Código do de evento = 110110 
        /// </summary>
        [PersistenceProperty("TipoEvento", DirectionParameter.OutputOnlyInsert)]
        public uint TipoEvento { get { return _tipoEvento; } set { _tipoEvento = value; } }

        /// <summary>
        /// Seqüencial do evento para o mesmo tipo de evento.  Para 
        /// maioria dos eventos será 1, nos casos em que possa 
        /// existir mais de um evento, como é o caso da carta de 
        /// correção, o autor do evento deve numerar de forma seqüencial. 
        /// </summary>
        [PersistenceProperty("NumeroSequencialEvento", DirectionParameter.OutputOnlyInsert)]
        public int NumeroSequencialEvento { get; set; }

        private string _versaoEvento = "1.00";
        /// <summary>
        /// Versão do evento 
        /// </summary>
        [PersistenceProperty("VersaoEvento", DirectionParameter.OutputOnlyInsert)]
        public string VersaoEvento { get { return _versaoEvento; } set { _versaoEvento = value; } }

        private string _descricaoEvento = "Carta de Correcao";
        /// <summary>
        /// “Carta de Correção”  ou  “Carta de Correcao” 
        /// </summary>
        [PersistenceProperty("DescricaoEvento", DirectionParameter.OutputOnlyInsert)]
        public string DescricaoEvento { get { return _descricaoEvento; } set { _descricaoEvento = value; } }

        /// <summary>
        /// Correção a ser considerada, texto livre. A correção mais recente substitui as anteriores. 
        /// </summary>
        [PersistenceProperty("Correcao")]
        public string Correcao { get; set; }

        private string _condicaoUso = 
            "A Carta de Correcao e disciplinada pelo paragrafo 1o-A do art. 7o do Convenio S/N, de 15 de dezembro de 1970 e pode ser utilizada " +
            "para regularizacao de erro ocorrido na emissao de documento fiscal, desde que o erro nao esteja relacionado com: I - as variaveis que " +
            "determinam o valor do imposto tais como: base de calculo, aliquota, diferenca de preco, quantidade, valor da operacao ou da prestacao; " +
            "II - a correcao de dados cadastrais que implique mudanca do remetente ou do destinatario; III - a data de emissao ou de saida.";

        /// <summary>
        /// Condições de uso da Carta de Correção
        /// </summary>
        [PersistenceProperty("CondicaoUso", DirectionParameter.OutputOnlyInsert)]
        public string CondicaoUso { get { return _condicaoUso; } set { _condicaoUso = value; } }

        [PersistenceProperty("Situacao")]
        public SituacaoEnum Situacao { get; set; }

        /// <summary>
        /// Número do Protocolo da NF-e 1 posição (1-Secretaria da Fazenda Estadual, 2-RFB), 2 
        /// </summary>
        [PersistenceProperty("Protocolo")]
        public string Protocolo { get; set; }

        /// <summary>
        /// Identificador da TAG a ser assinada, a regra de formação 
        /// do Id é: 
        /// “ID” + tpEvento +  chave da NF-e + nSeqEvento 
        /// </summary>
        [PersistenceProperty("IdInfEvento", DirectionParameter.OutputOnlyInsert)]
        public string IdInfEvento { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool EditarExcluirVisible
        {
            get
            {
                return Situacao != SituacaoEnum.Recusada && Situacao != SituacaoEnum.Registrada;
            }
        }

        public bool EnviarVisible
        {
            get
            {
                return Situacao == SituacaoEnum.Ativa || Situacao == SituacaoEnum.Recusada;
            }
        }

        public bool Imprimir
        {
            get
            {
                return Situacao == SituacaoEnum.Registrada;
            }
        }

        public string DescrSituacao
        {
            get 
            {
                switch (Situacao)
                {
                    case SituacaoEnum.Ativa:
                        return "Ativa";
                    case SituacaoEnum.Recusada:
                        return "Recusada";
                    case SituacaoEnum.Registrada:
                        return "Registrada";
                    default:
                        return "N/D";
                }
            }
        }

        #endregion
    }
}