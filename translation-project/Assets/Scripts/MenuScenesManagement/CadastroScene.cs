﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CadastroScene : AbstractScreenReader {

    public UnityEngine.UI.Toggle toggle;
    public void Cadastrar()
    {
        // TO DO
        SceneManager.LoadScene("MenuScene");
    }

    public void Voltar()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void isTermoUso(bool isOn)
    {
        toggle.isOn = isOn;
        Debug.Log(toggle.isOn);
    }
}
