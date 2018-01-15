using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Projeto
{
    public class PosicaoPecaIndividual : IPosicaoPecaIndividual
    {
        #region Contrutor Padrão

        public PosicaoPecaIndividual()
        {

        }

        public PosicaoPecaIndividual(IPosicaoPecaIndividual pp)
        {
            IdPosicaoPecaIndividual = Guid.NewGuid();
            IdPecaItemProj = pp.IdPecaItemProj;
            CoordX = pp.CoordX;
            CoordY = pp.CoordY;
            Orientacao = pp.Orientacao;
            Valor = pp.Valor;
        }

        #endregion

        #region Propriedades

        public Guid IdPosicaoPecaIndividual { get; set; }

        public Guid IdPecaItemProj { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public int Orientacao { get; set; }

        public float Valor { get; set; }

        #endregion
    }
}
