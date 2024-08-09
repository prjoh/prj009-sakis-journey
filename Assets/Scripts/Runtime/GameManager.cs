using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : SingletonBehaviour<GameManager>
{
  [SerializeField] private GameObject mainMenuUI;
  [SerializeField] private GameObject playerUI;
  [SerializeField] private GameSceneObject firstGameScene;
  [SerializeField] private GameSceneObject debugGameScene;

  private GameSceneObject m_currentGameScene = null;
  public GameSceneObject CurrentGameScene
  {
    get { return m_currentGameScene; }
  }

  private CameraController m_cameraController;

  protected override void Awake()
  {
    base.Awake();

    Debug.Assert(firstGameScene, "[GameManager] No GameSceneObject reference detected! Please make sure to set it in the editor.");

    // TODO: Development mode
    // m_currentGameScene = debugGameScene;
    // SceneManager.LoadScene(debugGameScene.scene.name, LoadSceneMode.Single);

    m_cameraController = Camera.main.GetComponent<CameraController>();
    m_cameraController.SetMovementState(eCameraMovementState.Focused, transform);

    // LoadFirstScene();
    EnableMainMenuUI(true);
    EnablePlayerUI(false);
  }

  public void Start()
  {
    m_cameraController.DoFadeIn(Color.white);
  }

  public void NewGame()
  {
    EnableMainMenuUI(false);

    m_currentGameScene = firstGameScene;
    
    var asyncOp = SceneManager.LoadSceneAsync(firstGameScene.scene.name, LoadSceneMode.Additive);
    
    asyncOp.completed += (op) => {
      EnablePlayerUI(true);

      var playerSpawner = GameObject.Find("InitialPlayerSpawner").GetComponent<PlayerSpawner>();
      var playerTransform = playerSpawner.SpawnPlayer();
      
      m_cameraController.SetMovementState(m_currentGameScene.initialCameraMovementState, playerTransform);
      m_cameraController.DoFadeIn(m_currentGameScene.fadeInColor);
    
      PlayerData.Instance.BindPlayerTransform(playerTransform); // TODO: Total bullcrap!

      DataPersistenceManager.Instance.CreateNewGame();
    };
  }

  public void LoadGame()
  {
    EnableMainMenuUI(false);

    // TODO: We have to load the file first, and fill in game objects at different times???

    m_currentGameScene = firstGameScene;

    var asyncOp = SceneManager.LoadSceneAsync(firstGameScene.scene.name, LoadSceneMode.Additive);

    asyncOp.completed += (op) => {
      EnablePlayerUI(true);

      var playerSpawner = GameObject.Find("InitialPlayerSpawner").GetComponent<PlayerSpawner>();
      var playerTransform = playerSpawner.SpawnPlayer();
      
      m_cameraController.SetMovementState(m_currentGameScene.initialCameraMovementState, playerTransform);
      m_cameraController.DoFadeIn(m_currentGameScene.fadeInColor);
    
      PlayerData.Instance.BindPlayerTransform(playerTransform); // TODO: Total bullcrap!

      DataPersistenceManager.Instance.LoadGame();
    };
  }

  private void EnableMainMenuUI(bool isActive)
  {
    mainMenuUI.SetActive(isActive);
  }

  private void EnablePlayerUI(bool isActive)
  {
    playerUI.SetActive(isActive);
  }

  // TODO: Bad dependecies... SceneTransitionID, eSpriteDirection
  public void LoadGameScene(GameSceneObject gameScene, SceneTransitionID transitionId, eSpriteDirection playerDirection)
  {
    m_cameraController.SetMovementState(eCameraMovementState.Focused, transform);  // TODO: Avoiding null reference

    SceneManager.UnloadSceneAsync(m_currentGameScene.scene.name);
    m_currentGameScene = gameScene;

    var asyncOp = SceneManager.LoadSceneAsync(m_currentGameScene.scene.name, LoadSceneMode.Additive);
    asyncOp.completed += (op) => {
      var sceneTransitionObject = GameObject.Find(transitionId.GetID());
      var playerSpawner = sceneTransitionObject.GetComponentInChildren<PlayerSpawner>();
      var playerTransform = playerSpawner.SpawnPlayer();

      var graphicsController = playerTransform.GetComponentInChildren<GraphicsController>();
      graphicsController.SetSpriteDirection(playerDirection);

      m_cameraController.SetMovementState(m_currentGameScene.initialCameraMovementState, playerTransform);
      m_cameraController.DoFadeIn(m_currentGameScene.fadeInColor);
    };
  }

  private void OnApplicationQuit()
  {
    DataPersistenceManager.Instance.SaveGame();
  }
}
