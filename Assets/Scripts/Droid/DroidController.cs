using MoonSharp.Interpreter;
using UnityEngine;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using TMPro;


[MoonSharpUserData]
public class DroidController : MonoBehaviour
{
    private float tileSize = 1f;
    private float speed = 4f;
    private bool working = false;
    private ConcurrentQueue<System.Action> actionQueue = new ConcurrentQueue<System.Action>();
    public ConcurrentQueue<string> errors = new ConcurrentQueue<string>();
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 lookDir = Vector2.down;
    private Vector2 targetPos = Vector2.zero;
    private Vector2 lastPos = Vector2.zero;
    private int animDir = 0;
    private int delay = 0;
    public const int MAX_QUEUE_SIZE = 50;
    [SerializeField] private GameObject objectContainer;
    [SerializeField] private GameObject obstacleList;
    [SerializeField] private GameObject itemList;
    [SerializeField] private GameObject outputsList;
    [SerializeField] private UIManager uiManager;
    [SerializeField] public TMP_Text errorMessage;
    [SerializeField] private int targetItems;
    private GameObject holding = null;
    private int droppedItems = 0;

    public AudioSource audioSource;
    public AudioClip dropSound;
    public AudioClip pickUp;
    public AudioClip progressSound;
    public AudioClip winSound;
    public AudioClip blockSound;

    private Vector2 initialPosition  = Vector2.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        targetPos = rb.position;
        lastPos = rb.position;
        initialPosition = rb.position;
    }

    private void FixedUpdate()
    {
        if (errors.Count > 0)
        {

            errors.TryDequeue(out var error);
            Debug.Log(error);
            if (error != null)
                errorMessage.text = error;
        }

        if (!working)
        {
            if (actionQueue.Count == 0)
            {
                return;
            }

            delay--;
            if(delay > 0)
            {
                return;
            }
            
            if(actionQueue.TryDequeue(out var action))
            {
                working = true;
                action.Invoke();
            }
            else
            {
                return;
            }
        }

        if (rb.linearVelocity != Vector2.zero)
        {
            checkStop();
        }

    }

    private void checkStop()
    {
        float distToMove = Vector2.Distance(rb.position, targetPos);
        float distMoved = Vector2.Distance(rb.position, lastPos);
        
        if (distToMove <= 0.01 || distMoved >= tileSize)
        {
            working = false;
            rb.linearVelocity = Vector2.zero;
            rb.position = targetPos;
            return;
        }
    }


    public void Move(ScriptExecutionContext context, CallbackArguments args)
    {

        while (actionQueue.Count >= MAX_QUEUE_SIZE)
        {
            Thread.Sleep(50);
        }

        actionQueue.Enqueue(() =>
        {
            Vector2 lookPos = rb.position + (tileSize * lookDir);
            GameObject lookingAt = FindObjectAt(lookPos);
         
            if (lookingAt != null && lookingAt.activeInHierarchy)
            {
                audioSource.PlayOneShot(blockSound);
                working = false;
                delay = 5;
                return;
            }

            rb.linearVelocity = lookDir * speed;
            lastPos = rb.position;
            targetPos = lookPos;
            delay = 5;
        });
    }

    public void TurnLeft()
    {
        while (actionQueue.Count >= MAX_QUEUE_SIZE)
        {
            Thread.Sleep(50);
        }
        actionQueue.Enqueue(() =>
        {
            animDir = (animDir + 1) % 4;
            animator.SetInteger("direction", animDir);
            lookDir = new Vector2(-lookDir.y, lookDir.x);
            working = false;
            delay = 10;
        });
    }

    public void TurnRight()
    {
        while (actionQueue.Count >= MAX_QUEUE_SIZE)
        {
            Thread.Sleep(50);
        }
        actionQueue.Enqueue(() =>
        {
            animDir = (animDir + 3) % 4;
            animator.SetInteger("direction", animDir);
            lookDir = new Vector2(lookDir.y, -lookDir.x);
            working = false;
            delay = 10;
        });
    }

    public void PickUp()
    {
        while (actionQueue.Count >= MAX_QUEUE_SIZE)
        {
            Thread.Sleep(50);
        }
        actionQueue.Enqueue(() =>
        {
            if (holding != null)
            {
                audioSource.PlayOneShot(blockSound);
                working = false;
                delay = 5;
                return;
            }

            Vector2 lookPos = rb.position + (tileSize * lookDir);
            GameObject lookingAt = FindObjectAt(lookPos, itemList);
            if (lookingAt == null)
            {
                audioSource.PlayOneShot(blockSound);
                working = false;
                delay = 5;
                return;
            }

            holding = lookingAt;
            lookingAt.SetActive(false);
            audioSource.PlayOneShot(pickUp);
            working = false;
            delay = 5;
        });
    }

    public void PutDown()
    {
        while (actionQueue.Count >= MAX_QUEUE_SIZE)
        {
            Thread.Sleep(50);
        }
        actionQueue.Enqueue(() =>
        {
            if (holding == null) {
                audioSource.PlayOneShot(blockSound);
                working = false;
                delay = 5;
                return;
            }

            Vector2 lookPos = rb.position + (tileSize * lookDir);
            GameObject lookingAt = FindObjectAt(lookPos);

            if (lookingAt != null && lookingAt.activeInHierarchy)
            {
                if (!lookingAt.transform.IsChildOf(outputsList.transform)) {
                    audioSource.PlayOneShot(blockSound);
                    working = false;
                    delay = 5;
                    return;
                }

                holding.transform.position = new Vector2(-50,-50);
                holding = null;
                working = false;
                delay = 10;
                droppedItems += 1;

                Debug.Log(droppedItems);

                if(droppedItems == targetItems)
                {
                    uiManager.showVictoryScreen();
                    audioSource.PlayOneShot(winSound);
                }
                else
                {
                    audioSource.PlayOneShot(progressSound);
                }
                    return;
            }

            holding.transform.position = lookPos;
            audioSource.PlayOneShot(dropSound);
            holding = null;
            holding.SetActive(true);
            working = false;
            delay = 5;
            });
    }

    public int getDroppedItems()
    {
        return droppedItems;
    }

    private GameObject FindObjectAt(Vector2 position, GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (Vector2.Distance(child.position, position) < 0.01f)
            {
                return child.gameObject;
            }
        }
        return null;
    }


    private GameObject FindObjectAt(Vector2 position)
    {
        foreach (Transform child in objectContainer.transform)
        {
            GameObject lookingAt = FindObjectAt(position, child.gameObject);
            if (lookingAt != null)
                return lookingAt;
        }
        return null;
    }

    public void ResetAll()
    {
        foreach (Transform list in objectContainer.transform)
        {
            foreach (Transform obj in list)
            {
                obj.GetComponent<Resetter>().ResetPosition();
            }
        }
        rb.linearVelocity = Vector2.zero;
        animDir = 0;
        animator.SetInteger("direction", animDir);
        rb.position = initialPosition;
        targetPos = initialPosition;
        lastPos = initialPosition;
        holding = null;
        droppedItems = 0;
        delay = 0;
        working = false;
        actionQueue = new ConcurrentQueue<System.Action>();
        lookDir = Vector2.down;
    }
}