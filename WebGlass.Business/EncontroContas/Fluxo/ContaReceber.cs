using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.EncontroContas.Fluxo
{
    public sealed class ContaReceber : BaseFluxo<ContaReceber>
    {
        private ContaReceber() { }

        #region Ajax

        private static Ajax.IContaReceber _ajax = null;

        public static Ajax.IContaReceber Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.ContaReceber();

                return _ajax;
            }
        }

        #endregion

        /// <summary>
        /// Adiciona uma conta recebida ao encontro de contas
        /// </summary>
        /// <param name="idContaR"></param>
        public void AddContaR(uint idEncontroContas, uint idContaR)
        {
            if (idEncontroContas == 0)
                throw new Exception("Informe o encontro de contas a pagar/receber.");

            if (idContaR == 0)
                throw new Exception("Informe a conta a receber.");

            if (TemVinculo(idEncontroContas, idContaR))
                throw new Exception("Esta conta a receber já foi adicionada.");

            string tipoConta;
            if (!MesmoTipoConta(idEncontroContas, idContaR, out tipoConta))
                throw new Exception("Apenas contas do tipo '" + tipoConta + "' são permitidas nesse encontro de contas.");

            ContasReceberEncontroContas novo = new ContasReceberEncontroContas();
            novo.IdEncontroContas = idEncontroContas;
            novo.IdContaR = idContaR;

            ContasReceberEncontroContasDAO.Instance.Insert(novo);
        }

        /// <summary>
        /// Verifica se a conta a receber possuiu vinculo com o encontro
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public bool TemVinculo(uint idEncontroContas, uint idContaR)
        {
            try
            {
                return ContasReceberEncontroContasDAO.Instance.TemVinculo(idEncontroContas, idContaR);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool MesmoTipoConta(uint idEncontroContas, uint idContaR, out string tipoConta)
        {
            tipoConta = EncontroContasDAO.Instance.ObtemTipoConta(null, idEncontroContas);
            if (String.IsNullOrEmpty(tipoConta))
                return true;

            return ContasReceberDAO.Instance.ObtemTipoConta(idContaR) == tipoConta;
        }
    }
}
