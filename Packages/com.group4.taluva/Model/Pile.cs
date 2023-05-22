using System;
using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Pile<T>
    {
        /// <summary>
        /// The stack representing the content of the pile.
        /// </summary>
        public Stack<T> Content { get; }

        [Obsolete("Use Content instead")]
        public Stack<T> _stack => Content;

        /// <summary>
        /// The list of items played from the pile.
        /// </summary>
        public List<T> Played { get; }

        [Obsolete("Use Played instead")]
        public List<T> _played => Played;

        /// <summary>
        /// Gets an array of the remaining items in the pile.
        /// </summary>
        /// <returns>An array of the remaining items.</returns>
        public T[] GetRemaining() => _stack.ToArray();

        /// <summary>
        /// Initializes a new instance of the Pile class with the specified list of items.
        /// </summary>
        /// <param name="list">The list of items in the pile.</param>
        public Pile(T[] list)
        {
            Random random = new Random();
            Played = new List<T>();

            Content = new Stack<T>(list.OrderBy(_ => random.Next()));
        }

        /// <summary>
        /// Initializes a new instance of the Pile class with the specified list of items.
        /// </summary>
        /// <param name="list">The list of items in the pile.</param>
        public Pile(List<T> list)
        {
            Content = new Stack<T>(list);
            Played = new List<T>();
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
            _played.RemoveAt(_played.Count - 1);
        }

        /// <summary>
        /// Gets the number of items remaining in the pile.
        /// </summary>
        public int NbKeeping => _stack.Count;
    }
}
