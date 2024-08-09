using UnityEngine;


public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public Transform SpawnPlayer()
    {
      Debug.Assert(playerPrefab, "[PlayerSpawner] No player prefab reference fount! Please make sure to add it in the editor.");
      var player = Instantiate(playerPrefab, transform.position, Quaternion.identity, gameObject.transform);
      player.transform.SetParent(null);
      return player.transform;
    }
}
