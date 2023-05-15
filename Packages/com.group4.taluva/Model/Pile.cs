using System;
using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Pile<T>
    {
        public Stack<T> _stack { get; }
        public readonly List<T> _played;
        
        public T[] GetRemaining() => _stack.ToArray();

        public Pile(T[] list)
        {
            Random random = new();
            _stack = new(list.Length);
            _played = new();

            _stack = new(list.OrderBy(_ => random.Next()));
        }

        public T Draw()
        {
            T c = _stack.Pop();
            _played.Add(c);
            return c;
        }

        public void Stack(T chunk)
        {
            _stack.Push(chunk);
            _played.Remove(chunk);
        }

        public int NbKeeping => _stack.Count;
    }
}