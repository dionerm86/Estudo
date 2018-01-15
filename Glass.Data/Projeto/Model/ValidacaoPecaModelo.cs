using GDA;
using System.Xml.Serialization;
using Glass.Data.DAL;
using Glass.Log;
using System.ComponentModel;

namespace Glass.Data.Model
{

    [PersistenceBaseDAO(typeof(ValidacaoPecaModeloDAO))]
    [XmlRoot("validacaoPecaModelo")]
    [PersistenceClass("validacao_peca_modelo")]
    public class ValidacaoPecaModelo : ModelBaseCadastro
    {
        #region Enumeradores

        /// <summary>
        /// Possíveis tipos de validação da peça do projeto modelo.
        /// </summary>
        public enum TipoValidacaoPecaModelo : int
        {
            /// <summary>
            /// Bloquear, não permite a confirmação do projeto.
            /// </summary>
            [Description("Bloquear")]
            Bloquear = 1,

            /// <summary>
            /// Informa ao cliente que existem medidas incorretas no projeto, não impede a confirmação.
            /// </summary>
            [Description("Somente Informar")]
            SomenteInformar,

            /// <summary>
            /// Segue a configuração que informa se a validação deve bloquear o cálculo ou somente informar a mensagem cadastrada.
            /// </summary>
            [Description("Considerar Configuração")]
            ConsiderarConfiguracao
        }

        /// <summary>
        /// Possíveis tipos de comparadores de validação da peça do projeto modelo.
        /// </summary>
        public enum TipoComparadorExpressaoValidacao : int
        {
            [Description("=")]
            Igual = 1,

            [Description(">")]
            Maior,

            [Description("<")]
            Menor,

            [Description(">=")]
            MaiorOuIgual,

            [Description("<=")]
            MenorOuIgual,

            [Description("<>")]
            Diferente
        }

        #endregion

        #region Propriedades

        [XmlAttribute("idValidacaoPecaModelo")]
        [PersistenceProperty("IDVALIDACAOPECAMODELO", PersistenceParameterType.IdentityKey)]
        public int IdValidacaoPecaModelo { get; set; }

        [XmlAttribute("idPecaProjMod")]
        [PersistenceProperty("IDPECAPROJMOD")]
        public int IdPecaProjMod { get; set; }

        [Log("Primeira expressão validação")]
        [XmlAttribute("primeiraExpressaoValidacao")]
        [PersistenceProperty("PRIMEIRAEXPRESSAOVALIDACAO")]
        public string PrimeiraExpressaoValidacao { get; set; }

        [Log("Segunda expressão validação")]
        [XmlAttribute("segundaExpressaoValidacao")]
        [PersistenceProperty("SEGUNDAEXPRESSAOVALIDACAO")]
        public string SegundaExpressaoValidacao { get; set; }

        [Log("Tipo comparador")]
        [XmlAttribute("tipoComparador")]
        [PersistenceProperty("TIPOCOMPARADOR")]
        public int TipoComparador { get; set; }

        [Log("Mensagem")]
        [XmlAttribute("mensagem")]
        [PersistenceProperty("MENSAGEM")]
        public string Mensagem { get; set; }

        [Log("Tipo Validação")]
        [XmlAttribute("tipoValidacao")]
        [PersistenceProperty("TIPOVALIDACAO")]
        public int TipoValidacao { get; set; }

        #endregion
    }
}