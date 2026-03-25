using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] int initialSize = 50;
    
    Stack<GameObject> _poolStack;
    Transform _poolContainer;

    public void Init()
    {
        _poolStack = new Stack<GameObject>();
        _poolContainer = new GameObject("BallPoolContainer").transform;
        
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewBall();
        }
    }

    GameObject CreateNewBall()
    {
        GameObject obj = Instantiate(ballPrefab, _poolContainer);
        obj.SetActive(false);
        _poolStack.Push(obj);
        return obj;
    }

    public GameObject Get()
    {
        if (_poolStack.Count > 0)
        {
            GameObject obj = _poolStack.Pop();
            obj.SetActive(true);
            return obj;
        }

        return CreateNewBall();
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _poolStack.Push(obj);
    }
}