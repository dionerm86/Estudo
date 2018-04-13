using System.Collections.Generic;

namespace Glass.Data.Model
{
    public interface IDadosAmbiente
    {
        IEnumerable<IAmbienteCalculo> Obter();
    }
}
