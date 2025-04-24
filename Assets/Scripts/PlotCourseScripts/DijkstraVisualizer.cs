using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DijkstraVisualizer : MonoBehaviour
{
    [SerializeField] private NavigationPuzzle puzzle;
    [SerializeField] private Transform chartContainer;
    [SerializeField] private GameObject tableRowPrefab;
    [SerializeField] private TextMeshProUGUI currentNodeText;

    private List<DijkstraStep> steps;
    private int currentStep = 0;

    public void InitializeSteps(int start, int end)
    {
        steps = puzzle.GetDijkstraSteps(start, end);
        currentStep = 0;
        UpdateTableUI();
    }

    public void NextStep()
    {
        if (currentStep < steps.Count - 1)
        {
            currentStep++;
            UpdateTableUI();
        }
    }

    public void PreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            UpdateTableUI();
        }
    }

    private void UpdateTableUI()
    {
        foreach (Transform child in chartContainer)
            Destroy(child.gameObject);

        var step = steps[currentStep];

        // Update current node text
        if (step.currentNode == 0)
        {
            currentNodeText.text = "Currently Node: Start";
        }
        else
        {
            string currentNodeName = puzzle.getPlanetName(steps[currentStep - 1].currentNode);
            currentNodeText.text = $"Current Node: {currentNodeName}";
        }

        foreach (var kvp in step.distances)
        {
            GameObject row = Instantiate(tableRowPrefab, chartContainer);

            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 3)
            {
                // Column 0: Node Name
                texts[0].text = puzzle.getPlanetName(kvp.Key);

                // Column 1: Distance
                texts[1].text = kvp.Value == int.MaxValue ? "∞" : kvp.Value.ToString();

                // Column 2: Previous Node Name
                if (step.previous[kvp.Key].HasValue)
                {
                    texts[2].text = puzzle.getPlanetName(step.previous[kvp.Key].Value);
                }
                else
                {
                    texts[2].text = "-";
                }

                // Row Highlighting
                if (step.visited.Contains(kvp.Key))
                {
                    row.GetComponent<Image>().color = new Color(0.95f, 0.95f, 0.90f); // visited
                }
                else if (kvp.Key == step.currentNode)
                {
                    row.GetComponent<Image>().color = new Color(0.6f, 0.7f, 1f); // current
                }
            }
        }
    }
}
