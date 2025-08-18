using UnityEngine;

    public abstract class  PassiveBase:MonoBehaviour
    {
        protected PlayerController owner;
        
        public virtual void Intialize(PlayerController player)
        {
            owner = player;
        }    

        
        
    }
