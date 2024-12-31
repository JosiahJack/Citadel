using System.Collections.Generic;

namespace CSM.Runtime.Utilities {
  /// <summary>
  /// Early stopping boolean logic iteration on enumerators
  /// </summary>
  static class BooleanIteratorLogic {
    internal static bool AllFalse(IEnumerator<bool> e) {
      while (e.MoveNext()) {
        if (e.Current) {
          return false;
        }
      }

      return true;
    }

    internal static bool Any(IEnumerator<bool> e) {
      while (e.MoveNext()) {
        if (e.Current) {
          return true;
        }
      }

      return false;
    }

    static bool Last(IEnumerator<bool> e) {
      var val = false;
      while (e.MoveNext()) {
        val = e.Current;
      }

      return val;
    }

    static bool OnlyLast(IEnumerator<bool> e) {
      var val = false;
      while (e.MoveNext()) {
        if (val) {
          return false;
        }

        val = e.Current;
      }

      return val;
    }

    internal static bool OnlyFirst(IEnumerator<bool> e) {
      e.MoveNext();
      var val = e.Current;
      while (e.MoveNext()) {
        if (e.Current) {
          return false;
        }
      }

      return val;
    }

    static bool First(IEnumerator<bool> e) {
      e.MoveNext();
      return e.Current;
    }
  }
}