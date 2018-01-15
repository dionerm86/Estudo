using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Rota.Ajax
{
    public interface IBuscarEValidar
    {
        string GetRota(string codRota);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetRota(string codRota)
        {
            try
            {
                var rota = RotaDAO.Instance.GetByCodInterno(codRota);

                if (rota == null)
                    return "Erro|Não foi encontrada nenhuma rota com o código informado.";

                return "Ok|" + rota.IdRota + "|" + rota.Descricao;
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar rota.", ex);
            }
        }
    }
}
