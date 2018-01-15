using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FornadaDAO))]
    [PersistenceClass("fornada")]
    public class Fornada : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IdFornada", PersistenceParameterType.IdentityKey)]
        public int IdFornada { get; set; }

        [PersistenceProperty("Capacidade")]
        public decimal Capacidade { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("QtdeLida", DirectionParameter.InputOptional)]
        public int QtdeLida { get; set; }

        [PersistenceProperty("M2Lido", DirectionParameter.InputOptional)]
        public decimal M2Lido { get; set; }

        [PersistenceProperty("Etiquetas", DirectionParameter.InputOptional)]
        public string Etiquetas { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("IdCorVidro", DirectionParameter.InputOptional)]
        public int IdCorVidro { get; set; }

        [PersistenceProperty("CorVidro", DirectionParameter.InputOptional)]
        public string CorVidro { get; set; }

        [PersistenceProperty("Espessura", DirectionParameter.InputOptional)]
        public int Espessura { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal Aproveitamento
        {
            get
            {
                if (M2Lido == 0 || Capacidade == 0)
                    return 0;

                return Math.Round((M2Lido * 100) / Capacidade, 2);
            }
        }
    

        #endregion
    }
}
