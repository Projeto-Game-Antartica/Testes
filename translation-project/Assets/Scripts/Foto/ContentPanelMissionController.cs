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

    private ReadableTexts readableTexts;
    
    private void Start()
    {
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
        if (Parameters.ISWHALEIDENTIFIED)
        {
            CheckWhaleName();
        }
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
        gameObject.SetActive(false);
    }

    public void CheckWhaleName()
    {
        if (Parameters.HIGH_CONTRAST) HighContrastText.ChangeTextBackgroundColor();

        string whale_name = whaleController.getWhaleById(Parameters.WHALE_ID).whale_name;

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
            saveButton.interactable = true;
        }
    }

    public void ClearNameInputField()
    {
        whaleNameInput.text = string.Empty;
    }
}
