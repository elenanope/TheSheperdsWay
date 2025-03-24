using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameManager : MonoBehaviour
{

    void Start()
    {
        GameManager.Instance.appearingOfSheeps = true;
        GameManager.Instance.gameObject.SetActive(false);
        GameManager.Instance.gameObject.SetActive(true);
        Debug.Log("Se ha reseteado el game manager");
    }
}
