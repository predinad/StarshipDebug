using System.Collections.Generic;
using System.Globalization;
//using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Burst.CompilerServices;

public class SortingPuzzle : MonoBehaviour
{
	public TMP_Dropdown drop1;
	public TMP_Dropdown drop2;
	public TMP_Dropdown drop3;
	public TMP_Dropdown drop4;
	public GameObject box1;
	public GameObject box2;
	public GameObject box3;
	public GameObject box4;
	public GameObject box5;
	public GameObject button;
	public GameObject button2;
	public GameObject icon;
	public GameObject failimg;
	public GameObject completeimg;

	//my attempt to universalize the dimensions
	float screenWidth;
	float multiplier;
	private float[] xPositions;

	[Header("object links")]
	[SerializeField] private TaskManager taskManager; // Reference to the the global quest tracker
	[SerializeField] private string taskName;
	[SerializeField] private GameObject sortPuzzle;
	public Trigger trigger;

	//adding this to handle errors from unserialized links
	private void Awake()
	{
		screenWidth = (float)Screen.width;
		multiplier = screenWidth / 760.0f;
		xPositions = new float[] { 385.27f * multiplier, 461.27f * multiplier, 533.57f * multiplier, 610.27f * multiplier, 687.27f * multiplier };

		if (taskManager == null)
		{
			Debug.LogError("EventSystem is not assigned to EngineTracker! Please assign it in the Inspector.");
			enabled = false;
		}
		// Ensure EngineTracker is assigned
		if (sortPuzzle == null)
		{
			Debug.LogError("PuzzleArea is not assigned to EngineTracker! Please assign it in the Inspector.");
			enabled = false;
		}
	}

	public void OnButtonPress2()
	{
		trigger.close();
	}

	public void OnButtonPress()
	{
		// GameObject[] boxes = { box1, box2, box3, box4, box5 };

		// foreach (GameObject box in boxes)
		// {
		// 	if (box != null) // Safety check
		// 	{
		// 		Debug.Log(box.name + " Position: " + box.transform.position);
		// 	}
		// }
		button2.SetActive(false);
		failimg.SetActive(false);
		completeimg.SetActive(false);
		if (drop1.value == 2 && drop2.value == 1 && drop3.value == 1 && drop4.value == 1)
		{
			solved();
			return;
		}
		if (drop1.value == 1 && drop2.value == 2 && drop3.value == 2 && drop4.value == 2)
		{
			solved();
			return;
		}
		lose();
	}

	public void lose()
	{
		if (drop1.value == 1 && drop2.value == 1 && drop3.value == 1 && drop4.value == 1)
		{
			largetosmall();
		}
		if (drop1.value == 2 && drop2.value == 2 && drop3.value == 2 && drop4.value == 2)
		{
			largetosmall();
		}
		failimg.SetActive(true);
	}

	public void largetosmall()
	{
		GameObject[] boxes = { box1, box2, box3, box4, box5 };
		for (int i = 0; i < boxes.Length; i++)
		{
			if (boxes[i] != null)
			{
				Vector3 newPosition = boxes[i].transform.position;
				newPosition.x = xPositions[4 - i];
				boxes[i].transform.position = newPosition;
			}
		}
	}

	public void solved()
	{
		GameObject[] boxes = { box1, box2, box3, box4, box5 };
		completeimg.SetActive(true);
		button.SetActive(false);
		button2.SetActive(true);
		icon.SetActive(false);
		Trigger.isSolved = true;
		for (int i = 0; i < boxes.Length; i++)
		{
			if (boxes[i] != null)
			{
				Vector3 newPosition = boxes[i].transform.position;
				newPosition.x = xPositions[i];
				boxes[i].transform.position = newPosition;
			}
		}
		taskManager.CompleteTask(taskName);
		//adding a call to the new timer method
		// StartCoroutine(CoRoutineSolved()); 
	}

	//for use with time-delayed events, such as closing the puzzle after solved	
	// private IEnumerator CoRoutineSolved()
	// {
	// 	yield return new WaitForSeconds(1.5f);

	// 	if (taskManager != null)
	// 	{
	// 		taskManager.CompleteTask("Sort Supplies");
	// 	}
	// 	if (sortPuzzle != null)
	// 	{
	// 		sortPuzzle.gameObject.SetActive(false);
	// 	}
	// }

}