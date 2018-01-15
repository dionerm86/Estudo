using System;
using GDA;
using Glass.Data.Helper;
using System.Xml.Serialization;
using Glass.Data.DAL;
using Glass.Log;
using System.Linq;
using System.Collections.Generic;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProjetoModeloDAO))]
    [XmlRoot("projetoModelo")]
	[PersistenceClass("projeto_modelo")]
	public class ProjetoModelo
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            Ativo = 1,
            Inativo
        }

        #endregion

        #region Propriedades

        [XmlAttribute("idProjetoModelo")]
        [PersistenceProperty("IDPROJETOMODELO", PersistenceParameterType.IdentityKey)]
        public uint IdProjetoModelo { get; set; }

        [Log("Grupo", "Descricao", typeof(GrupoModeloDAO))]
        [XmlAttribute("idGrupoModelo")]
        [PersistenceProperty("IDGRUPOMODELO")]
        public uint IdGrupoModelo { get; set; }

        [Log("C�digo")]
        [XmlAttribute("codigo")]
        [PersistenceProperty("CODIGO")]
        public string Codigo { get; set; }

        [Log("Descri��o")]
        [XmlAttribute("descricao")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Nome Figura")]
        [XmlAttribute("nomeFigura")]
        [PersistenceProperty("NOMEFIGURA")]
        public string NomeFigura { get; set; }

        [Log("Nome Figura Associada")]
        [XmlAttribute("nomeFiguraAssociada")]
        [PersistenceProperty("NOMEFIGURAASSOCIADA")]
        public string NomeFiguraAssociada { get; set; }

        [Log("Altura Figura")]
        [XmlAttribute("alturaFigura")]
        [PersistenceProperty("ALTURAFIGURA")]
        public int AlturaFigura { get; set; }

        [Log("Largura Figura")]
        [XmlAttribute("larguraFigura")]
        [PersistenceProperty("LARGURAFIGURA")]
        public int LarguraFigura { get; set; }

        [Log("Espessura")]
        [XmlAttribute("espessura")]
        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        [Log("Texto Or�amento")]
        [XmlAttribute("textoOrcamento")]
        [PersistenceProperty("TEXTOORCAMENTO")]
        public string TextoOrcamento { get; set; }

        [Log("Texto Or�amento Vidro")]
        [XmlAttribute("textoOrcamentoVidro")]
        [PersistenceProperty("TEXTOORCAMENTOVIDRO")]
        public string TextoOrcamentoVidro { get; set; }

        /// <summary>
        /// 1-Qtd, Largura V�o, Altura V�o
        /// 2-Qtd, Largura V�o, Altura V�o, Portas(Largura)
        /// 3-Qtd, Largura V�o, Altura V�o, Trinco (U)
        /// 4-Qtd, Largura V�o, Altura V�o, Altura Puxador
        /// 5-Qtd, Largura V�o, Altura V�o, Altura Fechadura
        /// 6-Qtd, Largura V�o, Altura V�o, Altura Fechadura, Portas(Largura)
        /// 7-Qtd, Largura V�o, Altura V�o, Altura Puxador, Portas(Largura)
        /// 8-Qtd, Largura V�o, Altura Porta, Altura V�o, Altura Fechadura, Esp Tubo(U)
        /// 9-Qtd, Largura V�o, Altura Porta, Altura V�o, Altura Puxador, Esp Tubo(U)
        /// 10-Qtd(D), Largura(D), Altura(D), Altura Puxador(D), Qtd(E), Largura(E), Altura(E), Altura Puxador(E)
        /// 11-Qtd, Largura Porta, Largura V�o, Altura V�o, Altura Puxador
        /// 12-Qtd, Largura V�o, Altura porta, Altura V�o, Altura Puxador
        /// 13-Qtd, Largura Porta, Largura V�o, Altura Porta, Altura V�o, Altura Puxador
        /// 14-Qtd, Largura V�o, Altura Inferior, Altura V�o
        /// 15-Qtd, Largura Colante, Largura Passante, Altura V�o
        /// 16-Qtd, Largura V�o, Altura B�scula, Altura V�o
        /// 17-Qtd, Largura V�o, Largura B�scula, Altura V�o
        /// 18-Qtd, Largura B�scula, Largura V�o, Altura B�scula, Altura V�o
        /// 19-Qtd, Largura Pivotante, Largura V�o, Altura V�o
        /// 20-Qtd, Largura V�o, Altura Pivotante, Altura V�o
        /// </summary>
        [XmlAttribute("tipoMedidasInst")]
        [PersistenceProperty("TIPOMEDIDASINST")]
        public int TipoMedidasInst { get; set; }

        /// <summary>
        /// Identifica quais medidas ser�o desenhadas em tempo de execu��o
        /// 0-Nenhuma
        /// 1-Altura Puxador/Fechadura Lateral Esquerda
        /// </summary>
        [XmlAttribute("tipoDesenho")]
        [PersistenceProperty("TIPODESENHO")]
        public int TipoDesenho { get; set; }

        /// <summary>
        /// Indica como deve ser feito o c�lculo dos alum�nios de cada modelo
        /// </summary>
        [XmlAttribute("tipoCalcAluminio")]
        [PersistenceProperty("TIPOCALCALUMINIO")]
        public int TipoCalcAluminio { get; set; }

        /// <summary>
        /// Verifica se neste projeto precisa ser inserido o eixo do puxador
        /// </summary>
        [Log("Eixo Puxador")]
        [XmlAttribute("eixoPuxador")]
        [PersistenceProperty("EIXOPUXADOR")]
        public bool EixoPuxador { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// </summary>
        [XmlAttribute("situacao")]
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Produto para NF-e", "CodInterno", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPRODPARANF")]
        public uint? IdProdParaNf { get; set; }

        [PersistenceProperty("CorVidro")]
        public string CorVidro { get; set; }

        #endregion

        #region Propriedades Estendidas

        [XmlIgnore]
        [PersistenceProperty("DESCRGRUPO", DirectionParameter.InputOptional)]
        public string DescrGrupo { get; set; }

        #endregion

        #region Propriedades de Suporte

        public byte[] ImagemProjetoModelo { get; set; }

        [XmlIgnore]
        public string MedidasProjMod { get; set; }

        [XmlIgnore]
        public bool EditVisible
        {
            get { return true; }
        }

        [XmlIgnore]
        public bool AtivarInativarVisible
        {
            get
            {
                if (Configuracoes.ProjetoConfig.ControleModeloProjeto.ApenasAdminSyncAtivarModeloProjeto)
                {
                    if (UserInfo.GetUserInfo.IsAdminSync || Situacao == 1)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
        }

        [Log("Situa��o")]
        [XmlIgnore]
        public string DescrSituacao
        {
            get { return Situacao == 1 ? "Ativo" : Situacao == 2 ? "Inativo" : "N/D"; }
        }

        [XmlIgnore]
        public string CodDescr
        {
            get { return Codigo + (!String.IsNullOrEmpty(Descricao) ? " - " + Descricao : String.Empty); }
        }
        
        [XmlIgnore]
        public string ModeloPath
        {
            get 
            {
                return "../../Handlers/LoadImage.ashx?path=" + Utils.GetModelosProjetoPath + NomeFigura +
                    "&largura=" + (int)(LarguraFigura * 0.53) + "&altura=" + (int)(AlturaFigura * 0.53);
            }
        }

        [XmlIgnore]
        public string DescrCorVidro
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CorVidro))
                    return "";

                var cores = new List<string>();

                foreach (var idCorVidr in CorVidro.Split(',').Select(f=>f.StrParaUint()))
                {
                    cores.Add(CorVidroDAO.Instance.GetNome(idCorVidr));
                }

                return string.Join(", ", cores);
            }
        }

        #endregion
    }
}