using System.Collections;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Taluva.Model;

namespace Taluva.Utils;

public class DynamicMatrix<T> : IEnumerable<T>
{
    private readonly Dictionary<int, Dictionary<int, T>> matrix;
    // Dans la théorie, cette liste devra ajouter 0.5 à ces coordonées en Y quand X % 2 == 1

    public DynamicMatrix()
    {
        matrix = new Dictionary<int, Dictionary<int, T>>();
    }

    public int MaxLine => matrix.Keys.Order().ToArray()[matrix.Keys.Count - 1];

    public int MinLine => matrix.Keys.Order().ToArray()[0];

    public int MaxColumn(int line) => matrix[line].Keys.Order().ToArray()[matrix[line].Keys.Count - 1];

    public int MinColumn(int line) => matrix[line].Keys.Order().ToArray()[0];

    public bool ContainsLine(int line) => matrix.ContainsKey(line);

    public bool ContainsColumn(int line, int column) => matrix[line].ContainsKey(column);

    public void Add(T value, Point coordonees)
    {
        if (!matrix.ContainsKey(coordonees.X)) {
            Dictionary<int, T> tmp = new Dictionary<int, T>();
            tmp.Add(coordonees.Y, value);
            matrix.Add(coordonees.X, tmp);
            return;
        }

        if (matrix[coordonees.X].ContainsKey(coordonees.Y))
            matrix[coordonees.X][coordonees.Y] = value;
        else {
            matrix[coordonees.X].Add(coordonees.Y, value);
        }

    }

    public T GetValue(Point coordonnes) => matrix[coordonnes.X][coordonnes.Y];

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var x in matrix.OrderBy(x => x.Key)) {
            foreach (var y in x.Value.OrderBy(y => y.Key)) {
                yield return y.Value;
            }
        }

    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool IsVoid(Point coordonnes) => matrix.ContainsKey(coordonnes.X) ? !matrix[coordonnes.X].ContainsKey(coordonnes.Y) : true;

    public bool IsEmpty() => matrix.Count == 0;

    override
    public string ToString()
    {
        string s = "";
        bool pair = true;

        for (int i = MinLine; i <= MaxLine; i++) {
            if (!pair)
                s += " ";

            if (ContainsLine(i))
                for (int j = MinColumn(i); j <= MaxColumn(i); j++) {
                    if (ContainsColumn(i,j))
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