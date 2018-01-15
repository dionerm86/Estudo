using System;
using GDA;
using Glass.Data.RelDAL;
using Glass.Data.DAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PedidoConferidoDAO))]
    public class PedidoConferido
    {
        #region Enumeradores

        public enum SituacaoEsp : int
        {
            Aberto = 1,
            Finalizado,
            Impresso,
            ImpressoComum
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        private string _nomeCliente;

        [PersistenceProperty("NOMECLIENTE")]
        public string NomeCliente
        {
            get { return BibliotecaTexto.GetThreeFirstWords(_nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty); }
            set { _nomeCliente = value; }
        }

        private string _conferente;

        [PersistenceProperty("CONFERENTE")]
        public string Conferente
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_conferente); }
            set { _conferente = value; }
        }

        [PersistenceProperty("SITUACAOESPELHO")]
        public int SituacaoEspelho { get; set; }

        [PersistenceProperty("TOTALPEDIDOORIGINAL")]
        public decimal TotalPedidoOriginal { get; set; }

        [PersistenceProperty("TOTALPEDIDOCONFERIDO")]
        public decimal TotalPedidoConferido { get; set; }

        [PersistenceProperty("DATAESPELHO")]
        public DateTime DataEspelho { get; set; }

        [PersistenceProperty("GEROUEXCEDENTE")]
        public string GerouExcedente { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get
            {
                switch (SituacaoEspelho)
                {
                    case (int)SituacaoEsp.Aberto:
                        return "Aberto";
                    case (int)SituacaoEsp.Finalizado:
                        return "Finalizado";
                    case (int)SituacaoEsp.Impresso:
                        return "Impresso";
                    case (int)SituacaoEsp.ImpressoComum:
                        return "Impresso Comum";
                    default:
                        return String.Empty;
                }
            }
        }

        public decimal Diferenca
        {
            get { return TotalPedidoConferido - TotalPedidoOriginal; }
        }

        public bool ExibirRelatorioPedido
        {
            get { return PedidoDAO.ExibirRelatorioPedido(IdPedido); }
        }

        #endregion
    }
}