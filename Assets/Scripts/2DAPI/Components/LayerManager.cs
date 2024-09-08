using UnityEngine;

public class LayerManager : MonoBehaviour
{
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public LayerMask hazardLayer;
    [SerializeField] public LayerMask collectableLayer;
    [SerializeField] public LayerMask interactableLayer;
    [SerializeField] public LayerMask platformLayer;

}
