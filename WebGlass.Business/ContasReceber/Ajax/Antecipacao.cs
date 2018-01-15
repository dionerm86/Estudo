using System;
using Glass.Data.DAL;

namespace WebGlass.Business.ContasReceber.Ajax
{
    public interface IAntecipacao
    {
        string Antecipar(string idsContasRec, string idContaBanco, string total, string taxa, string juros,
            string iof, string dataRec, string obs);
    }

    internal class Antecipacao : IAntecipacao
    {
        public string Antecipar(string idsContasRec, string idContaBanco, string total, string taxa, string juros,
            string iof, string dataRec, string obs)
        {
            try
            {
                uint idAntecipContaRec = AntecipContaRecDAO.Instance.Antecipar(idsContasRec.TrimEnd(','),
                    Glass.Conversoes.StrParaUint(idContaBanco), decimal.Parse(total.Replace("R$", "").Replace(" ", "")), decimal.Parse(taxa),
                    decimal.Parse(juros), decimal.Parse(iof), DateTime.Parse(dataRec), obs);

                return "Ok|" + idAntecipContaRec;
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao antecipar boletos.", ex);
            }
        }
    }
}
