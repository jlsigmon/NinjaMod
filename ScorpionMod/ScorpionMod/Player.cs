using UnityEngine;

namespace NinjaMod
{
    public class Player
    {
        public PlayerControl playerdata;
        public bool isNinja;

        public Player(PlayerControl playerdata)
        {
            this.playerdata = playerdata;
            isNinja = false;
        }
        
        public void Update()
        {
            if (isNinja & (CustomGameOptions.ShowNinja | this == PlayerController.getLocalPlayer()))
            {
                playerdata.nameText.Color = new Color(48 / 255.0f, 223 / 255.0f, 48 / 255.0f);
            }
        }
    }
}