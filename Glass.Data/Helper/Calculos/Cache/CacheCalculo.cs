using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glass.Comum.Cache;

namespace Glass.Data.Helper.Calculos.Cache
{
    class CacheCalculo<T, ID>
    {
        private readonly CacheMemoria<int?, ID> cacheHashCode;
        private readonly Func<T, ID> idItem;
        private int tempoExpiracaoSegundos;
        private IEnumerable<PropertyInfo> propriedades;

        public CacheCalculo(string nome, Func<T, ID> idItem, int tempoExpiracaoSegundos = 3)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome do cache não pode ser nulo ou vazio.", "nome");

            if (idItem == null)
                throw new ArgumentException("Identificador do objeto não pode ser nulo.", "idItem");

            Func<Type, string> nomeTipo = tipo => tipo.Name;
            var nomeTipoT = nomeTipo(typeof(T));

            cacheHashCode = new CacheMemoria<int?, ID>(string.Format("{0}:hashCode", nomeTipoT));

            this.idItem = idItem;
            this.tempoExpiracaoSegundos = tempoExpiracaoSegundos;
        }

        public bool ItemEstaNoCache(T item)
        {
            var id = idItem(item);
            var hashCodeCache = cacheHashCode.RecuperarDoCache(id);

            if (hashCodeCache == null)
            {
                return false;
            }

            int hashCode = RecuperarHashCodeObjeto(item);
            return hashCode == hashCodeCache;
        }

        public void AtualizarItemNoCache(T item)
        {
            var id = idItem(item);
            int hashCode = RecuperarHashCodeObjeto(item);
            
            cacheHashCode.AtualizarItemNoCache(hashCode, id);
        }

        public void AlterarTempoExpiracaoSegundos(int novoTempoExpiracaoSegundos)
        {
            tempoExpiracaoSegundos = novoTempoExpiracaoSegundos;
        }

        private int RecuperarHashCodeObjeto(T objeto)
        {
            var valoresParaHash = ObterPropriedades()
                .Select(propriedade => ObterValorPropriedade(propriedade, objeto))
                .ToList();

            return RecuperarHashCodeListaObjetos(valoresParaHash);
        }

        private IEnumerable<PropertyInfo> ObterPropriedades()
        {
            if (propriedades == null)
            {
                propriedades = typeof(T)
                    .GetProperties()
                    .ToList();
            }

            return propriedades;
        }

        private object ObterValorPropriedade<U>(PropertyInfo propriedade, U inspect)
        {
            try
            {
                return propriedade.GetValue(inspect, null);
            }
            catch
            {
                return null;
            }
        }

        private int RecuperarHashCodeListaObjetos<U>(IEnumerable<U> sequence)
        {
            return sequence
                .Select(item => item != null
                    ? item.GetHashCode()
                    : 0)
                .Aggregate((total, nextCode) => total ^ nextCode);
        }
    }
}
