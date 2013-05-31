using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;

namespace Unique
{
    public static class TreeExtensions
    {
        /// <summary>
        /// ���ؿ������������Ӵ�Ԫ�ؼ��ϣ�����������
        /// </summary>
        public static IEnumerable<DependencyObject> Descendants(this DependencyObject item)
        {
            foreach (var child in item.ChildrenEx())
            {
                yield return child;

                foreach (var grandChild in child.Descendants())
                {
                    yield return grandChild;
                }
            }
        }

        /// <summary>
        /// ���ؿ������������Ӵ�Ԫ�ؼ��ϣ���������
        /// </summary>
        public static IEnumerable<DependencyObject> DescendantsAndSelf(this DependencyObject item)
        {
            yield return item;

            foreach (var child in item.Descendants())
            {
                yield return child;
            }
        }

        /// <summary>
        /// ���ؿ����������и���Ԫ�ؼ��ϣ�����������
        /// </summary>
        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject item)
        {
            var parent = item.ParentEx();
            while (parent != null)
            {
                yield return parent;
                parent = parent.ParentEx();
            }
        }

        /// <summary>
        /// ���ؿ����������и���Ԫ�ؼ��ϣ���������
        /// </summary>
        public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject item)
        {
            yield return item;

            foreach (var ancestor in item.Ancestors())
            {
                yield return ancestor;
            }
        }

        /// <summary>
        /// ���ؿ���������һ�����е���Ԫ�أ�����������
        /// </summary>
        public static IEnumerable<DependencyObject> Elements(this DependencyObject item)
        {
            return item.ChildrenEx();
        }

        /// <summary>
        /// ���ؿ����������Ԫ��λ��ͬһ�������ĵ�˳��λ�ڸ�Ԫ��ǰ�������Ԫ��
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsBeforeSelf(this DependencyObject item)
        {
            var parent = item.ParentEx();
            if (parent == null)
                yield break;
            foreach (var child in item.Elements().TakeWhile(child => !child.Equals(item)))
            {
                yield return child;
            }
        }

        /// <summary>
        /// ���ؿ����������Ԫ��λ��ͬһ�������ĵ�˳��λ�ڸ�Ԫ�غ��������Ԫ��
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAfterSelf(this DependencyObject item)
        {
            var parent = item.ParentEx();
            if (parent == null)
                yield break;
            var afterSelf = false;
            foreach (var child in parent.Elements())
            {
                if (afterSelf)
                    yield return child;
                if (child.Equals(item))
                    afterSelf = true;
            }
        }

        /// <summary>
        /// ���ؿ���������һ�����е���Ԫ�أ���������
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAndSelf(this DependencyObject item)
        {
            yield return item;

            foreach (var child in item.Elements())
            {
                yield return child;
            }
        }

        /// <summary>
        /// ���ؿ������������Ӵ������ͷ���Ҫ���Ԫ�ؼ��ϣ�����������
        /// </summary>
        public static IEnumerable<T> Descendants<T>(this DependencyObject item)
        {
            return item.Descendants().Where(i => i is T).Cast<T>();
        }

        /// <summary>
        /// ���ؿ����������Ԫ��λ��ͬһ�������ĵ�˳��λ�ڸ�Ԫ��ǰ��ķ�������Ҫ�������Ԫ��
        /// </summary>
        public static IEnumerable<T> ElementsBeforeSelf<T>(this DependencyObject item)
        {
            return item.ElementsBeforeSelf().Where(i => i is T).Cast<T>();
        }

        /// <summary>
        /// ���ؿ����������Ԫ��λ��ͬһ�������ĵ�˳��λ�ڸ�Ԫ�غ���ķ�������Ҫ�������Ԫ��
        /// </summary>
        public static IEnumerable<T> ElementsAfterSelf<T>(this DependencyObject item)
        {
            return item.ElementsAfterSelf().Where(i => i is T).Cast<T>();
        }

        /// <summary>
        /// ���ؿ������������Ӵ������ͷ���Ҫ���Ԫ�ؼ��ϣ���������
        /// </summary>
        public static IEnumerable<T> DescendantsAndSelf<T>(this DependencyObject item)
        {
            return item.DescendantsAndSelf().Where(i => i is T).Cast<T>();
        }

        /// <summary>
        /// ���ؿ����������и��������ͷ���Ҫ���Ԫ�ؼ��ϣ�����������
        /// </summary>
        public static IEnumerable<T> Ancestors<T>(this DependencyObject item)
        {
            return item.Ancestors().Where(i => i is T).Cast<T>();
        }

        /// <summary>
        /// ���ؿ����������и��������ͷ���Ҫ���Ԫ�ؼ��ϣ���������
        /// which match the given type.
        /// </summary>
        public static IEnumerable<T> AncestorsAndSelf<T>(this DependencyObject item)
        {
            return item.AncestorsAndSelf().Where(i => i is T).Cast<T>();
        }

        /// <summary>
        /// ���ؿ���������һ����������Ҫ���������Ԫ�أ�����������
        /// </summary>
        public static IEnumerable<T> Elements<T>(this DependencyObject item)
        {
            return item.Elements().Where(i => i is T).Cast<T>();
        }

        /// <summary>
        /// ���ؿ���������һ����������Ҫ���������Ԫ�أ���������
        /// </summary>
        public static IEnumerable<T> ElementsAndSelf<T>(this DependencyObject item)
        {
            return item.ElementsAndSelf().Where(i => i is T).Cast<T>();
        }

    }

    public static class EnumerableTreeExtensions
    {
        /// <summary>
        /// ��Ԫ�ؼ���Ӧ����ͬ�ĺ����������ظú����Ľ������
        /// </summary>
        private static IEnumerable<DependencyObject> DrillDown(this IEnumerable<DependencyObject> items,
            Func<DependencyObject, IEnumerable<DependencyObject>> function)
        {
            return items.SelectMany(function);
        }

        /// <summary>
        /// ��Ԫ�ؼ���Ӧ����ͬ�ĺ����������ظú��������������Ҫ��ļ���
        /// </summary>
        public static IEnumerable<T> DrillDown<T>(this IEnumerable<DependencyObject> items,
            Func<DependencyObject, IEnumerable<DependencyObject>> function)
            where T : DependencyObject
        {
            return items.SelectMany(item => function(item).OfType<T>());
        }

        /// <summary>
        /// ���ؼ��������е��Ӵ�Ԫ�ؼ��ϣ�����������
        /// </summary>
        public static IEnumerable<DependencyObject> Descendants(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.Descendants());
        }

        /// <summary>
        /// ���ؼ��������е��Ӵ�Ԫ�ؼ��ϣ���������
        /// </summary>
        public static IEnumerable<DependencyObject> DescendantsAndSelf(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.DescendantsAndSelf());
        }

        /// <summary>
        /// ���ؼ��������еĸ���Ԫ�ؼ��ϣ�����������
        /// </summary>
        public static IEnumerable<DependencyObject> Ancestors(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.Ancestors());
        }

        /// <summary>
        /// ���ؼ��������еĸ���Ԫ�ؼ��ϣ���������
        /// </summary>
        public static IEnumerable<DependencyObject> AncestorsAndSelf(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.AncestorsAndSelf());
        }

        /// <summary>
        /// Returns a collection of child elements.
        /// </summary>
        public static IEnumerable<DependencyObject> Elements(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.Elements());
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// </summary>
        public static IEnumerable<DependencyObject> ElementsAndSelf(this IEnumerable<DependencyObject> items)
        {
            return items.DrillDown(i => i.ElementsAndSelf());
        }

        /// <summary>
        /// Returns a collection of descendant elements which match the given type.
        /// </summary>
        public static IEnumerable<T> Descendants<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.Descendants());
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements.
        /// which match the given type.
        /// </summary>
        public static IEnumerable<T> DescendantsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.DescendantsAndSelf());
        }

        /// <summary>
        /// Returns a collection of ancestor elements which match the given type.
        /// </summary>
        public static IEnumerable<T> Ancestors<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.Ancestors());
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements.
        /// which match the given type.
        /// </summary>
        public static IEnumerable<T> AncestorsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.AncestorsAndSelf());
        }

        /// <summary>
        /// Returns a collection of child elements which match the given type.
        /// </summary>
        public static IEnumerable<T> Elements<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.Elements());
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// which match the given type.
        /// </summary>
        public static IEnumerable<T> ElementsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            return items.DrillDown<T>(i => i.ElementsAndSelf());
        }
    }
}