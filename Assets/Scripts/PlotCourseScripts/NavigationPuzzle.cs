using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPuzzle : MonoBehaviour
{
    [SerializeField] private List<PlanetNode> planetNodes; // List of planet nodes in the puzzle
    [SerializeField] private List<Connection> connections; // List of connections between the planets
    public Dictionary<PlanetNode, List<Connection>> nodeConnections = new();

    [SerializeField] private List<Sprite> randomPlanetImages; // List of random planet images to choose from
    [SerializeField] private List<string> randomPlanetNames; // List of random planet names to choose from
    [SerializeField] private int minWeight = 1;
    [SerializeField] private int maxWeight = 10;

    void Start()
    {
        InitializePuzzle();
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

            img.sprite = randomPlanetImages[Random.Range(0, randomPlanetImages.Count)];

            if (i < shuffledNames.Count)
            {
                name.text = shuffledNames[i];
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
        CacheConnections();
    }

    public List<Connection> GetConnectionsForNode(PlanetNode node)
    {
        List<Connection> result = new List<Connection>();
        foreach (var conn in connections)
        {
            if (conn.nodeA == node || conn.nodeB == node)
            {
                result.Add(conn);
            }
        }
        return result;
    }

    private void CacheConnections()
    {
        nodeConnections.Clear();
        foreach (var node in planetNodes)
            nodeConnections[node] = new List<Connection>();

        foreach (var conn in connections)
        {
            nodeConnections[conn.nodeA].Add(conn);
            nodeConnections[conn.nodeB].Add(conn);
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
}

[System.Serializable]
public class PlanetNode
{
    [SerializeField] public GameObject planetGameObject; // Reference to the planet GameObject
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
