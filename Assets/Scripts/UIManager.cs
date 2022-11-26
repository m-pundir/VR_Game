using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public void SetVisibility(GameObject theObject, bool visibility)
    {
        theObject.SetActive(visibility);
    }

}
