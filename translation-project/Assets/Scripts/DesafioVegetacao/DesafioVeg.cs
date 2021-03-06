﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DesafioVeg : AbstractScreenReader
{

    private string currentDescription;

    //Scenario
    public GameObject Plant, PlantDetached, PlantToolBox, BowlFull, BagFull, FramePlaced, BowlPlaced, OkDialog, AnalysisScreen, GameScreen;

    public TMPro.TMP_InputField HarvestTime, HarvesterName, HarvestLatitude, HarvestLongitude, HarvestNumber, HarvestLocation;

    public OkDialog OkDialogBox;

    Action okDialogCallback;

    public GameObject instruction_interface;

    public HUDMJController hud;

    public Selectable FirstTool, SecondTool, ThirdTool, ForthTool;

    public Button audioButton;
    public Button librasButton;
    public Button resetButton;
    public Button backButton;

    //Tools
    public GameObject[] Tools = new GameObject[6];
    private int currentToolIndex;

    public TMPro.TMP_Text GameCommandsText;

    string gameCommands = @"1- Para acessar as duas áreas da tela (área do solo da vegetação; área dos itens): tecla F6
2- Quando iniciar o desafio, o foco estará na área dos itens. 
3- Navegação na área dos itens: teclas direcionais (direita e esquerda)
4- Navegação na área do solo da vegetação: teclas direcionais (direita, esquerda, cima e baixo)
5- Realizar coleta: tecla espaço ou tecla enter 
6- Navegar pontuação (HP/XP): tecla A
7- Pausar o jogo e redirecionar para menu: tecla P";

    private enum Tool {
        Frame,
        Knife,
        Spatula,
        SpatulaWithPlant,
        Bowl,
        Bag,
        None=-1
    }

    public GameObject[] Grid = new GameObject[16];

    private int selectedGridIndex;

    private int plantIndex;
    private int bowlIndex;

    private int[] possibleBowlStartIndexes = new int[] {
        // 0, 1, 2, 3,
        // 4, 7,
        // 8, 11, 
        // 12, 13, 14, 15
        3
    };

    private int doneHarvest = 0;
    private int correctClassification = 0;

    private GameState currentGameState = GameState.Initial;

    private System.Random rnd = new System.Random();

    private enum GameState {
        Initial,
        PlantFixed,
        FramePlaced,
        PlantDetached,
        SpatulaWithPlant,
        BowlPlaced,
        PlantInBowl,
        BowlInBag
    }

    // must implement sounds of warnings and button press

    // Start is called before the first frame update
    void Start() {
        ResetHarvestScreen();        
    }

    private bool isAnySelected(params Selectable[] selectables) {
        foreach(Selectable s in selectables) {
            if(s.gameObject == EventSystem.current.currentSelectedGameObject)
                return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update() {

        if(selectedGridIndex > -1 && currentToolIndex > -1) {
            Tools[currentToolIndex].transform.SetParent(Grid[selectedGridIndex].transform, false);
        }

        if (ActionInput.GetKeyDown(KeyCode.F1))
            instruction_interface.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(instruction_interface.activeSelf)
                instruction_interface.SetActive(false);
            else
                hud.TryQuit();
        }

        if (ActionInput.GetKeyDown(KeyCode.F6)) {
            if(isAnySelected(FirstTool, SecondTool, ThirdTool, ForthTool))
                Grid[selectedGridIndex].GetComponent<Button>().Select();
            else
                FirstTool.Select();
        }

        if (Input.GetKeyDown(InputKeys.REPEAT_KEY) && currentDescription != null) {
            ReadText(currentDescription);
        }


        if(Input.GetKeyDown(InputKeys.MJMENU_KEY))
        {
            // if(isAnySelected(audioButton, librasButton, resetButton, backButton))
            //     FirstTool.Select();
            // else if(isAnySelected(FirstTool, SecondTool, ThirdTool, ForthTool))
            //     Grid[selectedGridIndex].GetComponent<Button>().Select();
            // else
                audioButton.Select();
        }

    }

    public void StartGame() {
        FirstTool.Select();
        resetButton.interactable = true;
        
        //PlayerPreferences.M010_Desafio_Done = true;

        currentDescription = ReadableTexts.instance.GetReadableText("m010_desafio_screen", LocalizationManager.instance.GetLozalization());
        ReadText(currentDescription);
    }

    public void ResetHarvestScreen() {
        

        GameCommandsText.text = gameCommands;
        
        selectedGridIndex = 0;
        plantIndex = rnd.Next(16);
        do //Find a place to the bowl different from the vegetation place
            bowlIndex = possibleBowlStartIndexes[rnd.Next(possibleBowlStartIndexes.Length)];
        while(bowlIndex == plantIndex);

        currentToolIndex = -1;

        changeGameState(GameState.PlantFixed);

        FirstTool.Select();
    }

    private void changeTool(int toolIndex) {
        currentToolIndex = toolIndex;
        
        foreach(GameObject tool in Tools)
            tool.SetActive(false); 

        if(toolIndex > -1) {
            //Tools[toolIndex].transform.SetParent(Grid[selectedGridIndex].transform, false);
            Tools[toolIndex].SetActive(true);

            Grid[selectedGridIndex].GetComponent<Button>().Select();
        }
    }

    private void changeTool(Tool tool) {
        changeTool((int)tool);
    }

    //Function to attempt choose other tool. Depending on game state it will suceed
    public void AttemptChangeTool(int tool) {
        //Later must restrict this
        if((Tool)tool == Tool.Spatula && (currentGameState == GameState.SpatulaWithPlant || currentGameState == GameState.BowlPlaced)) {
            tool = (int)Tool.SpatulaWithPlant;
        }

        if((Tool)tool == Tool.Frame && currentGameState >= GameState.FramePlaced)
            tool = -1;

        if((Tool)tool == Tool.Bowl && bowlIndex > -1)
            tool = -1;

        changeTool(tool);
    }

    private void warningMessage(string message) {
        OkDialogBox.Show(message);
    }

    private void takeAction(int gridIndex) {
        //warningMessage("GRIDCLICK" + gridIndex);

        switch(currentGameState) {
            case GameState.PlantFixed:
                if(currentToolIndex == (int)Tool.Frame && plantIndex == selectedGridIndex) {
                    changeGameState(GameState.FramePlaced);
                    changeTool(-1);              
                } else {
                    warningMessage("Você precisa colocar o quadrante!");
                }
                break;

            case GameState.FramePlaced:
                if(currentToolIndex == (int)Tool.Knife && plantIndex == selectedGridIndex)
                    changeGameState(GameState.PlantDetached);
                else
                    warningMessage("Você precisa cortar a vegetação!");
                break;

            case GameState.PlantDetached:
                if(currentToolIndex == (int)Tool.Spatula && plantIndex == selectedGridIndex) {
                    //changeGameState(GameState.SpatulaWithPlant);
                    changeGameState(GameState.BowlPlaced);
                    changeTool(Tool.SpatulaWithPlant);
                } else
                    warningMessage("Você precisa pegar a vegetação!");
                break;

            case GameState.SpatulaWithPlant:
                if(currentToolIndex == (int)Tool.Bowl && plantIndex != selectedGridIndex) {
                    bowlIndex = selectedGridIndex;
                    changeGameState(GameState.BowlPlaced);
                    changeTool(-1);
                } else
                    warningMessage("Você precisa posicionar o potinho!");
                break;

            case GameState.BowlPlaced:
                if(currentToolIndex == (int)Tool.SpatulaWithPlant && selectedGridIndex == bowlIndex) {
                    changeGameState(GameState.PlantInBowl);
                    changeTool(Tool.Spatula);
                } else
                    warningMessage("Você precisa colocar a vegetação no potinho!");
                break;

            case GameState.PlantInBowl:
                if(currentToolIndex == (int)Tool.Bag && selectedGridIndex == bowlIndex) {
                    changeGameState(GameState.BowlInBag);
                    changeTool(-1);
                    finishHarvest();
                } else
                    warningMessage("Você precisa colocar o potinho no saco!");
                break;

            case GameState.BowlInBag:
                break;

            default:
                break;
        }
    }

    private void finishHarvest() {
        doneHarvest++;
        OkDialogBox.Show("Parabéns coleta realizada, agora você precisa classificá-la.", showAnalysisScreen);
    }

    private void showAnalysisScreen() {
        AnalysisScreen.GetComponent<AnalysisVegScreen>().ResetScreen();
        GameScreen.SetActive(false);
        AnalysisScreen.SetActive(true);
    }

    public void ShowHarvestScreen() {
        GameScreen.SetActive(true);
        AnalysisScreen.SetActive(false);
    }



    private void changeGameState(GameState state) {
        Plant.SetActive(false);  
        PlantDetached.SetActive(false); 
        BowlFull.SetActive(false); 
        BagFull.SetActive(false);
        FramePlaced.SetActive(false);
        BowlPlaced.SetActive(false);
        PlantToolBox.SetActive(false);

        currentGameState = state;

        switch(currentGameState) {
            case GameState.Initial:
                break;

            case GameState.PlantFixed:
                Plant.SetActive(true);
                Plant.transform.SetParent(Grid[plantIndex].transform, false);
                BowlPlaced.SetActive(true);
                BowlPlaced.transform.SetParent(Grid[bowlIndex].transform, false);
                break;

            case GameState.FramePlaced:
                FramePlaced.SetActive(true);
                Plant.SetActive(true);
                FramePlaced.transform.SetParent(Grid[plantIndex].transform, false);
                Plant.transform.SetParent(Grid[plantIndex].transform, false);
                FramePlaced.transform.SetSiblingIndex(0);
                BowlPlaced.SetActive(true);
                BowlPlaced.transform.SetParent(Grid[bowlIndex].transform, false);
                break;

            case GameState.PlantDetached:
                FramePlaced.SetActive(true);
                PlantDetached .SetActive(true);
                FramePlaced.transform.SetParent(Grid[plantIndex].transform, false);
                PlantDetached.transform.SetParent(Grid[plantIndex].transform, false);
                BowlPlaced.SetActive(true);
                BowlPlaced.transform.SetParent(Grid[bowlIndex].transform, false);
                break;

            case GameState.SpatulaWithPlant:
                FramePlaced.SetActive(true);
                FramePlaced.transform.SetParent(Grid[plantIndex].transform, false);
                break;

            case GameState.BowlPlaced:
                PlantToolBox.SetActive(true);
                FramePlaced.SetActive(true);
                FramePlaced.transform.SetParent(Grid[plantIndex].transform, false);
                BowlPlaced.SetActive(true);
                BowlPlaced.transform.SetParent(Grid[bowlIndex].transform, false);
                break;

            case GameState.PlantInBowl:
                FramePlaced.SetActive(true);
                FramePlaced.transform.SetParent(Grid[plantIndex].transform, false);
                BowlFull.SetActive(true);
                BowlFull.transform.SetParent(Grid[bowlIndex].transform, false);
                break;

            case GameState.BowlInBag:
                FramePlaced.SetActive(true);
                FramePlaced.transform.SetParent(Grid[plantIndex].transform, false);
                BagFull.SetActive(true);
                BagFull.transform.SetParent(Grid[bowlIndex].transform, false);
                break;

            default:
                break;

        }
    }


    public void OnTestButtonClick() {
        //Plant.SetActive(false);
        //PlantDetached.SetActive(true);
        //Grid[0].
        //Plant.transform.SetParent(Grid[1].transform, false);
        //changeTool(currentToolIndex + 1);
        //changeGameState(GameState.BowlInBag);
        //ShowOkDialog("teste", finishHarvest);
    }

    public void OnGridClick(int gridIndex) {
        takeAction(gridIndex);
    }

    public void OnGridEnter(int gridIndex) {
        selectGrid(gridIndex);
    }

    public void OnGridExit() {
        //deselectGrid();
    }

    private void selectGrid(int gridIndex) {
        selectedGridIndex = gridIndex;
    }

    private void deselectGrid() {
        selectedGridIndex = -1;
    }

    public void DoAfter(int secs, UnityAction action) {
        StartCoroutine(DoAfterCoroutine(secs, action));
    }

    public IEnumerator DoAfterCoroutine(int secs, UnityAction action) {
        yield return new WaitForSeconds(secs);
        action();
    }

    //Volta para o acampamento
    public void ReturnToCamp() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ScenesNames.M010Camp);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(ScenesNames.M010Desafio);
    }
}
