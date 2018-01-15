using Glass.Estoque.Negocios.Entidades;
using System;
using System.Collections.Generic;

namespace Glass.Estoque.Negocios
{
    public interface IMovChapaFluxo
    {
        IList<MovChapa> ObtemMovChapa(string idsCorVidro, float espessura, DateTime dataIni, DateTime dataFim);
    }
}
