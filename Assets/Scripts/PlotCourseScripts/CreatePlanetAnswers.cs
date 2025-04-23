using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlanetAnswers : MonoBehaviour
{
    [SerializeField] private GameObject planetPrefab; // Prefab for the planet answer
    [SerializeField] private Transform answerTransform; // Parent transform to hold the planet answers
    [SerializeField] private Transform defaultTransform; // Default transform for the planet answers

    public void CreatePlanets(List<PlanetNode> planetNodes)
    {
        ResetAnswers(); // Clear existing planets before creating new ones

        // Create new planets based on the provided planet nodes
        for (int i = 0; i < planetNodes.Count; i++)
        {
            PlanetNode planetNode = planetNodes[i];
            if (planetNode == null || planetNode.planetGameObject == null)
            {
                Debug.LogWarning($"PlanetNode or its GameObject is null at index {i}.");
                continue;
            }
            GameObject planet = Instantiate(planetPrefab, defaultTransform.position, Quaternion.identity, defaultTransform);
            planet.name = planetNodes[i].planetGameObject.name;
            planet.GetComponent<PlanetAnswerData>().planetIndex = planetNode.planetNumber; // Set the planet number to the answer

            planet.GetComponent<UnityEngine.UI.Image>().sprite = planetNode.planetSprite; // Set the sprite of the planet
            planet.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = planetNode.planetName; // Set the name of the planet
        }
    }
    public int[] GetPlayerAnswer()
    {
        int answerCount = answerTransform.childCount;

        int[] playerAnswers = new int[answerCount];
        for (int i = 0; i < answerCount; i++)
        {
            playerAnswers[i] = answerTransform.GetChild(i).GetComponent<PlanetAnswerData>().planetIndex; // Get the planet number from the answer
        }

        return playerAnswers;
    }

    private void ResetAnswers()
    {
        // Clear existing planets
        foreach (Transform child in answerTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in defaultTransform)
        {
            Destroy(child.gameObject);
        }
    }
}

