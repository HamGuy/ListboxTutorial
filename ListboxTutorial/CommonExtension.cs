using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Unique
{
    public static class CommonExtension
    {
        #region DependencyObject Extension

        /// <summary>
        /// 返回可视树中该元素的所有子元素
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> ChildrenEx(this DependencyObject item)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(item);
            for (var i = 0; i < childrenCount; i++)
            {
                yield return VisualTreeHelper.GetChild(item, i);
            }
        }

        /// <summary>
        /// 返回可视树中该元素的父元素
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static DependencyObject ParentEx(this DependencyObject item)
        {
            return VisualTreeHelper.GetParent(item);
        }
        #endregion

        #region IEnumerable Extension
        /// <summary>
        /// 枚举中的每个对象执行相同的动作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void ForEachEx<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        /// <summary>
        /// 枚举中的每个对象执行相同的动作
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <typeparam name="S">需要执行动作的类型</typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void ForEachEx<S,T>(this IEnumerable<T> items, Action<S> action)
            where S : class
        {
            foreach (var item in items.OfType<S>())
            {
                action(item);
            }
        }

        #endregion
    }
}
