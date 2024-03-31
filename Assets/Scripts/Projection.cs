using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    [SerializeField] private Transform[] _obstacles;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations;
    
     
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;

    private void Start()
    {
        CreatePhysicsScene();
    }

    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("SimulationPhysics", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        AddObstacles();

    }

    private void AddObstacles()
    {
        foreach (Transform obj in _obstacles)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            ghostObj.GetComponent<Renderer>().enabled = false;
            if (ghostObj.TryGetComponent<SoundEmitter>(out SoundEmitter soundEmitter))
            {
                soundEmitter.enabled = false;
            }
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
        }
    }

    public void SimulateTrajectory(GrenadeBall ball, Vector3 pos, Vector3 velocity)
    {
        var ghostObj = Instantiate(ball, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);
        
        ghostObj.Init(velocity);
        
        _line.positionCount = _maxPhysicsFrameIterations;

        for (int i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostObj.transform.position);
        }
        
        Destroy(ghostObj.gameObject);
    }
}