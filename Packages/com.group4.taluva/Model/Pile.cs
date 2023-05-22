using System;
using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Pile<T>
    {
        public Stack<T> Content { get; }
        [Obsolete("Use Content instead")]
        public Stack<T> _stack => Content;
        public List<T> Played { get; }
        [Obsolete("Use Played instead")]
        public List<T> _played => Played;
        
        public T[] GetRemaining() => _stack.ToArray();

        /// <summary>
        /// Initializes a new instance of the Pile class with the specified list of items.
        /// </summary>
        /// <param name="list">The list of items in the pile.</param>
        public Pile(T[] list)
        {
            Random random = new();
            Played = new();

            Content = new(list.OrderBy(_ => random.Next()));
        }

        public Pile(List<T> list)
        {
            Content = new(list);
            Played = new();

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
        public void Stack(T stack)
        {
            _stack.Push(stack);
            _played.RemoveAt(_played.Count - 1);
        }

        public int NbKeeping => _stack.Count;
    }
}
