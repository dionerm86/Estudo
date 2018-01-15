using System;
using Glass.Data.DAL;

namespace WebGlass.Business.EncontroContas.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        #region Ajax

        private static Ajax.IBuscarEValidar _ajax = null;

        public static Ajax.IBuscarEValidar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.BuscarEValidar();

                return _ajax;
            }
        }

        #endregion

        /// <summary>
        /// Valida se o cliente e o fornecedor são os mesmos.
        /// </summary>
        /// <param name="obj"></param>
        public void ValidaClienteFornecedor(uint idCli, uint idFornec)
        {
            if (idCli == 0)
                throw new Exception("Informe o cliente.");
            if (idFornec == 0)
                throw new Exception("Informe o fornecedor.");

            Glass.Data.Model.Cliente cliente = ClienteDAO.Instance.GetElement(idCli);
            Glass.Data.Model.Fornecedor fornecedor = FornecedorDAO.Instance.GetElement(idFornec);

            if (!cliente.TipoPessoa.ToUpper().Equals(fornecedor.TipoPessoa.ToUpper()))
                throw new Exception("Cliente e fornecedor não possuem o mesmo tipo de pessoa (Física/Jurídica)");

            if (!cliente.CpfCnpj.ToUpper().Equals(fornecedor.CpfCnpj.ToUpper()))
                throw new Exception("Cliente e fornecedor não possuem o mesmo " + (cliente.TipoPessoa.ToUpper().Equals("F") ? "CPF" : "CNPJ"));
        }

        /// <summary>
        /// Valida se o cliente e o fornecedor são os mesmos.
        /// </summary>
        /// <param name="idEncontroContas"></param>
        public void ValidaClienteFornecedor(uint idEncontroContas)
        {
            Glass.Data.Model.EncontroContas ec = EncontroContasDAO.Instance.GetElement(idEncontroContas);
            ValidaClienteFornecedor(ec.IdCliente, ec.IdFornecedor);
        }

        /// <summary>
        /// Verifica se alguma conta do encontro ja foi paga
        /// </summary>
        /// <param name="idEncontroContas"></param>
        public void ValidaContasPagarReceber(uint idEncontroContas)
        {
            if (idEncontroContas == 0)
                throw new Exception("Informe o encontro de contas a pagar/receber.");

            string pagar = ContasPagarEncontroContasDAO.Instance.ValidaContasPagar(null, idEncontroContas);
            if (!string.IsNullOrEmpty(pagar))
                throw new Exception("As contas a pagar: " + pagar + " já foram pagas.");

            string receber = ContasReceberEncontroContasDAO.Instance.ValidaContasReceber(null, idEncontroContas);
            if (!string.IsNullOrEmpty(receber))
                throw new Exception("As contas a receber: " + receber + " já foram recebidas.");
        }

        /// <summary>
        /// Valida um encontro para a finalização do mesmo
        /// </summary>
        /// <param name="idEncontroContas"></param>
        public void ValidaEncontro(uint idEncontroContas, DateTime? dtVenc)
        {
            if (!dtVenc.HasValue)
                throw new Exception("Informe da data de vencimento.");

            var situacao = EncontroContasDAO.Instance.ObtemSituacao(idEncontroContas);

            if (situacao != Glass.Data.Model.EncontroContas.SituacaoEncontroContas.Aberto)
                throw new Exception("Este encontro de contas a pagar/receber está " + EncontroContasDAO.Instance.ObtemSituacaoStr((int)situacao) + ".");

            ValidaClienteFornecedor(idEncontroContas);
            ValidaContasPagarReceber(idEncontroContas);
        }

        /// <summary>
        /// Valida um encontro para o cancelamento do mesmo
        /// </summary>
        /// <param name="idEncontroContas"></param>
        public void ValidaEncontroCancelamento(uint idEncontroContas)
        {
            if (EncontroContasDAO.Instance.TemContaPagaRecebida(idEncontroContas))
                throw new Exception("Este encontro de contas a pagar/receber já possui uma conta a " +
                    (EncontroContasDAO.Instance.ObtemTipoContaGerar(idEncontroContas) == 1 ? "paga" : "recebida") + ".");
        }

        /// <summary>
        /// Valida um encontro para retificar o mesmo
        /// </summary>
        /// <param name="idEncontroContas"></param>
        public void ValidaEncontroRetificar(uint idEncontroContas, string contasPg, string contasR, DateTime? dtVenc)
        {
            if (!dtVenc.HasValue)
                throw new Exception("Informe da data de vencimento.");

            var situacao = EncontroContasDAO.Instance.ObtemSituacao(idEncontroContas);

            if (situacao != Glass.Data.Model.EncontroContas.SituacaoEncontroContas.Finalizado)
                throw new Exception("Este encontro de contas a pagar/receber está " + EncontroContasDAO.Instance.ObtemSituacaoStr((int)situacao) + ".");

            if ((contasPg == null || String.IsNullOrEmpty((contasPg = contasPg.Trim(',', ' '))))
                && (contasR == null || String.IsNullOrEmpty((contasR = contasR.Trim(',', ' ')))))
                throw new Exception("Nenhuma conta a pagar/receber a ser removida.");

            if (EncontroContasDAO.Instance.TemContaPagaRecebida(idEncontroContas))
                throw new Exception("Este encontro de contas a pagar/receber já possui uma conta a " +
                    (EncontroContasDAO.Instance.ObtemTipoContaGerar(idEncontroContas) == 1 ? "paga" : "recebida") + ".");
        }
    }
}
