using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dev : MonoBehaviour
{
    bool isActive = false;
    public GameObject testUI;

    public void SetActiveBool()
    {
        isActive = !isActive;
        testUI.SetActive(isActive);
    }
}
