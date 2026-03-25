using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] Text testText;
    public void ChangeText(string text)
    {
        testText.text = text;
    }
}

