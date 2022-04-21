using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class go_pve : MonoBehaviour
{

    public void OnClickStartButton()
    {
        SceneManager.LoadScene("PvE");
    }
}
