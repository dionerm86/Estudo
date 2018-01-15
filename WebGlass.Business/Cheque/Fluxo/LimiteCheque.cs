using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.Cheque.Fluxo
{
    public sealed class LimiteCheque : BaseFluxo<LimiteCheque>
    {
        private LimiteCheque() { }
        
        public Entidade.LimiteCheque[] ObtemItens(string cpfCnpj, string sortExpression, int startRow, int pageSize)
        {
            var itens = LimiteChequeCpfCnpjDAO.Instance.ObtemItens(cpfCnpj, sortExpression, startRow, pageSize).ToArray();
            return Array.ConvertAll(itens, x => new Entidade.LimiteCheque(x));
        }

        public int ObtemNumeroItens(string cpfCnpj)
        {
            return LimiteChequeCpfCnpjDAO.Instance.ObtemNumeroItens(cpfCnpj);
        }

        public decimal ObtemLimite(string cpfCnpj)
        {
            return LimiteChequeCpfCnpjDAO.Instance.ObtemLimite(cpfCnpj);
        }

        public decimal ObtemValorUtilizado(string cpfCnpj)
        {
            return LimiteChequeCpfCnpjDAO.Instance.ObtemValorChequesAbertos(cpfCnpj, null);
        }

        public void SalvaLimite(Entidade.LimiteCheque limite)
        {
            LimiteChequeCpfCnpjDAO.Instance.InsertOrUpdate(limite._limite);
        }

        public KeyValuePair<string, string>[] ObtemCpfCnpj()
        {
            var itens = LimiteChequeCpfCnpjDAO.Instance.ObtemCpfCnpj();
            return Array.ConvertAll(itens, x => new KeyValuePair<string, string>(x, Formatacoes.FormataCpfCnpj(x)));
        }
    }
}
