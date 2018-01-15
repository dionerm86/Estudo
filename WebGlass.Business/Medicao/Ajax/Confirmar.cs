using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Medicao.Ajax
{
    public interface IConfirmar
    {
        string ConfirmarMedicao(string idsMedicao, string idMedidor, string dataEfetuar);
    }

    internal class Confirmar : IConfirmar
    {
        public string ConfirmarMedicao(string idsMedicao, string idMedidor, string dataEfetuar)
        {
            try
            {
                MedicaoDAO.Instance.SetMedicoesForMedidor(Glass.Conversoes.StrParaUint(idMedidor), 
                    idsMedicao.TrimEnd(','), DateTime.Parse(dataEfetuar));

                return "ok\tMedições associadas.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao atribuir medições ao medidor.", ex);
            }
        }
    }
}
