using UnityEngine;
using UnityEngine.UI;

public class ShopPlane : MonoBehaviour
{
    [SerializeField] protected GameObject _acceptBuyingWindow;
    [SerializeField] protected Menu _menu;
    [SerializeField] protected ContainerLoot _containerLoot;
    [SerializeField] protected PlaneStatistics _planeStatistics;
    [SerializeField] protected int _id;
    [SerializeField] protected int _cost;

    private void Start()
    {
        if (_menu.CheckPlane(_id) == 1)
        {
            ChangeData("� ������");
        }
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        images[1].sprite = _planeStatistics.planesImages[_id];
    }
    private void Update()
    {
        if (_menu.CheckPlane(_id) == 1)
        {
            ChangeData("� ������");
        }
    }

    protected void ChangeData(string message)
    {
        Text[] texts = GetComponentsInChildren<Text>();
        texts[1].text = message;
        Image[] images = GetComponentsInChildren<Image>();
        images[3].enabled = false;
    }

    public void BuyPlane()
    {
        if (_menu.CheckPlane(_id) == 1)
        {
            ChangeData("� ������ ������");
        }
        else
        {
            CreateAcceptBuyingWindow(_id);
            ChangeData("��������� ������");
        }
    }

    protected void CreateAcceptBuyingWindow(int id)
    {
        Text[] texts = _acceptBuyingWindow.GetComponentsInChildren<Text>();
        texts[1].text = _containerLoot.planesNames[id];
        texts[2].text = _cost.ToString();
        _acceptBuyingWindow.SetActive(true);
    }
}