﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : AbstractScreenReader {

    public GameObject panelInstruction;
    public GameObject panelContent;
    public GameObject cameraOverlaySprites;

    public AudioClip cameraBeep;
    public AudioSource audioSource;

    public UnityEngine.UI.Button dirButton;
    public UnityEngine.UI.Button esqButton;
    public UnityEngine.UI.Button cimaButton;
    public UnityEngine.UI.Button baixoButton;

    public Sprite seta_dir_preta;
    public Sprite seta_dir_amarela;
    public Sprite seta_cima_preta;
    public Sprite seta_cima_amarela;
    
    private const float SPEED = 70.0f;
    
    /*
     * Startup Settings
     */
    private void Awake()
    {
        // camera doesnt start at any border
        Parameters.RIGHT_BORDER = false;
        Parameters.LEFT_BORDER  = false;
        Parameters.UP_BORDER    = false;
        Parameters.DOWN_BORDER  = false;
    }

    // Update is called once per frame
    void Update ()
    {
        // camera se movimenta quando os paineis estao desabilitados
        if (!panelInstruction.activeSelf && !panelContent.activeSelf)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                MoveUp();
                cimaButton.image.sprite = seta_cima_amarela;
            }
            else
            {
                cimaButton.image.sprite = seta_cima_preta;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                MoveDown();
                baixoButton.image.sprite = seta_cima_amarela;
            }
            else
            {
                baixoButton.image.sprite = seta_cima_preta;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            { 
                MoveLeft();
                esqButton.image.sprite = seta_dir_amarela;
            }
            else
            {
                esqButton.image.sprite = seta_dir_preta;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            { 
                MoveRight();
                dirButton.image.sprite = seta_dir_amarela;
            }
            else
            {
                dirButton.image.sprite = seta_dir_preta;
            }
        }
    }
    
    public void MoveRight()
    {
        if (!audioSource.isPlaying && Parameters.ACCESSIBILITY) audioSource.PlayOneShot(cameraBeep);

        if (transform.position.x >= Parameters.RIGHT_LIMIT)
        {
            transform.position = new Vector3(Parameters.RIGHT_LIMIT, transform.position.y, Parameters.Z_POSITION);
            Parameters.RIGHT_BORDER = true;
        }
        else
        {
            transform.position += new Vector3(SPEED * Time.deltaTime, 0, 0);
            Parameters.LEFT_BORDER = false;
        }
    }

    public void MoveLeft()
    {
        if (!audioSource.isPlaying && Parameters.ACCESSIBILITY) audioSource.PlayOneShot(cameraBeep);

        if (transform.position.x <= Parameters.LEFT_LIMIT)
        {
            transform.position = new Vector3(Parameters.LEFT_LIMIT, transform.position.y, Parameters.Z_POSITION);
            Parameters.LEFT_BORDER = true;
        }
        else
        {
            transform.position += new Vector3(-SPEED * Time.deltaTime, 0, 0);
            Parameters.LEFT_BORDER = false;
        }
    }

    public void MoveUp()
    {
        if (!audioSource.isPlaying && Parameters.ACCESSIBILITY) audioSource.PlayOneShot(cameraBeep);

        if (transform.position.y >= Parameters.UP_LIMIT)
        {
            transform.position = new Vector3(transform.position.x, Parameters.UP_LIMIT, Parameters.Z_POSITION);
            Parameters.UP_BORDER = true;
        }
        else
        {
            transform.position += new Vector3(0, SPEED * Time.deltaTime, 0);
            Parameters.UP_BORDER = false;
        }
    }

    public void MoveDown()
    {
        if (!audioSource.isPlaying && Parameters.ACCESSIBILITY) audioSource.PlayOneShot(cameraBeep);

        if (transform.position.y <= Parameters.DOWN_LIMIT)
        {
            transform.position = new Vector3(transform.position.x, Parameters.DOWN_LIMIT, Parameters.Z_POSITION);
            Parameters.DOWN_BORDER = true;
        }
        else
        {
            transform.position += new Vector3(0, -SPEED * Time.deltaTime, 0);
            Parameters.DOWN_BORDER = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter");
        Parameters.ISWHALEONCAMERA = true;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("trigger stay");
        Parameters.ISWHALEONCAMERA = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("trigger exit");
        Parameters.ISWHALEONCAMERA = false;
    }
}
