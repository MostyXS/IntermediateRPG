using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace RPG.Extensions
{
    public static class Extensions
    {
        public static void Clear(this Transform transform)
        {

            foreach(Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }



    }
}
