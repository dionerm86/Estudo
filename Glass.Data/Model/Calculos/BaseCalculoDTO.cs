using Glass.Comum.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model.Calculos
{
    abstract class BaseCalculoDTO
    {
        /// <summary>
        /// Recupera um item a partir do cache, se existir. Caso não exista, cria um mecanismo para recuperação do
        /// dado a partir do banco e depois atualiza o cache.
        /// </summary>
        protected Lazy<T> ObterUsandoCache<T, ID>(CacheMemoria<T, ID> cache, ID id, Func<T> recuperarDoBanco,
            bool forcarAtualizacao = false)
            where T : class, new()
        {
            T itemCache = RecuperarDoCache(cache, id, forcarAtualizacao);

            if (itemCache != null)
            {
                return new Lazy<T>(() => itemCache);
            }

            return new Lazy<T>(() =>
            {
                itemCache = RecuperarDoCache(cache, id, forcarAtualizacao);

                if (itemCache == null)
                {
                    try
                    {
                        itemCache = recuperarDoBanco() ?? new T();
                    }
                    catch
                    {
                        itemCache = new T();
                    }

                    cache.AtualizarItemNoCache(itemCache, id);
                }

                return itemCache;
            });
        }

        private T RecuperarDoCache<T, ID>(CacheMemoria<T, ID> cache, ID id, bool forcarAtualizacao)
            where T : class, new()
        {
            return forcarAtualizacao
                ? null
                : cache.RecuperarDoCache(id);
        }
    }
}
