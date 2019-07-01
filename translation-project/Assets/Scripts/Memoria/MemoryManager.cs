﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryManager : MonoBehaviour {

    public Sprite[] cardFace;
    public Sprite[] cardText;
    public Sprite cardBack;

    public GameObject[] cards;

    public Button confirmarButton;
    public Button cancelarButton;

    public int[] index; 
    private int matches = 9;
    private int miss = 0;

    private bool init = false;

    public static int CARDFACE = 1;
    public static int CARDTEXT = 2;

    public Text missText;
    public Text matchesText;

    private AudioSource audioSource;

    public AudioClip correctAudio;
    public AudioClip wrongAudio;

    private List<int> c;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update () {
        if (!init)
            initializeCards();

        if ((Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Space)) && !Card.DO_NOT)
        {
            checkCards();
        }

        if (c != null && c.Count >= 2)
        {
            Card.DO_NOT = true;
            confirmarButton.interactable = true;
            cancelarButton.interactable = true;

            Debug.Log(c.Count);
        }
        else
        {
            confirmarButton.interactable = false;
            cancelarButton.interactable = false;
        }
        
        Debug.Log(Card.DO_NOT);
    }

    void initializeCards()
    {
        // first 9 cards with images
        for (int i = 1; i < 10; i++)
        {
            bool test = false;
            int choice = 0;
            while (!test)
            {
                choice = Random.Range(0, cards.Length);
                test = !(cards[choice].GetComponent<Card>().initialized);
            }

            cards[choice].GetComponent<Card>().cardValue = i;
            cards[choice].GetComponent<Card>().initialized = true;

            //Debug.Log(choice);
            
            cards[choice].GetComponent<Card>().setupGraphics(CARDFACE);

        }

        // last 9 images with text
        for (int i = 1; i < 10; i++)
        {
            bool test = false;
            int choice = 0;
            while (!test)
            {
                choice = Random.Range(0, cards.Length);
                test = !(cards[choice].GetComponent<Card>().initialized);
            }

            cards[choice].GetComponent<Card>().cardValue = i;
            cards[choice].GetComponent<Card>().initialized = true;

            //Debug.Log(choice);

            cards[choice].GetComponent<Card>().setupGraphics(CARDTEXT);
        }
        
        
        if (!init)
            init = true;
    }

    public Sprite getCardBack()
    {
        return cardBack;
    }

    public Sprite getCardFace(int i)
    {
        return cardFace[i-1];
    }

    public Sprite getCardText(int i)
    {
        return cardText[i-1];
    }

    void checkCards()
    {
        c = new List<int>();

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].GetComponent<Card>().state == 1)
                c.Add(i);
        }

        //Debug.Log(c.Count);
        //if (c.Count == 2)
        //    cardComparison(c);
    }

    public void CompareCards()
    {
        cardComparison(c);
    }

    public void Cancel()
    {
        Debug.Log(c.Count);

        for (int i = 0; i < c.Count; i++)
        {
            cards[c[i]].GetComponent<Card>().state = 0;
            cards[c[i]].GetComponent<Card>().turnCardDown();
        }
    }

    void cardComparison(List<int> c)
    {
        Card.DO_NOT = true;

        int x = 0;

        if(cards[c[0]].GetComponent<Card>().cardValue == cards[c[1]].GetComponent<Card>().cardValue)
        {
            audioSource.PlayOneShot(correctAudio);

            x = 2;
            matches--;
            matchesText.text = "Pares restantes: " + matches;
            if (matches == 0)
            {
                Debug.Log("Fim de Jogo!!");
            }
        }
        else
        {
            audioSource.PlayOneShot(wrongAudio);

            miss++;
            missText.text = "Tentativas incorretas: " + miss;
        }

        for(int i = 0; i<c.Count; i++)
        {
            cards[c[i]].GetComponent<Card>().state = x;
            cards[c[i]].GetComponent<Card>().falseCheck();
        }

        c.Clear();
    }
}
