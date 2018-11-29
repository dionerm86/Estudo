using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.SIntegra
{
    internal class Registro54 : Registro
    {
        #region Campos Privados

        private NotaFiscal _nf = null;
        private Loja _loja = null;
        private Cliente _cliente = null;
        private Fornecedor _fornecedor = null;
        private ProdutosNf _prodNf = null;
        private int _numItem = 0;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="prodNf">O produto da nota fiscal.</param>
        public Registro54(ProdutosNf prodNf, int numItem)
        {
            _prodNf = prodNf;
            _nf = NotaFiscalDAO.Instance.GetElement(prodNf.IdNf);
            _numItem = numItem;
            
            if (_nf.IdLoja != null) _loja = LojaDAO.Instance.GetElement(_nf.IdLoja.Value);
            if (_nf.IdCliente != null) _cliente = ClienteDAO.Instance.GetElementByPrimaryKey(_nf.IdCliente.Value);
            if (_nf.IdFornec != null) _fornecedor = FornecedorDAO.Instance.GetElement(_nf.IdFornec.Value);
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 54; }
        }

        public string CNPJ
        {
            get { return FormatCpfCnpjInscEst((_nf.TipoDocumento == 2 || _fornecedor == null) && _cliente != null ? _cliente.CpfCnpj : _fornecedor != null ? _fornecedor.CpfCnpj : string.Empty); }
        }

        public string Modelo
        {
            get { return NotNullString(_nf.Modelo); }
        }

        public string Serie
        {
            get { return NotNullString(_nf.Serie); }
        }

        public uint Numero
        {
            get { return _nf.NumeroNFe; }
        }

        public string CFOP
        {
            get { return NotNullString(_prodNf.IdCfop > 0 ? _prodNf.CodCfop : _nf.CodCfop); }
        }

        public string CST
        {
            get { return NotNullString(!String.IsNullOrEmpty(_prodNf.Cst) ? _prodNf.CstOrig + _prodNf.Cst : "060"); }
        }

        public int NumeroItem
        {
            get { return _numItem; }
        }

        public string CodigoProduto
        {
            get { return NotNullString(_prodNf.CodInterno); }
        }

        public float Quantidade
        {
            get { return ProdutosNfDAO.Instance.ObtemQtdDanfe(null, _prodNf, true); }
        }

        public decimal ValorProduto
        {
            get { return _prodNf.Total; }
        }

        public decimal ValorDesconto
        {
            get
            {
                if (_nf.TotalProd == 0)
                {
                    return 0;
                }

                return Math.Round((ValorProduto / _nf.TotalProd) * _nf.Desconto, 2);
            }
        }

        public decimal BaseCalcICMS
        {
            get { return _prodNf.BcIcms; }
        }

        public decimal BaseCalcICMSST
        {
            get { return _prodNf.BcIcmsSt; }
        }

        public decimal ValorIPI
        {
            get { return _prodNf.ValorIpi; }
        }

        public float AliquotaICMS
        {
            get { return _prodNf.AliqIcms; }
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
            string n02 = CNPJ.PadLeft(14, '0');
            string n03 = Modelo.PadLeft(2, '0');
            string n04 = Serie.ToString().PadRight(3);
            string n05 = Numero.ToString().PadLeft(6, '0');
            string n06 = CFOP.PadLeft(4, '0');
            string n07 = CST.PadRight(3);
            string n08 = NumeroItem.ToString().PadLeft(3, '0');

            string n09 = !string.IsNullOrEmpty(CodigoProduto) && CodigoProduto.Length > 14 ?
                CodigoProduto.Substring(0,14) : !string.IsNullOrEmpty(CodigoProduto) ?
                CodigoProduto.PadRight(14) : "".PadRight(14);

            string n10 = Quantidade.ToString("0#######.##0").Remove(8, 1);
            string n11 = ValorProduto.ToString("0#########.#0").Remove(10, 1);
            string n12 = ValorDesconto.ToString("0#########.#0").Remove(10, 1);
            string n13 = BaseCalcICMS.ToString("0#########.#0").Remove(10, 1);
            string n14 = BaseCalcICMSST.ToString("0#########.#0").Remove(10, 1);
            string n15 = ValorIPI.ToString("0#########.#0").Remove(10, 1);
            string n16 = AliquotaICMS.ToString("0#.#0").Remove(2, 1);

            // Retorna os dados formatados
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10 +
                n11 + n12 + n13 + n14 + n15 + n16;
        }

        #endregion
    }
}
