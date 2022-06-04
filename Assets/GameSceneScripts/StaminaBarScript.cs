using UnityEngine;
using UnityEngine.UI;

public class StaminaBarScript : MonoBehaviour
{
    public Slider slider;

    //Establece la stamina maxima, es decir, establece la longitud maxima del slider
    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }

    //Establece el valor actual del slider respecto al valor maximo
    public void SetStamina(int stamina)
    {
        slider.value = stamina;
    }
}
