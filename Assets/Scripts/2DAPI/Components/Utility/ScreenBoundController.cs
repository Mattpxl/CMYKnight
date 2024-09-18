using UnityEngine;
public class ScreenBoundController : MonoBehaviour
{
    [SerializeField] private float _screenBoarder;
    [SerializeField] private Camera _camera;
    public void muteOffScreen(GameObject gameObj)
    {
         Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        if 
        (
            screenPosition.x < 0 ||
            screenPosition.x > _camera.pixelWidth ||
            screenPosition.y < 0 ||
            screenPosition.y > _camera.pixelHeight
        )   
        {
            gameObj.GetComponent<AudioSource>().mute = true;
        }
        else 
        {
            gameObj.GetComponent<AudioSource>().mute = false;
        }
    }
    public void idleOffScreen(GameObject gameObj)
    {
         Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        if 
        (
            screenPosition.x < 0 ||
            screenPosition.x > _camera.pixelWidth ||
            screenPosition.y < 0 ||
            screenPosition.y > _camera.pixelHeight
        )   
        {
            gameObj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        else 
        {
            gameObj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
    public Vector2 boarderStop(Vector2 velocity)
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        if 
        (
            (screenPosition.x < _screenBoarder && velocity.x < 0) || 
            (screenPosition.x > _camera.pixelWidth - _screenBoarder && velocity.x > 0)
        )   return new Vector2(0, velocity.y);
        if 
        (
            (screenPosition.y < _screenBoarder && velocity.y < 0) || 
            (screenPosition.y > _camera.pixelHeight - _screenBoarder && velocity.y > 0)
        )   return new Vector2(velocity.x, 0);
        else return velocity;
    }
    public Vector2 boarderTurnAround(Vector2 targetDirection)
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        if 
        (
            (screenPosition.x < _screenBoarder && targetDirection.x < 0) || 
            (screenPosition.x > _camera.pixelWidth - _screenBoarder && targetDirection.x > 0)
        )   return new Vector2(-targetDirection.x, targetDirection.y);
        if 
        (
            (screenPosition.y < _screenBoarder && targetDirection.y < 0) || 
            (screenPosition.y > _camera.pixelHeight - _screenBoarder && targetDirection.y > 0)
        )   return new Vector2(targetDirection.x, -targetDirection.y);
        else return targetDirection;
    }
    public bool destroyWhenOffScreen(GameObject gameObject)
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        if 
        (
            screenPosition.x < 0 ||
            screenPosition.x > _camera.pixelWidth ||
            screenPosition.y < 0 ||
            screenPosition.y > _camera.pixelHeight
        )   return true; else return false;
    }
     public bool destroyWhenOffScreen(GameObject gameObject, float offset)
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        if 
        (
            screenPosition.x < 0 ||
            screenPosition.x > _camera.pixelWidth * offset ||
            screenPosition.y < 0 ||
            screenPosition.y > _camera.pixelHeight * offset
        )   return true; else return false;
    }
}