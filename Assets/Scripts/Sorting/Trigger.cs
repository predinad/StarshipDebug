using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
	[SerializeField] private bool triggerActive = false;
	public GameObject popupPanel;
	private bool isActive = false;
	static public bool isSolved = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			popupPanel.SetActive(false);
			isActive = false;
			triggerActive = true;
			//Debug.Log("Hello, Unity Console!");
		}
	}


	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			triggerActive = false;
			popupPanel.SetActive(false);
			isActive = false;
			//Debug.Log("Goodbye, Unity Console!");
		}
	}


	private void Update()
	{
		if (triggerActive && Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.Interact)) && !isSolved)
		{
			isActive = !isActive;
			popupPanel.SetActive(isActive);

		}
		else if (triggerActive && Input.GetKeyDown(InputManager.Instance.GetKey(GameAction.Interact)))
		{
			triggerActive = false;
			popupPanel.SetActive(false);
		}
	}

}