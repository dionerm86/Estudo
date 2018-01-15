using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Pedido.Entidade
{
    public class PedidoFinalizarFinanceiro
    {
        private Glass.Data.Model.Pedido _pedido;
        private string _nomeFuncionario;

        #region Construtores

        public PedidoFinalizarFinanceiro()
            : this(new Glass.Data.Model.Pedido())
        {
        }

        internal PedidoFinalizarFinanceiro(Glass.Data.Model.Pedido model)
        {
            _pedido = model;
        }

        #endregion

        #region Propriedades

        public uint Codigo
        {
            get { return _pedido.IdPedido; }
        }

        public string CodigoExibir
        {
            get { return _pedido.IdPedidoExibir; }
        }

        public string PedidoCli
        {
            get { return _pedido.CodCliente; }
        }

        public string NomeCliente
        {
            get { return _pedido.NomeCliente; }
        }

        public string NomeLoja
        {
            get { return _pedido.NomeLoja; }
        }

        public string NomeFuncionario
        {
            get
            {
                if (_nomeFuncionario == null)
                {
                    _nomeFuncionario = FuncionarioDAO.Instance.GetNome(_pedido.IdFunc);

                    if (_pedido.IdFuncCliente > 0)
                        _nomeFuncionario = FuncionarioDAO.Instance.GetNome(_pedido.IdFuncCliente.Value) +
                            String.Format(" (Cad.: {0})", _nomeFuncionario);
                }

                return _nomeFuncionario;
            }
        }

        public decimal Total
        {
            get { return _pedido.Total; }
        }

        public string TipoVenda
        {
            get { return _pedido.DescrTipoVenda; }
        }

        public DateTime DataPedido
        {
            get { return _pedido.DataPedido; }
        }

        public DateTime? DataEntrega
        {
            get { return _pedido.DataEntrega; }
        }

        public Glass.Data.Model.Pedido.SituacaoPedido Situacao
        {
            get { return _pedido.Situacao; }
        }

        public string DescricaoSituacao
        {
            get { return _pedido.DescrSituacaoPedido; }
        }

        public string TipoPedido
        {
            get { return _pedido.DescricaoTipoPedido; }
        }

        public string MotivoFinanceiro
        {
            get
            {
                return Finalizar ? _pedido.MotivoErroFinalizarFinanc :
                    Confirmar ? _pedido.MotivoErroConfirmarFinanc : null;
            }
        }

        public System.Drawing.Color CorLinhaLista
        {
            get { return _pedido.CorLinhaLista; }
        }

        public bool Finalizar
        {
            get { return _pedido.Situacao == Glass.Data.Model.Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro; }
        }

        public bool Confirmar
        {
            get { return _pedido.Situacao == Glass.Data.Model.Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro; }
        }

        public bool UsarControleReposicao
        {
            get { return _pedido.UsarControleReposicao; }
        }

        public bool ExibirRelatorio
        {
            get { return _pedido.ExibirRelatorio; }
        }

        public bool ExibirRelatorioCalculo
        {
            get { return _pedido.ExibirRelatorioCalculo; }
        }

        public bool ExibirNotaPromissoria
        {
            get { return _pedido.ExibirNotaPromissoria; }
        }

        public bool ExibirImpressaoProjeto
        {
            get { return _pedido.ExibirImpressaoProjeto; }
        }

        #endregion
    }
}
