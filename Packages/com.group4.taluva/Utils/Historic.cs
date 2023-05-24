using System;
using System.Collections.Generic;
using System.Text;

namespace Taluva.Utils
{
    public class Historic<T> : List<T> where T : ICloneable
    {
        public int Index { get; private set; } = -1;

        public Historic() { }

        public Historic(Historic<T> original)
        {
            foreach (var c in original)
            {
                Add((T)c.Clone());
            }
        }

        public bool CanUndo => Index != -1;
        public bool CanRedo => Index != Count - 1;

        public new void Add(T element)
        {
            while (CanRedo)
                RemoveAt(Index + 1);

            Index++;
            base.Add(element);
        }

        public T Undo()
        {
            if (Count == 0)
                throw new InvalidOperationException();

            return this[Index--];
        }

        public T Redo()
        {
            if (Index == Count - 1)
                throw new InvalidOperationException();

            return this[++Index];
        }

        public T Last => this[Index];
        public T Next => this[Index + 1];

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < Count; i++)
            {
                T t = this[i];
                if (i == Index)
                    sb.Append('*');
                sb.Append(t).Append('\n');
            }

            return sb.ToString();
        }
    }
}