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

namespace SpacestationGame
{
    public enum EntityRequestType
    {
        Add,
        Remove
    }

    class EntityRequest
    {
        public Entity Target;
        public EntityRequestType Type;

        public EntityRequest(Entity ent, EntityRequestType type)
        {
            this.Target = ent;
            this.Type = type;
        }
    }

    public class EntityContainer : Entity
    {
        private List<EntityRequest> Requests = new List<EntityRequest>();

        public List<Entity> Items = new List<Entity>();

        public override void Draw(MainGame game, EntityContainer parent)
        {
            foreach (Entity item in this.Items)
            {
                item.Draw(game, this);
            }
        }

        public override void Update(MainGame game, EntityContainer parent, GameTime time)
        {
            foreach (Entity item in this.Items)
            {
                item.Update(game, this, time);
            }
        }

        public void Add(Entity ent)
        {
            this.Requests.Add(new EntityRequest(ent, EntityRequestType.Add));
        }

        public void Remove(Entity ent)
        {
            this.Requests.Add(new EntityRequest(ent, EntityRequestType.Remove));
        }

        public void Update()
        {
            foreach (EntityRequest item in Requests)
            {
                switch (item.Type)
                {
                    case EntityRequestType.Add:
                        this.Items.Add(item.Target);
                        break;
                    case EntityRequestType.Remove:
                        this.Items.Remove(item.Target);
                        break;
                    default:
                        break;
                }
            }

            Requests.Clear();
        }
    }
}
