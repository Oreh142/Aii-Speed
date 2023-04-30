using UnityEngine;
using UnityEngine.UI;

public class AcceptCaseBuying : MonoBehaviour
{
    [SerializeField] private GameObject[] _containerObjects;
    [SerializeField] private Text _bodyText;
    [SerializeField] private Text _priceText;
    [SerializeField] private Text _freeContainersText;
    private string[] _containerNames;
    private int _price;
    private int _id;

    private void FillCasesNamesArray()
    {
        _containerNames = new string[_containerObjects.Length];
        for (int i = 0; i < _containerObjects.Length; i++)
        {
            _containerNames[i] = _containerObjects[i].GetComponentsInChildren<Text>()[0].text.ToString();
        }
    }

    public void CreateBuyingWindow(int price, int id)
    {
        gameObject.SetActive(true);
        FillCasesNamesArray();
        _freeContainersText = gameObject.GetComponentsInChildren<Text>()[1];
        _price = price;
        _id = id;
        _priceText.text = FormatNumsHelper.FormatNum((float)price);
        _bodyText.text = "Вы уверены, что хотите открыть " + _containerNames[id] + "?";
        int recievedCases = PlayerPrefsSafe.GetInt("recievedReward" + id);
        _freeContainersText.text = recievedCases + " бесплатно";
        if (recievedCases > 0)
        {
            _bodyText.text += " Вы можете открыть бесплатно " + recievedCases + " раз";
            _freeContainersText.color = Color.white;
        }
        else
        {
            _bodyText.text += " У вас нет бесплатных открытий";
            _freeContainersText.color = Color.red;
        }
    }

    public void GetContainer(bool isRecieved)
    {
        if (_containerObjects[_id].GetComponent<MoneyContainer>())
        {
            if ((!isRecieved & _containerObjects[_id].GetComponent<MoneyContainer>().GetDayLimit() > 0) | isRecieved)
            {
                MoneyContainer cont = _containerObjects[_id].GetComponent<MoneyContainer>();
                cont.StartContainer(isRecieved, _price);
                _id = 0;
                _price = 0;
                gameObject.SetActive(false);
            }
        }

    }

    public void OnEscapeButtonClicked()
    {
        _id = 0;
        _price = 0;
        gameObject.SetActive(false);
    }
}