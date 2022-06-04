using UnityEngine;
using UnityEngine.SceneManagement;
 
public class SceneSwitcher : MonoBehaviour
{

    //Abre la escena que se le pase por parametro
    public void OpenScene(int index) {
        SceneManager.LoadScene(index);
    }
}
