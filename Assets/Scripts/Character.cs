using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private float _health;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private float _coolDown;
    [SerializeField] private Slider _slider;
    private float _currentHealth;
    private Character _concurent;
    private Coroutine _fightCor;
    private Coroutine _moveCor;
    public event Action<Character> OnDone;
    public bool IsFighting { private set; get; }

    private void Awake()
    {
        _currentHealth = _health;
        _slider.maxValue = _health;
        _slider.value = _currentHealth;
    }

    public void Construct(float damage, float coolDown)
    {
        _damage = damage;
        _coolDown = coolDown;
    }

    public void MoveTo(Vector3 pos)
    {
        if (IsFighting)
            return;
        if (_moveCor != null)
            StopCoroutine(_moveCor);
        var path = GridData.Instance.CalculatePath(transform.position, pos);
        if (path == null)
        {
            OnDone?.Invoke(this);
            return;
        }
        _moveCor = StartCoroutine(MovingCor(path));
    }

    public void MoveTo(Character character)
    {
        IsFighting = true;
        if (_moveCor != null)
            StopCoroutine(_moveCor);
        var path = GridData.Instance.CalculatePath(transform.position, character.transform.position);
        if (path == null)
        {
            OnDone?.Invoke(this);
            return;
        }
        _moveCor = StartCoroutine(MovingCor(path, character));
    }

    public void ApplyDamage(float damage)
    {
        _currentHealth -= damage;
        _slider.value = _currentHealth;
        if (_currentHealth < 0)
            Kill();
    }

    public void Fight(Character character)
    {
        IsFighting = true;
        if (_moveCor != null)
        {
            StopCoroutine(_moveCor);
            _moveCor = null;
        }
        _concurent = character;
        _fightCor = StartCoroutine(FightCor());
    }

    private IEnumerator FightCor()
    {
        while (_concurent.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(_coolDown);
            _concurent.ApplyDamage(_damage);
        }
        IsFighting = false;
        _fightCor = null;
        _concurent = null;
        OnDone?.Invoke(this);
    }

    private IEnumerator MovingCor(List<Vector3> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (i == path.Count - 1)
                break;
            Debug.DrawLine(path[i], path[i + 1], Color.red, 5);
        }
        while(path.Count > 0)
        {
            var point = path[0];
            while ((point - transform.position).sqrMagnitude > 0.1f)
            {
                yield return null;
                transform.position = Vector3.MoveTowards(transform.position, point, _moveSpeed * Time.deltaTime); 
            }
            path.RemoveAt(0);
        }
        _moveCor = null;
        OnDone?.Invoke(this);
    }
    
    private IEnumerator MovingCor(List<Vector3> path, Character character)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (i == path.Count - 1)
                break;
            Debug.DrawLine(path[i], path[i + 1], Color.red, 5);
        }
        while(path.Count > 0)
        {
            var point = path[0];
            while ((point - transform.position).sqrMagnitude > 0.1f)
            {
                yield return null;
                transform.position = Vector3.MoveTowards(transform.position, point, _moveSpeed * Time.deltaTime); 
            }
            path.RemoveAt(0);
        }
        _moveCor = null;
        Fight(character);
        character.Fight(this);
    }

    private void Kill()
    {
        gameObject.SetActive(false);
        _concurent = null;
        IsFighting = false;
        if (_fightCor != null)
            StopCoroutine(_fightCor);
        if (_moveCor != null)
            StopCoroutine(_moveCor);
        _fightCor = null;
        _moveCor = null;
    }
}
