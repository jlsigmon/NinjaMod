using UnityEngine;

namespace ScorpionMod
{
    public class Player
    {
        public PlayerControl playerdata;
        public bool isScorpion;

        public Player(PlayerControl playerdata)
        {
            this.playerdata = playerdata;
            isScorpion = false;
        }
        
        public void Update()
        {
            if (isScorpion & (CustomGameOptions.ShowScorpion | this == PlayerController.getLocalPlayer()))
            {
                playerdata.nameText.Color = new Color(48 / 255.0f, 223 / 255.0f, 48 / 255.0f);
            }
        }
    }
}