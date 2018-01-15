using System;
using GDA;
using Glass.Data.DAL;
using System.ComponentModel;

namespace Glass.Data.Model
{
    #region Enumeradores

    /// <summary>
    /// Possíveis tipos de arquivo para a mesa de corte.
    /// </summary>
    [Flags]
    public enum TipoArquivoMesaCorte
    {
        /// <summary>
        /// SAG.
        /// </summary>
        [Description("SAG")]
        SAG = 1,
        /// <summary>
        /// FOR TXT
        /// </summary>
        [Description("FORTxt")]
        FORTxt = 2,
        /// <summary>
        /// ISO.
        /// </summary>
        [Description("ISO")]
        ISO = 4,
        /// <summary>
        /// DXF.
        /// </summary>
        [Description("DXF")]
        DXF = 8,
        /// <summary>
        /// FML Básico
        /// </summary>
        [Description("FML Básico")]
        FMLBasico = 16,
        /// <summary>
        /// FML.
        /// </summary>
        [Description("FML")]
        FML = 32
    }

    /// <summary>
    /// Possíveis tipos de projeto.
    /// </summary>
    public enum TipoProjetoMesaCorte
    {
        /// <summary>
        /// Outros.
        /// </summary>
        [Description("Outros")]
        Outros,
        /// <summary>
        /// Correr.
        /// </summary>
        [Description("Correr")]
        Correr = 1,
        /// <summary>
        /// Porta.
        /// </summary>
        [Description("Porta")]
        Porta,
        /// <summary>
        /// Correr Plus.
        /// </summary>
        [Description("CorrerPlus")]
        CorrerPlus,
        /// <summary>
        /// Carrinho.
        /// </summary>
        [Description("Carrinho")]
        Carrinho
    }

    #endregion

    [PersistenceBaseDAO(typeof(ArquivoMesaCorteDAO))]
    [PersistenceClass("arquivo_mesa_corte")]
    public class ArquivoMesaCorte : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDARQUIVOMESACORTE", PersistenceParameterType.IdentityKey)]
        public int IdArquivoMesaCorte { get; set; }

        [PersistenceProperty("IDARQUIVOCALCENGINE")]
        public int IdArquivoCalcEngine { get; set; }

        [PersistenceProperty("ARQUIVO")]
        public string Arquivo { get; set; }

        [PersistenceProperty("TIPOARQUIVO")]
        public TipoArquivoMesaCorte TipoArquivo { get; set; }

        [PersistenceProperty("TIPOPROJETO")]
        public TipoProjetoMesaCorte TipoProjeto { get; set; }

        #endregion

        #region Propriedades estendidas

        [PersistenceProperty("CODIGO", DirectionParameter.InputOptional)]
        public string Codigo { get; set; }

        #endregion
    }
}