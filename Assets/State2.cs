using UnityEngine;
using System.Collections;

namespace BGE
{

    public abstract class State2
    {

        public FighterFSM owner;

        public State2(FighterFSM owner)
        {
            this.owner = owner;
        }

        public abstract string Description();
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}
