using System;
using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Pile<T>
    {
        public readonly Stack<T> _stack;
        private readonly List<T> _played;

        /// <summary>
        /// Initializes a new instance of the Pile class with the specified list of items.
        /// </summary>
        /// <param name="list">The list of items in the pile.</param>
        public Pile(T[] list)
        {
            Random random = new Random();
            _stack = new Stack<T>(list.Length);
            _played = new List<T>();

            foreach (T item in list.OrderBy(_ => random.Next()))
            {
                _stack.Push(item);
            }
        }

        /// <summary>
        /// Draws an item from the pile.
        /// </summary>
        /// <returns>The drawn item.</returns>
        public T Draw()
        {
            T item = _stack.Pop();
            _played.Add(item);
            return item;
        }

        /// <summary>
        /// Restores an item back to the pile.
        /// </summary>
        /// <param name="item">The item to be restored.</param>
        public void Stack(T item)
        {
            _stack.Push(item);
            _played.Remove(item);
        }
    }
}
