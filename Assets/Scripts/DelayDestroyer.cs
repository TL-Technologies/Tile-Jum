using System;
using System.Collections;
using UnityEngine;

public class DelayDestroyer : MonoBehaviour
{
    [SerializeField] private Type _type;

    [ConditionalField(nameof(_type), Type.Particle)] [SerializeField]
    private ParticleSystem _particleSystem;

    [ConditionalField(nameof(_type), Type.Custom)] [SerializeField]
    private float _delay;

    [ConditionalField(nameof(_type), Type.Animator)] [SerializeField]
    private Animator _anim;

    private void Awake()
    {
        switch (_type)
        {
            
            case Type.Particle:
                if (!_particleSystem)
                    _particleSystem = GetComponent<ParticleSystem>();
                break;
            case Type.Animator:
                if (!_anim)
                    _anim = GetComponent<Animator>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
//        Debug.Log(GetTime());
        yield return SimpleCoroutine.DelayEnumerator(GetTime());
        Destroy(gameObject);
    }

    private float GetTime()
    {
        switch (_type)
        {
            case Type.Custom:
                return _delay;
            case Type.Particle:
                return _particleSystem.main.duration;
            case Type.Animator:
                return _anim.GetCurrentAnimatorStateInfo(0).length /
                       _anim.GetCurrentAnimatorStateInfo(0).speedMultiplier;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public enum Type
    {
        Custom,
        Particle,
        Animator
    }
}