﻿using System;
using System.Linq;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.RetalhoProducao.Fluxo
{
    public sealed class DisponibilizarRetalho : BaseFluxo<DisponibilizarRetalho>
    {
        private DisponibilizarRetalho() { }

        public Entidade.DisponibilizarRetalho[] ObtemRetalhosDisponiveis(string numEtiqueta)
        {
            var retalhos = RetalhoProducaoDAO.Instance.GetForRpt(null, null, null, null, null, null, (int)Glass.Data.Model.RetalhoProducao.SituacaoRetalho.Disponivel,
                null, 0, 0, 0, 0, 0, numEtiqueta, null).ToArray();
            return Array.ConvertAll(retalhos, x => new Entidade.DisponibilizarRetalho(x));
        }

        public Entidade.DisponibilizarRetalho[] ObtemRetalhosEmEstoque(string numEtiqueta)
        {
            var retalhos = RetalhoProducaoDAO.Instance.GetForRpt(null, null, null, null, null, null, (int)Glass.Data.Model.RetalhoProducao.SituacaoRetalho.EmEstoque,
                null, 0, 0, 0, 0, 0, numEtiqueta, null).ToArray();
            return Array.ConvertAll(retalhos, x => new Entidade.DisponibilizarRetalho(x));
        }

        public void MarcaRetalhoDisponivel(uint codigoRetalho)
        {
            FilaOperacoes.RetalhoProducao.AguardarVez();

            try
            {
                if (RetalhoProducaoDAO.Instance.ObtemSituacao(codigoRetalho) != Glass.Data.Model.RetalhoProducao.SituacaoRetalho.EmEstoque)
                    throw new Exception("Só é possível alterar retalhos disponíveis para o estoque.");

                RetalhoProducaoDAO.Instance.AlteraSituacao(codigoRetalho, Glass.Data.Model.RetalhoProducao.SituacaoRetalho.Disponivel);
            }
            finally
            {
                FilaOperacoes.RetalhoProducao.ProximoFila();
            }
        }

        public void MarcaRetalhoEmEstoque(uint codigoRetalho)
        {
            FilaOperacoes.RetalhoProducao.AguardarVez();

            try
            {
                if (RetalhoProducaoDAO.Instance.ObtemSituacao(codigoRetalho) != Glass.Data.Model.RetalhoProducao.SituacaoRetalho.Disponivel)
                    throw new Exception("Só é possível alterar retalhos em estoque para disponíveis.");

                RetalhoProducaoDAO.Instance.AlteraSituacao(codigoRetalho, Glass.Data.Model.RetalhoProducao.SituacaoRetalho.EmEstoque);
            }
            finally
            {
                FilaOperacoes.RetalhoProducao.ProximoFila();
            }
        }
    }
}
