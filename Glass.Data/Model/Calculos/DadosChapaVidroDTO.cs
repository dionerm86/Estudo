using GDA;
using Glass.Comum.Cache;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model.Calculos
{
    class DadosChapaVidroDTO : BaseCalculoDTO, IDadosChapaVidro
    {
        private static readonly CacheMemoria<ChapaVidro, uint> cacheChapasVidro;
        private readonly IProdutoCalculo produtoCalculo;

        private readonly Lazy<ChapaVidro> chapaVidro;
        
        static DadosChapaVidroDTO()
        {
            cacheChapasVidro = new CacheMemoria<ChapaVidro, uint>("chapasVidro");
        }

        internal DadosChapaVidroDTO(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            this.produtoCalculo = produtoCalculo;
            chapaVidro = ObtemChapaVidro(sessao, produtoCalculo.IdProduto);
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

        private Lazy<ChapaVidro> ObtemChapaVidro(GDASession sessao, uint idProduto)
        {
            return ObterUsandoCache(
                cacheChapasVidro,
                idProduto,
                () => ChapaVidroDAO.Instance.GetElement(sessao, produtoCalculo.IdProduto)
            );
        }
    }
}
