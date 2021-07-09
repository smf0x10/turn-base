using UnityEngine;
using UnityEngine.UI;

public class Util : MonoBehaviour
{
    static Util instance; // The instance of the object
    public static GameState gameState = GameState.playerTurn;
    public GameObject damage; // The number that appears when something gets hurt
    public Transform canvas; // The canvas to draw the damage number on
    public Camera mainCamera; // The main camera object
    public float DMG_HORIZ_SPREAD; // The base horizontal spread of the numbers
    public float DMG_VERT_SPEED; // The vertical speed of the numbers
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Displays a number signifying damage
    /// </summary>
    /// <param name="thingHurt">The position of the thing to get hurt</param>
    /// <param name="value">The damage done</param>
    /// <param name="speedMult">Higher numbers make the number faster</param>
    public static void ShowDamageNumber(Vector3 thingHurt, int value, float speedMult = 1)
    {
        Debug.Log(Camera.current);
        GameObject go = Instantiate(instance.damage, instance.mainCamera.WorldToScreenPoint(thingHurt, Camera.MonoOrStereoscopicEye.Mono), new Quaternion(0, 0, 0, 0), instance.canvas);
        go.GetComponent<Text>().text = value.ToString();
        go.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-instance.DMG_HORIZ_SPREAD * speedMult, instance.DMG_HORIZ_SPREAD * speedMult), instance.DMG_VERT_SPEED * speedMult);
        Destroy(go, 10);
    }

}

public enum GameState
{
    playerTurn,
    playerChooseTargets,
    playerWalkToTarget,
    playerAttack,
    playerWalkBack,
    enemyTurn
}
