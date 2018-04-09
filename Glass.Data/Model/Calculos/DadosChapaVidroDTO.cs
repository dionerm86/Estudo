using GDA;
using Glass.Comum.Cache;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model.Calculos
{
    class DadosChapaVidroDTO : IDadosChapaVidro
    {
        private static readonly CacheMemoria<ChapaVidro, uint> chapasVidro;
        private readonly IProdutoCalculo produtoCalculo;
        private readonly ChapaVidro chapaVidro;
        
        static DadosChapaVidroDTO()
        {
            chapasVidro = new CacheMemoria<ChapaVidro, uint>("chapasVidro");
        }

        internal DadosChapaVidroDTO(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            this.produtoCalculo = produtoCalculo;
            chapaVidro = ObtemChapaVidro(sessao, produtoCalculo);
        }

        public bool ProdutoPossuiChapaVidro()
        {
            return chapaVidro.IdProd == produtoCalculo.IdProduto;
        }

        public int AlturaMinimaChapaVidro()
        {
            return chapaVidro.AlturaMinima;
        }

        public int AlturaChapaVidro()
        {
            return chapaVidro.Altura;
        }

        public int LarguraMinimaChapaVidro()
        {
            return chapaVidro.LarguraMinima;
        }

        public int LarguraChapaVidro()
        {
            return chapaVidro.Largura;
        }

        public float PercentualAcrescimoM2ChapaVidro(float m2)
        {
            if (chapaVidro.TotM2Minimo3 > 0 && m2 >= (chapaVidro.TotM2Minimo3 * produtoCalculo.Qtde))
            {
                return chapaVidro.PercAcrescimoTotM23 / 100;
            }
            else if (chapaVidro.TotM2Minimo2 > 0 && m2 >= (chapaVidro.TotM2Minimo2 * produtoCalculo.Qtde))
            {
                return chapaVidro.PercAcrescimoTotM22 / 100;
            }
            else if (chapaVidro.TotM2Minimo1 > 0 && m2 >= (chapaVidro.TotM2Minimo1 * produtoCalculo.Qtde))
            {
                return chapaVidro.PercAcrescimoTotM21 / 100;
            }

            return 0;
        }

        private ChapaVidro ObtemChapaVidro(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            var chapaVidroCache = chapasVidro.RecuperarDoCache(produtoCalculo.IdProduto);

            if (chapaVidroCache == null)
            {
                chapaVidroCache = ChapaVidroDAO.Instance.GetElement(sessao, produtoCalculo.IdProduto) ?? new ChapaVidro();
                chapasVidro.AtualizarItemNoCache(chapaVidroCache, produtoCalculo.IdProduto);
            }

            return chapaVidroCache;
        }
    }
}
