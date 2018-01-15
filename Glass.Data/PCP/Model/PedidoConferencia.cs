using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoConferenciaDAO))]
    [PersistenceClass("pedido_conferencia")]
    public class PedidoConferencia : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoConferencia : int
        {
            Aberta=1,
            EmAndamento,
            Finalizada
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPEDIDO", PersistenceParameterType.Key)]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDCONFERENTE")]
        public uint? IdConferente { get; set; }

        [PersistenceProperty("DATAINI")]
        public DateTime? DataIni { get; set; }

        [PersistenceProperty("DATAFIM")]
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// 1-Aberta
        /// 2-EmAndamento
        /// 3-Finalizada
        /// 4-Cancelada
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeCliente;

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente
        {
            get { return _nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty; }
            set { _nomeCliente = value; }
        }

        public string NomeInicialCli
        {
            get { return BibliotecaTexto.GetThreeFirstWords(_nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty); }
        }

        private string _conferente;

        [PersistenceProperty("Conferente", DirectionParameter.InputOptional)]
        public string Conferente
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_conferente != null ? _conferente.ToUpper() : String.Empty); }
            set { _conferente = value; }
        }

        private string _vendedor;

        [PersistenceProperty("Vendedor", DirectionParameter.InputOptional)]
        public string Vendedor
        {
            get { return BibliotecaTexto.GetFirstName(_vendedor != null ? _vendedor.ToUpper() : String.Empty); }
            set { _vendedor = value; }
        }

        private string _nomeLoja;

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja
        {
            get { return _nomeLoja != null ? _nomeLoja.ToUpper() : String.Empty; }
            set { _nomeLoja = value; }
        }

        [PersistenceProperty("DataEntrega", DirectionParameter.InputOptional)]
        public DateTime? DataEntrega { get; set; }

        private string _telCli;

        [PersistenceProperty("TelCli", DirectionParameter.InputOptional)]
        public string TelCli
        {
            get { return _telCli != null ? _telCli : String.Empty; }
            set { _telCli = value; }
        }

        private string _localObra;

        [PersistenceProperty("LocalObra", DirectionParameter.InputOptional)]
        public string LocalObra
        {
            get { return _localObra != null ? _localObra : String.Empty; }
            set { _localObra = value; }
        }

        #endregion

        #region Propriedades de Suporte

        [PersistenceProperty("criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case 1:
                        return "Aberta";
                    case 2:
                        return "Em Andamento";
                    case 3:
                        return "Finalizada";
                    case 4:
                        return "Cancelada";
                    default:
                        return String.Empty;
                }
            }
        }

        #endregion
    }
}