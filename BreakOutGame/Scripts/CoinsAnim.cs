// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CoinsAnim : MonoBehaviour
// {
//     Animator animator;
//     public BouncyBall bouncyBall;
//     private int lastCoins;
//     // Start is called before the first frame update
//     void Start()
//     {
//         animator = GetComponent<Animator>();
//         if (bouncyBall != null)
//         {
//             lastCoins = bouncyBall.coins;
//             Debug.Log("Initial lastCoins: " + lastCoins);
//         }

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (bouncyBall.coins != lastCoins)
//         {
//             animator.SetTrigger("GoAnim");
//             lastCoins = bouncyBall.coins;
//             Debug.Log("Coins changed! New coins: " + lastCoins);
//         }
//     }
// }
