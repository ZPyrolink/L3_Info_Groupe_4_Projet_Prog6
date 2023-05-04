using System;
using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Pile<T>
    {
        private readonly Stack<T> _stack;
        private readonly List<T> _played;

        public Pile(T[] list)
        {
            Random random = new();
            _stack = new(list.Length);
            _played = new();

            _stack = new(list.OrderBy(x => random.Next()));
        }

        public T Draw()
        {
            T c = _stack.Pop();
            _played.Add(c);
            return c;
        }
    }
}