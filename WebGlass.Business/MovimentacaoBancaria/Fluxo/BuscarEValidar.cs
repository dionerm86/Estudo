using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.MovimentacaoBancaria.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public IList<Entidade.MovimentacaoBancaria> ObtemMovimentacoesParaConciliacao(uint codigoContaBanco, DateTime dataTermino)
        {
            var mov = MovBancoDAO.Instance.ObtemMovimentacoesParaConciliacao(codigoContaBanco, dataTermino.ToString("dd/MM/yyyy"));
            return mov.Select(x => new Entidade.MovimentacaoBancaria(x)).ToList();
        }

        public IList<Entidade.MovimentacaoBancaria> ObtemMovimentacoesDaConciliacao(uint codigoConciliacaoBancaria)
        {
            var mov = MovBancoDAO.Instance.ObtemMovimentacoesDaConciliacao(codigoConciliacaoBancaria);
            return mov.Select(x => new Entidade.MovimentacaoBancaria(x)).ToList();
        }
    }
}
