using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aviateca : MonoBehaviour
{
    [SerializeField] GameObject _planeFrame;
    [SerializeField] Transform _parent;
    private ContainerLoot _containerLoot;
    private PlaneStatistics _planeStatistics;
    private Menu _menu;
    private GameObject[] _planesObj;

    void Start()
    {
        _menu = GetComponent<Menu>();
        _containerLoot = GetComponent<ContainerLoot>();
        _planeStatistics = GetComponent<PlaneStatistics>();
        _planesObj = new GameObject[_menu.PlanesNumber()];
        UpdateAviateca();
    }

    public void UpdateAviateca()
    {
        if (_parent.childCount > 0)
        {
            for (int i = 0; i < _menu.PlanesNumber(); i++)
            {
                Destroy(_planesObj[i]);
            }
        }
        for (int i = 0; i < _menu.PlanesNumber(); i++)
        {
            _planesObj[i] = Instantiate(_planeFrame, _parent);
            Image[] images = _planesObj[i].GetComponentsInChildren<Image>();
            images[1].sprite = _planeStatistics.planesImages[i];
            images[0].sprite = _containerLoot.rarityOfFrame[_containerLoot.CheckRarity(i)];
            if (_menu.CheckPlane(i) == 0)
            {
                images[1].color = Color.black;
            }
            Text planeText = _planesObj[i].GetComponentInChildren<Text>();
            planeText.text = _containerLoot.planesNames[i].ToString();
        }
    }
}
