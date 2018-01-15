using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    public interface IProvedorArquivoCartaoNaoIdentificado
    {
        bool PodeCancelarArquivo(int idArquivoCartaoNaoIdentificado); 
    }
}
