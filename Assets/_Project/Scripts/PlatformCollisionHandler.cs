using System;
using UnityEngine;

namespace Platformer._Project.Scripts
{
    public class PlatformCollisionHandler : MonoBehaviour
    {
        private Transform platform; // The platform if any we are on top of

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                // if the contact normal is pointing up, we are on top of the platform
                if (other.contacts[0].normal.y < 0.5f) return;
                
                platform = other.transform;
                transform.SetParent(platform);
            }
        }
    }
}