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

        private readonly Lazy<ChapaVidro> chapaVidro;
        
        static DadosChapaVidroDTO()
        {
            chapasVidro = new CacheMemoria<ChapaVidro, uint>("chapasVidro");
        }

        internal DadosChapaVidroDTO(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            this.produtoCalculo = produtoCalculo;
            chapaVidro = new Lazy<ChapaVidro>(() => ObtemChapaVidro(sessao, produtoCalculo));
        }

        public bool ProdutoPossuiChapaVidro()
        {
            return chapaVidro.Value.IdProd == produtoCalculo.IdProduto;
        }

        public int AlturaMinimaChapaVidro()
        {
            return chapaVidro.Value.AlturaMinima;
        }

        public int AlturaChapaVidro()
        {
            return chapaVidro.Value.Altura;
        }

        public int LarguraMinimaChapaVidro()
        {
            return chapaVidro.Value.LarguraMinima;
        }

        public int LarguraChapaVidro()
        {
            return chapaVidro.Value.Largura;
        }

        public float PercentualAcrescimoM2ChapaVidro(float m2)
        {
            if (chapaVidro.Value.TotM2Minimo3 > 0 && m2 >= (chapaVidro.Value.TotM2Minimo3 * produtoCalculo.Qtde))
            {
                return chapaVidro.Value.PercAcrescimoTotM23 / 100;
            }
            else if (chapaVidro.Value.TotM2Minimo2 > 0 && m2 >= (chapaVidro.Value.TotM2Minimo2 * produtoCalculo.Qtde))
            {
                return chapaVidro.Value.PercAcrescimoTotM22 / 100;
            }
            else if (chapaVidro.Value.TotM2Minimo1 > 0 && m2 >= (chapaVidro.Value.TotM2Minimo1 * produtoCalculo.Qtde))
            {
                return chapaVidro.Value.PercAcrescimoTotM21 / 100;
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
