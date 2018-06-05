using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glass.Comum.Cache;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Cache
{
    abstract class CacheCalculo
    {
        protected static readonly IDictionary<Type, IEnumerable<PropertyInfo>> propriedades;

        static CacheCalculo()
        {
            propriedades = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        }
    }

    class CacheCalculo<T, ID> : CacheCalculo
    {
        private readonly CacheMemoria<int?, ID> cacheHashCode;
        private readonly Func<T, ID> idItem;
        
        public CacheCalculo(string nome, Func<T, ID> idItem, int tempoExpiracaoSegundos = 3)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome do cache não pode ser nulo ou vazio.", "nome");

            if (idItem == null)
                throw new ArgumentException("Identificador do objeto não pode ser nulo.", "idItem");

            Func<Type, string> nomeTipo = tipo => tipo.Name;
            var nomeTipoT = nomeTipo(typeof(T));

            cacheHashCode = new CacheMemoria<int?, ID>(
                string.Format("{0}:hashCode", nomeTipoT),
                tempoExpiracaoSegundos
            );
            
            this.idItem = idItem;
        }

        public bool ItemEstaNoCache(T item)
        {
            var id = idItem(item);
            var hashCodeCache = cacheHashCode.RecuperarDoCache(id);

            if (hashCodeCache == null)
            {
                return false;
            }

            int hashCode = RecuperarHashCodeObjeto(item, typeof(T));
            return hashCode == hashCodeCache;
        }

        public void AtualizarItemNoCache(T item)
        {
            var id = idItem(item);
            int hashCode = RecuperarHashCodeObjeto(item, typeof(T));
            
            cacheHashCode.AtualizarItemNoCache(hashCode, id);
        }

        public void AlterarTempoExpiracaoSegundos(int novoTempoExpiracaoSegundos)
        {
            cacheHashCode.AlterarTempoExpiracaoSegundos(novoTempoExpiracaoSegundos);
        }

        private int RecuperarHashCodeObjeto(object objeto, Type tipo)
        {
            if (objeto == null)
                return 1;

            if (tipo.FullName.Contains("Glass."))
            {
                var valoresParaHash = ObterPropriedades(tipo)
                    .Select(propriedade => ObterValorPropriedade(propriedade, objeto))
                    .ToList();

                return RecuperarHashCodeListaObjetos(valoresParaHash);
            }

            return objeto.GetHashCode();
        }

        private IEnumerable<PropertyInfo> ObterPropriedades(Type tipo)
        {
            if (!propriedades.ContainsKey(tipo))
            {
                propriedades.Add(tipo, tipo
                    .GetProperties()
                    .ToList());
            }

            return propriedades[tipo];
        }

        private Tuple<Type, object> ObterValorPropriedade(PropertyInfo propriedade, object objeto)
        {
            try
            {
                // Ignora propriedades indexadas
                if (!propriedade.GetIndexParameters().Any())
                    return new Tuple<Type, object>(propriedade.PropertyType, propriedade.GetValue(objeto, null));
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        private int RecuperarHashCodeListaObjetos(IEnumerable<Tuple<Type, object>> sequencia)
        {
            if (!sequencia.Any())
                return 1;

            return sequencia
                .Where(item => item != null)
                .Select(item => RecuperarHashCodeObjeto(item.Item2, item.Item1))
                .Aggregate((total, proximo) => total ^ proximo);
        }
    }
}
