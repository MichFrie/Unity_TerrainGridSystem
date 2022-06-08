using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    public interface ICommand
    {
        void SaveMovement(int targetCell);
        void UndoMovement();
    }
    
    public static CommandManager Instance { get; set; }

    Stack<int> CommandsBuffer;
    void Awake()
    {
        Instance = this;
        CommandsBuffer = new Stack<int>();
    }

    public void AddCell(int lastCell)
    {
        CommandsBuffer.Push(lastCell);
        Debug.Log("New Cell " + lastCell.ToString());
    }
}
    
