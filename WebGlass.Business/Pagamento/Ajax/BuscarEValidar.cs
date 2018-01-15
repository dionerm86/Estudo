using Glass.Data.DAL;

namespace WebGlass.Business.Pagamento.Ajax
{
    public interface IBuscarEValidar
    {
        string ValidarPagamento(string idPagtoStr);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string ValidarPagamento(string idPagtoStr)
        {
            uint idPagto = Glass.Conversoes.StrParaUint(idPagtoStr);
            if (!PagtoDAO.Instance.Exists(idPagto))
                return "Erro#Pagamento não existe.";

            if (PagtoDAO.Instance.ObtemSituacao(idPagto) == (int)Glass.Data.Model.Pagto.SituacaoPagto.Cancelado)
                return "Erro#Pagamento cancelado.";

            return "Ok";
        }
    }
}
