using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum GameState
{
    Writing,
    Auto,
    Stop,
}

public class LifeGame : MonoBehaviour, IPointerClickHandler
{
    //*****セル生成関連*****
    [SerializeField] uint _rows = 1;
    [SerializeField] uint _columns = 1;
    [SerializeField] Cell _cellPrefab = default;
    GridLayoutGroup _gridLayoutGroup = default;
    Cell[,] _cells;
    public Cell[,] Cells { get => _cells; }

    //*****セル判定関連*****
    [Tooltip("次の世代に移行するフラグ")]
    bool _canStepGenerate = true;


    [SerializeField] private GameState _currentGameState = GameState.Writing;
    public GameState CurrentGameState { get => _currentGameState; set => _currentGameState = value; }

    void Start()
    {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = (int)_columns;
        GenerateCells();
        //すべてのセルに自分の周りのセルを持たせる
        foreach (var cell in _cells)
        {
            cell.AroundCells = SearchAroundCell(cell);
        }
    }

    /// <summary> セルを生成する </summary>
    void GenerateCells()
    {
        _cells = new Cell[_rows, _columns];
        var parent = _gridLayoutGroup.transform;
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                cell.Index = new int[2] { r, c };
                _cells[r, c] = cell;
            }
        }
    }

    /// <summary> ボタンから呼び、GameStatusを変える </summary>
    public void ChangeGameState(string state)
    {
        GameState _nextState = (GameState)Enum.Parse(typeof(GameState), state);
        if (_nextState == GameState.Auto)
        {
            _nextState = _currentGameState == GameState.Auto ? GameState.Writing : GameState.Auto;
        }
        _currentGameState = _nextState;
    }

    /// <summary> マウスクリックでセルのステータスを変える </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        var target = eventData.pointerCurrentRaycast.gameObject;
        if ((_currentGameState == GameState.Writing))
        {
            if (target.TryGetComponent<Cell>(out var cell))
            {
                if (cell.CurrentState == CellState.Alive) cell.CurrentState = CellState.Dead;
                else cell.CurrentState = CellState.Alive;
                cell.OnCellStateChanged();
            }

        }

    }

    /// <summary> 周りのセルを返す </summary>
    /// <param name="checkCell"></param>
    /// <returns></returns>
    List<Cell> SearchAroundCell(Cell checkCell)
    {
        int[] index = new int[2] { checkCell.Index[0], checkCell.Index[1] };
        List<Cell> aroundCells = new List<Cell>();

        for (int aroundR = index[0] - 1; aroundR <= index[0] + 1; aroundR++)
        {
            if (aroundR < 0 || _rows <= aroundR) continue;
            for (int aroundC = index[1] - 1; aroundC <= index[1] + 1; aroundC++)
            {
                if (aroundC < 0 || _columns <= aroundC) continue;
                if (aroundR == index[0] && aroundC == index[1]) continue;
                var addedCell = Cells[aroundR, aroundC];
                aroundCells.Add(addedCell);
            }
        }
        return aroundCells;
    }
    private void Update()
    {
        if (CurrentGameState == GameState.Auto)
        {
            if (_canStepGenerate)
            {
                _canStepGenerate = false;
                StartCoroutine(AutoTimer());
            }
        }
    }

    /// <summary> 次の世代にステップする </summary>
    /// <returns>引数を指定した場合は、n世代後までスキップする</returns>
    public void StepGeneration(int n = 1)
    {
        for (int i = 0; i < n; i++)
        {
            foreach (var cell in _cells)
            {
                cell.CanAliveNextGene = JudgeDeadOrAlive(cell);
            }
            foreach (var cell in _cells)
            {
                cell.CurrentState = cell.CanAliveNextGene ? CellState.Alive : CellState.Dead;
            }
        }

        foreach (var cell in _cells) cell.OnCellStateChanged();


    }

    IEnumerator AutoTimer()
    {
        StepGeneration();
        yield return new WaitForSeconds(1);
        _canStepGenerate = true;
    }

    /// <summary> 生死を判定する </summary>
    /// <returns></returns>
    bool JudgeDeadOrAlive(Cell checkCell)
    {
        int aroundAliveCellsCount = 0;
        bool canAliveNextGene = false; //次の世代で生きられるか
        foreach (var aroundCell in checkCell.AroundCells)
        {
            if (aroundCell.CurrentState == CellState.Alive) aroundAliveCellsCount++;
        }
        //死んでいるとき
        if (checkCell.CurrentState == CellState.Dead)
        {
            //誕生できるかどうか判定
            if (aroundAliveCellsCount == 3)
            {
                canAliveNextGene = true;
            }
        }
        else //生きているとき
        {
            //生き残れるかどうか判定
            if (aroundAliveCellsCount < 2 || aroundAliveCellsCount > 3) canAliveNextGene = false;
            else canAliveNextGene = true;
        }

        return canAliveNextGene;

    }



}
