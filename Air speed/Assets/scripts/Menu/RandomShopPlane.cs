using UnityEngine;
using UnityEngine.UI;

public class RandomShopPlane : ShopPlane
{
    [SerializeField] private int[] _idsOfPlanesToRandom;
    [SerializeField] private string _key;

    private void Start()
    {
        _id = PlayerPrefsSafe.GetInt(_key);
        if (_menu.CheckPlane(_id) == 1)
        {
            ChangeData("В ангаре");
        }
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        images[1].sprite = _planeStatistics.planesImages[_id];
        Text[] texts = gameObject.GetComponentsInChildren<Text>();
        texts[0].text = _containerLoot.planesNames[_id];
        texts[1].text = _cost.ToString("0");
    }
    private void Update()
    {
        if (_menu.CheckPlane(_id) == 1)
        {
            ChangeData("В ангаре");
        }
    }

    public void ReloadPlane()
    {
        _id = _idsOfPlanesToRandom[Random.Range(0, _idsOfPlanesToRandom.Length)];
        PlayerPrefsSafe.SetInt(_key, _id);
    }

    public int GetPlaneId()
    {
        return _id;
    }
}
