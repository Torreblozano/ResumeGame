using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal2D : MonoBehaviour
{
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    [SerializeField] private Vector3 playerPos;
    [SerializeField] private string backGroundmusicKey;
    public void TPPlayer()
    {
        GameManager.Instance.TpPlayer(minBounds,maxBounds,playerPos, backGroundmusicKey);
    }
}
