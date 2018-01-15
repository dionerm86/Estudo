using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Orcamento.Ajax
{
    public interface IDuplicar
    {
        string DuplicarOrcamento(string idOrcamentoStr);
    }

    internal class Duplicar : IDuplicar
    {
        public string DuplicarOrcamento(string idOrcamentoStr)
        {
            try
            {
                uint idOrcamento = Glass.Conversoes.StrParaUint(idOrcamentoStr);
                return "Ok;" + OrcamentoDAO.Instance.Duplicar(idOrcamento);
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao duplicar orçamento.", ex);
            }
        }
    }
}
