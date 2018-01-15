using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Comum.Teste
{
    [ProvedorTraducao(typeof(ProvedorTraducaoTipoCalculo))]
    public enum TipoCalculo
    {
        Qtd = 1,
        M2,
        Perimetro
    }

    class ProvedorTraducaoTipoCalculo : IProvedorMultiplaTraducao
    {
        public IEnumerable<IEnumerable<InformacoesEnumerador>> ObterTraducoes()
        {
            return new InformacoesEnumerador[][]
            {
                new InformacoesEnumerador[]
                {
                    new InformacoesEnumerador(TipoCalculo.Qtd, ""),
                    new InformacoesEnumerador(TipoCalculo.M2, "m²"),
                    new InformacoesEnumerador(TipoCalculo.Perimetro, "ml"),
                },
                new InformacoesEnumerador[]
                {
                    new InformacoesEnumerador(TipoCalculo.Qtd, "Qtd."),
                    new InformacoesEnumerador(TipoCalculo.M2, "M²"),
                    new InformacoesEnumerador(TipoCalculo.Perimetro, "ML"),
                }
            };
        }
    }
}
