// /*
// Created by Darsan
// */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallParticleColorChangeEffect : BallParticleEffect
{
    [SerializeField]private List<ParticleSystem> _oldColorParticles = new List<ParticleSystem>();

    public Color OldColor {
        get => _oldColorParticles.FirstOrDefault()?.main.startColor.color ?? Color.white;
        set => _oldColorParticles.ForEach(system =>
        {
            var main = system.main;
            main.startColor = value;
        });
    }
}