using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    internal abstract class OutOfBoundIndicator : MonoBehaviour
    {
        internal abstract void ShowIndicator(Vector3 position, Vector3 motorSpaceCenter, Side side);
        internal abstract void HideIndicator();
    }

}
