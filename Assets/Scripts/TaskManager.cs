using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * TaskManager is responsible for managing the task list.
 * It handles the display and completion of tasks.
 */

public class TaskManager : MonoBehaviour
{
    /// Struct to represent a task
    /// Each task has a name, a completion status, and a reference to its checkmark GameObject.
    [System.Serializable]
    public struct Task
    {
        public string taskName;
        public bool isCompleted;
        public GameObject checkMark;
        public TextMeshProUGUI taskText;
    }

    /// List of tasks to be managed
    /// The tasks are serialized so they can be easily set in the Unity Inspector.
    [SerializeField] private List<Task> tasks = new();

    /// Dictionary to map task names to their corresponding GameObjects
    /// This is used so that task objects in the list can be referenced by name.
    private Dictionary<string, Task> taskMap;

    private void Awake()
    {
        //Build the lookup dictionary for tasks
        taskMap = new Dictionary<string, Task>();
        foreach (var task in tasks) 
        {
            if(!taskMap.ContainsKey(task.taskName) && task.checkMark != null && task.taskText != null)
            {
                taskMap.Add(task.taskName, task);
                task.checkMark.SetActive(false); // Ensure checkmark is hidden at start
                task.taskText.text = task.taskName; //Set the UI text to match the task name
            }
            else 
            {
                Debug.LogWarning("Duplicate task name or missing checkmark for task: " + task.taskName);
            }
        }
    }  

    // Method to mark a task as comepleted
    public void CompleteTask(string taskName)
    {
        if(taskMap.TryGetValue(taskName, out Task task))
        {
            task.isCompleted = true;
            task.checkMark.SetActive(true);
            taskMap[taskName] = task; //Update the original task in the dictionary 
        }
        else
        {
            Debug.LogWarning("Task not found:" + taskName);
        }
    }
}
