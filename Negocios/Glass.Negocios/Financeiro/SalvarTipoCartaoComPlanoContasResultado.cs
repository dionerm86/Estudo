using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios
{
    public class SalvarTipoCartaoComPlanoContasResultado : Colosoft.Business.OperationResult
    {
        #region Variaveis Locais

        private Entidades.TipoCartaoCredito _tipoCartaoCredito;
        private Entidades.PlanoContas _planoContaDevolucaoPagto;
        private Entidades.PlanoContas _planoContaEntrada;
        private Entidades.PlanoContas _planoContaEstorno;
        private Entidades.PlanoContas _planoContaEstornoChequeDev;
        private Entidades.PlanoContas _planoContaEstornoDevolucaoPagto;
        private Entidades.PlanoContas _planoContaEstornoEntrada;
        private Entidades.PlanoContas _planoContaEstornoRecPrazo;
        private Entidades.PlanoContas _planoContaFunc;
        private Entidades.PlanoContas _planoContaRecChequeDev;
        private Entidades.PlanoContas _planoContaRecPrazo;
        private Entidades.PlanoContas _planoContaVista;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public SalvarTipoCartaoComPlanoContasResultado(Colosoft.IMessageFormattable mensagem)
            : base(false, mensagem)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public SalvarTipoCartaoComPlanoContasResultado(Entidades.TipoCartaoCredito tipoCartaoCredito,
            Entidades.PlanoContas planoContaDevolucaoPagto,
            Entidades.PlanoContas planoContaEntrada,
            Entidades.PlanoContas planoContaEstorno,
            Entidades.PlanoContas planoContaEstornoChequeDev,
            Entidades.PlanoContas planoContaEstornoDevolucaoPagto,
            Entidades.PlanoContas planoContaEstornoEntrada,
            Entidades.PlanoContas planoContaEstornoRecPrazo,
            Entidades.PlanoContas planoContaFunc,
            Entidades.PlanoContas planoContaRecChequeDev,
            Entidades.PlanoContas planoContaRecPrazo,
            Entidades.PlanoContas planoContaVista)
            : base(true, null)
        {
            _tipoCartaoCredito = tipoCartaoCredito;
            _planoContaDevolucaoPagto = planoContaDevolucaoPagto;
            _planoContaEntrada = planoContaEntrada;
            _planoContaEstorno = planoContaEstorno;
            _planoContaEstornoChequeDev = planoContaEstornoChequeDev;
            _planoContaEstornoDevolucaoPagto = planoContaEstornoDevolucaoPagto;
            _planoContaEstornoEntrada = planoContaEstornoEntrada;
            _planoContaEstornoRecPrazo = planoContaEstornoRecPrazo;
            _planoContaFunc = planoContaFunc;
            _planoContaRecChequeDev = planoContaRecChequeDev;
            _planoContaRecPrazo = planoContaRecPrazo;
            _planoContaVista = planoContaVista;
        }

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Salva os dados do resultado.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult Salvar(Colosoft.Data.IPersistenceSession session)
        {
            var resultadoFinal = new Colosoft.Business.SaveResult();

            if ((_planoContaDevolucaoPagto != null && !(resultadoFinal = _planoContaDevolucaoPagto.Save(session))) ||
                (_planoContaEntrada != null && !(resultadoFinal = _planoContaEntrada.Save(session))) ||
                (_planoContaEstorno != null && !(resultadoFinal = _planoContaEstorno.Save(session))) ||
                (_planoContaEstornoChequeDev != null && !(resultadoFinal = _planoContaEstornoChequeDev.Save(session))) ||
                (_planoContaEstornoDevolucaoPagto != null && !(resultadoFinal = _planoContaEstornoDevolucaoPagto.Save(session))) ||
                (_planoContaEstornoEntrada != null && !(resultadoFinal = _planoContaEstornoEntrada.Save(session))) ||
                (_planoContaEstornoRecPrazo != null && !(resultadoFinal = _planoContaEstornoRecPrazo.Save(session))) ||
                (_planoContaFunc != null && !(resultadoFinal = _planoContaFunc.Save(session))) ||
                (_planoContaRecChequeDev != null && !(resultadoFinal = _planoContaRecChequeDev.Save(session))) ||
                (_planoContaRecPrazo != null && !(resultadoFinal = _planoContaRecPrazo.Save(session))) ||
                (_planoContaVista != null && !(resultadoFinal = _planoContaVista.Save(session))) ||
                (_tipoCartaoCredito != null && !(resultadoFinal = _tipoCartaoCredito.Save(session))))
                return resultadoFinal;

            return new Colosoft.Business.SaveResult(true, null);
        }

        #endregion
    }
}
