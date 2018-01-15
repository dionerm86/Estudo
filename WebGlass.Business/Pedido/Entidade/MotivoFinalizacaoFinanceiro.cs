using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Pedido.Entidade
{
    public class MotivoFinalizacaoFinanceiro
    {
        private Glass.Data.Model.ObservacaoFinalizacaoFinanceiro _observacao;
        private string _nomeFuncionarioCadastro;

        #region Construtores

        public MotivoFinalizacaoFinanceiro()
            : this(new Glass.Data.Model.ObservacaoFinalizacaoFinanceiro())
        {
        }

        internal MotivoFinalizacaoFinanceiro(Glass.Data.Model.ObservacaoFinalizacaoFinanceiro model)
        {
            _observacao = model;
        }

        #endregion

        #region Propriedades

        public uint CodigoPedido
        {
            get { return _observacao.IdPedido; }
        }

        public string Motivo
        {
            get { return _observacao.DescrMotivo; }
        }

        public string Observacao
        {
            get { return _observacao.Observacao; }
        }

        public string NomeFuncionarioCadastro
        {
            get
            {
                if (_nomeFuncionarioCadastro == null)
                    _nomeFuncionarioCadastro = FuncionarioDAO.Instance.GetNome(_observacao.IdFuncCad);

                return _nomeFuncionarioCadastro;
            }
        }

        public DateTime DataCadastro
        {
            get { return _observacao.DataCad; }
        }

        public System.Drawing.Color? CorLinhaLista
        {
            get
            {
                switch (_observacao.Motivo)
                {
                    case Glass.Data.Model.ObservacaoFinalizacaoFinanceiro.MotivoEnum.NegacaoFinalizar:
                    case Glass.Data.Model.ObservacaoFinalizacaoFinanceiro.MotivoEnum.NegacaoConfirmar:
                        return System.Drawing.Color.Red;

                    default:
                        return null;
                }
            }
        }

        public string MotivoFinanceiro
        {
            get { return _observacao.MotivoFinanceiro; }
        }

        public string IdNomeCliente { get { return _observacao.IdNomeCliente; } }

        #endregion
    }
}
