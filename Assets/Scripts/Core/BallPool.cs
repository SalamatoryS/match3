using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] int initialSize = 25;
    
    Stack<GameObject> _poolStack;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
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

        GameObject newObj = Instantiate(ballPrefab, transform);
        newObj.SetActive(true);
        return newObj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _poolStack.Push(obj);
    }
}