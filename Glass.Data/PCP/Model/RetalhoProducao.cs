using System;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(RetalhoProducaoDAO))]
    [PersistenceClass("retalho_producao")]
    public class RetalhoProducao
    {
        #region Enumeradores

        public enum SituacaoRetalho : int
        {
            Disponivel = 1,
            Cancelado,
            EmUso,
            EmEstoque,
            Vendido,
            Perda
        }

        #endregion

        #region Variaveis Locais

        private string _nomeFunc { get; set; }

        #endregion

        #region Propriedades

        [PersistenceProperty("IdRetalhoProducao", PersistenceParameterType.IdentityKey)]
        public uint IdRetalhoProducao { get; set; }

        [PersistenceProperty("IdProdPedProducaoOrig")]
        public uint? IdProdPedProducaoOrig { get; set; }

        //[PersistenceProperty("IdProdPedProducaoNovo")]
        //public uint? IdProdPedProducaoNovo { get; set; }

        [PersistenceProperty("IdProdNf")]
        public uint? IdProdNf { get; set; }

        [PersistenceProperty("IdProd")]
        public uint IdProd { get; set; }

        [PersistenceProperty("Situacao")]
        public SituacaoRetalho Situacao { get; set; }

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
                    case SituacaoRetalho.Disponivel: return "Disponível";
                    case SituacaoRetalho.EmUso: return "Em uso";
                    case SituacaoRetalho.Cancelado: return "Cancelado";
                    case SituacaoRetalho.EmEstoque: return "Em estoque";
                    case SituacaoRetalho.Vendido: return "Vendido";
                    case SituacaoRetalho.Perda: return "Perda";
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
                    _numEtiqueta = RetalhoProducaoDAO.Instance.ObtemNumeroEtiqueta(IdRetalhoProducao);

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
                    _totM = Glass.Global.CalculosFluxo.ArredondaM2(Largura, Altura, 1, (int)IdProd, false, 0, false);

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
                    _cancelarVisible = RetalhoProducaoDAO.Instance.PodeCancelar(null, IdRetalhoProducao);

                return _cancelarVisible.GetValueOrDefault();
            }
        }

        public bool PerdaVisible
        {
            get
            {
                return Situacao != SituacaoRetalho.Cancelado && Situacao != SituacaoRetalho.Vendido && Situacao != SituacaoRetalho.Perda;
            }
        }

        #endregion
    }
}