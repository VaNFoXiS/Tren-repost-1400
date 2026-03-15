using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemInfio itemInfio;
    public GameObject itemGameObject;

    public abstract void Use();
}
