using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] int initialSize = 25;
    
    Stack<GameObject> _poolStack;

    public void Init()
    {
        _poolStack = new Stack<GameObject>();
        
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewBall();
        }
    }

    GameObject CreateNewBall()
    {
        GameObject obj = Instantiate(ballPrefab, transform);
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