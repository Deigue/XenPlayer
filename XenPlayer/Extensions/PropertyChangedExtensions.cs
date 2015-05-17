using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

public static class PropertyChangedExtensions
{
    private const string PROPSELECTOR_PROPNAME = "propertySelector";
    private const string SELECTOR_MUSTBEPROP = "PropertySelector must select a property type.";

    public static void Notify<T>(this Action<string> notifier, Expression<Func<T>> propertySelector)
    {
        if (notifier != null)
        {
            notifier(GetPropertyName(propertySelector));
        }
    }

    public static void Notify<T>(this PropertyChangedEventHandler handler, Expression<Func<T>> propertySelector)
    {
        if (handler != null)
        {
            var memberExpression = GetMemberExpression(propertySelector);
            if (memberExpression != null)
            {
                var sender = ((ConstantExpression)memberExpression.Expression).Value;
                handler(sender, new PropertyChangedEventArgs(memberExpression.Member.Name));
            }
        }
        //else we don't raise error for handler == null, because it is null when no has attached to the event..
    }

    public static MemberExpression GetMemberExpression<T>(Expression<Func<T>> propertySelector)
    {
        var memberExpression = propertySelector.Body as MemberExpression;
        if (memberExpression != null)
        {
            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException(PROPSELECTOR_PROPNAME, SELECTOR_MUSTBEPROP);
            }

            return memberExpression;
        }

        // for WPF
        var unaryExpression = propertySelector.Body as UnaryExpression;
        if (unaryExpression != null)
        {
            var innerMemberExpression = unaryExpression.Operand as MemberExpression;
            if (innerMemberExpression != null)
            {
                if (innerMemberExpression.Member.MemberType != MemberTypes.Property)
                {
                    throw new ArgumentException(PROPSELECTOR_PROPNAME, SELECTOR_MUSTBEPROP);
                }

                return innerMemberExpression;
            }
        }

        // all else
        return null;
    }


    public static string GetPropertyName<T>(Expression<Func<T>> propertySelector)
    {
        var memberExpression = propertySelector.Body as MemberExpression;
        if (memberExpression == null)
        {
            var unaryExpression = propertySelector.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
        }

        if (memberExpression != null)
        {
            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException(PROPSELECTOR_PROPNAME, SELECTOR_MUSTBEPROP);
            }

            return memberExpression.Member.Name;
        }

        return null;
    }
}
