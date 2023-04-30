using UnityEngine;

public class SpawnBackgrounds : MonoBehaviour
{
    [SerializeField] private GameObject _background;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        for (int i = -19; i <= 19; i += 19)
        {
            GameObject bg = Instantiate(_background, new Vector2(i, 0), Quaternion.identity);
            if (_gameManager.GetIsRainy())
            {
                SpriteRenderer sprite = bg.GetComponent<SpriteRenderer>();
                sprite.color = new Color(0.9f, 0.9f, 0.9f, 1);
            }
        }
    }
}