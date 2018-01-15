using System;
using System.Collections.Generic;
using Glass.Data.DAL;

namespace WebGlass.Business.SubgrupoProd.Ajax
{
    public interface IBuscarEValidar
    {
        string LoadSubgruposAlterarDados(string idGrupoProdStr, string isNenhum);
        string LoadSubgruposAlterarGrupo(string idGrupoProdStr, string isNenhum);
        string GetSubgrupos(string idGrupo, string textoVazio);
        string ExibirProducao(string idGrupo, string idSubgrupo);
        string IsSubgrupoProducao(string idGrupo, string idSubgrupo);
        string ExibirBenef(string idGrupo, string idSubgrupo);
        string GetSubgrupos(string idGrupo);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string LoadSubgruposAlterarDados(string idGrupoProdStr, string isNenhum)
        {
            int idGrupoProd = Glass.Conversoes.StrParaInt(idGrupoProdStr);
            string retorno = "";

            foreach (var s in SubgrupoProdDAO.Instance.GetForFilter(idGrupoProd))
            {
                if (s.IdSubgrupoProd == 0 && isNenhum == "true")
                    s.Descricao = "Nenhum";

                retorno += String.Format("<option value='{0}'>{1}</option>", s.IdSubgrupoProd, s.Descricao);
            }

            return retorno;
        }

        public string LoadSubgruposAlterarGrupo(string idGrupoProdStr, string isNenhum)
        {
            return LoadSubgruposAlterarDados(idGrupoProdStr, isNenhum);
        }

        public string GetSubgrupos(string idGrupo, string textoVazio)
        {
            try
            {
                string retorno = "";
                foreach (var s in SubgrupoProdDAO.Instance.GetForFilter(Glass.Conversoes.StrParaInt(idGrupo)))
                {
                    if (s.IdSubgrupoProd == 0)
                        s.Descricao = textoVazio;

                    retorno += string.Format("<option value='{0}'>{1}</option>", s.IdSubgrupoProd, s.Descricao == null ? string.Empty : s.Descricao.Replace("/", ""));
                }

                if (retorno == "")
                    retorno = "<option value=0>Nenhum</option>";

                return retorno;
            }
            catch
            {
                return "<option></option>";
            }
        }

        public string ExibirProducao(string idGrupo, string idSubgrupo)
        {
            try
            {
                var grupo = !String.IsNullOrEmpty(idGrupo) ? Glass.Conversoes.StrParaInt(idGrupo) : 0;
                var subgrupo = !String.IsNullOrEmpty(idSubgrupo) ? Glass.Conversoes.StrParaInt(idSubgrupo) : 0;
                Glass.Data.Model.TipoSubgrupoProd tipoSubgrupo = subgrupo > 0 ? SubgrupoProdDAO.Instance.GetElementByPrimaryKey((uint)subgrupo).TipoSubgrupo : Glass.Data.Model.TipoSubgrupoProd.Indefinido;

                if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(grupo) || tipoSubgrupo == Glass.Data.Model.TipoSubgrupoProd.PVB)
                {
                    var tiposCalc = new List<int> { (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd };
                    if (!Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(grupo))
                        tiposCalc.AddRange(new[] { (int)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2, (int)Glass.Data.Model.TipoCalculoGrupoProd.QtdDecimal });

                    return tiposCalc.Contains(Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(grupo, subgrupo)).ToString().ToLower();
                }
                else
                    return "false";
            }
            catch
            {
                return "false";
            }
        }

        public string IsSubgrupoProducao(string idGrupo, string idSubgrupo)
        {
            try
            {
                var grupo = !String.IsNullOrEmpty(idGrupo) ? Glass.Conversoes.StrParaInt(idGrupo) : 0;
                return SubgrupoProdDAO.Instance.IsSubgrupoProducao(grupo, !String.IsNullOrEmpty(idSubgrupo) ? 
                    (int?)Glass.Conversoes.StrParaInt(idSubgrupo) : null).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        public string ExibirBenef(string idGrupo, string idSubgrupo)
        {
            try
            {
                var grupo = !String.IsNullOrEmpty(idGrupo) ? Glass.Conversoes.StrParaInt(idGrupo) : 0;
                var subgrupo = !String.IsNullOrEmpty(idSubgrupo) ? Glass.Conversoes.StrParaInt(idSubgrupo) : 0;

                if (GrupoProdDAO.Instance.IsVidro(grupo))
                    /* Chamado 49252. */
                    return "true";
                else
                    return "false";
            }
            catch
            {
                return "false";
            }
        }

        public string GetSubgrupos(string idGrupo)
        {
            try
            {
                string retorno = "0;";
                foreach (var s in SubgrupoProdDAO.Instance.GetList(Glass.Conversoes.StrParaInt(idGrupo)))
                    retorno += "|" + s.IdSubgrupoProd + ";" + s.Descricao;

                return retorno;
            }
            catch (Exception ex)
            {
                return "0;(" + Glass.MensagemAlerta.FormatErrorMsg("Erro", ex) + ")";
            }
        }
    }
}
