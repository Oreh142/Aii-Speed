using UnityEngine;

public class OpponentMoving : MonoBehaviour
{
    private Vector2 _target;
    private float _speed;
    private Opponents _opponents;

    private void Start()
    {
        _opponents = GetComponentInParent<Opponents>();
        _speed = _opponents.speed;
        _target = new(-20, transform.position.y);
    }

    private void Update()
    {
        _speed = _opponents.speed;
        transform.position = Vector2.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
        transform.position = new Vector2(transform.position.x, transform.position.y);
        if (transform.position.x == -20)
        {
            DeletePlane();
        }
    }

    public void DeletePlane()
    {
        Destroy(gameObject);
    }
}