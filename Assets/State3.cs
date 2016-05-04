using UnityEngine;
using System.Collections;

namespace BGE
{

    public abstract class State3
    {

        public HunterFSM owner;

        public State3(HunterFSM owner)
        {
            this.owner = owner;
        }

        public abstract string Description();
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}
