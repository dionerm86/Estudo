using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.DAL;
using System.Linq;
using System.ComponentModel;
using System.Xml.Serialization;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis tipos de setor.
    /// </summary>
    public enum TipoSetor
    {
        /// <summary>
        /// Por Benef.
        /// </summary>
        [Description("Por Benef.")]
        PorBenef = 1,
        /// <summary>
        /// Pendente.
        /// </summary>
        [Description("Pendente")]
        Pendente,
        /// <summary>
        /// Pronto.
        /// </summary>
        [Description("Pronto")]
        Pronto,
        /// <summary>
        /// Entregue.
        /// </summary>
        [Description("Entregue")]
        Entregue,
        /// <summary>
        /// Por roteiro
        /// </summary>
        [Description("Por Roteiro")]
        PorRoteiro,
        /// <summary>
        /// Exp. Carregamento.
        /// </summary>
        [Description("Exp. Carregamento")]
        ExpCarregamento
    }

    /// <summary>
    /// Possíveis cores da tela do setor.
    /// </summary>
    public enum CorTelaSetor
    {
        /// <summary>
        /// Nenhuma cor.
        /// </summary>
        [Description("")]
        Nenhuma = 0,
        /// <summary>
        /// Azul.
        /// </summary>
        [Description("Azul")]
        Azul = 1,
        /// <summary>
        /// Branco.
        /// </summary>
        [Description("Branco")]
        Branco,
        /// <summary>
        /// Verde.
        /// </summary>
        [Description("Verde")]
        Verde,
        /// <summary>
        /// Cinza.
        /// </summary>
        [Description("Cinza")]
        Cinza, 
        /// <summary>
        /// Laranja.
        /// </summary>
        [Description("Laranja")]
        Laranja
    }

    /// <summary>
    /// Possíveis cores do setor.
    /// </summary>
    public enum CorSetor
    {
        /// <summary>
        /// Preto
        /// </summary>
        [Description("Preto")]
        [SoapEnum("Black")]
        Preto = 1,
        /// <summary>
        /// Cinza claro.
        /// </summary>
        [Description("Cinza Claro")]
        [SoapEnum("Gray")]
        CinzaClaro = 2,
        /// <summary>
        /// Cinza escuro.
        /// </summary>
        [Description("Cinza Escuro")]
        [SoapEnum("DarkGray")]
        CinzaEscuro = 3,
        /// <summary>
        /// Vermelho claro.
        /// </summary>
        [Description("Vermelho Claro")]
        [SoapEnum("Red")]
        VermelhoClaro = 4,
        /// <summary>
        /// Laranja.
        /// </summary>
        [Description("Laranja")]
        [SoapEnum("Orange")]
        Laranja = 5,
        /// <summary>
        /// Marron escuro.
        /// </summary>
        [Description("Marrom Escuro")]
        [SoapEnum("SaddleBrown")]
        MarromEscuro = 6,
        /// <summary>
        /// Verde claro.
        /// </summary>
        [Description("Verde Claro")]
        [SoapEnum("LimeGreen")]
        VerdeClaro = 7,
        /// <summary>
        /// Verde escuro.
        /// </summary>
        [Description("Verde Escuro")]
        [SoapEnum("Green")]
        VerdeEscuro = 8,
        /// <summary>
        /// Azul Claro.
        /// </summary>
        [Description("Azul Claro")]
        [SoapEnum("LightBlue")]
        AzulClaro = 9,
        /// <summary>
        /// Azul escuro.
        /// </summary>
        [Description("Azul Escuro")]
        [SoapEnum("Blue")]
        AzulEscuro = 10,
        /// <summary>
        /// Ciano.
        /// </summary>
        [Description("Ciano")]
        [SoapEnum("DarkCyan")]
        Ciano = 11,
        /// <summary>
        /// Roxo Claro.
        /// </summary>
        [Description("Roxo Claro")]
        [SoapEnum("Purple")]
        RoxoClaro = 12,
        /// <summary>
        /// Vermelho escuro.
        /// </summary>
        [Description("Vermelho Escuro")]
        [SoapEnum("Maroon")]
        VermelhoEscuro = 13,
        /// <summary>
        /// Marrom claro.
        /// </summary>
        [Description("Marrom Claro")]
        [SoapEnum("Chocolate")]
        MarromClaro = 14,
        /// <summary>
        /// Roxo escuro.
        /// </summary>
        [Description("Roxo Escuro")]
        [SoapEnum("Indigo")]
        RoxoEscuro = 15,
        /// <summary>
        /// Amarelo.
        /// </summary>
        [Description("Amarelo")]
        [SoapEnum("Gold")]
        Amarelo = 16
    }

    [PersistenceBaseDAO(typeof(SetorDAO))]
    [PersistenceClass("setor")]
    public class Setor : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDSETOR", PersistenceParameterType.IdentityKey)]
        public int IdSetor { get; set; }

        [PersistenceProperty("IDCNC")]
        public int? IdCnc { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Sigla")]
        [PersistenceProperty("SIGLA")]
        public string Sigla { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// 1-Por Benef.
        /// 2-Pendente
        /// 3-Pronto
        /// 4-Entregue
        /// </summary>
        [PersistenceProperty("TIPO")]
        public TipoSetor Tipo { get; set; }

        /// <summary>
        /// 1-Preto
        /// 2-Cinza Claro
        /// 3-Cinza Escuro
        /// 4-Vermelho
        /// 5-Laranja
        /// 6-Amarelo
        /// 7-Verde Claro
        /// 8-Verde Escuro
        /// 9-Azul Claro
        /// 10-Azul Escuro
        /// </summary>
        [PersistenceProperty("COR")]
        public CorSetor Cor { get; set; }

        /// <summary>
        /// 1-Azul
        /// 2-Branco
        /// 3-Verde
        /// 4-Cinza
        /// 5-Laranja
        /// </summary>
        [PersistenceProperty("CORTELA")]
        public CorTelaSetor CorTela { get; set; }

        [Log("Núm. Sequência")]
        [PersistenceProperty("NUMSEQ")]
        public int NumeroSequencia { get; set; }

        [Log("Entrada Estoque")]
        [PersistenceProperty("ENTRADAESTOQUE")]
        public bool EntradaEstoque { get; set; }

        [Log("Impedir Avanço")]
        [PersistenceProperty("IMPEDIRAVANCO")]
        public bool ImpedirAvanco { get; set; }

        [Log("Informar Rota")]
        [PersistenceProperty("INFORMARROTA")]
        public bool InformarRota { get; set; }

        [Log("Corte")]
        [PersistenceProperty("CORTE")]
        public bool Corte { get; set; }

        [Log("Forno")]
        [PersistenceProperty("FORNO")]
        public bool Forno { get; set; }

        [Log("Laminado")]
        [PersistenceProperty("LAMINADO")]
        public bool Laminado { get; set; }

        [Log("Exibir Setores Lidos")]
        [PersistenceProperty("EXIBIRSETORES")]
        public bool ExibirSetores { get; set; }

        [Log("Exibir Imagem Completa")]
        [PersistenceProperty("EXIBIRIMAGEMCOMPLETA")]
        public bool ExibirImagemCompleta { get; set; }

        [Log("Tempo Login")]
        [PersistenceProperty("TEMPOLOGIN")]
        public int TempoLogin { get; set; }

        [Log("Consultar Antes")]
        [PersistenceProperty("CONSULTARANTES")]
        public bool ConsultarAntes { get; set; }

        [Log("Exibir no Relatório")]
        [PersistenceProperty("EXIBIRRELATORIO")]
        public bool ExibirRelatorio { get; set; }

        private double _desafioPerda;

        [Log("Desafio de Perda Mensal")]
        [PersistenceProperty("DESAFIOPERDA")]
        public double DesafioPerda
        {
            get { return Math.Round(_desafioPerda, 4); }
            set { _desafioPerda = value; }
        }

        private double _metaPerda;

        [Log("Meta de Perda Mensal")]
        [PersistenceProperty("METAPERDA")]
        public double MetaPerda
        {
            get { return Math.Round(_metaPerda, 4); }
            set { _metaPerda = value; }
        }

        [Log("Capacidade Diária de Produção")]
        [PersistenceProperty("CAPACIDADEDIARIA")]
        public int? CapacidadeDiaria { get; set; }

        [Log("Ignorar Capacidade Diária de Produção")]
        [PersistenceProperty("IGNORARCAPACIDADEDIARIA")]
        public bool IgnorarCapacidadeDiaria { get; set; }

        [Log("Permitir Leitura Fora do Roteiro")]
        [PersistenceProperty("PERMITIRLEITURAFORAROTEIRO")]
        public bool PermitirLeituraForaRoteiro { get; set; }

        [Log("Exibir no Painel Comercial")]
        [PersistenceProperty("EXIBIRPAINELCOMERCIAL")]
        public bool ExibirPainelComercial { get; set; }

        [Log("Exibir no Painel da Produção")]
        [PersistenceProperty("EXIBIRPAINELPRODUCAO")]
        public bool ExibirPainelProducao { get; set; }

        [Log("Tempo para alerta de inatividade")]
        [PersistenceProperty("TempoAlertaInatividade")]
        public int TempoAlertaInatividade { get; set; }

        [Log("Informar Cavalete?")]
        [PersistenceProperty("InformarCavalete")]
        public bool InformarCavalete { get; set; }

        [Log("Gerenciar Fornada?")]
        [PersistenceProperty("GerenciarFornada")]
        public bool GerenciarFornada { get; set; }

        [Log("Altura")]
        [PersistenceProperty("Altura")]
        public int Altura { get; set; }

        [Log("Largura")]
        [PersistenceProperty("Largura")]
        public int Largura { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRBENEF", DirectionParameter.InputOptional)]
        public string DescrBenef { get; set; }

        [PersistenceProperty("DESCRCNC", DirectionParameter.InputOptional)]
        public string DescrCnc { get; set; }

        /// <summary>
        /// Campo usado para determinar o horário que uma determinada peça passou neste setor
        /// </summary>
        [PersistenceProperty("DATALEITURA", DirectionParameter.InputOptional)]
        public DateTime DataLeitura { get; set; }

        private string _funcLeitura;

        [PersistenceProperty("FUNCLEITURA", DirectionParameter.InputOptional)]
        public string FuncLeitura
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_funcLeitura); }
            set { _funcLeitura = value; }
        }

        [PersistenceProperty("BENEFSETOR", DirectionParameter.InputOptional)]
        public string BenefSetor { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Situação")]
        public string DescrSituacao
        {
            get { return Colosoft.Translator.Translate(Situacao).Format(); }
        }

        [Log("Tipo")]
        public string DescrTipo
        {
            get { return Colosoft.Translator.Translate(Tipo).Format(); }
        }

        [Log("Cor")]
        public string DescrCor
        {
            get { return Colosoft.Translator.Translate(Cor).Format(); }
        }

        [Log("Cor Tela")]
        public string DescrCorTela
        {
            get 
            {
                return Colosoft.Translator.Translate(CorTela).Format();
            }
        }

        public string DescrCorSystem
        {
            get 
            {
                var field = typeof(CorSetor).GetField(Cor.ToString());
                if (field == null) return "Black";
                return ((SoapEnumAttribute)field.GetCustomAttributes(typeof(SoapEnumAttribute), false).First()).Name;
            }
        }

        public List<uint> Beneficiamentos
        {
            get
            {
                if (String.IsNullOrEmpty(BenefSetor))
                    return new List<uint>();

                return BenefSetor.Split(',').Select(x => Glass.Conversoes.StrParaUint(x)).ToList();
            }
            set
            {
                BenefSetor = String.Join(",", value.Select(x => x.ToString()).ToArray());
            }
        }

        public bool SetorPertenceARoteiro
        {
            get { return Tipo == TipoSetor.PorRoteiro || Tipo == TipoSetor.PorBenef; }
        }

        #endregion
    }
}