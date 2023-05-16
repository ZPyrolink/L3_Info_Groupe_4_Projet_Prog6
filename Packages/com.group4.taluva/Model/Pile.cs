using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Taluva.Model
{
    public class Pile<T>
    {
        public Stack<T> _stack { get; }
        public readonly List<T> _played;
        
        public T[] GetRemaining() => _stack.ToArray();

        public Pile(T[] list)
        {
            System.Random random = new();
            _stack = new(list.Length);
            _played = new();

            _stack = new(list.OrderBy(_ => random.Next()));
        }

        public Pile(List<T> list)
        {
            _stack = new(list.Count);
            _played = new();

            foreach(T item in list)
            {
                _stack.Push(item);
            }
        }

        public T Draw()
        {
            T c = _stack.Pop();
            _played.Add(c);
            return c;
        }

        public void Stack(T stack, T remove)
        {
            _stack.Push(stack);
            Debug.Log("b " + _played.Remove(remove));
        }

        public int NbKeeping => _stack.Count;
    }
}