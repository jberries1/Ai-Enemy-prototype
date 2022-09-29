using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeManager : MonoBehaviour
{
    [SerializeField] private int _amountOfChars;
    [SerializeField] private Character _characterPrefab;
    private readonly List<Character> _currentCharacters = new List<Character>();
    private readonly List<Character> _reserved = new List<Character>();
    private System.Action _currentTick;
    private System.Action<Character> _currentEventAction;

    private void Start()
    {
        var grid = GridData.Instance.Grid;
        for (int i = 0; i < _amountOfChars; i++)
        {
            var maxX = grid.GetWidth();
            var maxY = grid.GetHeight();
            var pos = Vector3.zero;
            var x = Random.Range(0, maxX);
            var y = Random.Range(0, maxY);
            var node = grid.GetGridObject(x, y);
            while (!node.isWalkable)
            {
                x = Random.Range(0, maxX);
                y = Random.Range(0, maxY);
                node = grid.GetGridObject(x, y);
            }
            pos = grid.GetWorldPosition(x, y);
            var c = Instantiate(_characterPrefab, pos, Quaternion.identity);
            c.Construct(Random.Range(5f, 12f), Random.Range(0.3f, 1f));
            _currentCharacters.Add(c);
            c.OnDone += OnCharacterDoneMovement;
        }
        //Uncomment if wants case 1

        //for (int i = 0; i < _currentCharacters.Count; i++)
        //{
        //    var maxX = grid.GetWidth();
        //    var maxY = grid.GetHeight();
        //    var pos = grid.GetWorldPosition(Random.Range(0, maxX), Random.Range(0, maxY));
        //    _currentCharacters[i].MoveTo(pos);
        //}

        //Case1
        //_currentEventAction = Case1Event;
        //_currentTick = WalkingAndFight;

        //Case2
        _currentEventAction = Case2Event;
        FindingNearestAndFight();
    }

    private void Update()
    {
        _currentTick?.Invoke();
    }
    
    //Case 1
    private void WalkingAndFight()
    {
        foreach (var c in _currentCharacters)
        {
            if (c.IsFighting || !c.gameObject.activeSelf)
                continue;
            foreach (var c1 in _currentCharacters)
            {
                if (c == c1 || c1.IsFighting || !c1.gameObject.activeSelf)
                    continue;
                if ((c.transform.position - c1.transform.position).sqrMagnitude < 1.5f)
                {
                    c.Fight(c1);
                    c1.Fight(c);
                    break;
                }
            }
        }
    }

    //Case 2
    private void FindingNearestAndFight()
    {
        foreach (var c in _currentCharacters)
        {
            if (c.IsFighting || !c.gameObject.activeSelf || _reserved.Contains(c))
                continue;
            var minDist = float.MaxValue;
            Character currentChar = null;
            foreach (var c1 in _currentCharacters)
            {
                if (c == c1 || c1.IsFighting || !c1.gameObject.activeSelf || _reserved.Contains(c1))
                    continue;
                var dist = (c.transform.position - c1.transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    currentChar = c1;
                }
            }
            if (currentChar == null) 
                continue;
            _reserved.Add(currentChar);
            _reserved.Add(c);
            c.MoveTo(currentChar);
        }
    }

    private void OnCharacterDoneMovement(Character character)
    {
        _currentEventAction?.Invoke(character);
    }

    private void Case1Event(Character character)
    {
        var grid = GridData.Instance.Grid;
        var maxX = grid.GetWidth();
        var maxY = grid.GetHeight();
        var x = Random.Range(0, maxX);
        var y = Random.Range(0, maxY);
        var pos = grid.GetWorldPosition(x, y);
        character.MoveTo(pos);
    }

    private void Case2Event(Character character)
    {
        _reserved.Remove(character); 
        FindingNearestAndFight();
    }
}
