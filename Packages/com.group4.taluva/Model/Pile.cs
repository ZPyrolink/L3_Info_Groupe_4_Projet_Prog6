using System;
using System.Collections.Generic;
using System.Linq;

namespace Taluva.Model
{
    public class Pile<T> where T: ICloneable
    {
        public Stack<T> Content { get; }
        [Obsolete("Use Content instead", true)]
        public Stack<T> _stack => Content;
        public List<T> Played { get; }
        [Obsolete("Use Played instead", true)]
        public List<T> _played => Played;
        
        public T[] GetRemaining() => Content.ToArray();

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
            Content = new Stack<T>(p.Content.Reverse().Select(item => (T)item.Clone()));
            Played = new List<T>(p.Played.Select(item => (T)item.Clone()));
        }


        public Pile<T> Clone()
        {
            return new Pile<T>(this);
        }
        public T Draw()
        {
            T c = Content.Pop();
            Played.Add(c);
            return c;
        }
        

        public void Stack(T stack)
        {
            Content.Push(stack);
            Played.RemoveAt(Played.Count - 1);
        }

        public int NbKeeping => Content.Count;
    }
}
