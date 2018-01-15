using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public sealed class FormulaExpressaoCalculoDAO : BaseDAO<FormulaExpressaoCalculo, FormulaExpressaoCalculoDAO>
    {
        #region Busca Padrão

        private string Sql()
        {
            var sql = "SELECT * FROM formula_expressao_calculo";
            return sql;
        }
        public IList<FormulaExpressaoCalculo> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new FormulaExpressaoCalculo[] { new FormulaExpressaoCalculo() };

            var sql = Sql();
            var lista = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize);
            return lista.ToList();
        }

        public int GetCount()
        {
            int count = GetCountReal();
            return count > 0 ? count : 1;
        }

        public int GetCountReal()
        {
            return GetCountWithInfoPaging(Sql());
        }

        #endregion

        public List<FormulaExpressaoCalculo> ObtemFormulaExpressaoPorArrayPosicaoPecaModelo(PosicaoPecaModelo[] arrayPosicaoPecaModelo)
        {
            var listaTodasFormulas = GetAll();
            List<FormulaExpressaoCalculo> listFormulaPeca = new List<FormulaExpressaoCalculo>();
            foreach(var posicao in arrayPosicaoPecaModelo)
            {
                var temp = ObtemFormulaExpressaoPorCalcPosicaoPeca(ref listaTodasFormulas, posicao.Calc);
                if (temp.Count > 0)
                    listFormulaPeca.AddRange(temp);
            }
            return listFormulaPeca;
        }

        public List<FormulaExpressaoCalculo> ObtemFormulaExpressaoPorArrayPosicaoPecaIndividual(PosicaoPecaIndividual[] arrayPosicaoPecaIndividual)
        {
            var listaTodasFormulas = GetAll();
            List<FormulaExpressaoCalculo> listFormulaPeca = new List<FormulaExpressaoCalculo>();
            foreach (var posicao in arrayPosicaoPecaIndividual)
            {
                var temp = ObtemFormulaExpressaoPorCalcPosicaoPeca(ref listaTodasFormulas, posicao.Calc);
                if (temp.Count > 0)
                    listFormulaPeca.AddRange(temp);
            }
            return listFormulaPeca;
        }

        public List<FormulaExpressaoCalculo> ObtemFormulaExpressaoPorCalcPosicaoPeca(ref FormulaExpressaoCalculo[] listaTodasFormulas, string expressao)
        {
            List<FormulaExpressaoCalculo> listFormulaPeca = new List<FormulaExpressaoCalculo>();

            for (int i = 0; i < listaTodasFormulas.Length; i++)
                if (listaTodasFormulas[i].Expressao != null && listaTodasFormulas[i].Expressao != "")
                    if (expressao.Contains(listaTodasFormulas[i].Descricao))
                    {
                        expressao = expressao.Replace(listaTodasFormulas[i].Descricao, listaTodasFormulas[i].Expressao);
                        listFormulaPeca.Add(listaTodasFormulas[i]);
                        i = -1;
                    }
            return listFormulaPeca;
        }

        #region Obtem valor dos campos

        public int? ObterIdFormulaPelaDescricao(string descricao)
        {
            return ObtemValorCampo<int?>("IdFormulaExpreCalc", "Descricao=?descricao", new GDA.GDAParameter("?descricao", descricao));
        }

        public string ObterDescricao(int idFormulaExpreCalc)
        {
            return ObtemValorCampo<string>("Descricao", string.Format("IdFormulaExpreCalc={0}", idFormulaExpreCalc));
        }

        #endregion

        #region Valida inserção/atualização/deleção
        
        /// <summary>
        /// Valida qualquer atualização feita no registro de fórmula de expressão de cálculo.
        /// </summary>
        private void ValidarSalvarFormulaExpressaoCalculo(FormulaExpressaoCalculo formulaExpressaoCalculo, string mensagem, bool inserindoDescricaoNova)
        {
            // Caso a descrição esteja vazia, não permite a atualização do registro.
            if (string.IsNullOrEmpty(formulaExpressaoCalculo.Descricao))
                throw new Exception("Informe a descrição da fórmula de expressão de cálculo.");

            // Caso já exista uma fórmula com a mesma descrição, não permite a atualização do registro.
            if (inserindoDescricaoNova ? ObterIdFormulaPelaDescricao(formulaExpressaoCalculo.Descricao) > 0 : false)
                throw new Exception("Já existe uma fórmula de expressão de cálculo cadastrada com essa descrição.");

            #region Impede a atualização da fórmula que possuir associação com outra tabela ou fórmula.

            // Verifica a associação da fórmula na tabela posicao_peca_individual.
            if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM posicao_peca_individual WHERE Calc LIKE ?descricao;",
                new GDA.GDAParameter("?descricao", "%" + formulaExpressaoCalculo.Descricao + "%")) > 0)
                throw new Exception(string.Format(mensagem, "um cálculo de peça individual."));

            // Verifica a associação da fórmula na tabela posicao_peca_modelo.
            if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM posicao_peca_modelo WHERE Calc LIKE ?descricao;",
                new GDA.GDAParameter("?descricao", "%" + formulaExpressaoCalculo.Descricao + "%")) > 0)
                throw new Exception(string.Format(mensagem, "um cálculo de peça modelo."));

            // Verifica a associação da fórmula na tabela validacao_peca_modelo.
            if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM validacao_peca_modelo WHERE PrimeiraExpressaoValidacao LIKE ?descricao OR SegundaExpressaoValidacao LIKE ?descricao;",
                new GDA.GDAParameter("?descricao", "%" + formulaExpressaoCalculo.Descricao + "%")) > 0)
                throw new Exception(string.Format(mensagem, "um cálculo de validação de peça."));

            // Verifica a associação da fórmula na tabela material_projeto_modelo.
            if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM material_projeto_modelo WHERE CalculoQtde LIKE ?descricao OR CalculoAltura LIKE ?descricao;",
                new GDA.GDAParameter("?descricao", "%" + formulaExpressaoCalculo.Descricao + "%")) > 0)
                throw new Exception(string.Format(mensagem, "um cálculo de material de projeto."));

            // Verifica a associação da fórmula na tabela peca_projeto_modelo.
            if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM peca_projeto_modelo WHERE CalculoAltura LIKE ?descricao OR CalculoLargura LIKE ?descricao OR CalculoQtde LIKE ?descricao;",
                new GDA.GDAParameter("?descricao", "%" + formulaExpressaoCalculo.Descricao + "%")) > 0)
                throw new Exception(string.Format(mensagem, "um cálculo de peça do modelo de projeto."));

            // Verifica a associação da fórmula na tabela formula_expressao_calculo, no campo expressão da fórmula.
            if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM formula_expressao_calculo WHERE Expressao LIKE ?descricao;",
                new GDA.GDAParameter("?descricao", "%" + formulaExpressaoCalculo.Descricao + "%")) > 0)
                throw new Exception(string.Format(mensagem, "uma outra fórmula de cálculo."));

            // Verifica a existência da fórmula na tabela formula_expressao_calculo com a descrição semelhante à informada.
            if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM formula_expressao_calculo WHERE Descricao LIKE ?descricao;",
                new GDA.GDAParameter("?descricao", "%" + formulaExpressaoCalculo.Descricao + "%")) > (inserindoDescricaoNova ? 0 : 1))
                throw new Exception(string.Format(mensagem, "uma outra fórmula de cálculo."));

            // Verifica a existência de um fórmula na tabela formula_expressao_calculo que sua descrição esteja contida na descrição do registro que está sendo atualizado.
            if (objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM formula_expressao_calculo WHERE INSTR(?descricao, Descricao) > 0;",
                new GDA.GDAParameter("?descricao", formulaExpressaoCalculo.Descricao)) > (inserindoDescricaoNova ? 0 : 1))
                throw new Exception(string.Format(mensagem, "uma outra fórmula de cálculo."));

            #endregion
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(FormulaExpressaoCalculo objInsert)
        {
            // Valida a inserção da fórmula.
            ValidarSalvarFormulaExpressaoCalculo(objInsert, "Não é possível inserir uma fórmula com esta descrição. Ela está vinculada a pelo menos {0}", true);

            return base.Insert(objInsert);
        }

        public override int Update(FormulaExpressaoCalculo objUpdate)
        {
            // Recupera a descrição atual da fórmula.
            var descricaoAtual = ObterDescricao((int)objUpdate.IdFormulaExpreCalc);

            // Valida a descrição somente se tiver sido alterada.
            if (descricaoAtual != objUpdate.Descricao)
            {
                // Salva a descrição nova da fórmula para que seja validada após a descrição atual.
                var descricaoNova = objUpdate.Descricao;
                objUpdate.Descricao = descricaoAtual;

                // Valida a descrição atual da fórmula.
                ValidarSalvarFormulaExpressaoCalculo(objUpdate, "Não é possível alterar a descrição desta fórmula. Ela está vinculada a pelo menos {0}", false);

                // Salva a descrição nova no objeto.
                objUpdate.Descricao = descricaoNova;

                // Valida a descrição nova.
                ValidarSalvarFormulaExpressaoCalculo(objUpdate, "Não é possível alterar a descrição desta fórmula. Ela está vinculada a pelo menos {0}", true);
            }
            
            return base.Update(objUpdate);
        }

        public override int Delete(FormulaExpressaoCalculo objDelete)
        {
            // Recupera a descrição da fórmula.
            objDelete.Descricao = ObtemValorCampo<string>("Descricao", string.Format("IdFormulaExpreCalc={0}", objDelete.IdFormulaExpreCalc));
            
            // Valida a exclusão da fórmula.
            ValidarSalvarFormulaExpressaoCalculo(objDelete, "Não é possível excluir esta fórmula. Ela está vinculada a pelo menos {0}", false);
            
            return base.Delete(objDelete);
        }

        #endregion
    }
}