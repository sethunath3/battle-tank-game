﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankBattle.Tank;

namespace TankBattle.Bullet
{
    public class BulletView : MonoBehaviour
    {
        private BulletController controller;

        public void Start() {
            GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        }

        public void SetController(BulletController _controller)
        {
            controller = _controller;
        }

        public void DestroyBulletView()
        {
            Destroy(gameObject);
        }

        private void OnTriggerExit(Collider collider) 
        {
            if (collider.gameObject.layer == 8)
            {
                if (controller != null)
                {
                    controller.DestroyBullet();
                }
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            IDamageable damagableComponent = collision.gameObject.GetComponent<IDamageable>();
            if (damagableComponent != null)
            {
                if(damagableComponent.TakeDamage(controller.GetBulletDamagePower(), controller.sourceTank))
                {
                    controller.DestroyBullet();
                }
            }
            // if (collision.gameObject.tag == "Tank")
            // {
            //     TankController colliderTank = collision.gameObject.GetComponent<TankView>().GetController();
            //     if(colliderTank != controller.sourceTank)
            //     {
            //         colliderTank.ApplyDamage(controller.GetBulletDamagePower(), controller.sourceTank);
                    
                    
            //     }
            // }
        }   
    }
}
