using System.Collections.Generic;
using System.Globalization;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPuzzle : MonoBehaviour
{
    [SerializeField] private List<PlanetNode> planetNodes;
    [SerializeField] private List<Connection> connections;
    [SerializeField] private CreatePlanetAnswers createPlanetAnswers;
    [SerializeField] private List<Sprite> randomPlanetImages;
    [SerializeField] private List<string> randomPlanetNames;
    [SerializeField] private int minWeight = 1;
    [SerializeField] private int maxWeight = 10;
    [SerializeField] private TaskManager taskManager; // Reference to the TaskManager to update tasks
    [SerializeField] private NavigationUIHandler navigationUIHandler; // Reference to the NavigationUIHandler for UI management
    [SerializeField] private string taskName; // Name of the task to be updated

    //UI elements
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject incorrectSubmissionText;
    [SerializeField] TextMeshProUGUI attemptsText;
    [SerializeField] TextMeshProUGUI completionAttemptsText;

    private Dictionary<int, List<Connection>> nodeConnections = new(); // planetNumber -> connections
    private Dictionary<int, PlanetNode> nodeByNumber = new(); // planetNumber -> PlanetNode
    private int attempts = 0; // Number of attempts made by the player

    public void StartPuzzle()
    {
        //Hide the retry button and incorrect submission text at the start of the puzzle
        retryButton.SetActive(false);
        incorrectSubmissionText.SetActive(false);

        // Reset the puzzle state
        foreach (var node in planetNodes)
        {
            node.planetGameObject.SetActive(false); // Hide all planets
        }

        foreach (var conn in connections)
        {
            conn.connectionImage.gameObject.SetActive(false); // Hide all connections
            conn.connectionText.gameObject.SetActive(false); // Hide all connection texts
        }

        // Reset the attempts counter
        attempts = 0;
        attemptsText.text = $"Attempts: {attempts}";

        incorrectSubmissionText.SetActive(false); // Hide incorrect submission text
        retryButton.SetActive(false); // Hide retry button

        InitializePuzzle(); // Reinitialize the puzzle
        createPlanetAnswers.CreatePlanets(planetNodes); // Recreate planets based on the initialized nodes

        // Reset the puzzle state
        foreach (var node in planetNodes)
        {
            node.planetGameObject.SetActive(true); // Hide all planets
        }

        foreach (var conn in connections)
        {
            conn.connectionImage.gameObject.SetActive(true); // Hide all connections
            conn.connectionText.gameObject.SetActive(true); // Hide all connection texts
        }
    }

    public void StopSubmissions()
    {
        // Disable dragging for the answers
        createPlanetAnswers.DisableDraggingAnswers();

        // Show the retry button and incorrect submission text
        retryButton.SetActive(true); // Show retry button
        incorrectSubmissionText.SetActive(true); // Show incorrect submission text
    }

    public void RetryPuzzle()
    {
        // Reset the answers and enable dragging again
        createPlanetAnswers.EnableDraggingAnswers();
        createPlanetAnswers.ResetAnswers();

        // Hide the retry button and incorrect submission text
        retryButton.SetActive(false); // Hide retry button
        incorrectSubmissionText.SetActive(false); // Hide incorrect submission text
    }

    public void InitializePuzzle()
    {
        // Shuffle the name list to prevent duplicates
        List<string> shuffledNames = new List<string>(randomPlanetNames);
        ShuffleList(shuffledNames); // We'll define this below

        // Randomize planet visuals and assign unique names
        for (int i = 0; i < planetNodes.Count; i++)
        {
            var node = planetNodes[i];
            var img = node.planetGameObject.GetComponent<UnityEngine.UI.Image>();
            var name = node.planetGameObject.GetComponentInChildren<TextMeshProUGUI>();

            node.planetNumber = i; // Assign a unique number to each planet node
            node.planetSprite = randomPlanetImages[Random.Range(0, randomPlanetImages.Count)];
            img.sprite = node.planetSprite;

            if (i < shuffledNames.Count)
            {
                node.planetName = shuffledNames[i];
                name.text = node.planetName;
            }
            else
            {
                Debug.LogWarning("Not enough unique names in randomPlanetNames!");
                name.text = $"Planet {i}";
            }
        }

        // Randomize connection weights
        foreach (var conn in connections)
        {
            int weight = Random.Range(minWeight, maxWeight + 1);
            conn.weight = weight;
            conn.connectionText.text = weight.ToString();
        }

        // Cache connections for each node
        CachePlanetLookup();
        CacheConnections();
    }

    public int[] GetShortestPath(int startPlanetNumber, int endPlanetNumber)
    {
        Dictionary<int, int> distances = new();
        Dictionary<int, int?> previous = new();
        HashSet<int> visited = new();
        List<(int, int)> queue = new(); // (distance, planetNumber)

        foreach (var node in planetNodes)
        {
            distances[node.planetNumber] = int.MaxValue;
            previous[node.planetNumber] = null;
        }

        distances[startPlanetNumber] = 0;
        queue.Add((0, startPlanetNumber));

        while (queue.Count > 0)
        {
            queue.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            var (currentDist, currentPlanet) = queue[0];
            queue.RemoveAt(0);

            if (currentPlanet == endPlanetNumber)
                break;

            if (visited.Contains(currentPlanet))
                continue;

            visited.Add(currentPlanet);

            foreach (var connection in nodeConnections[currentPlanet])
            {
                int neighbor = (connection.nodeA.planetNumber == currentPlanet)
                    ? connection.nodeB.planetNumber
                    : connection.nodeA.planetNumber;

                if (visited.Contains(neighbor))
                    continue;

                int tentativeDist = currentDist + connection.weight;

                if (tentativeDist < distances[neighbor])
                {
                    distances[neighbor] = tentativeDist;
                    previous[neighbor] = currentPlanet;
                    queue.Add((tentativeDist, neighbor));
                }
            }
        }

        // Reconstruct path
        List<int> path = new();
        int? current = endPlanetNumber;
        while (current != null)
        {
            path.Insert(0, current.Value);
            current = previous[current.Value];
        }

        return path.ToArray();
    }

    public List<DijkstraStep> GetDijkstraSteps(int start, int end)
    {
        List<DijkstraStep> steps = new();
        Dictionary<int, int> distances = new();
        Dictionary<int, int?> previous = new();
        HashSet<int> visited = new();
        List<(int, int)> queue = new();

        foreach (var node in planetNodes)
        {
            distances[node.planetNumber] = int.MaxValue;
            previous[node.planetNumber] = null;
        }

        distances[start] = 0;
        queue.Add((0, start));

        while (queue.Count > 0)
        {
            queue.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            var (currentDist, current) = queue[0];
            queue.RemoveAt(0);

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            // Record step
            DijkstraStep step = new()
            {
                currentNode = current,
                distances = new Dictionary<int, int>(distances),
                previous = new Dictionary<int, int?>(previous),
                visited = new List<int>(visited)
            };
            steps.Add(step);

            foreach (var conn in nodeConnections[current])
            {
                int neighbor = conn.nodeA.planetNumber == current ? conn.nodeB.planetNumber : conn.nodeA.planetNumber;
                if (visited.Contains(neighbor)) continue;

                int newDist = currentDist + conn.weight;
                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previous[neighbor] = current;
                    queue.Add((newDist, neighbor));
                }
            }
        }

        return steps;
    }


    public void CheckAnswer()
    {
        bool isCorrect = true;

        int[] playerAnswers = createPlanetAnswers.GetPlayerAnswer();
        int start = planetNodes[0].planetNumber;
        int end = planetNodes[planetNodes.Count - 1].planetNumber;
        int[] correctAnswers = GetShortestPath(start, end);

        Debug.Log($"Player answers: {string.Join(", ", playerAnswers)}");
        Debug.Log($"Correct answers: {string.Join(", ", correctAnswers)}");

        if (playerAnswers.Length != correctAnswers.Length)
        {
            Debug.Log("Incorrect answer length.");
            isCorrect = false;
        }

        for (int i = 0; i < playerAnswers.Length && i < correctAnswers.Length; i++)
        {
            if (playerAnswers[i] != correctAnswers[i])
            {
                Debug.Log($"Incorrect answer at index {i}. Expected {correctAnswers[i]}, got {playerAnswers[i]}.");
                isCorrect = false;
            }
        }

        if (!isCorrect)
        {
            StopSubmissions(); // Stop submissions if the answer is incorrect

            // Increment the attempts counter
            attempts++;
            attemptsText.text = $"Attempts: {attempts}";

            Debug.Log("Incorrect answer!");
        }
        else
        {
            attempts++;
            completionAttemptsText.text = $"You took {attempts} attempts for this puzzle!"; // Update the attempts text for completion

            // Mark the task as completed in the TaskManager
            if (taskManager != null)
            {
                taskManager.CompleteTask(taskName); // Mark the task as completed
                Debug.Log($"Task '{taskName}' marked as completed.");
            }
            else
            {
                Debug.LogWarning("TaskManager reference is not set in NavigationPuzzle.");
            }

            if (navigationUIHandler != null)
            {
                navigationUIHandler.ShowNextPanel(); // Show the next panel in the UI
            }
            else
            {
                Debug.LogWarning("NavigationUIHandler reference is not set in NavigationPuzzle.");
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    private void CachePlanetLookup()
    {
        nodeByNumber.Clear();
        foreach (var node in planetNodes)
            nodeByNumber[node.planetNumber] = node;
    }

    private void CacheConnections()
    {
        nodeConnections.Clear();
        foreach (var node in planetNodes)
            nodeConnections[node.planetNumber] = new List<Connection>();

        foreach (var conn in connections)
        {
            nodeConnections[conn.nodeA.planetNumber].Add(conn);
            nodeConnections[conn.nodeB.planetNumber].Add(conn);
        }
    }
}

[System.Serializable]
public class PlanetNode
{
    [SerializeField] public GameObject planetGameObject; // Reference to the planet GameObject
    public string planetName;
    public Sprite planetSprite;
    public int planetNumber; // Unique number for the planet
}

[System.Serializable]
public class Connection
{
    [SerializeField] public PlanetNode nodeA;
    [SerializeField] public PlanetNode nodeB;
    public int weight;

    [SerializeField] public TextMeshProUGUI connectionText;
    [SerializeField] public UnityEngine.UI.Image connectionImage;
}

[System.Serializable]
public class DijkstraStep
{
    public int currentNode;
    public Dictionary<int, int> distances = new();
    public Dictionary<int, int?> previous = new();
    public List<int> visited = new();
}
