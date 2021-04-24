using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuChanger.Components
{
    public class Updater : MonoBehaviour
    {
        public Action action = () => { };

        int counter = 0;
        public void Update()
        {
            if (counter != 0) counter = (counter + 1) % 8;
            else
            {
                action();
                counter++;
            }
        }
    }
}
