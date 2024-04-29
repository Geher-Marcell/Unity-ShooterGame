using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerManager))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float airDrag = 0.5f;
    
    private Rigidbody2D _rb;
    private Camera _cam;
    private float _xDir;
    private float _yDir;
    
    public void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
    }
    
    public void Update()
    {
        _xDir = Input.GetAxisRaw("Horizontal");
        _yDir = Input.GetAxisRaw("Vertical");
        
        LimitSpeed();
        ApplyAirDrag();
        
        Rotate();
    }

    public void FixedUpdate() => Move();

    private void Move()
    {
        if (_xDir == 0 && _yDir == 0) return;
        
        Vector2 moveDir = new Vector2(_xDir, _yDir).normalized;
        
        _rb.AddForce(moveDir * acceleration, ForceMode2D.Impulse);
    }

    private void LimitSpeed()
    {
        if (_rb.velocity.magnitude > maxSpeed) _rb.velocity = _rb.velocity.normalized * maxSpeed;
    }

    private void ApplyAirDrag()
    {
        if (_xDir != 0 || _yDir != 0) return;
        
        var velocity = _rb.velocity;
        velocity = new Vector2(velocity.x * (airDrag/100), velocity.y * (airDrag/100));
        _rb.velocity = velocity;
    }
    
    private void Rotate()
    {
        Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - _rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        _rb.rotation = angle;
    }
}
