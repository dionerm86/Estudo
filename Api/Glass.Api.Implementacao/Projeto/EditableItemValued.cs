using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Representa um item editavél com valor padrão.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditableItemValued<T> : Glass.Api.Projeto.IEditableItemValued
    {
        /// <summary>
        /// Item.
        /// </summary>
        public T Item { get; }

        /// <summary>
        /// Valor padrão.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Se o item é editável.
        /// </summary>
        public bool IsEditable { get; }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isEditable"></param>
        public EditableItemValued(T item, int defaultValue, bool isEditable)
        {
            Item = item;
            Value = defaultValue;
            IsEditable = isEditable;
        }

    }
}
