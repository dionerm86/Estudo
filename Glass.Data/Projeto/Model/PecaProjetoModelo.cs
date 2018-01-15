using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Xml.Serialization;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PecaProjetoModeloDAO))]
    [XmlRoot("pecaProjetoModelo")]
	[PersistenceClass("peca_projeto_modelo")]
	public class PecaProjetoModelo
    {
        #region Propriedades

        [XmlAttribute("idPecaProjMod")]
        [PersistenceProperty("IDPECAPROJMOD", PersistenceParameterType.IdentityKey)]
        public uint IdPecaProjMod { get; set; }

        [XmlAttribute("idProjetoModelo")]
        [PersistenceProperty("IDPROJETOMODELO")]
        public uint IdProjetoModelo { get; set; }

        [Log("Arquivo da Mesa de Corte", "Arquivo", typeof(ArquivoMesaCorteDAO))]
        [XmlElement("idArquivoMesaCorte")]
        [PersistenceProperty("IDARQUIVOMESACORTE")]
        public uint? IdArquivoMesaCorte { get; set; }

        [Log("Aplicação", "CodInterno", typeof(EtiquetaAplicacaoDAO))]
        [XmlIgnore]
        [PersistenceProperty("IDAPLICACAO")]
        public uint? IdAplicacao { get; set; }

        [Log("Processo", "CodInterno", typeof(EtiquetaProcessoDAO))]
        [XmlIgnore]
        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [Log("Altura")]
        [XmlAttribute("Altura")]
        [PersistenceProperty("ALTURA")]
        public int Altura { get; set; }

        [Log("Largura")]
        [XmlAttribute("largura")]
        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [Log("Altura 03MM")]
        [XmlAttribute("altura03mm")]
        [PersistenceProperty("ALTURA03MM")]
        public int Altura03MM { get; set; }

        [Log("Largura 03MM")]
        [XmlAttribute("largura03mm")]
        [PersistenceProperty("LARGURA03MM")]
        public int Largura03MM { get; set; }

        [Log("Altura 04MM")]
        [XmlAttribute("altura04mm")]
        [PersistenceProperty("ALTURA04MM")]
        public int Altura04MM { get; set; }

        [Log("Largura 04MM")]
        [XmlAttribute("largura04mm")]
        [PersistenceProperty("LARGURA04MM")]
        public int Largura04MM { get; set; }

        [Log("Altura 05MM")]
        [XmlAttribute("altura05mm")]
        [PersistenceProperty("ALTURA05MM")]
        public int Altura05MM { get; set; }

        [Log("Largura 05MM")]
        [XmlAttribute("largura05mm")]
        [PersistenceProperty("LARGURA05MM")]
        public int Largura05MM { get; set; }

        [Log("Altura 06MM")]
        [XmlAttribute("altura06mm")]
        [PersistenceProperty("ALTURA06MM")]
        public int Altura06MM { get; set; }

        [Log("Largura 06MM")]
        [XmlAttribute("largura06mm")]
        [PersistenceProperty("LARGURA06MM")]
        public int Largura06MM { get; set; }
 
        [Log("Altura 08MM")]
        [XmlAttribute("altura08mm")]
        [PersistenceProperty("ALTURA08MM")]
        public int Altura08MM { get; set; }

        [Log("Largura 08MM")]
        [XmlAttribute("largura08mm")]
        [PersistenceProperty("LARGURA08MM")]
        public int Largura08MM { get; set; }

        [Log("Altura 10MM")]
        [XmlAttribute("altura10mm")]
        [PersistenceProperty("ALTURA10MM")]
        public int Altura10MM { get; set; }

        [Log("Largura 10MM")]
        [XmlAttribute("largura10mm")]
        [PersistenceProperty("LARGURA10MM")]
        public int Largura10MM { get; set; }

        [Log("Altura 12MM")]
        [XmlAttribute("altura12mm")]
        [PersistenceProperty("ALTURA12MM")]
        public int Altura12MM { get; set; }

        [Log("Largura 12MM")]
        [XmlAttribute("largura12mm")]
        [PersistenceProperty("LARGURA12MM")]
        public int Largura12MM { get; set; }

        /// <summary>
        /// 1-Instalação
        /// 2-Caixilho
        /// 3-Desprumo
        /// 4-Molde
        /// </summary>
        [XmlAttribute("tipo")]
        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [Log("Tipo Arquivo")]
        [XmlElement("tipoArquivo")]
        [PersistenceProperty("TIPOARQUIVO")]
        public TipoArquivoMesaCorte? TipoArquivo { get; set; }

        [Log("Quantidade")]
        [XmlAttribute("qtde")]
        [PersistenceProperty("QTDE")]
        public int Qtde { get; set; }

        [Log("Item")]
        [XmlAttribute("item")]
        [PersistenceProperty("ITEM")]
        public string Item { get; set; }

        [Log("Cálculo Qtde.")]
        [XmlAttribute("calculoQtde")]
        [PersistenceProperty("CALCULOQTDE")]
        public string CalculoQtde { get; set; }

        [Log("Cálculo Altura")]
        [XmlAttribute("calculoAltura")]
        [PersistenceProperty("CALCULOALTURA")]
        public string CalculoAltura { get; set; }

        [Log("Cálculo Largura")]
        [XmlAttribute("calculoLargura")]
        [PersistenceProperty("CALCULOLARGURA")]
        public string CalculoLargura { get; set; }

        [Log("Redondo")]
        [XmlAttribute("redondo")]
        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [Log("Obs")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [XmlIgnore]
        [PersistenceProperty("CODMODELO", DirectionParameter.InputOptional)]
        public string CodModelo { get; set; }

        [Log("Arquivo da Mesa de Corte")]
        [XmlIgnore]
        [PersistenceProperty("CODARQMESA", DirectionParameter.InputOptional)]
        public string CodArqMesa { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NOMEFIGURA", DirectionParameter.InputOptional)]
        public string NomeFigura { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ALTURAFIGURA", DirectionParameter.InputOptional)]
        public int AlturaFigura { get; set; }

        [XmlIgnore]
        [PersistenceProperty("LARGURAFIGURA", DirectionParameter.InputOptional)]
        public int LarguraFigura { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        #endregion

        #region Propriedades de Suporte

        public byte[] ImagemPecaProjetoModelo { get; set; }

        [Log("Tipo")]
        [XmlIgnore]
        public string DescrTipo
        {
            get
            {
                switch (Tipo)
                {
                    case 1:
                        return "Instalação";
                    case 2:
                        return "Caixilho";
                    case 3:
                        return "Desprumo";
                    case 4:
                        return "Molde";
                    default:
                        return "";
                }
            }
        }

        [XmlIgnore]
        public string DescrTipoSigla
        {
            get 
            {
                switch (Tipo)
                {
                    case 1:
                        return "I";
                    case 2:
                        return "CX";
                    case 3:
                        return "D";
                    case 4:
                        return "M";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// Propriedade utilizada apenas para facilitar obtenção de dados
        /// </summary>
        [XmlIgnore]
        public string DescrProd { get; set; }

        /// <summary>
        /// Propriedade utilizada apenas para facilitar obtenção de dados
        /// </summary>
        [XmlIgnore]
        public uint IdProd { get; set; }

        /// <summary>
        /// Propriedade utilizada para associar o materialItemProjeto com a pecaItemProjeto
        /// </summary>
        [XmlIgnore]
        public uint IdPecaItemProj { get; set; }

        [XmlIgnore]
        public string ModeloPath
        {
            get { return Utils.GetModelosProjetoVirtualPath + NomeFigura; }
        }

        [XmlIgnore]
        public string ModeloProjetoPath
        {
            get { return Utils.GetModelosProjetoVirtualPath + CodModelo + ".jpg"; }
        }

        public string MotivoCancelamento { get; set; }

        [XmlIgnore]
        public int[] FlagsArqMesa { get; set; }

        public string FlagsArqMesaDescricao { get; set; }

        #endregion

        #region Propriedades do Beneficiamento

        private GenericBenefCollection _beneficiamentos = null;

        [XmlIgnore]
        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (IdPecaProjMod == 0)
                        _beneficiamentos = new GenericBenefCollection();

                    if (_beneficiamentos == null)
                        _beneficiamentos = new List<PecaModeloBenef>(PecaModeloBenefDAO.Instance.GetByPecaProjMod(IdPecaProjMod));
                }
                catch
                {
                    _beneficiamentos = new GenericBenefCollection();
                }

                return _beneficiamentos;
            }
            set { _beneficiamentos = value; }
        }

        /// <summary>
        /// Recarrega a lista de beneficiamentos do banco de dados.
        /// </summary>
        public void RefreshBeneficiamentos()
        {
            _beneficiamentos = null;
        }

        [Log("Beneficiamentos")]
        public string DescrBeneficiamentos
        {
            get { return Beneficiamentos.DescricaoBeneficiamentos; }
        }

        #endregion
    }
}