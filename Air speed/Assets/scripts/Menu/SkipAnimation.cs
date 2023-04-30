using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipAnimation : MonoBehaviour
{
    [SerializeField] Text skipAnimText;
    [SerializeField] ContainerLoot _containerLoot;
    private int _try;

    public void OnBgClicked()
    {
        if (_try == 0 && _containerLoot.isScrolling)
        {
            _try++;
            skipAnimText.text = "Нажмите ещё раз, чтобы пропустить анимацию...";
        }
        else if (_try == 1 && _containerLoot.isScrolling)
        {
            skipAnimText.text = " ";
            _containerLoot.ShowResults();
            _try = 0;
        }
    }
    private void Update()
    {
        if (!_containerLoot.isScrolling)
        {
            skipAnimText.text = "";
        }
    }
}
