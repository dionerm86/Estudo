using GDA;
using Glass.Data.DAL;
using Glass.Log;
using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis tipos de processo.
    /// </summary>
    public enum EtiquetaTipoProcesso
    {
        /// <summary>
        /// Instalação.
        /// </summary>
        [Description("Instalação")]
        Instalacao = 1,
        /// <summary>
        /// Caixilho.
        /// </summary>
        [Description("Caixilho")]
        Caixilho
    }

    [PersistenceBaseDAO(typeof(EtiquetaProcessoDAO))]
	[PersistenceClass("etiqueta_processo")]
	public class EtiquetaProcesso : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPROCESSO", PersistenceParameterType.IdentityKey)]
        public int IdProcesso { get; set; }

        [Log("CODINTERNO")]
        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [Log("DESCRICAO")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("IDAPLICACAO")]
        [PersistenceProperty("IDAPLICACAO")]
        [PersistenceForeignKey(typeof(EtiquetaAplicacao), "IdAplicacao")]
        public int? IdAplicacao { get; set; }

        [Log("DESTACARETIQUETA")]
        [PersistenceProperty("DESTACARETIQUETA")]
        public bool DestacarEtiqueta { get; set; }

        /// <summary>
        /// Define se a peça com este processo irá gerar uma forma inexistente na exportação para o Optyway 
        /// para o conferente saber que precisa criar uma forma para a mesma, desde que a mesma não possua forma.
        /// </summary>
        [Log("GERARFORMAINEXISTENTE")]
        [PersistenceProperty("GERARFORMAINEXISTENTE")]
        public bool GerarFormaInexistente { get; set; }

        /// <summary>
        /// Define se a peça com esse processo irá gerar Arquivo de Mesa
        /// </summary>
        [Log("GERARARQUIVODEMESA")]
        [PersistenceProperty("GERARARQUIVODEMESA")]
        public bool GerarArquivoDeMesa { get; set; }

        [Log("Número dias úteis data de entrega")]
        [PersistenceProperty("NUMERODIASUTEISDATAENTREGA")]
        public int NumeroDiasUteisDataEntrega { get; set; }

        [Log("SITUACAO")]
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        [Log("TIPOPROCESSO")]
        [PersistenceProperty("TIPOPROCESSO")]
        public EtiquetaTipoProcesso? TipoProcesso { get; set; }

        [PersistenceProperty("TipoPedido")]
        public string TipoPedido { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("DESCRAPLICACAO", DirectionParameter.InputOptional)]
        public string DescrAplicacao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get { return Colosoft.Translator.Translate(Situacao).Format(); }
        }

        public string DescrTipoProcesso
        {
            get 
            {
                return Colosoft.Translator.Translate(TipoProcesso).Format();
            }
        }

        [Log("TipoPedido")]
        public string DescricaoTipoPedido
        {
            get
            {
                var retorno = string.Empty;

                if (string.IsNullOrEmpty(TipoPedido))
                    return retorno;

                foreach (var tipo in TipoPedido.Split(','))
                {
                    switch (tipo)
                    {
                        case "5":
                            retorno += "Mão de obra Especial, ";
                            break;
                        case "3":
                            retorno += "Mão de obra, ";
                            break;
                        case "4":
                            retorno += "Produção, ";
                            break;
                        case "1":
                            retorno += "Venda, ";
                            break;
                    }
                }

                return retorno.TrimEnd(',', ' ');
            }
        }

        #endregion
    }
}