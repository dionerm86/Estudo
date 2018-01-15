using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Produto.Ajax
{
    public interface IAlterarDados
    {
        string AlterarDadosFiscais(string idsProd, string novaAliqICMS, string novaAliqICMSST, string novaAliqIPI,
            string novaMVA, string novaNCM, string cst, string cstIpi, string csosn, string codEx,
            string genProd, string tipoMerc, string planoContabil, string substituirICMS, string substituirMVA,
            string AlterarICMS, string alterarMVA, string cest);
        
        string AlterarGrupo(string idsProd, string idNovoGrupo, string idNovoSubgrupo);
    }

    internal class AlterarDados : IAlterarDados
    {
        public string AlterarDadosFiscais(string idsProd, string novaAliqICMS, string novaAliqICMSST, string novaAliqIPI,
            string novaMVA, string novaNCM, string cst, string cstIpi, string csosn, string codEx,
            string genProd, string tipoMerc, string planoContabil, string substituirICMS, string substituirMVA,
            string AlterarICMS, string alterarMVA, string cest)
        {
            try
            {
                ProdutoDAO.Instance.AlteraDadosFiscais(idsProd.TrimEnd(','), novaAliqICMS, Glass.Conversoes.StrParaFloat(novaAliqICMSST),
                    Glass.Conversoes.StrParaFloat(novaAliqIPI), novaMVA, novaNCM, cst, cstIpi, csosn, codEx, genProd,
                    tipoMerc, planoContabil, bool.Parse(substituirICMS), bool.Parse(substituirMVA), bool.Parse(AlterarICMS),
                    bool.Parse(alterarMVA), cest);

                return "Ok#Produtos alterados com sucesso!";
            }
            catch (Exception ex)
            {
                return "Erro#Falha ao alterar os seguintes produtos:\n\n" + ex.Message.Replace("|", "\n");
            }
        }

        public string AlterarGrupo(string idsProd, string idNovoGrupo, string idNovoSubgrupo)
        {
            try
            {
                uint idGrupo = Glass.Conversoes.StrParaUint(idNovoGrupo);
                uint? idSubgrupo = Glass.Conversoes.StrParaUintNullable(idNovoSubgrupo);

                ProdutoDAO.Instance.AlterarGrupoSubgrupo(idsProd.TrimEnd(','), idGrupo, idSubgrupo);

                return "Ok#Produtos alterados com sucesso!";
            }
            catch (Exception ex)
            {
                return "Erro#" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao alterar grupo/subgrupo.", ex);
            }
        }
    }
}
