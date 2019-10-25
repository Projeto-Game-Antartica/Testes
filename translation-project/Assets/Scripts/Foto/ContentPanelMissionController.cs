﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// not used
public class ContentPanelMissionController : AbstractScreenReader {
    
    public Button saveButton;
    public InputField whaleNameInput;
    public WhaleController whaleController;
    public GameObject confirmFoto;

    private ReadableTexts readableTexts;

    public TextMeshProUGUI confirmText;

    public TextMeshProUGUI whaleCountText;

    public GameObject WinImage;

    private int count = 0;
    
    private void Start()
    {
        whaleCountText.text = count.ToString();
        //TolkUtil.Load();
        //Parameters.ACCESSIBILITY = true;
        
        readableTexts = GameObject.Find("ReadableTexts").GetComponent<ReadableTexts>();
        ReadInstructions();
        saveButton.Select();
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ReadInstructions();
        }

        // not the best way
        //if (Parameters.ISWHALEIDENTIFIED)
        //{
        //    CheckWhaleName();
        //}
	}
    
    private void OnEnable()
    {
        if (Parameters.HIGH_CONTRAST) HighContrastText.ChangeTextBackgroundColor();
    }

    public void ReadInstructions()
    {
        ReadText(readableTexts.GetReadableText(ReadableTexts.key_foto_catalogDescription, LocalizationManager.instance.GetLozalization()));
    }

    public void Save()
    {
        //whaleController.getWhaleById(Parameters.WHALE_ID).indentified = true;

        if (string.IsNullOrEmpty(whaleNameInput.text))
        {
            Debug.Log("O nome não pode ser vazio!");
            whaleNameInput.Select();
        }
        else
        {
            whaleController.getWhaleById(Parameters.WHALE_ID).whale_name = whaleNameInput.text;
            confirmFoto.SetActive(true);

            count++;

            whaleCountText.text = count.ToString();

            if (count < 4)
            {
                if (!whaleController.getWhaleById(Parameters.WHALE_ID).whale_name.Equals(""))
                    confirmText.text = "Parabéns, baleia identificada. Realize uma nova foto.";
                else
                    confirmText.text = "Parabéns, baleia cadastrada. Realize uma nova foto.";

                StartCoroutine(BackToPhotoCoroutine());
            }
            else 
            {
                StartCoroutine(FinishMissionCoroutine());
            }
            //gameObject.SetActive(false);
        }
    }

    public void CheckWhaleName()
    {
        if (Parameters.HIGH_CONTRAST) HighContrastText.ChangeTextBackgroundColor();

        string whale_name = whaleController.getWhaleById(Parameters.WHALE_ID).whale_name;

        Debug.Log(whale_name);

        if (!whale_name.Equals(""))
        {
            Debug.Log(whale_name);
            whaleNameInput.text = whale_name;
            //whaleNameInput.text = whale_name;
            whaleNameInput.interactable = false;
        }
        else
        {
            whaleNameInput.interactable = true;
            whaleNameInput.Select();
        }
            saveButton.interactable = true;

        //whaleController.getWhaleById(Parameters.WHALE_ID).indentified = true;
    }

    public void ClearNameInputField()
    {
        whaleNameInput.text = string.Empty;
    }

    public void ReturnToShip()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ScenesNames.M004Ship);
    }

    public IEnumerator BackToPhotoCoroutine()
    {
        yield return new WaitForSeconds(2);

        confirmFoto.SetActive(false);
        gameObject.SetActive(false);
    }

    public IEnumerator FinishMissionCoroutine()
    {
        WinImage.SetActive(true);

        yield return new WaitForSeconds(7);

        ReturnToShip();
    }
}
