using System.Collections;

namespace Taluva.Model;

public class Pile<T>
{
    private Stack<T> pile;
    private List<T> played;

    public Pile(T[] list)
    {
        Random random = new Random();
        list = list.OrderBy(x => random.Next()).ToArray();
        for (int i = 0; i < list.Length; i++)
        {
            pile.Push(list[i]);
        }
        played = new List<T>();
    }

    public T draw()
    {
        T c = pile.Pop();
        played.Add(c);
        return c;
    }
}