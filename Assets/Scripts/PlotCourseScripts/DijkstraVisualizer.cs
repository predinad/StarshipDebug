using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class DijkstraVisualizer : MonoBehaviour
{
    [SerializeField] private NavigationPuzzle puzzle;
    [SerializeField] private GameObject stepUIPrefab;
    [SerializeField] private Transform chartContainer;
    private List<DijkstraStep> steps;
    private int currentStep = 0;

    public void InitializeSteps(int start, int end)
    {
        steps = puzzle.GetDijkstraSteps(start, end);
        currentStep = 0;
        UpdateUI();
    }

    public void NextStep()
    {
        if (currentStep < steps.Count - 1)
        {
            currentStep++;
            UpdateUI();
        }
    }

    public void PreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        var step = steps[currentStep];
        // Clear existing chart elements
        foreach (Transform child in chartContainer) Destroy(child.gameObject);

        // Create a basic visual (you can get fancy with bars/graphs)
        foreach (var kvp in step.distances)
        {
            var ui = Instantiate(stepUIPrefab, chartContainer);
            ui.GetComponent<TextMeshProUGUI>().text = $"Planet {kvp.Key}: {kvp.Value}";
        }
    }
}
