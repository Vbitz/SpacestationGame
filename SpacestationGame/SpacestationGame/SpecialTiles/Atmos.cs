using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Vbitz;

namespace SpacestationGame
{
    public class Gas : Entity
    {
        public Gas(string name)
        {

        }

        public virtual void OnPlayerTick(SSPlayer player, float level)
        {

        }
    }

    public class OxygenGas : Gas
    {
        public OxygenGas() : 
            base("Oxygen")
        {
        
        }

        public override void OnPlayerTick(SSPlayer player, float level)
        {
            if (player.OxygenLevel < 1.0f)
            {
                player.OxygenLevel += level;
                if (player.OxygenLevel > 1.0f)
                {
                    player.OxygenLevel = 1.0f;
                }
            }
        }
    }

    public class NitrogenGas : Gas
    {
        public NitrogenGas() : 
            base("Nitrogen")
        {
        
        }
    }

    public class Atmos : SSTile
    {
        public Atmos(AtmosType type) :
            base(SSTileTypes.Atmos, "Atmos (Hidden)")
        {
            switch (type)
            {
                case AtmosType.Normal:
                    AddGas(new OxygenGas(), 0.3f);
                    AddGas(new NitrogenGas(), 0.7f);
                    break;
                case AtmosType.Space:
                    break;
                default:
                    break;
            }
        }

        public void AddGas(Gas type, float level)
        {

        }

        public override void OnPlayerTick(SSPlayer ply)
        {

        }
    }
}
