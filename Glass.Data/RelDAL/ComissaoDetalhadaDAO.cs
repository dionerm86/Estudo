using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelModel;
using System.Collections.Generic;

namespace Glass.Data.RelDAL
{
    public sealed class ComissaoDetalhadaDAO : Glass.Pool.Singleton<ComissaoDetalhadaDAO>
    {
        private ComissaoDetalhadaDAO() { }

        public ComissaoDetalhada[] GetByFuncionario(uint idFuncionario)
        {
            List<ComissaoDetalhada> retorno = new List<ComissaoDetalhada>();

            try
            {
                List<Funcionario> f = new List<Funcionario>();
                if (idFuncionario > 0)
                    f.Add(FuncionarioDAO.Instance.GetElementByPrimaryKey(idFuncionario));
                else
                    f.AddRange(FuncionarioDAO.Instance.GetVendedoresByComissao());

                foreach (Funcionario func in f)
                {
                    ComissaoDetalhada novo = new ComissaoDetalhada();
                    novo.IdFuncionario = (uint)func.IdFunc;
                    novo.TipoFuncionario = (int)ComissaoDetalhada.TipoFunc.Funcionario;
                    novo.NomeFuncionario = func.Nome;
                    novo.Cpf = func.Cpf;
                    novo.TelefoneContato = func.TelCont;
                    novo.TelefoneResidencial = func.TelRes;
                    novo.TelefoneCelular = func.TelCel;
                    novo.Email = func.Email;
                    novo.Logradouro = func.Endereco;
                    novo.Complemento = func.Compl;
                    novo.Bairro = func.Bairro;
                    novo.Cidade = func.Cidade;
                    novo.Cep = func.Cep;
                    novo.Obs = func.Obs;

                    retorno.Add(novo);
                }
            }
            catch { }

            return retorno.ToArray();
        }

        public ComissaoDetalhada[] GetByComissionado(uint idComissionado)
        {
            List<ComissaoDetalhada> retorno = new List<ComissaoDetalhada>();

            try
            {
                List<Comissionado> c = new List<Comissionado>();
                if (idComissionado > 0)
                    c.Add(ComissionadoDAO.Instance.GetElementByPrimaryKey(idComissionado));
                else
                    c.AddRange(ComissionadoDAO.Instance.GetComissionadosByComissao());

                foreach (Comissionado com in c)
                {
                    ComissaoDetalhada novo = new ComissaoDetalhada();
                    novo.IdFuncionario = (uint)com.IdComissionado;
                    novo.TipoFuncionario = (int)ComissaoDetalhada.TipoFunc.Comissionado;
                    novo.NomeFuncionario = com.Nome;
                    novo.Cpf = com.CpfCnpj;
                    novo.TelefoneContato = com.TelCont;
                    novo.TelefoneResidencial = com.TelRes;
                    novo.TelefoneCelular = com.TelCel;
                    novo.Email = com.Email;
                    novo.Logradouro = com.Endereco;
                    novo.Complemento = com.Compl;
                    novo.Bairro = com.Bairro;
                    novo.Cidade = com.Cidade;
                    novo.Cep = com.Cep;
                    novo.Banco = com.Banco;
                    novo.Agencia = com.Agencia;
                    novo.Conta = com.Conta;
                    novo.Obs = com.Obs;

                    retorno.Add(novo);
                }
            }
            catch { }

            return retorno.ToArray();
        }

        public ComissaoDetalhada[] GetByTipo(uint id, int tipoFunc)
        {
            if (tipoFunc == (int)ComissaoDetalhada.TipoFunc.Comissionado)
                return GetByComissionado(id);
            else
                return GetByFuncionario(id);
        }
    }
}
