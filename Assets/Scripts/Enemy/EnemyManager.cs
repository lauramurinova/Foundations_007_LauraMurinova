using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemyController> enemies;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _ragdollPrefab;
    [SerializeField] private float _threshold = 0.5f;
    [SerializeField] private float coolDownTime = 30f;
    
    public List<Vector3> _investigationPoints = new List<Vector3>();
    private static EnemyManager _instance = null;
    
    [Button("Add Enemy")]
    private void AddPatrolPoint()
    {
        EnemyController newEnemy = Instantiate(_enemyPrefab, transform).GetComponent<EnemyController>();
        newEnemy.name = "Robot_Enemy (" + (enemies.Count + 1) + ")";
        enemies.Add(newEnemy);
        
    #if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(newEnemy, "Add Enemy");
    #endif
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    

    /// <summary>
    /// Destroys the robot by disabling the original one and enabling the ragdoll robot.
    /// </summary>
    public void DestroyRobot(EnemyController robot)
    {
        robot.DisableMovement();
        StartCoroutine(RagdollTransition(robot, 0.5f));
    }

    /// <summary>
    /// Smooth transition of enemy robot to ragdoll in given duration.
    /// </summary>
    private IEnumerator RagdollTransition(EnemyController robot, float duration)
    {
        yield return new WaitForSeconds(duration);
        Instantiate(_ragdollPrefab, robot.transform.position, robot.transform.rotation, transform);
        if (enemies.Remove(robot))
        {
            Destroy(robot.gameObject);
        }
    }
    

    /// <summary>
    /// Starts the time while the players can investigate the same point - giving the player the option to escape.
    /// Duration time is based on the coolDownTime variable.
    /// </summary>
    public void StartCoolDown(Vector3 investigationPoint, float distanceFactor)
    {
        StartCoroutine(CoolDownTime(investigationPoint, distanceFactor));
    }
    
    /// <summary>
    /// Cool down time counter.
    /// </summary>
    private IEnumerator CoolDownTime(Vector3 investigatePoint, float distanceTogo)
    {
        yield return new WaitForSeconds(coolDownTime);
        _investigationPoints.Remove(investigatePoint);
    }

    /// <summary>
    /// Returns the singleton instance of the manager.
    /// </summary>
    public static EnemyManager GetInstance()
    {
        return _instance;
    }

    /// <summary>
    /// Returns the list of the enemies in the game.
    /// </summary>
    /// <returns></returns>
    public List<EnemyController> GetEnemies()
    {
        return enemies;
    }
    
    /// <summary>
    /// Checks whether the point can be investigated, true if enough time passed from the last investigation of the point, false if not.
    /// </summary>
    public bool CanInvestigatePoint(Vector3 investigationPoint)
    {
        return !IsPointInvestigated(investigationPoint);
    }

    /// <summary>
    /// Adds the point as investigated thanks to the investigation points list.
    /// </summary>
    public void MarkInvestigationPoint(Vector3 investigationPoint)
    {
        _investigationPoints.Add(investigationPoint);
    }

    /// <summary>
    ///  Returns whether a point is being investigated or not, based on if the point is in the investigation points list.
    /// </summary>
    private bool IsPointInvestigated(Vector3 point)
    {
        foreach (Vector3 investigationPoint in _investigationPoints)
        {
            if (Vector3.Distance(point, investigationPoint) < _threshold)
            {
                return true;
            }
        }
        return false;
    }
}
