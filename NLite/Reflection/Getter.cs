using System.Reflection;
using NLite.Internal;
using System;
using NLite.Collections;
using System.Collections.Generic;

namespace NLite.Reflection
{
	
	/// <summary>
	/// 访问器扩展类
	/// </summary>
	public static class GetterExtensions
	{
        private static readonly Dictionary<MemberInfo, Getter> getterCache = new Dictionary<MemberInfo, Getter>();
        /// <summary>
        /// 快速调用访问器方法
        /// </summary>
        /// <param name="member">成员访问器</param>
        /// <param name="target">目标对象</param>
        /// <returns></returns>
        public static object Get(this MemberInfo member, object target)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            Getter getter;
            if (!getterCache.TryGetValue(member, out getter))
                getterCache[member] = getter = member.GetGetter();

            return getter(target);
        }
	}
}
