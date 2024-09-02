using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public Transform _spawnPoint;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public LayerMask hazardLayer;
    [SerializeField] public LayerMask collectableLayer;
    [SerializeField] public LayerMask interactableLayer;

}
