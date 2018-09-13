using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Financeiro.Negocios.Componentes
{
    public class ChequesFluxo : IChequesFluxo
    {
        /// <summary>
        /// Método de geração de arquivo de exportação do cheque.
        /// </summary>
        /// <param name="idDeposito">id do depósito.</param>
        /// <returns>arquivo de exportação.</returns>
        public Entidades.Cheques.Arquivo GerarArquivoCheques(uint idDeposito)
        {
            var lstCheques = Data.DAL.ChequesDAO.Instance.GetByDeposito(idDeposito);

            if (lstCheques == null || !lstCheques.Any())
                throw new Exception("Nenhum cheque encontrado.");

            if (!lstCheques.Any(f => !string.IsNullOrEmpty(f.Cmc7)))
                throw new Exception("Nenhum cheque no depósito selecionado tem CMC7 cadastrado.");

            var itens = lstCheques.Where(f => !string.IsNullOrEmpty(f.Cmc7)).Select(f => new Entidades.Cheques.Item(f)).ToList();

            InfoFatura(ref itens);

            return new Entidades.Cheques.Arquivo(itens);
        }

        /// <summary>
        /// Preenche as informações da fatura ligada ao cheque
        /// </summary>
        /// <param name="itens">cheques</param>
        private void InfoFatura(ref List<Entidades.Cheques.Item> itens)
        {
            foreach (var item in itens)
            {
                var idNfRecebimento = Data.DAL.ChequesDAO.Instance.ObtemIdsNfRecebimento(item.IdLiberarPedido, item.IdPedido, item.IdAcerto);

                if (string.IsNullOrEmpty(idNfRecebimento))
                    continue;

                var notaFiscal = Data.DAL.NotaFiscalDAO.Instance.GetElement(idNfRecebimento.Split(',')[0].StrParaUint());

                item.Fatura = notaFiscal.NumeroNFe;
                item.ValorFatura = notaFiscal.TotalNota;
                item.ChaveDanfe = notaFiscal.ChaveAcesso;
                item.SerieFatura = notaFiscal.Serie;
            }

        }
    }
}
