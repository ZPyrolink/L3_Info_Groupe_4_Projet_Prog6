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

        public DynamicMatrix() => _matrix = new();

        /// <summary>
        /// Line maximum in the matrix
        /// </summary>
        public int MaxLine => _matrix.Keys.Order().ElementAt(_matrix.Keys.Count - 1);

        /// <summary>
        /// Line minimum in the matrix
        /// </summary>
        public int MinLine => _matrix.Keys.Order().First();

        /// <summary>
        /// Column maximum of the line
        /// </summary>
        /// <param name="line">Line whose want the max column</param>
        /// <returns>Return the max column in the line</returns>
        public int MaxColumn(int line) => _matrix[line].Keys.Order().ElementAt(_matrix[line].Keys.Count - 1);

        /// <summary>
        /// Column minimum of the line
        /// </summary>
        /// <param name="line">Line whose want the min column</param>
        /// <returns>Return the min column in the line</returns>
        public int MinColumn(int line) => _matrix[line].Keys.Order().First();

        /// <summary>
        /// Check if the matrix contains the line
        /// </summary>
        /// <param name="line">The line to test</param>
        /// <returns>Return if the line exist in the matrix</returns>
        public bool ContainsLine(int line) => _matrix.ContainsKey(line);

        /// <summary>
        /// Check if the column exist for the line
        /// </summary>
        /// <param name="line">The line to test</param>
        /// <param name="column">The column to test</param>
        /// <returns>Return if the column exist for the this line</returns>
        public bool ContainsColumn(int line, int column) => _matrix[line].ContainsKey(column);

        /// <summary>
        /// Check if a point exist on the matrix
        /// </summary>
        /// <param name="p">The point to test</param>
        /// <returns>Return if the point exist</returns>
        public bool ContainsColumn(Vector2Int p) => ContainsColumn(p.x, p.y);

        /// <summary>
        /// Add the value at the coordonees.
        /// Be careful! If the position given isn't a void it will replace it.
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <param name="coordonees">Position whose we want to add</param>
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

        /// <summary>
        /// Check if we can remove a position ans remove if we can
        /// </summary>
        /// <param name="p">Position to remove</param>
        /// <returns>Return if we have remove the object at the position</returns>
        public bool Remove(Vector2Int p)
        {
            bool remove = ContainsLine(p.x) && ContainsColumn(p) && _matrix[p.x].Remove(p.y);

            if (remove && _matrix[p.x].Count == 0)
                _matrix.Remove(p.x);

            return remove;
        }

        /// <summary>
        /// Find a value in the matrix.
        /// Be careful! We must check before if the position is a void or not.
        /// We can't use this on a void.
        /// </summary>
        /// <param name="coordonnes">Coordonnes of the object</param>
        /// <returns>Return the object at the position</returns>
        [Obsolete("Use the getter of the indexer instead!")]
        public T GetValue(Vector2Int coordonnes) => this[coordonnes];

        /// <summary>
        /// New version of GetValue and SetValue
        /// </summary>
        /// <param name="co">Coordonnes of the object</param>
        /// <returns>Return the object at the position</returns>
        public T this[Vector2Int co]
        {
            get => _matrix[co.x][co.y];
            set
            {
                if (!_matrix.ContainsKey(co.x))
                {
                    _matrix.Add(co.x, new() { { co.y, value } });
                    return;
                }

                if (_matrix[co.x].ContainsKey(co.y))
                    _matrix[co.x][co.y] = value;
                else
                    _matrix[co.x].Add(co.y, value);
            }
        }

        /// <summary>
        /// Enumerator of the matrix.
        /// It will browse the map line by line
        /// </summary>
        /// <returns>An enumerator for the map</returns>
        public IEnumerator<T> GetEnumerator() => _matrix
            .OrderBy(x => x.Key)
            .SelectMany(x => x.Value
                .OrderBy(y => y.Key), (_, y) => y.Value)
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Check if there is something at the position
        /// </summary>
        /// <param name="coordonnes">The position to test</param>
        /// <returns>Return if there is something at the coordonnes</returns>
        public bool IsVoid(Vector2Int coordonnes) =>
            !_matrix.ContainsKey(coordonnes.x) || !_matrix[coordonnes.x].ContainsKey(coordonnes.y);

        /// <summary>
        /// Check if the matrix is empty
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