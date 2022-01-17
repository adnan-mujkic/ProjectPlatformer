using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VelocityTests
{
   [Test]
   public void JumpingVelocityTest() {
      Assert.AreEqual(CharacterControl.GetJunmpingVelocity(10f), new Vector2(0f, 10f));
   }

   [Test]
   public void MovementVelocityTest() {
      Assert.AreEqual(CharacterControl.GetMovementVelocity(10f, 10f), new Vector2(10f, 10f));
   }
}
