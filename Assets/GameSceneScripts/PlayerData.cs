[System.Serializable]
public class PlayerData
{
    //Clase para establecer los datos que se pueden guardar del jugador. En este caso, 
    //la salud y la posicion del personaje

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
