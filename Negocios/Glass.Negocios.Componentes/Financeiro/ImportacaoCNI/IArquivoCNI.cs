using Glass.Financeiro.Negocios.Entidades;
using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Componentes.LayoutCNI
{
    public interface IArquivoCNI
    {
        List<CartaoNaoIdentificado> Importar(int idArqCni);

        bool LayoutValido();
    }
}
