using System;
using System.Windows;
using System.Windows.Media;

namespace Common.Presentation {
  /// <summary>
  ///   Defines a set of extension methods for <see cref="DependencyObject">Dependency Objects</see>.
  /// </summary>
  /// <threadsafety static="false" instance="false" />
  public static class DependencyObjectEx {
    public static DependencyObject GetParentRoot(this DependencyObject dependencyObject) {
      if (dependencyObject == null) {
        return null;
      }
      
      DependencyObject current = dependencyObject;
      while (true) {
        DependencyObject parent = VisualTreeHelper.GetParent(current);

        if (parent == null) {
          return current;
        }
          
        current = parent;
      }
    }

    public static TypeToFind GetClosestParentOfType<TypeToFind>(this DependencyObject dependencyObject) 
      where TypeToFind: DependencyObject 
    {
      if (dependencyObject == null) {
        return null;
      }

      while (true) {
        dependencyObject = VisualTreeHelper.GetParent(dependencyObject);

        if ((dependencyObject == null) || (dependencyObject is TypeToFind)) {
          return (TypeToFind)dependencyObject;
        }
      }
    }
  }
}
