using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _speed;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uIManager;

    private void Start()
    {
        _maxSpeed = _gameManager.controllAbility;
        _speed = _maxSpeed * 0.1f;
    }

    private void FixedUpdate()
    {
        Moving();
    }

    private void Moving()
    {
        if (!_uIManager.GetPausedStatus())
        {
            if (_gameManager.IsTheEnd())
            {
                if (_speed > _maxSpeed * -1)
                {
                    _speed -= _maxSpeed * 0.1f;
                }
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, 10), _speed * Time.deltaTime);
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    if (transform.position.y != 4.3f)
                    {
                        if (_speed <= _maxSpeed)
                        {
                            _speed += _maxSpeed * 0.1f;
                        }
                        _gameManager.ChangeSpeed(true);
                    }
                    else if (transform.position.y == 4.3f)
                    {
                        _speed = 0;
                    }
                }
                else
                {
                    if (transform.position.y != -4.3f)
                    {
                        if (_speed >= _maxSpeed * -1)
                        {
                            _speed -= _maxSpeed * 0.1f;
                        }
                        _gameManager.ChangeSpeed(false);
                    }
                    else if (transform.position.y == -4.3f)
                    {
                        _speed = 0;
                    }
                }
                if (transform.position.y >= -4.3f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, 4.31f), _speed * Time.deltaTime);
                }
                CheckBorders();
            }
        }
    }

    private void CheckBorders()
    {
        if (transform.position.y > 4.3f)
        {
            transform.position = new Vector2(-5, 4.3f);
        }
        else if (transform.position.y < -4.3f)
        {
            transform.position = new Vector2(-5, -4.3f);
        }
    }
}