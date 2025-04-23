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

    private Dictionary<int, List<Connection>> nodeConnections = new(); // planetNumber -> connections
    private Dictionary<int, PlanetNode> nodeByNumber = new(); // planetNumber -> PlanetNode

    void Start()
    {
        InitializePuzzle();
        createPlanetAnswers.CreatePlanets(planetNodes); // Create planets based on the initialized nodes


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

    public void CheckAnswer()
    {
        int[] playerAnswers = createPlanetAnswers.GetPlayerAnswer();
        int start = planetNodes[0].planetNumber;
        int end = planetNodes[planetNodes.Count - 1].planetNumber;
        int[] correctAnswers = GetShortestPath(start, end);

        Debug.Log($"Player answers: {string.Join(", ", playerAnswers)}");
        Debug.Log($"Correct answers: {string.Join(", ", correctAnswers)}");

        if (playerAnswers.Length != correctAnswers.Length)
        {
            Debug.Log("Incorrect answer length.");
            return;
        }

        for (int i = 0; i < playerAnswers.Length; i++)
        {
            if (playerAnswers[i] != correctAnswers[i])
            {
                Debug.Log($"Incorrect answer at index {i}. Expected {correctAnswers[i]}, got {playerAnswers[i]}.");
                return;
            }
        }

        Debug.Log("Correct answer!");
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
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
