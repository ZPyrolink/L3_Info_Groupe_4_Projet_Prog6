using Taluva.Model;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerMgr : MonoBehaviour
{
    public static PlayerMgr Instance { get; private set; }

    [SerializeField]
    private DebugPlayer[] players =
    {
        new("P1", PlayerColor.Red.GetColor(), new[] { 0, 1, 2 }),
        new("P2", PlayerColor.Green.GetColor(), new[] { 1, 1, 2 }),
        new("P3", PlayerColor.Blue.GetColor(), new[] { 2, 1, 2 }),
        new("P4", PlayerColor.Yellow.GetColor(), new[] { 3, 1, 2 })
    };

    [FormerlySerializedAs("currentPlayerIndex")]
    [SerializeField]
    private int currentIndex;

    public int CurrentIndex
    {
        get => currentIndex;
        set
        {
            if (value >= players.Length)
                currentIndex = 0;
            else if (value < 0)
                currentIndex = players.Length - 1;
            else
                currentIndex = value;
            
            OnPlayerChanged();
        }
    }
    
    public DebugPlayer Current => players[currentIndex];

    public int Length => players.Length;

    public DebugPlayer this[int i] => players[i];

    [SerializeField]
    private UnityEvent playerChanged;

    private void OnPlayerChanged() => playerChanged?.Invoke();

    private void Awake()
    {
        Instance = this;
    }
}