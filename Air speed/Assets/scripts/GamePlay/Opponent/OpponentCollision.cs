using UnityEngine;

public class OpponentCollision : MonoBehaviour
{
    private OpponentsSpawner _opponentsSpawner;
    private GameManager _gameManager;
    private bool _isdead;

    private void Start()
    {
        _opponentsSpawner = GameObject.Find("GameManager").GetComponent<OpponentsSpawner>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<OpponentCollision>())
        {
            Destroy(gameObject);
            _opponentsSpawner.OpponentsRespawn();
        }
        else if (!_isdead)
        {
            _gameManager.CreateFireInPlane(gameObject.transform, false);
            Destroy(gameObject, 10);
            _isdead = true;
        }
    }
}