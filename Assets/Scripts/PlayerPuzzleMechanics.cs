using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPuzzleMechanics : MonoBehaviour
{
    /// <summary>
    /// The watering can.
    /// </summary>
    [SerializeField]
    private GameObject _wateringCan;

    /// <summary>
    /// The collision object for the fountain.
    /// </summary>
    [SerializeField]
    private GameObject _fountainWater;

    /// <summary>
    /// The collision object for the dead section of the maze
    /// </summary>
    [SerializeField]
    private GameObject _deadGarden;

    private Vector3 _rayOrigin;

    private float _maxRayDistance;

    private string _wateringCanName;
    private string _fountainWaterName;
    private string _deadGardenName;

	private void Start()
    {
        _wateringCanName = _wateringCan.name;
        _fountainWaterName = _fountainWater.name;
        _deadGardenName = _deadGarden.name;
	}
	
	private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(_rayOrigin, Vector3.forward, out hit, _maxRayDistance);

            hit.collider.name;
        }
	}

}
