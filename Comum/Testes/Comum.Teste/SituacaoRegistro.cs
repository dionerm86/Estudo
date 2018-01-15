using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Comum.Teste
{   
    [ProvedorTraducao(typeof(ProvedorTraducaoSituacaoRegistro))]
    public enum SituacaoRegistro
    {
        Ativo,
        Inativo
    }

    public class ProvedorTraducaoSituacaoRegistro : IProvedorTraducao
    {
        public IEnumerable<InformacoesEnumerador> ObterTraducoes()
        {
            return new InformacoesEnumerador[]
            {
                new InformacoesEnumerador(0, "Ativo", "Registro Ativo"),
                new InformacoesEnumerador(1, "Ativo", "Registro Inativo")
            };
        }
    }
}
