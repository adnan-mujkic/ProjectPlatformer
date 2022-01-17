using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VelocityTests
{
   [Test]
   public void JumpingGravityTest() {
      Assert.AreEqual(
         CharacterControl.GetGravityVelocity(2f, 0.1f), 
         new Vector3(0f, Physics.gravity.y / 10f, 0f));
   }

   [Test]
   public void MovementVelocityTest() {
      Assert.AreEqual(CharacterControl.GetMovementVelocity(10f, 10f), new Vector2(10f, 10f));
   }
}
