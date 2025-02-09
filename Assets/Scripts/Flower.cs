// /*
// Created by Darsan
// */

using System;
using UnityEngine;

[DefaultExecutionOrder(2000)]
public class Flower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;

    private void FixedUpdate()
    {
        transform.position = _target.position + _offset;
    }
}