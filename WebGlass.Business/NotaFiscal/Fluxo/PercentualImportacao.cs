using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGlass.Business.NotaFiscal.Fluxo
{
    public sealed class PercentualImportacao : BaseFluxo<PercentualImportacao>
    {
        private PercentualImportacao() { }

        #region Ajax

        private static Ajax.IPercentualImportacao _ajax = null;

        public static Ajax.IPercentualImportacao Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.PercentualImportacao();

                return _ajax;
            }
        }

        #endregion
    }
}
