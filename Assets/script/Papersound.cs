using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Papersound : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlaySound2D("Paper");
    }
}