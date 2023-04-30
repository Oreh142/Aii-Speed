using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerParent;
    [SerializeField] private GameObject[] _playerPrefabs;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = gameObject.GetComponent<GameManager>();
        int id = _gameManager.idPlayerPlane;
        SpawnPlayer(_playerPrefabs, id, _playerParent.transform);
    }

    private void SpawnPlayer(GameObject[] playerPrefabs, int id, Transform parent)
    {
        Instantiate(playerPrefabs[id], new Vector2(0, 0), Quaternion.identity, parent);
    }
}