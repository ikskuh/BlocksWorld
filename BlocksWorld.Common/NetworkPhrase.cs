using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksWorld
{
    public enum NetworkPhrase
    {
        None,
        
        /****************************
         * Client & Server 
         ****************************/
        SetBlock,
        RemoveBlock,

        UpdateDetail,
        DestroyDetail,

        /****************************
         * To Client Only 
         ****************************/
        UpdateProxy,
        DestroyProxy,

        LoadWorld,
        SpawnPlayer,

        CreateDetail,

        /****************************
         * To Server Only 
         ****************************/
        SetPlayer,
        CreateNewDetail,
    }
}
