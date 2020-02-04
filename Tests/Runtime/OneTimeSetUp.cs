using NUnit.Framework;
using UnityEngine.SceneManagement;

namespace PlayModeTests.Tests
{
    public class OneTimeSetUp
    {
        [OneTimeSetUp]
        public void SetUpOnce()
        {
            SceneManager.CreateScene("TestScene");
        }
    }
}