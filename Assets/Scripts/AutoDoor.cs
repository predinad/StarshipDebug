using System;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    private bool powerOn = true;
    private Animator animator;
    private BoxCollider2D collider1;
    private AudioSource audioSource;
    private string soundFileName = "Door1"; 
    AudioClip loadedClip;


    void Awake()
    {
        // Automatically get the BoxCollider2D component on this GameObject
        collider1 = GetComponent<BoxCollider2D>();
        if (collider1 == null)
        {
            Debug.LogError("No BoxCollider found on this GameObject!");
        }

        //and the same for the animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("AutoDoor is missing an Animator component!");
        }
        
        //audio file loading
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on this GameObject!");
            enabled = false;
        }

        // Load the AudioClip from the Resources folder
        loadedClip = Resources.Load<AudioClip>(soundFileName);
        if (loadedClip != null)
        {
            Debug.Log($"Loaded audio clip: {loadedClip.name}");
        }
        else
        {
            Debug.LogError($"Could not load audio clip '{soundFileName}' from the Resources folder!");
        }
    }
    void Start()
    {

    }

    // Enables the BoxCollider on this GameObject.
    public void Close()
    {
        if (!powerOn){
            return;
        }
        
        animator.SetBool("PlayerNear", false);

        if (collider1 != null)
        {
            collider1.enabled = true;
        }
    }


    // Disables the BoxCollider on this GameObject.
    public void Open()
    {
        if (!powerOn){
            return;
        }
        animator.SetBool("PlayerNear", true);


        if (collider1 != null)
        {
            collider1.enabled = false;
        }
    }

    public void PlaySoundEffect()
    {
        Debug.LogWarning("sound effect called.");
        if (audioSource != null && loadedClip != null)
        {
            audioSource.PlayOneShot(loadedClip);
        }
        else
        {
            Debug.LogWarning("AudioSource not found or AudioClip not loaded.");
        }
    }



    public void PowerOn()
    {
        powerOn = true;
    }
    public void PowerOff()
    {
        powerOn = false;
    }

}