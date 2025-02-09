using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsFeedback : MonoBehaviour
{

    [SerializeField] private Text _text;
    private int _value;

    public int Value
    {
        get { return _value; }
        set
        {
            _text.text =(value > 0 ? "+": "-") + value;
            _value = value;
        }
    }

    private void Awake()
    {
    }
}
