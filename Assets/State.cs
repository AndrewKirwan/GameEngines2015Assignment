using UnityEngine;
using System.Collections;

namespace BGE
{

    public abstract class State
    {

        public ResourceFSM owner;

        public State(ResourceFSM owner)
        {
            this.owner = owner;
        }

        public abstract string Description();
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}
