using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager _gameManager;
    private bool _isdead;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (!_isdead)
        {
            _gameManager.CreateFireInPlane(gameObject.transform, true);
            _gameManager.TheEndGame();
            _isdead = true;
            Destroy(gameObject, 10);
        }
    }
}