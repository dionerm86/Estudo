using Glass.Data.DAL;

namespace WebGlass.Business.Loja.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public string ObtemNomeLoja(uint codigoLoja)
        {
            return LojaDAO.Instance.GetNome(codigoLoja);
        }
    }
}
