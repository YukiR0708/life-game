using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Cell�̏�� </summary>
public enum CellState
{
    Alive,
    Dead, // or None
}
public class Cell : MonoBehaviour
{
    [SerializeField] private CellState _currentCellState = CellState.Dead;
    public CellState CurrentState { get => _currentCellState; set => _currentCellState = value; }
    LifeGame _lifeGame = default;
    /// <summary> 0�X�^�[�g�ŃC���f�b�N�X������ </summary>
    int[] index = new int[2];
    public int[] Index { get => index; set => index = value; }
    Image _image = default;
    [Tooltip("���̐���Ő����c��邩�ǂ���")]
    bool _canAliveNextGene = false;
    public bool CanAliveNextGene { get => _canAliveNextGene; set => _canAliveNextGene = value; }
    List<Cell> _aroundCells = new List<Cell>();
    public List<Cell> AroundCells{ get => _aroundCells; set => _aroundCells = value; }

    void Start()
    {
        _image = GetComponent<Image>();
        _lifeGame = GetComponentInParent<LifeGame>();
    }

    /// <summary> Cell�̐����ɍ��킹�ĐF��ς��� </summary>
    public void OnCellStateChanged()
    {
        _image.color = _currentCellState == CellState.Dead ? Color.white : Color.black;
    }

}
