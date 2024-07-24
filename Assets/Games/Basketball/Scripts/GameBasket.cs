using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tool;
using UnityEngine;

namespace Basket
{
    [CreateAssetMenu(fileName = "Game_Basket", menuName = "Game/Basket/Game_Basket")]
    public class GameBasket : GameSO
    {
        public async override Task GameInit()
        {
            try
            {
                Debug.Log("Basket");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}