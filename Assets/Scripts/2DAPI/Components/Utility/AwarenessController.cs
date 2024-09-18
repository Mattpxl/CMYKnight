using System.Collections;
using UnityEngine;
public class AwarenessController : MonoBehaviour
{
    public float _AwarenessDistance;
    public bool isAware;
    public float _awarenessDelay;
    private Vector2 _targetDirection, _target;
    public bool canCheck = true;
    public void setTarget(Vector2 target) { 
        _target = target;
    }
    public void awarenessCheck(){  // used in Update
        Vector2 targetVector = _target - (Vector2)transform.position;
        isAware = targetVector.magnitude <= _AwarenessDistance ? true : false; // sets awareness state 
        _targetDirection = isAware ? targetVector.normalized : Vector2.zero; // update target direction
    }
    public Vector2 getCurrentTarget(){
        if (isAware) return this._target;
        else return Vector2.zero;
    }
    public Vector2 getTargetDirection(){
        if (isAware) return _targetDirection;
        else return Vector2.zero;
    }
    public IEnumerator AwarenessDelay()
    {
        canCheck = false;
        yield return new WaitForSeconds(_awarenessDelay);
        canCheck = true;
    }
}
