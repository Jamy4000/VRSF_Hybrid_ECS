using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestUniTest {

	[Test]
	public void TestUniTestSimplePasses() {
        // Use the Assert class to test conditions.

        // Use AAA (Act Arrange Assert) patern

        // Act
        var expected = 3;

        // Arrange
        var operation = 2 + 1;

        // Assert
        Assert.IsTrue(operation == expected);

        // Both work, but this is a better version in this case, there is a lot of test case, choose wisely
        Assert.AreEqual(operation, expected);

    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator TestUniTestWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // Same here, this is a dumnb example, and we may need to rework the app code to improve testability.

        // Act
        var expectedPosition = new Vector3(0, 0, 0);

        // Arrange
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "UnitTestCube";
        cube.transform.position = new Vector3(0, 0, 0);

        // yield to skip a frame
        // Mostly used when you need to skip a framce from an update method
        yield return null;

        // Assert
        Assert.AreEqual(cube.transform.position, expectedPosition);
    }

    // Free the resources after each test
    [TearDown]
    public void CleanAfterEveryTest()
    {
        // Here we look for the cube, if we dont remove it, it stays in the scene.
        var cube = GameObject.Find("UnitTestCube");
        GameObject.DestroyImmediate(cube);
    }
}
