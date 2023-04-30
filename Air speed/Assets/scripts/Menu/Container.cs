using System;
using UnityEngine;
using UnityEngine.UI;

public class Container : MonoBehaviour
{
    [SerializeField] protected ContainerLoot _containerLoot;
    [SerializeField] protected AcceptCaseBuying _acceptCaseBuying;
    [SerializeField] protected int _id;
    [SerializeField] protected int _price;
    [SerializeField] protected float[] _planeChance;
    [SerializeField] protected float[] _moneyChances;
    protected PlayfabManager _playfabManager;
}