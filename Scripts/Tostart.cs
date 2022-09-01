using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tostart : MonoBehaviour
{

    public void ToStartButten()
    {
        SceneManager.LoadSceneAsync("Main");
    }
}