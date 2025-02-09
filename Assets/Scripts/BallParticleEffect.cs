// /*
// Created by Darsan
// */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallParticleEffect : MonoBehaviour
{
    [SerializeField]protected List<ParticleSystem> _colorParticles = new List<ParticleSystem>();


    public Color Color
    {
        get => _colorParticles.FirstOrDefault()?.main.startColor.color ?? Color.white;
        set => _colorParticles.ForEach(system =>
        {
            var main = system.main;
            main.startColor = value;
        });
    }



}