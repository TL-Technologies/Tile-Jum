using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUI : MonoBehaviour
{
    [SerializeField] private Text _startLvlTxt, _endLvlTxt;
    [SerializeField] private Slider _slider;

    private int _level;
    private float _progress;

    public int Level
    {
        get => _level;
        set
        {
            _startLvlTxt.text = value.ToString();
            _endLvlTxt.text = (value+1).ToString();
            _level = value;
        }
    }

    public float Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            _slider.normalizedValue = value;
        }
    }
}
