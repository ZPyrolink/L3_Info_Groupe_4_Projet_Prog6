using System;
using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Pile<T> where T: ICloneable
    {
        public Stack<T> Content { get; }
        [Obsolete("Use Content instead")]
        public Stack<T> _stack => Content;
        public List<T> Played { get; }
        [Obsolete("Use Played instead")]
        public List<T> _played => Played;
        
        public T[] GetRemaining() => _stack.ToArray();

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

        public Pile(Pile<T> p)
        {
            Content = new Stack<T>();
            foreach (T tmp in p._stack)
            {
                Content.Push((T)tmp.Clone());
            }
            Played = new List<T>(p.Played);
        }

        public Pile<T> Clone()
        {
            return new Pile<T>(this);
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
