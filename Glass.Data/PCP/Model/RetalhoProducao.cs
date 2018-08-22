using System;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Log;
using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis situações do retalho.
    /// </summary>
    public enum SituacaoRetalhoProducao : int
    {
        /// <summary>
        /// Disponível.
        /// </summary>
        [Description("Disponível")]
        Disponivel = 1,
        /// <summary>
        /// Cancelado.
        /// </summary>
        [Description("Cancelado")]
        Cancelado,
        /// <summary>
        /// Em uso.
        /// </summary>
        [Description("Em uso")]
        EmUso,
        /// <summary>
        /// Em estoque.
        /// </summary>
        [Description("Em estoque")]
        EmEstoque,
        /// <summary>
        /// Vendido
        /// </summary>
        [Description("Vendido")]
        Vendido,
        /// <summary>
        /// Vendido
        /// </summary>
        [Description("Perda")]
        Perda,
        /// <summary>
        /// Indisponível.
        /// </summary>
        [Description("Indisponível")]
        Indisponivel
    }

    [PersistenceBaseDAO(typeof(RetalhoProducaoDAO))]
    [PersistenceClass("retalho_producao")]
    public class RetalhoProducao
    {
        #region Variaveis Locais

        private string _nomeFunc { get; set; }

        #endregion

        #region Propriedades

        [PersistenceProperty("IdRetalhoProducao", PersistenceParameterType.IdentityKey)]
        public int IdRetalhoProducao { get; set; }

        [PersistenceProperty("IdProdPedProducaoOrig")]
        public uint? IdProdPedProducaoOrig { get; set; }

        //[PersistenceProperty("IdProdPedProducaoNovo")]
        //public uint? IdProdPedProducaoNovo { get; set; }

        [PersistenceProperty("IdProdNf")]
        public uint? IdProdNf { get; set; }

        [PersistenceProperty("IdProd")]
        [PersistenceForeignKey(typeof(Produto), nameof(Produto.IdProd))]
        public int IdProd { get; set; }

        [PersistenceProperty("Situacao")]
        public SituacaoRetalhoProducao Situacao { get; set; }

        [Log("Data de Criação")]
        [PersistenceProperty("DataCad")]
        public DateTime DataCad { get; set; }

        [Log("Funcionário Criação", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("UsuCad")]
        public uint UsuCad { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("Descricao", DirectionParameter.InputOptional)]
        public string Descricao { get; set; }

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("Largura", DirectionParameter.InputOptional)]
        public int Largura { get; set; }

        [PersistenceProperty("Altura", DirectionParameter.InputOptional)]
        public int Altura { get; set; }

        [Log("Etiqueta que está usando")]
        [PersistenceProperty("EtiquetaUsando", DirectionParameter.InputOptional)]
        public string EtiquetaUsando { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("TotMUsando", DirectionParameter.InputOptional)]
        public double TotMUsando { get; set; }

        [PersistenceProperty("DataUso", DirectionParameter.InputOptional)]
        public DateTime? DataUso { get; set; }

        [PersistenceProperty("NumeroNFe", DirectionParameter.InputOptional)]
        public int NumeroNFe { get; set; }

        [PersistenceProperty("Lote", DirectionParameter.InputOptional)]
        public string Lote { get; set; }

        [PersistenceProperty("Obs", DirectionParameter.InputOptional)]
        public string Obs { get; set; }

        [PersistenceProperty("Usuario", DirectionParameter.InputOptional)]
        public LoginUsuario Usuario { get; set; }

        [PersistenceProperty("NomeFunc", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get{ return BibliotecaTexto.GetTwoFirstNames(_nomeFunc); }
            set { _nomeFunc = value; }
        }

        [PersistenceProperty("Espessura", DirectionParameter.InputOptional)]
        public float Espessura { get; set; }

        [PersistenceProperty("CorVidro", DirectionParameter.InputOptional)]
        public string CorVidro { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Situação")]
        public string SituacaoString
        {
            get
            {
                switch (Situacao)
                {
                    case SituacaoRetalhoProducao.Disponivel: return "Disponível";
                    case SituacaoRetalhoProducao.EmUso: return "Em uso";
                    case SituacaoRetalhoProducao.Cancelado: return "Cancelado";
                    case SituacaoRetalhoProducao.EmEstoque: return "Em estoque";
                    case SituacaoRetalhoProducao.Vendido: return "Vendido";
                    case SituacaoRetalhoProducao.Perda: return "Perda";
                }

                return "";
            }
        }

        [Log("Descrição")]
        public string DescricaoRetalho
        {
            get { return CodInterno + " - " + Descricao + " " + Largura + "x" + Altura; }
        }

        private string _numEtiqueta;

        [Log("Número Etiqueta")]
        public string NumeroEtiqueta
        {
            get
            {
                if (_numEtiqueta == null)
                    _numEtiqueta = RetalhoProducaoDAO.Instance.ObtemNumeroEtiqueta((uint)IdRetalhoProducao);

                return _numEtiqueta;
            }
        }

        public string DescricaoRetalhoComEtiqueta
        {
            get
            {
                return (!String.IsNullOrEmpty(NumeroEtiqueta) ? "<b>Etiqueta: " + NumeroEtiqueta +
                    "</b> - " : "") + DescricaoRetalho + String.Format(" ({0:0.##} m²)", TotM);
            }
        }

        private float? _totM = null;

        public float TotM
        {
            get
            {
                if (_totM == null)
                    _totM = Glass.Global.CalculosFluxo.ArredondaM2(Largura, Altura, 1, IdProd, false, 0, false);

                return _totM.GetValueOrDefault();
            }
        }

        public bool DentroFolga { get; set; }

        private bool? _cancelarVisible = null;

        public bool CancelarVisible
        {
            get
            {
                if (_cancelarVisible == null)
                    _cancelarVisible = RetalhoProducaoDAO.Instance.PodeCancelar(null, (uint)IdRetalhoProducao);

                return _cancelarVisible.GetValueOrDefault();
            }
        }

        public bool PerdaVisible
        {
            get
            {
                return Situacao != SituacaoRetalhoProducao.Cancelado && Situacao != SituacaoRetalhoProducao.Vendido && Situacao != SituacaoRetalhoProducao.Perda;
            }
        }

        #endregion
    }
}