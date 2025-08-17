using UnityEngine;

    public abstract class  PassiveBase:MonoBehaviour
    {
        protected Player owner;
        
        public virtual void Intialize(Player player)
        {
            owner = player;
        }    

        
        
    }
