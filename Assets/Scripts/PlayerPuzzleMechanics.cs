using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPuzzleMechanics : MonoBehaviour
{
    /// <summary>
    /// The watering can to pick up
    /// </summary>
    [SerializeField]
    private GameObject _wateringCan;

    /// <summary>
    /// The held watering can
    /// </summary>
    [SerializeField]
    private GameObject _heldWateringCan;

    /// <summary>
    /// The collision object for the fountain
    /// </summary>
    [SerializeField]
    private GameObject _fountainWater;

    /// <summary>
    /// The held full watering can
    /// </summary>
    [SerializeField]
    private GameObject _heldWateringCanFull;

    /// <summary>
    /// The collision object for the dead section of the maze
    /// </summary>
    [SerializeField]
    private GameObject _deadGarden;

    /// <summary>
    /// The gate at the end of the maze
    /// </summary>
    [SerializeField]
    private Animator _gardenGate;

    /// <summary>
    /// The wood log to pick up
    /// </summary>
    [SerializeField]
    private GameObject _woodLog;

    /// <summary>
    /// The held wood log
    /// </summary>
    [SerializeField]
    private GameObject _heldWoodLog;

    /// <summary>
    /// The table saw
    /// </summary>
    [SerializeField]
    private GameObject _tableSaw;

    /// <summary>
    /// The held axe handle
    /// </summary>
    [SerializeField]
    private GameObject _heldAxeHandle;

    /// <summary>
    /// The axe head to use with the handle
    /// </summary>
    [SerializeField]
    private GameObject _axeHead;

    /// <summary>
    /// The held assembled axe
    /// </summary>
    [SerializeField]
    private GameObject _heldAxeFinal;

    /// <summary>
    /// The tree to cut down
    /// </summary>
    [SerializeField]
    private GameObject _bridgeTree;

    /// <summary>
    /// The bridge between islands 1 and 3
    /// </summary>
    [SerializeField]
    private Animator _finalBridge;

    /// <summary>
    /// The frozen gate lock
    /// </summary>
    [SerializeField]
    private GameObject _gateLock;

    /// <summary>
    /// The winter gate
    /// </summary>
    [SerializeField]
    private Animator _winterGate;

    /// <summary>
    /// The spring essence to find
    /// </summary>
    [SerializeField]
    private GameObject _springEssence;

    /// <summary>
    /// The summer essence to find
    /// </summary>
    [SerializeField]
    private GameObject _summerEssence;

    /// <summary>
    /// The fall essence to find
    /// </summary>
    [SerializeField]
    private GameObject _fallEssence;

    /// <summary>
    /// The winter essence to find
    /// </summary>
    [SerializeField]
    private GameObject _winterEssence;

    /// <summary>
    /// The held spring essence
    /// </summary>
    [SerializeField]
    private GameObject _heldSpringEssence;

    /// <summary>
    /// The held summer essence
    /// </summary>
    [SerializeField]
    private GameObject _heldSummerEssence;

    /// <summary>
    /// The held fall essence
    /// </summary>
    [SerializeField]
    private GameObject _heldFallEssence;

    /// <summary>
    /// The held winter essence
    /// </summary>
    [SerializeField]
    private GameObject _heldWinterEssence;

    /// <summary>
    /// The spring essence in it's holder
    /// </summary>
    [SerializeField]
    private GameObject _finalSpringEssence;

    /// <summary>
    /// The summer essence in it's holder
    /// </summary>
    [SerializeField]
    private GameObject _finalSummerEssence;

    /// <summary>
    /// The fall essence in it's holder
    /// </summary>
    [SerializeField]
    private GameObject _finalFallEssence;

    /// <summary>
    /// The winter essence in it's holder
    /// </summary>
    [SerializeField]
    private GameObject _finalWinterEssence;

    /// <summary>
    /// The spring essence holder
    /// </summary>
    [SerializeField]
    private GameObject _springHolder;

    /// <summary>
    /// The summer essence holder
    /// </summary>
    [SerializeField]
    private GameObject _summerHolder;

    /// <summary>
    /// The fall essence holder
    /// </summary>
    [SerializeField]
    private GameObject _fallHolder;

    /// <summary>
    /// The winter essence holder
    /// </summary>
    [SerializeField]
    private GameObject _winterHolder;

    [SerializeField]
    private AudioClip _waterFillingSound;

    [SerializeField]
    private AudioClip _wateringSound;

    [SerializeField]
    private AudioClip _sawSound;

    [SerializeField]
    private AudioClip _choppingSound;

    [SerializeField]
    private AudioClip _iceMeltingSound;

    [SerializeField]
    private AudioClip _essenceSound;

    [SerializeField]
    private AudioClip _essenceHolderSound;

    /// <summary>
    /// The empty object from which to raycast
    /// </summary>
    [SerializeField]
    private Transform _rayOrigin;

    /// <summary>
    /// The max reach for the player
    /// </summary>
    [SerializeField]
    private float _maxRayDistance;

    // Spring puzzle objects
    private const string WateringCanName     = "WATERING_CAN_EMPTY=0";
    private const string FountainWaterName   = "FOUNTAIN_WATER=0";
    private const string FullWateringCanName = "WATERING_CAN_FULL=1";
    private const string DeadGardenName      = "DEAD_GARDEN=1";

    // Summer puzzle objects
    private const string WoodLogName    = "WOOD_LOG=2";
    private const string TableSawName   = "TABLE_SAW=2";
    private const string AxeHandleName  = "AXE_HANDLE=3";
    private const string AxeHeadName    = "AXE_HEAD=3";
    private const string FinalAxeName   = "FINAL_AXE=4";
    private const string BridgeTreeName = "BRIDGE_TREE=4";

    // Winter puzzle objects
    private const string GateLockName = "GATE_LOCK=5";

    // Essences in the wild
    private const string SpringEssenceName = "SPRING_ESSENCE=-1";
    private const string SummerEssenceName = "SUMMER_ESSENCE=-2";
    private const string FallEssenceName   = "FALL_ESSENCE=-3";
    private const string WinterEssenceName = "WINTER_ESSENCE=-4";

    // Essences in their holders
    private const string FinalSpringEssenceName = "FINAL_SPRING_ESSENCE=-1";
    private const string FinalSummerEssenceName = "FINAL_SUMMER_ESSENCE=-2";
    private const string FinalFallEssenceName   = "FINAL_FALL_ESSENCE=-3";
    private const string FinalWinterEssenceName = "FINAL_WINTER_ESSENCE=-4";

    // Essence holders
    private const string SpringHolderName = "SPRING_HOLDER=-1";
    private const string SummerHolderName = "SUMMER_HOLDER=-2";
    private const string FallHolderName   = "FALL_HOLDER=-3";
    private const string WinterHolderName = "WINTER_HOLDER=-4";

    // Comes from the bridge tree gameobject
    private Animator _bridgeTreeAnimator;

    private string _currentlyHeldName;
    
    private AudioSource _soundEffect;

    private void Awake()
    {
        _currentlyHeldName = string.Empty;
        _soundEffect = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _bridgeTreeAnimator = _bridgeTree.GetComponent<Animator>();

        _wateringCan.name   = WateringCanName;
        _fountainWater.name = FountainWaterName;
        _deadGarden.name    = DeadGardenName;
        _woodLog.name       = WoodLogName;
        _tableSaw.name      = TableSawName;
        _axeHead.name       = AxeHeadName;
        _bridgeTree.name    = BridgeTreeName;
        _gateLock.name      = GateLockName;

        _springEssence.name = SpringEssenceName;
        _summerEssence.name = SummerEssenceName;
        _fallEssence.name   = FallEssenceName;
        _winterEssence.name = WinterEssenceName;

        _finalSpringEssence.name = FinalSpringEssenceName;
        _finalSummerEssence.name = FinalSummerEssenceName;
        _finalFallEssence.name   = FinalFallEssenceName;
        _finalWinterEssence.name = FinalWinterEssenceName;

        _springHolder.name = SpringHolderName;
        _summerHolder.name = SummerHolderName;
        _fallHolder.name   = FallHolderName;
        _winterHolder.name = WinterHolderName;
	}
	
	private void Update()
    {
#if DEBUG
        Debug.DrawLine(_rayOrigin.position, _rayOrigin.position + _rayOrigin.forward * _maxRayDistance, Color.magenta);
#endif
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(_rayOrigin.position, _rayOrigin.forward, out hit, _maxRayDistance))
            {
#if DEBUG
                print("Hit: " + hit.collider.name);
#endif
                string hitName = hit.collider.name;

                switch (hitName)
                {
                    case WateringCanName:
                    case WoodLogName:
                    case SpringEssenceName:
                    case SummerEssenceName:
                    case FallEssenceName:
                    case WinterEssenceName:
                    case FinalSpringEssenceName:
                    case FinalSummerEssenceName:
                    case FinalFallEssenceName:
                    case FinalWinterEssenceName:
                    {
                        TryHold(hitName);
                        break;
                    }

                    case FountainWaterName:
                    case DeadGardenName:
                    case TableSawName:
                    case AxeHeadName:
                    case BridgeTreeName:
                    case GateLockName:
                    case SpringHolderName:
                    case SummerHolderName:
                    case FallHolderName:
                    case WinterHolderName:
                    {
                        TryActivate(hit.collider.name);
                        break;
                    }
                }
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        int seasonId = int.Parse(other.name.Split('=')[1]);

        switch (seasonId)
        {
            case 1:
            {
                break;
            }

            case 2:
            {
                break;
            }

            case 3:
            {
                break;
            }

            case 4:
            {
                break;
            }
        }
    }

    private void TryHold(string objectNameToHold)
    {
#if DEBUG
        print("TryHold: " + objectNameToHold);
#endif

        if (_currentlyHeldName.Equals(string.Empty))
        {
            _currentlyHeldName = objectNameToHold;

            switch (_currentlyHeldName)
            {
                case WateringCanName:
                {
                    _wateringCan.SetActive(false);
                    _heldWateringCan.SetActive(true);
                    break;
                }
                case WoodLogName:
                {
                    _woodLog.SetActive(false);
                    _heldWoodLog.SetActive(true);
                    break;
                }
                case SpringEssenceName:
                {
                    _springEssence.SetActive(false);
                    _heldSpringEssence.SetActive(true);

                    _soundEffect.PlayOneShot(_essenceSound);

                    _gardenGate.Play("gateOpening_anim");

                    break;
                }
                case SummerEssenceName:
                {
                    _summerEssence.SetActive(false);
                    _heldSummerEssence.SetActive(true);

                    _soundEffect.PlayOneShot(_essenceSound);

                    _finalBridge.Play("bridgeAppearing_anim");

                    break;
                }
                case FallEssenceName:
                {
                    _fallEssence.SetActive(false);
                    _heldFallEssence.SetActive(true);

                    _soundEffect.PlayOneShot(_essenceSound);

                    break;
                }
                case WinterEssenceName:
                {
                    _winterEssence.SetActive(false);
                    _heldWinterEssence.SetActive(true);

                    _soundEffect.PlayOneShot(_essenceSound);

                    break;
                }
                case FinalSpringEssenceName:
                {
                    _finalSpringEssence.SetActive(false);
                    _heldSpringEssence.SetActive(true);

                    _soundEffect.PlayOneShot(_essenceSound);

                    break;
                }
                case FinalSummerEssenceName:
                {
                    _finalSummerEssence.SetActive(false);
                    _heldSummerEssence.SetActive(true);

                    _soundEffect.PlayOneShot(_essenceSound);

                    break;
                }
                case FinalFallEssenceName:
                {
                    _finalFallEssence.SetActive(false);
                    _heldFallEssence.SetActive(true);

                    _soundEffect.PlayOneShot(_essenceSound);

                    break;
                }
                case FinalWinterEssenceName:
                {
                    _finalWinterEssence.SetActive(false);
                    _heldWinterEssence.SetActive(true);

                    _soundEffect.PlayOneShot(_essenceSound);

                    break;
                }
            }
        }
        else
        {
            // TODO: display "Can't hold" message ???
        }
#if DEBUG
        print("Currently Holding: " + _currentlyHeldName);
#endif
    }

    private void TryActivate(string objectNameToActivate)
    {
#if DEBUG
        print("TryActivate: " + objectNameToActivate);
#endif
        if (!_currentlyHeldName.Equals(string.Empty))
        {
            int heldId = int.Parse(_currentlyHeldName.Split('=')[1]);
            int objectId = int.Parse(objectNameToActivate.Split('=')[1]);

            if (heldId == objectId)
            {
                print("Equal objects");
                switch (heldId)
                {
                    // Fill the watering can
                    case 0:
                    {
                        _heldWateringCan.SetActive(false);
                        _heldWateringCanFull.SetActive(true);
                        _currentlyHeldName = FullWateringCanName;

                        _soundEffect.PlayOneShot(_waterFillingSound);

                        break;
                    }

                    // Water the garden
                    case 1:
                    {
                        _heldWateringCanFull.SetActive(false);
                        _currentlyHeldName = string.Empty;

                        _soundEffect.PlayOneShot(_wateringSound);

                        // Remove barrier
                        _deadGarden.SetActive(false);

                        // Change dead garden to live garden

                        break;
                    }

                    // Cut the log
                    case 2:
                    {
                        _heldWoodLog.SetActive(false);
                        _heldAxeHandle.SetActive(true);
                        _currentlyHeldName = AxeHandleName;

                        _soundEffect.PlayOneShot(_sawSound);

                        break;
                    }

                    // Create the axe
                    case 3:
                    {
                        _axeHead.SetActive(false);
                        _heldAxeHandle.SetActive(false);
                        _heldAxeFinal.SetActive(true);
                        _currentlyHeldName = FinalAxeName;
                        break;
                    }

                    // Chop the tree
                    case 4:
                    {
                        _heldAxeFinal.SetActive(false);
                        _currentlyHeldName = string.Empty;

                        _soundEffect.PlayOneShot(_choppingSound);

                        _bridgeTreeAnimator.Play("treeFalling_anim");

                        // Remove barrier

                        break;
                    }

                    // Put spring essence in it's holder
                    case -1:
                    {
                        _heldSpringEssence.SetActive(false);
                        _finalSpringEssence.SetActive(true);
                        _currentlyHeldName = string.Empty;

                        _soundEffect.PlayOneShot(_essenceHolderSound);

                        break;
                    }

                    // Put summer essence in it's holder
                    case -2:
                    {
                        _heldSummerEssence.SetActive(false);
                        _finalSummerEssence.SetActive(true);
                        _currentlyHeldName = string.Empty;

                        _soundEffect.PlayOneShot(_essenceHolderSound);

                        break;
                    }

                    // Put fall essence in it's holder
                    case -3:
                    {
                        _heldFallEssence.SetActive(false);
                        _finalFallEssence.SetActive(true);
                        _currentlyHeldName = string.Empty;

                        _soundEffect.PlayOneShot(_essenceHolderSound);

                        break;
                    }

                    // Put winter essence in it's holder
                    case -4:
                    {
                        _heldWinterEssence.SetActive(false);
                        _finalWinterEssence.SetActive(true);
                        _currentlyHeldName = string.Empty;

                        _soundEffect.PlayOneShot(_essenceHolderSound);

                        break;
                    }
                }
            }
            // Disgusting special case to melt the ice
            else if (heldId == int.Parse(SummerEssenceName.Split('=')[1]) &&
                     objectId == int.Parse(GateLockName.Split('=')[1]))
            {
                _gateLock.SetActive(false);

                _soundEffect.PlayOneShot(_iceMeltingSound);

                _winterGate.Play("winterGateOpening_anim");

            }
        }
#if DEBUG
        print("Currently Holding: " + _currentlyHeldName);
#endif
    }
}
