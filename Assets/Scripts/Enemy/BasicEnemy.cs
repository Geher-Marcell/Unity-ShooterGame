using UnityEngine;

namespace Enemy
{
    public class BasicEnemy : Enemy
    {
        //[Header("Script Specific Variables")]
        public override void Update()
        {
            //Add force towards player
            Vector2 direction = Player.position - transform.position;
            Rb.AddForce(direction.normalized * Acceleration, ForceMode2D.Force);
            
            //Rotate towards player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            
            base.Update();
        }
    }
}