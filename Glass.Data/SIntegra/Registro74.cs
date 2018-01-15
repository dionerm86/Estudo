using System;

namespace Glass.Data.SIntegra
{
    internal class Registro74 : Registro
    {
        #region Campos Privados

        private DateTime _dataInventario;
        private string _codigoProduto;
        private float _qtd;
        private float _valor;
        private string _cnpjProp;
        private string _inscEstProp;
        private string _ufProp;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        public Registro74(DateTime dataInventario, string codigoProduto, float qtd, float valor, string cnpjProp,
            string inscEstProp, string ufProp)
        {
            _dataInventario = dataInventario;
            _codigoProduto = codigoProduto;
            _qtd = qtd;
            _valor = valor;
            _cnpjProp = cnpjProp;
            _inscEstProp = inscEstProp;
            _ufProp = ufProp;
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 74; }
        }

        public DateTime DataInventario
        {
            get { return _dataInventario; }
        }

        public string CodigoProduto
        {
            get { return NotNullString(_codigoProduto); }
        }

        public float Quantidade
        {
            get { return _qtd; }
        }

        public float ValorProduto
        {
            get { return _valor > 0 ? _valor : _valor * -1; }
        }

        /// <summary>
        /// 1 - Mercadorias de propriedade do Informante e em seu poder
        /// 2 - Mercadorias de propriedade do Informante em poder de terceiros
        /// 3 - Mercadorias de propriedade de terceiros em poder do Informante
        /// </summary>
        public int CodigoPosse
        {
            get { return 1; }
        }

        public string CnpjProp
        {
            get { return FormatCpfCnpjInscEst(_cnpjProp); }
        }

        public string InscEstProp
        {
            get { return FormatCpfCnpjInscEst(_inscEstProp); }
        }

        public string UfProp
        {
            get { return NotNullString(_ufProp); }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 54 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os dados para o retorno do método
            string n01 = Tipo.ToString();
            string n02 = DataInventario.ToString("yyyyMMdd");

            string n03 = !string.IsNullOrEmpty(CodigoProduto) && CodigoProduto.Length > 14 ?
                CodigoProduto.Substring(0, 14) : !string.IsNullOrEmpty(CodigoProduto) ?
                CodigoProduto.PadRight(14) : "".PadRight(14);

            string n04 = Quantidade.ToString("0#########.##0").Remove(10, 1);
            string n05 = ValorProduto.ToString("0##########.#0").Remove(11, 1);
            string n06 = CodigoPosse.ToString();
            string n07 = CnpjProp.PadLeft(14, '0');
            string n08 = "".PadRight(14);
            string n09 = UfProp;
            string n10 = "".PadRight(45);

            // Retorna os dados formatados
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10;
        }

        #endregion
    }
}