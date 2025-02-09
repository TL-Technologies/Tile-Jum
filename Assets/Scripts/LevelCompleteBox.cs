using UnityEngine;

public class LevelCompleteBox:MonoBehaviour
{
    [SerializeField] private GameObject _effect;
    public void Complete()
    {
        Instantiate(_effect,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}