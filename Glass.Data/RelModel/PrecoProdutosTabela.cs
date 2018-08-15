using GDA;
using Glass.Data.RelDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PrecoProdutosTabelaDAO))]
    [PersistenceClass("produto")]
    public class PrecoProdutosTabela
    {
        [PersistenceProperty("CodInterno")]
        public string CodInterno { get; set; }

        [PersistenceProperty("DescProduto")]
        public string DescProduto { get; set; }

        [PersistenceProperty("DescGrupo")]
        public string DescGrupo { get; set; }

        [PersistenceProperty("DescSubgrupo")]
        public string DescSubgrupo { get; set; }

        [PersistenceProperty("ValorOriginal")]
        public decimal ValorOriginal { get; set; }

        [PersistenceProperty("ValorTabela")]
        public decimal ValorTabela { get; set; }

        [PersistenceProperty("Altura")]
        public int Altura { get; set; }

        [PersistenceProperty("Largura")]
        public int Largura { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        [PersistenceProperty("PercDescAcrescimo")]
        public double PercDescAcrescimo { get; set; }

        public string DescrPercDescAcrescimo
        {
            get
            {
                var perc = PercDescAcrescimo;

                if (perc > 1)
                    return "+" + ((perc - 1) * 100).ToString("0.##") + "%";
                else if (perc < 1)
                    return "-" + ((1 - perc) * 100).ToString("0.##") + "%";

                return "0%";
            }
        }
    }
}
