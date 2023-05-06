using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Taluva.Utils
{
    public class DynamicMatrix<T> : IEnumerable<T>
    {
        private readonly Dictionary<int, Dictionary<int, T>> _matrix;
        // Dans la théorie, cette liste devra ajouter 0.5 à ces coordonées en Y quand X % 2 == 1

        public DynamicMatrix()
        {
            _matrix = new();
        }

        public int MaxLine => _matrix.Keys.Order().ElementAt(_matrix.Keys.Count - 1);

        public int MinLine => _matrix.Keys.Order().ToArray().First();

        public int MaxColumn(int line) => _matrix[line].Keys.Order().ElementAt(_matrix[line].Keys.Count - 1);

        public int MinColumn(int line) => _matrix[line].Keys.Order().First();

        public bool ContainsLine(int line) => _matrix.ContainsKey(line);

        public bool ContainsColumn(int line, int column) => _matrix[line].ContainsKey(column);
        public bool ContainsColumn(Vector2Int p) => _matrix[p.x].ContainsKey(p.y);

        public void Add(T value, Vector2Int coordonees)
        {
            if (!_matrix.ContainsKey(coordonees.x))
            {
                _matrix.Add(coordonees.x, new() { { coordonees.y, value } });
                return;
            }

            if (_matrix[coordonees.x].ContainsKey(coordonees.y))
                _matrix[coordonees.x][coordonees.y] = value;
            else
                _matrix[coordonees.x].Add(coordonees.y, value);
        }

        public bool Remove(Vector2Int p)
        {
            bool remove = ContainsLine(p.x) && ContainsColumn(p) && _matrix[p.x].Remove(p.y);

            if (remove && _matrix[p.x].Count == 0)
                _matrix.Remove(p.x);

            return remove;
        }

        [Obsolete("Use the indexer instead!")]
        public T GetValue(Vector2Int coordonnes) => _matrix[coordonnes.x][coordonnes.y];

        public T this[Vector2Int co] => _matrix[co.x][co.y];

        public IEnumerator<T> GetEnumerator() => _matrix
            .OrderBy(x => x.Key)
            .SelectMany(x => x.Value
                .OrderBy(y => y.Key), (x, y) => y.Value)
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsVoid(Vector2Int coordonnes) =>
            !_matrix.ContainsKey(coordonnes.x) || !_matrix[coordonnes.x].ContainsKey(coordonnes.y);

        [Obsolete("Use the property instead !")]
        public bool IsEmpty() => _matrix.Count == 0;

        /// <summary>
        /// ToDo: Remove the method and rename this property
        /// </summary>
        public bool Empty => _matrix.Count == 0;

        public override string ToString()
        {
            StringBuilder s = new();
            bool pair = true;

            for (int i = MinLine; i <= MaxLine; i++)
            {
                if (!pair)
                    s.Append(" ");

                if (ContainsLine(i))
                    for (int j = MinColumn(i); j <= MaxColumn(i); j++)
                    {
                        if (ContainsColumn(i, j))
                            s.Append(_matrix[i][j]);
                        else
                            s.Append(" ");
                        s.Append(" ");
                    }

                s.Append("\n");
                pair = !pair;
            }

            return s.ToString();
        }
    }
}