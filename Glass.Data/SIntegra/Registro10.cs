using System;
using Glass.Data.Model;

namespace Glass.Data.SIntegra
{
    internal class Registro10 : Registro
    {
        #region Enumeradores

        /// <summary>
        /// Enumeração com o tipo de estrutura de arquivo.
        /// </summary>
        public enum TipoEstruturaArquivo
        {
            /// <summary>
            /// 1 - Estrutura conforme Convênio ICMS 57/95 na versão do Convênio ICMS 31/99.
            /// </summary>
            Versao1 = 1,

            /// <summary>
            /// 2 - Estrutura conforme Convênio ICMS 57/95 na versão atual.
            /// </summary>
            Versao2,

            /// <summary>
            /// 3 - Estrutura conforme Convênio ICMS 76/03 e 20/04.
            /// </summary>
            Versao3
        }

        /// <summary>
        /// Enumeração com o tipo de natureza das operações.
        /// </summary>
        public enum TipoNaturezaOperacoes
        {
            /// <summary>
            /// 1 - Interestaduais somente operações sujeitas ao regime de substituição tributária.
            /// </summary>
            InterestaduaisTributadas = 1,

            /// <summary>
            /// 2 - Interestaduais - operações com ou sem substituição tributária.
            /// </summary>
            InterestaduaisSubstTributaria,

            /// <summary>
            /// 3 - Totalidade das operações do informantes.
            /// </summary>
            TodasAsOperacoes
        }

        /// <summary>
        /// Enumeração com o tipo de finalidade do arquivo.
        /// </summary>
        public enum TipoFinalidade
        {
            /// <summary>
            /// 1 - Arquivo normal.
            /// </summary>
            Normal = 1,

            /// <summary>
            /// 2 - Retificação total de arquivo: substituição total de informações prestadas pelo contribuinte referentes a este período.
            /// </summary>
            RetificacaoTotal,

            /// <summary>
            /// 3 - Retificação aditiva de arquivo: acréscimo de informação não incluída em arquivos já apresentados.
            /// </summary>
            RetificacaoAditiva,

            /// <summary>
            /// 5 - Desfazimento: arquivo de informação referente a operações/prestações não efetivadas. Neste caso, o arquivo deverá conter, além dos registros tipo 10 e tipo 90, apenas os registros referentes as operações/prestações não efetivadas.
            /// </summary>
            Desfazimento = 5
        }

        #endregion

        #region Campos Privados

        private Loja _loja = null;
        private DateTime _inicio, _fim;
        private TipoNaturezaOperacoes _natureza = TipoNaturezaOperacoes.TodasAsOperacoes;
        private TipoFinalidade _finalidade = TipoFinalidade.Normal;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="loja">O objeto com os dados da loja.</param>
        /// <param name="inicio">A data de início das informações.</param>
        /// <param name="fim">A data de fim das informações.</param>
        /// <param name="natureza">Natureza das operações da loja.</param>
        /// <param name="finalidade">Finalidade do arquivo.</param>
        public Registro10(Loja loja, DateTime inicio, DateTime fim, TipoNaturezaOperacoes natureza, TipoFinalidade finalidade)
        {
            _loja = loja;
            _inicio = inicio;
            _fim = fim;
            _natureza = natureza;
            _finalidade = finalidade;
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 10; }
        }

        public string CNPJ
        {
            get { return FormatCpfCnpjInscEst(_loja.Cnpj); }
        }

        public string InscEstadual
        {
            get { return FormatCpfCnpjInscEst(_loja.InscEst); }
        }

        public string Nome
        {
            get { return NotNullString(_loja.RazaoSocial); }
        }

        public string Municipio
        {
            get { return NotNullString(_loja.Cidade); }
        }

        public string UF
        {
            get { return NotNullString(_loja.Uf); }
        }

        public string Fax
        {
            get { return FormatTelefone(NotNullString(_loja.Fax)); }
        }

        public DateTime DataInicio
        {
            get { return _inicio; }
        }

        public DateTime DataFim
        {
            get { return _fim; }
        }

        public TipoEstruturaArquivo IdentificacaoEstrutura
        {
            get { return TipoEstruturaArquivo.Versao3; }
        }

        public TipoNaturezaOperacoes NaturezaOperacoes
        {
            get { return _natureza; }
        }

        public TipoFinalidade FinalidadeArquivo
        {
            get { return _finalidade; }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 10 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os campos para montar a resposta
            string n01 = Tipo.ToString().PadLeft(2, '0');
            string n02 = CNPJ.PadLeft(14, '0');
            string n03 = InscEstadual.PadRight(14);
            string n04 = Nome.PadRight(35);
            string n05 = Municipio.PadRight(30);
            string n06 = UF.PadRight(2);
            string n07 = Fax.PadLeft(10, '0');
            string n08 = FormatData(DataInicio);
            string n09 = FormatData(DataFim);
            string n10 = ((int)IdentificacaoEstrutura).ToString();
            string n11 = ((int)NaturezaOperacoes).ToString();
            string n12 = ((int)FinalidadeArquivo).ToString();

            // Retorna a string
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10 + 
                n11 + n12;
        }

        #endregion
    }
}