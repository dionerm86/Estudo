using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ChapaVidroDAO))]
	[PersistenceClass("chapa_vidro")]
	public class ChapaVidro
    {
        #region Propriedades

        [PersistenceProperty("IDCHAPAVIDRO", PersistenceParameterType.IdentityKey)]
        public uint IdChapaVidro { get; set; }

        [Log("Produto")]
        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [Log("Altura")]
        [PersistenceProperty("ALTURA")]
        public int Altura { get; set; }

        [Log("Largura")]
        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [Log("Altura mínima")]
        [PersistenceProperty("ALTURAMINIMA")]
        public int AlturaMinima { get; set; }

        [Log("Largura mínima")]
        [PersistenceProperty("LARGURAMINIMA")]
        public int LarguraMinima { get; set; }

        [Log("Tot. M2 mínimo 1")]
        [PersistenceProperty("TOTM2MINIMO1")]
        public float TotM2Minimo1 { get; set; }

        [Log("Perc. Acres. Tot. M2 1")]
        [PersistenceProperty("PERCACRESCIMOTOTM21")]
        public float PercAcrescimoTotM21 { get; set; }

        [Log("Tot. M2 mínimo 2")]
        [PersistenceProperty("TOTM2MINIMO2")]
        public float TotM2Minimo2 { get; set; }

        [Log("Perc. Acres. Tot. M2 2")]
        [PersistenceProperty("PERCACRESCIMOTOTM22")]
        public float PercAcrescimoTotM22 { get; set; }

        [Log("Tot. M2 mínimo 3")]
        [PersistenceProperty("TOTM2MINIMO3")]
        public float TotM2Minimo3 { get; set; }

        [Log("Perc. Acresc. Tot. M2 3")]
        [PersistenceProperty("PERCACRESCIMOTOTM23")]
        public float PercAcrescimoTotM23 { get; set; }

        [Log("Quantidade")]
        [PersistenceProperty("QTDE")]
        public int Quantidade { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// </summary>
        [Log("Situação")]
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRPROD", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("CODINTERNOPROD", DirectionParameter.InputOptional)]
        public string CodInternoProd { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdDescrProduto
        {
            get { return CodInternoProd + " - " + DescrProduto; }
        }

        public string PercAcrescimoTotM2String
        {
            get { return string.Format("({0}% | {1}% | {2}%)", PercAcrescimoTotM21, PercAcrescimoTotM22, PercAcrescimoTotM23); }
        }

        public string DescricaoSituacao
        {
            get { return ((Glass.Situacao)this.Situacao).ToString(); }
        }


        #endregion
    }
}