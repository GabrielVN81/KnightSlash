using UnityEngine;
using UnityEngine.SceneManagement;
 
public class SceneSwitcher : MonoBehaviour
{

    public void OpenScene(int index) {
        SceneManager.LoadScene(index);
    }
}
