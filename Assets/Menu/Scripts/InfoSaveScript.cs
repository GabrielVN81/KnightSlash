using UnityEngine;
using TMPro;

public class InfoSaveScript : MonoBehaviour
{
    public TMP_Text textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        PlayerData pData = SaveManager.LoadPlayerData();
        if (pData == null)
        {
            this.gameObject.SetActive(false);
        } else
        {
            textMeshPro.SetText("Partida guardada.\nSalud actual: "+pData.salud+"hp");
        }
    }

}
