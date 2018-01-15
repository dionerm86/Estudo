using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.EncontroContas.Fluxo
{
    public sealed class ContaPagar: BaseFluxo<ContaPagar>
    {
        private ContaPagar() { }

        #region Ajax

        private static Ajax.IContaPagar _ajax = null;

        public static Ajax.IContaPagar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.ContaPagar();

                return _ajax;
            }
        }

        #endregion

        /// <summary>
        /// Adiciona uma conta a pagar ao encontro de contas
        /// </summary>
        /// <param name="idContaPg"></param>
        public void AddContaPg(uint idEncontroContas, uint idContaPg)
        {
            if (idEncontroContas == 0)
                throw new Exception("Informe o encontro de contas a pagar/receber.");

            if (idContaPg == 0)
                throw new Exception("Informe a conta a pagar.");

            if (TemVinculo(idEncontroContas, idContaPg))
                throw new Exception("Esta conta a pagar já foi adicionada.");

            string tipoConta;
            if (FinanceiroConfig.PermitirApenasContasMesmoTipoEncontroContas && !MesmoTipoConta(idEncontroContas, idContaPg, out tipoConta))
                throw new Exception("Apenas contas do tipo '" + tipoConta + "' são permitidas nesse encontro de contas.");

            ContasPagarEncontroContas novo = new ContasPagarEncontroContas();
            novo.IdEncontroContas = idEncontroContas;
            novo.IdContaPg = idContaPg;

            ContasPagarEncontroContasDAO.Instance.Insert(novo);
        }

        /// <summary>
        /// Verifica se a conta a pagar possuiu vinculo com o encontro
        /// </summary>
        /// <param name="idEncontroContas"></param>
        /// <param name="idContaPg"></param>
        /// <returns></returns>
        public bool TemVinculo(uint idEncontroContas, uint idContaPg)
        {
            return ContasPagarEncontroContasDAO.Instance.TemVinculo(idEncontroContas, idContaPg);
        }

        public bool MesmoTipoConta(uint idEncontroContas, uint idContaPg, out string tipoConta)
        {
            tipoConta = EncontroContasDAO.Instance.ObtemTipoConta(null, idEncontroContas);
            if (string.IsNullOrEmpty(tipoConta))
                return true;

            return ContasPagarDAO.Instance.ObtemTipoConta(idContaPg) == tipoConta;
        }
    }
}
