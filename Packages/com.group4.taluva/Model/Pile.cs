using System;
using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Pile<T>
    {
        public Stack<T> _stack { get; set; }
        public readonly List<T> _played;
        
        public T[] GetRemaining() => _stack.ToArray();

        public Pile(T[] list)
        {
            Random random = new();
            _played = new();

            _stack = new(list.OrderBy(_ => random.Next()));
        }

        public Pile(List<T> list)
        {
            _stack = new(list);
            _played = new();

        }

        public T Draw()
        {
            T c = _stack.Pop();
            _played.Add(c);
            return c;
        }

        public void Stack(T stack)
        {
            _stack.Push(stack);
            _played.RemoveAt(_played.Count - 1);
        }

        public int NbKeeping => _stack.Count;
    }
}