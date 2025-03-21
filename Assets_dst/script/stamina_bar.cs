using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stamina_bar : MonoBehaviour
{

    public Slider stamina;

    public void SetMaxstamina(int maxStamina)
    {
        stamina.maxValue = maxStamina;
        stamina.value = maxStamina;
    }
    public void Setstamina(float currStamina)
    {
        stamina.value = currStamina;
    }
}
