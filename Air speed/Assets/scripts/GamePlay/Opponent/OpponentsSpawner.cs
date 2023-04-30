using System.Collections;
using UnityEngine;

public class OpponentsSpawner : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private GameObject[] _opponentPrefabs;
    [SerializeField] private int[] _exceptions;
    private GameManager gameManager;
    private int tryInt = 0;
    private float _speed;

    private void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        _exceptions[0] = gameManager.idPlayerPlane;
        CreatePlanesDeal();
    }
    private void CreatePlanesDeal() //Зависимо от скорости самолёта ускоряем или замедляем кд респавна врагов
    {
        float min, max;
        float scoreK = (gameManager.score / 60000) + 1;
        _speed = FormatNumsHelper.FormatToUnitySpeed(gameManager.currentSpeed);
        min = 9 / (_speed * scoreK);
        max = min * 1.5f;

        StartCoroutine(SpawnOppnoentsCoroiutine(min, max));
    }
    IEnumerator SpawnOppnoentsCoroiutine(float min, float max)
    {
        yield return new WaitForSeconds(Random.Range(min, max));
        int number;
        if (Random.Range(0, 20) > 17 && gameManager.currentSpeed <= 600)
        {
            number = 2;
        }
        else
        {
            number = 1;
        }
        RandomId(number);
        CreatePlanesDeal();
    }
    public void RandomId(int number)
    {
        int id = 0;
        for (int i = 0; i < number; i++)
        {
            bool isready = false;
            while (!isready)
            {
                int expeptionsNumber = 0;
                id = Random.Range(0, _opponentPrefabs.Length);
                for (int j = 0; j < _exceptions.Length; j++)
                {
                    if (id != _exceptions[j])
                    {
                        expeptionsNumber++;
                    }
                }
                if (expeptionsNumber == _exceptions.Length)
                {
                    isready = true;
                }
            }
            SpawnOpponents(id);
        }
    }
    private int ChangeRandomId(int id)
    {
        while (id == 12 ^ id == 11 ^ id == 14 ^ id == gameManager.idPlayerPlane)
        {
            id = Random.Range(0, _opponentPrefabs.Length);
        }
        return id;
    }
    public void OpponentsRespawn()
    {
        tryInt++;
        if (tryInt == 2)
        {
            RandomId(2);
        }
    }
    private void SpawnOpponents(int id)
    {
        Instantiate(_opponentPrefabs[id], new Vector2(Random.Range(18f, 22f), Random.Range(-4.3f, 4.3f)), Quaternion.Euler(0, 180, 0), _parent);
    }
    public void StopSpawn()
    {
        StopAllCoroutines();
    }
}
