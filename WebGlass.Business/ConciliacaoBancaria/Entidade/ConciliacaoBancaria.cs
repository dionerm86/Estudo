using System;
using Glass.Data.DAL;

namespace WebGlass.Business.ConciliacaoBancaria.Entidade
{
    public class ConciliacaoBancaria
    {
        internal Glass.Data.Model.ConciliacaoBancaria _conciliacao;
        private string _descricaoContaBancaria, _nomeFuncionarioCadastro;

        public enum SituacaoEnum
        {
            Ativa = Glass.Data.Model.ConciliacaoBancaria.SituacaoEnum.Ativa,
            Cancelada = Glass.Data.Model.ConciliacaoBancaria.SituacaoEnum.Cancelada
        }

        #region Construtores

        public ConciliacaoBancaria()
            : this(new Glass.Data.Model.ConciliacaoBancaria())
        {
        }

        internal ConciliacaoBancaria(Glass.Data.Model.ConciliacaoBancaria model)
        {
            _conciliacao = model;
        }

        #endregion

        #region Propriedades

        public uint Codigo
        {
            get { return _conciliacao.IdConciliacaoBancaria; }
            set { _conciliacao.IdConciliacaoBancaria = value; }
        }

        public uint CodigoContaBancaria
        {
            get { return _conciliacao.IdContaBanco; }
            set
            {
                _conciliacao.IdContaBanco = value;
                _descricaoContaBancaria = null;
            }
        }

        public string DescricaoContaBancaria
        {
            get
            {
                if (_descricaoContaBancaria == null)
                    _descricaoContaBancaria = ContaBancoDAO.Instance.GetDescricao(CodigoContaBancaria);

                return _descricaoContaBancaria;
            }
        }

        public DateTime DataConciliada
        {
            get { return _conciliacao.DataConciliada; }
            set { _conciliacao.DataConciliada = value; }
        }

        public DateTime DataCadastro
        {
            get { return _conciliacao.DataCad; }
            set { _conciliacao.DataCad = value; }
        }

        public uint CodigoFuncionarioCadastro
        {
            get { return _conciliacao.Usucad; }
            set
            {
                _conciliacao.Usucad = value;
                _nomeFuncionarioCadastro = null;
            }
        }

        public string NomeFuncionarioCadastro
        {
            get
            {
                if (_nomeFuncionarioCadastro == null)
                    _nomeFuncionarioCadastro = FuncionarioDAO.Instance.GetNome(CodigoFuncionarioCadastro);

                return _nomeFuncionarioCadastro;
            }
        }

        public SituacaoEnum Situacao
        {
            get { return (SituacaoEnum)_conciliacao.Situacao; }
            set { _conciliacao.Situacao = (int)value; }
        }

        public string DescricaoSituacao
        {
            get { return _conciliacao.DescricaoSituacao; }
        }

        private bool? _ultimaConciliacao;

        public bool PodeCancelar
        {
            get
            {
                if (Situacao == SituacaoEnum.Ativa)
                {
                    if (_ultimaConciliacao == null)
                        _ultimaConciliacao = DataConciliada == ConciliacaoBancariaDAO.Instance.
                            ObtemDataUltimaConciliacao(CodigoContaBancaria).GetValueOrDefault();

                    return _ultimaConciliacao.GetValueOrDefault();
                }
                else
                    return false;
            }
        }

        public bool ExibirExportarExel
        {
            get
            {
                return Situacao == SituacaoEnum.Ativa;
            }
        }

        #endregion
    }
}
