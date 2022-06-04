[System.Serializable]
public class PlayerData
{
    public float salud;
    public float [] position = new float [3];

    public PlayerData(PlayerMovement player)
    {
        salud = player.health;
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
