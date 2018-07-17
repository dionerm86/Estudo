using System;
using GDA;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TrocaDevolucaoDAO))]
    [PersistenceClass("troca_devolucao")]
    public class TrocaDevolucao
    {
        #region Enumeradores

        public enum TipoTrocaDev
        {
            Troca = 1,
            Devolucao
        }

        public enum SituacaoTrocaDev
        {
            Aberta = 1,
            Finalizada,
            Cancelada
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDTROCADEVOLUCAO", PersistenceParameterType.IdentityKey)]
        public uint IdTrocaDevolucao { get; set; }

        [Log("Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("IDFUNCCANC")]
        public uint? IdFuncCanc { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [Log("Tipo Perda", "Descricao", typeof(TipoPerdaDAO))]
        [PersistenceProperty("IDTIPOPERDA")]
        public uint? IdTipoPerda { get; set; }

        [Log("Subtipo Perda", "Descricao", typeof(SubtipoPerdaDAO))]
        [PersistenceProperty("IDSUBTIPOPERDA")]
        public uint? IdSubtipoPerda { get; set; }

        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Data da Troca")]
        [PersistenceProperty("DATATROCA")]
        public DateTime DataTroca { get; set; }

        [PersistenceProperty("VALOREXCEDENTE")]
        public decimal ValorExcedente { get; set; }

        [Log("Crédito Gerado")]
        [PersistenceProperty("CREDITOGERADO")]
        public decimal CreditoGerado { get; set; }

        [Log("Crédito Máx.")]
        [PersistenceProperty("CREDITOGERADOMAX")]
        public decimal CreditoGeradoMax { get; set; }

        [PersistenceProperty("VALORCREDITOAOFINALIZAR")]
        public decimal? ValorCreditoAoFinalizar { get; set; }

        [PersistenceProperty("CREDITOGERADOFINALIZAR")]
        public decimal? CreditoGeradoFinalizar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADOFINALIZAR")]
        public decimal? CreditoUtilizadoFinalizar { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Usar Pedidos Reposição")]
        [PersistenceProperty("USARPEDIDOREPOSICAO")]
        public bool UsarPedidoReposicao { get; set; }

        [PersistenceProperty("IDORIGEMTROCADEVOLUCAO")]
        public uint? IdOrigemTrocaDevolucao { get; set; }

        [PersistenceProperty("IdSetor")]
        public int? IdSetor { get; set; }

        [PersistenceProperty("DataErro")]
        public DateTime? DataErro { get; set; }

        [PersistenceProperty("UsuCad")]
        public int UsuCad { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("IDLOJA", DirectionParameter.InputOptional)]
        public ulong IdLoja { get; set; }

        [PersistenceProperty("IDFUNCIONARIOASSOCIADOCLIENTE", DirectionParameter.InputOptional)]
        public uint IdFuncionarioAssociadoCliente { get; set; }

        private string _nomeFuncionarioAssociadoCliente;

        [PersistenceProperty("NOMEFUNCIONARIOASSOCIADOCLIENTE", DirectionParameter.InputOptional)]
        public string NomeFuncionarioAssociadoCliente
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFuncionarioAssociadoCliente); }
            set { _nomeFuncionarioAssociadoCliente = value; }
        }

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        private string _nomeFunc;

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeFunc); }
            set { _nomeFunc = value; }
        }

        [PersistenceProperty("NomeUsuCad", DirectionParameter.InputOptional)]
        public string NomeUsuCad { get; set; }

        [PersistenceProperty("CLIREVENDA", DirectionParameter.InputOptional)]
        public bool CliRevenda { get; set; }

        [PersistenceProperty("TIPOENTREGAPEDIDO", DirectionParameter.InputOptional)]
        public int TipoEntregaPedido { get; set; }

        [PersistenceProperty("CREDITOCLIENTE", DirectionParameter.InputOptional)]
        public decimal CreditoCliente { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("ENDERECOCLIENTE", DirectionParameter.InputOptional)]
        public string EnderecoCliente { get; set; }

        [PersistenceProperty("NUMEROCLIENTE", DirectionParameter.InputOptional)]
        public string NumeroCliente { get; set; }

        [PersistenceProperty("COMPLCLIENTE", DirectionParameter.InputOptional)]
        public string ComplCliente { get; set; }

        [PersistenceProperty("BAIRROCLIENTE", DirectionParameter.InputOptional)]
        public string BairroCliente { get; set; }

        [PersistenceProperty("CIDADECLIENTE", DirectionParameter.InputOptional)]
        public string CidadeCliente { get; set; }

        [PersistenceProperty("UFCLIENTE", DirectionParameter.InputOptional)]
        public string UfCliente { get; set; }

        [PersistenceProperty("CEPCLIENTE", DirectionParameter.InputOptional)]
        public string CepCliente { get; set; }

        private string _telContCliente;

        [PersistenceProperty("TELCONTCLIENTE", DirectionParameter.InputOptional)]
        public string TelContCliente
        {
            get { return !String.IsNullOrEmpty(_telContCliente) ? _telContCliente.Trim(' ', '/') : ""; }
            set { _telContCliente = value; }
        }

        [PersistenceProperty("DESCRORIGEMTROCADEVOLUCAO", DirectionParameter.InputOptional)]
        public string DescrOrigemTrocaDevolucao { get; set; }

        [PersistenceProperty("Setor", DirectionParameter.InputOptional)]
        public string Setor { get; set; }

        [PersistenceProperty("Loja", DirectionParameter.InputOptional)]
        public string Loja { get; set; }

        [PersistenceProperty("QtdePecas", DirectionParameter.InputOptional)]
        public Double QtdePecas { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Tipo")]
        public string DescrTipo
        {
            get
            {
                switch ((TipoTrocaDev)Tipo)
                {
                    case TipoTrocaDev.Devolucao: return "Devolução";
                    case TipoTrocaDev.Troca: return "Troca";
                    default: return "";
                }
            }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch ((SituacaoTrocaDev)Situacao)
                {
                    case SituacaoTrocaDev.Aberta: return "Aberta";
                    case SituacaoTrocaDev.Cancelada: return "Cancelada";
                    case SituacaoTrocaDev.Finalizada: return "Finalizada";
                    default: return "";
                }
            }
        }

        [Log("Tipo Valor")]
        public string DescrValor
        {
            get { return ValorExcedente > 0 ? "Valor Excedente" : "Crédito Gerado"; }
        }

        [Log("Valor")]
        public string Valor
        {
            get
            {
                return ValorExcedente > 0 ? ValorExcedente.ToString("C") : CreditoGerado.ToString("C") +
                    (CreditoGeradoMax != CreditoGerado ? " (máx. " + CreditoGeradoMax.ToString("C") + ")" : "");
            }
        }

        public Color CorValor
        {
            get { return ValorExcedente > 0 ? Color.Red : Color.Green; }
        }

        public bool EditEnabled
        {
            get { return Situacao == (int)SituacaoTrocaDev.Aberta && Config.PossuiPermissao(Config.FuncaoMenuEstoque.EfetuarTrocaDevolucao); }
        }

        public bool CancelEnabled
        {
            get { return Situacao != (int)SituacaoTrocaDev.Cancelada && Config.PossuiPermissao(Config.FuncaoMenuEstoque.EfetuarTrocaDevolucao); }
        }

        [Log("Movimentação Crédito")]
        public string MovimentacaoCredito
        {
            get
            {
                if (IdPedido > 0 && PedidoDAO.Instance.IsPedidoReposicao(null, IdPedido.ToString()) || 
                    PedidoDAO.Instance.IsPedidoGarantia(null, IdPedido.ToString()))
                    return String.Empty;

                decimal utilizado = CreditoUtilizadoFinalizar != null ? CreditoUtilizadoFinalizar.Value : 0;
                decimal gerado = CreditoGerado + (CreditoGeradoFinalizar != null ? CreditoGeradoFinalizar.Value : 0);

                if (ValorCreditoAoFinalizar == null || (ValorCreditoAoFinalizar == 0 && (utilizado + gerado) == 0))
                    return "";

                return "Crédito inicial: " + ValorCreditoAoFinalizar.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoFinalizar.Value - utilizado + gerado).ToString("C");
            }
        }

        public string ObsFuncCanc
        {
            get { return Obs + (IdFuncCanc > 0 ? " Cancelada por " + FuncionarioDAO.Instance.GetNome(IdFuncCanc.Value) : ""); }
        }

        public bool ExibirUsarPedidoReposicao
        {
            get
            {
                return IdPedido > 0 &&
                    PedidoDAO.Instance.IsPedidoReposto(null, IdPedido.Value) &&
                    PedidoReposicaoDAO.Instance.PedidoParaTroca(PedidoDAO.Instance.IdReposicao(IdPedido.Value).GetValueOrDefault());
            }
        }

        public bool EditarPedido
        {
            get { return ProdutoTrocadoDAO.Instance.GetCountReal(IdTrocaDevolucao) == 0; }
        }

        public bool EditarUsarPedidoReposicao
        {
            get { return ProdutoTrocaDevolucaoDAO.Instance.GetCountReal(IdTrocaDevolucao) == 0; }
        }

        public bool PermitirAlterarCreditoGerado
        {
            get
            {
                return Configuracoes.EstoqueConfig.PermitirAlterarCreditoGerado && ValorExcedente == 0;
            }
        }

        #endregion
    }
}