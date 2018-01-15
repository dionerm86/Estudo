using System.Collections.Generic;
using WebGlass.Business.ConhecimentoTransporte.Fluxo;

namespace WebGlass.Business.ConhecimentoTransporte
{
    /// <summary>
    /// Classe de métodos utilizados no data source
    /// </summary>
    public class NotaFiscalCteOds : BaseFluxo<NotaFiscalCteOds>
    {
        private NotaFiscalCteOds() { }

        public IEnumerable<Entidade.NfeCte> GetList(uint idCte, string sortExpression, int startRow, int pageSize)
        {
            if (idCte == 0)
                return new List<Entidade.NfeCte>();

            return BuscarNotaFiscalCte.Instance.GetList(idCte, sortExpression, startRow, pageSize);
        }

        public Entidade.NfeCte GetNfeCte(uint idCte, uint idNf)
        {
            return BuscarNotaFiscalCte.Instance.GetNfeCte(idCte, idNf);
        }

        public int GetCount()
        {
            return BuscarNotaFiscalCte.Instance.GetCount();
        }

        public uint Insert(Entidade.NfeCte nfeCte)
        {
            return CadastrarNotaFiscalCte.Instance.Insert(nfeCte);
        }

        public int Delete(WebGlass.Business.ConhecimentoTransporte.Entidade.NfeCte nfeCte)
        {
            return DeletarNotaFiscalCte.Instance.Delete(nfeCte);
        }
    }
}
