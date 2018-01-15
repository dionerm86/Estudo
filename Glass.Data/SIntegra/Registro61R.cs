using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.SIntegra
{
    internal class Registro61R : Registro
    {
        #region Campos Privados

        private int _mesEmissao;
        private int _anoEmissao;
        private Produto _produto;
        private ProdutosNf[] _produtosMes = null;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="prod">O produto que está sendo considerado.</param>
        /// <param name="mesEmissao">O mês do resumo fiscal.</param>
        /// <param name="anoEmissao">O ano do resumo fiscal.</param>
        public Registro61R(Produto prod, int mesEmissao, int anoEmissao)
        {
            _produto = prod;
            _mesEmissao = mesEmissao;
            _anoEmissao = anoEmissao;
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 61; }
        }

        public char Subtipo
        {
            get { return 'R'; }
        }

        public int MesEmissao
        {
            get { return _mesEmissao; }
        }

        public int AnoEmissao
        {
            get { return _anoEmissao; }
        }

        public string CodigoProduto
        {
            get { return _produto.CodInterno; }
        }

        public float Quantidade
        {
            get
            {
                if (_produtosMes == null || _produtosMes.Length == 0)
                    _produtosMes = RecuperaProdutosMes(_produto, _mesEmissao, _anoEmissao);

                float retorno = 0;
                foreach (ProdutosNf pNf in _produtosMes)
                    retorno += pNf.Qtde;

                return retorno;
            }
        }

        public decimal ValorBruto
        {
            get
            {
                if (_produtosMes == null || _produtosMes.Length == 0)
                    _produtosMes = RecuperaProdutosMes(_produto, _mesEmissao, _anoEmissao);

                decimal retorno = 0;
                foreach (ProdutosNf pNf in _produtosMes)
                    retorno += pNf.Total;

                return retorno;
            }
        }

        public decimal BaseCalcICMS
        {
            get
            {
                if (_produtosMes == null || _produtosMes.Length == 0)
                    _produtosMes = RecuperaProdutosMes(_produto, _mesEmissao, _anoEmissao);

                decimal retorno = 0;
                foreach (ProdutosNf pNf in _produtosMes)
                    retorno += pNf.BcIcms;

                return retorno;
            }
        }

        public float AliquotaICMS
        {
            get { return IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(null, (uint)_produto.IdProd, _produto.IdLojaIcms, _produto.IdFornecIcms, _produto.IdClienteIcms); }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera uma lista dos produtos das notas fiscais de um mês e ano específicos.
        /// </summary>
        /// <param name="prod"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        /// <returns></returns>
        private static ProdutosNf[] RecuperaProdutosMes(Produto prod, int mes, int ano)
        {
            return ProdutosNfDAO.Instance.GetByIdProdPeriodo((uint)prod.IdProd, new DateTime(ano, mes, 1), new DateTime(ano, mes + 1, 1).AddDays(-1));
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 61R para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os dados para o retorno do método
            string n01 = Tipo.ToString();
            string n02 = Subtipo.ToString();
            string n03 = MesEmissao.ToString("0#") + AnoEmissao.ToString("0###");

            string n04 = !string.IsNullOrEmpty(CodigoProduto) && CodigoProduto.Length > 14 ?
                CodigoProduto.Substring(0, 14) : !string.IsNullOrEmpty(CodigoProduto) ?
                CodigoProduto.PadRight(14) : "".PadRight(14);

            string n05 = Quantidade.ToString("0#########.##0").Remove(10, 1);
            string n06 = ValorBruto.ToString("0#############.#0").Remove(14, 1);
            string n07 = BaseCalcICMS.ToString("0#############.#0").Remove(14, 1);
            string n08 = AliquotaICMS.ToString("0#.#0").Remove(2, 1);
            string n09 = "".PadRight(54);

            // Retorna os dados formatados
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09;
        }

        #endregion
    }
}