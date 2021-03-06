﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TailMissionSceneManager : AbstractScreenReader
{

    public MJInstructionInterfaceController instructionInterface;
    public Button audioButton;
    public LifeExpController lifeExpController;

    public CameraOverlayMissionController cameraOverlayController;

    public AudioSource audioSource;
    public AudioClip avisoClip;
    public AudioClip closeClip;

    public GameObject panelContent;
    public GameObject confirmQuit;
    public GameObject panelBorderAumentado;
    public ButtonSelectionHorizontalController buttonSelectionHorizontalController;

    private bool isOnMenu = false;

    // Update is called once per frame
    void Update()
    {
        if (panelContent.activeSelf && !confirmQuit.gameObject.activeSelf)
        {
            // allow catalog navigation
            buttonSelectionHorizontalController.enabled = true;
            ReadText("Catálogo de fotos");
        }
        else
        {
            // menu's navigation
            buttonSelectionHorizontalController.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (instructionInterface.gameObject.activeSelf)
            {
                instructionInterface.gameObject.SetActive(false);
                audioSource.PlayOneShot(closeClip);
            }
            else if (panelBorderAumentado.activeSelf)
            {
                panelBorderAumentado.SetActive(false);
                audioSource.PlayOneShot(closeClip);
            }
            else
            {
                TryReturnToShip();
            }
        }

        if(!ContentPanelMissionController.isOnInputfield)
        {
            if (Input.GetKeyDown(InputKeys.MJMENU_KEY))
            {
                if (!isOnMenu)
                {
                    audioButton.Select();
                    isOnMenu = true;
                }
                else
                {
                    cameraOverlayController.GetComponentInChildren<Button>().Select();
                    isOnMenu = false;
                }
            }

            if (Input.GetKeyDown(InputKeys.PARAMETERS_KEY))
            {
                lifeExpController.ReadHPandEXP();
            }
        }

        if (Input.GetKeyDown(InputKeys.INSTRUCTIONS_KEY))
        {
            if (!instructionInterface.gameObject.activeSelf)
            {
                instructionInterface.gameObject.SetActive(true);
                instructionInterface.ReadInstructions();
                instructionInterface.GetComponentInChildren<Button>().Select();
            }
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(ScenesNames.M004TailMission);
    }

    public void TryReturnToShip()
    {
        audioSource.PlayOneShot(avisoClip);

        confirmQuit.SetActive(true);

        ReadText(ReadableTexts.instance.GetReadableText(ReadableTexts.key_gameplay_aviso_botoes, LocalizationManager.instance.GetLozalization()));

        ReadText(confirmQuit.GetComponentInChildren<TMPro.TextMeshProUGUI>().text);
        confirmQuit.GetComponentInChildren<Button>().Select();
    }

    public void ReturnToShip()
    {
        confirmQuit.SetActive(false);
        SceneManager.LoadScene(ScenesNames.M004Ship);
    }

    public IEnumerator ReturnToShipCoroutine()
    {
        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene(ScenesNames.M004Ship);
    }

    public void ReturnToMJ()
    {
        confirmQuit.SetActive(false);
    }
}
