using System.Collections;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using Taluva.Model;

namespace Taluva.Utils;

public class DynamicMatrix<T> : IEnumerable<T>
{
    private readonly Dictionary<Point, T> matrixP;
    private readonly Dictionary<Point, T> matrixI; // Dans la théorie, cette liste devra ajouter 0.5 à ces coordonées en Y

    public DynamicMatrix()
    {
        matrixI = new Dictionary<Point, T>();
        matrixP = new Dictionary<Point, T>();
    }

    public void Add(T value, Point coordonees)
    {

        if (coordonees.X % 2 == 0)
            if (matrixP.ContainsKey(coordonees))
                matrixP[coordonees] = value;
            else
                matrixP.Add(coordonees, value);
        else
            if(matrixI.ContainsKey(coordonees))
                matrixI[coordonees] = value;
            else
                matrixI.Add(coordonees, value);
    }

    public T GetValue(Point coordonnes) => coordonnes.X % 2 == 0 ? matrixP[coordonnes] : matrixI[coordonnes];

    public IEnumerator<T> GetEnumerator()
    {
        foreach (KeyValuePair<Point, T> p in matrixP.Union(matrixI).OrderBy(key => key.Key))
            yield return p.Value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

}