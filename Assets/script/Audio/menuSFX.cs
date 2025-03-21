using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuSFX : MonoBehaviour
{
    public void OnHover() {
        SoundManager.Instance.PlaySound3D("Hover", transform.position);
    }

    public void OnClick() {
        SoundManager.Instance.PlaySound3D("Click", transform.position);
    }

    public void OnClickPlay() {
        SoundManager.Instance.PlaySound3D("Start", transform.position);
    }


}
