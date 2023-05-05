using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Taluva.Utils
{
    public class DynamicMatrix<T> : IEnumerable<T>
    {
        private readonly Dictionary<int, Dictionary<int, T>> matrix;
        // Dans la théorie, cette liste devra ajouter 0.5 à ces coordonées en Y quand X % 2 == 1

        public DynamicMatrix()
        {
            matrix = new();
        }

        public int MaxLine => matrix.Keys.Order().ToArray()[matrix.Keys.Count - 1];

        public int MinLine => matrix.Keys.Order().ToArray()[0];

        public int MaxColumn(int line) => matrix[line].Keys.Order().ToArray()[matrix[line].Keys.Count - 1];

        public int MinColumn(int line) => matrix[line].Keys.Order().ToArray()[0];

        public bool ContainsLine(int line) => matrix.ContainsKey(line);

        public bool ContainsColumn(int line, int column) => matrix[line].ContainsKey(column);
        public bool ContainsColumn(Vector2Int p) => matrix[p.x].ContainsKey(p.y);

        public void Add(T value, Vector2Int coordonees)
        {
            if (!matrix.ContainsKey(coordonees.x))
            {
                Dictionary<int, T> tmp = new();
                tmp.Add(coordonees.y, value);
                matrix.Add(coordonees.x, tmp);
                return;
            }

            if (matrix[coordonees.x].ContainsKey(coordonees.y))
                matrix[coordonees.x][coordonees.y] = value;
            else
            {
                matrix[coordonees.x].Add(coordonees.y, value);
            }

        }

        public bool Remove(Vector2Int p) {
            bool remove = false;
            if(ContainsLine(p.x))
                if(ContainsColumn(p))
                    remove = matrix[p.x].Remove(p.y);

            if(remove)
                if (matrix[p.x].Count == 0)
                    matrix.Remove(p.x);
            return remove;
        }


        public T GetValue(Vector2Int coordonnes) => matrix[coordonnes.x][coordonnes.y];

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var x in matrix.OrderBy(x => x.Key))
            {
                foreach (var y in x.Value.OrderBy(y => y.Key))
                {
                    yield return y.Value;
                }
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsVoid(Vector2Int coordonnes) =>
            matrix.ContainsKey(coordonnes.x) ? !matrix[coordonnes.x].ContainsKey(coordonnes.y) : true;

        public bool IsEmpty() => matrix.Count == 0;

        override
            public string ToString()
        {
            string s = "";
            bool pair = true;

            for (int i = MinLine; i <= MaxLine; i++)
            {
                if (!pair)
                    s += " ";

                if (ContainsLine(i))
                    for (int j = MinColumn(i); j <= MaxColumn(i); j++)
                    {
                        if (ContainsColumn(i, j))
                            s += matrix[i][j];
                        else
                            s += " ";
                        s += " ";
                    }

                s += '\n';
                pair = !pair;
            }

            return s;
        }

    }
}