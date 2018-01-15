using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Projeto
{
    public class PosicaoPeca : IPosicaoPeca
    {
        #region Construtor padrão

        public PosicaoPeca()
        {

        }

        public PosicaoPeca(IPosicaoPeca pp)
        {
            IdPosicaoPeca = Guid.NewGuid();
            IdItemProjeto = pp.IdItemProjeto;
            CoordX = pp.CoordX;
            CoordY = pp.CoordY;
            Orientacao = pp.Orientacao;
            Valor = pp.Valor;
        }

        #endregion

        #region Propriedades

        public Guid IdPosicaoPeca { get; set; }

        public Guid IdItemProjeto { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public int Orientacao { get; set; }

        public float Valor { get; set; }

        #endregion
    }
}
