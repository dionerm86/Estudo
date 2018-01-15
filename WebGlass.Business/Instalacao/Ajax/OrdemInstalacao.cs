using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Instalacao.Ajax
{
    public interface IOrdemInstalacao
    {
        string Nova(string idsInstalacao, string idsEquipe, string tipoInstalacao,
            string dataInstalacao, string produtos, string obs, string noCache);
    }

    internal class OrdemInstalacao : IOrdemInstalacao
    {
        public string Nova(string idsInstalacao, string idsEquipe, string tipoInstalacao, 
            string dataInstalacao, string produtos, string obs, string noCache)
        {
            try
            {
                uint idOrdemInst = InstalacaoDAO.Instance.NovaOrdemInstalacao(idsInstalacao, DateTime.Parse(dataInstalacao),
                    Glass.Conversoes.StrParaInt(tipoInstalacao), idsEquipe, produtos, obs);

                return "ok\t" + idOrdemInst;
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao gerar Ordem de Instalação.", ex);
            }
        }
    }
}
