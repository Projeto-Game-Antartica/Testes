﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : AbstractScreenReader {

    // Use this for initialization
    private IEnumerator Start () {
        
        // accessibility parameters start disabled
        Parameters.ACCESSIBILITY = false;
        Parameters.HIGH_CONTRAST = false;
        Parameters.BOLD = false;

        // change button color
        Parameters.BUTTONCONTRAST = true;

        //TolkUtil.Load();

        //ReadText("Jogo Expedição Antártica versão 1.0. O jogo está carregando...");
        //Debug.Log("Jogo Expedição Antártica versão 1.0. O jogo está carregando...");

        // set resolution to one of "accepted" by the game
        Screen.SetResolution(1024, 768, true);

        yield return new WaitForSeconds(2f);

        while (!LocalizationManager.instance.GetIsReady())
        {
            Debug.Log("loading...");
            yield return null;
        }

        Debug.Log("loaded");
        SceneManager.LoadScene(ScenesNames.Login);
        //SceneManager.LoadScene(ScenesNames.Menu);
    }

}
