using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.SIntegra
{
    internal class Registro75 : Registro
    {
        #region Campos Privados

        private ProdutosNf _prodNf;
        private Produto _prod;
        private uint _idLoja;
        private DateTime _dataInicial;
        private DateTime _dataFinal;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="prod">O produto usado para o registro.</param>
        public Registro75(ProdutosNf prodNf, uint idLoja, DateTime dataInicio, DateTime dataFim)
        {
            _prodNf = prodNf;
            _idLoja = idLoja;
            _dataInicial = dataInicio;
            _dataFinal = dataFim;
        }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="prod">O produto usado para o registro.</param>
        public Registro75(Produto prod, uint idLoja, DateTime dataInicio, DateTime dataFim)
        {
            _prodNf = null;
            _prod = prod;
            _idLoja = idLoja;
            _dataInicial = dataInicio;
            _dataFinal = dataFim;
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 75; }
        }

        public DateTime DataInicial
        {
            get { return _dataInicial; }
        }

        public DateTime DataFinal
        {
            get { return _dataFinal; }
        }

        public string CodigoProduto
        {
            get { return NotNullString(_prodNf != null ? _prodNf.CodInterno : _prod.CodInterno); }
        }

        public string CodigoNCM
        {
            get { return NotNullString(_prodNf != null ? _prodNf.Ncm : _prod.Ncm); }
        }

        public string Descricao
        {
            get { return NotNullString(_prodNf != null ? _prodNf.DescrProduto : _prod.Descricao); }
        }

        public string UnidadeMedida
        {
            get { return NotNullString(_prodNf != null ? _prodNf.Unidade : _prod.Unidade); }
        }

        public float AliquotaIPI
        {
            get { return _prodNf != null ? _prodNf.AliqIpi : _prod.AliqIPI; }
        }

        public float AliquotaICMS
        {
            get
            {
                return _prodNf != null ? 
                    // Chamado 13546.
                    // O registro 75 deve buscar como padrão a regra de ICMS intraestadual, para isso passamos por parâmetro
                    // somente a identificação da loja para que a alíquota intraestadual seja recuperada.
                    IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)_prodNf.IdProd, _idLoja, null, null) :
                    IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)_prod.IdProd, _idLoja, null, null);
                /*return _prodNf != null ? _prodNf.AliqIcms :
                    IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto((uint)_prod.IdProd, _prod.IdLojaIcms, _prod.IdFornecIcms, _prod.IdClienteIcms);*/
            }
        }

        public decimal ReducaoBaseCalcICMS
        {
            get { return 0; }
        }

        public decimal BaseCalcICMSST
        {
            get { return _prodNf != null ? _prodNf.BcIcmsSt : 0; }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 75 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os dados para o retorno do método
            string n01 = Tipo.ToString();
            string n02 = FormatData(DataInicial);
            string n03 = FormatData(DataFinal);

            string n04 = !string.IsNullOrEmpty(CodigoProduto) && CodigoProduto.Length > 14 ?
                CodigoProduto.Substring(0, 14) : !string.IsNullOrEmpty(CodigoProduto) ?
                CodigoProduto.PadRight(14) : "".PadRight(14);

            string n05 = CodigoNCM.Length > 8 ? CodigoNCM.Substring(0, 8).PadRight(8) : CodigoNCM.PadRight(8);

            string n06 = !string.IsNullOrEmpty(Descricao) && Descricao.Length > 53 ?
                Descricao.Substring(0, 53) : !string.IsNullOrEmpty(Descricao) ?
                Descricao.PadRight(53) : "".PadRight(53);

            string n07 = UnidadeMedida.Trim().PadRight(6);
            string n08 = AliquotaIPI.ToString("0##.#0").Remove(3, 1);
            string n09 = AliquotaICMS.ToString("0#.#0").Remove(2, 1);
            string n10 = ReducaoBaseCalcICMS.ToString("0##.#0").Remove(3, 1);
            string n11 = BaseCalcICMSST.ToString("0##########.#0").Remove(11, 1);

            // Retorna os dados formatados
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10 + n11;
        }

        #endregion
    }
}