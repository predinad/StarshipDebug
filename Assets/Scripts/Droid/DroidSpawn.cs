using System;
using UnityEngine;

public class DroidSpawn : MonoBehaviour
{
    [SerializeField] private GameObject objectContainer;
    [SerializeField] private GameObject itemList;
    [SerializeField] private GameObject outputsList;
    [SerializeField] private int itemCount;
    [SerializeField] private Vector2 dir;


    internal Vector2 GetInitDir()
    {
        return dir;
    }

    internal Vector2 GetInitPos()
    {
        return transform.position;
    }

    internal int getItemCount()
    {
        return itemCount;
    }

    internal GameObject GetItemList()
    {
        return itemList;
    }

    internal GameObject GetObjectContainer()
    {
        return objectContainer;
    }

    internal GameObject getOutputList()
    {
        return outputsList;
    }
}
