using System;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace WebGlass.Business.Instalacao.Ajax
{
    public interface IEquipe
    {
        string InsereColocador(string idEquipe, string tipoEquipe, string idFunc, string idTipoFunc);
    }

    internal class Equipe : IEquipe
    {
        public string InsereColocador(string idEquipe, string tipoEquipe, string idFunc, string idTipoFunc)
        {
            if (FuncEquipeDAO.Instance.IsAssociated(Glass.Conversoes.StrParaUint(idFunc), Glass.Conversoes.StrParaUint(idEquipe)))
                return "Funcionário já associado à equipe.";

            // Não permite adicionar funcionários do tipo instalador temperado se a equipe for de Colocação Comum
            if (tipoEquipe == "1" && Glass.Conversoes.StrParaUint(idTipoFunc) == (uint)Utils.TipoFuncionario.InstaladorTemperado)
                return "Instalador Temperado não pode ser adicionado à uma equipe de Instalação Comum.";

            // Não permite adicionar funcionários do tipo instalador comum se a equipe for de Colocação Temperado
            else if (tipoEquipe == "2" && Glass.Conversoes.StrParaUint(idTipoFunc) == (uint)Utils.TipoFuncionario.InstaladorComum) // Colocação Temperado
                return "Instalador Comum não pode ser adicionado à uma equipe de Instalação Temperado.";

            try
            {
                FuncEquipe funcEquipe = new FuncEquipe();
                funcEquipe.IdEquipe = Glass.Conversoes.StrParaUint(idEquipe);
                funcEquipe.Idfunc = Glass.Conversoes.StrParaUint(idFunc);
                FuncEquipeDAO.Instance.Insert(funcEquipe);
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Falha ao associar funcionário à Equipe.", ex);
            }

            return "ok";
        }
    }
}
