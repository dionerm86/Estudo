using Glass.Comum.Cache;
using Glass.Data.DAL;

namespace Glass.Data.Model.Calculos
{
    class DadosChapaVidro : IDadosChapaVidro
    {
        private readonly CacheMemoria<ChapaVidro, uint> chapasVidro;

        internal DadosChapaVidro()
        {
            chapasVidro = new CacheMemoria<ChapaVidro, uint>("chapasVidro");
        }

        public bool ProdutoPossuiChapaVidro(IProdutoCalculo produto)
        {
            return ObtemChapaVidro(produto)
                .IdProd == produto.IdProduto;
        }

        public int AlturaMinimaChapaVidro(IProdutoCalculo produto)
        {
            return ObtemChapaVidro(produto)
                .AlturaMinima;
        }

        public int AlturaChapaVidro(IProdutoCalculo produto)
        {
            return ObtemChapaVidro(produto)
                .Altura;
        }

        public int LarguraMinimaChapaVidro(IProdutoCalculo produto)
        {
            return ObtemChapaVidro(produto)
                .LarguraMinima;
        }

        public int LarguraChapaVidro(IProdutoCalculo produto)
        {
            return ObtemChapaVidro(produto)
                .Largura;
        }

        public float PercentualAcrescimoM2ChapaVidro(IProdutoCalculo produto, float m2)
        {
            var chapaVidro = ObtemChapaVidro(produto);

            if (chapaVidro.TotM2Minimo3 > 0 && m2 >= (chapaVidro.TotM2Minimo3 * produto.Qtde))
            {
                return chapaVidro.PercAcrescimoTotM23 / 100;
            }
            else if (chapaVidro.TotM2Minimo2 > 0 && m2 >= (chapaVidro.TotM2Minimo2 * produto.Qtde))
            {
                return chapaVidro.PercAcrescimoTotM22 / 100;
            }
            else if (chapaVidro.TotM2Minimo1 > 0 && m2 >= (chapaVidro.TotM2Minimo1 * produto.Qtde))
            {
                return chapaVidro.PercAcrescimoTotM21 / 100;
            }

            return 0;
        }

        private ChapaVidro ObtemChapaVidro(IProdutoCalculo produto)
        {
            var chapaVidro = chapasVidro.RecuperarDoCache(produto.IdProduto);

            if (chapaVidro == null)
            {
                chapaVidro = ChapaVidroDAO.Instance.GetElement(produto.IdProduto) ?? new ChapaVidro();
                chapasVidro.AtualizarItemNoCache(chapaVidro, produto.IdProduto);
            }

            return chapaVidro ?? new ChapaVidro();
        }
    }
}
