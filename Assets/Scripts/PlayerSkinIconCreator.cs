// /*
// Created by Darsan
// */

using System;
using System.Collections;
using UnityEngine;

public class PlayerSkinIconCreator : MonoBehaviour
{
    private IEnumerator Start()
    {
        foreach (var playerSkin in ResourceManager.PlayerSkins)
        {
            var go = Instantiate(playerSkin.prefab);
            go.layer = 31;
            go.transform.rotation = Quaternion.AngleAxis(-30, Vector3.right) * Quaternion.AngleAxis(-30, Vector3.up);

            IconCreator.CreateIcon(go, new Vector2(0.35f, 0.35f), 1 << go.layer,
                $"Assets/Sprites/PlayerIcons/{playerSkin.id}_icon.png");
            yield return new WaitForSeconds(0.5f);
            Destroy(go);
            yield return new WaitForSeconds(0.3f);
        }
        Debug.Log("Icons Created!");
    }
}