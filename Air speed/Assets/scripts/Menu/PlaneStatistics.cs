using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneStatistics : MonoBehaviour
{
    public Sprite[] planesImages;
    private Menu _menu;
    private ContainerLoot _containerLoot;
    [SerializeField] private GameObject _framePrefab;
    [SerializeField] private Transform _frameParent;

    private void Start()
    {
        _menu = GetComponent<Menu>();
        _containerLoot = GetComponent<ContainerLoot>();
        ShowStatistics();
    }
    public void ShowStatistics()
    {
        int num = _menu.PlanesNumber();
        for (int i = 0; i < num; i++)
        {
            if (PlayerPrefsSafe.GetFloat("totalGames" + i) > 0)
            {
                GameObject frame = Instantiate(_framePrefab, _frameParent);
                Text[] texts = frame.GetComponentsInChildren<Text>();
                texts[0].text = _containerLoot.planesNames[i];
                texts[1].text = FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalGames" + i));
                texts[2].text = FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalScore" + i));
                texts[3].text = FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalDistance" + i)) + " ì";
                texts[4].text = FormatNumsHelper.FormatTime(PlayerPrefsSafe.GetFloat("totalTime" + i));
                texts[5].text = FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalScore" + i) / PlayerPrefsSafe.GetFloat("totalGames" + i));
                texts[6].text = FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalDistance" + i) / PlayerPrefsSafe.GetFloat("totalGames" + i)) + " ì";
                texts[7].text = FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalSpeed" + i) / PlayerPrefsSafe.GetFloat("totalGames" + i)) + " Êì/÷";
                texts[8].text = FormatNumsHelper.FormatTime(PlayerPrefsSafe.GetFloat("totalTime" + i) / PlayerPrefsSafe.GetFloat("totalGames" + i));
                texts[9].text = FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("topScore" + i));
                texts[10].text = FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("topDistance" + i)) + " ì";
                Image[] images = frame.GetComponentsInChildren<Image>();
                images[4].sprite = planesImages[i];
            }
        }
    }
}
