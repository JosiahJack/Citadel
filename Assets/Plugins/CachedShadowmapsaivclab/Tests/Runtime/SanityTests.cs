using NUnit.Framework;

namespace CSM.Tests.Runtime {
  /// <summary>
  ///
  /// </summary>
  [TestFixture]
  public class SanityTests {
    /// <summary>
    /// 
    /// </summary>
    [Test]
    public void Sanity() { Assert.That(true, expression : Is.True); }
  }
}
