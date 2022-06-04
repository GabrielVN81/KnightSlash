using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BackScript : MonoBehaviour, IPointerClickHandler
{

    public TMP_Text textMeshPro;

    public void OnPointerClick(PointerEventData eventData) // 3
    {
        Debug.Log("Back request");
    }

}