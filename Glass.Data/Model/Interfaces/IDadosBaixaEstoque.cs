using System.Collections.Generic;

namespace Glass.Data.Model
{
    public interface IDadosBaixaEstoque
    {
        IEnumerable<float> QuantidadesBaixaEstoque();
    }
}
