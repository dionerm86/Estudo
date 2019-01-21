using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Xml.Serialization;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PecaItemProjetoDAO))]
	[PersistenceClass("peca_item_projeto")]
	public class PecaItemProjeto : IPecaItemProjeto
    {
        #region Propriedades

        [PersistenceProperty("IDPECAITEMPROJ", PersistenceParameterType.IdentityKey)]
        public uint IdPecaItemProj { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint IdItemProjeto { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint? IdProd { get; set; }

        [PersistenceProperty("ALTURA")]
        public int Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        /// <summary>
        /// 1-Instalação
        /// 2-Caixilho
        /// </summary>
        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [PersistenceProperty("QTDE")]
        public int Qtde { get; set; }

        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("IMAGEMEDITADA")]
        public bool ImagemEditada { get; set; }

        [PersistenceProperty("Guid")]
        public Guid Guid { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDPECAPROJMOD", DirectionParameter.InputOptional)]
        public uint IdPecaProjMod { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ITEM", DirectionParameter.InputOptional)]
        public string Item { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDPEDIDO", DirectionParameter.InputOptional)]
        public uint? IdPedido { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDPRODPED", DirectionParameter.InputOptional)]
        public uint? IdProdPed { get; set; }
 
        [XmlIgnore]
        [PersistenceProperty("QTDEPRODPED", DirectionParameter.InputOptional)]
        public int? QtdeProdPed { get; set; }

        [PersistenceProperty("Ncm", DirectionParameter.InputOptional)]
        public string Ncm { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDARQUIVOMESACORTE", DirectionParameter.InputOptional)]
        public uint? IdArquivoMesaCorte { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TIPOARQUIVOMESACORTE", DirectionParameter.InputOptional)]
        public int? TipoArquivoMesaCorte { get; set; }

        /// <summary>
        /// Quantidade que será exibida no relatório de projetos.
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("QTDEEXIBIRRELATORIO", DirectionParameter.InputOptional)]
        public int QtdeExibirRelatorio { get; set; }

        #endregion

        #region Propriedades de Suporte

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

        [XmlIgnore]
        public string TituloAltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Altura" : "Largura"; }
        }

        [XmlIgnore]
        public string TituloAltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Largura" : "Altura"; }
        }

        [XmlIgnore]
        public string AltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Altura.ToString() : Largura.ToString(); }
        }

        [XmlIgnore]
        public string AltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Largura.ToString() : Altura.ToString(); }
        }

        private string _etiquetas = null;

        [XmlIgnore]
        public string Etiquetas
        {
            get
            {
                if (_etiquetas == null)
                    _etiquetas = PecaItemProjetoDAO.Instance.ObtemEtiquetas(IdPedido, IdProdPed, Qtde);

                return _etiquetas;
            }
        }

        [XmlIgnore]
        public bool PossuiArquivoDXF
        {
            get
            {
                return System.IO.File.Exists(PCPConfig.CaminhoSalvarDxf + IdPecaItemProj + ".dxf");
            }
        }

        #endregion

        #region Propriedades do Beneficiamento

        private List<PecaItemProjBenef> _beneficiamentos = null;

        [XmlIgnore]
        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (IdProd.GetValueOrDefault() == 0 || !ProdutoDAO.Instance.CalculaBeneficiamento((int)IdProd.Value))
                        _beneficiamentos = new List<PecaItemProjBenef>();

                    if (_beneficiamentos == null)
                        _beneficiamentos = new List<PecaItemProjBenef>(PecaItemProjBenefDAO.Instance.GetByPecaItemProj(IdPecaItemProj));
                }
                catch
                {
                    _beneficiamentos = new List<PecaItemProjBenef>();
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

        [XmlIgnore]
        public string DescrBeneficiamentos
        {
            get { return Beneficiamentos.DescricaoBeneficiamentos; }
        }

        /// <summary>
        /// Usado para exportação de pedido.
        /// </summary>
        public string ServicosInfoBenef
        {
            get
            {
                GenericBenefCollection benef = Beneficiamentos.ToPecasItemProjeto(IdPecaItemProj);
                foreach (GenericBenef b in benef)
                    b.IdBenefConfig = 0;

                return benef.ServicosInfo;
            }
            set { Beneficiamentos.ServicosInfo = value; }
        }

        #endregion

        #region Propriedades para Log

        public uint IdLog
        {
            get { return Glass.Conversoes.StrParaUint(IdPecaItemProj + (!String.IsNullOrEmpty(Item) ? Item.PadLeft(2, '0') : "00")); }
        }

        /// <summary>
        /// Propriedade com o tipo de alteração feita.
        /// Usara para o Log de alterações.
        /// </summary>
        [Log("Tipo de Alteração")]
        public string TipoAlteracao { get; set; }

        Guid IPecaItemProjeto.IdPecaItemProj
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IPosicaoPecaIndividual> PosicoesPeca
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
