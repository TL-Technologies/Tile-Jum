using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerfectFeedback : MonoBehaviour
{
    [SerializeField] private Text _diamondTxt;
    private int _coinsReward;

    public int CoinsReward
    {
        get => _coinsReward;
        set
        {
            _diamondTxt.text = value.ToString();
            _coinsReward = value;
        }
    }
}
