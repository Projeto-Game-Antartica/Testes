﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageMenu : AbstractMenu {

    private Button brButton;

    private const string instructions = "Selecione o idioma do jogo. Há dois botões, o primeiro com a bandeira do Brasil" +
                                        "referindo-se ao idioma portugûes e o segundo uma bandeira do Reino Unido" +
                                        "referindo-se ao idioma inglês. Utilize as setas para cima ou baixo ou a tecla TAB" +
                                        "para navegar pelos botões. Utilize a tecla ENTER para selecioná-los.";

    private void Start()
    {

        brButton = GameObject.Find("locales-ptbr").GetComponent<Button>();

        TolkUtil.Load();

        TolkUtil.Instructions();
        TolkUtil.Speak(instructions);

        brButton.Select();  
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            TolkUtil.Speak(instructions);
        }
    }
}
