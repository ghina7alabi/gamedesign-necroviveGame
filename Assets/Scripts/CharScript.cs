﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class CharScript : MonoBehaviour
{
    public Rigidbody2D playerbody;
    public float speed;
    public float jumpForce;
    float jumps;
    public static int PlayerState;
    public static int sanity;
    public static float intensity;

    GameObject NPC;

    GameObject cameraGO;

    // Start is called before the first frame update
    void Start()
    {
        jumps = 1;
        speed = 10;
        jumpForce = 16.5f;
        PlayerState = 0;
        sanity = 4;

        cameraGO = GameObject.FindGameObjectWithTag("MainCamera");
        intensity = cameraGO.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.value;
        intensity = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(PlayerState);
        if (PlayerState != 2 && PlayerState != 3)
        {
            playerbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), playerbody.velocity.y / speed) * speed;

            if (Input.GetKeyDown(KeyCode.UpArrow) && jumps > 0)
            {
                jumps = 0;
                playerbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.localScale = new Vector3(1, 3, 1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.localScale = new Vector3(-1, 3, 1);
            }
        }
        else
        {
            playerbody.velocity = new Vector2(0, playerbody.velocity.y / speed) * speed;
        }

        if (PlayerState == 1 && Input.GetKeyUp(KeyCode.Space))
        {
            PlayerState = 2;
        }

        if (PlayerState == 2)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                //talk here
                PlayerState = 3;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                if (NPC.GetComponent<NPCScript>().sick == false)
                {
                    sanity --;
                }
                //absorb life here
                PlayerState = 3;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //give life here
                PlayerState = 3;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                sanity++;
                //cancel
                PlayerState = 1;
            }
        }

        if (PlayerState == 3 && Input.GetKeyUp(KeyCode.Space))
        {
            PlayerState = 1;
        }


        //the whole sanity thing
        {
            if (sanity > 4)
            {
                sanity = 4;
            }
            if (sanity == 4)
            {
                if (intensity > 0.2f)
                {
                    intensity = intensity - 0.05f * Time.deltaTime;
                }
                else if (intensity < 0.2f)
                {
                    intensity = intensity + 0.05f * Time.deltaTime;
                }
            }
            else if (sanity == 3)
            {
                if (intensity > 0.325f)
                {
                    intensity = intensity - 0.05f * Time.deltaTime;
                }
                else if (intensity < 0.325f)
                {
                    intensity = intensity + 0.05f * Time.deltaTime;
                }
            }
            else if (sanity == 2)
            {
                if (intensity > 0.425f)
                {
                    intensity = intensity - 0.07f * Time.deltaTime;
                }
                else if (intensity < 0.425f)
                {
                    intensity = intensity + 0.07f * Time.deltaTime;
                }
            }
            else if (sanity == 1)
            {
                if (intensity > 0.575f)
                {
                    intensity = intensity - 0.05f * Time.deltaTime;
                }
                else if (intensity < 0.575f)
                {
                    intensity = intensity + 0.05f * Time.deltaTime;
                }
            }
            else if (sanity < 1)
            {
                if (intensity < 50f)
                {
                    intensity = intensity + 0.1f * Time.deltaTime;
                }
                if (intensity > 0.60f)
                {
                    intensity = intensity + 0.5f * Time.deltaTime;
                }
                if (intensity > 1f)
                {
                    intensity = intensity + 1f * Time.deltaTime;
                }
                if (intensity > 20f)
                {
                    intensity = intensity + 1f * Time.deltaTime;
                }
            }
            if (intensity > 6f)
            {
                SceneManager.LoadScene("GameOverScene");
            }
            cameraGO.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.value = intensity;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Ground" && col.gameObject.transform.position.y < this.gameObject.transform.position.y - 1.9f)
        {
            jumps = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "NPC" && PlayerState == 0)
        {
            PlayerState = 1;
            NPC = col.gameObject;
            Debug.Log(NPC.name);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "NPC" && PlayerState == 0)
        {
            PlayerState = 1;
            NPC = col.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "NPC" && PlayerState == 1)
        {
            PlayerState = 0;
        }
    }
}
