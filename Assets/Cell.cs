using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellState
{
    Alive,
    Dead,
}
public class Cell : MonoBehaviour
{
    private CellState _currentState = CellState.Dead;
    public CellState CurrentState{ get => _currentState; set => _currentState = value; }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
