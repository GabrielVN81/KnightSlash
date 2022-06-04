using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    public Slider slider;

    //Establece la vida maxima, es decir, establece la longitud maxima del slider
    public void SetMaxHealth(int health) 
    {
        slider.maxValue = health;
        slider.value = health;
    }

    //Establece el valor actual del slider respecto al valor maximo
    public void SetHealth(int health) 
    {
        slider.value = health;
    }
}
